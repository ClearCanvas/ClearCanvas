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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuPrintReport", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuPrintReport", "Apply")]
	[IconSet("apply", "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class PrintReportTool : ReportingWorkflowItemTool
	{
		public PrintReportTool()
			: base("PrintReport")
		{
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1)
					return false;

				var item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				return item.ReportRef != null;
			}
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			if (item.ReportRef == null)
				return false;

			try
			{
				var title = string.Format(SR.FormatPrintReportMessage,
										  Formatting.MrnFormat.Format(item.Mrn),
										  Formatting.PersonNameFormat.Format(item.PatientName),
										  Formatting.AccessionFormat.Format(item.AccessionNumber));

				var component = new PrintReportComponent(item.OrderRef, item.ReportRef);
				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					title);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}

			// always return false - we didn't change any data
			return false;
		}
	}
}
