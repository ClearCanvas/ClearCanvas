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
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's default <see cref="MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="IViewerShortcutManager"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DefaultMouseToolButtonAttribute : Attribute
	{
		private readonly MouseButtonShortcut _shortcut;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultMouseToolButtonAttribute(XMouseButtons mouseButton)
		{
			_shortcut = new MouseButtonShortcut(mouseButton);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultMouseToolButtonAttribute(XMouseButtons mouseButton, ModifierFlags modifierFlags)
		{
			_shortcut = new MouseButtonShortcut(mouseButton, modifierFlags);
		}

		/// <summary>
		/// Gets the associated <see cref="MouseButtonShortcut"/>.
		/// </summary>
		public MouseButtonShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}
