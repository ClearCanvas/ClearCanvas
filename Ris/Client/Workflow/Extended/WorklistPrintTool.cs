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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print Worklist", "Print")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print Worklist", "Print")]
	[Tooltip("apply", "Print Worklist")]
	[IconSet("apply", "PrintSmall.png", "PrintMedium.png", "PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ActionPermission("apply", Application.Extended.Common.AuthorityTokens.Workflow.Worklist.Print)]
	public class WorklistPrintTool : Tool<IWorkflowItemToolContext>
	{
		public bool Enabled
		{
			get { return this.Context.SelectedFolder != null && this.Context.SelectedFolder.ItemsTable.Items.Count > 0; }
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public void Print()
		{
			var selectedFolder = this.Context.SelectedFolder;
			if(selectedFolder == null)
				return;

			var fsName = selectedFolder.FolderSystem != null ? selectedFolder.FolderSystem.Title : "";
			var folderName = selectedFolder.Name;
			var folderDescription = selectedFolder.Tooltip;
			var totalItemCount = selectedFolder.TotalItemCount;
			var items = new List<object>();
			foreach (var item in selectedFolder.ItemsTable.Items)
				items.Add(item);

			ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				new WorklistPrintComponent(fsName, folderName, folderDescription, totalItemCount, items),
				SR.TitlePrintWorklist);
		}
	}
}
