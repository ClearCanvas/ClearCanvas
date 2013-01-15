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

using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionDocument : Document
	{
		private readonly ReportingWorklistItemSummary _worklistItem;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private TranscriptionComponent _component;

		public TranscriptionDocument(ReportingWorklistItemSummary worklistItem, IReportingWorkflowItemToolContext context)
			: base(worklistItem.ProcedureStepRef, context.DesktopWindow)
		{
			_worklistItem = worklistItem;
			_folderName = context.SelectedFolder.Name;

			if(context.SelectedFolder is TranscriptionWorkflowFolder)
			{
				_worklistRef = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((TranscriptionWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		public override string GetTitle()
		{
			return TranscriptionDocument.GetTitle(_worklistItem);
		}

		public override bool SaveAndClose()
		{
			_component.SaveReport(true);
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new TranscriptionComponent(_worklistItem, _folderName, _worklistRef, _worklistClassName);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Transcription", _worklistItem);
		}

		public static string GetTitle(ReportingWorklistItemSummary item)
		{
			return string.Format("Transcription - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Transcription - ", "");
		}
	}
}