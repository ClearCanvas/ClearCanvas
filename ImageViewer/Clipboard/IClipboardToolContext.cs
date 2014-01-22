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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// The clipboard tool context.
	/// </summary>
	/// <remarks>
	/// Provides clipboard tools access to items in the clipboard.
	/// </remarks>
	public interface IClipboardToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> in which the clipboard is hosted.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the parent clipboard component.
		/// </summary>
		ClipboardComponent Component { get; }

		/// <summary>
		/// Gets a list of the items in the clipboard.
		/// </summary>
		IList<IClipboardItem> ClipboardItems { get; }

		/// <summary>
		/// Gets a list of the selected items in the clipboard.
		/// </summary>
		ReadOnlyCollection<IClipboardItem> SelectedClipboardItems { get; }

		/// <summary>
		/// Occurs when the list of clipboard items has changed.
		/// </summary>
		event EventHandler ClipboardItemsChanged;

		/// <summary>
		/// Occurs when the list of selected clipboard items has changed.
		/// </summary>
		event EventHandler SelectedClipboardItemsChanged;
	}
}