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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class StudyItemSelection : ICollection<IStudyItem>
	{
		private readonly ICollection<IStudyItem> _master;
		private readonly List<IStudyItem> _innerList = new List<IStudyItem>();
		private event EventHandler _selectionChanged;
		private bool _suspendEvents = false;

		internal StudyItemSelection(ICollection<IStudyItem> master)
		{
			_master = master;
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		public void SuspendEvents()
		{
			_suspendEvents = true;
		}

		public void ResumeEvents(bool triggerEventImmediately)
		{
			_suspendEvents = false;

			if (triggerEventImmediately)
				NotifySelectionChanged();
		}

		protected void NotifySelectionChanged()
		{
			if (!_suspendEvents)
				EventsHelper.Fire(_selectionChanged, this, new EventArgs());
		}

		public void Add(IStudyItem item)
		{
			if (!_innerList.Contains(item) && _master.Contains(item))
			{
				_innerList.Add(item);
				NotifySelectionChanged();
			}
		}

		public void Clear()
		{
			if (_innerList.Count > 0)
			{
				_innerList.Clear();
				NotifySelectionChanged();
			}
		}

		public bool Contains(IStudyItem item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(IStudyItem[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		bool ICollection<IStudyItem>.IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IStudyItem item)
		{
			if (_innerList.Remove(item))
			{
				NotifySelectionChanged();
				return true;
			}
			return false;
		}

		public IEnumerator<IStudyItem> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}