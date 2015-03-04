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
using System.Linq;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public class WorkItemStatusFilter
	{
		private static readonly WorkItemStatusEnum[] _terminatedStatuses = new[]
		                                                                   	{
		                                                                   		WorkItemStatusEnum.Complete,
		                                                                   		WorkItemStatusEnum.Failed,
		                                                                   		WorkItemStatusEnum.Canceled,
		                                                                   		WorkItemStatusEnum.Deleted,
		                                                                   		WorkItemStatusEnum.DeleteInProgress
		                                                                   	};

		private static readonly WorkItemStatusEnum[] _waitingStatuses = new[]
		                                                                	{
		                                                                		WorkItemStatusEnum.Pending,
		                                                                		WorkItemStatusEnum.Idle
		                                                                	};

		private static readonly WorkItemStatusEnum[] _deletedStatuses = new[]
		                                                                	{
		                                                                		WorkItemStatusEnum.Deleted,
		                                                                		WorkItemStatusEnum.DeleteInProgress
		                                                                	};

		private readonly Func<IQueryable<WorkItem>, IQueryable<WorkItem>> _filterFunction;

		private WorkItemStatusFilter(Func<IQueryable<WorkItem>, IQueryable<WorkItem>> filterFunction)
		{
			_filterFunction = filterFunction;
		}

		public IQueryable<WorkItem> Apply(IQueryable<WorkItem> inputQuery)
		{
			return _filterFunction(inputQuery);
		}

		/// <summary>
		/// No-op.
		/// </summary>
		public static readonly WorkItemStatusFilter Nil = new WorkItemStatusFilter(q => q);

		/// <summary>
		/// <see cref="WorkItemStatusEnum.Pending"/> OR <see cref="WorkItemStatusEnum.Idle"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter WaitingToProcess = new WorkItemStatusFilter(q => q.Where(w => _waitingStatuses.Contains(w.Status)));

		/// <summary>
		/// NOT <see cref="WaitingToProcess"/> AND NOT <see cref="Terminated"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter Running = new WorkItemStatusFilter(q => q.Where(w => !_waitingStatuses.Contains(w.Status) && !_terminatedStatuses.Contains(w.Status)));

		/// <summary>
		/// NOT <see cref="WorkItemStatusEnum.Pending"/> AND NOT <see cref="Terminated"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter RunningOrIdle = new WorkItemStatusFilter(q => q.Where(w => w.Status != WorkItemStatusEnum.Pending && !_terminatedStatuses.Contains(w.Status)));

		/// <summary>
		/// NOT <see cref="Terminated"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter Active = new WorkItemStatusFilter(q => q.Where(w => !_terminatedStatuses.Contains(w.Status)));

		/// <summary>
		/// <see cref="WorkItemStatusEnum.Complete"/>, <see cref="WorkItemStatusEnum.Canceled"/>, <see cref="WorkItemStatusEnum.Deleted"/>, <see cref="WorkItemStatusEnum.DeleteInProgress"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter Terminated = new WorkItemStatusFilter(q => q.Where(w => _terminatedStatuses.Contains(w.Status)));

		/// <summary>
		/// <see cref="WorkItemStatusEnum.Deleted"/> OR <see cref="WorkItemStatusEnum.DeleteInProgress"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter Deleted = new WorkItemStatusFilter(q => q.Where(w => _deletedStatuses.Contains(w.Status)));

		/// <summary>
		/// NOT <see cref="WorkItemStatusEnum.Deleted"/> AND NOT <see cref="WorkItemStatusEnum.DeleteInProgress"/>.
		/// </summary>
		public static readonly WorkItemStatusFilter NotDeleted = new WorkItemStatusFilter(q => q.Where(w => !_deletedStatuses.Contains(w.Status)));

		public static WorkItemStatusFilter StatusIs(WorkItemStatusEnum status)
		{
			return new WorkItemStatusFilter(q => q.Where(w => w.Status == status));
		}

		public static WorkItemStatusFilter StatusIsNot(WorkItemStatusEnum status)
		{
			return new WorkItemStatusFilter(q => q.Where(w => w.Status != status));
		}

		public static WorkItemStatusFilter StatusIn(params WorkItemStatusEnum[] statuses)
		{
			return new WorkItemStatusFilter(q => q.Where(w => statuses.Contains(w.Status)));
		}

		public static WorkItemStatusFilter StatusNotIn(params WorkItemStatusEnum[] statuses)
		{
			return new WorkItemStatusFilter(q => q.Where(w => !statuses.Contains(w.Status)));
		}

		public static implicit operator WorkItemStatusFilter(WorkItemStatusEnum? status)
		{
			return status.HasValue ? StatusIs(status.Value) : Nil;
		}
	}
}