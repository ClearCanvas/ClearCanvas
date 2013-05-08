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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	/// <summary>
	/// Base class for implementing archives.
	/// </summary>
	/// <remarks>
	/// Archives supported by ImageServer are implemented via the <see cref="ImageServerArchiveExtensionPoint"/>
	/// plugin.  These plugins must implement the <see cref="IImageServerArchivePlugin"/> 
	/// interface.  The ImageServerArchiveBase class implements a base set of methods that
	/// will be used by any archive plugin.
	/// </remarks>
	public abstract class ImageServerArchiveBase : IImageServerArchivePlugin, IDisposable
	{
		protected PartitionArchive _partitionArchive;
		private ServerPartition _serverPartition;
		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
		private readonly FilesystemSelector _selector;

		/// <summary>
		/// The <see cref="ServerPartition"/> associated with the archive.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
		}

		/// <summary>
		/// A <see cref="FilesystemSelector"/> used to select a filesystem when restoring studies.
		/// </summary>
		public FilesystemSelector Selector
		{
			get { return _selector; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ImageServerArchiveBase()
		{
			_selector = new FilesystemSelector(FilesystemMonitor.Instance);
		}

		public abstract ArchiveTypeEnum ArchiveType { get; }
        public abstract PartitionArchive PartitionArchive { get; set; }
	
		/// <summary>
		/// The persistent store.
		/// </summary>
		public IPersistentStore PersistentStore
		{
			get { return _store; }
		}

		public abstract void Start(PartitionArchive archive);
		public abstract void Stop();

		/// <summary>
		/// Get a list of restore candidates for the archive.
		/// </summary>
		/// <remarks>
		/// Note that at the current time only one cadidate is returned at a time.
		/// </remarks>
		/// <returns>A restore candidate.  null will be returned if no candidates exist.</returns>
		public virtual RestoreQueue GetRestoreCandidate()
		{
			RestoreQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServerPlatform.ProcessorId;
				parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
				IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

				if (queueItem != null)
					updateContext.Commit();
			}

			return queueItem;
		}

		/// <summary>
		/// Get candidates for archival on the <see cref="PartitionArchive"/>.
		/// </summary>
		/// <returns>A list of archive candidates.  The list will be empty if no candidates exist.</returns>
		public ArchiveQueue GetArchiveCandidate()
		{
			ArchiveQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryArchiveQueueParameters parms = new QueryArchiveQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServerPlatform.ProcessorId;

				IQueryArchiveQueue broker = updateContext.GetBroker<IQueryArchiveQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

				if (queueItem != null)
					updateContext.Commit();
			}

			return queueItem;
		}

		/// <summary>
		/// Reset any archival request that may have been left In Progress when the service last shut down.
		/// </summary>
		public void ResetFailedArchiveQueueItems()
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IArchiveQueueEntityBroker broker = updateContext.GetBroker<IArchiveQueueEntityBroker>();

				ArchiveQueueSelectCriteria criteria = new ArchiveQueueSelectCriteria();
				criteria.ProcessorId.EqualTo(ServerPlatform.ProcessorId);
				criteria.ArchiveQueueStatusEnum.EqualTo(ArchiveQueueStatusEnum.InProgress);

				IList<ArchiveQueue> failedList = broker.Find(criteria);
				foreach (ArchiveQueue failedItem in failedList)
				{
					UpdateArchiveQueue(updateContext, failedItem, ArchiveQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));

					Platform.Log(LogLevel.Warn,
					             "Reseting ArchiveQueue entry {0} to Pending that was In Progress at startup for PartitionArchive {1}",
					             failedItem.Key, _partitionArchive.Description);

				}

				if (failedList.Count > 0)
					updateContext.Commit();
				else 
					Platform.Log(LogLevel.Info,"No ArchiveQueue entries to reset on startup for archive {0}",_partitionArchive.Description);
			}
			
		}

		/// <summary>
		/// Reset any failed restore requests that may have been left In Progress when the service last shut down.
		/// </summary>
		public void ResetFailedRestoreQueueItems()
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IRestoreQueueEntityBroker broker = updateContext.GetBroker<IRestoreQueueEntityBroker>();

				ArchiveStudyStorageSelectCriteria selectStudyStorage = new ArchiveStudyStorageSelectCriteria();
				selectStudyStorage.PartitionArchiveKey.EqualTo(_partitionArchive.Key);

				RestoreQueueSelectCriteria criteria = new RestoreQueueSelectCriteria();
				criteria.ProcessorId.EqualTo(ServerPlatform.ProcessorId);
				criteria.RestoreQueueStatusEnum.EqualTo(RestoreQueueStatusEnum.InProgress);
				criteria.ArchiveStudyStorageRelatedEntityCondition.Exists(selectStudyStorage);

				IList<RestoreQueue> failedList = broker.Find(criteria);
				foreach (RestoreQueue failedItem in failedList)
				{
					UpdateRestoreQueue(updateContext, failedItem, RestoreQueueStatusEnum.Pending, Platform.Time.AddMinutes(2));

					Platform.Log(LogLevel.Warn,
								 "Reseting RestoreQueue entry {0} to Pending that was In Progress at startup for PartitionArchive '{1}' on ",
								 failedItem.Key, _partitionArchive.Description);
				}

				if (failedList.Count > 0)
					updateContext.Commit();
				else
					Platform.Log(LogLevel.Info, "No RestoreQueue entries to reset on startup for archive {0}", _partitionArchive.Description);
			}
		}

		/// <summary>
		/// Load the server partition information for the the archive.
		/// </summary>
		public void LoadServerPartition()
		{
			_serverPartition = ServerPartition.Load(_partitionArchive.ServerPartitionKey);
		}

		/// <summary>
		/// Update an <see cref="ArchiveQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		public void UpdateArchiveQueue(ArchiveQueue item, ArchiveQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				if (UpdateArchiveQueue(updateContext,item,status,scheduledTime))
				{
					updateContext.Commit();
				}
			}
		}

		/// <summary>
		/// Update an <see cref="ArchiveQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		/// <param name="updateContext">The update context</param>
		public bool UpdateArchiveQueue(IUpdateContext updateContext, ArchiveQueue item, ArchiveQueueStatusEnum status, DateTime scheduledTime)
		{
			UpdateArchiveQueueParameters parms = new UpdateArchiveQueueParameters();
			parms.ArchiveQueueKey = item.GetKey();
			parms.ArchiveQueueStatusEnum = status;
			parms.ScheduledTime = scheduledTime;
			parms.StudyStorageKey = item.StudyStorageKey;
			if (!String.IsNullOrEmpty(item.FailureDescription))
				parms.FailureDescription = item.FailureDescription;

			IUpdateArchiveQueue broker = updateContext.GetBroker<IUpdateArchiveQueue>();

			if (broker.Execute(parms))
			{
				return true;
			}

			Platform.Log(LogLevel.Error, "Unexpected failure updating ArchiveQueue entry {0}", item.GetKey());
			return false;
		}

		/// <summary>
		/// Update a <see cref="RestoreQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		public void UpdateRestoreQueue(RestoreQueue item, RestoreQueueStatusEnum status, DateTime scheduledTime)
		{
			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				if (UpdateRestoreQueue(updateContext, item, status, scheduledTime))
					updateContext.Commit();
			}
		}

		/// <summary>
		/// Update a <see cref="RestoreQueue"/> entry.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="status">The status to set the entry to.</param>
		/// <param name="scheduledTime">The scheduled time to set the entry to.</param>
		/// <param name="updateContext">The update context</param>
		public bool UpdateRestoreQueue(IUpdateContext updateContext, RestoreQueue item, RestoreQueueStatusEnum status, DateTime scheduledTime)
		{
			UpdateRestoreQueueParameters parms = new UpdateRestoreQueueParameters();
			parms.RestoreQueueKey = item.GetKey();
			parms.RestoreQueueStatusEnum = status;
			parms.ScheduledTime = scheduledTime;
			parms.StudyStorageKey = item.StudyStorageKey;
			if (!String.IsNullOrEmpty(item.FailureDescription))
				parms.FailureDescription = item.FailureDescription;
				
			IUpdateRestoreQueue broker = updateContext.GetBroker<IUpdateRestoreQueue>();

			if (broker.Execute(parms))
			{
				return true;
			}
			
			Platform.Log(LogLevel.Error, "Unexpected failure updating RestoreQueue entry {0}", item.GetKey());
			return false;
		}

		/// <summary>
		/// Dispose the class.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
