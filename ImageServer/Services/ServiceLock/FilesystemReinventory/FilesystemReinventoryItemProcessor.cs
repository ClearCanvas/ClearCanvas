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
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemReinventory
{
    /// <summary>
    /// Class for processing 'FilesystemReinventory' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    class FilesystemReinventoryItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
    	#region Private Members
        private IPersistentStore _store;
        private IList<ServerPartition> _partitions;
        #endregion

        #region Private Methods

		private static void RemoveStudyStorage(StudyStorageLocation location)
		{
			// NOTE:  This was an IUpdateContext, however, it was modified to be an IReadContext
			// after having problems w/ locks on asystem with a fair amount of load.  The 
			// updates are just automatically committed within the stored procedure when it
			// runs...
			using (IReadContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				// Setup the delete parameters
				var parms = new DeleteStudyStorageParameters
				                                     	{
				                                     		ServerPartitionKey = location.ServerPartitionKey,
				                                     		StudyStorageKey = location.Key
				                                     	};

				// Get the Insert Instance broker and do the insert
				var delete = updateContext.GetBroker<IDeleteStudyStorage>();

				if (false == delete.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0}",
								 location.StudyInstanceUid);
				}
			}
		}

		private Study GetStudyAndQueues(StudyStorageLocation location, out int integrityQueueCount, out int workQueueCount)
		{
			using (IReadContext context = _store.OpenReadContext())
			{
				var integrityBroker = context.GetBroker<IStudyIntegrityQueueEntityBroker>();
				var integrityCriteria = new StudyIntegrityQueueSelectCriteria();
				integrityCriteria.StudyStorageKey.EqualTo(location.Key);
				integrityQueueCount = integrityBroker.Count(integrityCriteria);

				var workBroker = context.GetBroker<IWorkQueueEntityBroker>();
				var workCriteria = new WorkQueueSelectCriteria();
				workCriteria.StudyStorageKey.EqualTo(location.Key);
				workQueueCount = workBroker.Count(workCriteria);

				var procedure = context.GetBroker<IStudyEntityBroker>();
				var criteria = new StudySelectCriteria();
				criteria.StudyStorageKey.EqualTo(location.Key);
				return procedure.FindOne(criteria);
			}
		}

		private void ReinventoryFilesystem(Filesystem filesystem)
        {
			var filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach(DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
	            ServerPartition partition;
	            if (GetServerPartition(partitionDir.Name, out partition) == false)
                    continue;

                foreach(DirectoryInfo dateDir in partitionDir.GetDirectories())
                {
                    if (dateDir.FullName.EndsWith("Deleted", StringComparison.InvariantCultureIgnoreCase)
						|| dateDir.FullName.EndsWith(ServerPlatform.ReconcileStorageFolder, StringComparison.InvariantCultureIgnoreCase))
						continue;

	                foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
					{
                        if (studyDir.FullName.EndsWith("Deleted", StringComparison.InvariantCultureIgnoreCase))
                            continue;
					    
						// Check for Cancel message
						if (CancelPending) return;

						String studyInstanceUid = studyDir.Name;

						StudyStorageLocation location;
						List<FileInfo> fileList;
						if (GetStudyStorageLocation(partition.Key, studyInstanceUid, out location))
						{
                            #region Study record exists in db

                            int integrityQueueCount;
                            int workQueueCount;
                            Study theStudy = GetStudyAndQueues(location, out integrityQueueCount, out workQueueCount);
                            if (theStudy != null)
                                continue;

                            if (integrityQueueCount != 0 && workQueueCount != 0)
                                continue;

                            fileList = LoadSopFiles(studyDir, false);

                            if (fileList.Count == 0)
                            {
                                Platform.Log(LogLevel.Warn, "Found empty study folder with StorageLocation, deleteing StorageLocation: {0}\\{1}",
                                             dateDir.Name, studyDir.Name);
                                studyDir.Delete(true);

                                RemoveStudyStorage(location);
                                continue;
                            }

                            // WriteLock the new study storage for study processing
                            if (!location.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled))
                            {
                            	string failureReason;
								if (!ServerHelper.LockStudy(location.Key,QueueStudyStateEnum.ProcessingScheduled, out failureReason))
                                    Platform.Log(LogLevel.Error, "Unable to lock study {0} for Study Processing", location.StudyInstanceUid);
                            } 
                            #endregion
						}
						else
						{
                            #region Directory not in DB, 

                            fileList = LoadSopFiles(studyDir, true);

                            if (fileList.Count == 0)
                            {
                                Platform.Log(LogLevel.Warn, "Found empty study folder: {0}\\{1}", dateDir.Name, studyDir.Name);
                                continue;
                            }

							DicomFile file = LoadFileFromList(fileList);

                            if (file == null)
                            {
                                Platform.Log(LogLevel.Warn, "Found directory with no readable files: {0}\\{1}", dateDir.Name, studyDir.Name);
                                continue;
                            }

                            // Do a second check, using the study instance uid from a file in the directory.
                            // had an issue with trailing periods on uids causing us to not find the 
                            // study storage, and insert a new record into the database.
                            studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
                            if (GetStudyStorageLocation(partition.Key, studyInstanceUid, out location))
                            {
                                continue;
                            }

                            StudyStorage storage;
                            if (GetStudyStorage(partition, studyInstanceUid, out storage))
                            {
                                Platform.Log(LogLevel.Warn, "Study {0} on filesystem partition {1} is offline {2}", studyInstanceUid,
                                             partition.Description, studyDir.ToString());
                                continue;
                            }

                            Platform.Log(LogLevel.Info, "Reinventory inserting study storage location for {0} on partition {1}", studyInstanceUid,
                                         partition.Description);

                            // Insert StudyStorage
                            using (IUpdateContext update = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                            {
                                var studyInsert = update.GetBroker<IInsertStudyStorage>();
                                var insertParms = new InsertStudyStorageParameters
                                                                           	{
                                                                           		ServerPartitionKey = partition.GetKey(),
                                                                           		StudyInstanceUid = studyInstanceUid,
                                                                           		Folder = dateDir.Name,
                                                                           		FilesystemKey = filesystem.GetKey(),
                                                                           		QueueStudyStateEnum =
                                                                           			QueueStudyStateEnum.Idle
                                                                           	};
                            	if (file.TransferSyntax.LosslessCompressed)
                                {
                                    insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
                                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                                }
                                else if (file.TransferSyntax.LossyCompressed)
                                {
                                    insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
                                    insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
                                }
                                else
                                {
                                    insertParms.TransferSyntaxUid = file.TransferSyntax.UidString;
                                    insertParms.StudyStatusEnum = StudyStatusEnum.Online;
                                }

                                location = studyInsert.FindOne(insertParms);

                                // WriteLock the new study storage for study processing
                                var lockStudy = update.GetBroker<ILockStudy>();
                                var lockParms = new LockStudyParameters
                                                                	{
                                                                		StudyStorageKey = location.Key,
                                                                		QueueStudyStateEnum =
                                                                			QueueStudyStateEnum.ProcessingScheduled
                                                                	};
                            	if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
                                    Platform.Log(LogLevel.Error, "Unable to lock study {0} for Study Processing", location.StudyInstanceUid);

                                update.Commit();
                            }		 
                            #endregion					
						}

					    string studyXml = location.GetStudyXmlPath();
						FileUtils.Delete(studyXml);

                        string studyGZipXml = location.GetCompressedStudyXmlPath();
                        FileUtils.Delete(studyGZipXml);


						foreach (FileInfo sopFile in fileList)
						{
							String sopInstanceUid = sopFile.Name.Replace(sopFile.Extension, string.Empty);

							using (var context = new ServerExecutionContext())
							{
								// Just use a read context here, in hopes of improving 
								// performance.  Every other place in the code should use
								// Update contexts when doing transactions.
								var workQueueInsert =
									context.ReadContext.GetBroker<IInsertWorkQueue>();

								var queueInsertParms =
									new InsertWorkQueueParameters
										{
											WorkQueueTypeEnum = WorkQueueTypeEnum.StudyProcess,
											StudyStorageKey = location.GetKey(),
											ServerPartitionKey = partition.GetKey(),
											SeriesInstanceUid = sopFile.Directory.Name,
											SopInstanceUid = sopInstanceUid,
											ScheduledTime = Platform.Time,
											WorkQueuePriorityEnum = WorkQueuePriorityEnum.Low
										};

								if (workQueueInsert.FindOne(queueInsertParms) == null)
									Platform.Log(LogLevel.Error,
									             "Failure attempting to insert SOP Instance into WorkQueue during Reinventory.");
							}
						}
					}

                	// Cleanup the date directory, if its empty.
                	DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
                }
            }
        }
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _partitions)
            {
                if (part.PartitionFolder == partitionFolderName)
                {
                    partition = part;
                    return true;
                }                
            }

            partition = null;
            return false;
        }

		private static bool GetStudyStorage(ServerPartition partition, string studyInstanceUid, out StudyStorage storage)
		{
			using (var context = new ServerExecutionContext())
			{
				storage = StudyStorage.Load(context.ReadContext, partition.Key, studyInstanceUid);
				if (storage != null)
					return true;

				return false;
			}
		}

        #endregion

        #region Public Methods
        protected override void OnProcess(Model.ServiceLock item)
        {
            _store = PersistentStoreRegistry.GetDefaultStore();
			using (var context = new ServerExecutionContext())
			{
				var broker = context.ReadContext.GetBroker<IServerPartitionEntityBroker>();
				var criteria = new ServerPartitionSelectCriteria();
				criteria.AeTitle.SortAsc(0);

				_partitions = broker.Find(criteria);
			}

        	ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

            Platform.Log(LogLevel.Info, "Starting reinventory of filesystem: {0}", info.Filesystem.Description);

            ReinventoryFilesystem(info.Filesystem);

            item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
							 "Filesystem Reinventory of {0} has been canceled, rescheduling.  Note that the entire Filesystem will be reinventoried again.",
							 info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
			{
				Platform.Log(LogLevel.Info, "Filesystem Reinventory of {0} has completed.",
							 info.Filesystem.Description);
				UnlockServiceLock(item, false, Platform.Time.AddDays(1));
			}
        }

        
        #endregion

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
