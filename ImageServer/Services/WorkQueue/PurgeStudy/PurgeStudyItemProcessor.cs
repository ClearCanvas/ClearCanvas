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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.PurgeStudy
{

    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class PurgeStudyItemProcessor : BaseItemProcessor
	{
		#region Private Methods
		private void RemoveFilesystem()
		{
			string path = StorageLocation.GetStudyPath();
			DirectoryUtility.DeleteIfExists(path, true);
		}

		private void RemoveDatabase(Model.WorkQueue item)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				// Setup the delete parameters
				DeleteFilesystemStudyStorageParameters parms = new DeleteFilesystemStudyStorageParameters();

				parms.ServerPartitionKey = item.ServerPartitionKey;
				parms.StudyStorageKey = item.StudyStorageKey;
				parms.StudyStatusEnum = StudyStatusEnum.Nearline; // TODO: Don't we set Nearline only if all the storage location are purged?

				// Get the Insert Instance broker and do the insert
				IDeleteFilesystemStudyStorage delete = updateContext.GetBroker<IDeleteFilesystemStudyStorage>();

				if (false == delete.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0} on partition {1}",
								 StorageLocation.StudyInstanceUid, ServerPartition.Description);
				}
				else
				{
					// Unlock the study, too
					ILockStudy studyLock = updateContext.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters();
					lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
					lockParms.StudyStorageKey = item.StudyStorageKey;
					studyLock.Execute(lockParms);

					updateContext.Commit();
				}
			}
		}
		#endregion

		#region Overridden Protected Method

		protected override void ProcessItem(Model.WorkQueue item)
		{
			Platform.Log(LogLevel.Info,
			             "Purging study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
			             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
			             Study.AccessionNumber, ServerPartition.Description);

			RemoveFilesystem();


			RemoveDatabase(item);

			// No need to remove / update the Queue entry, it was deleted as part of the delete process.
		}

		#endregion

		private void ReinsertFilesystemQueue(TimeSpan delay)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker broker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidSelectCriteria workQueueUidCriteria = new WorkQueueUidSelectCriteria();
				workQueueUidCriteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
				broker.Delete(workQueueUidCriteria);

				FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters
				                                        	{
				                                        		FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.PurgeStudy,
				                                        		ScheduledTime = Platform.Time + delay,
				                                        		StudyStorageKey = WorkQueueItem.StudyStorageKey,
				                                        		FilesystemKey = StorageLocation.FilesystemKey,
				                                        		QueueXml = WorkQueueItem.Data
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

        protected override bool CanStart()
        {
        	IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
        	                                                                new[]
        	                                                                	{
        	                                                                		WorkQueueTypeEnum.StudyProcess,
        	                                                                		WorkQueueTypeEnum.ReconcileStudy,
        	                                                                		WorkQueueTypeEnum.WebEditStudy,
        	                                                                		WorkQueueTypeEnum.CleanupDuplicate,
        	                                                                		WorkQueueTypeEnum.CleanupStudy
        	                                                                	},
        	                                                                new[]
        	                                                                	{
        	                                                                		WorkQueueStatusEnum.Idle,
        	                                                                		WorkQueueStatusEnum.InProgress,
        	                                                                		WorkQueueStatusEnum.Pending,
        	                                                                		WorkQueueStatusEnum.Failed
        	                                                                	});
			if (!(relatedItems == null || relatedItems.Count == 0) || StorageLocation.IsReconcileRequired)
            {
				Platform.Log(LogLevel.Info, "PurgeStudy entry for study {0} has existing WorkQueue entry, reinserting into FilesystemQueue", StorageLocation.StudyInstanceUid);
				TimeSpan delay = TimeSpan.FromMinutes(60);
				ReinsertFilesystemQueue(delay);
				PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				return false;
            }

        	return true;
        }
    }
}
