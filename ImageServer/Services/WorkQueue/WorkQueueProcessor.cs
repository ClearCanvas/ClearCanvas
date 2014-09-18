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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Engine for acquiring WorkQueue items and finding plugins to process them.
    /// </summary>
    sealed public class WorkQueueProcessor
    {
        #region Members

        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory> _extensions = new Dictionary<WorkQueueTypeEnum, IWorkQueueProcessorFactory>();
		private readonly WorkQueueThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly ManualResetEvent _terminateEvent;
        private bool _stop;
		private readonly Dictionary<WorkQueueTypeEnum, WorkQueueTypeProperties> _propertiesDictionary = new Dictionary<WorkQueueTypeEnum,WorkQueueTypeProperties>();

        #endregion

		#region Constructor
		public WorkQueueProcessor(int numberThreads, ManualResetEvent terminateEvent, string name)
        {
            _terminateEvent = terminateEvent;
            _threadStop = new ManualResetEvent(false);

			using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IWorkQueueTypePropertiesEntityBroker broker = ctx.GetBroker<IWorkQueueTypePropertiesEntityBroker>();
				WorkQueueTypePropertiesSelectCriteria criteria = new WorkQueueTypePropertiesSelectCriteria();

				IList<WorkQueueTypeProperties> propertiesList = broker.Find(criteria);
				foreach (WorkQueueTypeProperties prop in propertiesList)
					_propertiesDictionary.Add(prop.WorkQueueTypeEnum, prop);
			}


			_threadPool =
				new WorkQueueThreadPool(numberThreads,
				                        WorkQueueSettings.Instance.PriorityWorkQueueThreadCount,
				                        WorkQueueSettings.Instance.MemoryLimitedWorkQueueThreadCount,
				                        _propertiesDictionary) {ThreadPoolName = name + " Pool"};


			WorkQueueFactoryExtensionPoint ep = new WorkQueueFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Warn, "No WorkQueueFactory Extension found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    IWorkQueueProcessorFactory factory = obj as IWorkQueueProcessorFactory;
                    if (factory != null)
                    {
                        WorkQueueTypeEnum type = factory.GetWorkQueueType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
	    }
		#endregion

        #region Public Methods
		/// <summary>
		/// Stop the WorkQueue processor
		/// </summary>
		public void Stop()
		{
			_terminateEvent.Set(); // make sure it's set
			_stop = true;
			if (_threadPool.Active)
				_threadPool.Stop();
		}

		/// <summary>
		/// The processing thread.
		/// </summary>
		/// <remarks>
		/// This method queries the database for WorkQueue entries to work on, and then uses
		/// a thread pool to process the entries.
		/// </remarks>
		public void Run()
		{
			// Reset any queue items related to this system that are in a "In Progress" state.
			try
			{
				ResetFailedItems();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e,
				             "Unable to reset WorkQueue items on startup.  There may be WorkQueue items orphaned in the queue.");
			}

			// Force the alert to be displayed right away, if it happens
			DateTime lastLog = Platform.Time.AddMinutes(-61);

			if (!_threadPool.Active)
				_threadPool.Start();

			Platform.Log(LogLevel.Info, "Work Queue Processor running...");

			while (true)
			{
				if (_stop)
					return;

				bool threadsAvailable = _threadPool.CanQueueItem;
				bool memoryAvailable = WorkQueueSettings.Instance.WorkQueueMinimumFreeMemoryMB == 0
				                      ||
				                      SystemResources.GetAvailableMemory(SizeUnits.Megabytes) >
				                      WorkQueueSettings.Instance.WorkQueueMinimumFreeMemoryMB;
				
				if (threadsAvailable && memoryAvailable)
				{
					try
					{
						Model.WorkQueue queueListItem = GetWorkQueueItem(ServerPlatform.ProcessorId);
						if (queueListItem == null)
						{
							/* No result found, or MemoryLimited threads not available, and no non-memory limited WorkQueue items */
							WaitHandle.WaitAny(new WaitHandle[] { _threadStop, _terminateEvent }, WorkQueueSettings.Instance.WorkQueueQueryDelay, false);
							_threadStop.Reset();
							continue;
						}

						if (!_extensions.ContainsKey(queueListItem.WorkQueueTypeEnum))
						{
							Platform.Log(LogLevel.Error,
							             "No extensions loaded for WorkQueue item type: {0}.  Failing item.",
							             queueListItem.WorkQueueTypeEnum);

							//Just fail the WorkQueue item, not much else we can do
							FailQueueItem(queueListItem, "No plugin to handle WorkQueue type: " + queueListItem.WorkQueueTypeEnum);
							continue;
						}

						try
						{
							IWorkQueueProcessorFactory factory = _extensions[queueListItem.WorkQueueTypeEnum];
							IWorkQueueItemProcessor processor = factory.GetItemProcessor();

							// Enqueue the actual processing of the item to the thread pool.  
							_threadPool.Enqueue(processor, queueListItem, ExecuteProcessor);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkQueue processor.");
							FailQueueItem(queueListItem, "Failure getting WorkQueue processor: " + e.Message);
							continue;
						}
					}
					catch (Exception e)
					{
						// Wait for only 1.5 seconds
						Platform.Log(LogLevel.Error, e, "Exception occured when processing WorkQueue item.");
						_terminateEvent.WaitOne(3000, false);
					}
				}
				else
				{
					if ((lastLog.AddMinutes(60) < Platform.Time) && !memoryAvailable)
					{
						lastLog = Platform.Time;
						Platform.Log(LogLevel.Error, "Unable to process WorkQueue entries, Minimum memory not available, minimum MB required: {0}, current MB available:{1}",
											 WorkQueueSettings.Instance.WorkQueueMinimumFreeMemoryMB,
											 SystemResources.GetAvailableMemory(SizeUnits.Megabytes));

						ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "WorkQueue", AlertTypeCodes.NoResources,
                                             null, TimeSpan.Zero,
						                     "Unable to process WorkQueue entries, Minimum memory not available, minimum MB required: {0}, current MB available:{1}",
						                     WorkQueueSettings.Instance.WorkQueueMinimumFreeMemoryMB,
						                     SystemResources.GetAvailableMemory(SizeUnits.Megabytes));
					}
					// wait for new opening in the pool or termination
					WaitHandle.WaitAny(new WaitHandle[] { _threadStop, _terminateEvent }, 3000, false);
					_threadStop.Reset();
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reset queue items that were unadvertly left in "in progress" state by previous run. 
		/// </summary>
		public static void ResetFailedItems()
		{
			WorkQueueStatusEnum pending = WorkQueueStatusEnum.Pending;
			WorkQueueStatusEnum failed = WorkQueueStatusEnum.Failed;

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				var reset = ctx.GetBroker<IWorkQueueReset>();
				var parms = new WorkQueueResetParameters
				{
					ProcessorID = ServerPlatform.ProcessorId,
					// reschedule to start again now
					RescheduleTime = Platform.Time,
					// retry will expire X minutes from now (so other process MAY NOT remove them)
					RetryExpirationTime = Platform.Time.AddMinutes(2),
					// if an entry has been retried more than WorkQueueMaxFailureCount, it should be failed
					MaxFailureCount = 3,
					// failed item expires now (so other process can remove them if desired)
					FailedExpirationTime = Platform.Time,
				};

				IList<Model.WorkQueue> modifiedList = reset.Find(parms);

				if (modifiedList != null)
				{
					// output the list of items that have been reset
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(pending))
							Platform.Log(LogLevel.Info, "Cleanup: Reset Queue Item : {0} --> Status={1} Scheduled={2} ExpirationTime={3}",
										 queueItem.GetKey().Key,
										 queueItem.WorkQueueStatusEnum,
										 queueItem.ScheduledTime,
										 queueItem.ExpirationTime);
					}

					// output the list of items that have been failed because it exceeds the max retry count
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(failed))
							Platform.Log(LogLevel.Info, "Cleanup: Fail Queue Item  : {0} : FailureCount={1} ExpirationTime={2}",
										 queueItem.GetKey().Key,
										 queueItem.FailureCount,
										 queueItem.ExpirationTime);
					}
				}

				ctx.Commit();
			}
		}

		/// <summary>
		/// The actual delegate 
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="queueItem"></param>
		private void ExecuteProcessor(IWorkQueueItemProcessor processor, Model.WorkQueue queueItem)
		{
			try
			{
				processor.Process(queueItem);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
				             "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (GUID: {1})",
				             queueItem.WorkQueueTypeEnum,
				             queueItem.GetKey());
				String error = e.InnerException != null ? e.InnerException.Message : e.Message;

				FailQueueItem(queueItem, error);
			}
			finally
			{
                // Signal the parent thread, so it can query again
                _threadStop.Set();

                // Cleanup the processor
                processor.Dispose();
			}

		}

    	/// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
		/// <param name="failureDescription">The reason for the failure.</param>
        private void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            // Must retry to reset the status of the entry in case of db error
            // Failure to do so will create stale work queue entry (stuck in "In Progress" state)
            // which can only be recovered by restarting the service.
            while(true) 
            {
                try
                {
                	WorkQueueTypeProperties prop = _propertiesDictionary[item.WorkQueueTypeEnum];
                    using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
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
                        if ((item.FailureCount + 1) > prop.MaxFailureCount)
                        {
                            Platform.Log(LogLevel.Error,
                                         "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
                                         item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, failureDescription);
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                            parms.ScheduledTime = Platform.Time;
                            parms.ExpirationTime = Platform.Time.AddDays(1);

                            OnWorkQueueEntryFailed(item, failureDescription);
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}. Failure Reason: {3}",
                                         item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, failureDescription);
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                            parms.ScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                            parms.ExpirationTime =
                                Platform.Time.AddSeconds((prop.MaxFailureCount - item.FailureCount) *
                                                         prop.FailureDelaySeconds);
                        }

                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                         item.GetKey().ToString());
                        }
                        else
                        {
                            updateContext.Commit();
                            break; // done
                        }
                    }                    
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error,  "Error occurred when calling FailQueueItem. Retry later. {0}", ex.Message);
                    _terminateEvent.WaitOne(2000, false);
                    if (_stop)
                    {
                        Platform.Log(LogLevel.Warn, "Service is stopping. Retry to fail the entry is terminated.");
                        break;
                    }                        
                }                
            }
        }

        private void OnWorkQueueEntryFailed(Model.WorkQueue item, string error)
        {
            RaiseAlert(item, AlertLevel.Error, error);
        }

		public void RaiseAlert(Model.WorkQueue queueItem, AlertLevel level, string message)
		{
			WorkQueueTypeProperties prop = _propertiesDictionary[queueItem.WorkQueueTypeEnum];
			if (prop.AlertFailedWorkQueue || level == AlertLevel.Critical)
			{
				ServerPlatform.Alert(AlertCategory.Application, level,
									 queueItem.WorkQueueTypeEnum.ToString(), AlertTypeCodes.UnableToProcess,
									 GetWorkQueueContextData(queueItem), TimeSpan.Zero,
									 "Work Queue item failed: Type={0}, GUID={1}: {2}",
									 queueItem.WorkQueueTypeEnum,
									 queueItem.GetKey(), message);
			}
		}

        /// <summary>
		/// Method for getting next <see cref="WorkQueue"/> entry.
		/// </summary>
		/// <param name="processorId">The Id of the processor.</param>
		/// <remarks>
		/// </remarks>
		/// <returns>
		/// A <see cref="WorkQueue"/> entry if found, or else null;
		/// </returns>
		public Model.WorkQueue GetWorkQueueItem(string processorId)
		{
			Model.WorkQueue queueListItem = null;

			// First check for Stat WorkQueue items.
			if (_threadPool.MemoryLimitedThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters
					                                 	{
					                                 		ProcessorID = processorId,
					                                 		WorkQueuePriorityEnum = WorkQueuePriorityEnum.Stat
					                                 	};

					queueListItem = select.FindOne(parms);
					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// If we don't have the max high priority threads in use,
			// first see if there's any available
			if (queueListItem == null
			    && _threadPool.HighPriorityThreadsAvailable)
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters
					                                 	{
					                                 		ProcessorID = processorId,
					                                 		WorkQueuePriorityEnum = WorkQueuePriorityEnum.High
					                                 	};

					queueListItem = select.FindOne(parms);
					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// If we didn't find a high priority work queue item, and we have threads 
			// available for memory limited work queue items, query for the next queue item available.
			if (queueListItem == null
			    && _threadPool.MemoryLimitedThreadsAvailable)
			{
				using (IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters
					                                 	{
					                                 		ProcessorID = processorId
					                                 	};

					queueListItem = select.FindOne(parms);
					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// This logic only accessed if memory limited and priority threads are used up 
			if (queueListItem == null
			    && !_threadPool.MemoryLimitedThreadsAvailable)
			{
				using (IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters
					                                 	{
					                                 		ProcessorID = processorId,
					                                 		WorkQueuePriorityEnum = WorkQueuePriorityEnum.Stat,
					                                 		MemoryLimited = true
					                                 	};

					queueListItem = select.FindOne(parms);
					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			// This logic only accessed if memory limited and priority threads are used up 
			if (queueListItem == null
			    && !_threadPool.MemoryLimitedThreadsAvailable)
			{
				using (IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					IQueryWorkQueue select = updateContext.GetBroker<IQueryWorkQueue>();
					WorkQueueQueryParameters parms = new WorkQueueQueryParameters
					                                 	{
					                                 		ProcessorID = processorId, 
															MemoryLimited = true
					                                 	};

					queueListItem = select.FindOne(parms);
					if (queueListItem != null)
						updateContext.Commit();
				}
			}

			return queueListItem;
		}

		private static WorkQueueAlertContextData GetWorkQueueContextData(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");

			WorkQueueAlertContextData contextData = new WorkQueueAlertContextData
			                                        	{
			                                        		WorkQueueItemKey = item.Key.Key.ToString()
			                                        	};

			StudyStorage storage = StudyStorage.Load(item.StudyStorageKey);
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

    	#endregion
    }
}
