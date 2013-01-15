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
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    /// <summary>
    /// Engine for acquiring WorkItems and finding plugins to process them.
    /// </summary>
    sealed public class WorkItemProcessor : QueueProcessor
    {
        #region Members

        private readonly Dictionary<string, IWorkItemProcessorFactory> _extensions = new Dictionary<string, IWorkItemProcessorFactory>();
		private readonly WorkItemThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly string _name;

        #endregion

		#region Constructor

        private WorkItemProcessor(int numberStatThreads, int numberNormalThreads, string name)
        {
            _name = name;
            _threadStop = new ManualResetEvent(false);

			_threadPool = new WorkItemThreadPool(numberStatThreads,numberNormalThreads)
			                  {
			                      ThreadPoolName = name + " Pool"
			                  };

			var ep = new WorkItemProcessorFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the workqueue processor. 
                Platform.Log(LogLevel.Fatal, "No WorkItemFactory Extensions found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    var factory = obj as IWorkItemProcessorFactory;
                    if (factory != null)
                    {
                        var type = factory.GetWorkQueueType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
	    }

		#endregion

        #region Public Properties

        public static WorkItemProcessor Instance { get; private set; }

        /// <summary>
        /// The Thread Name.
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initialize the singleton <see cref="WorkItemProcessor"/>.
        /// </summary>
        /// <param name="numberStatThreads"></param>
        /// <param name="numberNormalThreads"></param>
        /// <param name="name"></param>
        public static void CreateProcessor(int numberStatThreads, int numberNormalThreads, string name)
        {
            if (Instance != null) throw new ApplicationException("Processor already created!");

            Instance = new WorkItemProcessor(numberStatThreads, numberNormalThreads, name);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Signal the processor to stop sleeping and check for Shutdown or new WorkItem
        /// </summary>
        public void SignalThread()
        {
            _threadStop.Set();
        }

        /// <summary>
        /// Stop the WorkItem processor
		/// </summary>
		public override void RequestStop()
		{
            base.RequestStop();

			_threadStop.Set();

			if (_threadPool.Active)
				_threadPool.Stop();
		}

		/// <summary>
		/// The processing thread.
		/// </summary>
		/// <remarks>
        /// This method queries the database for WorkItem entries to work on, and then uses
		/// a thread pool to process the entries.
		/// </remarks>
        protected override void RunCore()
        {
            // Reset any in progress WorkItems if we crashed while processing.
		    ResetInProgressWorkItems();

		    if (!_threadPool.Active)
		        _threadPool.Start();

		    Platform.Log(LogLevel.Info, "WorkItem Processor running...");

            while (true)
            {
                if (StopRequested)
                    return;

                // First, use upto 1/2 of the normal threads to delete 
                // work items that should be purged, we had testing issues where the deletion items were
                // never removed, and decided to be a bit aggressive with ensuring they get removed.
                if ( _threadPool.NormalThreadsAvailable > 1)
                {
                    var deleteList = GetWorkItemsToDelete(_threadPool.NormalThreadsAvailable / 2);
                    if (deleteList != null && deleteList.Count > 0)
                    {
                        QueueWorkItems(deleteList);
                    }
                }

                var list = GetWorkItems(_threadPool.StatThreadsAvailable, _threadPool.NormalThreadsAvailable);
                
                if ( list == null)
                {
                    /* No threads available, wait for one to complete. */
                    if (_threadStop.WaitOne(2500, false))
                        _threadStop.Reset();
                    continue;
                }
                if  (list.Count == 0)
                {
                    // No result found 
                    if (_threadStop.WaitOne(2500, false))
                        _threadStop.Reset(); 
                    continue;
                }

                // Queue up the workItems
                QueueWorkItems(list);
            }
		}

        internal static List<WorkItem> GetWorkItems(int statThreadsAvailable, int normalThreadsAvailable)
        {
            List<WorkItem> list = null;
            if (statThreadsAvailable > 0)
            {
                list = WorkItemQuery.GetWorkItems(statThreadsAvailable, WorkItemPriorityEnum.Stat);
            }

            if ((list == null || list.Count == 0) && normalThreadsAvailable > 0)
            {
                list = WorkItemQuery.GetWorkItems(normalThreadsAvailable, WorkItemPriorityEnum.High);
            }

            if ((list == null || list.Count == 0) && normalThreadsAvailable > 0)
            {
                list = WorkItemQuery.GetWorkItems(normalThreadsAvailable, WorkItemPriorityEnum.Normal);
            }

            return list;
        }

        /// <summary>
        /// Cancel a current running WorkItem
        /// </summary>
        /// <param name="workItemOid"></param>
        public void Cancel(long workItemOid)
        {
            _threadPool.Cancel(workItemOid);
        }
        #endregion

		#region Private Methods

        private void QueueWorkItems(IEnumerable<WorkItem> list )
        {
            try
            {
                foreach (var item in list)
                {
                    if (!_extensions.ContainsKey(item.Request.WorkItemType))
                    {
                        Platform.Log(LogLevel.Error,
                                     "No extensions loaded for WorkItem item type: {0}.  Failing item.",
                                     item.Type);

                        //Just fail the WorkQueue item, not much else we can do
                        var proxy = new WorkItemStatusProxy(item);
                        proxy.Fail("No plugin to handle WorkItem type: " + item.Type, WorkItemFailureType.Fatal);
                        continue;
                    }

                    try
                    {
                        IWorkItemProcessorFactory factory = _extensions[item.Request.WorkItemType];
                        IWorkItemProcessor processor = factory.GetItemProcessor();

                        // Enqueue the actual processing of the item to the thread pool.  
                        _threadPool.Enqueue(processor, item, ExecuteProcessor);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception creating WorkItem processor.");
                        var proxy = new WorkItemStatusProxy(item);
                        proxy.Fail("No plugin to handle WorkItem type: " + item.Type, WorkItemFailureType.Fatal);
                    }
                }
            }
            catch (Exception e)
            {
                // Wait for only 3 seconds
                Platform.Log(LogLevel.Error, e, "Exception occured when processing WorkItem item.");
                _threadStop.WaitOne(3000, false);
            }
        }

		/// <summary>
		/// The actual delegate 
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="queueItem"></param>
        private void ExecuteProcessor(IWorkItemProcessor processor, WorkItem queueItem)
		{
		    var proxy = new WorkItemStatusProxy(queueItem);

            try
            {
                Platform.Log(proxy.LogLevel, "Starting processing of {0} WorkItem for OID {1}", queueItem.Type, queueItem.Oid);

                if (proxy.Item.Status == WorkItemStatusEnum.Deleted || proxy.Item.Status == WorkItemStatusEnum.DeleteInProgress)
                {
                    if (!processor.Initialize(proxy))
                    {
                        Platform.Log(LogLevel.Error, "Unable to intialize WorkItem processor for: {0}.  Directly deleting.", proxy.Request.ActivityTypeString);
                        proxy.Delete();
                        return;
                    }

                    // Delete the entry
                    processor.Delete();
                    return;
                }

                if (!processor.Initialize(proxy))
                {
                    proxy.Postpone();
                    return;
                }

                processor.Process();
            }
            catch (NotEnoughStorageException e)
            {
                // No space. Can fail right way. 
                Platform.Log(LogLevel.Error, "Not enough storage space when processing WorkQueue item of type {0}.  Failing Queue item. (Oid: {1})", queueItem.Type, queueItem.Oid);                
                proxy.Fail(SR.ExceptionNotEnoughStorage, WorkItemFailureType.Fatal);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e,
                             "Unexpected exception when processing WorkQueue item of type {0}.  Failing Queue item. (Oid: {1})",
                             queueItem.Type,
                             queueItem.Oid);
                String error = e.InnerException != null ? e.InnerException.Message : e.Message;

                proxy.Fail(error, WorkItemFailureType.NonFatal);
            }
			finally
			{
                // Signal the parent thread, so it can query again
                _threadStop.Set();

                // Cleanup the processor
                processor.Dispose();
                Platform.Log(proxy.LogLevel, "Done processing of {0} WorkItem for OID {1} and status {2}", proxy.Item.Type, proxy.Item.Oid, proxy.Item.Status);
			}
		}

     
        /// <summary>
        /// Method for getting next <see cref="WorkItem"/> entry.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        /// A <see cref="WorkItem"/> entry if found, or else null;
        /// </returns>
        private List<WorkItem> GetWorkItemsToDelete(int count)
        {
            try
            {
                // Get WorkItems that have expired that need to be deleted
                using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                {
                    var workItemBroker = context.GetWorkItemBroker();

                    var workItems = workItemBroker.GetWorkItemsToDelete(count);

                    foreach (var item in workItems)
                    {
                        item.Status = WorkItemStatusEnum.DeleteInProgress;
                    }

                    context.Commit();
                    if (workItems.Count > 0)
                        return workItems;
                }

                // Get entries already marked as deleted by the GUI.
                using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                {
                    var workItemBroker = context.GetWorkItemBroker();

                    var workItems = workItemBroker.GetWorkItemsDeleted(count);

                    foreach (var item in workItems)
                    {
                        item.Status = WorkItemStatusEnum.DeleteInProgress;
                    }

                    context.Commit();

                    return workItems;
                }
            }
            catch (Exception)
            {
                return new List<WorkItem>();
            }
        }

        /// <summary>
        /// Called on startup to reset InProgress WorkItems back to Pending.
        /// </summary>
        private void ResetInProgressWorkItems()
        {
            bool reindexInProgress = false;

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Pending;
                    if (item.Type.Equals(ReindexRequest.WorkItemTypeString))
                        reindexInProgress = true;
                }

                context.Commit();
            }

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.DeleteInProgress, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Deleted;
                }

                context.Commit();
            }

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();
                var list = workItemBroker.GetWorkItems(null, WorkItemStatusEnum.Canceling, null);

                foreach (var item in list)
                {
                    item.Status = WorkItemStatusEnum.Canceled;
                    if (item.Type.Equals(ReindexRequest.WorkItemTypeString))
                        reindexInProgress = true;
                }

                context.Commit();
            }

            if (reindexInProgress)
            {
                using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                {
                    var studyBroker = context.GetStudyBroker();
                    var studyList = studyBroker.GetReindexStudies();

                    foreach (var item in studyList)
                    {
                        item.Reindex = false;
                    }

                    context.Commit();
                }
            }
        }

    	#endregion
    }
}
