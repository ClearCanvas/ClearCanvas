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
using System.Collections.Specialized;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IStudyFilterColumnCollection : IList<StudyFilterColumn>, IEnumerable<string>, IEnumerable
	{
		StringCollection Serialize();
		void Deserialize(StringCollection columns);
		int IndexOf(string columnKey);
		StudyFilterColumn this[string columnKey] { get; }
		bool Contains(string columnKey);
	}

	partial class StudyFilterComponent
	{
		private class StudyFilterColumnCollection : IStudyFilterColumnCollection
		{
			private readonly List<StudyFilterColumn> _innerList = new List<StudyFilterColumn>();
			private readonly StudyFilterComponent _owner;

			public StudyFilterColumnCollection(StudyFilterComponent owner)
			{
				_owner = owner;
			}

			public StringCollection Serialize()
			{
				StringCollection columns = new StringCollection();
				foreach (StudyFilterColumn column in _innerList)
				{
					columns.Add(column.Key);
				}
				return columns;
			}

			public void Deserialize(StringCollection columns)
			{
				if (columns != null)
				{
					foreach (StudyFilterColumn column in _innerList)
						column.Owner = null;

					_innerList.Clear();
					foreach (string columnKey in columns)
					{
						StudyFilterColumn column = StudyFilterColumn.CreateColumn(columnKey);
						if (column != null)
							_innerList.Add(column);
					}
					_owner.ColumnsChanged(_innerList);

					foreach (StudyFilterColumn column in _innerList)
						column.Owner = _owner;
				}
			}

			public int IndexOf(StudyFilterColumn item)
			{
				return _innerList.IndexOf(item);
			}

			public int IndexOf(string columnKey)
			{
				for (int n = 0; n < _innerList.Count; n++)
					if (_innerList[n].Key == columnKey)
						return n;
				return -1;
			}

			public void Insert(int index, StudyFilterColumn item)
			{
				if (_innerList.Contains(item))
					throw new ArgumentException("Column already exists in collection.", "item");
				_innerList.Insert(index, item);
				_owner.ColumnInserted(index, item);
				item.Owner = _owner;
			}

			public void RemoveAt(int index)
			{
				StudyFilterColumn item = _innerList[index];
				_innerList.RemoveAt(index);
				_owner.ColumnRemoved(index, item);
				item.Owner = null;
			}

			public StudyFilterColumn this[int index]
			{
				get { return _innerList[index]; }
				set
				{
					if (_innerList.Contains(value))
						throw new ArgumentException("Column already exists in collection.", "value");

					StudyFilterColumn item = _innerList[index];
					_innerList[index] = value;
					_owner.ColumnChanged(index, item, value);
					item.Owner = null;
					value.Owner = _owner;
				}
			}

			public StudyFilterColumn this[string columnKey]
			{
				get
				{
					for (int n = 0; n < _innerList.Count; n++)
						if (_innerList[n].Key == columnKey)
							return _innerList[n];
					throw new KeyNotFoundException();
				}
			}

			public void Add(StudyFilterColumn item)
			{
				if (_innerList.Contains(item))
					throw new ArgumentException("Column already exists in collection.", "item");
				int index = _innerList.Count;
				_innerList.Add(item);
				_owner.ColumnInserted(index, item);
				item.Owner = _owner;
			}

			public void Clear()
			{
				foreach (StudyFilterColumn column in _innerList)
					column.Owner = null;

				_innerList.Clear();
				_owner.ColumnsChanged(this);

				foreach (StudyFilterColumn column in _innerList)
					column.Owner = _owner;
			}

			public bool Contains(StudyFilterColumn item)
			{
				return _innerList.Contains(item);
			}

			public bool Contains(string columnKey)
			{
				for (int n = 0; n < _innerList.Count; n++)
					if (_innerList[n].Key == columnKey)
						return true;
				return false;
			}

			void ICollection<StudyFilterColumn>.CopyTo(StudyFilterColumn[] array, int arrayIndex)
			{
				_innerList.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _innerList.Count; }
			}

			bool ICollection<StudyFilterColumn>.IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(StudyFilterColumn item)
			{
				int index = _innerList.IndexOf(item);
				if (index >= 0)
				{
					_innerList.RemoveAt(index);
					_owner.ColumnRemoved(index, item);
					item.Owner = null;
					return true;
				}
				return false;
			}

			public IEnumerator<StudyFilterColumn> GetEnumerator()
			{
				return _innerList.GetEnumerator();
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				foreach (StudyFilterColumn column in this)
					yield return column.Key;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerable<uint> DicomTags
			{
				get
				{
					foreach (StudyFilterColumn column in this)
					{
						if (column is IDicomTagColumn)
						{
							yield return ((IDicomTagColumn) column).Tag;
						}
					}
				}
			}
		}
	}
}