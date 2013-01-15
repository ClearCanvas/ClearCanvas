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
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Revise Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Revise Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class ReviseProtocolTool : ProtocolWorkflowItemTool
	{
		public ReviseProtocolTool()
			: base("ReviseProtocol")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Reporting.DraftProtocolFolder), this);
		}

		public override bool Enabled
		{
			get { return this.Context.GetOperationEnablement("ReviseSubmittedProtocol"); }
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			ReportingWorklistItem replacementStep = null;
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					ReviseSubmittedProtocolResponse response = service.ReviseSubmittedProtocol(new ReviseSubmittedProtocolRequest(item.OrderRef));
					replacementStep = response.ReplacementStep;
				});

			if (replacementStep == null)
				return false;

			// open the report editor
			ProtocollingComponentDocument protocollingComponentDocument = new ProtocollingComponentDocument(
				replacementStep,
				ProtocollingComponentMode.Edit,
				this.Context);
			protocollingComponentDocument.Open();

			Type selectedFolderType = this.Context.SelectedFolder.GetType();
			protocollingComponentDocument.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };

			return true;
		}

		private static bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
		{
			Workspace workspace = DocumentManager.Get<ProtocollingComponentDocument>(item.OrderRef);
			if (workspace != null)
			{
				workspace.Activate();
				return true;
			}
			return false;
		}
	}
}