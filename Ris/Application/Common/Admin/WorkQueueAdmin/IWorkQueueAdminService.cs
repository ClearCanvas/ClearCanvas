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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	/// <summary>
	/// Provides services for administering the work queue.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IWorkQueueAdminService
	{
		[OperationContract]
		GetWorkQueueFormDataResponse GetWorkQueueFormData(GetWorkQueueFormDataRequest request);

		/// <summary>
		/// Lists all items in the work queue.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request);

		/// <summary>
		/// Load details for a specified work queue item
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		LoadWorkQueueItemForEditResponse LoadWorkQueueItemForEdit(LoadWorkQueueItemForEditRequest request);

		/// <summary>
		/// Purges all completed work queue items.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		PurgeCompletedWorkQueueItemsResponse PurgeCompletedWorkQueueItems(PurgeCompletedWorkQueueItemsRequest request);

		/// <summary>
		/// Resubmits the specified work queue item for processing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		ResubmitWorkQueueItemResponse ResubmitWorkQueueItem(ResubmitWorkQueueItemRequest request);

		/// <summary>
		/// Removes the specified work queue item.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		RemoveWorkQueueItemResponse RemoveWorkQueueItem(RemoveWorkQueueItemRequest request);
	}
}
