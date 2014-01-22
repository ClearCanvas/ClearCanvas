#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Events;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;
using DeleteDirectoryCommand = ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    public enum DuplicateProcessResultAction
    {
        /// <summary>
        /// Duplicate is identical and has been deleted from the filesystem
        /// </summary>
        Delete,

        /// <summary>
        /// Duplicate is different from the existing copy and a SIQ entry has been created.
        /// </summary>
        Reconcile,

        /// <summary>
        /// Duplicate has overwritten the existing SOP Instance
        /// </summary>
        Accept
    }

    /// <summary>
    /// Processor for 'StudyProcess' <see cref="WorkQueue"/> entries.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery= RecoveryModes.Automatic)]
    public class StudyProcessItemProcessor : BaseItemProcessor, ICancelable
    {
        public class ProcessDuplicateResult
        {
            public DuplicateProcessResultAction ActionTaken { get; set; }
        }

        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;

        protected StudyProcessStatistics Statistics;
    	protected StudyProcessorContext Context;
        #endregion

        #region Public Properties
       
        #endregion

        #region Constructors
        public StudyProcessItemProcessor()
        {
            Statistics = new StudyProcessStatistics();            
        }

        #endregion Constructors

        #region Private Methods

        void SaveDuplicateReport(WorkQueueUid uid, string sourceFile, string destinationFile, DicomFile dupFile, StudyXml studyXml)
        {
            using (var processor = new ServerCommandProcessor("Save duplicate report"))
            {
                processor.AddCommand(new RenameFileCommand(sourceFile, destinationFile, false));

                // Update the StudyStream object
                processor.AddCommand( new InsertStudyXmlCommand(dupFile, studyXml, Context.StorageLocation));

                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                processor.Execute();
            }
        }

        private ProcessDuplicateResult ProcessDuplicateReport(DicomFile dupFile, DicomFile baseFile, WorkQueueUid uid, StudyXml studyXml)
        {
            var result = new ProcessDuplicateResult();

            DateTime? dupTime = DateTimeParser.ParseDateAndTime(dupFile.DataSet, 0, DicomTags.InstanceCreationDate,
                                                                DicomTags.InstanceCreationTime);

            DateTime? baseTime = DateTimeParser.ParseDateAndTime(baseFile.DataSet, 0, DicomTags.InstanceCreationDate,
                                                                 DicomTags.InstanceCreationTime);

            if (dupTime.HasValue && baseTime.HasValue)
            {
                if (dupTime.Value <= baseTime.Value)
                {
                    RemoveWorkQueueUid(uid, dupFile.Filename);
                    result.ActionTaken = DuplicateProcessResultAction.Delete;
                    return result;
                }
            }

            result.ActionTaken = DuplicateProcessResultAction.Accept;
            SaveDuplicateReport(uid, dupFile.Filename, baseFile.Filename, dupFile, studyXml);
            return result;
        }

        private ProcessDuplicateResult OverwriteDuplicate(DicomFile dupFile, WorkQueueUid uid, StudyXml studyXml)
        {
            Platform.Log(LogLevel.Info, "Overwriting duplicate SOP {0}", uid.SopInstanceUid);

            var result = new ProcessDuplicateResult();
            result.ActionTaken = DuplicateProcessResultAction.Accept;

            using (var processor = new ServerCommandProcessor("Overwrite duplicate instance"))
            {
                var destination = Context.StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
                processor.AddCommand(new RenameFileCommand(dupFile.Filename, destination, false));

				// Do so that the FileSize calculation inInsertStudyXmlCommand works
	            dupFile.Filename = destination;

                // Update the StudyStream object
	            var insertStudyXmlCommand = new InsertStudyXmlCommand(dupFile, studyXml, Context.StorageLocation);
                processor.AddCommand(insertStudyXmlCommand);

				// Ideally we don't need to insert the instance into the database since it's a duplicate.
				// However, we need to do so to ensure the Study record is recreated if we are dealing with an orphan study.
				// For other cases, this will cause the instance count in the DB to be out of sync with the filesystem.
				// But it will be corrected at the end of the processing when the study verification is executed.
				processor.AddCommand(new InsertInstanceCommand(dupFile, Context.StorageLocation));

                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                if (!processor.Execute())
                {
					EventManager.FireEvent(this, new FailedUpdateSopEventArgs { File = dupFile, ServerPartitionEntry = Context.StorageLocation.ServerPartition, WorkQueueUidEntry = uid, WorkQueueEntry = WorkQueueItem, FileLength = (ulong)insertStudyXmlCommand.FileSize, FailureMessage = processor.FailureReason});
					
					// cause the item to fail
                    throw new Exception(string.Format("Error occurred when trying to overwrite duplicate in the filesystem."), processor.FailureException);
                }

				EventManager.FireEvent(this, new UpdateSopEventArgs { File = dupFile, ServerPartitionEntry = Context.StorageLocation.ServerPartition, WorkQueueUidEntry = uid, WorkQueueEntry = WorkQueueItem, FileLength = (ulong)insertStudyXmlCommand.FileSize });
            }

            return result;
        }

		private ProcessDuplicateResult OverwriteAndUpdateDuplicate(DicomFile dupFile, WorkQueueUid uid, StudyXml studyXml)
		{
			Platform.Log(LogLevel.Info, "Overwriting duplicate SOP {0}", uid.SopInstanceUid);

			var result = new ProcessDuplicateResult();
			result.ActionTaken = DuplicateProcessResultAction.Accept;

			using (var processor = new ServerCommandProcessor("Overwrite duplicate instance"))
			{
				var destination = Context.StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
				processor.AddCommand(new RenameFileCommand(dupFile.Filename, destination, false));

				// Do so that the FileSize calculation inInsertStudyXmlCommand works
				dupFile.Filename = destination;

				// Update the StudyStream object
				var insertStudyXmlCommand = new InsertStudyXmlCommand(dupFile, studyXml, Context.StorageLocation);
				processor.AddCommand(insertStudyXmlCommand);

				// Ideally we don't need to insert the instance into the database since it's a duplicate.
				// However, we need to do so to ensure the Study record is recreated if we are dealing with an orphan study.
				// For other cases, this will cause the instance count in the DB to be out of sync with the filesystem.
				// But it will be corrected at the end of the processing when the study verification is executed.
				processor.AddCommand(new UpdateInstanceCommand(Context.StorageLocation.ServerPartition,Context.StorageLocation,dupFile));

				processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

				if (!processor.Execute())
				{
					EventManager.FireEvent(this, new FailedUpdateSopEventArgs { File = dupFile, ServerPartitionEntry = Context.StorageLocation.ServerPartition, WorkQueueUidEntry = uid, WorkQueueEntry = WorkQueueItem, FileLength = (ulong)insertStudyXmlCommand.FileSize, FailureMessage = processor.FailureReason });

					// cause the item to fail
					throw new Exception(string.Format("Error occurred when trying to overwrite duplicate in the filesystem."), processor.FailureException);
				}

				EventManager.FireEvent(this, new UpdateSopEventArgs { File = dupFile, ServerPartitionEntry = Context.StorageLocation.ServerPartition, WorkQueueUidEntry = uid, WorkQueueEntry = WorkQueueItem, FileLength = (ulong)insertStudyXmlCommand.FileSize });
			}

			return result;
		}

		private ProcessDuplicateResult ProcessDuplicate(DicomFile dupFile, WorkQueueUid uid, StudyXml studyXml)
		{
			var result = new ProcessDuplicateResult();

			var data = uid.SerializeWorkQueueUidData;

			string duplicateSopPath = ServerHelper.GetDuplicateUidPath(StorageLocation, uid);
			string basePath = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
			if (!File.Exists(basePath))
			{
				// NOTE: This is special case. The file which caused dicom service to think this sop is a duplicate
				// no longer exists in the study folder. Perhaps it has been moved to another folder during auto reconciliation.
				// We have nothing to compare against so let's just throw it into the SIQ queue.
				CreateDuplicateSIQEntry(uid, dupFile, null);
				result.ActionTaken = DuplicateProcessResultAction.Reconcile;
			}
			else
			{
				var duplicateEnum = data.DuplicateProcessing.HasValue ? data.DuplicateProcessing.Value : DuplicateProcessingEnum.Compare;

				// Check if system is configured to override the rule for this study
				if (duplicateEnum == DuplicateProcessingEnum.OverwriteSop)
				{
					return OverwriteDuplicate(dupFile, uid, studyXml);
				}

				// Check if system is configured to override the rule for this study
				if (duplicateEnum == DuplicateProcessingEnum.OverwriteSopAndUpdateDatabase)
				{
					return OverwriteAndUpdateDuplicate(dupFile, uid, studyXml);
				}

				var baseFile = new DicomFile(basePath);
				baseFile.Load();

				if (duplicateEnum == DuplicateProcessingEnum.OverwriteReport)
				{
					return ProcessDuplicateReport(dupFile, baseFile, uid, studyXml);
				}

				// DuplicateProcessingEnum.Compare
				if (!dupFile.TransferSyntax.Equals(baseFile.TransferSyntax))
				{
					// If they're compressed, and we have a codec, lets decompress and still do the comparison
					if (dupFile.TransferSyntax.Encapsulated
					    && !dupFile.TransferSyntax.LossyCompressed
					    && DicomCodecRegistry.GetCodec(dupFile.TransferSyntax) != null)
					{
						dupFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
					}

					if (baseFile.TransferSyntax.Encapsulated
					    && !baseFile.TransferSyntax.LossyCompressed
					    && DicomCodecRegistry.GetCodec(baseFile.TransferSyntax) != null)
					{
						baseFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
					}

					if (dupFile.TransferSyntax.Encapsulated || baseFile.TransferSyntax.Encapsulated)
					{
						string failure = String.Format("Base file transfer syntax is '{0}' while duplicate file has '{1}'",
						                               baseFile.TransferSyntax, dupFile.TransferSyntax);

						var list = new List<DicomAttributeComparisonResult>();
						var compareResult = new DicomAttributeComparisonResult
							{
								ResultType = ComparisonResultType.DifferentValues,
								TagName = DicomTagDictionary.GetDicomTag(DicomTags.TransferSyntaxUid).Name,
								Details = failure
							};
						list.Add(compareResult);
						CreateDuplicateSIQEntry(uid, dupFile, list);
						result.ActionTaken = DuplicateProcessResultAction.Reconcile;
						return result;
					}
				}

				var failureReason = new List<DicomAttributeComparisonResult>();
				if (baseFile.DataSet.Equals(dupFile.DataSet, ref failureReason))
				{
					Platform.Log(LogLevel.Info,
					             "Duplicate SOP being processed is identical.  Removing SOP: {0}",
					             baseFile.MediaStorageSopInstanceUid);


					RemoveWorkQueueUid(uid, duplicateSopPath);
					result.ActionTaken = DuplicateProcessResultAction.Delete;

				}
				else
				{
					CreateDuplicateSIQEntry(uid, dupFile, failureReason);
					result.ActionTaken = DuplicateProcessResultAction.Reconcile;
				}
			}

            return result;
        }

        void CreateDuplicateSIQEntry(WorkQueueUid uid, DicomFile file, List<DicomAttributeComparisonResult> differences)
        {
            Platform.Log(LogLevel.Info, "Duplicate SOP is different from existing copy. Creating duplicate SIQ entry. SOP: {0}", uid.SopInstanceUid);

            using (var processor = new ServerCommandProcessor("Create Duplicate SIQ Entry"))
            {
                var insertCommand = new InsertOrUpdateEntryCommand(
                    uid.GroupID, StorageLocation, file,
                    ServerHelper.GetDuplicateGroupPath(StorageLocation, uid),
                    string.IsNullOrEmpty(uid.RelativePath)
                        ? Path.Combine(StorageLocation.StudyInstanceUid, uid.SopInstanceUid + "." + uid.Extension)
                        : uid.RelativePath,
                    differences);
                processor.AddCommand(insertCommand);

                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                processor.Execute();
            }
            
        }

        /// <summary>
        /// ProcessSavedFile a specific DICOM file related to a <see cref="WorkQueue"/> request.
        /// </summary>
        /// <param name="queueUid"></param>
        /// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
        /// <param name="file">The file being processed.</param>
        /// <param name="compare">Indicates whether to compare the DICOM file against the study in the system.</param>
        protected virtual void ProcessFile(WorkQueueUid queueUid, DicomFile file, StudyXml stream, bool compare)
        {
            var processor = new SopInstanceProcessor(Context) {EnforceNameRules = true};

        	var fileInfo = new FileInfo(file.Filename);
			long fileSize = fileInfo.Length;

			processor.InstanceStats.FileLoadTime.Start();
			processor.InstanceStats.FileLoadTime.End();
			processor.InstanceStats.FileSize = (ulong)fileSize;
			string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:" + fileInfo.Name);
			processor.InstanceStats.Description = sopInstanceUid;

            string group = queueUid.GroupID ?? ServerHelper.GetUidGroup(file, ServerPartition, WorkQueueItem.InsertTime);

            ProcessingResult result = processor.ProcessFile(group, file, stream, compare, true, queueUid, null, SopInstanceProcessorSopType.NewSop);

            if (result.Status == ProcessingStatus.Reconciled)
            {
                // file has been saved by SopInstanceProcessor in another place for reconcilation
                // Note: SopInstanceProcessor has removed the WorkQueueUid so we
                // only need to delete the file here.
                FileUtils.Delete(fileInfo.FullName);
            }
			
			Statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				Statistics.Modality = processor.Modality;

			// Update the statistics
			Statistics.NumInstances++;
        	Statistics.AddSubStats(processor.InstanceStats);
        }
        

        /// <summary>
        /// ProcessSavedFile all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item.</param>
        /// <returns>Number of instances that have been processed successfully.</returns>
        private int ProcessUidList(Model.WorkQueue item)
        {
        	StudyXml studyXml = LoadStudyXml(StorageLocation);

            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                if (sop.Failed)
                    continue;

				if (CancelPending)
				{
					Platform.Log(LogLevel.Info, "Processing of study canceled for shutdown: {0}", StorageLocation.StudyInstanceUid);
                    return successfulProcessCount;	
				}

                if (ProcessWorkQueueUid(item, sop, studyXml))
                    successfulProcessCount++;
            }

            return successfulProcessCount;
        }


        /// <summary>
        /// ProcessSavedFile a specified <see cref="WorkQueueUid"/>
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="sop">The <see cref="WorkQueueUid"/> being processed</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
        /// <returns>true if the <see cref="WorkQueueUid"/> is successfully processed. false otherwise</returns>
        protected virtual bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(sop, "sop");
            Platform.CheckForNullReference(studyXml, "studyXml");

            OnProcessUidBegin(item, sop);

            string path = null;
            
            try
            {
                if (sop.Duplicate && sop.Extension != null)
                {
                    path = ServerHelper.GetDuplicateUidPath(StorageLocation, sop);
                    var file = new DicomFile(path);
                    file.Load();

                    InstancePreProcessingResult result = PreProcessFile(sop, file);

                    if (false ==file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid) 
                            || result.DiscardImage)
                    {
                        RemoveWorkQueueUid(sop, null);
                    }
                    else 
                    {
                    	var duplicateResult = ProcessDuplicate(file, sop, studyXml);
                        if (duplicateResult.ActionTaken == DuplicateProcessResultAction.Delete || duplicateResult.ActionTaken == DuplicateProcessResultAction.Accept)
                        {
                            // make sure the folder is also deleted if it's empty
                            string folder = Path.GetDirectoryName(path);

                            String reconcileRootFolder = ServerHelper.GetDuplicateFolderRootPath(StorageLocation);
                            DirectoryUtility.DeleteIfEmpty(folder, reconcileRootFolder);
                        }
                    }
                }
                else
                {
                    try
                    {
                        path = StorageLocation.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid);
                        var file = new DicomFile(path);
                        file.Load();

                        InstancePreProcessingResult result = PreProcessFile(sop, file);

                        if (false == file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid) 
                            || result.DiscardImage)
                        {
                            RemoveWorkQueueUid(sop, path);
                        }
                        else
                        {
                            ProcessFile(sop, file, studyXml, !result.AutoReconciled);
                        }
                    }
                    catch (DicomException ex)
                    {
                        // bad file. Remove it from the filesystem and the queue
                        RemoveBadDicomFile(path, ex.Message);
                        DeleteWorkQueueUid(sop);
                        return false;
                    }
                    
                }
                
                return true;
            }
            catch (StudyIsNearlineException)
            {
                // handled by caller
                throw;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                item.FailureDescription = e.InnerException != null ? 
					String.Format("{0}:{1}", e.GetType().Name, e.InnerException.Message) : String.Format("{0}:{1}", e.GetType().Name, e.Message);

				//No longer needed.  Update was moved into the SopInstanceProcessor
                //sop.FailureCount++;
                //UpdateWorkQueueUid(sop);
                return false;
                
            }
            finally
            {
                OnProcessUidEnd(item, sop);
            }            
        }
        #endregion


        /// <summary>
        /// Apply changes to the file prior to processing it.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="file"></param>
        protected virtual InstancePreProcessingResult PreProcessFile(WorkQueueUid uid, DicomFile file)
        {
            String contextID = uid.GroupID ?? String.Format("{0}_{1}",
                String.IsNullOrEmpty(file.SourceApplicationEntityTitle) ? ServerPartition.AeTitle : file.SourceApplicationEntityTitle, 
                WorkQueueItem.InsertTime.ToString("yyyyMMddHHmmss"));

            var result = new InstancePreProcessingResult();
            
            var patientNameRules = new PatientNameRules(Study);
            UpdateItem updateItem = patientNameRules.Apply(file);

            result.Modified = updateItem != null;

            var autoBaseReconciler = new AutoReconciler(contextID, StorageLocation);
            InstancePreProcessingResult reconcileResult = autoBaseReconciler.Process(file);
            result.AutoReconciled = reconcileResult != null;
            result.Modified |= reconcileResult != null;
            
            if (reconcileResult!=null && reconcileResult.DiscardImage)
            {
                result.DiscardImage = true;
            }

            // if the studyuid is modified, the file will be deleted by the caller.
            if (file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid))
            {
                if (result.Modified)
                    file.Save();
            }

            
            return result;
        }

        private static void RemoveWorkQueueUid(WorkQueueUid uid, string fileToDelete)
        {
            using (var processor = new ServerCommandProcessor("Remove Work Queue Uid"))
            {
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (String.IsNullOrEmpty(fileToDelete) == false)
                {
                    processor.AddCommand(new FileDeleteCommand(fileToDelete, true));

                }

                if (!processor.Execute())
                {
                    String error = String.Format("Unable to delete Work Queue Uid {0}: {1}", uid.Key, processor.FailureReason);
                    Platform.Log(LogLevel.Error, error);
                    throw new ApplicationException(error, processor.FailureException);
                }
            }

        }

        private void CheckIfStudyIsLossy()
        {
            if (StorageLocation.StudyStatusEnum == StudyStatusEnum.OnlineLossy && StorageLocation.IsLatestArchiveLossless)
            {
                // This should fail the entry and force user to restore the study
                throw new ApplicationException("Unexpected study state: the study is lossy compressed.");
            }
        }


        #region Protected Methods

        /// <summary>
        /// Called before the specified <see cref="WorkQueueUid"/> is processed
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="uid">The <see cref="WorkQueueUid"/> being processed</param>
        protected virtual void OnProcessUidBegin(Model.WorkQueue item, WorkQueueUid uid)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(uid, "uid");

        }

        /// <summary>
        /// Called after the specified <see cref="WorkQueueUid"/> has been processed
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="uid">The <see cref="WorkQueueUid"/> being processed</param>
        protected virtual void OnProcessUidEnd(Model.WorkQueue item, WorkQueueUid uid)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(uid, "uid");

            if (uid.Duplicate)
            {
                String dupPath = ServerHelper.GetDuplicateUidPath(StorageLocation, uid);
                // Delete the container if it's empty
                var f = new FileInfo(dupPath);

                if (f.Directory!=null && DirectoryUtility.DeleteIfEmpty(f.Directory.FullName))
                {
                    DirectoryUtility.DeleteIfEmpty(ServerHelper.GetDuplicateGroupPath(StorageLocation, uid));
                }
            }
        }

        #endregion

        #region Overridden Protected Method

    	protected override void OnProcessItemEnd(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            base.OnProcessItemEnd(item);

            Statistics.UidsLoadTime.Add(UidsLoadTime);
            Statistics.StorageLocationLoadTime.Add(StorageLocationLoadTime);
            Statistics.StudyXmlLoadTime.Add(StudyXmlLoadTime);
            Statistics.DBUpdateTime.Add(DBUpdateTime);

            if (Statistics.NumInstances > 0)
            {
                Statistics.CalculateAverage();
                StatisticsLogger.Log(LogLevel.Info, false, Statistics);
            }

        }

        protected override  void OnProcessItemBegin(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            Statistics = new StudyProcessStatistics
                          	{
                          		Description = String.Format("{0}[Key={1}]", item.WorkQueueTypeEnum, item.Key.Key)
                          	};
        }
        
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");

            // Verify the study is not lossy online and lossless in the archive.
            // This could happen if the images were received WHILE the study was being lossy compressed.
            // The study state would not be set until the compression was completed or partially completed.
            CheckIfStudyIsLossy();


            Statistics.TotalProcessTime.Start();
            bool successful;
        	bool idle = false;
            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            int totalUidCount = WorkQueueUidList.Count;

            if (totalUidCount == 0)
            {
                successful = true;
                idle = true;
            }
            else
            {
                try
                {
                    Context = new StudyProcessorContext(StorageLocation, WorkQueueItem);

                    // Load the rules engine
                    _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, item.ServerPartitionKey);
                    _sopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
                    _sopProcessedRulesEngine.Load();
                    Statistics.SopProcessedEngineLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);
                    Context.SopProcessedRulesEngine = _sopProcessedRulesEngine;
                    
                    if (Study != null)
                    {
                        Platform.Log(LogLevel.Info, "Processing study {0} for Patient {1} (PatientId:{2} A#:{3}), {4} objects",
                                     Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                                     Study.AccessionNumber, WorkQueueUidList.Count);
                    }
                    else
                    {
                        Platform.Log(LogLevel.Info, "Processing study {0}, {1} objects",
                                     StorageLocation.StudyInstanceUid, WorkQueueUidList.Count);
                    }

                    // ProcessSavedFile the images in the list
                    successful = ProcessUidList(item) > 0;
                }
                catch (StudyIsNearlineException ex)
                {
                    // delay until the target is restored
                    // NOTE: If the study could not be restored after certain period of time, this entry will be failed.
                    if (ex.RestoreRequested)
                    {
                        PostponeItem(string.Format("Unable to auto-reconcile at this time: the target study {0} is not online yet. Restore has been requested.", ex.StudyInstanceUid));
                        return;
                    }
                	// fail right away
                	FailQueueItem(item, string.Format("Unable to auto-reconcile at this time: the target study {0} is not nearline and could not be restored.", ex.StudyInstanceUid));
                	return;
                }
            }
            Statistics.TotalProcessTime.End();

			if (successful)
			{
				if (idle && item.ExpirationTime <= Platform.Time)
				{
					// Run Study / Series Rules Engine.
					var engine = new StudyRulesEngine(StorageLocation, ServerPartition);
					engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);

					// Log the FilesystemQueue related entries
					StorageLocation.LogFilesystemQueue();

					// Delete the queue entry.
					PostProcessing(item,
					               WorkQueueProcessorStatus.Complete,
					               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}
				else if (idle)
					PostProcessing(item,
								   WorkQueueProcessorStatus.IdleNoDelete, // Don't delete, so we ensure the rules engine is run later.
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				else
					PostProcessing(item,
								   WorkQueueProcessorStatus.Pending,
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			else
			{
				bool allFailedDuplicate = CollectionUtils.TrueForAll(WorkQueueUidList, uid => uid.Duplicate && uid.Failed);

				if (allFailedDuplicate)
				{
					Platform.Log(LogLevel.Error, "All entries are duplicates");

					PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
					return;
				}
				PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
			}				
        }


        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
				string failureReason;
				if (!ServerHelper.LockStudy(WorkQueueItem.StudyStorageKey, QueueStudyStateEnum.ProcessingScheduled, out failureReason))
				{
					Platform.Log(LogLevel.Debug,
								 "StudyProcess cannot start at this point. Study is being locked by another processor. WriteLock Failure reason={0}",
								 failureReason);
					PostponeItem(String.Format("Study is being locked by another processor: {0}", failureReason));
                    return false;
                }
            }

            return true;
        }
        #endregion        
    }
}
