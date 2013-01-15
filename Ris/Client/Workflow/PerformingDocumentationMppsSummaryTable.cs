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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class PerformingDocumentationMppsSummaryTable : Table<ModalityPerformedProcedureStepDetail>
	{
		public PerformingDocumentationMppsSummaryTable()
		{
			this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
								 SR.ColumnName,
								 FormatDescription,
								 5.0f));

			this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
								 SR.ColumnState,
								 FormatStatus,
								 1.2f));

			var sortColumn = 
				new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
								 SR.ColumnStartTime,
								 mpps => mpps.StartTime,
								 1.5f);

			this.Columns.Add(sortColumn);
			this.Sort(new TableSortParams(sortColumn, true));

			var endTimeColumn = new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
								 SR.ColumnEndTime,
								 mpps => mpps.EndTime,
								 1.5f);

			this.Columns.Add(endTimeColumn);
		}

		private static string FormatStatus(ModalityPerformedProcedureStepDetail mpps)
		{
			return mpps.State.Code == "CM" ? "Performed" : mpps.State.Value;
		}

		private static string FormatDescription(ModalityPerformedProcedureStepDetail mpps)
		{
			var description = StringUtilities.Combine(mpps.ModalityProcedureSteps, " / ",
				delegate(ModalityProcedureStepSummary mps)
				{
					var modifier = ProcedureFormat.FormatModifier(mps.Procedure.Portable, mps.Procedure.Laterality);
					return string.IsNullOrEmpty(modifier) 
						? mps.Description 
						: string.Format("{0} ({1})", mps.Description, modifier);
				});

			return description;
		}
	}
}