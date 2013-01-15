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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutGroupCollection : IList<PresetVoiLutGroup>
	{
		private readonly List<PresetVoiLutGroup> _groups;

		public PresetVoiLutGroupCollection()
		{
			_groups = new List<PresetVoiLutGroup>();
		}

		#region IList<PresetVoiLutGroup> Members

		public int IndexOf(PresetVoiLutGroup item)
		{
			return _groups.IndexOf(item);
		}

		public void Insert(int index, PresetVoiLutGroup item)
		{
			if (_groups.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

			_groups.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_groups.RemoveAt(index);
		}

		public PresetVoiLutGroup this[int index]
		{
			get
			{
				return _groups[index];
			}
			set
			{
				if (_groups.Contains(value))
					throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

				_groups[index] = value;
			}
		}

		#endregion

		#region ICollection<PresetVoiLutGroup> Members

		public void Add(PresetVoiLutGroup item)
		{
			if (_groups.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetGroupAlreadyExists);

			_groups.Add(item);
		}

		public void Clear()
		{
			_groups.Clear();
		}

		public bool Contains(PresetVoiLutGroup item)
		{
			return _groups.Contains(item);
		}

		public void CopyTo(PresetVoiLutGroup[] array, int arrayIndex)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public int Count
		{
			get { return _groups.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PresetVoiLutGroup item)
		{
			return _groups.Remove(item);
		}

		#endregion

		#region IEnumerable<PresetVoiLutGroup> Members

		public IEnumerator<PresetVoiLutGroup> GetEnumerator()
		{
			return _groups.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _groups.GetEnumerator();
		}

		#endregion

		public void Sort()
		{
			_groups.Sort();
		}

		public PresetVoiLutGroupCollection Clone()
		{
			PresetVoiLutGroupCollection clone = new PresetVoiLutGroupCollection();
			foreach (PresetVoiLutGroup group in _groups)
				clone.Add(group.Clone());

			return clone;
		}
	}
}
