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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[MenuAction("Open", "global-menus/MenuTools/MenuUtilities/MenuStudyFilters", "Open")]
	[IconSet("Open", "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class LaunchStudyFiltersTool : Tool<IDesktopToolContext>
	{
		private string _lastFolder = string.Empty;

		public void Open()
		{
			SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
			args.AllowCreateNewFolder = false;
			args.Path = _lastFolder;
			args.Prompt = SR.MessageSelectFolderToFilter;

			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastFolder = result.FileName;

				StudyFilterComponent component = new StudyFilterComponent();
				component.BulkOperationsMode = true;

				if (component.Load(base.Context.DesktopWindow, true, result.FileName))
				{
					component.Refresh(true);
					base.Context.DesktopWindow.Workspaces.AddNew(component, SR.StudyFilters);
				}

				component.BulkOperationsMode = false;
			}
		}
	}
}