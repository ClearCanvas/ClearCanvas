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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ClearCanvas.ImageViewer.Clipboard
{
	internal class ClipboardItemList : IList<IClipboardItem>
	{
		private readonly BindingList<IClipboardItem> _bindingList;

		public ClipboardItemList(BindingList<IClipboardItem> bindingList)
		{
			_bindingList = bindingList;
		}

		public BindingList<IClipboardItem> BindingList
		{
			get { return _bindingList; }
		}

		#region IList<IClipboardItem> Members

		public int IndexOf(IClipboardItem item)
		{
			return _bindingList.IndexOf(item);
		}

		public void Insert(int index, IClipboardItem item)
		{
			_bindingList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			if (index < _bindingList.Count)
			{
				if (_bindingList[index].IsLocked)
					throw new InvalidOperationException("Unable to remove item because it is locked.");

				_bindingList.RemoveAt(index);
			}
		}

		public IClipboardItem this[int index]
		{
			get { return _bindingList[index]; }
			set { throw new InvalidOperationException("Cannot set items via the indexer."); }
		}

		#endregion

		#region ICollection<IClipboardItem> Members

		public void Add(IClipboardItem item)
		{
			_bindingList.Add(item);
		}

		public void Clear()
		{
			if (_bindingList.Any(item => item.IsLocked))
				throw new InvalidOperationException("Unable to clear the list; there is a locked item.");

			_bindingList.Clear();
		}

		public bool Contains(IClipboardItem item)
		{
			return _bindingList.Contains(item);
		}

		public void CopyTo(IClipboardItem[] array, int arrayIndex)
		{
			_bindingList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _bindingList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IClipboardItem item)
		{
			if (item.IsLocked)
				throw new InvalidOperationException("Unable to remove item because it is locked.");

			return _bindingList.Remove(item);
		}

		#endregion

		#region IEnumerable<IClipboardItem> Members

		public IEnumerator<IClipboardItem> GetEnumerator()
		{
			return _bindingList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _bindingList.GetEnumerator();
		}

		#endregion
	}
}