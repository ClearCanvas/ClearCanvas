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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an image viewer.
	/// </summary>
	public interface IImageViewer : IDisposable
	{
		/// <summary>
		/// Get the host <see cref="IDesktopWindow"/>.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Gets the image viewer's <see cref="StudyTree"/>.
		/// </summary>
		StudyTree StudyTree { get; }

		/// <summary> 
		/// Gets the <see cref="ILayoutManager"/> associated with this image viewer. 
		/// </summary> 
		ILayoutManager LayoutManager { get; }

		/// <summary>
		/// Gets the <see cref="PhysicalWorkspace"/>.
		/// </summary>
		IPhysicalWorkspace PhysicalWorkspace { get; }

		/// <summary>
		/// Gets the <see cref="LogicalWorkspace"/>.
		/// </summary>
		ILogicalWorkspace LogicalWorkspace { get; }

		/// <summary>
		/// Gets the <see cref="EventBroker"/>.
		/// </summary>
		EventBroker EventBroker { get; }

		/// <summary>
		/// Occurs when the <see cref="IImageViewer"/> is in the process of closing.
		/// </summary>
		/// <remarks>
		/// This event is separate and distinct from <see cref="IImageViewerToolContext.ViewerClosing"/> which is fired before the viewer
		/// closes and provides a mechanism for viewer tools to abort closing the viewer.
		/// </remarks>
		event EventHandler Closing;

		/// <summary>
		/// Gets the currently selected <see cref="IImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		IImageBox SelectedImageBox { get; }

		/// <summary>
		/// Gets the currently selected <see cref="ITile"/>
		/// </summary>
		/// <value>The currently selected <see cref="ITile"/>, or <b>null</b> if
		/// no <see cref="ITile"/> is currently selected.</value>
		ITile SelectedTile { get; }

		/// <summary>
		/// Gets the currently selected <see cref="IPresentationImage"/>
		/// </summary>
		/// <value>The currently selected <see cref="IPresentationImage"/>, or <b>null</b> if
		/// no <see cref="IPresentationImage"/> is currently selected.</value>
		IPresentationImage SelectedPresentationImage { get; }

		/// <summary>
		/// Gets the <see cref="CommandHistory"/> for this image viewer.
		/// </summary>
		CommandHistory CommandHistory { get; }

		/// <summary>
		/// Gets a string containing the patients currently loaded in this
		/// <see cref="IImageViewer"/>.
		/// </summary>		
		string PatientsLoadedLabel { get; }

		/// <summary>
		/// Gets the <see cref="IAction"/>s exported by this <see cref="IImageViewer"/>.
		/// </summary>
		IActionSet ExportedActions { get; }

		/// <summary>
		/// Gets the namespace that qualifies the global action models owned by this <see cref="IImageViewer"/>. This value may not be null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This namespace only applies to the global action models (i.e. global menu and toolbar sites) when the
		/// <see cref="IImageViewer"/> is launched as a <see cref="IWorkspace"/>. In contrast, the namespace returned
		/// by <see cref="ActionsNamespace"/> applies to local action models such as the viewer context menu.
		/// </para>
		/// </remarks>
		string GlobalActionsNamespace { get; }

		/// <summary>
		/// Gets the namespace that qualifies the global action models owned by this <see cref="IImageViewer"/>. This value may not be null.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This namespace only applies to local action models such as the viewer context menu. In contrast,
		/// the namespace returned by <see cref="GlobalActionsNamespace"/> applies to the global action models
		/// (i.e. global menu and toolbar sites) when the <see cref="IImageViewer"/> is launched as a <see cref="IWorkspace"/>.
		/// </para>
		/// </remarks>
		string ActionsNamespace { get; }

		/// <summary>
		/// Gets the associated <see cref="IPriorStudyLoader"/>.
		/// </summary>
		IPriorStudyLoader PriorStudyLoader { get; }

		/// <summary>
		/// A place for extensions to store custom data about the viewer.
		/// </summary>
		ExtensionData ExtensionData { get; }

		/// <summary>
		/// Loads a study using the specified parameters.
		/// </summary>
		/// <remarks>After this method is executed, the image viewer's <see cref="StudyTree"/>
		/// will be populated with the appropriate <see cref="Study"/>, <see cref="Series"/> 
		/// and <see cref="Sop"/> objects.
		/// 
		/// By default, the Framework provides an implementation of 
		/// <see cref="IStudyLoader"/> called <b>LocalStoreStudyLoader</b> which loads
		/// studies from the local database.  If you have implemented your own 
		/// <see cref="IStudyLoader"/> and want to load a study using that implementation,
		/// just pass in the name provided by <see cref="IStudyLoader.Name"/> as the source.
		/// </remarks>
		/// <param name="loadStudyArgs">A <see cref="LoadStudyArgs"/> object containing information about the study to be loaded.</param>
		/// <exception cref="InUseLoadStudyException">The specified study is in use and cannot be opened at this time.</exception>
		/// <exception cref="NearlineLoadStudyException">The specified study is nearline and cannot be opened at this time.</exception>
		/// <exception cref="OfflineLoadStudyException">The specified study is offline and cannot be opened at this time.</exception>
		/// <exception cref="NotFoundLoadStudyException">The specified study could not be found.</exception>
		/// <exception cref="LoadStudyException">One or more images could not be opened, or an unspecified error has occurred.</exception>
		/// <exception cref="StudyLoaderNotFoundException">The specified <see cref="IStudyLoader"/> could not be found.</exception>
		void LoadStudy(LoadStudyArgs loadStudyArgs);

		/// <summary>
		/// Loads images with the specified file paths.
		/// </summary>
		/// <param name="path">The file path of the image.</param>
		/// <exception cref="LoadSopsException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		void LoadImages(string[] path);

		/// <summary>
		/// Loads images with the specified file paths and displays a progress bar.
		/// </summary>
		/// <param name="files">A list of file paths.</param>
		/// <param name="desktop">The desktop window.  This is necessary for
		/// a progress bar to be shown.</param>
		/// <param name="cancelled">A value that indicates whether the operation
		/// was cancelled.</param>
		/// <exception cref="LoadSopsException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		void LoadImages(string[] files, IDesktopWindow desktop, out bool cancelled);

		/// <summary>
		/// Lays out the images in the <see cref="IImageViewer"/> using
		/// the current layout manager.
		/// </summary>
		void Layout();
	}
}