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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class ProtocolWorkflowItemTool : WorkflowItemTool<ReportingWorklistItemSummary, IReportingWorkflowItemToolContext>
	{
		protected ProtocolWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected ReportingWorklistItemSummary GetSelectedItem()
		{
			if (this.Context.SelectedItems.Count != 1)
				return null;
			return CollectionUtils.FirstElement(this.Context.SelectedItems);
		}

		protected EntityRef GetSupervisorRef()
		{
			ProtocollingSupervisorSelectionComponent component = new ProtocollingSupervisorSelectionComponent();
			if (ApplicationComponentExitCode.Accepted == ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleSelectSupervisor))
			{
				return component.Staff != null ? component.Staff.StaffRef : null;
			}
			else
				return null;
		}
	}
}
