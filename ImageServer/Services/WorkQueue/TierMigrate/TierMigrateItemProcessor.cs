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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using DeleteDirectoryCommand = ClearCanvas.ImageServer.Core.Command.DeleteDirectoryCommand;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    /// <summary>
    /// States of the tier migration entry. Used for resume after auto-recovery.
    /// </summary>
    public enum TierMigrationProcessingState
    {
        NotStated,
        Migrated,
    }

    /// <summary>
    /// The data in the Data column.
    /// </summary>
    public class TierMigrationWorkQueueData
    {
        #region Private Members

    	#endregion

        #region Public Properties

    	public TierMigrationProcessingState State { get; set; }

    	#endregion
    }

    
    /// <summary>
    /// Class for processing TierMigrate <see cref="WorkQueue"/> entries.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class TierMigrateItemProcessor : BaseItemProcessor
    {
        #region Private Static Members
        private static readonly Object _statisticsLock = new Object();
        private static TierMigrationAverageStatistics _average = new TierMigrationAverageStatistics();
        private static int _studiesMigratedCount;

        #endregion


        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected override void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            DBUpdateTime.Add(
                delegate
                {
                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                        UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters
                                                          	{
                                                          		ProcessorID = ServerPlatform.ProcessorId,
                                                          		WorkQueueKey = item.GetKey(),
                                                          		StudyStorageKey = item.StudyStorageKey,
                                                          		FailureCount = item.FailureCount + 1,
                                                          		FailureDescription = failureDescription
                                                          	};

                    	Platform.Log(LogLevel.Error,
                                     "Failing {0} WorkQueue entry ({1}): {2}", 
                                     item.WorkQueueTypeEnum, 
                                     item.GetKey(), failureDescription);

                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                        parms.ScheduledTime = Platform.Time;
                        parms.ExpirationTime = Platform.Time.AddDays(1);
						
                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                         item.GetKey().ToString());
                        }
                        else
                            updateContext.Commit();
                    }
                }
                );


        }


		protected override void ProcessItem(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");

            Platform.Log(LogLevel.Info,
                             "Starting Tier Migration of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);

            // The WorkQueue Data column contains the state of the processing. It is set to "Migrated" if the entry has been
            // successfully executed once.  
            TierMigrationProcessingState state = GetCurrentState(item);
            switch(state)
            {
                case TierMigrationProcessingState.Migrated:
                    Platform.Log(LogLevel.Info, "Study has been migrated to {0} on {1}", StorageLocation.FilesystemPath, StorageLocation.FilesystemTierEnum.Description);
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    break;

                case TierMigrationProcessingState.NotStated:
                    ServerFilesystemInfo currFilesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
                    ServerFilesystemInfo newFilesystem = FilesystemMonitor.Instance.GetLowerTierFilesystemForStorage(currFilesystem);
                    
                    if (newFilesystem == null)
                    {
                        // This entry shouldn't have been scheduled in the first place.
                        // It's possible that the folder wasn't full when the entry was scheduled. 
                        // Another possiblity is the study was migrated but an error was encountered when updating the entry.//
                        // We should re-insert the filesystem queue so that if the study will be migrated if the space is freed up 
                        // in the future.
                        String msg = String.Format(
                                "Study '{0}' cannot be migrated: no writable filesystem can be found in lower tiers for filesystem '{1}'. Reschedule migration for future.",
                                StorageLocation.StudyInstanceUid, currFilesystem.Filesystem.Description);

                        Platform.Log(LogLevel.Warn, msg);
                        ReinsertFilesystemQueue();
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    }
                    else
                    {
                        DoMigrateStudy(StorageLocation, newFilesystem);

                        // Update the state separately so that if the validation (done in the PostProcessing method) fails, 
                        // we know the study has been migrated when we resume after auto-recovery has been completed.
                        UpdateState(item.Key, TierMigrationProcessingState.Migrated);
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    
                    }
                    break;

                default:
                    throw new NotImplementedException("Not implemented");
            }

		}

        private static TierMigrationProcessingState GetCurrentState(Model.WorkQueue item)
        {
            if (item.Data == null)
            {
                //TODO: What about old entries?
                return TierMigrationProcessingState.NotStated;
            }
        	TierMigrationWorkQueueData data = XmlUtils.Deserialize<TierMigrationWorkQueueData>(item.Data);
        	return data.State;
        }

        private static void UpdateState(ServerEntityKey key, TierMigrationProcessingState state)
        {
            TierMigrationWorkQueueData data = new TierMigrationWorkQueueData {State = state};

        	using(IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker broker = context.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueUpdateColumns parms = new WorkQueueUpdateColumns {Data = XmlUtils.SerializeAsXmlDoc(data)};
            	if (!broker.Update(key, parms))
                    throw new ApplicationException("Unable to update work queue state");
            	context.Commit();
            }
        }

        /// <summary>
        /// Migrates the study to new tier
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="newFilesystem"></param>
        private void DoMigrateStudy(StudyStorageLocation storage, ServerFilesystemInfo newFilesystem)
        {
            Platform.CheckForNullReference(storage, "storage");
            Platform.CheckForNullReference(newFilesystem, "newFilesystem");

            TierMigrationStatistics stat = new TierMigrationStatistics {StudyInstanceUid = storage.StudyInstanceUid};
        	stat.ProcessSpeed.Start();
    	    StudyXml studyXml = storage.LoadStudyXml();
            stat.StudySize = (ulong) studyXml.GetStudySize(); 

            Platform.Log(LogLevel.Info, "About to migrate study {0} from {1} to {2}", 
                    storage.StudyInstanceUid, storage.FilesystemTierEnum, newFilesystem.Filesystem.Description);
			
            string newPath = Path.Combine(newFilesystem.Filesystem.FilesystemPath, storage.PartitionFolder);
    	    DateTime startTime = Platform.Time;
            DateTime lastLog = Platform.Time;
    	    int fileCounter = 0;
    	    ulong bytesCopied = 0;
    	    long instanceCountInXml = studyXml.NumberOfStudyRelatedInstances;
            
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Migrate Study"))
            {
                TierMigrationContext context = new TierMigrationContext
                                               	{
                                               		OriginalStudyLocation = storage,
                                               		Destination = newFilesystem
                                               	};

				// The multiple CreateDirectoryCommands are done so that rollback of the directories being created happens properly if either of the directories already exist.
				var origFolder = context.OriginalStudyLocation.GetStudyPath();
                processor.AddCommand(new CreateDirectoryCommand(newPath));

                newPath = Path.Combine(newPath, context.OriginalStudyLocation.StudyFolder);
                processor.AddCommand(new CreateDirectoryCommand(newPath));

                newPath = Path.Combine(newPath, context.OriginalStudyLocation.StudyInstanceUid);
                // don't create this directory so that it won't be backed up by MoveDirectoryCommand

				var copyDirCommand = new CopyDirectoryCommand(origFolder, newPath, 
                    delegate (string path)
                        {
                            // Update the progress. This is useful if the migration takes long time to complete.

                            FileInfo file = new FileInfo(path);
                            bytesCopied += (ulong)file.Length;
                            fileCounter++;
                            if (file.Extension != null && file.Extension.Equals(ServerPlatform.DicomFileExtension, StringComparison.InvariantCultureIgnoreCase))
                            {
                                TimeSpan elapsed = Platform.Time - lastLog;
                                TimeSpan totalElapsed = Platform.Time - startTime;
                                double speedInMBPerSecond = 0;
                                if (totalElapsed.TotalSeconds > 0)
                                {
                                    speedInMBPerSecond = (bytesCopied / 1024f / 1024f) / totalElapsed.TotalSeconds;
                                }

                                if (elapsed > TimeSpan.FromSeconds(WorkQueueSettings.Instance.TierMigrationProgressUpdateInSeconds))
                                {
                                    #region Log Progress

                                    StringBuilder stats = new StringBuilder();
                                    if (instanceCountInXml != 0)
                                    {
                                        float pct = (float)fileCounter / instanceCountInXml;
                                        stats.AppendFormat("{0} files moved [{1:0.0}MB] since {2} ({3:0}% completed). Speed={4:0.00}MB/s",
                                                    fileCounter, bytesCopied / 1024f / 1024f, startTime, pct * 100, speedInMBPerSecond);
                                    }
                                    else
                                    {
                                        stats.AppendFormat("{0} files moved [{1:0.0}MB] since {2}. Speed={3:0.00}MB/s",
                                                    fileCounter, bytesCopied / 1024f / 1024f, startTime, speedInMBPerSecond);

                                    }

                                    Platform.Log(LogLevel.Info, "Tier migration for study {0}: {1}", storage.StudyInstanceUid, stats.ToString());
                                    try
                                    {
                                        using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                                        {
                                            IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                                            WorkQueueUpdateColumns parameters = new WorkQueueUpdateColumns
                                                                                	{FailureDescription = stats.ToString()};
                                        	broker.Update(WorkQueueItem.GetKey(), parameters);
                                            ctx.Commit();
                                        }
                                    }
                                    catch
                                    {
                                    	// can't log the progress so far... just ignore it
                                    }
                                    finally
                                    {
                                        lastLog = DateTime.Now;
                                    } 
                                    #endregion
                                }
                            }
                        });
                processor.AddCommand(copyDirCommand);

                DeleteDirectoryCommand delDirCommand = new DeleteDirectoryCommand(origFolder, false)
                                                       	{RequiresRollback = false};
            	processor.AddCommand(delDirCommand);
                
                TierMigrateDatabaseUpdateCommand updateDbCommand = new TierMigrateDatabaseUpdateCommand(context);
                processor.AddCommand(updateDbCommand);

                Platform.Log(LogLevel.Info, "Start migrating study {0}.. expecting {1} to be moved", storage.StudyInstanceUid, ByteCountFormatter.Format(stat.StudySize));
                if (!processor.Execute())
                {
                	if (processor.FailureException != null)
                        throw processor.FailureException;
                	throw new ApplicationException(processor.FailureReason);
                }

            	stat.DBUpdate = updateDbCommand.Statistics;
                stat.CopyFiles = copyDirCommand.CopySpeed;
                stat.DeleteDirTime = delDirCommand.Statistics;
            }

            stat.ProcessSpeed.SetData(bytesCopied);
            stat.ProcessSpeed.End();

            Platform.Log(LogLevel.Info, "Successfully migrated study {0} from {1} to {2} in {3} [ {4} files, {5} @ {6}, DB Update={7}, Remove Dir={8}]",
                            storage.StudyInstanceUid, 
                            storage.FilesystemTierEnum,
                            newFilesystem.Filesystem.FilesystemTierEnum,
                            TimeSpanFormatter.Format(stat.ProcessSpeed.ElapsedTime), 
                            fileCounter,
                            ByteCountFormatter.Format(bytesCopied), 
                            stat.CopyFiles.FormattedValue,
                            stat.DBUpdate.FormattedValue,
                            stat.DeleteDirTime.FormattedValue);

    	    string originalPath = storage.GetStudyPath();
            if (Directory.Exists(storage.GetStudyPath()))
            {
                Platform.Log(LogLevel.Info, "Original study folder could not be deleted. It must be cleaned up manually: {0}", originalPath);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, WorkQueueItem.WorkQueueTypeEnum.ToString(), 1000, GetWorkQueueContextData(WorkQueueItem), TimeSpan.Zero,
                    "Study has been migrated to a new tier. Original study folder must be cleaned up manually: {0}", originalPath);
            }

            UpdateAverageStatistics(stat);
            
        }

        private static void UpdateAverageStatistics(TierMigrationStatistics stat)
        {
            lock(_statisticsLock)
            {
                _average.AverageProcessSpeed.AddSample(stat.ProcessSpeed);
                _average.AverageStudySize.AddSample(stat.StudySize);
                _average.AverageStudySize.AddSample(stat.ProcessSpeed);
                _average.AverageFileMoveTime.AddSample(stat.CopyFiles);
                _average.AverageDBUpdateTime.AddSample(stat.DBUpdate);
                _studiesMigratedCount++;
                if (_studiesMigratedCount % 5 ==0)
                {
                    StatisticsLogger.Log(LogLevel.Info, _average);
                    _average = new TierMigrationAverageStatistics();
                }
            }
        }

        protected override bool CanStart()
        {
			
			IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
                                                    new []
			                                           {
			                                               WorkQueueTypeEnum.StudyProcess, 
                                                           WorkQueueTypeEnum.ReconcileStudy, 
                                                           WorkQueueTypeEnum.ProcessDuplicate,
                                                           WorkQueueTypeEnum.ReconcilePostProcess,
                                                           WorkQueueTypeEnum.ReconcileCleanup
                                                        }, null);

            IList<StudyIntegrityQueue> reconcileList = ServerHelper.FindSIQEntries(WorkQueueItem.StudyStorageKey, null);
            
            if ((relatedItems == null || relatedItems.Count == 0) && (reconcileList == null || reconcileList.Count == 0))
                return true; // nothing related in the work queue and nothing in the reconcile queue
            

            Platform.Log(LogLevel.Info,
						 "Tier Migrate entry for study {0} has conflicting WorkQueue entry, reinserting into FilesystemQueue",
						 StorageLocation.StudyInstanceUid);
		
			ReinsertFilesystemQueue();

			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			return false;
		}

        private void ReinsertFilesystemQueue()
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker broker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidSelectCriteria workQueueUidCriteria = new WorkQueueUidSelectCriteria();
                workQueueUidCriteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
                broker.Delete(workQueueUidCriteria);

                FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters
                                                        	{
                                                        		FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate,
                                                        		ScheduledTime = Platform.Time.AddMinutes(10),
                                                        		StudyStorageKey = WorkQueueItem.StudyStorageKey,
                                                        		FilesystemKey = StorageLocation.FilesystemKey
                                                        	};

            	IInsertFilesystemQueue insertQueue = updateContext.GetBroker<IInsertFilesystemQueue>();

                if (false == insertQueue.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unexpected failure inserting FilesystemQueue entry");
                }
                else
                    updateContext.Commit();
            }
        }
    }    
}
