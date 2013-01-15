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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class StepType
	{
		public const string TranscriptionReview = "Transcription Review";
		public const string Interpretation = "Interpretation";
		public const string Transcription = "Transcription";
		public const string Verification = "Verification";
		public const string Publication = "Publication";
	}

	public class StepState
	{
		public const string Scheduled = "SC";
		public const string InProgress = "IP";
		public const string Completed = "CM";
	}

	public abstract class ReportingWorkflowItemTool : WorkflowItemTool<ReportingWorklistItemSummary, IReportingWorkflowItemToolContext>
	{
		protected ReportingWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IReportingWorkflowService));
		}

		protected bool ActivateIfAlreadyOpen(ReportingWorklistItemSummary item)
		{
			var document = DocumentManager.Get<ReportDocument>(item.ProcedureStepRef);
			if (document != null)
			{
				document.Open();
				return true;
			}
			return false;
		}

		protected void OpenReportEditor(ReportingWorklistItemSummary item)
		{
			OpenReportEditor(item, true);
		}

		protected void OpenReportEditor(ReportingWorklistItemSummary item, bool shouldOpenImages)
		{
			if (ActivateIfAlreadyOpen(item))
				return;

			if (!ReportingSettings.Default.AllowMultipleReportingWorkspaces)
			{
				var documents = DocumentManager.GetAll<ReportDocument>();

				// Show warning message and ask if the existing document should be closed or not
				if (documents.Count > 0)
				{
					if (UserElectsToLeaveExistingDocumentOpen(item, documents))
						return;

					// close documents and continue
					CollectionUtils.ForEach(documents, document => document.SaveAndClose());
				}
			}

			// open the report editor
			var doc = new ReportDocument(item, shouldOpenImages, this.Context);
			doc.Open();

			// Need to re-invalidate folders that open a report document, since cancelling the report
			// can re-insert items into the same folder.
			var selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
			doc.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };

			if (doc.UserCancelled)
				doc.Close();
		}

		private bool UserElectsToLeaveExistingDocumentOpen(ReportingWorklistItemSummary item, IEnumerable<ReportDocument> documents)
		{
			var firstDocument = CollectionUtils.FirstElement(documents);
			firstDocument.Open();

			var message = string.Format(SR.MessageReportingComponentAlreadyOpened,
				ReportDocument.StripTitle(firstDocument.GetTitle()),
				ReportDocument.StripTitle(ReportDocument.GetTitle(item)));
			return DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);
		}

		protected ReportingWorklistItemSummary GetSelectedItem()
		{
			return this.Context.SelectedItems.Count != 1
				? null
				: CollectionUtils.FirstElement(this.Context.SelectedItems);
		}

		protected EntityRef GetSupervisorRef()
		{
			var supervisorSelectionComponent = new ReportingSupervisorSelectionComponent();
			var supervisorSelected = ApplicationComponentExitCode.Accepted
				== ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, supervisorSelectionComponent, SR.TitleSelectSupervisor);

			if (!supervisorSelected)
				return null;

			return supervisorSelectionComponent.Staff != null ? supervisorSelectionComponent.Staff.StaffRef : null;
		}
	}
}
