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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Basic producer-consumer queue.
	/// </summary>
	/// <typeparam name="T">The type to be used in the queue.</typeparam>
	public class BlockingQueue<T>
	{
		private readonly object _syncLock = new object();
		private readonly Queue<T> _queue;
		private bool _continueBlocking;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BlockingQueue()
		{
			_queue = new Queue<T>();
			_continueBlocking = true;
		}

		/// <summary>
		/// Removes the item at the head of the queue.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If no items are available, this call will block until an item becomes available, 
		/// unless the <see cref="ContinueBlocking"/> member has been set to false.
		/// </para>
		/// <para>
		/// This method will not throw an exception.
		/// </para>
		/// </remarks>
		/// <param name="value">The value of the next item in the queue, or <b>default(T)</b> 
		/// if <see cref="ContinueBlocking"/> is false and the queue is empty.</param>
		/// <returns>True if the item returned (via the out parameter) was in the queue, otherwise false.</returns>
		public bool Dequeue(out T value)
		{
			value = default(T);

			lock (_syncLock)
			{
				while (_continueBlocking && _queue.Count == 0)
					Monitor.Wait(_syncLock);

				if (_queue.Count == 0)
					return false;

				value = _queue.Dequeue();
			}

			return true;
		}

		/// <summary>
		/// Adds the specified item to the end of the queue.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown when the input item is null.</exception>
		/// <param name="item">The item to enqueue.</param>
		public void Enqueue(T item)
		{
			Platform.CheckForNullReference(item, "item");
			lock (_syncLock)
			{
				_queue.Enqueue(item);
				Monitor.Pulse(_syncLock);
			}
		}

		/// <summary>
		/// Indicates whether or not the <b>Dequeue</b> methods should block until the queue
		/// becomes non-empty.
		/// </summary>
		/// <remarks>
		/// When set to false, all actively waiting threads 
		/// (e.g. currently blocked, calling <b>Dequeue</b>) are  released so they 
		/// can determine whether or not they should quit.
		/// </remarks>
		public bool ContinueBlocking
		{
			get
			{
				lock (_syncLock)
				{
					return _continueBlocking;
				}
			}
			set
			{
				lock (_syncLock)
				{
					_continueBlocking = value;
					if (!_continueBlocking)
					{
						//release all waiting threads.
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		/// <summary>
		/// Returns the number of items remaining in the queue.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_syncLock)
				{
					return _queue.Count;
				}
			}
		}
	}
}
