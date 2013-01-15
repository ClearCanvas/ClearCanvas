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
using System.Diagnostics;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    internal class WorkItemQuery : IDisposable
    {
        #region Private Members

        private readonly DataAccessContext _context;
        private readonly List<WorkItem> _nowRunningWorkItems = new List<WorkItem>();
        private readonly List<WorkItem> _postponedWorkItems = new List<WorkItem>(); 
        
        #endregion

        #region Contructors

        private WorkItemQuery()
        {
            _context = new DataAccessContext(DataAccessContext.WorkItemMutex);
        }

        #endregion


        #region Public Static Methods

        public static List<WorkItem> GetWorkItems(int count, WorkItemPriorityEnum priority)
        {
            using (var query = new WorkItemQuery())
            {
                return query.InternalGetWorkItems(count, priority);
            }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method for getting next <see cref="WorkItem"/> entry.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="priority">Search for stat items.</param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        /// A <see cref="WorkItem"/> entry if found, or else null;
        /// </returns>
        private List<WorkItem> InternalGetWorkItems(int count, WorkItemPriorityEnum priority)
        {
            _nowRunningWorkItems.Clear();
            _postponedWorkItems.Clear();
            
            var itemsToPublish = new List<WorkItemData>();
            try
            {
                var workItemBroker = _context.GetWorkItemBroker();
                List<WorkItem> workItems = workItemBroker.GetWorkItemsForProcessing(count * 4, priority);
                foreach (var item in workItems)
                {
                    string reason;
                    if (CanStart(item, out reason))
                    {
                        item.Status = WorkItemStatusEnum.InProgress;
                        _nowRunningWorkItems.Add(item);
                    }
                    else
                    {
                        Postpone(item);
                        _postponedWorkItems.Add(item);
                        WorkItemProgress progress = item.Progress;
                        if (progress != null)
                        {
                            progress.StatusDetails = reason;
                            item.Progress = progress;
                            itemsToPublish.Add(WorkItemDataHelper.FromWorkItem(item));
                        }
                    }

                    if (_nowRunningWorkItems.Count >= count)
                        break;
                }

                _context.Commit();

                return new List<WorkItem>(_nowRunningWorkItems);
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected error querying for {0} {1} priority WorkItems", count, priority.GetDescription());
                return null;
            }
            finally
            {
                if (itemsToPublish.Count > 0)
                {
                    WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(WorkItemsChangedEventType.Update, itemsToPublish);
                }
            }
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        private void Postpone(WorkItem item)
        {
            DateTime now = Platform.Time;

            var timeWindowRequest = item.Request as IWorkItemRequestTimeWindow;

            if (timeWindowRequest != null && item.Request.Priority != WorkItemPriorityEnum.Stat)
            {
                DateTime scheduledTime = timeWindowRequest.GetScheduledTime(now, WorkItemServiceSettings.Default.PostponeSeconds);
                item.ProcessTime = scheduledTime;
                item.ScheduledTime = scheduledTime;
            }
            else
            {
                item.ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds);
            }

            if (item.ProcessTime > item.ExpirationTime)
                item.ExpirationTime = item.ProcessTime;
        }

        private bool CanStart(WorkItem item, out string reason)
        {
            if (item.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive)
                return CanStartNonExclusive(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdateTrigger)
                return CanStartStudyUpdateTrigger(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdate)
                return CanStartStudyUpdate(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
                return CanStartStudyDelete(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                return CanStartStudyRead(item, out reason);

            return CanStartExclusive(item, out reason);
        }

        private bool CanStartNonExclusive(WorkItem workItem, out string reason)
        {
            if (false)
            {
                Predicate<WorkItem> canRunConcurrently = w => w.Request.ConcurrencyType != WorkItemConcurrency.Exclusive;
                if (CompetingStudyWorkItem(workItem, canRunConcurrently, out reason))
                {
                    //Can't really ever get here because even though a "non-exclusive" item could technically be a "study work item",
                    //the only thing it can't run concurrently with is an "exclusive" item, whether it's a "study work item" or not.
                    //So, the only thing to check is whether or not there is a competing exclusive item.
                    return false;
                }
            }

            return !CompetingExclusiveWorkItem(workItem, out reason);
        }

        private bool CanStartStudyUpdateTrigger(WorkItem workItem, out string reason)
        {
            //A retrieve (trigger) waits for other retrieves already running, as well as other running imports/receives (study import).
            Predicate<WorkItem> canRunConcurrently = w => w.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive;
            if (CompetingStudyWorkItem(workItem, canRunConcurrently, out reason))
                return false;

            return !CompetingExclusiveWorkItem(workItem, out reason);
        }

        private bool CanStartStudyUpdate(WorkItem workItem, out string reason)
        {
            //An import can start running when a retrieve is already running.
            Predicate<WorkItem> canRunConcurrently = w => w.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdateTrigger || w.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive;
            if (CompetingStudyWorkItem(workItem, canRunConcurrently, out reason))
                return false;

            return !CompetingExclusiveWorkItem(workItem, out reason);
        }

        private bool CanStartStudyDelete(WorkItem workItem, out string reason)
        {
            Predicate<WorkItem> canRunConcurrently = w => w.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive;
            if (CompetingStudyWorkItem(workItem, canRunConcurrently, out reason))
                return false;

            return !CompetingExclusiveWorkItem(workItem, out reason);
        }

        private bool CanStartStudyRead(WorkItem workItem, out string reason)
        {
            Predicate<WorkItem> canRunConcurrently = w => w.Request.ConcurrencyType == WorkItemConcurrency.StudyRead || w.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive;
            if (CompetingStudyWorkItem(workItem, canRunConcurrently, out reason))
                return false;

            return !CompetingExclusiveWorkItem(workItem, out reason);
        }

        private bool CanStartExclusive(WorkItem workItem, out string reason)
        {
            //The input item is either Pending or Idle.
            if (workItem.Status == WorkItemStatusEnum.Pending)
            {
                //The current item is Exclusive and Pending.

                //A pending work item scheduled before, or of higher priority, should stop the current item from starting.
                var moreImportantWorkItems = GetPendingWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantWorkItems, out reason))
                    return false;

                //Anything already running or idle must finish first, regardless of priority or schedule.
                var runningOrIdleWorkItems = GetRunningOrIdleWorkItems();
                if (MustWaitForAny(runningOrIdleWorkItems, out reason))
                    return false;
            }
            else
            {
                //The current item is Exclusive and Idle.

                //Should never really happen, but only an already running item, or another idle item scheduled before or of higher priority, can stop the current one from starting.
                var moreImportantWorkItems = GetIdleWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantWorkItems, out reason))
                    return false;

                var runningWorkItems = GetRunningWorkItems();
                if (MustWaitForAny(runningWorkItems, out reason))
                    return false;
            }

            return true;
        }

        private bool CompetingStudyWorkItem(WorkItem workItem, Predicate<WorkItem> canRunConcurrently, out string reason)
        {
            reason = string.Empty;

            //Not a study work item, no need to check.
            if (String.IsNullOrEmpty(workItem.StudyInstanceUid))
                return false;

            //The input item is either Pending or Idle.
            if (workItem.Status == WorkItemStatusEnum.Pending)
            {
                //A pending work item (same study) scheduled before, or of higher priority, should stop the current item from starting.
                //However, the only reason the other item would still be pending is if it was waiting for something else,
                //so we'll start as long as the current item doesn't have a concurrency conflict with the other item.
                //Then we can safely make the most of the available processing time.
                var moreImportantWorkItems = GetPendingStudyWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantWorkItems.Where(w => !canRunConcurrently(w)), out reason))
                    return true;

                //Anything already running or idle (same study) must finish first, regardless of priority or schedule.
                var runningOrIdleWorkItems = GetRunningOrIdleStudyWorkItems(workItem);
                if (MustWaitForAny(runningOrIdleWorkItems.Where(w => !canRunConcurrently(w)), out reason))
                    return true;
            }
            else //Idle
            {
                //Should never really happen, but only an already running item (same study), or another idle item scheduled before or of higher priority, can stop the current one from starting.
                var moreImportantWorkItems = GetIdleStudyWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantWorkItems.Where(w => !canRunConcurrently(w)), out reason))
                    return true;

                var runningWorkItems = GetRunningStudyWorkItems(workItem);
                if (MustWaitForAny(runningWorkItems.Where(w => !canRunConcurrently(w)), out reason))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// For anything not itself exclusive, check whether or not there is a competing exclusive work item.
        /// </summary>
        /// <param name="workItem"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private bool CompetingExclusiveWorkItem(WorkItem workItem, out string reason)
        {
            if (workItem.Status == WorkItemStatusEnum.Pending)
            {
                //The item is Pending and is NOT itself Exclusive.
                var runningOrIdleExclusiveWorkItems = GetRunningOrIdleExclusiveWorkItems();
                if (MustWaitForAny(runningOrIdleExclusiveWorkItems, out reason))
                    return true;

                var moreImportantExclusiveWorkItems = GetPendingExclusiveWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantExclusiveWorkItems, out reason))
                    return true;
            }
            else
            {
                //The item is Idle and is NOT itself Exclusive.

                //Although it would never actually happen, because the logic in this class prevents it, the only thing that could
                //possibly stop the current (NOT exclusive) work item from starting is an exclusive work item that is already running,
                //or another idle (exclusive) item that is scheduled before it, or is of a higher priority.
                var runningExclusiveWorkItems = GetRunningExclusiveWorkItems();
                if (MustWaitForAny(runningExclusiveWorkItems, out reason))
                    return true;

                var moreImportantExclusiveWorkItems = GetIdleExclusiveWorkItemsScheduledBeforeOrHigherPriority(workItem);
                if (MustWaitForAny(moreImportantExclusiveWorkItems, out reason))
                    return true;
            }

            return false;
        }

        private bool MustWaitForAny(IEnumerable<WorkItem> workItems, out string reason)
        {
            //The actual item being waited on is the one of lowest priority with process time furthest in the future.
            workItems = workItems.OrderByDescending(w => w.ProcessTime).OrderByDescending(w => w.Priority);
            foreach (var workItem in workItems)
            {
                reason = string.Format("Waiting for: {0}", workItem.Request.ActivityDescription);
                return true;
            }

            reason = String.Empty;
            return false;
        }

        private static IEnumerable<WorkItem> CombineWorkItems(IEnumerable<WorkItem> workItems, IEnumerable<WorkItem> addOrReplaceItems)
        {
            // A bit hacky, but it avoids having to commit each change before requerying.

            // This method is used to combine items that are already running (or idle) with items just started by this class
            // that are not seen by database queries, so that we can determine which work item the current item is "waiting" for.

            var addOrReplaceItemsList = addOrReplaceItems.ToList();

            // First remove any items in the "add or replace" list, then union the 2 lists.
            // This yields all the items in the 2 lists with no duplication.
            // Note that there may be a fancier way to do this with LINQ, but I couldn't figure it out.
            var combined = workItems.Where(o => !addOrReplaceItemsList.Any(i => o.Oid == i.Oid)).Union(addOrReplaceItemsList);
            return combined;
        }

        private IEnumerable<WorkItem> ExcludeNowRunningItems(IEnumerable<WorkItem> workItems)
        {
            return workItems.Where(w => !_nowRunningWorkItems.Any(r => w.Oid == r.Oid));
        }

        #region Non-Study Work Item Queries
        #region Exclusive
        private IEnumerable<WorkItem> GetPendingExclusiveWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime,
                                                                                                     workItem.Priority,
                                                                                                     WorkItemStatusEnum.Pending,
                                                                                                     null,
                                                                                                     WorkItemConcurrency.Exclusive);
            //The stuff now running is not Pending anymore.
            //Note that we don't do anything with the items we postponed because we only change the Scheduled Time
            //for items with a time window, and that change will get picked up on the next query for items to "start".
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetIdleExclusiveWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime,
                                                                                                     workItem.Priority,
                                                                                                     WorkItemStatusEnum.Idle,
                                                                                                     null,
                                                                                                     WorkItemConcurrency.Exclusive);
            //The stuff now running is not Idle anymore.
            //Note that we don't do anything with the items we postponed because we only change the Scheduled Time
            //for items with a time window, and that change will get picked up on the next query for items to "start".
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetRunningExclusiveWorkItems()
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItems(WorkItemConcurrency.Exclusive, WorkItemStatusFilter.Running, null);

            //Include exclusive items that are now running.
            var nowRunningExclusiveItems = _nowRunningWorkItems.Where(w => w.Request.ConcurrencyType == WorkItemConcurrency.Exclusive);
            return CombineWorkItems(workItems, nowRunningExclusiveItems);
        }

        private IEnumerable<WorkItem> GetRunningOrIdleExclusiveWorkItems()
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItems(WorkItemConcurrency.Exclusive, WorkItemStatusFilter.RunningOrIdle, null);

            //Include exclusive items that are now running. Any that remained idle will also be in the list.
            var nowRunningExclusiveItems = _nowRunningWorkItems.Where(w => w.Request.ConcurrencyType == WorkItemConcurrency.Exclusive);
            return CombineWorkItems(workItems, nowRunningExclusiveItems);
        }

        #endregion

        private IEnumerable<WorkItem> GetIdleWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime,
                                                                                            workItem.Priority,
                                                                                            WorkItemStatusEnum.Idle, null);
            //The stuff now running is not Idle anymore.
            //Note that we don't do anything with the items we postponed because we only change the Scheduled Time
            //for items with a time window, and that change will get picked up on the next query for items to "start".
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetRunningWorkItems()
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItems(null, WorkItemStatusFilter.Running, null);
            
            //Include anything that is now running.
            return CombineWorkItems(workItems, _nowRunningWorkItems);
        }

        private IEnumerable<WorkItem> GetPendingWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime,
                                                                                            workItem.Priority,
                                                                                            WorkItemStatusEnum.Pending, null);

            //The stuff now running is not Pending anymore.
            //Note that we don't do anything with the items we postponed because we only change the Scheduled Time
            //for items with a time window, and that change will get picked up on the next query for items to "start".
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetRunningOrIdleWorkItems()
        {
            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItems(null, WorkItemStatusFilter.RunningOrIdle, null);
            //Include exclusive items that are now running. Any that remained idle will also be in the list.
            return CombineWorkItems(workItems, _nowRunningWorkItems);
        }

        #endregion
        #region Study Work Item Queries

        private IEnumerable<WorkItem> GetIdleStudyWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var workItems = GetStudyWorkItemsScheduledBeforeOrHigherPriority(workItem, WorkItemStatusEnum.Idle);
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetRunningStudyWorkItems(WorkItem workItem)
        {
            var workItems = GetStudyWorkItems(workItem, WorkItemStatusFilter.Running);
            //Include anything we just started for the same study.
            var nowRunningSameStudy = _nowRunningWorkItems.Where(w => w.StudyInstanceUid == workItem.StudyInstanceUid);
            return CombineWorkItems(workItems, nowRunningSameStudy);
        }

        private IEnumerable<WorkItem> GetPendingStudyWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem)
        {
            var workItems = GetStudyWorkItemsScheduledBeforeOrHigherPriority(workItem, WorkItemStatusEnum.Pending);
            //The stuff now running is not Pending anymore.
            return ExcludeNowRunningItems(workItems);
        }

        private IEnumerable<WorkItem> GetRunningOrIdleStudyWorkItems(WorkItem workItem)
        {
            var workItems = GetStudyWorkItems(workItem, WorkItemStatusFilter.RunningOrIdle);
            //Include anything we just started for the same study. Any that remained Idle will also be in the list.
            var nowRunningSameStudy = _nowRunningWorkItems.Where(w => w.StudyInstanceUid == workItem.StudyInstanceUid);
            return CombineWorkItems(workItems, nowRunningSameStudy);
        }

        private IEnumerable<WorkItem> GetStudyWorkItems(WorkItem workItem, WorkItemStatusFilter statusFilter)
        {
            //Don't bother if the work item's not for a study.
            if (String.IsNullOrEmpty(workItem.StudyInstanceUid))
                return new List<WorkItem>();

            var broker = _context.GetWorkItemBroker();
            var workItems = broker.GetWorkItems(null, statusFilter, workItem.StudyInstanceUid);

            //Shouldn't get the same one back, but do this just to be safe.
            return workItems.Where(w => w.Oid != workItem.Oid);
        }

        private IEnumerable<WorkItem> GetStudyWorkItemsScheduledBeforeOrHigherPriority(WorkItem workItem, WorkItemStatusFilter statusFilter)
        {
            //Don't bother if the work item's not for a study.
            if (String.IsNullOrEmpty(workItem.StudyInstanceUid))
                return new List<WorkItem>();

            var broker = _context.GetWorkItemBroker();

            var workItems = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime, workItem.Priority, statusFilter, workItem.StudyInstanceUid);

            //Shouldn't get the same one back, but do this just to be safe.
            return workItems.Where(w => w.Oid != workItem.Oid);
        }

        #endregion
        #endregion
    }
}
