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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines interface to context for tools that operate on workflow folders.
	/// </summary>
    public interface IWorkflowFolderToolContext : IToolContext
    {
		/// <summary>
		/// Gets the set of folders in the folder system.
		/// </summary>
        IEnumerable<IFolder> Folders { get; }

		/// <summary>
		/// Gets the currently selected folder, or null if no folder is selected.
		/// </summary>
        IFolder SelectedFolder { get; }

		/// <summary>
		/// Occurs when <see cref="SelectedFolder"/> changes.
		/// </summary>
        event EventHandler SelectedFolderChanged;

		/// <summary>
		/// Gets the desktop window.
		/// </summary>
        IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Invalidates all folders in the folder system.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Invalidates all folders of the specified class in the folder system.
		/// </summary>
		void InvalidateFolders(Type folderClass);
    }
}
