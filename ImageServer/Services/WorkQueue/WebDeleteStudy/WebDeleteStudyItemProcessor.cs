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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate;
using DeleteDirectoryCommand = ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    public class WebDeleteStudyItemProcessor : DeleteStudyItemProcessor
    {
        private DeletionLevel _level;
        private string _reason;
        private string _userId;
        private string _userName;
        private IList<IWebDeleteProcessorExtension> _extensions;
        private List<Series> _seriesToDelete;

        public DeletionLevel Level
        {
            get { return _level; }
        }


        #region Overridden Protected Methods

        /// <summary>
        /// Overrides the base method to return the validation mode based on the deletion level
        /// </summary>
        /// <returns></returns>
        protected override StudyIntegrityValidationModes GetValidationMode()
        {
            switch(_level)
            {
                case DeletionLevel.Study:
                	// There's no need to verify the study if it will be deleted.
                    return StudyIntegrityValidationModes.None; 

                case DeletionLevel.Series:
                    return StudyIntegrityValidationModes.Default;

                case DeletionLevel.Instance:
                    return StudyIntegrityValidationModes.Default;
            }

            return StudyIntegrityValidationModes.Default;
        }

        protected override void OnProcessItemBegin(Model.WorkQueue item)
        {
            base.OnProcessItemBegin(item);

            WebDeleteWorkQueueEntryData data = ParseQueueData(item);
            _level = data.Level;
            _reason = data.Reason;
            _userId = data.UserId;
            _userName = data.UserName;
        }

        protected override DeleteStudyContext CreatePluginProcessingContext()
        {
            DeleteStudyContext context = base.CreatePluginProcessingContext();
            context.UserId = _userId;
            context.UserName = _userName;
            return context;
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.Log(LogLevel.Info, "Begin {0} ({1} level) GUID={2}", item.WorkQueueTypeEnum, Level, item.Key);
            
            switch (Level)
            {
                case DeletionLevel.Series: ProcessSeriesLevelDelete(item);
                    break;
                case DeletionLevel.Study: ProcessStudyLevelDelete(item);
                    break;
                case DeletionLevel.Instance: ProcessInstanceLevelDelete(item);
                    break;

                default:
                    throw new NotImplementedException();
            }

        }

		protected override bool CanStart()
		{
			string failureReason;
			if (!StorageLocation.QueueStudyStateEnum.Equals(QueueStudyStateEnum.WebDeleteScheduled)
			    &&
			    !ServerHelper.LockStudy(WorkQueueItem.StudyStorageKey, QueueStudyStateEnum.WebDeleteScheduled, out failureReason))
			{
				Platform.Log(LogLevel.Debug,
				             "ProcessDuplicate cannot start at this point. Study is being locked by another processor. WriteLock Failure reason={0}",
				             failureReason);

				PostponeItem(string.Format("Study is being locked by another processor: {0}", failureReason));
				return false;
			}

			return base.CanStart();
		}

    	#endregion

        #region Private Methods

        private static WebDeleteWorkQueueEntryData ParseQueueData(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item.Data, "item.Data");

            var data = XmlUtils.Deserialize<WebDeleteWorkQueueEntryData>(item.Data);
            return data;
        }


        private void ProcessSeriesLevelDelete(Model.WorkQueue item)
        {
            // ensure the Study is loaded.
            Study study = StorageLocation.Study;
            Platform.CheckForNullReference(study, "Study record doesn't exist");

            Platform.Log(LogLevel.Info, "Processing Series Level Deletion for Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber);

            _seriesToDelete = new List<Series>();
            bool completed = false;
            try
            {
            	// Load the list of Series to be deleted from the WorkQueueUid
            	LoadUids(item);

				// Go through the list of series and add commands
				// to delete each of them. It's all or nothing.                
                using (var processor = new ServerCommandProcessor(String.Format("Deleting Series from study {0}, A#:{1}, Patient: {2}, ID:{3}", study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId)))
                {
                    StudyXml studyXml = StorageLocation.LoadStudyXml();
                    IDictionary<string, Series> existingSeries = StorageLocation.Study.Series;


                    // Add commands to delete the folders and update the xml
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        // Delete from study XML
                        if (studyXml.Contains(uid.SeriesInstanceUid))
                        {
                            //Note: DeleteDirectoryCommand  doesn't throw exception if the folder doesn't exist
                            var xmlUpdate = new RemoveSeriesFromStudyXml(studyXml, uid.SeriesInstanceUid);
                            processor.AddCommand(xmlUpdate);
                        }

                        // Delete from filesystem
                        string path = StorageLocation.GetSeriesPath(uid.SeriesInstanceUid);
                        if (Directory.Exists(path))
                        {
                            var delDir = new DeleteDirectoryCommand(path, true);
                            processor.AddCommand(delDir);
                        }
                    }

					// flush the updated xml to disk
                    processor.AddCommand(new SaveXmlCommand(studyXml, StorageLocation));

                    

                    // Update the db.. NOTE: these commands are executed at the end.
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        // Delete from DB
                    	WorkQueueUid queueUid = uid;
                        Series theSeries = existingSeries[queueUid.SeriesInstanceUid];
                        if (theSeries!=null)
                        {
                            _seriesToDelete.Add(theSeries);
                            var delSeries = new DeleteSeriesFromDBCommand(StorageLocation, theSeries);
                            processor.AddCommand(delSeries);
                            delSeries.Executing += DeleteSeriesFromDbExecuting;
                        }
                        else
                        {
                            // Series doesn't exist 
                            Platform.Log(LogLevel.Info, "Series {0} is invalid or no longer exists", uid.SeriesInstanceUid);
                        }

						// The WorkQueueUid must be cleared before the entry can be removed from the queue
                        var deleteUid = new DeleteWorkQueueUidCommand(uid);
                        processor.AddCommand(deleteUid);

						// Force a re-archival if necessary
                    	processor.AddCommand(new InsertArchiveQueueCommand(item.ServerPartitionKey, item.StudyStorageKey));
                    }

                    if (!processor.Execute())
                        throw new ApplicationException(
                            String.Format("Error occurred when series from Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber), processor.FailureException);
                    else
                    {
                        foreach (Series series in _seriesToDelete)
                        {
                            OnSeriesDeleted(series);
                        }
                    }
                }


                completed = true;
            }
            finally
            {
                if (completed)
                {
                    OnCompleted();
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }
                else
                {
                    PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }
            }
            
        }

        private void ProcessInstanceLevelDelete(Model.WorkQueue item)
        {
            // ensure the Study is loaded.
            Study study = StorageLocation.Study;
            Platform.CheckForNullReference(study, "Study record doesn't exist");

            Platform.Log(LogLevel.Info, "Processing Instance Level Deletion for Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber);

            bool completed = false;
            try
            {
                // Load the list of Sop Instances to be deleted from the WorkQueueUid
                LoadUids(item);

                // Go through the list of series and add commands
                // to delete each of them. It's all or nothing.                
                using (var processor = new ServerCommandProcessor(String.Format("Deleting Series from study {0}, A#:{1}, Patient: {2}, ID:{3}", study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId)))
                {
                    StudyXml studyXml = StorageLocation.LoadStudyXml();
                    IDictionary<string, Series> existingSeries = StorageLocation.Study.Series;


                    // Add commands to delete the folders and update the xml
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        // Delete from study XML
                        if (studyXml.Contains(uid.SeriesInstanceUid, uid.SopInstanceUid))
                        {
                            //Note: DeleteDirectoryCommand  doesn't throw exception if the folder doesn't exist
                            var xmlUpdate = new RemoveInstanceFromStudyXmlCommand(StorageLocation, studyXml, uid.SeriesInstanceUid, uid.SopInstanceUid);
                            processor.AddCommand(xmlUpdate);
                        }

                        // Delete from filesystem
                        string path = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
                        if (File.Exists(path))
                        {
                            var delDir = new FileDeleteCommand(path, true);
                            processor.AddCommand(delDir);
                        }
                    }

                    // flush the updated xml to disk
                    processor.AddCommand(new SaveXmlCommand(studyXml, StorageLocation));


                    // Update the db.. NOTE: these commands are executed at the end.
                    foreach (WorkQueueUid uid in WorkQueueUidList)
                    {
                        // Delete from DB
                        if (studyXml.Contains(uid.SeriesInstanceUid, uid.SopInstanceUid))
                        {
                            var delInstance = new UpdateInstanceCountCommand(StorageLocation, uid.SeriesInstanceUid, uid.SopInstanceUid);
                            processor.AddCommand(delInstance);
                            delInstance.Executing += DeleteSeriesFromDbExecuting;
                        }                       
                        else
                        {
                            // SOP doesn't exist 
                            Platform.Log(LogLevel.Info, "SOP {0} is invalid or no longer exists", uid.SopInstanceUid);
                        }

                        // The WorkQueueUid must be cleared before the entry can be removed from the queue
                        var deleteUid = new DeleteWorkQueueUidCommand(uid);
                        processor.AddCommand(deleteUid);

                        // Force a re-archival if necessary
                        processor.AddCommand(new InsertArchiveQueueCommand(item.ServerPartitionKey, item.StudyStorageKey));
                    }

                    if (!processor.Execute())
                        throw new ApplicationException(
                            String.Format("Error occurred when series from Study {0}, A#: {1}",
                                         study.StudyInstanceUid, study.AccessionNumber), processor.FailureException);                  
                }

                completed = true;
            }
            finally
            {
                if (completed)
                {
                    OnCompleted();
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                }
                else
                {
                    PostProcessing(item, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }
            }
        }

        private void OnCompleted()
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach (IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    var context = new WebDeleteProcessorContext(this, Level, StorageLocation, _reason, _userId, _userName );
                    extension.OnCompleted(context, _seriesToDelete);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }
        }

        private void DeleteSeriesFromDbExecuting(object sender, EventArgs e)
        {
            var cmd = sender as DeleteSeriesFromDBCommand;
			if (cmd!=null)
				OnDeletingSeriesInDatabase(cmd.Series);
        }

        private void OnDeletingSeriesInDatabase(Series series)
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach(IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    var context = new WebDeleteProcessorContext(this, Level, StorageLocation, _reason, _userId, _userName);
                    extension.OnSeriesDeleting(context, series);
                }   
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }

        }

        private void OnSeriesDeleted(Series seriesUid)
        {
            EnsureWebDeleteExtensionsLoaded();
            foreach (IWebDeleteProcessorExtension extension in _extensions)
            {
                try
                {
                    var context = new WebDeleteProcessorContext(this, Level, StorageLocation, _reason, _userId, _userName);
                    extension.OnSeriesDeleted(context, seriesUid);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error occurred in the extension but was ignored");
                }
            }
        }

        private void EnsureWebDeleteExtensionsLoaded()
        {
            if (_extensions==null)
            {
                var xp = new WebDeleteProcessorExtensionPoint();
                _extensions = CollectionUtils.Cast<IWebDeleteProcessorExtension>(xp.CreateExtensions());    
            }
        }

        private void ProcessStudyLevelDelete(Model.WorkQueue item)
        {
            LoadExtensions();

            OnDeletingStudy();

            FindAllRelatedDirectories();

            if (Study == null)
                Platform.Log(LogLevel.Info, "Deleting Study {0} on partition {1}",
                             StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
            else
                Platform.Log(LogLevel.Info,
                             "Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);

            RemoveFilesystem();

            RemoveDatabase(item);

            OnStudyDeleted();

            if (Study == null)
                Platform.Log(LogLevel.Info, "Completed Deleting Study {0} on partition {1}",
                             StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
            else
                Platform.Log(LogLevel.Info,
                             "Completed Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);
        }

        
        #endregion
    
    }

    internal class DeleteSeriesCommandProcessorEventArgs:EventArgs
    {
        private readonly string _seriesInstanceUid;

        public DeleteSeriesCommandProcessorEventArgs(string seriesInstanceUid)
        {
            _seriesInstanceUid = seriesInstanceUid;
        }

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
        }
    }
}
