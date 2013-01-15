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
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Workflow
{
	/// <summary>
	/// A specialization of <see cref="QueueProcessor{TItem}"/> that operates on items of type <see cref="WorkQueueItem"/>.
	/// </summary>
	public abstract class WorkQueueProcessor : EntityQueueProcessor<WorkQueueItem>
	{
		protected WorkQueueProcessor(int batchSize, TimeSpan sleepTime)
			:base(batchSize, sleepTime)
		{
		}

		/// <summary>
		/// Gets the next batch of items from the queue.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected override IList<WorkQueueItem> GetNextEntityBatch(int batchSize)
		{
			return PersistenceScope.CurrentContext.GetBroker<IWorkQueueItemBroker>().GetPendingItems(WorkQueueItemType, batchSize);
		}

		/// <summary>
		/// Called to mark a queue item as being claimed for processing.
		/// </summary>
		/// <param name="item"></param>
		protected override void MarkItemClaimed(WorkQueueItem item)
		{
			// do nothing
		}

		/// <summary>
		/// Called when <see cref="QueueProcessor{TItem}.ProcessItem"/> succeeds.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="item"></param>
		protected override void OnItemSucceeded(WorkQueueItem item)
		{
			// See if the item needs to be rescheduled for another processing before completing it.
			DateTime rescheduleTime;
			if (ShouldReschedule(item, null, out rescheduleTime))
				item.Reschedule(rescheduleTime);
			else
				item.Complete();
		}

		/// <summary>
		/// Called when <see cref="QueueProcessor{TItem}.ProcessItem"/> throws an exception.
		/// </summary>
		/// <remarks>
		/// Subclasses should not need to override this method.
		/// </remarks>
		/// <param name="item"></param>
		/// <param name="error"></param>
		protected override void OnItemFailed(WorkQueueItem item, Exception error)
		{
			// mark item as failed
			item.Fail(error.Message);

			// optionally reschedule the item
			DateTime retryTime;
			if (ShouldReschedule(item, error, out retryTime))
				item.Reschedule(retryTime);
		}

		/// <summary>
		/// Gets the type of work queue item that this processor operates on.
		/// </summary>
		protected abstract string WorkQueueItemType { get; }

		/// <summary>
		/// Called after a work item fails, to determine whether it should be re-tried.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="error"></param>
		/// <param name="retryTime"></param>
		/// <returns></returns>
		//protected abstract bool ShouldRetry(WorkQueueItem item, Exception error, out DateTime retryTime);

		/// <summary>
		/// Called after a work item is processed, regardless of whether it succeeded or failed,
		/// to determine whether it should be rescheduled for further processing.
		/// </summary>
		/// <remarks>
		/// If the item failed, it may need to rescheduled to try again.  If the item succeeded, it may need to be
		/// rescheduled for further work.
		/// </remarks>
		/// <param name="item">Item in question.</param>
		/// <param name="error">If processing failed, the exception that it failed with.  Otherwise null.</param>
		/// <param name="rescheduleTime">Time for which processing should be rescheduled.</param>
		/// <returns></returns>
		/// <remarks>
		/// Subclasses should override this method if it takes several stages to completely process an item.
		/// </remarks>
		protected virtual bool ShouldReschedule(WorkQueueItem item, Exception error, out DateTime rescheduleTime)
		{
			// By default, an item should not be rescheduled
			rescheduleTime = DateTime.MinValue;
			return false;
		}
	}
}
