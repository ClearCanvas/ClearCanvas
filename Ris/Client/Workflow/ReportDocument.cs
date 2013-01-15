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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportDocument : Document
	{
		private readonly ReportingWorklistItemSummary _worklistItem;
		private readonly bool _shouldOpenImages;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private ReportingComponent _component;

		public ReportDocument(ReportingWorklistItemSummary worklistItem, bool shouldOpenImages, IReportingWorkflowItemToolContext context)
			: base(worklistItem.ProcedureStepRef, context.DesktopWindow)
		{
			_worklistItem = worklistItem;
			_folderName = context.SelectedFolder.Name;
			_shouldOpenImages = shouldOpenImages;

			if (context.SelectedFolder is ReportingWorkflowFolder)
			{
				_worklistRef = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistRef;
				_worklistClassName = ((ReportingWorkflowFolder)context.SelectedFolder).WorklistClassName;
			}
			else
			{
				_worklistRef = null;
				_worklistClassName = null;
			}
		}

		public override string GetTitle()
		{
			return GetTitle(_worklistItem);
		}

		public override bool SaveAndClose()
		{
			_component.SaveReport(true);
			return Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new ReportingComponent(_worklistItem, _folderName, _worklistRef, _worklistClassName, _shouldOpenImages);
			return _component;
		}

		public override OpenWorkspaceOperationAuditData GetAuditData()
		{
			return new OpenWorkspaceOperationAuditData("Reporting", _worklistItem);
		}

		/// <summary>
		/// Indicates if a user interaction cancelled the opening of the <see cref="ReportingComponent"/>
		/// </summary>
		/// <remarks>
		/// Should only be called after <see mref="Open()"/>
		/// </remarks>
		public bool UserCancelled
		{
			get
			{
				Platform.CheckForNullReference(_component, "_component");
				return _component.UserCancelled;
			}
		}

		public static string GetTitle(ReportingWorklistItemSummary item)
		{
			return string.Format("Report - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Report - ", "");
		}
	}
}
