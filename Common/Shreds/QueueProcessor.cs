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

namespace ClearCanvas.Common.Shreds
{
	/// <summary>
	/// Abstract base class for queue processor classes.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class QueueProcessor
	{
		private volatile bool _stopRequested;

		/// <summary>
		/// Runs the processor.
		/// </summary>
		/// <remarks>
		/// This method is expected to block indefinitely until the <see cref="RequestStop"/>
		/// method is called, at which point it should exit in a timely manner.
		/// </remarks>
		public void Run()
		{
			RunCore();
		}

		/// <summary>
		/// Requests the task to exit gracefully.
		/// </summary>
		/// <remarks>
		/// This method will be called on a thread other than the thread on which the task is executing.
		/// This method should return quickly - it should not block.  A typical implementation simply
		/// sets a flag that causes the <see cref="Run"/> method to terminate.
		/// must be implemented in such a way as to heed
		/// a request to stop within a timely manner.
		/// </remarks>
		public virtual void RequestStop()
		{
			_stopRequested = true;
		}

		/// <summary>
		/// A name for the queue processor.
		/// </summary>
		/// <remarks>
		/// The thread in the <see cref="QueueProcessorShred"/> corresponding to this QueueProcessor is given its Name.
		/// </remarks>
		public virtual string Name
		{
			get { return null; }
		}

		/// <summary>
		/// Implements the main logic of the processor.
		/// </summary>
		/// <remarks>
		/// Implementation is expected to run indefinitely but must poll the
		/// <see cref="StopRequested"/> property and exit in a timely manner when true.
		/// </remarks>
		protected abstract void RunCore();

		/// <summary>
		/// Gets a value indicating whether this processor has been requested to terminate.
		/// </summary>
		protected bool StopRequested
		{
			get { return _stopRequested; }
		}

	}

	/// <summary>
	/// Abstract base class for queue processor classes.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <remarks>
	/// <para>
	/// This class implements the logic to process a queue of items.  It polls the queue
	/// for a batch of items to process, processes those items, and then polls the queue
	/// again.  If the queue is empty, it sleeps for a preset amount of time.
	/// </para>
	/// <para>
	/// All threading is handled externally by <see cref="QueueProcessorShred"/>.
	/// </para>
	/// </remarks>
	public abstract class QueueProcessor<TItem> : QueueProcessor
	{
		private const int SnoozeIntervalInMilliseconds = 100;

		private readonly int _batchSize;
		private TimeSpan _sleepTime;
		private int _sleepTimeFactor;
		private bool _suspendRequested;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="batchSize">Max number of items to pull off queue for processing.</param>
		/// <param name="sleepTime"></param>
		protected QueueProcessor(int batchSize, TimeSpan sleepTime)
		{
			_batchSize = batchSize;
			_sleepTime = sleepTime;
		}

		/// <summary>
		/// Gets the next batch of items from the queue.
		/// </summary>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		protected abstract IList<TItem> GetNextBatch(int batchSize);

		/// <summary>
		/// Claims the item for processing.
		/// </summary>
		/// <remarks>
		/// This method is provided to enable the possibility of clustered queue processing - that is,
		/// having multiple processes operate on the same queue concurrently.  In this case,
		/// a process needs to claim a given item to ensure that no other process operates on it.
		/// The default implementation of this method does nothing and returns true.
		/// </remarks>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool ClaimItem(TItem item)
		{
			return true;
		}

		/// <summary>
		/// Called to process a queue item.
		/// </summary>
		/// <param name="item"></param>
		protected abstract void ProcessItem(TItem item);

		/// <summary>
		/// Requests the task to suspend.  The processor will pause for some duration then resume processing.
		/// </summary>
		protected void RequestSuspend(int sleepTimeFactor)
		{
			_suspendRequested = true;
			_sleepTimeFactor = sleepTimeFactor;
		}

		/// <summary>
		/// Gets a value indicating whether this processor has been requested to suspend.
		/// </summary>
		protected bool SuspendRequested
		{
			get { return _suspendRequested; }
		}

		#region Override Methods

		/// <summary>
		/// Implements the main logic of the processor.
		/// </summary>
		/// <remarks>
		/// Implementation is expected to run indefinitely but must poll the
		/// StopRequested property and exit in a timely manner when true.
		/// </remarks>
		protected sealed override void RunCore()
		{
			while (!StopRequested)
			{
				try
				{
					var items = GetNextBatch(_batchSize);

					// if no items, sleep
					if (items.Count == 0 && !StopRequested)
					{
						Sleep();
					}
					else
					{
						// process each item
						foreach (var item in items)
						{
							// break if stop requested
							// (unprocessed items will remain in queue and be picked up next time)
							if (StopRequested)
								break;

							// attempt to claim the item for processing
							if(ClaimItem(item))
							{
								// process the item
								ProcessItem(item);
							}

							// Suspend if requested
							// (unprocessed items will remain in queue and be picked up next time)
							if (this.SuspendRequested)
							{
								Suspend();
								break;
							}
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					if(!StopRequested)
						Sleep();
				}
			}
		}

		#endregion

		#region Helpers

		private void Suspend()
		{
			Sleep(_sleepTimeFactor);
			_suspendRequested = false;
		}

		private void Sleep()
		{
			Sleep(1);
		}

		private void Sleep(int sleepTimeFactor)
		{
			// sleep for the total sleep time, unless stop requested
			var totalMilliseconds = _sleepTime.TotalMilliseconds * sleepTimeFactor;
			for (var i = 0; i < totalMilliseconds && !StopRequested; i += SnoozeIntervalInMilliseconds)
			{
				Thread.Sleep(SnoozeIntervalInMilliseconds);
			}
		}

		#endregion

	}
}
