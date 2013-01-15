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
	[ButtonAction("activate", "servertree-toolbar/ToolbarAddServer", "AddNewServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuAddServer", "AddNewServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAddServer")]
	[IconSet("activate", "Icons.AddServerToolSmall.png", "Icons.AddServerToolMedium.png", "Icons.AddServerToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class AddServerTool : ServerTreeTool
	{
		public AddServerTool()
		{
		}

		private void AddNewServer()
		{
			ServerTree serverTree = this.Context.ServerTree;
			this.Context.UpdateType = (int)ServerUpdateType.Add;
			DicomServerEditComponent editor = new DicomServerEditComponent(serverTree);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, SR.TitleAddNewServer);
			this.Context.UpdateType = (int)ServerUpdateType.None; 
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.Enabled = !this.Context.IsReadOnly && this.Context.ServerTree.CurrentNode.IsServerGroup;
		}
	}
}