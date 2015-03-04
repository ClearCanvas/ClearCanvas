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
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReprocessStudy
{
    /// <summary>
    /// Information about the study being reprocessed
    /// </summary>
    class StudyInfo
    {
        #region Private Members

        private readonly Dictionary<string, SeriesInfo> _series = new Dictionary<string, SeriesInfo>();
        #endregion

        #region Constructors
        
        public StudyInfo(string studyInstanceUid)
        {
            StudyInstanceUid = studyInstanceUid;
        }

        #endregion

        #region Public Properties

        public string StudyInstanceUid { get; set; }

        public SeriesInfo this[string seriesUid]
        {
            get
            {
                if (!_series.ContainsKey(seriesUid))
                {
                    return null;
                }

                return _series[seriesUid];
            }
            set
            {
                if (_series.ContainsKey(seriesUid))
                    _series[seriesUid] = value;
                else
                    _series.Add(seriesUid, value);
            }
        }

        public IEnumerable<SeriesInfo> Series
        {
            get { return _series.Values; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a series with specified series instance uid into the study
        /// </summary>
        /// <param name="seriesUid"></param>
        /// <returns></returns>
        public SeriesInfo AddSeries(string seriesUid)
        {
            var series = new SeriesInfo(seriesUid);
            _series.Add(seriesUid, series);

            return series;
        }

        #endregion
    }

    /// <summary>
    /// Information about the series of the study being reprocessed
    /// </summary>
    class SeriesInfo
	{
		#region Private Members

        private readonly Dictionary<string, SopInfo> _sopInstances = new Dictionary<string, SopInfo>();
		#endregion

		#region Constructors
		public SeriesInfo(string seriesUid)
        {
            SeriesUid = seriesUid;
		}
		#endregion

		#region Public Properties

        public string SeriesUid { get; set; }

        public SopInfo this[string sopUid]
        {
            get
            {
            	if (_sopInstances.ContainsKey(sopUid))
                    return _sopInstances[sopUid];
            	return null;
            }
        	set
            {
                if (_sopInstances.ContainsKey(sopUid))
                    _sopInstances[sopUid] = value;
                else
                    _sopInstances.Add(sopUid, value);
            }
        }

        public IEnumerable<SopInfo> Instances
        {
            get { return _sopInstances.Values; }
		}
		#endregion

		#region Public Methods
		public SopInfo AddInstance(string instanceUid)
        {
            var sop = new SopInfo(instanceUid);
            _sopInstances.Add(instanceUid, sop);

            return sop;
		}
		#endregion
	}

    /// <summary>
    /// Information about the sop instance being reprocessed.
    /// </summary>
    class SopInfo
    {
        #region Private Members

        #endregion

        #region Constructors
        public SopInfo(string instanceUid)
        {
            SopInstanceUid = instanceUid;
        }
        #endregion

        #region Public Properties

        public string SopInstanceUid { get; set; }

        #endregion
    }



    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Manual)]
    public class ReprocessStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members

        private bool _completed;
        private ReprocessStudyQueueData _queueData;
        private List<string> _additionalFilesToProcess;
        #endregion

        #region Overrriden Protected Methods

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.StudyStorageKey, "item.StudyStorageKey");

            bool successful = true;
            string failureDescription = null;
            
            // The processor stores its state in the Data column
            ReadQueueData(item);


            if (_queueData.State == null || !_queueData.State.ExecuteAtLeastOnce)
            {
                // Added for ticket #9673:
                // If the study folder does not exist and the study has been archived, trigger a restore and we're done
                if (!Directory.Exists(StorageLocation.GetStudyPath()))
                {
                    if (StorageLocation.ArchiveLocations.Count > 0)
                    {
                        Platform.Log(LogLevel.Info,
                                     "Reprocessing archived study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4} without study data on the filesystem.  Inserting Restore Request.",
                                     Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                                     Study.AccessionNumber, ServerPartition.Description);

                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);

                        // Post process had to be done first so the study is unlocked so the RestoreRequest can be inserted.
                        ServerHelper.InsertRestoreRequest(StorageLocation);

                        RaiseAlert(WorkQueueItem, AlertLevel.Warning,
                                   string.Format(
                                       "Found study {0} for Patient {1} (A#:{2})on Partition {3} without storage folder, restoring study.",
                                       Study.StudyInstanceUid, Study.PatientsName, Study.AccessionNumber, ServerPartition.Description));                        
                        return;
                    }
                }

				if (Study == null)
					Platform.Log(LogLevel.Info,
					             "Reprocessing study {0} on Partition {1}", StorageLocation.StudyInstanceUid,
					             ServerPartition.Description);
				else
					Platform.Log(LogLevel.Info,
					             "Reprocessing study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
					             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
					             Study.AccessionNumber, ServerPartition.Description);

                CleanupDatabase();
            }
            else
            {
                if (_queueData.State.Completed)
                {
                    #region SAFE-GUARD CODE: PREVENT INFINITE LOOP

                    // The processor indicated it had completed reprocessing in previous run. The entry should have been removed and this block of code should never be called.
                    // However, we have seen ReprocessStudy entries that mysterously contain rows in the WorkQueueUid table.
                    // The rows prevent the entry from being removed from the database and the ReprocessStudy keeps repeating itself.

                    
                    // update the state first, increment the CompleteAttemptCount
                    _queueData.State.ExecuteAtLeastOnce = true;
                    _queueData.State.Completed = true;
                    _queueData.State.CompleteAttemptCount++;
                    SaveState(item, _queueData);

                    if (_queueData.State.CompleteAttemptCount < 10)
                    {
                        // maybe there was db error in previous attempt to remove the entry. Let's try again.
                        Platform.Log(LogLevel.Info, "Resuming Reprocessing study {0} but it was already completed!!!", StorageLocation.StudyInstanceUid);
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    }
                    else
                    {
                        // we are definitely stuck.
                        Platform.Log(LogLevel.Error, "ReprocessStudy {0} for study {1} appears stuck. Aborting it.", item.Key, StorageLocation.StudyInstanceUid);
                        item.FailureDescription = "This entry had completed but could not be removed.";
                        PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    }

                    return;

                    #endregion
                }

                if (Study == null)
					Platform.Log(LogLevel.Info,
								 "Resuming Reprocessing study {0} on Partition {1}", StorageLocation.StudyInstanceUid,
								 ServerPartition.Description);
				else
					Platform.Log(LogLevel.Info,
                             "Resuming Reprocessing study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);
                
            }

			// As per #12583, Creation of the SopInstanceProcessor should occur after the CleanupDatabase() call.
			var context = new StudyProcessorContext(StorageLocation, WorkQueueItem);

			// TODO: Should we enforce the patient's name rule?
			// If we do, the Study record will have the new patient's name 
			// but how should we handle the name in the Patient record?
			const bool enforceNameRules = false;
			var processor = new SopInstanceProcessor(context) { EnforceNameRules = enforceNameRules };

			var seriesMap = new Dictionary<string, List<string>>();

            StudyXml studyXml = LoadStudyXml();

            var reprocessedCounter = 0;
        	var skippedCount = 0;
            var removedFiles = new List<FileInfo>();
            try
            {
                // Traverse the directories, process 500 files at a time
                var isCancelled = FileProcessor.Process(StorageLocation.GetStudyPath(), "*.*",
                                      delegate(string path, out bool cancel)
                                          {
                                              #region Reprocess File

                                              var file = new FileInfo(path);
                                              
                                              // ignore all files except those ending ".dcm"
                                              // ignore "bad(0).dcm" files too
                                              if (Regex.IsMatch(file.Name.ToUpper(), "[0-9]+\\.DCM$"))
                                              {
                                                  try
                                                  {
                                                      var dicomFile = new DicomFile(path);
                                                      dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
                                                      
                                                      string seriesUid = dicomFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
                                                      string instanceUid =dicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0,string.Empty);
                                                      if (studyXml.Contains(seriesUid, instanceUid))
                                                      {
                                                          if (!seriesMap.ContainsKey(seriesUid))
                                                          {
                                                              seriesMap.Add(seriesUid, new List<string>());
                                                          }
                                                          if (!seriesMap[seriesUid].Contains(instanceUid))
                                                              seriesMap[seriesUid].Add(instanceUid);
                                                          else
                                                          {
                                                              Platform.Log(LogLevel.Warn, "SOP Instance UID in {0} appears more than once in the study.", path);
                                                          }

                                                      	skippedCount++;
                                                      }
                                                      else
                                                      {
                                                          Platform.Log(ServerPlatform.InstanceLogLevel, "Reprocessing SOP {0} for study {1}",instanceUid, StorageLocation.StudyInstanceUid);
                                                          string groupId = ServerHelper.GetUidGroup(dicomFile, StorageLocation.ServerPartition, WorkQueueItem.InsertTime);
                                                          ProcessingResult result = processor.ProcessFile(groupId, dicomFile, studyXml, true, false, null, null, SopInstanceProcessorSopType.ReprocessedSop);
                                                          switch (result.Status)
                                                          {
                                                              case ProcessingStatus.Success:
                                                                  reprocessedCounter++;
                                                                  if (!seriesMap.ContainsKey(seriesUid))
                                                                  {
                                                                      seriesMap.Add(seriesUid, new List<string>());
                                                                  }

                                                                  if (!seriesMap[seriesUid].Contains(instanceUid))
                                                                      seriesMap[seriesUid].Add(instanceUid);
                                                                  else
                                                                  {
                                                                      Platform.Log(LogLevel.Warn, "SOP Instance UID in {0} appears more than once in the study.", path);
                                                                  }
                                                                  break;

                                                              case ProcessingStatus.Reconciled:
                                                                  Platform.Log(LogLevel.Warn, "SOP was unexpectedly reconciled on reprocess SOP {0} for study {1}. It will be removed from the folder.", instanceUid, StorageLocation.StudyInstanceUid);
                                                                  failureDescription = String.Format("SOP Was reconciled: {0}", instanceUid);

                                                                  // Added for #10620 (Previously we didn't do anything here)
                                                                  // Because we are reprocessing files in the study folder, when file needs to be reconciled it is copied to the reconcile folder
                                                                  // Therefore, we need to delete the one in the study folder. Otherwise, there will be problem when the SIQ entry is reconciled.
                                                                  // InstanceAlreadyExistsException will also be thrown by the SOpInstanceProcessor if this ReprocessStudy WQI 
                                                                  // resumes and reprocesses the same file again.
                                                                  // Note: we are sure that the file has been copied to the Reconcile folder and there's no way back. 
                                                                  // We must get rid of this file in the study folder.
                                                                  FileUtils.Delete(path);

                                                                  // Special handling: if the file is one which we're supposed to reprocess at the end (see ProcessAdditionalFiles), we must remove the file from the list
                                                                  if (_additionalFilesToProcess != null && _additionalFilesToProcess.Contains(path))
                                                                  {
                                                                      _additionalFilesToProcess.Remove(path);
                                                                  }

                                                                  break;
                                                          }
                                                      }
                                                  }
                                                  catch (DicomException ex)
                                                  {
                                                      // TODO : should we fail the reprocess instead? Deleting an dicom file can lead to incomplete study.
                                                      removedFiles.Add(file);
                                                      Platform.Log(LogLevel.Warn, "Skip reprocessing and delete {0}: Not readable.", path);
                                                      FileUtils.Delete(path);
                                                      failureDescription = ex.Message;
                                                  }
                                              }
                                              else if (!file.Extension.Equals(".xml") && !file.Extension.Equals(".gz"))
                                              {
                                                  // not a ".dcm" or header file, delete it
                                                  removedFiles.Add(file); 
                                                  FileUtils.Delete(path);
                                              }

                                              #endregion

											  if (reprocessedCounter>0 && reprocessedCounter % 200 == 0)
											  {
												  Platform.Log(LogLevel.Info, "Reprocessed {0} files for study {1}", reprocessedCounter + skippedCount, StorageLocation.StudyInstanceUid);
											  }

                                              cancel = reprocessedCounter >= 5000;

											  
                                          }, true);

                if (studyXml != null)
                {
                    EnsureConsistentObjectCount(studyXml, seriesMap);
                    SaveStudyXml(studyXml);
                }

                // Completed if either all files have been reprocessed 
                // or no more dicom files left that can be reprocessed.
				_completed = reprocessedCounter == 0 || !isCancelled;
            }
            catch (Exception e)
            {
                successful = false;
                failureDescription = e.Message;
                Platform.Log(LogLevel.Error, e, "Unexpected exception when reprocessing study: {0}", StorageLocation.StudyInstanceUid);
                Platform.Log(LogLevel.Error, "Study may be in invalid unprocessed state.  Study location: {0}", StorageLocation.GetStudyPath());
                throw;
            }
            finally
            {
                LogRemovedFiles(removedFiles);

                // Update the state
                _queueData.State.ExecuteAtLeastOnce = true;
                _queueData.State.Completed = _completed;
                _queueData.State.CompleteAttemptCount++;
                SaveState(item, _queueData);
                    
                if (!successful)
                {
                    FailQueueItem(item, failureDescription);
                }
                else 
                {
                    if (!_completed)
                    {
                        // Put it back to Pending
                        PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                    }
                    else
                    {
						// Reload the record from the database because referenced entities have been modified since the beginning.
						// Need to reload because we are passing the location to the rule engine.
						StorageLocation = CollectionUtils.FirstElement<StudyStorageLocation>(StudyStorageLocation.FindStorageLocations(item.ServerPartitionKey, StorageLocation.StudyInstanceUid), null);

                        LogHistory();

                        // Run Study / Series Rules Engine.
                        var engine = new StudyRulesEngine(StorageLocation, ServerPartition);
                        engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);

                        // Log the FilesystemQueue related entries
                        StorageLocation.LogFilesystemQueue();

                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);

                        Platform.Log(LogLevel.Info, "Completed reprocessing of study {0} on partition {1}", StorageLocation.StudyInstanceUid, ServerPartition.Description);
                    }                
                }
            }
        }

        protected override void PostProcessing(Model.WorkQueue item, WorkQueueProcessorStatus status, WorkQueueProcessorDatabaseUpdate resetQueueStudyState)
        {
            ProcessAdditionalFiles();
            base.PostProcessing(item, status, resetQueueStudyState);

        }

        /// <summary>
        /// Reprocess the file requested by the process which initiates the reprocess. 
        /// If the file is located outside the study folder, it will be moved to the incoming folder for import.
        /// </summary>
        private void ProcessAdditionalFiles()
        {
            if (_additionalFilesToProcess != null && _additionalFilesToProcess.Count > 0)
            {
                if (string.IsNullOrEmpty(base.GetServerPartitionIncomingFolder()))
                {
                    var error = "Some files need to be moved to the Incoming folder in order to be reprocess. However, there is no active incoming folder. Make sure the Import Files Service is enabled";
                    throw new Exception(error);
                }
                else
                {
                    try
                    {

                        foreach (var entry in _additionalFilesToProcess)
                        {
                            var path = FilesystemDynamicPath.Parse(entry);
                            var realPath = path.ConvertToAbsolutePath(StorageLocation);

                            try
                            {
                                // On one hand, if the file is in the study folder then we should have processed it in prev stage.
                                // But on the other hand, the original WQI may have failed because the file was missing. If this is the case, we should fail
                                var fileInfo = new FileInfo(realPath);
                                if (fileInfo.FullName.IndexOf(StorageLocation.GetStudyPath()) == 0)
                                {
                                    // only check if file exists if it's a DCM file. Other type of files (eg xml, gz) may have been deleted during reprocessing
                                    if (fileInfo.Extension.Equals(ServerPlatform.DicomFileExtension, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (!fileInfo.Exists)
                                            throw new FileNotFoundException(string.Format("{0} is expected but it could not be found", fileInfo.FullName));

                                        Platform.Log(LogLevel.Debug, "Skip reprocessing {0} since it is in the study folder", realPath);
                                    }                                                                        
                                }
                                else
                                {
                                    MoveFileToIncomingFolder(fileInfo.FullName);

                                    // delete empty directories if necessary
                                    if (path.Type == FilesystemDynamicPath.PathType.RelativeToReconcileFolder)
                                    {
                                        var folderToDelete = fileInfo.Directory.FullName;

                                        DirectoryUtility.DeleteIfEmpty(folderToDelete, StorageLocation.GetReconcileRootPath());
                                    }
                                }
                                
                                _queueData.AdditionalFiles.Remove(entry);

                            }
                            catch (FileNotFoundException)
                            {
                                throw; // File is missing. Better leave decision making to the user.
                            }
                            catch (DirectoryNotFoundException)
                            {
                                // ignore?
                            }
                        }

                    }
                    finally
                    {
                        // update the list so we don't have to reprocess again when the WQI resumes (that will cause problem because we may have moved the files somewhere else)
                        SaveState(WorkQueueItem, _queueData);
                    }                
                }                
            }
        }

        /// <summary>
        /// Move the file specified in the path to the incoming folder so that it will be imported again
        /// </summary>
        /// <param name="path"></param>
        private void MoveFileToIncomingFolder(string path)
        {
            Platform.Log(LogLevel.Debug, "Moving file {0} to incoming folder", path);

            // should not proceed because it may mean incomplete study
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("File is missing: {0}", path));
            
            // move the file to the Incoming folder to reprocess            
            using (var processor = new ServerCommandProcessor("Move file back to incoming folder"))
            {
                var fileInfo = new FileInfo(path);
                var incomingPath = GetServerPartitionIncomingFolder();
                incomingPath = Path.Combine(incomingPath, "FromWorkQueue");
                incomingPath = Path.Combine(incomingPath, StorageLocation.StudyInstanceUid);

                var createDirCommand = new CreateDirectoryCommand(incomingPath);
                processor.AddCommand(createDirCommand);

                incomingPath = Path.Combine(incomingPath, fileInfo.Name);
                var move = new RenameFileCommand(path, incomingPath, true);
                processor.AddCommand(move);

                if (!processor.Execute())
                {
                    throw new Exception("Unexpected error happened when trying to move file back to the incoming folder for reprocess", processor.FailureException);
                }

                Platform.Log(LogLevel.Info, "File {0} has been moved to the incoming folder.", path);

            }
        }

        private void LogRemovedFiles(List<FileInfo> removedFiles)
        {
            if (removedFiles.Count>0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("The following files have been deleted because they are not readable or have bad extensions:");
                for(int i=0; i< removedFiles.Count; i++)
                {
                    if (i == 0)
                        sb.AppendFormat("{0}", removedFiles[i].Name);
                    else
                        sb.AppendFormat(", {0}", removedFiles[i].Name);
                }
                Platform.Log(LogLevel.Warn, sb.ToString());
                RaiseAlert(WorkQueueItem, AlertLevel.Warning, sb.ToString());
            }
        }

        private void SaveState(Model.WorkQueue item, ReprocessStudyQueueData queueData)
        {
            // Update the queue state
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                queueData.State.ExecuteAtLeastOnce = true;
                var broker = updateContext.GetBroker<IWorkQueueEntityBroker>();
                var parms = new WorkQueueUpdateColumns {Data = XmlUtils.SerializeAsXmlDoc(_queueData)};
                broker.Update(item.GetKey(), parms);
                updateContext.Commit();
            }
        }


        private void CleanupDatabase()
        {
            // Delete StudyStorage related tables except the StudyStorage table itself
            // This will reset the object count in all levels. The Study, Patient, Series
            // records will be recreated when the file is reprocessed.
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                var broker = updateContext.GetBroker<IResetStudyStorage>();
                var criteria = new ResetStudyStorageParameters {StudyStorageKey = StorageLocation.GetKey()};
                if (!broker.Execute(criteria))
                {
                    throw new ApplicationException("Could not reset study storage");
                }
            	updateContext.Commit();
            }

			// the record has been deleted. Reset to Null to prevent it from being used accidentally.
            _theStudy = null;
        }

        private void LogHistory()
        {
            Platform.Log(LogLevel.Debug, "Logging history record...");
            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // create the change log based on info stored in the queue entry
                var changeLog = new ReprocessStudyChangeLog
                                    {
                                        TimeStamp = Platform.Time,
                                        StudyInstanceUid = StorageLocation.StudyInstanceUid,
                                        Reason = _queueData.ChangeLog != null ? _queueData.ChangeLog.Reason : "N/A",
                                        User = _queueData.ChangeLog != null ? _queueData.ChangeLog.User : "Unknown"
                                    };

                StudyHistory history = StudyHistoryHelper.CreateStudyHistoryRecord(ctx, StorageLocation, null, StudyHistoryTypeEnum.Reprocessed, null, changeLog);
                if (history != null)
                    ctx.Commit();
            }
        }

        private void SaveStudyXml(StudyXml studyXml)
        {
            var theMemento = studyXml.GetMemento(new StudyXmlOutputSettings());
            using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(StorageLocation.GetStudyXmlPath(), FileMode.Create),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(StorageLocation.GetCompressedStudyXmlPath(), FileMode.Create))
            {
                StudyXmlIo.WriteXmlAndGzip(theMemento, xmlStream, gzipStream);
                xmlStream.Close();
                gzipStream.Close();
            }
        }

        private StudyXml LoadStudyXml()
        {
            StudyXml studyXml;
            if (!_queueData.State.ExecuteAtLeastOnce)
            {
                FileUtils.Delete(StorageLocation.GetCompressedStudyXmlPath());
                FileUtils.Delete(StorageLocation.GetStudyXmlPath());
                studyXml = new StudyXml(StorageLocation.StudyInstanceUid);

            }
            else
            {
                // reuse the current xml and add more sop into it as we find
                studyXml = StorageLocation.LoadStudyXml();
                   
            }

            // If for some reason, the xml does not exist, recreate it
            return studyXml ?? new StudyXml(StorageLocation.StudyInstanceUid);
        }

        private void ReadQueueData(Model.WorkQueue item)
        {
            if (item.Data!=null)
            {
                _queueData = XmlUtils.Deserialize<ReprocessStudyQueueData>(item.Data);
            }

            if (_queueData == null)
            {
                _queueData = new ReprocessStudyQueueData
                                 {
                                     State = new ReprocessStudyState
                                                 {
                                                     ExecuteAtLeastOnce = false
                                                 }
                                 };
            }

            // Convert the paths stored in _queueData.AdditionalFiles to the actual paths
            if (_queueData.AdditionalFiles != null)
            {
                _additionalFilesToProcess = new List<string>();
                var list = _queueData.AdditionalFiles.ToArray();
                foreach (var entry in list)
                {
                    var path = FilesystemDynamicPath.Parse(entry);
                    var realPath = path.ConvertToAbsolutePath(StorageLocation);
                    _additionalFilesToProcess.Add(realPath);
                }
            }
                     
        }

        private void EnsureConsistentObjectCount(StudyXml studyXml, IDictionary<string, List<string>> processedSeriesMap)
        {
            Platform.CheckForNullReference(studyXml, "studyXml");

            // We have to ensure that the counts in studyXml and what we have processed are consistent.
            // Files or folder may be reprocessed but then become missing when then entry is resumed.
            // We have to removed them from the studyXml before committing the it.

            Platform.Log(LogLevel.Info, "Verifying study xml against the filesystems");
            int filesProcessed = 0;
            foreach (string seriesUid in processedSeriesMap.Keys)
            {
                filesProcessed += processedSeriesMap[seriesUid].Count;
            }

            // Used to keep track of the series to be removed.
            // We can't remove the item from the study xml while we are 
            // interating through it
            var seriesToRemove = new List<string>();
            foreach(SeriesXml seriesXml in studyXml)
            {
                if (!processedSeriesMap.ContainsKey(seriesXml.SeriesInstanceUid))
                {
                    seriesToRemove.Add(seriesXml.SeriesInstanceUid);
                }
                else
                {
                    //check all instance in the series
                    List<string> foundInstances = processedSeriesMap[seriesXml.SeriesInstanceUid];
                    var instanceToRemove = new List<string>();
                    foreach (InstanceXml instanceXml in seriesXml)
                    {
                        if (!foundInstances.Contains(instanceXml.SopInstanceUid))
                        {
                            // the sop no long exists in the filesystem
                            instanceToRemove.Add(instanceXml.SopInstanceUid);
                        }
                    }

                    foreach(string instanceUid in instanceToRemove)
                    {
                        seriesXml[instanceUid] = null;
                        Platform.Log(LogLevel.Info, "Removed SOP {0} in the study xml: it no longer exists.", instanceUid);
                    }
                }
            }

            foreach(string seriesUid in seriesToRemove)
            {
                studyXml[seriesUid] = null;
                Platform.Log(LogLevel.Info, "Removed Series {0} in the study xml: it no longer exists.", seriesUid);
                    
            }

            Platform.CheckTrue(studyXml.NumberOfStudyRelatedSeries == processedSeriesMap.Count,
               String.Format("Number of series in the xml do not match number of series reprocessed: {0} vs {1}",
               studyXml.NumberOfStudyRelatedInstances, processedSeriesMap.Count));
            
            Platform.CheckTrue(studyXml.NumberOfStudyRelatedInstances == filesProcessed, 
                String.Format("Number of instances in the xml do not match number of reprocessed: {0} vs {1}",
                studyXml.NumberOfStudyRelatedInstances, filesProcessed));

            Platform.Log(LogLevel.Info, "Study xml has been verified.");

            if (StorageLocation.Study != null)
            {
                // update the instance count in the db
                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    var broker = updateContext.GetBroker<IStudyEntityBroker>();
                    var columns = new StudyUpdateColumns
                                      {
                                          NumberOfStudyRelatedInstances = studyXml.NumberOfStudyRelatedInstances,
                                          NumberOfStudyRelatedSeries = studyXml.NumberOfStudyRelatedSeries
                                      };
                    broker.Update(StorageLocation.Study.GetKey(), columns);
                    updateContext.Commit();
                }
            }
            else
            {
                // alert orphaned StudyStorage entry
                RaiseAlert(WorkQueueItem, AlertLevel.Critical,
                           String.Format("Study {0} has been reprocessed but Study record was NOT created. Images reprocessed: {1}. Path={2}",
                           StorageLocation.StudyInstanceUid, filesProcessed,
                           StorageLocation.GetStudyPath()));
            }
            
        }

        #endregion

        protected override bool CanStart()
        {
            return true;// can start anytime
        }

    }
}
