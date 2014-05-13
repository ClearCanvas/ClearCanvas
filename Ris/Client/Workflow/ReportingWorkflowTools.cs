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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuSendToTranscription", "Apply")]
	[IconSet("apply", "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class CompleteInterpretationForTranscriptionTool : ReportingWorkflowItemTool
	{
		private readonly WorkflowConfigurationReader _workflowConfiguration = new WorkflowConfigurationReader();

		public CompleteInterpretationForTranscriptionTool()
			: base("CompleteInterpretationForTranscription")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.InTranscriptionFolder), this);

			base.Initialize();
		}

		public bool Visible
		{
			get { return _workflowConfiguration.EnableTranscriptionWorkflow; }
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

			this.Context.InvalidateFolders(typeof(Folders.Reporting.InTranscriptionFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					CompleteInterpretationForTranscriptionRequest request = new CompleteInterpretationForTranscriptionRequest(procedureStepRef);
					request.SupervisorRef = supervisorRef;
					service.CompleteInterpretationForTranscription(request);
				});
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuSubmitForReview", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible")]
	[IconSet("apply", "Icons.SubmitForReviewSmall.png", "Icons.SubmitForReviewMedium.png", "Icons.SubmitForReviewLarge.png")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Report.Create, Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class CompleteInterpretationForVerificationTool : ReportingWorkflowItemTool
	{
		public CompleteInterpretationForVerificationTool()
			: base("CompleteInterpretationForVerification")
		{
		}

		public bool Visible
		{
			get { return new WorkflowConfigurationReader().EnableInterpretationReviewWorkflow; }
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.AwaitingReviewFolder), this);

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

			this.Context.InvalidateFolders(typeof(Folders.Reporting.AwaitingReviewFolder));

			return true;
		}

		private void ExecuteHelper(EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					CompleteInterpretationForVerificationRequest request = new CompleteInterpretationForVerificationRequest(procedureStepRef);
					request.SupervisorRef = supervisorRef;
					service.CompleteInterpretationForVerification(request);
				});
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuDiscardReport", "Apply")]
	[MenuAction("apply", "folderexplorer-items-toolbar/MenuDiscardReport", "Apply")]
	[IconSet("apply", "Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[IconSetObserver("apply", "CurrentIconSet", "LabelChanged")]
	[LabelValueObserver("apply", "Label", "LabelChanged")]
	[TooltipValueObserver("apply", "Label", "LabelChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class DiscardReportTool : ReportingWorkflowItemTool
	{
		private IconSet _cancelAddendum = new IconSet("Icons.CancelAddendumSmall.png", "Icons.CancelAddendumSmall.png", "Icons.CancelAddendumSmall.png");
		private IconSet _cancelReport = new IconSet("Icons.CancelReportSmall.png", "Icons.CancelReportMedium.png", "Icons.CancelReportLarge.png");

		public DiscardReportTool()
			: base("CancelReportingStep")
		{
		}

		public string Label
		{
			get
			{
				ReportingWorklistItemSummary item = GetSelectedItem();
				return (item != null && item.IsAddendumStep) ? SR.TooltipDiscardAddendum : SR.TooltipDiscardReport;
			}
		}

		public IconSet CurrentIconSet
		{
			get
			{
				ReportingWorklistItemSummary item = GetSelectedItem();
				return (item != null && item.IsAddendumStep) ? _cancelAddendum : _cancelReport;
			}
		}

		public event EventHandler LabelChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			string msg = item.IsAddendumStep ? SR.MessageConfirmDiscardSelectedAddendum : SR.MessageConfirmDiscardSelectedReport;

			if (this.Context.DesktopWindow.ShowMessageBox(msg, MessageBoxActions.OkCancel)
				== DialogBoxAction.Cancel)
				return false;


			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					service.CancelReportingStep(new CancelReportingStepRequest(item.ProcedureStepRef, null));
				});

			// no point in invalidating "to be reported" folder because its communal

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuVerify", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuVerify", "Apply")]
	[IconSet("apply", "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create,
	   ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class VerifyTool : ReportingWorkflowItemTool
	{
		public VerifyTool()
			: base("Verify")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Reporting.VerifiedFolder), this);

			base.Initialize();
		}

		public override bool Enabled
		{
			get
			{
				return this.Context.SelectedItems.Count == 1 &&
					(this.Context.GetOperationEnablement("CompleteInterpretationAndVerify") ||
					this.Context.GetOperationEnablement("CompleteVerification"));
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			return this.Context.GetOperationEnablement("CompleteInterpretationAndVerify") ||
				this.Context.GetOperationEnablement("CompleteVerification");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// show PD dialog if required
			//return PreliminaryDiagnosis.ShowDialogOnVerifyIfRequired(item, this.Context.DesktopWindow,
			//	delegate
			//	{
					//try
					//{
						try
						{
							ExecuteHelper(item.ProcedureStepName, item.ProcedureStepRef, null);
						}
						catch (FaultException<SupervisorValidationException>)
						{
							ExecuteHelper(item.ProcedureStepName, item.ProcedureStepRef, GetSupervisorRef());
						}
					//}
					//catch (Exception e)
					//{
					//    ExceptionHandler.Report(e, this.Context.DesktopWindow);
					//}

					this.Context.InvalidateFolders(typeof(Folders.Reporting.VerifiedFolder));
			//	});
			return true;
		}

		private void ExecuteHelper(string procedureStepName, EntityRef procedureStepRef, EntityRef supervisorRef)
		{
			if (procedureStepName == StepType.Interpretation || procedureStepName == StepType.TranscriptionReview)
			{
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						CompleteInterpretationAndVerifyRequest request = new CompleteInterpretationAndVerifyRequest(procedureStepRef);
						request.SupervisorRef = supervisorRef;
						service.CompleteInterpretationAndVerify(request);
					});
			}
			else if (procedureStepName == StepType.Verification)
			{
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						CompleteVerificationRequest request = new CompleteVerificationRequest(procedureStepRef);
						request.SupervisorRef = supervisorRef;
						service.CompleteVerification(request);
					});
			}
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuAddAddendum", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuAddAddendum", "Apply")]
	[IconSet("apply", "Icons.AddAddendumToolSmall.png", "Icons.AddAddendumToolMedium.png", "Icons.AddAddendumToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class AddendumTool : ReportingWorkflowItemTool
	{
		public AddendumTool()
			: base("CreateAddendum")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			if (ActivateIfAlreadyOpen(item))
				return true;

			ReportingWorklistItemSummary interpretationWorklistItem = null;

			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					CreateAddendumResponse response = service.CreateAddendum(new CreateAddendumRequest(item.ProcedureRef));
					interpretationWorklistItem = response.ReportingWorklistItem;
				});

			this.Context.InvalidateFolders(typeof(Folders.Reporting.DraftFolder));

			if (ActivateIfAlreadyOpen(interpretationWorklistItem))
				return true;

			OpenReportEditor(interpretationWorklistItem);

			return true;
		}
	}

#if DEBUG	// only available in debug builds (only used for testing)
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuPublish", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Development.TestPublishReport)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class PublishReportTool : ReportingWorkflowItemTool
	{
		public PublishReportTool()
			: base("PublishReport")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					service.PublishReport(new PublishReportRequest(item.ProcedureStepRef));
				});

			return true;
		}
	}
#endif
}

