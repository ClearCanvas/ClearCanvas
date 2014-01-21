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
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemDelete
{
    /// <summary>
    /// Represents the state of a filesystem
    /// </summary>
    [XmlRoot("FilesystemState")]
    public class FilesystemState
    {
        #region Private Members
        private DateTime? _aboveHighWatermarkTimestamp;
        private DateTime? _lastHighWatermarkAlertTimestamp;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the timestamp when the filesystem is above the high watermark level
        /// </summary>
        public DateTime? AboveHighWatermarkTimestamp
        {
            get { return _aboveHighWatermarkTimestamp; }
            set { _aboveHighWatermarkTimestamp = value; }
        }

        /// <summary>
        /// Gets or sets the timestamp when an alert was generated because filesystem is above the high watermark level.
        /// </summary>
        public DateTime? LastHighWatermarkAlertTimestamp
        {
            get { return _lastHighWatermarkAlertTimestamp; }
            set { _lastHighWatermarkAlertTimestamp = value; }
        }
        #endregion
    }

	/// <summary>
	/// Statistics class when processing a set of FilesystemQueue entries.
	/// </summary>
	internal class FilesystemProcessStatistics : StatisticsSet
	{
        #region Constructors
		public FilesystemProcessStatistics(string name)
            : base(name)
        {
        }
        #endregion Constructors
	}

	/// <summary>
	/// Statistics class when processing an individual row from the FilesystemQueue table.
	/// </summary>
	internal class StudyProcessStatistics : StatisticsSet
	{
		#region Constructors
		public StudyProcessStatistics(string name)
            : base(name)
        {
        }
        #endregion Constructors

		public TimeSpanStatistics TotalTime
		{
			get
			{
				if (this["TotalTime"] == null)
					this["TotalTime"] = new TimeSpanStatistics("TotalTime");

				return (this["TotalTime"] as TimeSpanStatistics);
			}
			set { this["TotalTime"] = value; }
		}

		public TimeSpanStatistics StudyStorageTime
		{
			get
			{
				if (this["StudyStorageTime"] == null)
					this["StudyStorageTime"] = new TimeSpanStatistics("StudyStorageTime");

				return (this["StudyStorageTime"] as TimeSpanStatistics);
			}
			set { this["StudyStorageTime"] = value; }
		}

		public TimeSpanStatistics CalculateDirectorySizeTime
		{
			get
			{
				if (this["CalculateDirectorySizeTime"] == null)
					this["CalculateDirectorySizeTime"] = new TimeSpanStatistics("CalculateDirectorySizeTime");

				return (this["CalculateDirectorySizeTime"] as TimeSpanStatistics);
			}
			set { this["CalculateDirectorySizeTime"] = value; }
		}

		public TimeSpanStatistics DbUpdateTime
		{
			get
			{
				if (this["DbUpdateTime"] == null)
					this["DbUpdateTime"] = new TimeSpanStatistics("DbUpdateTime");

				return (this["DbUpdateTime"] as TimeSpanStatistics);
			}
			set { this["DbUpdateTime"] = value; }
		}

		public ulong DirectorySize
		{
			set
			{
				this["DirectorySize"] = new ByteCountStatistics("DirectorySize", value);
			}
			get
			{
				if (this["DirectorySize"] == null)
					this["DirectorySize"] = new ByteCountStatistics("DirectorySize");

				return (this["DirectorySize"] as ByteCountStatistics).Value;
			}
		}	
	}

    /// <summary>
    /// Class for processing 'FilesystemDelete' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    public class FilesystemDeleteItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Members
		private DateTime _scheduledTime = Platform.Time;
        private float _bytesToRemove;
        private int _studiesDeleted;
        private int _studiesMigrated;
		private int _studiesPurged;
        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the 'State' of the filesystem associated with the 'FilesystemDelete' <see cref="ServiceLock"/> item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fs"></param>
        private static void UpdateState(Model.ServiceLock item, ServerFilesystemInfo fs)
        {
            FilesystemState state = null;
            if (item.State != null && item.State.DocumentElement!=null)
            {
                //load from datatabase
                state = XmlUtils.Deserialize<FilesystemState>(item.State.DocumentElement);
            }

            if (state == null)
                state = new FilesystemState();

            if (fs.AboveHighWatermark)
            {
                // we don't want to generate alert if the filesystem is offline or not accessible.
                if (fs.Online && (fs.Readable || fs.Writeable))
                {
                    TimeSpan ALERT_INTERVAL = TimeSpan.FromMinutes(ServiceLockSettings.Default.HighWatermarkAlertInterval);

                    if (state.AboveHighWatermarkTimestamp == null)
                        state.AboveHighWatermarkTimestamp = Platform.Time;

                    TimeSpan elapse = (state.LastHighWatermarkAlertTimestamp != null) ? Platform.Time - state.LastHighWatermarkAlertTimestamp.Value : Platform.Time - state.AboveHighWatermarkTimestamp.Value;

                    if (elapse.Duration() >= ALERT_INTERVAL)
                    {
                        ServerPlatform.Alert(AlertCategory.System, AlertLevel.Warning, "Filesystem",
                                             AlertTypeCodes.LowResources, null, TimeSpan.Zero,
                                             SR.AlertFilesystemAboveHW,
                                             fs.Filesystem.Description,
                                             TimeSpanFormatter.Format(Platform.Time - state.AboveHighWatermarkTimestamp.Value, true));


                        state.LastHighWatermarkAlertTimestamp = Platform.Time;
                    }
                }
                else
                {
                    state.AboveHighWatermarkTimestamp = null;
                    state.LastHighWatermarkAlertTimestamp = null;
                }
            }
            else
            {
                state.AboveHighWatermarkTimestamp = null;
                state.LastHighWatermarkAlertTimestamp = null;
            }


            XmlDocument stateXml = new XmlDocument();
            stateXml.AppendChild(stateXml.ImportNode(XmlUtils.Serialize(state), true));

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
                columns.State = stateXml;

                IServiceLockEntityBroker broker = ctx.GetBroker<IServiceLockEntityBroker>();
                broker.Update(item.GetKey(), columns);
                ctx.Commit();
            }
        }


        /// <summary>
        /// Process StudyDelete Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
        private void ProcessStudyDeleteCandidates(IList<FilesystemQueue> candidateList)
        {
			if (candidateList.Count > 0)
				Platform.Log(LogLevel.Debug, "Scheduling delete study for {0} eligible studies...", candidateList.Count);

			FilesystemProcessStatistics summaryStats = new FilesystemProcessStatistics("FilesystemDeleteInsert");

			foreach (FilesystemQueue queueItem in candidateList)
            {
				if (_bytesToRemove < 0 || CancelPending)
                    return;

				StudyProcessStatistics stats = new StudyProcessStatistics("DeleteStudy");
				stats.TotalTime.Start();

				stats.StudyStorageTime.Start();
                // First, get the StudyStorage locations for the study, and calculate the disk usage.
                StudyStorageLocation location;
				if (!FilesystemMonitor.Instance.GetWritableStudyStorageLocation(queueItem.StudyStorageKey, out location))
					continue;
				stats.StudyStorageTime.End();

				stats.CalculateDirectorySizeTime.Start();
                // Get the disk usage
                float studySize = EstimateFolderSizeFromStudyXml(location);
				stats.CalculateDirectorySizeTime.End();
            	stats.DirectorySize = (ulong) studySize;

				stats.DbUpdateTime.Start();
                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
					ILockStudy lockstudy = update.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters();
					lockParms.StudyStorageKey = location.Key;
					lockParms.QueueStudyStateEnum = QueueStudyStateEnum.DeleteScheduled;
					if (!lockstudy.Execute(lockParms) || !lockParms.Successful)
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study for inserting Delete, skipping study ({0}",
									 location.StudyInstanceUid);
						continue;
					}

					IInsertWorkQueueFromFilesystemQueue insertBroker = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

                    InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
                    insertParms.StudyStorageKey = location.GetKey();
                    insertParms.ServerPartitionKey = location.ServerPartitionKey;
					insertParms.ScheduledTime = _scheduledTime;
                    insertParms.DeleteFilesystemQueue = true;
                	insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.DeleteStudy;
                	insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.DeleteStudy;
				
                    WorkQueue insertItem = insertBroker.FindOne(insertParms);
					if (insertItem == null)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected problem inserting 'StudyDelete' record into WorkQueue for Study {0}", location.StudyInstanceUid);
                    }
                    else
                    {
                        update.Commit();
                        _bytesToRemove -= studySize;
                        _studiesDeleted++;
                    	_scheduledTime = _scheduledTime.AddSeconds(2);
                    }
                }
				stats.DbUpdateTime.End();
				stats.TotalTime.End();

				summaryStats.AddSubStats(stats);
				StatisticsLogger.Log(LogLevel.Debug, stats);
			}
			summaryStats.CalculateAverage();
			StatisticsLogger.Log(LogLevel.Info, false, summaryStats);
        }

		/// <summary>
		/// Process StudyPurge <see cref="FilesystemQueue"/> entries.
		/// </summary>
		/// <param name="candidateList">The list of candidates for purging</param>
		private void ProcessStudyPurgeCandidates(IList<FilesystemQueue> candidateList)
		{
			if (candidateList.Count > 0)
				Platform.Log(LogLevel.Debug, "Scheduling purge study for {0} eligible studies...", candidateList.Count);

			FilesystemProcessStatistics summaryStats = new FilesystemProcessStatistics("FilesystemPurgeInsert");

			foreach (FilesystemQueue queueItem in candidateList)
			{
				if (_bytesToRemove < 0 || CancelPending)
					break;

				StudyProcessStatistics stats = new StudyProcessStatistics("PurgeStudy");
				stats.TotalTime.Start();

				stats.StudyStorageTime.Start();
				// First, get the StudyStorage locations for the study, and calculate the disk usage.
				StudyStorageLocation location;
				if (!FilesystemMonitor.Instance.GetWritableStudyStorageLocation(queueItem.StudyStorageKey, out location))
					continue;
				stats.StudyStorageTime.End();

				stats.CalculateDirectorySizeTime.Start();
				// Get the disk usage
				float studySize = EstimateFolderSizeFromStudyXml(location);
				stats.CalculateDirectorySizeTime.End();
				stats.DirectorySize = (ulong) studySize;

				stats.DbUpdateTime.Start();
				// Update the DB
				using (
					IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					ILockStudy lockstudy = update.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters();
					lockParms.StudyStorageKey = location.Key;
					lockParms.QueueStudyStateEnum = QueueStudyStateEnum.PurgeScheduled;
					if (!lockstudy.Execute(lockParms) || !lockParms.Successful)
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study for inserting Study Purge, skipping study ({0}",
						             location.StudyInstanceUid);
						continue;
					}

                    IInsertWorkQueueFromFilesystemQueue insertBroker = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

					InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters();
					insertParms.StudyStorageKey = location.GetKey();
					insertParms.ServerPartitionKey = location.ServerPartitionKey;
					insertParms.ScheduledTime = _scheduledTime;
					insertParms.DeleteFilesystemQueue = true;
					insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.PurgeStudy;
					insertParms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.PurgeStudy;
					
                    WorkQueue insertItem = insertBroker.FindOne(insertParms);
					if (insertItem == null)
					{
						Platform.Log(LogLevel.Error, "Unexpected problem inserting 'PurgeStudy' record into WorkQueue for Study {0}",
						             location.StudyInstanceUid);
					}
					else
					{
						update.Commit();
						_bytesToRemove -= studySize;
						_studiesPurged++;
						_scheduledTime = _scheduledTime.AddSeconds(2);
					}
					
				}
				stats.DbUpdateTime.End();
				stats.TotalTime.End();

				summaryStats.AddSubStats(stats);
				StatisticsLogger.Log(LogLevel.Debug, stats);
			}
			summaryStats.CalculateAverage();
			StatisticsLogger.Log(LogLevel.Info, false, summaryStats);
		}

    	/// <summary>
        /// Process study migration candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
		private void ProcessStudyMigrateCandidates(IList<FilesystemQueue> candidateList)
        {
        	Platform.CheckForNullReference(candidateList, "candidateList");

        	if (candidateList.Count > 0)
        		Platform.Log(LogLevel.Debug, "Scheduling tier-migration for {0} eligible studies...", candidateList.Count);

			FilesystemProcessStatistics summaryStats = new FilesystemProcessStatistics("FilesystemTierMigrateInsert");
        	foreach (FilesystemQueue queueItem in candidateList)
        	{
				if (_bytesToRemove < 0 || CancelPending)
        		{
                    Platform.Log(LogLevel.Debug, "Estimated disk space has been reached.");
                    break;
        		}
				StudyProcessStatistics stats = new StudyProcessStatistics("TierMigrateStudy");
				stats.TotalTime.Start();

				stats.StudyStorageTime.Start();
        		// First, get the StudyStorage locations for the study, and calculate the disk usage.
				StudyStorageLocation location;
				if (!FilesystemMonitor.Instance.GetWritableStudyStorageLocation(queueItem.StudyStorageKey, out location))
					continue;
				stats.StudyStorageTime.End();

				stats.CalculateDirectorySizeTime.Start();
				// Get the disk usage
				float studySize = EstimateFolderSizeFromStudyXml(location);
				stats.CalculateDirectorySizeTime.End();
        		stats.DirectorySize = (ulong) studySize;

				stats.DbUpdateTime.Start();
        		using (
        			IUpdateContext update =
        				PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        		{
					ILockStudy lockstudy = update.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters
					                                	{
					                                		StudyStorageKey = location.Key,
					                                		QueueStudyStateEnum = QueueStudyStateEnum.MigrationScheduled
					                                	};
        			if (!lockstudy.Execute(lockParms) || !lockParms.Successful)
					{
						Platform.Log(LogLevel.Warn, "Unable to lock study for inserting Tier Migration. Reason:{0}. Skipping study ({1})",
                                     lockParms.FailureReason, location.StudyInstanceUid);
						continue;
					}

					IInsertWorkQueueFromFilesystemQueue broker = update.GetBroker<IInsertWorkQueueFromFilesystemQueue>();

					InsertWorkQueueFromFilesystemQueueParameters insertParms = new InsertWorkQueueFromFilesystemQueueParameters
                                                       	{
                                                       		StudyStorageKey = location.GetKey(),
                                                       		ServerPartitionKey = location.ServerPartitionKey,
                                                       		ScheduledTime = _scheduledTime,
                                                       		DeleteFilesystemQueue = true,
                                                       		WorkQueueTypeEnum = WorkQueueTypeEnum.MigrateStudy,
                                                       		FilesystemQueueTypeEnum =
                                                       			FilesystemQueueTypeEnum.TierMigrate
                                                       	};

        			Platform.Log(LogLevel.Debug, "Scheduling tier-migration for study {0} from {1} at {2}...",
        			             location.StudyInstanceUid, location.FilesystemTierEnum, _scheduledTime);
        			WorkQueue insertItem = broker.FindOne(insertParms);
					if (insertItem == null)
        			{
        				Platform.Log(LogLevel.Error,
        				             "Unexpected problem inserting 'MigrateStudy' record into WorkQueue for Study {0}",
        				             location.StudyInstanceUid);
        			}
        			else
        			{
        				update.Commit();
        				_bytesToRemove -= studySize;
        				_studiesMigrated++;

        				// spread out the scheduled migration entries based on the size
        				// assuming that the larger the study the longer it will take to migrate
        				// The assumed migration speed is arbitarily chosen.
        				double migrationSpeed = ServiceLockSettings.Default.TierMigrationSpeed*1024*1024; // MB / sec
        				TimeSpan estMigrateTime = TimeSpan.FromSeconds(studySize/migrationSpeed);
        				_scheduledTime = _scheduledTime.Add(estMigrateTime);
        			}
        		}
				stats.DbUpdateTime.End();
				stats.TotalTime.End();

        		summaryStats.AddSubStats(stats);
				StatisticsLogger.Log(LogLevel.Debug, stats);
			}

			summaryStats.CalculateAverage();
    		StatisticsLogger.Log(LogLevel.Info, false, summaryStats);
        }

		/// <summary>
		/// Do the actual Study Deletes
		/// </summary>
		/// <param name="item">The ServiceLock item</param>
		/// <param name="fs">The filesystem being worked on.</param>
		private void DoStudyDelete(ServerFilesystemInfo fs, Model.ServiceLock item)
        {
            DateTime deleteTime = Platform.Time;
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.DeleteStudy;

            while (_bytesToRemove > 0)
            {
				Platform.Log(LogLevel.Debug,
					 "{1:0.0} MBs needs to be removed from '{0}'. Querying for studies that can be deleted",
					 fs.Filesystem.Description, _bytesToRemove / (1024 * 1024));
                IList<FilesystemQueue> list =
                    GetFilesystemQueueCandidates(item, deleteTime, type, true);

                if (list.Count > 0)
                {
                    ProcessStudyDeleteCandidates(list);
					if (CancelPending) break;
                }
                else
                {
                    // No candidates
                    break;
                }
            }
        }

		/// <summary>
		/// Do the actual StudyPurge
		/// </summary>
		/// <param name="item"></param>
		/// <param name="fs">The filesystem being worked on.</param>
		private void DoStudyPurge(ServerFilesystemInfo fs, Model.ServiceLock item)
		{
			DateTime deleteTime = Platform.Time;
			FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.PurgeStudy;

			while (_bytesToRemove > 0)
			{
				Platform.Log(LogLevel.Debug,
							 "{1:0.0} MBs needs to be removed from '{0}'. Querying for studies that can be purged",
							 fs.Filesystem.Description, _bytesToRemove / (1024 * 1024));
				IList<FilesystemQueue> list =
					GetFilesystemQueueCandidates(item, deleteTime, type, false);

				if (list.Count > 0)
				{
					ProcessStudyPurgeCandidates(list);
					if (CancelPending) break;
				}
				else
				{
					// No candidates
					break;
				}
			}
		}

		/// <summary>
		/// Do the actual Study migration.
		/// </summary>
		/// <param name="fs">The filesystem</param>
		/// <param name="item">The ServiceLock item being processed.</param>
        private void DoStudyMigrate( ServerFilesystemInfo fs, Model.ServiceLock item)
        {
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.TierMigrate;

            while (_bytesToRemove > 0)
            {
				TimeSpanStatistics queryTime = new TimeSpanStatistics("Query Time");
				queryTime.Start();
                IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, Platform.Time, type, false);
				queryTime.End();
				Platform.Log(LogLevel.Info,
							 "{1:0.0} MBs needs to be removed from '{0}'. Found {2} studies that can be migrated in {3} seconds",
							 fs.Filesystem.Description, _bytesToRemove / (1024 * 1024),
							 list.Count,
							 queryTime.Value.TotalSeconds);
				if (list.Count > 0)
                {
                    ProcessStudyMigrateCandidates(list);
					if (CancelPending) break;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns number of Delete Study, Tier Migrate, and Study Purge work queue items 
        /// that are still Pending or In Progress for the filesystem associated with the
        /// specified <see cref="ServiceLock"/>.
        /// </summary>
        /// <param name="item">The ServiceLock item.</param>
        /// <returns>The number of WorkQueue entries pending.</returns>
        private int CheckWorkQueueCount(Model.ServiceLock item)
        {
			using (ServerExecutionContext context = new ServerExecutionContext())
			{
				IWorkQueueEntityBroker select = context.ReadContext.GetBroker<IWorkQueueEntityBroker>();

				WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

				criteria.WorkQueueTypeEnum.In(new[]
				                              	{
				                              		WorkQueueTypeEnum.DeleteStudy, WorkQueueTypeEnum.MigrateStudy,
				                              		WorkQueueTypeEnum.PurgeStudy
				                              	});

				// Do Pending status, in case there's a Failure status entry, we don't want to 
				// block on that.
				criteria.WorkQueueStatusEnum.In(new[] {WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress});

				FilesystemStudyStorageSelectCriteria filesystemCriteria = new FilesystemStudyStorageSelectCriteria();

				filesystemCriteria.FilesystemKey.EqualTo(item.FilesystemKey);

				criteria.FilesystemStudyStorageRelatedEntityCondition.Exists(filesystemCriteria);
				int count = select.Count(criteria);

				return count;

			}
        }

		private void MigrateStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			ServerFilesystemInfo newFilesystem = FilesystemMonitor.Instance.GetLowerTierFilesystemForStorage(fs);
			if (newFilesystem == null)
			{
				Platform.Log(LogLevel.Warn,
				             "No writable storage in lower tiers. Tier-migration for '{0}' is disabled at this time.",
				             fs.Filesystem.Description);
				return;
			}

			Platform.Log(LogLevel.Info, "Starting Tier Migration from {0}", fs.Filesystem.Description);

			try
			{
				DoStudyMigrate(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when scheduling tier-migration.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for migration from filesystem '{1}'",
			             _studiesMigrated, fs.Filesystem.Description);
		}

    	private void DeleteStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			Platform.Log(LogLevel.Info, "Starting query for Filesystem delete candidates on '{0}'.",
			             fs.Filesystem.Description);
			try
			{
				DoStudyDelete(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for removal from filesystem '{1}'", 
				_studiesDeleted, fs.Filesystem.Description);
		}

		private void PurgeStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
		{
			Platform.Log(LogLevel.Info, "Starting query for Filesystem Purge candidates on '{0}'.",
						 fs.Filesystem.Description);
			try
			{
				DoStudyPurge(fs, item);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
			}

			Platform.Log(LogLevel.Info, "{0} studies have been scheduled for purging from filesystem '{1}'", 
				_studiesPurged, fs.Filesystem.Description);
		}
    	#endregion


        /// <summary>
        /// Main <see cref="ServiceLock"/> processing routine.
        /// </summary>
        /// <param name="item">The <see cref="ServiceLock"/> item to process.</param>
        protected override void OnProcess(Model.ServiceLock item)
        {
            ServiceLockSettings settings = ServiceLockSettings.Default;
            ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

            UpdateState(item, fs);

            DateTime scheduledTime;
            if (!fs.Online)
            {
                Platform.Log(LogLevel.Info, "Filesystem {0} is not online. Watermark is not checked at this point.", fs.Filesystem.Description);
                scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteRecheckDelay);
            }
            else
            {
                _bytesToRemove = FilesystemMonitor.Instance.CheckFilesystemBytesToRemove(item.FilesystemKey);

                if (fs.AboveHighWatermark)
                {
                    int count = CheckWorkQueueCount(item);
                    if (count > 0)
                    {
                        // delay to avoid overshoot

                        Platform.Log(LogLevel.Info,
                                     "Delaying Filesystem ServiceLock check, {0} StudyDelete, StudyPurge or MigrateStudy items still in the WorkQueue for Filesystem: {1} (Current: {2}, High Watermark: {3})",
                                     count, fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);

                        scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteRecheckDelay);
                    }
                    else
                    {
                        Platform.Log(LogLevel.Info, "Filesystem above high watermark: {0} (Current: {1}, High Watermark: {2}",
                                     fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);

                        MigrateStudies(item, fs);

                        if (_bytesToRemove > 0 && !CancelPending)
                            DeleteStudies(item, fs);

                        if (_bytesToRemove > 0 && !CancelPending)
                            PurgeStudies(item, fs);


                        if (_studiesDeleted + _studiesMigrated + _studiesPurged == 0)
                        {
                            Platform.Log(LogLevel.Warn, "Fileystem '{0}' is above high watermark but no studies can be deleted, migrated or purged at this point", fs.Filesystem.Description);
                        }

                        scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteRecheckDelay);
                    }

                }
                else
                {
                    scheduledTime = Platform.Time.AddMinutes(settings.FilesystemDeleteCheckInterval);

                }
            }

            UnlockServiceLock(item, true, scheduledTime);
        }
       


        #region Public Methods
        
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
