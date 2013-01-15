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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutComparer : IComparer<PresetVoiLut>
	{
		#region IComparer<PresetVoiLutCollection> Members

		public int Compare(PresetVoiLut x, PresetVoiLut y)
		{
			if (x.KeyStroke == XKeys.None)
			{
				if (y.KeyStroke == XKeys.None)
					return 0;

				return 1;
			}
			else if (y.KeyStroke == XKeys.None)
			{
				return -1; 
			}

			if (x.KeyStroke < y.KeyStroke)
			{
				return -1;
			}
			else if (x.KeyStroke == y.KeyStroke)
			{
				int nameCompare = x.Operation.Name.CompareTo(y.Operation.Name);
				if (nameCompare < 0)
					return -1;
				else if (nameCompare == 0)
					return 0;
			}

			return 1; ;
		}

		#endregion
	}

	internal sealed class PresetVoiLutCollection : IList<PresetVoiLut>
	{
		private readonly List<PresetVoiLut> _presets;

		public PresetVoiLutCollection()
		{
			_presets = new List<PresetVoiLut>();
		}

		#region IList<PresetVoiLut> Members

		public int IndexOf(PresetVoiLut item)
		{
			return _presets.IndexOf(item);
		}

		public void Insert(int index, PresetVoiLut item)
		{
			if (_presets.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

			_presets.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_presets.RemoveAt(index);
		}

		public PresetVoiLut this[int index]
		{
			get
			{
				return _presets[index];
			}
			set
			{
				if (_presets.Contains(value))
					throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

				_presets[index] = value;
			}
		}

		#endregion

		#region ICollection<PresetVoiLut> Members

		public void Add(PresetVoiLut item)
		{
			if (_presets.Contains(item))
				throw new InvalidOperationException(SR.ExceptionAnEquivalentPresetAlreadyExists);

			_presets.Add(item);
		}

		public void Clear()
		{
			_presets.Clear();
		}

		public bool Contains(PresetVoiLut item)
		{
			return _presets.Contains(item);
		}

		public void CopyTo(PresetVoiLut[] array, int arrayIndex)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public int Count
		{
			get { return _presets.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PresetVoiLut item)
		{
			return _presets.Remove(item);
		}

		#endregion

		#region IEnumerable<PresetVoiLut> Members

		public IEnumerator<PresetVoiLut> GetEnumerator()
		{
			return _presets.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _presets.GetEnumerator();
		}

		#endregion

		public void Sort(IComparer<PresetVoiLut> comparer)
		{
			_presets.Sort(comparer);
		}

		public PresetVoiLutCollection Clone()
		{
			PresetVoiLutCollection clone = new PresetVoiLutCollection();
			foreach (PresetVoiLut preset in _presets)
				clone.Add(preset.Clone());

			return clone;
		}
	}
}
