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
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Document container for <see cref="ProtocolEditorComponent"/>
	/// </summary>
	class ProtocolDocument : Document
	{
		#region Private Members

		private readonly ReportingWorklistItemSummary _item;
		private readonly IContinuousWorkflowComponentMode _mode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private ProtocollingComponent _component;

		#endregion

		#region Constructor

		public ProtocolDocument(ReportingWorklistItemSummary item, IContinuousWorkflowComponentMode mode, IReportingWorkflowItemToolContext context)
			: base(item.OrderRef, context.DesktopWindow)
		{
			_item = item;
			_mode = mode;
			_folderName = context.SelectedFolder.Name;

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

		#endregion

		#region Document overrides

		public override string GetTitle()
		{
			return ProtocolDocument.GetTitle(_item);
		}

		public override bool SaveAndClose()
		{
			_component.Save(true);
			return base.Close();
		}

		public override IApplicationComponent GetComponent()
		{
			_component = new ProtocollingComponent(_item, _mode, _folderName, _worklistRef, _worklistClassName);
			return _component;
		}

		#endregion

		public static string GetTitle(ReportingWorklistItemSummary item)
		{
			return string.Format("Protocol - {0} - {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn));
		}

		public static string StripTitle(string title)
		{
			return title.Replace("Protocol - ", "");
		}
	}
}
