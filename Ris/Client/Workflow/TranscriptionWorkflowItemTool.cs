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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class TranscriptionWorkflowItemTool : WorkflowItemTool<ReportingWorklistItemSummary, IReportingWorkflowItemToolContext>
	{
		protected TranscriptionWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(ITranscriptionWorkflowService));
		}

		protected bool ActivateIfAlreadyOpen(ReportingWorklistItemSummary item)
		{
			var document = DocumentManager.Get<TranscriptionDocument>(item.ProcedureStepRef);
			if (document != null)
			{
				document.Open();
				return true;
			}
			return false;
		}

		protected void OpenTranscriptionEditor(ReportingWorklistItemSummary item)
		{
			if (ActivateIfAlreadyOpen(item))
				return;

			if (!TranscriptionSettings.Default.AllowMultipleTranscriptionWorkspaces)
			{
				var documents = DocumentManager.GetAll<TranscriptionDocument>();

				// Show warning message and ask if the existing document should be closed or not
				if (documents.Count > 0)
				{
					var firstDocument = CollectionUtils.FirstElement(documents);
					firstDocument.Open();

					var message = string.Format(SR.MessageTranscriptionComponentAlreadyOpened,
						TranscriptionDocument.StripTitle(firstDocument.GetTitle()), 
						TranscriptionDocument.StripTitle(TranscriptionDocument.GetTitle(item)));
					if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
						return;		// Leave the existing document open

					// close documents and continue
					CollectionUtils.ForEach(documents, document => document.SaveAndClose());
				}
			}

			// open the report editor
			var doc = new TranscriptionDocument(item, this.Context);
			doc.Open();

			// Need to re-invalidate folders that open a report document, since cancelling the report
			// can re-insert items into the same folder.
			var selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
			doc.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
		}

		protected ReportingWorklistItemSummary GetSelectedItem()
		{
			return this.Context.SelectedItems.Count != 1 
				? null 
				: CollectionUtils.FirstElement(this.Context.SelectedItems);
		}
	}
}
