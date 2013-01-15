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

using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.ImageViewer.Common
{
	/// <remarks>
	/// DO NOT USE THIS CLASS, as it will be removed or refactored in the future.
	/// </remarks>
	public class BlockingCollection<T> : ICollection<T>, IBlockingEnumerator<T>
	{
		private readonly object _syncLock = new object();
		private readonly List<T> _items = new List<T>();
		private volatile bool _isBlocking = true;

		#region IBlockingCollection<T> Members

		public bool IsBlocking
		{
			get { return _isBlocking; }
			set
			{
				if (_isBlocking == value)
					return;

				_isBlocking = value;
				if (!_isBlocking)
				{
					lock (_syncLock)
					{
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		#endregion

		private IEnumerable<T> GetItems()
		{
			while (_isBlocking)
			{
				T item;

				lock (_syncLock)
				{
					if (_items.Count == 0)
					{
						Monitor.Wait(_syncLock);
						if (_items.Count == 0)
							continue;
					}

					item = _items[0];
					_items.RemoveAt(0);
				}

				yield return item;
			}
		}

		public void Clear(out List<T> itemsRemoved)
		{
			lock (_syncLock)
			{
				itemsRemoved = new List<T>(_items);
				_items.Clear();
			}
		}

		#region ICollection<T> Members

		public void Add(T item)
		{
			lock (_syncLock)
			{
				if (!_items.Contains(item))
				{
					_items.Add(item);
					Monitor.Pulse(_syncLock);
				}
			}
		}

		public void AddRange(IEnumerable<T> items)
		{
			lock (_syncLock)
			{
				foreach (T item in items)
				{
					if (!_items.Contains(item))
					{
						_items.Add(item);
						Monitor.Pulse(_syncLock);
					}
				}
			}
		}

		public bool Remove(T item)
		{
			lock (_syncLock)
			{
				return _items.Remove(item);
			}
		}

		public void Clear()
		{
			lock (_syncLock)
			{
				_items.Clear();
			}
		}

		public bool Contains(T item)
		{
			lock (_syncLock)
			{
				return _items.Contains(item);
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (_syncLock)
			{
				_items.CopyTo(array, arrayIndex);
			}
		}

		public int Count
		{
			get
			{
				lock (_syncLock)
				{
					return _items.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return GetItems().GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetItems().GetEnumerator();
		}

		#endregion
	}
}