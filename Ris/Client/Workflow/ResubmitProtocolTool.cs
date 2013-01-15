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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Re-submit for Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Re-submit for Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.ResubmitOrderSmall.png", "Icons.ResubmitOrderMedium.png", "Icons.ResubmitOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Resubmit)]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class ResubmitProtocolTool : WorkflowItemTool<RegistrationWorklistItemSummary, IWorkflowItemToolContext<RegistrationWorklistItemSummary>>
	{
		public ResubmitProtocolTool()
			: base("ResubmitProtocol")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Registration.RejectedProtocolFolder));
			this.Context.InvalidateFolders(typeof(Folders.Registration.PendingProtocolFolder));

			return true;
		}
	}
}
