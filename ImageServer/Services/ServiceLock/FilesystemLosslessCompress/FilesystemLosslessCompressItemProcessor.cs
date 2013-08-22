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
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemLosslessCompress
{
	/// <summary>
	/// Class for processing FilesystemLosslessCompress <see cref="ServiceLock"/> entries.
	/// </summary>
	public class FilesystemLosslessCompressItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
	{
		#region Private Members
		private int _studiesInserted = 0;
		#endregion

		#region Private Methods
		/// <summary>
		/// Process StudyCompress Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
		/// </summary>
		/// <param name="candidateList">The list of candidate studies for deleting.</param>
		/// <param name="type">The type of compress candidate (lossy or lossless)</param>
		private void ProcessCompressCandidates(IEnumerable<FilesystemQueue> candidateList, FilesystemQueueTypeEnum type)
		{
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				DateTime scheduledTime = Platform.Time.AddSeconds(10);

				foreach (FilesystemQueue queueItem in candidateList)
				{
					// Check for Shutdown/Cancel
					if (CancelPending) break;

					// First, get the StudyStorage locations for the study, and calculate the disk usage.
					StudyStorageLocation location;
					if (!FilesystemMonitor.Instance.GetWritableStudyStorageLocation(queueItem.StudyStorageKey, out location))
						continue;

					StudyXml studyXml;
					try
					{
						studyXml = LoadStudyXml(location);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e, "Skipping compress candidate, unexpected exception loading StudyXml file for {0}",
						             location.GetStudyPath());
						continue;
					}

					using (
						IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
					{
						ILockStudy lockstudy = update.GetBroker<ILockStudy>();
						LockStudyParameters lockParms = new LockStudyParameters();
						lockParms.StudyStorageKey = location.Key;
						lockParms.QueueStudyStateEnum = QueueStudyStateEnum.CompressScheduled;
						if (!lockstudy.Execute(lockParms) || !lockParms.Successful)
						{
							Platform.Log(LogLevel.Warn,
							             "Unable to lock study for inserting Lossless Compress. Reason:{0}. Skipping study ({1})",
							             lockParms.FailureReason, location.StudyInstanceUid);
							continue;
						}

						scheduledTime = scheduledTime.AddSeconds(3);

						IInsertWorkQueueFromFilesystemQueue workQueueInsert = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

						InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
						insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.CompressStudy;
						insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.LosslessCompress;
						insertParms.StudyStorageKey = location.GetKey();
						insertParms.ServerPartitionKey = location.ServerPartitionKey;
						DateTime expirationTime = scheduledTime;
						insertParms.ScheduledTime = expirationTime;
						insertParms.DeleteFilesystemQueue = true;
						insertParms.Data = queueItem.QueueXml;
						insertParms.FilesystemQueueTypeEnum = type;
						insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.CompressStudy;

						try
						{
							WorkQueue entry = workQueueInsert.FindOne(insertParms);

							InsertWorkQueueUidFromStudyXml(studyXml, update, entry.GetKey());

							update.Commit();
							_studiesInserted++;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Skipping compress record, unexpected problem inserting 'CompressStudy' record into WorkQueue for Study {0}",
							             location.StudyInstanceUid);
							// throw; -- would cause abort of inserts, go ahead and try everything
						}
					}
				}
			}
		}

		#endregion

		#region Public Methods
        protected override void OnProcess(Model.ServiceLock item)
		{

			ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

			Platform.Log(LogLevel.Info,
			             "Starting check for studies to lossless compress on filesystem '{0}'.",
			             fs.Filesystem.Description);

			int delayMinutes = ServiceLockSettings.Default.FilesystemLosslessCompressRecheckDelay;

			try
			{
				DateTime deleteTime = Platform.Time;
				FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.LosslessCompress;

				IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, deleteTime, type, false);

				if (list.Count > 0)
				{
					ProcessCompressCandidates(list, type);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing LosslessCompress records.");
				delayMinutes = 5;
			}

			DateTime scheduledTime = Platform.Time.AddMinutes(delayMinutes);
			if (_studiesInserted == 0)
				Platform.Log(LogLevel.Info,
				             "No eligible candidates to lossless compress from filesystem '{0}'.  Next scheduled filesystem check {1}",
				             fs.Filesystem.Description, scheduledTime);
			else
				Platform.Log(LogLevel.Info,
				             "Completed inserting lossless compress candidates into WorkQueue: {0}.  {1} studies inserted.  Next scheduled filesystem check {2}",
				             fs.Filesystem.Description, _studiesInserted,
				             scheduledTime);

			UnlockServiceLock(item, true, scheduledTime);
		}

		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion
	}
}
