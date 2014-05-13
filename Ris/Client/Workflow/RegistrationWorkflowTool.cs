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
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class RegistrationWorkflowTool : WorkflowItemTool<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected RegistrationWorkflowTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IRegistrationWorkflowService));
		}
	}


	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuCheckIn", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuCheckIn", "Apply")]
	[IconSet("apply", "Icons.CheckInToolSmall.png", "Icons.CheckInToolMedium.png", "Icons.CheckInToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Procedure.CheckIn)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class CheckInProceduresTool : RegistrationWorkflowTool
	{
		public CheckInProceduresTool()
			: base("CheckInProcedure")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CheckedInFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			if (OrderCheckInHelper.CheckIn(item.OrderRef, PersonNameFormat.Format(item.PatientName), this.Context.DesktopWindow))
			{
				this.Context.InvalidateFolders(typeof(Folders.Registration.CheckedInFolder));
				return true;
			}
			return false;
		}
	}
}

