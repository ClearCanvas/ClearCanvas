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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuComplete", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuComplete", "Apply")]
	[IconSet("apply", "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class CompleteTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public CompleteTranscriptionTool()
			: base("CompleteTranscription")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Transcription.CompletedFolder), this);

			base.Initialize();
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					service.CompleteTranscription(new CompleteTranscriptionRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Transcription.CompletedFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuReject", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuReject", "Apply")]
	[IconSet("apply", "Icons.RejectTranscriptionSmall.png", "Icons.RejectTranscriptionMedium.png", "Icons.RejectTranscriptionLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class RejectTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public RejectTranscriptionTool()
			: base("RejectTranscription")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			TranscriptionRejectReasonComponent component = new TranscriptionRejectReasonComponent();
			if (this.Context.DesktopWindow.ShowDialogBox(component, SR.TitleReason) == DialogBoxAction.Ok)
			{
				Platform.GetService<ITranscriptionWorkflowService>(
					delegate(ITranscriptionWorkflowService service)
					{
						service.RejectTranscription(new RejectTranscriptionRequest(
							item.ProcedureStepRef, 
							component.Reason,
							CreateAdditionalCommentsNote(component.OtherReason)));
					});

				this.Context.InvalidateFolders(typeof (Folders.Transcription.CompletedFolder));
			}
			return true;
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			if (!string.IsNullOrEmpty(additionalComments))
				return new OrderNoteDetail(OrderNoteCategory.General.Key, additionalComments, null, false, null, null);
			else
				return null;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuSubmitForReview", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuSubmitForReview", "Apply")]
	[IconSet("apply", "Icons.VerifyReportSmall.png", "Icons.VerifyReportMedium.png", "Icons.VerifyReportLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class SubmitTranscriptionForReviewTool : TranscriptionWorkflowItemTool
	{
		private WorkflowConfigurationReader _workflowConfiguration = new WorkflowConfigurationReader();

		public SubmitTranscriptionForReviewTool()
			: base("SubmitTranscriptionForReview")
		{
		}

		public override void Initialize()
		{
			this.Context.RegisterDropHandler(typeof(Folders.Transcription.AwaitingReviewFolder), this);

			base.Initialize();
		}

		public bool Visible
		{
			get { return _workflowConfiguration.EnableTranscriptionReviewWorkflow; }
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			Platform.GetService<ITranscriptionWorkflowService>(
				delegate(ITranscriptionWorkflowService service)
				{
					service.SubmitTranscriptionForReview(new SubmitTranscriptionForReviewRequest(item.ProcedureStepRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Transcription.AwaitingReviewFolder));

			return true;
		}
	}

	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuOpenReport", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuOpenReport", "Apply")]
	[IconSet("apply", "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.Create)]
	[ExtensionOf(typeof(TranscriptionWorkflowItemToolExtensionPoint))]
	public class EditTranscriptionTool : TranscriptionWorkflowItemTool
	{
		public EditTranscriptionTool()
			: base("EditTranscription")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Transcription.DraftFolder), this);
			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		public override bool Enabled
		{
			get
			{
				ReportingWorklistItemSummary item = GetSelectedItem();

				if (this.Context.SelectedItems.Count != 1)
					return false;

				return
					this.Context.GetOperationEnablement("StartTranscription") ||

					// there is no specific workflow operation for editing a previously created draft,
					// so we enable the tool if it looks like a draft and SaveReport is enabled
					(this.Context.GetOperationEnablement("SaveTranscription") && item != null && item.ActivityStatus.Code == StepState.InProgress);
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItemSummary> items)
		{
			// this tool is only registered as a drop handler for the Drafts folder
			// and the only operation that would make sense in this context is StartInterpretation
			return this.Context.GetOperationEnablement("StartTranscription");
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			// open the report editor
			OpenTranscriptionEditor(item);

			return true;
		}
	}
}
