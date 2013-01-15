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
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
using ClearCanvas.Desktop;
using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	//[ExtensionOf(typeof(ClearCanvas.Workstation.Model.DesktopViewExtensionPoint))]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopViewExtensionPoint))]
	//public class WorkstationView : GtkView, IWorkstationView
	public class DesktopView : GtkView, IDesktopView
	{
		private static MainWindow _mainWin;

		//public WorkstationView()
		public DesktopView()
		{
		}
		
		private void Initialize()
		{
            Gtk.Application.Init();
            _mainWin = new MainWindow();
			_mainWin.DeleteEvent += OnDeleteMainWindow;
		}
		
		internal MainWindow MainWindow
		{
			get { return _mainWin; }
		}
		
		/// <summary>
		/// Starts the message pump of the underlying GUI toolkit.  Typically this method is expected to
		/// block for the duration of the application's execution.
		/// </summary>
		/// <remarks>
		/// The method assumes that the view relies on an underlying message pump, as most
		/// desktop GUI toolkits do.  This may need to change if a non-desktop (ie web) view
		/// is implemented.
		/// </remarks>
		public void RunMessagePump()
		{
			Initialize();
			Gtk.Application.Run();
		}
		
		/// <summary>
		/// Stops the underlying message pump, typically just prior to the termination of the application.
		/// </summary>
		public void QuitMessagePump()
		{
			Gtk.Application.Quit();
		}
		
		///<summary>
		/// Returns the GTK widget that implements this view, allowing a parent view to insert
		/// the widget as one of its children.
		/// </summary>
		public override object GuiElement
		{
			get { return _mainWin; }
		}
		
		public void CloseActiveWorkspace()
        {
           IWorkspace workspace = _mainWin.ActiveWorkspace;
			if(workspace != null)
			{
				//WorkstationModel.WorkspaceManager.Workspaces.Remove(workspace);
				DesktopApplication.WorkspaceManager.Workspaces.Remove(workspace);
			}
        }

		// Handles the main window close event
		private void OnDeleteMainWindow(object sender, EventArgs e)
        {
			QuitMessagePump();
        }
		
	}
}
