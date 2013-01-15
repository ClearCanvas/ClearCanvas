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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Represents the current message object's (e.g. <see cref="KeyboardButtonMessage"/>) state.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="KeyboardButtonDownPreview"/>
	/// <seealso cref="KeyboardButtonMessage"/>
	public sealed class KeyboardButtonShortcut : IEquatable<KeyboardButtonShortcut>, IEquatable<XKeys>
	{
		private readonly XKeys _keyData;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyboardButtonShortcut(XKeys keyData)
		{
			_keyData = keyData;
		}

        /// <summary>
        /// Gets whether or not this is an empty shortcut (e.g. no keystrokes).
        /// </summary>
        public bool IsEmpty { get { return _keyData == XKeys.None; } }
		
        /// <summary>
		/// Gets the Key Data, in its entirety.
		/// </summary>
		public XKeys KeyData
		{
			get { return _keyData; }
		}

		/// <summary>
		/// Gets the Key Code (modifiers removed), which is extracted from the <see cref="KeyData"/>.
		/// </summary>
		public XKeys KeyCode
		{
			get { return _keyData & XKeys.KeyCode; }
		}

		/// <summary>
		/// Indicates whether the Control key is down.
		/// </summary>
		public bool Control
		{
			get { return ((_keyData & XKeys.Control) == XKeys.Control); }
		}

		/// <summary>
		/// Indicates whether the Alt key is down.
		/// </summary>
		public bool Alt
		{
			get { return ((_keyData & XKeys.Alt) == XKeys.Alt); }
		}

		/// <summary>
		/// Indicates whether the Shift key is down.
		/// </summary>
		public bool Shift
		{
			get { return ((_keyData & XKeys.Shift) == XKeys.Shift); }
		}

		/// <summary>
		/// Gets a hashcode for this object instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _keyData.GetHashCode();
		}

		/// <summary>
		/// Determines if this object is equal to another.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is KeyboardButtonShortcut)
			{
				return this.Equals((KeyboardButtonShortcut)obj);
			}
			if (obj is XKeys)
			{
				return this.Equals((XKeys)obj);
			}

			return false;
		}

		#region IEquatable<KeyboardButtonShortcut> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(KeyboardButtonShortcut other)
		{
			return (other != null && other.KeyData == this.KeyData);
		}

		#endregion

		#region IEquatable<XKeys> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(XKeys other)
		{
			return this.KeyData == other;
		}

		#endregion

		/// <summary>
		/// Gets a string describing this object instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _keyData.ToString();
		}
	}
}
