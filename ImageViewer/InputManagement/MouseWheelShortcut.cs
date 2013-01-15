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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Represents the current message object's (e.g. <see cref="MouseWheelMessage"/>) state.
	/// </summary>
	/// <seealso cref="MouseWheelMessage"/>
	public sealed class MouseWheelShortcut : IEquatable<MouseWheelShortcut>, IEquatable<Modifiers>
	{
		private readonly Modifiers _modifiers;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut()
			: this(false, false, false)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(Modifiers modifiers)
		{
			_modifiers = modifiers ?? new Modifiers(ModifierFlags.None);
			_description = String.Format(SR.FormatMouseWheelShortcutDescription, _modifiers.ToString());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(ModifierFlags modifierFlags)
			: this(new Modifiers(modifierFlags))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(bool control, bool alt, bool shift)
			: this(new Modifiers(control, alt, shift))
		{
		}

		/// <summary>
		/// Gets the state of the modifier keys as a <see cref="ModifierFlags"/>.
		/// </summary>
		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		/// <summary>
		/// Determines if another object instance is equal to this one.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is MouseWheelShortcut)
				return this.Equals((MouseWheelShortcut)obj);
			else if (obj is Modifiers)
				return this.Equals((Modifiers) obj);

			return false;
		}

		#region IEquatable<MouseWheelShortcut> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(MouseWheelShortcut other)
		{
			return other != null && Modifiers.Equals(other.Modifiers);
		}

		#endregion

		#region IEquatable<Modifiers> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(Modifiers other)
		{
			return other != null && Modifiers.Equals(other);
		}

		#endregion

		/// <summary>
		/// Gets a hash code for this object instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _modifiers.GetHashCode();
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
