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
	/// Specifies a <see cref="ImageViewerTool"/>'s default <see cref="MouseWheelShortcut"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MouseWheelHandlerAttribute : Attribute
	{
		private readonly MouseWheelShortcut _shortcut;

		/// <summary>
		/// Constructor that accepts <see cref="ModifierFlags"/> as input.
		/// </summary>
		/// <param name="modifiers"></param>
		public MouseWheelHandlerAttribute(ModifierFlags modifiers)
		{
			_shortcut = new MouseWheelShortcut(modifiers);
		}

		/// <summary>
		/// Gets the <see cref="ImageViewerTool"/>'s <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut Shortcut
		{
			get { return _shortcut; }
		}
	}
}
