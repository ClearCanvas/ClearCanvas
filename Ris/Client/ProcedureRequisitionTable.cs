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

using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	class ProcedureRequisitionTable : Table<ProcedureRequisition>
	{
		private readonly TableColumn<ProcedureRequisition, string> _procedureColumn;
		private readonly TableColumn<ProcedureRequisition, string> _facilityColumn;
		private readonly TableColumn<ProcedureRequisition, string> _scheduledTimeColumn;
		private readonly TableColumn<ProcedureRequisition, string> _scheduledDurationColumn;
		private readonly TableColumn<ProcedureRequisition, string> _modalityColumn;

		public ProcedureRequisitionTable()
		{
			this.Columns.Add(_procedureColumn = new TableColumn<ProcedureRequisition, string>(SR.ColumnProcedure, ProcedureFormat.Format));
			this.Columns.Add(_facilityColumn = new TableColumn<ProcedureRequisition, string>(SR.ColumnFacility, FormatPerformingFacility));
			this.Columns.Add(_scheduledTimeColumn = new TableColumn<ProcedureRequisition, string>(SR.ColumnScheduledTime, FormatScheduledTime));
			this.Columns.Add(_scheduledDurationColumn = new TableColumn<ProcedureRequisition, string>(SR.ColumnScheduledDuration, FormatScheduledDuration));
			this.Columns.Add(_modalityColumn = new TableColumn<ProcedureRequisition, string>(SR.ColumnModality, FormatScheduledModality));
		}

		public TableColumn<ProcedureRequisition, string> ScheduledDurationColumn
		{
			get { return _scheduledDurationColumn; }
		}

		public TableColumn<ProcedureRequisition, string> ModalityColumn
		{
			get { return _modalityColumn; }
		}

		private string FormatPerformingFacility(ProcedureRequisition requisition)
		{
			var sb = new StringBuilder();
			if (requisition.PerformingFacility != null)
			{
				sb.Append(requisition.PerformingFacility.Name);
			}
			if (requisition.PerformingDepartment != null)
			{
				sb.Append(" (" + requisition.PerformingDepartment.Name + ")");
			}
			return sb.ToString();
		}

		private string FormatScheduledModality(ProcedureRequisition procedureRequisition)
		{
			return procedureRequisition.Modality != null ? procedureRequisition.Modality.Name : null;
		}

		private static string FormatScheduledTime(ProcedureRequisition item)
		{
			// if new or scheduled
			if (item.Status != null && item.Status.Code != "SC")
				return item.Status.Value;

			if (item.Cancelled)
				return "Cancel Pending";

			return Format.DateTime(item.ScheduledTime);
		}

		private string FormatScheduledDuration(ProcedureRequisition procedureRequisition)
		{
			return string.Format("{0} min", procedureRequisition.ScheduledDuration);
		}

	}
}
