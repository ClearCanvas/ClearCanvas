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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
	/// <summary>
	/// Delegate used with <see cref="WorkItemThreadPool"/> class.
	/// </summary>
	/// <param name="processor">The WorkItem processor.</param>
	/// <param name="queueItem">The actual WorkItem item.</param>
	internal delegate void WorkItemThreadDelegate(IWorkItemProcessor processor, WorkItem queueItem);

	/// <summary>
    /// Class used to pass parameters to threads in the <see cref="WorkItemThreadPool"/>.
	/// </summary>
	internal class WorkItemThreadParameter
	{
		private readonly IWorkItemProcessor _processor;
		private readonly WorkItem _item;
		private readonly WorkItemThreadDelegate _del;

		public WorkItemThreadParameter(IWorkItemProcessor processor, WorkItem item, WorkItemThreadDelegate del)
		{
			_item = item;
			_processor = processor;
			_del = del;
		}

		public IWorkItemProcessor Processor
		{
			get { return _processor; }
		}

        public WorkItem Item
		{
			get { return _item; }
		}

		public WorkItemThreadDelegate Delegate
		{
			get { return _del; }
		}
	}

	/// <summary>
	/// Class for managing the WorkItem thread pool.
	/// </summary>
	/// <remarks>
	/// This class is used to keep track of the current priorities of WorkItem entries being processed and 
	/// for requested new queue entries being processed.
	/// </remarks>
	internal class WorkItemThreadPool : ItemProcessingThreadPool<WorkItemThreadParameter>
	{
		#region Private Members
		private readonly object _syncLock = new object();
		private int _statPriorityCount;
		private int _totalThreadCount;
	    private readonly int _normalThreads;
        private readonly int _statThreads;

		private readonly List<WorkItemThreadParameter> _queuedItems;
		#endregion

		#region Properties
	    public int StatThreadsAvailable
	    {
	        get
	        {
                int available = Concurrency - (QueueCount + ActiveCount);
	            return available < 0 ? 0 : available;
	        }
	    }
        public int NormalThreadsAvailable
        {
            get
            {
                int available = Concurrency - (QueueCount + ActiveCount);
                if (_statPriorityCount < _statThreads)
                    available -= _statThreads - _statPriorityCount;
                return available < 0 ? 0 : available;
            }
        }
        #endregion

		#region Contructors
		/// <summary>
		/// Constructors.
		/// </summary>
        /// <param name="statThreadCount">Total threads to be put in the thread pool.</param>
        /// <param name="normalThreadCount"></param>
		public WorkItemThreadPool(int statThreadCount, int normalThreadCount)
			: base(statThreadCount + normalThreadCount)
		{
		    _normalThreads = normalThreadCount;
		    _statThreads = statThreadCount;
            _queuedItems = new List<WorkItemThreadParameter>(statThreadCount + normalThreadCount);		    
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
				foreach (WorkItemThreadParameter queuedItem in _queuedItems)
				{
                    queuedItem.Processor.Stop();
				}
			}
			return true;
		}

        /// <summary>
        /// Method called when a <see cref="WorkItem"/> item completes.
        /// </summary>
        /// <param name="item">The work item completing.</param>
        private void ThreadComplete(WorkItem item)
        {
            lock (_syncLock)
            {
                if (item.Priority.Equals(WorkItemPriorityEnum.Stat))
                    _statPriorityCount--;

                _totalThreadCount--;

                foreach (WorkItemThreadParameter queuedItem in _queuedItems)
                {
                    if (queuedItem.Item.Oid.Equals(item.Oid))
                    {
                        _queuedItems.Remove(queuedItem);
                        break;
                    }
                }
            }
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
					String.Format("WorkItemThreadPool: {0} stat priority, {1} total threads in use",
					              _statPriorityCount, _totalThreadCount);
			}
		}

		/// <summary>
		/// Enqueue a WorkItem entry for processing.
		/// </summary>
		/// <param name="processor"></param>
		/// <param name="item"></param>
		/// <param name="del"></param>
        public void Enqueue(IWorkItemProcessor processor, WorkItem item, WorkItemThreadDelegate del)
		{
			var parameter = new WorkItemThreadParameter(processor, item, del);

			lock (_syncLock)
			{
				if (item.Priority.Equals(WorkItemPriorityEnum.Stat))
					_statPriorityCount++;

				_totalThreadCount++;

				_queuedItems.Add(parameter);
			}
			
			Enqueue(parameter,delegate(WorkItemThreadParameter threadParameter)
			                       	{
			                       		threadParameter.Delegate(threadParameter.Processor, threadParameter.Item);

										ThreadComplete(threadParameter.Item);
			                       	});
		}

        /// <summary>
        /// Check if the specified <see cref="WorkItem"/> is in the thread pool and cancel if it is.
        /// </summary>
        /// <param name="workQueueOid"></param>
        public void Cancel(long workQueueOid)
        {
            foreach (WorkItemThreadParameter queuedItem in _queuedItems)
            {
                if (queuedItem.Item.Oid.Equals(workQueueOid))
                {
                    Platform.Log(LogLevel.Info, "Canceling {0} WorkItem with Oid {1}", queuedItem.Item.Type,
                                 workQueueOid);
                    queuedItem.Processor.Cancel();
                    return;
                }
            }
        }
		#endregion
	}
}
