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

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to an application component that acts as an explorer for
	/// an underlying <see cref="IFolderSystem"/>.
	/// </summary>
	public interface IFolderExplorerComponent
	{
		/// <summary>
		/// Gets a value indicating whether this folder explorer has already been initialized.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Instructs the folder explorer to initialize (build the folder system).
		/// </summary>
		void Initialize();

		/// <summary>
		/// Occurs when asynchronous initialization of this folder system has completed.
		/// </summary>
		event EventHandler Initialized;

		/// <summary>
		/// Invalidates all folders.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Gets the underlying folder system associated with this folder explorer.
		/// </summary>
		IFolderSystem FolderSystem { get; }

		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
		IFolder SelectedFolder { get; set; }

		/// <summary>
		/// Occurs when the selected folder changes.
		/// </summary>
		event EventHandler SelectedFolderChanged;

		/// <summary>
		/// Executes a search on the underlying folder system.
		/// </summary>
		/// <param name="searchParams"></param>
		void ExecuteSearch(SearchParams searchParams);

		/// <summary>
		/// Launches the advanced search component for the underlying folder system.
		/// </summary>
		void LaunchAdvancedSearchComponent();

		/// <summary>
		/// Gets the application component that displays the content of a folder for this folder system.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetContentComponent();
	}
}