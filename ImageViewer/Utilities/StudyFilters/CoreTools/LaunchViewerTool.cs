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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("launch", DefaultToolbarActionSite + "/ToolbarLaunchInViewer", "Launch")]
	[MenuAction("launch", DefaultContextMenuActionSite + "/MenuLaunchInViewer", "Launch")]
	[EnabledStateObserver("launch", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("launch", "OpenToolSmall.png", "OpenToolSmall.png", "OpenToolSmall.png")]
	[ViewerActionPermission("launch", ImageViewer.AuthorityTokens.Study.Open)]
	[ExtensionOf(typeof(StudyFilterToolExtensionPoint))]
	public class LaunchViewerTool : StudyFilterTool
	{
		public void Launch()
		{
			if (base.SelectedItems == null || base.SelectedItems.Count == 0)
				return;

			int n = 0;
			string[] selection = new string[base.SelectedItems.Count];
			foreach (IStudyItem item in base.SelectedItems)
			{
				if (!string.IsNullOrEmpty(item.Filename))
					selection[n++] = item.Filename;
			}

			bool cancelled = true;
			ImageViewerComponent viewer = new ImageViewerComponent();
			try
			{
				viewer.LoadImages(selection, base.Context.DesktopWindow, out cancelled);
			}
			catch (Exception ex)
			{
				base.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
			}

			if (cancelled)
			{
				viewer.Dispose();
				return;
			}

			try
			{
				LaunchImageViewerArgs launchArgs = new LaunchImageViewerArgs(WindowBehaviour.Auto);
				ImageViewerComponent.Launch(viewer, launchArgs);
			}
			catch (Exception ex)
			{
				base.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				Platform.Log(LogLevel.Error, ex, "ImageViewerComponent launch failure.");
			}
		}
	}
}