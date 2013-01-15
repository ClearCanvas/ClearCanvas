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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Helper class for manipulating the Image Availability Work Queue.
	/// </summary>
	public static class ImageAvailabilityWorkQueue
	{
		private const string ProcedureOIDKey = "ProcedureOID";
		public const string WorkQueueItemType = "ImageAvailability";

		/// <summary>
		/// Creates a new work item.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="expirationTime"></param>
		/// <returns></returns>
		public static WorkQueueItem CreateWorkQueueItem(Procedure p, TimeSpan expirationTime)
		{
			WorkQueueItem item = new WorkQueueItem(WorkQueueItemType);
			item.ExpirationTime = Platform.Time.Add(expirationTime);
			item.ExtendedProperties.Add(ProcedureOIDKey, p.GetRef().Serialize());

			return item;
		}

		/// <summary>
		/// Polls the queue for pending items.
		/// </summary>
		/// <param name="batchSize"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IList<WorkQueueItem> GetPendingItems(int batchSize, IPersistenceContext context)
		{
			return context.GetBroker<IWorkQueueItemBroker>().GetPendingItems(WorkQueueItemType, batchSize);
		}

		/// <summary>
		/// Gets the procedure associated with the specified work item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static Procedure GetProcedure(WorkQueueItem item, IPersistenceContext context)
		{
			EntityRef procedureRef = new EntityRef(item.ExtendedProperties[ProcedureOIDKey]);
			return context.Load<Procedure>(procedureRef, EntityLoadFlags.Proxy);
		}
	}
}
