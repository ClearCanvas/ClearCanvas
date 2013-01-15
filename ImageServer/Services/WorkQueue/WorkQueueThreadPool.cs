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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
	/// <summary>
	/// Delegate used with <see cref="WorkQueueThreadPool"/> class.
	/// </summary>
	/// <param name="processor">The WorkQueue processor.</param>
	/// <param name="queueItem">The actual WorkQueue item.</param>
	public delegate void WorkQueueThreadDelegate(IWorkQueueItemProcessor processor, Model.WorkQueue queueItem);

	/// <summary>
	/// Class used to pass parameters to threads in the <see cref="WorkQueueThreadPool"/>.
	/// </summary>
	public class WorkQueueThreadParameter
	{
		private readonly IWorkQueueItemProcessor _processor;
		private readonly Model.WorkQueue _item;
		private readonly WorkQueueThreadDelegate _del;

		public WorkQueueThreadParameter(IWorkQueueItemProcessor processor, Model.WorkQueue item, WorkQueueThreadDelegate del)
		{
			_item = item;
			_processor = processor;
			_del = del;
		}

		public IWorkQueueItemProcessor Processor
		{
			get { return _processor; }
		}

		public Model.WorkQueue Item
		{
			get { return _item; }
		}

		public WorkQueueThreadDelegate Delegate
		{
			get { return _del; }
		}
	}

	/// <summary>
	/// Class for managing the WorkQueue thread pool.
	/// </summary>
	/// <remarks>
	/// This class is used to keep track of the current types of WorkQueue entries being processed and 
	/// for requested new queue entries based on the current types being processed.
	/// </remarks>
	public class WorkQueueThreadPool : ItemProcessingThreadPool<WorkQueueThreadParameter>
	{
		#region Private Members
		private readonly object _syncLock = new object();
		private readonly int _highPriorityThreadLimit;
		private readonly int _memoryLimitedThreadLimit;
		private int _memoryLimitedCount = 0;
		private int _highPriorityCount = 0;
		private int _totalThreadCount = 0;
		private readonly IDictionary<WorkQueueTypeEnum, WorkQueueTypeProperties> _workQueuePropList;
		private readonly List<WorkQueueThreadParameter> _queuedItems;
		#endregion

		#region Properties
		/// <summary>
		/// Are there threads available for queueing?
		/// </summary>
		public bool CanQueueItem
		{
			get
			{
				return (QueueCount + ActiveCount) < Concurrency;
			}
		}

		/// <summary>
		/// High Priority threads available.
		/// </summary>
		public bool HighPriorityThreadsAvailable
		{
			get
			{
				lock (_syncLock)
				{
					return _highPriorityCount < _highPriorityThreadLimit;
				}
			}
		}

		/// <summary>
		/// Memory limited threads available.
		/// </summary>
		public bool MemoryLimitedThreadsAvailable
		{
			get
			{
				lock (_syncLock)
				{
					return _memoryLimitedCount < _memoryLimitedThreadLimit;
				}
			}
		}
		#endregion

		#region Contructors
		/// <summary>
		/// Constructors.
		/// </summary>
		/// <param name="totalThreadCount">Total threads to be put in the thread pool.</param>
		/// <param name="highPriorityCount">Maximum high priority threads.</param>
		/// <param name="memoryLimitedThreadLimit">Maximum memory limited threads.</param>
		/// <param name="propList">List of WorkQueue type properties.</param>
		public WorkQueueThreadPool(int totalThreadCount, int highPriorityCount, int memoryLimitedThreadLimit, IDictionary<WorkQueueTypeEnum, WorkQueueTypeProperties> propList )
			: base(totalThreadCount)
		{
			_highPriorityThreadLimit = highPriorityCount;
			_memoryLimitedThreadLimit = memoryLimitedThreadLimit;
			_workQueuePropList = propList;
			_queuedItems = new List<WorkQueueThreadParameter>(totalThreadCount + 1);
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Override of OnStop method.
		/// </summary>
		/// <param name="completeBeforeStop"></param>
		/// <returns></returns>
		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;
			lock (_syncLock)
			{
				foreach (WorkQueueThreadParameter queuedItem in _queuedItems)
				{
					ICancelable cancel = queuedItem.Processor as ICancelable;
					if (cancel != null)
						cancel.Cancel();
				}
			}
			return true;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Override.
		/// </summary>
		/// <returns>
		/// A string description of the thread pool with the number of queue items in use of various types.
		/// </returns>
		public override string ToString()
		{
			lock (_syncLock)
			{
				return
					String.Format("WorkQueueThreadPool: {0} high priority, {1} of {2} memory limited, {3} total threads in use",
					              _highPriorityCount, _memoryLimitedCount, _memoryLimitedThreadLimit,
					              _totalThreadCount);
			}
		}

		/// <summary>
		/// Method called when a <see cref="WorkQueue"/> item completes.
		/// </summary>
		/// <param name="queueItem">The queue item completing.</param>
		private void QueueItemComplete(Model.WorkQueue queueItem)
		{
			lock (_syncLock)
			{
				if (queueItem.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
					_highPriorityCount--;

				if (queueItem.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.Stat))
					_highPriorityCount--;

				WorkQueueTypeProperties prop = _workQueuePropList[queueItem.WorkQueueTypeEnum];
				if (prop.MemoryLimited)
					_memoryLimitedCount--;

				_totalThreadCount--;

				foreach(WorkQueueThreadParameter queuedItem in _queuedItems)
				{
					if (queuedItem.Item.Key.Equals(queueItem.Key))
					{
						_queuedItems.Remove(queuedItem);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Enqueue a WorkQueue entry for processing.
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="item"></param>
		/// <param name="del"></param>
		public void Enqueue(IWorkQueueItemProcessor processor, Model.WorkQueue item, WorkQueueThreadDelegate del)
		{
			WorkQueueThreadParameter parameter = new WorkQueueThreadParameter(processor, item, del);

			lock (_syncLock)
			{
				if (item.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.High))
					_highPriorityCount++;
				if (item.WorkQueuePriorityEnum.Equals(WorkQueuePriorityEnum.Stat))
					_highPriorityCount++;

				WorkQueueTypeProperties prop = _workQueuePropList[item.WorkQueueTypeEnum];
				if (prop.MemoryLimited)
					_memoryLimitedCount++;

				_totalThreadCount++;

				_queuedItems.Add(parameter);
			}
			
			Enqueue(parameter,delegate(WorkQueueThreadParameter threadParameter)
			                       	{
			                       		threadParameter.Delegate(threadParameter.Processor, threadParameter.Item);

										QueueItemComplete(threadParameter.Item);
			                       	});
		}
		#endregion
	}
}
