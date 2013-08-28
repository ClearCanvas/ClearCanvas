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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Verify Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Verify Protocol", "Apply")]
	[IconSet("apply", "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class VerifyProtocolTool : ProtocolWorkflowItemTool
	{
		public VerifyProtocolTool()
			: base("AcceptProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.CompletedProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			try
			{
				ExecuteHelper(item.ProcedureStepRef, null);
			}
			catch (FaultException<SupervisorValidationException>)
			{
				ExecuteHelper(item.ProcedureStepRef, GetSupervisorRef());
			}

			this.Context.InvalidateFolders(typeof(Folders.Reporting.CompletedProtocolFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					AcceptProtocolRequest request = new AcceptProtocolRequest(procedureStepRef, supervisorRef);
					service.AcceptProtocol(request);
				});
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Reject Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reject Protocol", "Apply")]
	[IconSet("apply", "Icons.RejectProtocolSmall.png", "Icons.RejectProtocolMedium.png", "Icons.RejectProtocolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class RejectProtocolTool : ProtocolWorkflowItemTool
	{
		public RejectProtocolTool()
			: base("RejectProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.RejectedProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			ProtocolReasonComponent component = new ProtocolReasonComponent();
			if (this.Context.DesktopWindow.ShowDialogBox(component, "Reason") == DialogBoxAction.Ok)
			{
				try
				{
					ExecuteHelper(item.ProcedureStepRef, component.Reason, component.OtherReason, null);
				}
				catch (FaultException<SupervisorValidationException>)
				{
					ExecuteHelper(item.ProcedureStepRef, component.Reason, component.OtherReason, GetSupervisorRef());
				}

				this.Context.InvalidateFolders(typeof (Folders.Reporting.RejectedProtocolFolder));
			}
			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EnumValueInfo reason, string otherReason, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					RejectProtocolRequest request = new RejectProtocolRequest(
						procedureStepRef, 
						supervisorRef,
						reason,
						CreateAdditionalCommentsNote(otherReason));
					service.RejectProtocol(request);
				});
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			if (!string.IsNullOrEmpty(additionalComments))
				return new OrderNoteDetail(OrderNoteCategory.Protocol.Key, additionalComments, null, false, null, null);
			else
				return null;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Submit for Review", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Submit for Review", "Apply")]
	[IconSet("apply", "Icons.SubmitForReviewSmall.png", "Icons.SubmitForReviewMedium.png", "Icons.SubmitForReviewLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class SubmitForReviewProtocolTool : ProtocolWorkflowItemTool
	{
		public SubmitForReviewProtocolTool()
			: base("SubmitProtocolForApproval")
		{
		}

		public bool Visible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(Application.Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview);
			}
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.AwaitingApprovalProtocolFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			try
			{
				ExecuteHelper(item.ProcedureStepRef, null);
			}
			catch (FaultException<SupervisorValidationException>)
			{
				ExecuteHelper(item.ProcedureStepRef, GetSupervisorRef());
			}

			this.Context.InvalidateFolders(typeof(Folders.Reporting.AwaitingApprovalProtocolFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					SubmitProtocolForApprovalRequest request = new SubmitProtocolForApprovalRequest(procedureStepRef, supervisorRef);
					service.SubmitProtocolForApproval(request);
				});
		}
	}

    [MenuAction("apply", "folderexplorer-items-contextmenu/Discard Protocol", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Discard Protocol", "Apply")]
	[IconSet("apply", "Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class DiscardProtocolTool : ProtocolWorkflowItemTool
	{
		public DiscardProtocolTool()
			: base("DiscardProtocol")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.ToBeProtocolledFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDiscardSelectedProtocol, MessageBoxActions.OkCancel)
				== DialogBoxAction.Cancel)
				return false;

			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					service.DiscardProtocol(new DiscardProtocolRequest(item.ProcedureStepRef));
				});

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/Reassign", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reassign", "Apply")]
	[IconSet("apply", "Icons.AssignSmall.png", "Icons.AssignMedium.png", "Icons.AssignLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Extended.Common.AuthorityTokens.Workflow.Protocol.Reassign)]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class ReassignTool : ReportingWorkflowItemTool
	{
		public ReassignTool()
			: base("ReassignProcedureStep")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			try
			{
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					new ReassignComponent(item),
					SR.TitleReassignItem);

				return exitCode == ApplicationComponentExitCode.Accepted;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return false;
			}
		}
	}

}