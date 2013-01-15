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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLutGroup : IEquatable<PresetVoiLutGroup>, IComparable<PresetVoiLutGroup>
	{
		private readonly string _modality;
		private readonly PresetVoiLutCollection _presets;
	
		public PresetVoiLutGroup(string modality)
		{
			Platform.CheckForEmptyString(modality, "modality");
			_modality = modality;
			_presets = new PresetVoiLutCollection();
		}

		public string Modality
		{
			get { return _modality; }	
		}

		public PresetVoiLutCollection Presets
		{
			get { return _presets; }
		}

		public bool AppliesTo(ImageSop sop)
		{
			return sop.Modality == _modality;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PresetVoiLutGroup)
				return this.Equals((PresetVoiLutGroup) obj);

			return false;
		}

		#region IEquatable<PresetVoiLutGroup> Members

		public bool Equals(PresetVoiLutGroup other)
		{
			if (other == null)
				return false;

			return this._modality == other._modality;
		}

		#endregion

		#region IComparable<PresetVoiLutGroup> Members

		public int CompareTo(PresetVoiLutGroup other)
		{
			return this.Modality.CompareTo(other.Modality);
		}

		#endregion

		internal PresetVoiLutGroup Clone()
		{
			PresetVoiLutGroup clone = new PresetVoiLutGroup(this.Modality);
			foreach (PresetVoiLut preset in _presets)
				clone._presets.Add(preset.Clone());

			return clone;
		}
	}
}
