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
	/// Represents the current message object's (e.g. <see cref="MouseButtonMessage"/>) state.
	/// </summary>
	/// <seealso cref="MouseButtonMessage"/>
	public sealed class MouseButtonShortcut : IEquatable<MouseButtonShortcut>, IEquatable<XMouseButtons>
	{
		private readonly XMouseButtons _mouseButton;
		private readonly Modifiers _modifiers;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonShortcut(XMouseButtons mouseButton, ModifierFlags modifierFlags)
			: this(mouseButton, new Modifiers(modifierFlags))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonShortcut(XMouseButtons mouseButton, Modifiers modifiers)
		{
			_mouseButton = mouseButton;
			_modifiers = modifiers ?? new Modifiers(ModifierFlags.None);
			_description = String.Format(SR.FormatMouseButtonShortcutDescription, _mouseButton.ToString(), _modifiers.ToString());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonShortcut(XMouseButtons mouseButton, bool control, bool alt, bool shift)
			: this(mouseButton, new Modifiers(control, alt, shift))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonShortcut(XMouseButtons mouseButton)
			: this(mouseButton, false, false, false)
		{
		}
		
		/// <summary>
		/// Gets the currently depressed mouse button, if any.
		/// </summary>
		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		/// <summary>
		/// Gets the current state of the modifier keys as a <see cref="Modifiers"/> object.
		/// </summary>
		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		/// <summary>
		/// Returns whether or not the shortcut is modified (any modifier keys pressed).
		/// </summary>
		public bool IsModified
		{
			get { return _modifiers.ModifierFlags != ModifierFlags.None; }
		}

		/// <summary>
		/// Determines if this object instance is equal to another.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is MouseButtonShortcut)
				return this.Equals((MouseButtonShortcut)obj);
			else if (obj is XMouseButtons)
				return this.Equals((XMouseButtons) obj);

			return false;
		}

		#region IEquatable<MouseButtonShortcut> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(MouseButtonShortcut other)
		{
			return other != null && other.MouseButton == this.MouseButton && other.Modifiers.Equals(this.Modifiers);
		}

		#endregion

		#region IEquatable<XMouseButtons> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(XMouseButtons other)
		{
			return this.Modifiers.Equals(ModifierFlags.None) && this.MouseButton == other;
		}

		#endregion

		/// <summary>
		/// Gets a hash code for this object instance.
		/// </summary>
		public override int GetHashCode()
		{
			int returnvalue = 7;
			returnvalue = 11 * returnvalue + _modifiers.GetHashCode();
			returnvalue = 11 * returnvalue + _mouseButton.GetHashCode();
			return returnvalue;
		}

		/// <summary>
		/// Gets a string describing this object instance.
		/// </summary>
		public override string ToString()
		{
			return _description;
		}
	}
}
