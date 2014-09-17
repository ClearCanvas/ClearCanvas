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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuReviseReport", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuReviseReport", "Apply")]
	[IconSet("apply","Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReviseReportTool : ReportingWorkflowItemTool
	{

		public ReviseReportTool()
			: base("ReviseReport")
		{
		}

		public override bool Enabled
		{
			get
			{
				return this.Context.GetOperationEnablement("ReviseResidentReport")
					|| this.Context.GetOperationEnablement("ReviseUnpublishedReport");
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			return this.Context.GetOperationEnablement("ReviseResidentReport")
				|| this.Context.GetOperationEnablement("ReviseUnpublishedReport");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			ReportingWorklistItemSummary replacementItem = null;

			if (this.Context.GetOperationEnablement("ReviseResidentReport"))
			{
				Platform.GetService(
					delegate(IReportingWorkflowService service)
					{
						var response = service.ReviseResidentReport(new ReviseResidentReportRequest(item.ProcedureStepRef));
						replacementItem = response.ReplacementInterpretationStep;
					});
			}
			else if (this.Context.GetOperationEnablement("ReviseUnpublishedReport"))
			{
				Platform.GetService(
					delegate(IReportingWorkflowService service)
					{
						var response = service.ReviseUnpublishedReport(new ReviseUnpublishedReportRequest(item.ProcedureStepRef));
						replacementItem = response.ReplacementVerificationStep;
					});
			}

			OpenReportEditor(replacementItem);

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuReturnToInterpreter", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuReturnToInterpreter", "Apply")]
	[IconSet("apply", "Icons.AssignSmall.png", "Icons.AssignMedium.png", "Icons.AssignLarge.png")]
	[VisibleStateObserver("apply", "Visible")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class ReturnToInterpreterTool : ReportingWorkflowItemTool
	{

		public ReturnToInterpreterTool()
			: base("ReturnToInterpreter")
		{
		}

		public bool Visible
		{
			get { return new WorkflowConfigurationReader().EnableInterpretationReviewWorkflow; }
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService((IReportingWorkflowService service) =>
				service.ReturnToInterpreter(new ReturnToInterpreterRequest(item.ProcedureStepRef)));

			return true;
		}
	}
}
