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
using System;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "folderexplorer-folders-toolbar/MenuRefreshAll", "Refresh")]
	[MenuAction("apply", "folderexplorer-folders-contextmenu/MenuRefreshAll", "Refresh")]
	[Tooltip("apply", "TooltipRefreshFolders")]
	[IconSet("apply", "Icons.RefreshSmall.png", "Icons.RefreshMedium.png", "Icons.RefreshLarge.png")]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class FolderRefreshTool : Tool<IFolderExplorerGroupToolContext>
	{
		public void Refresh()
		{
			if(this.Context.SelectedFolderSystem != null)
			{
				this.Context.SelectedFolderSystem.InvalidateFolders(true);
			}
		}
	}
}