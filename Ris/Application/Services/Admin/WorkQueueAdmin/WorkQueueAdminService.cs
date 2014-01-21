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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.WorkQueueAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IWorkQueueAdminService))]
	public class WorkQueueAdminService : ApplicationServiceBase, IWorkQueueAdminService
	{
		#region IWorkQueueAdminService Members

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public GetWorkQueueFormDataResponse GetWorkQueueFormData(GetWorkQueueFormDataRequest request)
		{
			var statuses = EnumUtils.GetEnumValueList<WorkQueueStatusEnum>(this.PersistenceContext);
			var types = CollectionUtils.Map<EnumValueInfo, string>(
				EnumUtils.GetEnumValueList<WorkQueueItemTypeEnum>(this.PersistenceContext),
				workQueueTypeEnum => workQueueTypeEnum.Code
			);

			return new GetWorkQueueFormDataResponse(statuses, types);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request)
		{
			var criteria = new WorkQueueItemSearchCriteria();

			if (request.Statuses != null && request.Statuses.Count > 0)
			{
				criteria.Status.In(CollectionUtils.Map<EnumValueInfo, WorkQueueStatus>(request.Statuses, status => EnumUtils.GetEnumValue<WorkQueueStatus>(status)));
			}

			if (request.Types != null && request.Types.Count > 0)
			{
				criteria.Type.In(request.Types);
			}

			if (request.ScheduledStartTime.HasValue || request.ScheduledEndTime.HasValue)
			{
				if (request.ScheduledStartTime.HasValue && request.ScheduledEndTime.HasValue)
				{
					criteria.ScheduledTime.Between(request.ScheduledStartTime.Value, request.ScheduledEndTime.Value);
				}
				else if (request.ScheduledStartTime.HasValue)
				{
					criteria.ScheduledTime.MoreThanOrEqualTo(request.ScheduledStartTime.Value);
				}
				else if (request.ScheduledEndTime.HasValue)
				{
					criteria.ScheduledTime.LessThanOrEqualTo(request.ScheduledEndTime.Value);
				}

				criteria.ScheduledTime.SortAsc(0);
			}
			else if (request.ProcessedStartTime.HasValue || request.ProcessedEndTime.HasValue)
			{
				if (request.ProcessedStartTime.HasValue && request.ProcessedEndTime.HasValue)
				{
					criteria.ProcessedTime.Between(request.ProcessedStartTime.Value, request.ProcessedEndTime.Value);
				}
				else if (request.ProcessedStartTime.HasValue)
				{
					criteria.CreationTime.MoreThanOrEqualTo(request.ProcessedStartTime.Value);
				}
				else if (request.ProcessedEndTime.HasValue)
				{
					criteria.ProcessedTime.LessThanOrEqualTo(request.ProcessedEndTime.Value);
				}

				criteria.ProcessedTime.SortAsc(0);
			}
			else
			{
				criteria.CreationTime.SortAsc(0);
			}

			var assembler = new WorkQueueItemAssembler();
			return new ListWorkQueueItemsResponse(
				CollectionUtils.Map<WorkQueueItem, WorkQueueItemSummary>(
					this.PersistenceContext.GetBroker<IWorkQueueItemBroker>().Find(criteria, request.Page),
					item => assembler.CreateWorkQueueItemSummary(item, this.PersistenceContext)));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public LoadWorkQueueItemForEditResponse LoadWorkQueueItemForEdit(LoadWorkQueueItemForEditRequest request)
		{
			var item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			var assembler = new WorkQueueItemAssembler();
			return new LoadWorkQueueItemForEditResponse(assembler.CreateWorkQueueItemDetail(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public PurgeCompletedWorkQueueItemsResponse PurgeCompletedWorkQueueItems(PurgeCompletedWorkQueueItemsRequest request)
		{
			throw new NotImplementedException();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public ResubmitWorkQueueItemResponse ResubmitWorkQueueItem(ResubmitWorkQueueItemRequest request)
		{
			var item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			item.Reschedule();
			this.PersistenceContext.SynchState();
			return new ResubmitWorkQueueItemResponse(new WorkQueueItemAssembler().CreateWorkQueueItemSummary(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public RemoveWorkQueueItemResponse RemoveWorkQueueItem(RemoveWorkQueueItemRequest request)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
