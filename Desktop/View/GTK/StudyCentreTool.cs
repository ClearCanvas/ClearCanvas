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
using System.Drawing;
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
//using ClearCanvas.ImageViewer.Tools;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;

using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	[MenuAction("activate", "MenuFile/MenuFileSearch")]
	[ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardStudyCentre")]
	[ClickHandler("activate", "Activate")]
//	[ClearCanvas.Workstation.Model.Actions.IconSet("activate", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]
	[ClearCanvas.Desktop.Actions.IconSet("activate", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]
	
//	[ExtensionOf(typeof(ClearCanvas.Workstation.Model.WorkstationToolExtensionPoint))]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class StudyCentreTool : StockTool
	{
		
		public StudyCentreTool()
		{
		}

		public void Activate()
		{
			object[] buttonResponses = new object[] {"Accept", ResponseType.Accept, "Cancel", ResponseType.Cancel};
			FileChooserDialog fileDialog = new FileChooserDialog("Local Studies", (Window)_mainView.GuiElement, FileChooserAction.SelectFolder, buttonResponses);
			
			int result = fileDialog.Run();
			string folder = fileDialog.Filename;
			fileDialog.Destroy();	// must manually destroy the dialog
			
			if(result == (int)ResponseType.Accept)
			{
				LocalImageLoader loader = new LocalImageLoader();
				string studyUID = loader.Load(folder);
				//if(studyUID == "" || WorkstationModel.StudyManager.StudyTree.GetStudy(studyUID) == null)
				if(studyUID == "" || ImageWorkspace.StudyManager.StudyTree.GetStudy(studyUID) == null)
				{
					//Platform.ShowMessageBox(ClearCanvas.Workstation.Model.SR.ErrorUnableToLoadStudy);
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
				}
				else
				{
					ImageWorkspace ws = new ImageWorkspace(studyUID);
					//WorkstationModel.WorkspaceManager.Workspaces.Add(ws);
					DesktopApplication.WorkspaceManager.Workspaces.Add(ws);
				}
			}
		}
	}
}
