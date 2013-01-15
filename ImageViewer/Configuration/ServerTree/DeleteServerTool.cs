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

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ButtonAction("activate", "servertree-toolbar/ToolbarDelete", "DeleteServerServerGroup")]
	[MenuAction("activate", "servertree-contextmenu/MenuDelete", "DeleteServerServerGroup")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class DeleteServerTool : ServerTreeTool
	{
		public DeleteServerTool()
		{
		}

		private void DeleteServerServerGroup()
		{
			ServerTree serverTree = this.Context.ServerTree;
			if (serverTree.CurrentNode.IsServer)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteServer, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
					return;

				this.Context.UpdateType = (int)ServerUpdateType.Delete;
                serverTree.DeleteCurrentNode();
                serverTree.Save();
                this.Context.UpdateType = (int)ServerUpdateType.None; 
			}
			else if (serverTree.CurrentNode.IsServerGroup)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteServerGroup, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
					return;

				this.Context.UpdateType = (int)ServerUpdateType.Delete;
                try
                {
                    serverTree.DeleteCurrentNode();
                    serverTree.Save();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, Context.DesktopWindow);
                }

                this.Context.UpdateType = (int)ServerUpdateType.None; 
			}
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.IsReadOnly)
			{
				this.Enabled = false;
			}
			else
			{
				//enable only if it's a server or server group, and is not the "My Servers" root node.
				this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
				               this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootServerGroup;
			}
		}
	}
}