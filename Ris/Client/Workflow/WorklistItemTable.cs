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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class WorklistItemTable<TItem> : Table<TItem>
		where TItem: WorklistItemSummaryBase
	{
		private const int NumRows = 2;
		private const int DescriptionRow = 1;

		public WorklistItemTable()
			: base(NumRows)
		{
			// Visible Columns
			var priorityColumn = new TableColumn<TItem, IconSet>(SR.ColumnPriority, item => GetPriorityIcon(item.OrderPriority), 0.2f)
									{
										Comparison = ComparePriorities,
										ResourceResolver = new ResourceResolver(this.GetType().Assembly)
									};

			var mrnColumn = new TableColumn<TItem, string>(SR.ColumnMRN, item => MrnFormat.Format(item.Mrn), 0.9f);
			var nameColumn = new TableColumn<TItem, string>(SR.ColumnName, item => PersonNameFormat.Format(item.PatientName), 1.5f);
			var scheduledForColumn = new DateTimeTableColumn<TItem>(SR.ColumnTime, item => item.Time, 1.1f);
			var descriptionRow = new TableColumn<TItem, string>(SR.ColumnDescription, FormatDescription, 1.0f, DescriptionRow);

			// Invisible but sortable columns
			var patientClassColumn = new TableColumn<TItem, string>(SR.ColumnPatientClass, FormatPatientClass, 1.0f) { Visible = false };

			var accessionNumberColumn = new TableColumn<TItem, string>(SR.ColumnAccessionNumber,
				item => AccessionFormat.Format(item.AccessionNumber), 1.0f) { Visible = false };

			var procedureNameColumn = new TableColumn<TItem, string>(SR.ColumnProcedure, ProcedureFormat.Format, 1.0f) { Visible = false };

			// The order of the addition determines the order of SortBy dropdown
			this.Columns.Add(priorityColumn);
			this.Columns.Add(mrnColumn);
			this.Columns.Add(nameColumn);
			this.Columns.Add(patientClassColumn);
			this.Columns.Add(procedureNameColumn);
			this.Columns.Add(accessionNumberColumn);
			this.Columns.Add(scheduledForColumn);
			this.Columns.Add(descriptionRow);

			// Sort the table by Scheduled Time initially
			this.Sort(new TableSortParams(scheduledForColumn, true));
		}

		private static int ComparePriorities(TItem item1, TItem item2)
		{
			return GetPriorityIndex(item1.OrderPriority) - GetPriorityIndex(item2.OrderPriority);
		}

		private static string FormatPatientClass(TItem item)
		{
			return item.PatientClass == null ? null : item.PatientClass.Value;
		}

		private static string FormatDescription(TItem item)
		{
			// if there is no accession number, this item represents a patient only, not an order
			return item.AccessionNumber == null ? null :
				string.Format("{0} {1}", AccessionFormat.Format(item.AccessionNumber), ProcedureFormat.Format(item));
		}

		private static int GetPriorityIndex(EnumValueInfo orderPriority)
		{
			if (orderPriority == null)
				return 0;

			switch (orderPriority.Code)
			{
				case "S": // Stat
					return 2;
				case "A": // Urgent
					return 1;
				default: // Routine
					return 0;
			}
		}

		private static IconSet GetPriorityIcon(EnumValueInfo orderPriority)
		{
			if (orderPriority == null)
				return null;

			switch (orderPriority.Code)
			{
				case "S": // Stats
					return new IconSet("DoubleExclamation.png");
				case "A": // Urgent
					return new IconSet("SingleExclamation.png");
				default:
					return null;
			}
		}
	}

	public class RegistrationWorklistTable : WorklistItemTable<RegistrationWorklistItemSummary>
	{
	}

	public class PerformingWorklistTable : WorklistItemTable<ModalityWorklistItemSummary>
	{
	}

	public class ReportingWorklistTable : WorklistItemTable<ReportingWorklistItemSummary>
	{
		
	}
}
