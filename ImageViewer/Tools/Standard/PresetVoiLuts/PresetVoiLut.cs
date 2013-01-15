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
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal sealed class PresetVoiLut : IEquatable<PresetVoiLut>
	{
		private KeyStrokeDescriptor _keyStrokeDescriptor;
		private readonly IPresetVoiLutOperation _operation;

		public PresetVoiLut(IPresetVoiLutOperation operation)
		{
			Platform.CheckForNullReference(operation, "operation");
			this._operation = operation;
			_keyStrokeDescriptor = XKeys.None;
		}

		public KeyStrokeDescriptor KeyStrokeDescriptor
		{
			get { return _keyStrokeDescriptor; }	
		}

		public XKeys KeyStroke
		{
			get { return _keyStrokeDescriptor.KeyStroke; }
			set { _keyStrokeDescriptor = value; }
		}

		public IPresetVoiLutOperation Operation
		{
			get { return _operation; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			if (obj is PresetVoiLut)
				return this.Equals((PresetVoiLut) obj);

			return false;
		}

		#region IEquatable<PresetVoiLut> Members

		public bool Equals(PresetVoiLut other)
		{
			if (other == null)
				return false;

			return (String.Compare(this.Operation.Name, other.Operation.Name, true) == 0 || (KeyStroke != XKeys.None && KeyStroke == other.KeyStroke));
		}

		#endregion

		public PresetVoiLut Clone()
		{
			PresetVoiLut clone = new PresetVoiLut(_operation);
			clone.KeyStroke = this.KeyStroke;
			return clone;
		}
	}
}
