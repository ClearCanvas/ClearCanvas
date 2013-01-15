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

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkQueueItemAssembler
	{
		public WorkQueueItemSummary CreateWorkQueueItemSummary(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemSummary summary = new WorkQueueItemSummary();

			summary.WorkQueueItemRef = workQueueItem.GetRef();
			summary.CreationTime = workQueueItem.CreationTime;
			summary.ScheduledTime = workQueueItem.ScheduledTime;
			summary.ExpirationTime = workQueueItem.ExpirationTime;
			summary.User = workQueueItem.User;
			summary.Type = workQueueItem.Type;
			summary.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			summary.ProcessedTime = workQueueItem.ProcessedTime;
			summary.FailureCount = workQueueItem.FailureCount;
			summary.FailureDescription = workQueueItem.FailureDescription;

			return summary;
		}

		public WorkQueueItemDetail CreateWorkQueueItemDetail(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemDetail detail = new WorkQueueItemDetail();

			detail.WorkQueueItemRef = workQueueItem.GetRef();
			detail.CreationTime = workQueueItem.CreationTime;
			detail.ScheduledTime = workQueueItem.ScheduledTime;
			detail.ExpirationTime = workQueueItem.ExpirationTime;
			detail.User = workQueueItem.User;
			detail.Type = workQueueItem.Type;
			detail.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			detail.ProcessedTime = workQueueItem.ProcessedTime;
			detail.FailureCount = workQueueItem.FailureCount;
			detail.FailureDescription = workQueueItem.FailureDescription;
			detail.ExtendedProperties = ExtendedPropertyUtils.Copy(workQueueItem.ExtendedProperties);

			return detail;
		}
	}
}