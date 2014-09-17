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
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public class WorkItemBroker : Broker
	{
		internal WorkItemBroker(DicomStoreDataContext context)
			: base(context) {}

		/// <summary>
		/// Gets the specified number of pending work items.
		/// </summary>
		/// <param name="n"></param>
		/// <param name="priority"> </param>
		/// <returns></returns>
		public List<WorkItem> GetWorkItemsForProcessing(int n, WorkItemPriorityEnum? priority = null)
		{
			return priority.HasValue
			       	? _getWorkItemsForProcessingByPriority(Context, n, Platform.Time, priority.Value).ToList()
                    : _getWorkItemsForProcessing(Context, n, Platform.Time).ToList();
		}

		/// <summary>
		/// Gets WorkItems to delete.
		/// </summary>
		/// <returns></returns>
		public List<WorkItem> GetWorkItemsToDelete(int n)
		{
            return _getWorkItemsToDelete(Context, n, Platform.Time).ToList();
		}

		/// <summary>
		/// Gets WorkItems marked as deleted.
		/// </summary>
		/// <returns></returns>
		public List<WorkItem> GetWorkItemsDeleted(int n)
		{
			return _getWorkItemsDeleted(Context, n).ToList();
		}

		public IEnumerable<WorkItem> GetWorkItems(WorkItemConcurrency concurrency, WorkItemStatusFilter statusFilter, string studyInstanceUid, long? identifier = null)
		{
			return GetWorkItems(concurrency.GetWorkItemTypes(), statusFilter, studyInstanceUid, identifier);
		}

		public IEnumerable<WorkItem> GetWorkItems(string type, WorkItemStatusFilter statusFilter, string studyInstanceUid, long? identifier = null)
		{
			List<string> types = !String.IsNullOrEmpty(type) ? new List<string>(new[] {type}) : null;
			return GetWorkItems(types, statusFilter, studyInstanceUid, identifier);
		}

		private IEnumerable<WorkItem> GetWorkItems(List<string> types, WorkItemStatusFilter statusFilter, string studyInstanceUid, long? identifier)
		{
			IQueryable<WorkItem> query = from w in Context.WorkItems select w;

			if (identifier.HasValue)
				query = query.Where(w => w.Oid == identifier.Value);

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

            query = query.Where(w => w.ScheduledTime < Platform.Time);
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
			return _getWorkItemByOid(Context, oid).FirstOrDefault();
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

		internal void DeleteAll()
		{
			Context.WorkItems.DeleteAllOnSubmit(Context.WorkItems);
		}

		#region Compiled Queries

		private static readonly Func<DicomStoreDataContext, int, DateTime, IQueryable<WorkItem>> _getWorkItemsForProcessing =
			CompiledQuery.Compile<DicomStoreDataContext, int, DateTime, IQueryable<WorkItem>>((context, n, now) => (from w in context.WorkItems
			                                                                                                        where w.ProcessTime < now
			                                                                                                              && (w.Status == WorkItemStatusEnum.Pending || w.Status == WorkItemStatusEnum.Idle)
			                                                                                                        orderby w.Priority , w.ProcessTime
			                                                                                                        select w).Take(n));

		private static readonly Func<DicomStoreDataContext, int, DateTime, WorkItemPriorityEnum, IQueryable<WorkItem>> _getWorkItemsForProcessingByPriority =
			CompiledQuery.Compile<DicomStoreDataContext, int, DateTime, WorkItemPriorityEnum, IQueryable<WorkItem>>((context, n, now, priority) => (from w in context.WorkItems
			                                                                                                                                        where w.ProcessTime < now && w.Priority == priority
			                                                                                                                                              && (w.Status == WorkItemStatusEnum.Pending || w.Status == WorkItemStatusEnum.Idle)
			                                                                                                                                        orderby w.ProcessTime
			                                                                                                                                        select w).Take(n));

		private static readonly Func<DicomStoreDataContext, int, DateTime, IQueryable<WorkItem>> _getWorkItemsToDelete =
			CompiledQuery.Compile<DicomStoreDataContext, int, DateTime, IQueryable<WorkItem>>((context, n, now) => (from w in context.WorkItems
			                                                                                                        where w.Status == WorkItemStatusEnum.Complete
			                                                                                                              && w.DeleteTime < now
			                                                                                                        select w).Take(n));

		private static readonly Func<DicomStoreDataContext, int, IQueryable<WorkItem>> _getWorkItemsDeleted =
			CompiledQuery.Compile<DicomStoreDataContext, int, IQueryable<WorkItem>>((context, n) => (from w in context.WorkItems
			                                                                                         where w.Status == WorkItemStatusEnum.Deleted
			                                                                                         select w).Take(n));

		private static readonly Func<DicomStoreDataContext, long, IQueryable<WorkItem>> _getWorkItemByOid =
			CompiledQuery.Compile<DicomStoreDataContext, long, IQueryable<WorkItem>>((context, oid) => (from w in context.WorkItems
			                                                                                            where w.Oid == oid
			                                                                                            select w).Take(1));

		#endregion
	}
}