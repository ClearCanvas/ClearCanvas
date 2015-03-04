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
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using System.Text;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    public class WorkQueueAlertContextData : DataContractBase
    {
        #region Private Members

        #endregion

        #region Public Properties

        public string WorkQueueItemKey { get; set; }

        public ValidationStudyInfo ValidationStudyInfo { get; set; }

        #endregion
    }


	/// <summary>
	/// Enum telling if a work queue entry had a fatal or nonfatal error.
	/// </summary>
	public enum WorkQueueProcessorFailureType
	{
		Fatal,
		NonFatal
	}

	/// <summary>
	/// Enum for telling when processing is complete for a WorkQueue item.
	/// </summary>
	public enum WorkQueueProcessorStatus
	{
		Complete,
		Pending,
		Idle,
		IdleNoDelete,
        CompleteDelayDelete
	}

	/// <summary>
	/// Flag telling if a database update should be done for a WorkQueue entry.
	/// </summary>
	public enum WorkQueueProcessorDatabaseUpdate
	{
		ResetQueueState,
		None
	}



    /// <summary>
    /// Base class used when implementing WorkQueue item processors.
    /// </summary>
    public abstract class BaseItemProcessor: IWorkQueueItemProcessor
    {
        #region Static Fields
        private static readonly Dictionary<Type, StudyIntegrityValidationModes> ProcessorsValidationSettings = new Dictionary<Type, StudyIntegrityValidationModes>();
        private static readonly Dictionary<Type, RecoveryModes> ProcessorsRecoverySettings = new Dictionary<Type, RecoveryModes>();
        private static readonly object SyncLock = new object();
        #endregion

        #region Private Fields
        private const int MAX_DB_RETRY = 5;
        private const float KB = 1024;
        private string _name = "Work Queue";
        private IReadContext _readContext;
        private TimeSpanStatistics _storageLocationLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _uidsLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _dBUpdateTime = new TimeSpanStatistics();
        private TimeSpanStatistics _studyXmlLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _processTime = new TimeSpanStatistics();
        private Model.WorkQueue _workQueueItem;
        private StudyStorageLocation _storageLocation;
        private IList<WorkQueueUid> _uidList;
        private ServerPartition _partition;
        protected Study _theStudy;
    	private bool _cancelPending;
    	private readonly object _syncRoot = new object();
    	private WorkQueueTypeProperties _workQueueProperties;
        private string _partitionIncomingFolder;
        private bool _partitionIncomingFolderLoaded;
        
        #endregion

        #region Constructors

        #endregion

        #region Protected Properties

        protected StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
			set { _storageLocation = value; }
        }

        protected IList<WorkQueueUid> WorkQueueUidList
        {
            get
            {
                if (_uidList==null)
                {
                    LoadUids(WorkQueueItem);
                }
                return _uidList;
            }
        }

        protected TimeSpanStatistics StorageLocationLoadTime
        {
            get { return _storageLocationLoadTime; }
            set { _storageLocationLoadTime = value; }
        }

        protected TimeSpanStatistics UidsLoadTime
        {
            get { return _uidsLoadTime; }
            set { _uidsLoadTime = value; }
        }

        protected TimeSpanStatistics DBUpdateTime
        {
            get { return _dBUpdateTime; }
            set { _dBUpdateTime = value; }
        }

        protected TimeSpanStatistics StudyXmlLoadTime
        {
            get { return _studyXmlLoadTime; }
            set { _studyXmlLoadTime = value; }
        }

        protected TimeSpanStatistics ProcessTime
        {
            get { return _processTime; }
            set { _processTime = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Model.WorkQueue WorkQueueItem
        {
            get { return _workQueueItem; }
        }

    	protected WorkQueueTypeProperties WorkQueueProperties
    	{
    		get
    		{
    			if (_workQueueProperties == null)
    			{
					using (ServerExecutionContext context = new ServerExecutionContext())
					{
						IWorkQueueTypePropertiesEntityBroker broker = context.ReadContext.GetBroker<IWorkQueueTypePropertiesEntityBroker>();
						WorkQueueTypePropertiesSelectCriteria criteria = new WorkQueueTypePropertiesSelectCriteria();
						criteria.WorkQueueTypeEnum.EqualTo(WorkQueueItem.WorkQueueTypeEnum);
						_workQueueProperties = broker.FindOne(criteria);
					}
    			}
    			return _workQueueProperties;
    		}
    	}

        protected ServerPartition ServerPartition
        {
            get
            {
                if (_workQueueItem == null || _workQueueItem.ServerPartitionKey == null)
                    return null;

                if (_partition==null)
                {
                    _partition = ServerPartitionMonitor.Instance.FindPartition(_workQueueItem.ServerPartitionKey);
                }

                return _partition;
            }
        }

    	protected bool CancelPending
    	{
			get { lock (_syncRoot) return _cancelPending; }
    	}

        protected Study Study
        {
            get
            {
                lock(_syncRoot)
                {
                    _theStudy = WorkQueueItem.Study;
                }
                return _theStudy;
            }
        }

        #endregion

        #region Protected Methods


        /// <summary>
        /// Gets the incoming folder for the <see cref="ServerPartition"/>. This method returns null if
        /// there's no incoming folder (e.g, if the Import Files Service is disabled)
        /// </summary>
        protected string GetServerPartitionIncomingFolder()
        {
            if (!_partitionIncomingFolderLoaded)
            {
                _partitionIncomingFolder = ServerPartition.GetIncomingFolder();
                _partitionIncomingFolderLoaded = true;
            }

            return _partitionIncomingFolder;
        }

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        protected bool LoadWritableStorageLocation(Model.WorkQueue item)
        {
			if (_storageLocation != null)
				return true;
			bool found = false;
            StorageLocationLoadTime.Add(
                delegate
                    {
                    	found = FilesystemMonitor.Instance.GetWritableStudyStorageLocation(item.StudyStorageKey, out _storageLocation);
                    }
                );

        	return found;
        }

		/// <summary>
		/// Load the storage location for the WorkQueue item.
		/// </summary>
		/// <param name="item">The item to load the location for.</param>
		protected bool LoadReadableStorageLocation(Model.WorkQueue item)
		{
			if (_storageLocation != null)
				return true;
			bool found = false;
			StorageLocationLoadTime.Add(
				delegate
				{
					found = FilesystemMonitor.Instance.GetReadableStudyStorageLocation(item.StudyStorageKey, out _storageLocation);
				}
				);

			return found;
		}

        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        protected void LoadUids(Model.WorkQueue item)
        {
            
            if (_uidList==null)
            {
				UidsLoadTime.Add(delegate
				                 	{
				                 		using (ServerExecutionContext context = new ServerExecutionContext())
				                 		{

				                 			IWorkQueueUidEntityBroker select = context.ReadContext.GetBroker<IWorkQueueUidEntityBroker>();

				                 			WorkQueueUidSelectCriteria parms = new WorkQueueUidSelectCriteria();

				                 			parms.WorkQueueKey.EqualTo(item.GetKey());
				                 			_uidList = select.Find(parms);

				                 			_uidList = TruncateList(item, _uidList);
				                 		}
				                 	}
					);
            }
        }


        protected static void Log(LogLevel level, String header, string message)
        {
            Platform.Log(level, String.Format("{0} : {1}", header, message));
        }
        protected static void Log(LogLevel level, String header, string message, params object[] args)
        {
            Platform.Log(level, String.Format("{0} : {1}", header, String.Format(message, args)));
        }

        protected void OnStudyIntegrityFailure(Model.WorkQueue item, string reason)
        {
            Platform.CheckMemberIsSet(Study, "Study");

            RecoveryModes mode = GetProcessorRecoveryMode();
            switch (mode)
            {
                case RecoveryModes.Automatic:
                    try
                    {
                        AutoRecoveryResult result = PerformAutoRecovery(item, reason);
                        if (result.Successful)
                        {
                            if (result.ReprocessWorkQueueEntry != null)
                            {
                                Platform.Log(LogLevel.Info, "Study needs to be reprocessed first. Current {0} entry will resume later.", item.WorkQueueTypeEnum);
                                PostponeItem(String.Format("{0}. Study needs to be reprocessed first.", reason), WorkQueueProcessorFailureType.NonFatal);
                            }
                            else
                            {
                                // auto-recovery without triggering any ReprocessStudy 
                                Platform.Log(LogLevel.Info, "Auto-recovery was successful. Current {0} entry will resume later.", item.WorkQueueTypeEnum);
                                PostponeItem(String.Format("{0}. Auto-recovery was triggered.", reason), WorkQueueProcessorFailureType.NonFatal);
                            }
                        }
                        else
                        {
                            // unable to recover.. fail it now
                            Platform.Log(LogLevel.Error, "Unable to recover from previous failure. Failing the entry.");
                            PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                        }
                    }
                    catch(InvalidStudyStateOperationException ex)
                    {
                        // unable to recover.. fail it now
                        Platform.Log(LogLevel.Error, ex, "Unable to recover from previous failure. Failing the entry.");

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(item.FailureDescription);
                        sb.AppendLine(String.Format("Auto-Recovery failed: {0}", ex.Message));
                        item.FailureDescription = sb.ToString();

                        PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    }
                    catch (Exception ex)
                    {
                        // unable to recover.. fail it now
                        Platform.Log(LogLevel.Error, ex, "Unable to recover from previous failure. Failing the entry.");
                        PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    }

                    break;
                default:
                    PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    break;
            }

        }


        protected void RemoveBadDicomFile(string file, string reason)
        {
            Platform.Log(LogLevel.Error, "Deleting unreadable dicom file: {0}. Reason={1}", file, reason);
            FileUtils.Delete(file);
            RaiseAlert(WorkQueueItem, AlertLevel.Critical, String.Format("Dicom file {0} is unreadable: {1}. It has been removed from the study.", file, reason));
        }

        /// <summary>
        /// Returns the max batch size for a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <returns>The maximum batch size for the <see cref="WorkQueue"/> item</returns>
        /// <param name="listCount">The number of available list items.</param>
        protected int GetMaxBatchSize(Model.WorkQueue item, int listCount)
        {
        	return WorkQueueProperties.MaxBatchSize == -1
        	       	? listCount : WorkQueueProperties.MaxBatchSize;
        }

        /// <summary>
        /// Truncate the SOP Instance Uid list
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <param name="list">The list of <see cref="WorkQueueUid"/> to be truncated, if needed</param>
        /// <return>A truncated list of <see cref="WorkQueueUid"/></return>
        protected IList<WorkQueueUid> TruncateList(Model.WorkQueue item, IList<WorkQueueUid> list)
        {
			if (item != null && list != null)
			{
				int maxSize = GetMaxBatchSize(item, list.Count);
				if (list.Count <= maxSize)
					return list;

				List<WorkQueueUid> newList = new List<WorkQueueUid>();
				foreach (WorkQueueUid uid in list)
				{
					if (!uid.Failed)
						newList.Add(uid);

					if (newList.Count >= maxSize)
						return newList;
				}

				// just return the whole list, they're all going to be skipped anyways!
				if (newList.Count == 0)
					return list;

				return newList;
			}

			return list;
        }

        /// <summary>
        /// Updates the status of a study to a new status
        /// </summary>
		protected virtual void UpdateStudyStatus(StudyStorageLocation theStudyStorage, StudyStatusEnum theStatus, TransferSyntax theSyntax)
        {
        	DBUpdateTime.Add(
        		delegate
        			{
        				using (
        					IUpdateContext updateContext =
        						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        				{
							// Select the Server Transfer Syntax
							ServerTransferSyntaxSelectCriteria syntaxCriteria = new ServerTransferSyntaxSelectCriteria();
							IServerTransferSyntaxEntityBroker syntaxBroker =
								updateContext.GetBroker<IServerTransferSyntaxEntityBroker>();
							syntaxCriteria.Uid.EqualTo(theSyntax.UidString);

							ServerTransferSyntax serverSyntax = syntaxBroker.FindOne(syntaxCriteria);
							if (serverSyntax == null)
							{
								Platform.Log(LogLevel.Error, "Unable to load ServerTransferSyntax for {0}.  Unable to update study status.", theSyntax.Name);
								return;
							}

							// Get the FilesystemStudyStorage update broker ready
        					IFilesystemStudyStorageEntityBroker filesystemQueueBroker =
								updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
							FilesystemStudyStorageUpdateColumns filesystemQueueUpdate = new FilesystemStudyStorageUpdateColumns
							                                                                {
							                                                                    ServerTransferSyntaxKey = serverSyntax.GetKey()
							                                                                };
        				    FilesystemStudyStorageSelectCriteria filesystemQueueCriteria = new FilesystemStudyStorageSelectCriteria();
        					filesystemQueueCriteria.StudyStorageKey.EqualTo(theStudyStorage.GetKey());

							// Get the StudyStorage update broker ready
        					IStudyStorageEntityBroker studyStorageBroker =
        						updateContext.GetBroker<IStudyStorageEntityBroker>();
							StudyStorageUpdateColumns studyStorageUpdate = new StudyStorageUpdateColumns
							                                                   {
							                                                       StudyStatusEnum = theStatus,
							                                                       LastAccessedTime = Platform.Time
							                                                   };

        				    if (!filesystemQueueBroker.Update(filesystemQueueCriteria,filesystemQueueUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update FilesystemQueue row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);
								
							}
							else if (!studyStorageBroker.Update(theStudyStorage.GetKey(),studyStorageUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update StudyStorage row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);								
							}
							else
								updateContext.Commit();
        				}
        			}
        		);
        }

		/// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the followings
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached.</item>
		/// <item>Pending: if the current batch has been processed successfully</item>
		/// <item>Idle : if current batch size = 0.</item>
		/// <item>Completed: if batch size =0 (idle) and the item has expired.</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="status">Indicates if complete.</param>
		/// <param name="resetQueueStudyState">Reset the queue study state back to Idle</param>
		protected virtual void PostProcessing(Model.WorkQueue item, WorkQueueProcessorStatus status, WorkQueueProcessorDatabaseUpdate resetQueueStudyState)
		{
		    Completed = status == WorkQueueProcessorStatus.Complete
		                     || (status == WorkQueueProcessorStatus.Idle && item.ExpirationTime < Platform.Time);
            if (Completed)
            {
                if (WorkQueueSettings.Instance.EnableStudyIntegrityValidation)
                {
                    Platform.Log(LogLevel.Info, "{0} has completed (GUID={1})", item.WorkQueueTypeEnum, item.GetKey().Key);
                    VerifyStudy(StorageLocation);
                }
            }

			DBUpdateTime.Add(
				delegate
				{
					using (
						IUpdateContext updateContext =
							PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
					{
						IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
						UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters
						                                      {
						                                          WorkQueueKey = item.GetKey(),
						                                          StudyStorageKey = item.StudyStorageKey,
						                                          ProcessorID = item.ProcessorID
						                                      };

						DateTime now = Platform.Time;

						if (item.FailureDescription != null)
							parms.FailureDescription = item.FailureDescription;

						DateTime scheduledTime = now.AddSeconds(WorkQueueProperties.ProcessDelaySeconds);


						if (item.ExpirationTime.HasValue && scheduledTime > item.ExpirationTime)
							scheduledTime = item.ExpirationTime.Value;
						
                        if (status == WorkQueueProcessorStatus.CompleteDelayDelete)
                        {
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Idle;
                            parms.FailureCount = item.FailureCount;
                            parms.FailureDescription = "";
                            parms.ScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.DeleteDelaySeconds);
							parms.ExpirationTime = Platform.Time.AddSeconds(WorkQueueProperties.DeleteDelaySeconds);
                            if (resetQueueStudyState == WorkQueueProcessorDatabaseUpdate.ResetQueueState)
                                parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
                        }
                        else if (status == WorkQueueProcessorStatus.Complete
							|| (status == WorkQueueProcessorStatus.Idle && item.ExpirationTime < Platform.Time))
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Completed;
							parms.FailureCount = item.FailureCount;
							parms.ScheduledTime = scheduledTime;
							if (resetQueueStudyState == WorkQueueProcessorDatabaseUpdate.ResetQueueState)
								parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

							parms.ExpirationTime = item.ExpirationTime; // Keep the same
						    Completed = true;
						}
						else if (status == WorkQueueProcessorStatus.Idle
						      || status == WorkQueueProcessorStatus.IdleNoDelete)
						{
							scheduledTime = now.AddSeconds(WorkQueueProperties.DeleteDelaySeconds);
							if (item.ExpirationTime.HasValue && scheduledTime > item.ExpirationTime)
								scheduledTime = item.ExpirationTime.Value;

							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Idle;
							parms.ScheduledTime = scheduledTime;
							parms.ExpirationTime = item.ExpirationTime; // keep the same
							parms.FailureCount = item.FailureCount;
						}
						else
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;

							parms.ExpirationTime = scheduledTime.AddSeconds(WorkQueueProperties.ExpireDelaySeconds);
							parms.ScheduledTime = scheduledTime;
							parms.FailureCount = item.FailureCount;
						}


						if (false == update.Execute(parms))
						{
							Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue Key: {1}", item.WorkQueueTypeEnum, item.Key.ToString());
						}
						else
							updateContext.Commit();
					}
				}
				);

		}

        /// <summary>
        /// Gets or sets a boolean value indicating whether the Work Queue Item is completed.
        /// </summary>
        protected bool Completed
        {
            get; set;
        }

        
        /// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the following
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached or a fatal error.</item>
		/// <item>Pending: if the number of retries has not been reached</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="processorFailureType">The failure is unrecoverable</param>
		protected virtual void PostProcessingFailure(Model.WorkQueue item, WorkQueueProcessorFailureType processorFailureType)
		{
    	    int retryCount = 0;
            while (true)
            {
                try
                {
                    #region Fail the entry

			DBUpdateTime.Add(
				delegate
					{
						using (
							IUpdateContext updateContext =
								PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
						{
							IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
							UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters
							                                      {
							                                          WorkQueueKey = item.GetKey(),
							                                          StudyStorageKey = item.StudyStorageKey,
							                                          ProcessorID = item.ProcessorID
							                                      };

						    if (item.FailureDescription != null)
								parms.FailureDescription = item.FailureDescription;

							parms.FailureCount = item.FailureCount + 1;
							if (processorFailureType == WorkQueueProcessorFailureType.Fatal)
							{
								Platform.Log(LogLevel.Error,
											 "Failing {0} WorkQueue entry ({1}), fatal error: {2}",
											 item.WorkQueueTypeEnum, item.GetKey(), item.FailureDescription);

								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now		

                                RaiseAlert(item, AlertLevel.Error, String.Format("Failing {0} WorkQueue entry ({1}), fatal error: {2}", item.WorkQueueTypeEnum, item.GetKey(), item.FailureDescription));
							}
							else if ((item.FailureCount + 1) > WorkQueueProperties.MaxFailureCount)
							{
								Platform.Log(LogLevel.Error,
                                             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
											 item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, item.FailureDescription);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now


                                RaiseAlert(item, AlertLevel.Error, String.Format("Failing {0} WorkQueue entry ({1}): {2}", item.WorkQueueTypeEnum, item.GetKey(), item.FailureDescription));
							}
							else
							{
								Platform.Log(LogLevel.Error,
                                             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}.",
								             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
								parms.ScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.FailureDelaySeconds);
								parms.ExpirationTime =
									   Platform.Time.AddSeconds((WorkQueueProperties.MaxFailureCount - item.FailureCount) *
									                        WorkQueueProperties.FailureDelaySeconds);
							}


							if (false == update.Execute(parms))
							{
								Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}",
											 item.WorkQueueTypeEnum, item.GetKey().ToString());
							}
							else
								updateContext.Commit();
						}
					}
				);

                    break; // done
                    #endregion
		}
                catch (Exception ex)
                {
                    if (ex is PersistenceException || ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex, "Error occurred when calling PostProcessingFailure. Max db retry count has been reached.");
                            
                            // can't do anything except throwing it.
                            throw;
                        }

                        Platform.Log(LogLevel.Error, ex, "Error occurred when calling PostProcessingFailure(). Retry later. GUID={0}", item.Key);
                        SleepForRetry();

                        // If service is stoping then stop
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Stop is requested. Attempt to fail WorkQueue entry is now terminated.");
                            break;
                        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }
        
		}

        /// <summary>
        /// Put the workqueue thread to sleep for 2-3 minutes.
        /// </summary>
        /// <remarks>
        /// This method does not return until 2-3 minutes later or if the service is stoppping.
        /// </remarks>
        private void SleepForRetry()
        {
            DateTime start = Platform.Time;
            Random rand = new Random();
            while(!CancelPending)
            {
                // sleep, wake up every 1-3 sec and check if the service is stopping
                Thread.Sleep(rand.Next(1000,3000));
                if (CancelPending)
                {
                    break;
                }

                // Sleep for 2-3 minutes
                DateTime now = Platform.Time;
                if (now - start > TimeSpan.FromMinutes(rand.Next(2, 3)))
                    break;
            }
        }


        /// <summary>
        /// Simple routine for abort (fail) a work queue item immediately.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        /// <param name="generateAlert"></param>
        protected virtual void AbortQueueItem(Model.WorkQueue item, string failureDescription, bool generateAlert)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    int count = retryCount;
                    DBUpdateTime.Add(
                        delegate
                        {
                            #region Fail the WorkQueue entry
                            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                            {
                                if (count>0)
                                    Platform.Log(LogLevel.Error, "Abort {0} WorkQueue entry ({1}). Retry # {2}. Reason: {3}", item.WorkQueueTypeEnum, item.GetKey(), count, failureDescription);
                                else
                                    Platform.Log(LogLevel.Error, "Abort {0} WorkQueue entry ({1}). Reason: {2}", item.WorkQueueTypeEnum, item.GetKey(), failureDescription);
                                IUpdateWorkQueue broker = updateContext.GetBroker<IUpdateWorkQueue>();
                                UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters
                                                                      {
                                                                          ProcessorID = ServerPlatform.ProcessorId,
                                                                          WorkQueueKey = item.GetKey(),
                                                                          StudyStorageKey = item.StudyStorageKey,
                                                                          FailureCount = item.FailureCount + 1,
                                                                          FailureDescription = failureDescription,
                                                                          WorkQueueStatusEnum = WorkQueueStatusEnum.Failed,
                                                                          ScheduledTime = Platform.Time,
                                                                          ExpirationTime = Platform.Time.AddDays(1)
                                                                      };

                                if (false == broker.Execute(parms))
                                {
                                    Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum, item.GetKey().ToString());
                                }
                                else
                                {
                                    updateContext.Commit();
                                }
                            }
                            #endregion
                        });
                    break; // done
                }
                catch (Exception ex)
                {

                    if (ex is PersistenceException || ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex, "Error occurred when calling AbortQueueItem. Max db retry count has been reached.");
                            throw;
                        }

                        Platform.Log(LogLevel.Error, ex, "Error occurred when calling AbortQueueItem. Retry later. GUID={0}", item.Key);
                        SleepForRetry();

                        // Service is stoping
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Stop is requested. Attempt to abort WorkQueue entry is now terminated.");
                            break;
                        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }

        }


        /// <summary>
        /// Routine for failing a work queue uid record.
        /// </summary>
        /// <param name="uid">The WorkQueueUid record to fail.</param>
        /// <param name="retry">A boolean value indicating whether a retry will be attempted later.</param>
        protected void FailWorkQueueUid(WorkQueueUid uid, bool retry)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker uidUpdateBroker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();
                if (!retry)
                    columns.Failed = true;
                else
                {
                    if (uid.FailureCount >= ImageServerCommonConfiguration.WorkQueueMaxFailureCount)
                    {
                        columns.Failed = true;
                    }
                    else
                    {
                        columns.FailureCount = ++uid.FailureCount;
                    }
                }

                if (uidUpdateBroker.Update(uid.GetKey(), columns))
                    updateContext.Commit();
                else
                    throw new ApplicationException(String.Format("FailUid(): Unable to update work queue uid {0}", uid.Key));
            }
            
        }
        
        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected virtual void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
            DBUpdateTime.Add(
                delegate
                    {
                            #region Remove the WorkQueue entry
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

                            var settings = WorkQueueSettings.Instance;
                            if ((item.FailureCount + 1) > WorkQueueProperties.MaxFailureCount)
                            {
                                Platform.Log(LogLevel.Error,
                                             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
                                                 item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1,
                                                 failureDescription);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                                parms.ScheduledTime = Platform.Time;
                                parms.ExpirationTime = Platform.Time.AddDays(1);

                                RaiseAlert(item, AlertLevel.Error, String.Format("Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
                                                         item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, failureDescription));
                            }
                            else
                            {
                                Platform.Log(LogLevel.Error,
                                             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
                                             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                                    parms.ScheduledTime =
                                        Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                                parms.ExpirationTime =
                                        Platform.Time.AddSeconds((WorkQueueProperties.MaxFailureCount -
                                                                  item.FailureCount) *
															 WorkQueueProperties.FailureDelaySeconds);
                            }

                            if (false == update.Execute(parms))
                            {
                                    Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}",
                                                 item.WorkQueueTypeEnum,
                                             item.GetKey().ToString());
                            }
                            else
                                updateContext.Commit();
                            } 
                            #endregion
                        });
                    break; // done
                        }
                catch (Exception ex)
                {
                    
                    if (ex is PersistenceException || ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex, "Error occurred when calling FailQueueItem. Max db retry count has been reached.");
                            throw;
                    }

                        Platform.Log(LogLevel.Error, ex, "Error occurred when calling FailQueueItem. Retry later. GUID={0}", item.Key);
                        SleepForRetry();

                        // Service is stoping
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Stop is requested. Attempt to fail WorkQueue entry is now terminated.");
                            break;
        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }
            
        }


    	/// <summary>
        /// Delete an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to delete.</param>
        protected virtual void DeleteWorkQueueUid(WorkQueueUid sop)
        {
            // Must retry in case of db error.
            // Failure to do so may lead to orphaned WorkQueueUid and FileNotFoundException 
            // when the work queue is reset.
    	    int retryCount = 0;
            while (true)
            {
                try
                {
                    TimeSpanStatistics time = TimeSpanStatisticsHelper.Measure(
                        delegate
                            {
                                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                                {
                                    IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                                    delete.Delete(sop.GetKey());
                                    updateContext.Commit();
                                }
                        });

                    DBUpdateTime.Add(time);
                    break; // done
        }
                catch (Exception ex)
                {
                    if (ex is PersistenceException || ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex, "Error occurred when calling DeleteWorkQueueUid. Max db retry count has been reached.");
                            WorkQueueItem.FailureDescription = String.Format("Error occurred when calling DeleteWorkQueueUid. Max db retry count has been reached.");
                            PostProcessingFailure(WorkQueueItem, WorkQueueProcessorFailureType.Fatal);
                            return;
                        }

                        Platform.Log(LogLevel.Error, ex, "Error occurred when calling DeleteWorkQueueUid(). Retry later. SOPUID={0}", sop.SopInstanceUid);
                        SleepForRetry();


                        // Service is stoping
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Termination Requested. DeleteWorkQueueUid() is now terminated.");
                            break;
                        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }

        }

        /// <summary>
        /// Update an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <remarks>
        /// Note that just the Duplicate, Failed, FailureCount, and Extension columns are updated from the
        /// input parameter <paramref name="sop"/>.
        /// </remarks>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to update.</param>
        protected virtual void UpdateWorkQueueUid(WorkQueueUid sop)
        {
            DBUpdateTime.Add(
                TimeSpanStatisticsHelper.Measure(
                delegate
                {
                     using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                     {
                         IWorkQueueUidEntityBroker update = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                         WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns
                                                                 {
                                                                     Duplicate = sop.Duplicate,
                                                                     Failed = sop.Failed,
                                                                     FailureCount = sop.FailureCount
                                                                 };

                         if (sop.Extension != null)
                             columns.Extension = sop.Extension;

                         update.Update(sop.GetKey(), columns);

                         updateContext.Commit();
                     }
                }));

            
        }

        /// <summary>
        /// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="location">The location a study is stored.</param>
        /// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
        protected virtual StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            var theXml = new StudyXml();

            StudyXmlLoadTime.Add(
                delegate
                    {
                        String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
                        if (File.Exists(streamFile))
                        {
                            using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                            {
                                var theMemento = new StudyXmlMemento();

                                StudyXmlIo.Read(theMemento, fileStream);

                                theXml.SetMemento(theMemento);

                                fileStream.Close();
                            }
                        }
                    }
                );

           return theXml;
        }


        private void PostponeItem(string reason, WorkQueueProcessorFailureType errorType)
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.PostponeDelaySeconds);
            DateTime expireTime = newScheduledTime.Add(TimeSpan.FromMinutes(2));
            PostponeItem(newScheduledTime, expireTime, reason, errorType);
        }

        protected void PostponeItem(string reasonText)
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkQueueProperties.PostponeDelaySeconds);
            DateTime expireTime = newScheduledTime.Add(TimeSpan.FromMinutes(2));
            PostponeItem(newScheduledTime, expireTime, reasonText, null);
        }

        protected void PostponeItem(DateTime newScheduledTime, DateTime expireTime, string reason)
        {
            PostponeItem(newScheduledTime, expireTime, reason, null);
        }

        protected void PostponeItem(DateTime newScheduledTime, DateTime expireTime, string postponeReason, WorkQueueProcessorFailureType? errorType)
        {
            Model.WorkQueue item = WorkQueueItem;
            DBUpdateTime.Add(
               delegate
               {
                   string stuckReason;
                   bool updatedBefore = item.LastUpdatedTime > DateTime.MinValue;

                   if (updatedBefore && AppearsStuck(item, out stuckReason))
                   {
                       string reason = String.IsNullOrEmpty(stuckReason)
                                          ? String.Format("Aborted because {0}", postponeReason)
                                          : String.Format("Aborted because {0}. {1}", postponeReason, stuckReason);
                       AbortQueueItem(item, reason, true);
                   }
                   else
                   {
                       InternalPostponeWorkQueue(item, newScheduledTime, expireTime, postponeReason, !updatedBefore, errorType);
                   }
               }
               );
        }

        private static bool AppearsStuck(Model.WorkQueue item, out string reason)
        {
            IList<Model.WorkQueue> allItems = WorkQueueHelper.FindWorkQueueEntries(item.StudyStorageKey, null);
            bool updatedBefore = item.LastUpdatedTime > DateTime.MinValue;
            reason = null;

            if (allItems.Count == 1 /* this is the only entry*/)
            {
                if (updatedBefore)
                {
                    if (item.LastUpdatedTime < Platform.Time - Settings.Default.InactiveWorkQueueMinTime)
                    {
                        reason = String.Format("This entry has not been updated since {0}", item.LastUpdatedTime);
                        return true;
                    }
                }
            }
            else
            {
                foreach (Model.WorkQueue anotherItem in allItems)
                {
                    if (anotherItem.Key.Equals(item.Key)) continue;

                    if (!WorkQueueHelper.IsActiveWorkQueue(anotherItem))
                    {
                        reason = "Another work queue entry for the same study appears stuck.";
                        return true;
                    }
                }

                // none of the other entries are active. Either they are all stuck or failed.
                // This entry is considered stuck if it hasn't been updated for long time.
                if (updatedBefore && item.LastUpdatedTime < Platform.Time - Settings.Default.InactiveWorkQueueMinTime)
                {
                    reason = String.Format("This entry has not been updated for since {0}", item.LastUpdatedTime);
                    return true;
                }
            }

            return false;
        }
        private void InternalPostponeWorkQueue(Model.WorkQueue item, DateTime newScheduledTime, DateTime expireTime, string reasonText,
            bool updateWorkQueueEntry, WorkQueueProcessorFailureType? errorType)
        {
            if (errorType!=null)
            {
                Platform.Log(LogLevel.Info, "Postpone {0} entry until {1}: {2}. [GUID={3}.] (This transaction is treated as a failure)", 
                            item.WorkQueueTypeEnum, newScheduledTime, reasonText, item.GetKey());
                item.FailureDescription = reasonText;
                PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
                return;
            }

            Platform.Log(LogLevel.Info, "Postpone {0} entry until {1}: {2}. [GUID={3}]", item.WorkQueueTypeEnum, newScheduledTime, reasonText, item.GetKey());

            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IPostponeWorkQueue broker = updateContext.GetBroker<IPostponeWorkQueue>();


                PostponeWorkQueueParameters parameters = new PostponeWorkQueueParameters
                                                             {
                                                                 WorkQueueKey = item.Key,
                                                                 Reason = reasonText,
                                                                 ScheduledTime = newScheduledTime,
                                                                 ExpirationTime = expireTime,
                                                                 UpdateWorkQueue = updateWorkQueueEntry
                                                             };

                if (broker.Execute(parameters) == false)
                {
                    Platform.Log(LogLevel.Error, "Unable to reschedule {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum, item.GetKey().ToString());
                }
                else
                {
                    updateContext.Commit();
                }
            }
        }

        /// <summary>
        /// Returns a list of related <see cref="Model.WorkQueue"/> with specified types and status (both are optional).
        /// and related to the given <see cref="Model.WorkQueue"/> 
        /// </summary>
        /// <param name="workQueueItem"></param>
        /// <param name="types"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected static IList<Model.WorkQueue> FindRelatedWorkQueueItems(Model.WorkQueue workQueueItem,
            IEnumerable<WorkQueueTypeEnum> types, IEnumerable<WorkQueueStatusEnum> status)
        {
            IList<Model.WorkQueue> list = WorkQueueHelper.FindWorkQueueEntries(workQueueItem.StudyStorageKey, null);

            if (list==null)
                return null;

            // remove the current item 
            CollectionUtils.Remove(list, item => item.Key.Equals(workQueueItem.Key));

            // Remove items if the type is in not the list
            if (types != null)
            {
                list = CollectionUtils.Select(list,
                                              item =>
                                              CollectionUtils.Contains(types, t => t.Equals(item.WorkQueueTypeEnum)));
            }

            // Remove items if the type is in the list
            if (status != null)
            {
                list = CollectionUtils.Select(list,
                                              item =>
                                              CollectionUtils.Contains(status,
                                                                       s => s.Equals(item.WorkQueueStatusEnum)));
            }

            return list;
        }

        protected static WorkQueueAlertContextData GetWorkQueueContextData(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            var contextData = new WorkQueueAlertContextData
                                                        {
                                                            WorkQueueItemKey = item.Key.ToString()
                                                        };

            StudyStorage storage = StudyStorage.Load(ServerExecutionContext.Current.PersistenceContext, item.StudyStorageKey);
            IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(storage);

            if (locations != null && locations.Count > 0)
            {
                StudyStorageLocation location = locations[0];
                if (location != null)
                {
                    contextData.ValidationStudyInfo = new ValidationStudyInfo
                                                      	{
                                                      		StudyInstaneUid = location.StudyInstanceUid
                                                      	};

                	// study info is not always available (eg, when all images failed to process)
                    if (location.Study != null)
                    {
                        contextData.ValidationStudyInfo.AccessionNumber = location.Study.AccessionNumber;
                        contextData.ValidationStudyInfo.PatientsId = location.Study.PatientId;
                        contextData.ValidationStudyInfo.PatientsName = location.Study.PatientsName;
                        contextData.ValidationStudyInfo.ServerAE = location.ServerPartition.AeTitle;
                        contextData.ValidationStudyInfo.StudyDate = location.Study.StudyDate;
                    }
                }
            }

            return contextData;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        protected static AutoRecoveryResult PerformAutoRecovery(Model.WorkQueue item, string reason)
        {
            AutoRecoveryResult result;

            //Note: need to reload the storage location because it may have changed after the processing (eg, tier migration)
            IList<StudyStorageLocation> storageLocations = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(item.StudyStorageKey));
            // storageLocations cannot be null for this operation
            Platform.CheckForNullReference(storageLocations, "storageLocations");
            Platform.CheckTrue(storageLocations.Count >= 1, "storageLocations.Count>=1");

            StudyStorageLocation storageLocation = storageLocations[0];
            Study study = storageLocation.LoadStudy(ServerExecutionContext.Current.PersistenceContext);

            Platform.Log(LogLevel.Info, "{4} failed. Reason:{5}. Attempting to perform auto-recovery for Study:{0}, A#:{1}, Patient:{2}, ID:{3}",
                         study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId, item.WorkQueueTypeEnum.ToString(), reason);

            StudyXml studyXml = storageLocation.LoadStudyXml();
            Platform.CheckForNullReference(studyXml, "studyXml does not exist");
            
            // Do a secondary check on the filesystem vs. the Study Xml.  
            // If these match, then the DB is just update to reflect the valid counts.  
            // If they don't match, a Reprocess entry is inserted into the WorkQueue.
            String studyFolder = storageLocation.GetStudyPath();
            long fileCounter = DirectoryUtility.Count(studyFolder, "*.dcm", true, null);

            // cache these values to avoid looping again
            int numStudyRelatedSeriesInXml = studyXml.NumberOfStudyRelatedSeries;
            int numStudyRelatedInstancesInXml = studyXml.NumberOfStudyRelatedInstances;

            if (fileCounter != numStudyRelatedInstancesInXml)
            {
                // reprocess the study
                Log(LogLevel.Info, "AUTO-RECOVERY", "# of study related instances in study Xml ({0}) appears incorrect. Study needs to be reprocessed.", numStudyRelatedInstancesInXml);
                StudyReprocessor reprocessor = new StudyReprocessor();
                String reprocessReason = String.Format("Auto-recovery from {0}. {1}", item.WorkQueueTypeEnum, reason);
                Model.WorkQueue reprocessEntry = reprocessor.ReprocessStudy(reprocessReason, storageLocation, Platform.Time);

                result = new AutoRecoveryResult
                                                {
                                                    Successful = reprocessEntry!=null, 
                                                    ReprocessWorkQueueEntry=reprocessEntry
                                                };
                return result;
            }

        	Log(LogLevel.Info, "AUTO-RECOVERY", "# of study related instances in study Xml ({0}) appears correct. Update database based on study xml", numStudyRelatedInstancesInXml);
        	// update the counts in db to match the study xml
        	// Update count for each series 
        	IDictionary<string, Series> seriesList = storageLocation.Study.Series;


        	foreach (Series series in seriesList.Values)
        	{
        		SeriesXml seriesXml = studyXml[series.SeriesInstanceUid];
        		if (seriesXml != null)
        		{
        			int numInstancesInSeriesXml = seriesXml.NumberOfSeriesRelatedInstances;
        			if (numInstancesInSeriesXml != series.NumberOfSeriesRelatedInstances)
        			{
        				// Update the count in the series table. Assuming the stored procedure
        				// will also update the count in the Study/Patient tables based on 
        				// the new value.
        				Log(LogLevel.Info, "AUTO-RECOVERY", "Update series in the db. UID:{0}, #Instance:{1}==>{2}", series.SeriesInstanceUid, series.NumberOfSeriesRelatedInstances, numInstancesInSeriesXml);
        				using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        				{
        					ISetSeriesRelatedInstanceCount broker = updateContext.GetBroker<ISetSeriesRelatedInstanceCount>();
        					SetSeriesRelatedInstanceCountParameters criteria = new SetSeriesRelatedInstanceCountParameters(storageLocation.GetKey(), series.SeriesInstanceUid)
        					                                                       {
        					                                                           SeriesRelatedInstanceCount = numInstancesInSeriesXml
        					                                                       };
        				    if (!broker.Execute(criteria))
        					{
        						throw new ApplicationException("Unable to update series related instance count in db");
        					}
        					updateContext.Commit();
        				}
        			}
        		}
        		else
        		{
        			// TODO: series in the db doesn't exist in the xml... the series was deleted from the study?
        			// For now we just reprocess the study. Can we delete the series? If so, we need to take care of the counts.
        			Log(LogLevel.Info, "AUTO-RECOVERY", "Found series in the db which does not exist in the study xml. Force to reprocess the study.");
        			StudyReprocessor reprocessor = new StudyReprocessor();
        			String reprocessReason = String.Format("Auto-recovery from {0}. {1}", item.WorkQueueTypeEnum, reason);
        			Model.WorkQueue reprocessEntry = reprocessor.ReprocessStudy(reprocessReason, storageLocation, Platform.Time);
                    result = new AutoRecoveryResult
                    {
                        Successful = reprocessEntry != null,
                        ReprocessWorkQueueEntry = reprocessEntry
                    };

                    return result;
        		}
        	}

        	if (numStudyRelatedSeriesInXml != storageLocation.Study.NumberOfStudyRelatedSeries ||
        	    numStudyRelatedInstancesInXml != storageLocation.Study.NumberOfStudyRelatedInstances)
        	{
        		Log(LogLevel.Info, "AUTO-RECOVERY", "Updating study related series and instance counts in the db. #Series: {0} ==> {1}.  #Instances: {2}==>{3}",
        		    storageLocation.Study.NumberOfStudyRelatedSeries, numStudyRelatedSeriesInXml,
        		    storageLocation.Study.NumberOfStudyRelatedInstances, numStudyRelatedInstancesInXml);

        		using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        		{
        			var broker = updateContext.GetBroker<ISetStudyRelatedInstanceCount>();
        			var criteria = new SetStudyRelatedInstanceCountParameters(storageLocation.GetKey())
        			                                                      {
        			                                                          StudyRelatedSeriesCount = numStudyRelatedSeriesInXml,
        			                                                          StudyRelatedInstanceCount = numStudyRelatedInstancesInXml
        			                                                      };
        		    if (!broker.Execute(criteria))
        			{
        				throw new ApplicationException("Unable to update study related instance count in db");
        			}
        		
                    updateContext.Commit();
        		}
        	}

        	Log(LogLevel.Info, "AUTO-RECOVERY", "Object count in the db have been corrected.");
            
            result = new AutoRecoveryResult
            {
                Successful = true
            };

            return result;
        }

        

        /// <summary>
        /// Called by the base before <see cref="ProcessItem"/> is invoked to determine 
        /// if the process can begin.
        /// </summary>
        /// <returns>True if the processing can begin. False otherwise.</returns>
        /// <remarks>
        /// </remarks>
        protected abstract bool CanStart();

        /// <summary>
        /// Called by the base to initialize the processor.
        /// </summary>
        protected virtual bool Initialize(Model.WorkQueue item, out string failureDescription)
        {
        	failureDescription = string.Empty;
        	return true;
        }

        /// <summary>
        /// Called before the <see cref="WorkQueue"/> item is processed
        /// </summary>
        /// <param name="item">The work queue item to be processed.</param>
        protected virtual void OnProcessItemBegin(Model.WorkQueue item)
        {
            //NOOP
        }

        /// <summary>
        /// Called after the <see cref="WorkQueue"/> item has been processed
        /// </summary>
        /// <param name="item">The work queue item which has been processed.</param>
        protected virtual void OnProcessItemEnd(Model.WorkQueue item)
        {
            // Update the study size
            
            if (Completed)
            {
                Study theStudy = Study ?? Study.Find(ServerExecutionContext.Current.ReadContext, item.StudyStorageKey);
                if (theStudy!=null)
                {
					if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.MigrateStudy))
						StorageLocation = CollectionUtils.FirstElement<StudyStorageLocation>( StudyStorageLocation.FindStorageLocations(item.ServerPartitionKey,theStudy.StudyInstanceUid),null);
                    if (File.Exists(StorageLocation.GetStudyXmlPath()))
                    {
                        StudyXml studyXml = StorageLocation.LoadStudyXml(true /* reload, in case it's changed */);
                        var size = (decimal) (studyXml.GetStudySize()/KB);

                        // only update if it's out-of-date
                        if (theStudy.StudySizeInKB != size)
                        {
                            using (
                                IUpdateContext ctx =
                                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(
                                        UpdateContextSyncMode.Flush))
                            {
                                var broker = ctx.GetBroker<IStudyEntityBroker>();
                                var parameters = new StudyUpdateColumns {StudySizeInKB = size};
                                if (broker.Update(theStudy.Key, parameters))
                                    ctx.Commit();
                            }
                        }
                    }
                }
            }
        }

        protected abstract void ProcessItem(Model.WorkQueue item);

		public void RaiseAlert(Model.WorkQueue queueItem, AlertLevel level, string message)
		{
			if (WorkQueueProperties.AlertFailedWorkQueue || level == AlertLevel.Critical)
			{
				ServerPlatform.Alert(AlertCategory.Application, level,
									 queueItem.WorkQueueTypeEnum.ToString(), AlertTypeCodes.UnableToProcess,
									 GetWorkQueueContextData(queueItem), TimeSpan.Zero,
									 "Work Queue item failed: Type={0}, GUID={1}: {2}",
									 queueItem.WorkQueueTypeEnum,
									 queueItem.GetKey(), message);
			}
		}

        #endregion

		public void Cancel()
		{
			lock (_syncRoot)
				_cancelPending = true;
		}

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (_readContext!=null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }

        public void Process(Model.WorkQueue item)
        {
            _workQueueItem = item;

            using (new WorkQueueProcessorContext(item))
            {
            	string failureDescription;
                if (!Initialize(item, out failureDescription))
                {
                	PostponeItem(failureDescription);
                	return;
                }

                if (!LoadWritableStorageLocation(item))
                {
                    PostponeItem("Unable to find writeable StorageLocation.");
                    return;
                }

                if (StorageLocation.QueueStudyStateEnum == QueueStudyStateEnum.ReprocessScheduled && !item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReprocessStudy))
                {
                    //TODO: Should we check if the state is correct (ie, there's actually a ReprocessStudy work queue entry)?
                    PostponeItem("Study is scheduled for reprocess");
                    return;
                }

                if (CanStart())
                {
                    try
                    {
                        OnProcessItemBegin(item);
                        ProcessTime.Start();
                        ProcessItem(item);
                        ProcessTime.End();
                    }
                    catch(StudyIntegrityValidationFailure ex)
                    {
                        item.FailureDescription = ex.Message;
                        OnStudyIntegrityFailure(WorkQueueItem, ex.Message);
                    }
                    finally
                    {
                        OnProcessItemEnd(item);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private RecoveryModes GetProcessorRecoveryMode()
        {
            lock (SyncLock)
            {
                RecoveryModes mode;

                if (!ProcessorsRecoverySettings.TryGetValue(GetType(), out mode))
                {
                    object[] attributes = GetType().GetCustomAttributes(typeof (StudyIntegrityValidationAttribute),
                                                                        true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        var att = attributes[0] as StudyIntegrityValidationAttribute;
                        if (!ProcessorsRecoverySettings.ContainsKey(GetType()))
                        {
                            ProcessorsRecoverySettings.Add(GetType(), att.Recovery);
                        }

                        return att.Recovery;
                    }
                }

                return mode;
            }
        }

        protected virtual StudyIntegrityValidationModes GetValidationMode()
        {
            lock (SyncLock)
            {
                StudyIntegrityValidationModes validationModes;
                if (!ProcessorsValidationSettings.TryGetValue(GetType(), out validationModes))
                {
                    object[] attributes = GetType().GetCustomAttributes(typeof (StudyIntegrityValidationAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        var att = attributes[0] as StudyIntegrityValidationAttribute;
                        ProcessorsValidationSettings.Add(GetType(), att.ValidationTypes);
                        return att.ValidationTypes;
                    }
                }

                return validationModes;
            }
        }

        private void VerifyStudy(StudyStorageLocation studyStorage)
        {
            Platform.CheckForNullReference(studyStorage, "studyStorage");

            // Only verify if the Study record exists
            // Explicitly load the study entry again, if a deletion happened, the counts will not be accurate.
            if (studyStorage.Study != null)
            {
                StudyIntegrityValidationModes mode = GetValidationMode();
                if (mode != StudyIntegrityValidationModes.None)
                {
                    Platform.Log(LogLevel.Info, "Verifying study {0}", studyStorage.StudyInstanceUid);

                    var validator = new StudyIntegrityValidator();
                    validator.ValidateStudyState(WorkQueueItem.WorkQueueTypeEnum.ToString(), studyStorage, mode);

                    Platform.Log(LogLevel.Info, "Study {0} has been verified", studyStorage.StudyInstanceUid);
                }
            }
        }

        #endregion

    }

    public class AutoRecoveryResult
    {
        public bool Successful { get; set; }
        public Model.WorkQueue ReprocessWorkQueueEntry { get; set; }
    }
}
