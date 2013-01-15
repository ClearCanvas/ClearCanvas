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

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// An attribute used by <see cref="MouseImageViewerTool"/> to specify it's 
	/// default <see cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>.
	/// </summary>
	/// <seealso cref="MouseImageViewerTool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.MouseButtonShortcut"/>
	/// <seealso cref="ClearCanvas.ImageViewer.InputManagement.IViewerShortcutManager"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseToolButtonAttribute : Attribute
	{
		private readonly XMouseButtons _mouseButton;
		private readonly bool _initiallyActive;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseToolButtonAttribute(XMouseButtons mouseButton, bool initiallyActive)
		{
			_mouseButton = mouseButton;
			_initiallyActive = initiallyActive;
		}

		/// <summary>
		/// Gets the mouse button assigned to the <see cref="MouseImageViewerTool"/>.
		/// </summary>
		public XMouseButtons MouseButton
		{
			get { return _mouseButton; }
		}

		/// <summary>
		/// Gets whether or not the tool should be initially active upon opening the viewer.
		/// </summary>
		public bool InitiallyActive
		{
			get { return _initiallyActive; }
		}
	}
}