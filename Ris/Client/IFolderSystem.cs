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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface to a folder system
	/// </summary>
	public interface IFolderSystem : IDisposable
	{
		#region Initialization

		/// <summary>
		/// Initializes the folder system with a context.  Set <see cref="context"/> to null to clear context.
		/// </summary>
		/// <param name="context"></param>
		void SetContext(IFolderSystemContext context);

		/// <summary>
		/// Initialize the folder system.
		/// </summary>
		/// <remarks>
		/// This method will be called after <see cref="SetContext"/> has been called. 
		/// </remarks>
		void Initialize();

		/// <summary>
		/// Gets a value indicating whether initialization of this folder system can be deferred.
		/// </summary>
		/// <remarks>
		/// If the folder system displays a status message, such as a total item count, in the
		/// title bar of the explorer, then deferring initialization is probably not a good idea,
		/// since the title bar will remain empty until the folder system is initialized.
		/// </remarks>
		bool LazyInitialize { get; }

		#endregion

		#region Infrastructure

		/// <summary>
		/// Gets the ID that identifies the folder system
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the resource resolver that is used to resolve icons.
		/// </summary>
		IResourceResolver ResourceResolver { get; }


		/// <summary>
		/// Gets the desktop window of the folder explorer that is hosting this folder system.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		#endregion

		#region Title

		/// <summary>
		/// Gets the text that should be displayed for the folder system
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Occurs when the <see cref="Title"/> property changes.
		/// </summary>
		event EventHandler TitleChanged;

		/// <summary>
		/// Gets the iconset that should be displayed for the folder system
		/// </summary>
		IconSet TitleIcon { get; }

		/// <summary>
		/// Occurs when the <see cref="TitleIcon"/> property changes.
		/// </summary>
		event EventHandler TitleIconChanged;

		#endregion

		#region Folders collection

		/// <summary>
		/// Gets the list of folders that belong to this folder system
		/// </summary>
		ObservableList<IFolder> Folders { get; }

		/// <summary>
		/// Occurs to indicate that the entire folder system should be rebuilt.
		/// </summary>
		/// <remarks>
		/// This is distinct from incremental changes to the <see cref="Folders"/> collection,
		/// which can be observed via the <see cref="ObservableList{TItem}.ItemAdded"/> and
		/// <see cref="ObservableList{TItem}.ItemRemoved"/> events.
		/// </remarks>
		event EventHandler FoldersChanged;

		/// <summary>
		/// Occurs to indicate that one or more properties of a folder have changed and the UI needs to be updated.
		/// </summary>
		event EventHandler<ItemEventArgs<IFolder>> FolderPropertiesChanged;

		#endregion

		#region Tool sets

		/// <summary>
		/// Gets the toolset defined for the folders
		/// </summary>
		IToolSet FolderTools { get; }

		/// <summary>
		/// Gets the toolset defined for the items in a folder
		/// </summary>
		IToolSet ItemTools { get; }

		#endregion

		#region Content viewing

		/// <summary>
		/// Obtains the custom content viewing component for this folder system, or null if the default items/preview view should be used.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetContentComponent();

		/// <summary>
		/// Gets the URL of the preview page as a function of the specified folder and items.
		/// </summary>
		/// <remarks>
		/// If <see cref="GetContentComponent"/> return a non-null value, then the this method is not called.
		/// </remarks>
		string GetPreviewUrl(IFolder folder, object[] items);

		/// <summary>
		/// Gets the audit data for previewing the specified items.
		/// </summary>
		/// <param name="folder"> </param>
		/// <param name="items"> </param>
		/// <returns></returns>
		PreviewOperationAuditData[] GetPreviewAuditData(IFolder folder, object[] items);

		#endregion

		#region Folder Invalidation

		/// <summary>
		/// Invalidates all folders. Use this method judiciously,
		/// as invalidating all folders will increase load on the system.
		/// </summary>
		void InvalidateFolders();

		/// <summary>
		/// Invalidates all folders. Use this method judiciously,
		/// as invalidating all folders will increase load on the system.
		/// </summary>
		/// <param name="resetPage">Reset to the first page.</param>
		void InvalidateFolders(bool resetPage);

		/// <summary>
		/// Invalidates all folders of the specified class in this folder system.
		/// </summary>
		void InvalidateFolders(Type folderClass);

		/// <summary>
		/// Invalidates the specified folder.
		/// </summary>
		/// <param name="folder"></param>
		void InvalidateFolder(IFolder folder);

		/// <summary>
		/// Occurs when one or more folders in the system have been invalidated.
		/// </summary>
		event EventHandler FoldersInvalidated;

		#endregion

		#region Search support

		/// <summary>
		/// Gets a value indicating whether this folder system supports searching.
		/// </summary>
		bool SearchEnabled { get; }

		/// <summary>
		/// Gets a value indicating whether this folder system supports advanced searching.
		/// </summary>
		bool AdvancedSearchEnabled { get; }

		/// <summary>
		/// Get a message to describe the type of search performed.
		/// </summary>
		string SearchMessage { get; }

		/// <summary>
		/// Performs a search, if enabled.
		/// </summary>
		/// <param name="params"></param>
		void ExecuteSearch(SearchParams @params);

		/// <summary>
		/// Returns a <see cref="SearchParams"/> object appropriate to this folder system
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		SearchParams CreateSearchParams(string search);

		/// <summary>
		/// Launches a <see cref="SearchComponentBase"/> appropriate to this folder system
		/// </summary>
		void LaunchSearchComponent();

		/// <summary>
		/// Indicates the type of <see cref="SearchComponentBase"/> appropriate to this folder system
		/// </summary>
		Type SearchComponentType { get; }

		#endregion

	}
}
