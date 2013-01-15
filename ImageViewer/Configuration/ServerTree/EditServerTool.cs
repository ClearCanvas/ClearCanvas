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
	[ButtonAction("activate", "servertree-toolbar/ToolbarEdit", "EditServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuEdit", "EditServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipEdit")]
	[IconSet("activate", "Icons.EditToolSmall.png", "Icons.EditToolMedium.png", "Icons.EditToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class EditServerTool : ServerTreeTool
	{
		public EditServerTool()
		{
		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void EditServer()
		{
			ServerTree serverTree = this.Context.ServerTree;
			this.Context.UpdateType = (int)ServerUpdateType.Edit;

			if (serverTree.CurrentNode.IsServer)
			{
				DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServer);
			}
			else
			{
				DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(serverTree, ServerUpdateType.Edit);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleEditServerGroup);
			}

			this.Context.UpdateType = (int)ServerUpdateType.None;
		}

		private void SetDoubleClickHandler()
		{
			this.Context.DefaultActionHandler = EditServer;
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			if (this.Context.IsReadOnly)
			{
				this.Enabled = false;
			}
			else
			{
				//enabled if it is a server or server group and is not the "My Servers" root node
				this.Enabled = (this.Context.ServerTree.CurrentNode.IsServer || this.Context.ServerTree.CurrentNode.IsServerGroup) &&
				               this.Context.ServerTree.CurrentNode != this.Context.ServerTree.RootServerGroup;
			}
		}
	}
}