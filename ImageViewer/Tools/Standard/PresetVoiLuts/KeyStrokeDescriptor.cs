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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class KeyStrokeDescriptor : IEquatable<KeyStrokeDescriptor>, IEquatable<XKeys>
	{
		private readonly XKeys _keyStroke;

		internal KeyStrokeDescriptor(XKeys keyStroke)
		{
			_keyStroke = keyStroke;
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
		}

		public override string ToString()
		{
			if (_keyStroke == XKeys.None)
				return String.Format("({0})", SR.None);

			return _keyStroke.ToString();
		}

		public override int GetHashCode()
		{
			return _keyStroke.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is KeyStrokeDescriptor)
				return this.Equals((KeyStrokeDescriptor)obj);

			if (obj is XKeys)
				return this.Equals((XKeys)obj);

			return false;
		}

		#region IEquatable<KeyStrokeDescriptor> Members

		public bool Equals(KeyStrokeDescriptor other)
		{
			if (other == null)
				return false;

			return this.KeyStroke == other.KeyStroke;
		}

		#endregion

		#region IEquatable<XKeys> Members

		public bool Equals(XKeys other)
		{
			return _keyStroke == other;
		}

		#endregion

		public static implicit operator XKeys(KeyStrokeDescriptor descriptor)
		{
			return descriptor._keyStroke;
		}

		public static implicit operator KeyStrokeDescriptor(XKeys keyStroke)
		{
			return new KeyStrokeDescriptor(keyStroke);
		}
	}
}
