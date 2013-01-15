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

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// A message object created by the view layer to allow a controlling object 
	/// (e.g. <see cref="TileController"/>) to handle mouse button messages.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="TileController"/>
	public sealed class MouseButtonMessage
	{
		/// <summary>
		/// An enum used to indicate the button state.
		/// </summary>
		public enum ButtonActions
		{
			/// <summary>
			/// Indicates that the button is down.
			/// </summary>
			Down,

			/// <summary>
			/// Indicates that the button has been released.
			/// </summary>
			Up
		};


		private readonly ButtonActions _buttonAction;
		private readonly Point _location; 
		private readonly MouseButtonShortcut _mouseButtonShortcut;
		private readonly uint _clickCount;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, bool control, bool alt, bool shift)
		{
			_location = location;
			_buttonAction = buttonAction;
			_clickCount = clickCount;
			_mouseButtonShortcut = new MouseButtonShortcut(mouseButton, control, alt, shift);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount, ModifierFlags modifierFlags)
			: this(location, mouseButton, buttonAction, clickCount, 
						(modifierFlags & ModifierFlags.Control) == ModifierFlags.Control,
						(modifierFlags & ModifierFlags.Alt) == ModifierFlags.Alt,
						(modifierFlags & ModifierFlags.Shift) == ModifierFlags.Shift)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseButtonMessage(Point location, XMouseButtons mouseButton, ButtonActions buttonAction, uint clickCount)
			: this(location, mouseButton, buttonAction, clickCount, false, false, false)
		{
		}

		/// <summary>
		/// Gets the mouse location.
		/// </summary>
		public Point Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Gets the current button state.
		/// </summary>
		public ButtonActions ButtonAction
		{
			get { return _buttonAction; }
		}

		/// <summary>
		/// Gets the associated <see cref="MouseButtonShortcut"/>.
		/// </summary>
		public MouseButtonShortcut Shortcut
		{
			get { return _mouseButtonShortcut; }
		}

		/// <summary>
		/// Gets the current mouse button click count.
		/// </summary>
		public uint ClickCount
		{
			get { return _clickCount; }
		}
	}
}
