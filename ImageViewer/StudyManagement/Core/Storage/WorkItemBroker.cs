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
using System.Linq;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public class WorkItemBroker : Broker
    {
        internal WorkItemBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

	    /// <summary>
	    /// Gets the specified number of pending work items.
	    /// </summary>
	    /// <param name="n"></param>
	    /// <param name="priority"> </param>
	    /// <returns></returns>
	    public List<WorkItem> GetWorkItemsForProcessing(int n, WorkItemPriorityEnum? priority = null)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            query = WorkItemStatusFilter.WaitingToProcess.Apply(query);
            query = query.Where(w => w.ProcessTime < DateTime.Now);
            if (priority.HasValue)
                query = query.Where(w => w.Priority == priority.Value);

            query = query.OrderBy(w => w.ProcessTime);
            if (!priority.HasValue)
                query = query.OrderBy(w => w.Priority);

            return query.Take(n).ToList();
        }

        /// <summary>
        /// Gets WorkItems to delete.
        /// </summary>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsToDelete(int n)
        {
            return (from w in Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Complete)
                          && w.DeleteTime < DateTime.Now
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// Gets WorkItems marked as deleted.
        /// </summary>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsDeleted(int n)
        {
            return (from w in Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Deleted)                          
                    select w).Take(n).ToList();
        }

        public IEnumerable<WorkItem> GetWorkItems(WorkItemConcurrency concurrency, WorkItemStatusFilter statusFilter, string studyInstanceUid)
        {
            return GetWorkItems(concurrency.GetWorkItemTypes(), statusFilter, studyInstanceUid);
        }

        public IEnumerable<WorkItem> GetWorkItems(string type, WorkItemStatusFilter statusFilter, string studyInstanceUid)
        {
            List<string> types = !String.IsNullOrEmpty(type) ? new List<string>(new[]{type}) : null;
            return GetWorkItems(types, statusFilter, studyInstanceUid);
        }

        private IEnumerable<WorkItem> GetWorkItems(List<string> types, WorkItemStatusFilter statusFilter, string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            if (types != null && types.Count > 0)
            {
                if (types.Count == 1)
                    query = query.Where(w => types[0] == w.Type);
                else
                    query = query.Where(w => types.Contains(w.Type));
            }

            statusFilter = statusFilter ?? WorkItemStatusFilter.NotDeleted;
            query = statusFilter.Apply(query);

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            query = query.OrderBy(w => w.ProcessTime);
            query = query.OrderBy(w => w.Priority);

            return query.AsEnumerable();
        }

	    /// <summary>
	    /// Get the WorkItems scheduled before <paramref name="scheduledTime"/> for <paramref name="studyInstanceUid"/>
	    /// and/or that are a higher priority and have not yet terminated (e.g. Waiting to run, or actively running).
	    /// </summary>
	    /// <param name="scheduledTime">The scheduled time to get related WorkItems for.</param>
	    /// <param name="priority">The priority of the workitem to compare with.</param>
        /// <param name="statusFilter">A filter for the work item status.</param>
        /// <param name="studyInstanceUid">The Study Instance UID to search for matching WorkItems. Can be null.</param>
        /// <param name="concurrency">The concurrency type of the items to be returned.</param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetWorkItemsScheduledBeforeOrHigherPriority(DateTime scheduledTime, WorkItemPriorityEnum priority, WorkItemStatusFilter statusFilter, string studyInstanceUid, WorkItemConcurrency concurrency)
        {
            return GetWorkItemsScheduledBeforeOrHigherPriority(scheduledTime, priority, statusFilter, studyInstanceUid, concurrency.GetWorkItemTypes().ToArray());
        }

	    /// <summary>
	    /// Get the WorkItems scheduled before <paramref name="scheduledTime"/> for <paramref name="studyInstanceUid"/>
	    /// and/or that are a higher priority and have not yet terminated (e.g. Waiting to run, or actively running).
	    /// </summary>
	    /// <param name="scheduledTime">The scheduled time to get related WorkItems for.</param>
	    /// <param name="priority">The priority of the workitem to compare with.</param>
        /// <param name="statusFilter">A filter for the work item status.</param>
        /// <param name="studyInstanceUid">The Study Instance UID to search for matching WorkItems. Can be null.</param>
        /// <param name="workItemTypes">The work item type(s) of the items to be returned. Can be null.</param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetWorkItemsScheduledBeforeOrHigherPriority(DateTime scheduledTime, WorkItemPriorityEnum priority, WorkItemStatusFilter statusFilter, string studyInstanceUid, params string[] workItemTypes)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            query = query.Where(w => w.ScheduledTime < DateTime.Now );
            query = query.Where(w => (w.ScheduledTime < scheduledTime && w.Priority <= priority) || w.Priority < priority);
            statusFilter = statusFilter ?? WorkItemStatusFilter.Active;
            query = statusFilter.Apply(query);

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            if (workItemTypes != null && workItemTypes.Length > 0)
            {
                if (workItemTypes.Length == 1)
                    query = query.Where(w => workItemTypes[0] == w.Type);
                else
                    query = query.Where(w => workItemTypes.Contains(w.Type));
            }

            query = query.OrderBy(w => w.ScheduledTime);
            query = query.OrderBy(w => w.Priority);
            return query.AsEnumerable();
        }

	    /// <summary>
        /// Get a specific WorkItem
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
		public WorkItem GetWorkItem(long oid)
		{
            var list = (from w in Context.WorkItems
                        where w.Oid == oid
                        select w);

            if (!list.Any()) return null;

            return list.First();		
		}

        /// <summary>
        /// Insert a WorkItem
        /// </summary>
        /// <param name="entity"></param>
        public void AddWorkItem(WorkItem entity)
        {
            Context.WorkItems.InsertOnSubmit(entity);
        }

        /// <summary>
        /// Delete WorkItemUid entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(WorkItem entity)
        {
            Context.WorkItems.DeleteOnSubmit(entity);
        }
    }
}
