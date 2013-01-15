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

using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Used internally by the framework to lookup different interfaces registered by, say, an <see cref="ImageViewerComponent"/> 
	/// that correspond to different mouse/keyboard shortcuts.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The shortcuts are usually associated with an <see cref="ClearCanvas.Desktop.Tools.ITool"/> having one or more of 
	/// the <see cref="MenuActionAttribute"/> or <see cref="KeyboardActionAttribute"/> attributes defined, or implementing
	/// either <see cref="IMouseButtonHandler"/> or <see cref="IMouseWheelHandler"/>.
	/// </para>
	/// <para>
	/// This interface is intended for internal framework use only.
	/// </para>
	/// </remarks>
	/// <seealso cref="IClickAction"/>
	/// <seealso cref="IMouseButtonHandler"/>
	/// <seealso cref="IMouseWheelHandler"/>
	/// <seealso cref="KeyboardButtonShortcut"/>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseWheelShortcut"/>
	public interface IViewerShortcutManager
	{
		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with the input <paramref name="shortcut"/>.
		/// </summary>
		/// <remarks>
		/// Will return null if there is no <see cref="IClickAction"/> associated with the <paramref name="shortcut"/>.
		/// </remarks>
		IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut);

		/// <summary>
		/// Gets the <see cref="IMouseButtonHandler"/>s assigned to the given shortcut.
		/// </summary>
		/// <remarks>
		/// In the case of <see cref="MouseImageViewerTool"/>s, the tool assigned to the specified shortcut
		/// is returned first, followed by any whose default shortcut matches the one specified.
		/// </remarks>
		IEnumerable<IMouseButtonHandler> GetMouseButtonHandlers(MouseButtonShortcut shortcut);

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with the input <paramref name="shortcut"/>.
		/// </summary>
		/// <remarks>
		/// Will return null if there is no <see cref="IMouseWheelHandler"/> associated with the <paramref name="shortcut"/>.
		/// </remarks>
		IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut);
	}
}
