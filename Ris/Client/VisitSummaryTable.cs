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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class VisitSummaryTable : Table<VisitSummary>
	{
		public VisitSummaryTable()
		{
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnVisitNumber, v => VisitNumberFormat.Format(v.VisitNumber), 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnVisitType, FormatVisitType, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnCurrentLocation, v => v.CurrentLocation != null ? v.CurrentLocation.Name : null, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnRoom, v => v.CurrentRoom, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnBed, v => v.CurrentBed, 1.0f));
			this.Columns.Add(new TableColumn<VisitSummary, string>(
				SR.ColumnStatus, v => v.Status.Value, 1.0f));
			this.Columns.Add(new DateTableColumn<VisitSummary>(
				SR.ColumnAdmitDateTime, v => v.AdmitTime, 1.0f));
			this.Columns.Add(new DateTableColumn<VisitSummary>(
				SR.ColumnDischargeDateTime, v => v.DischargeTime, 1.0f));
		}

		private static string FormatVisitType(VisitSummary v)
		{
			var sb = new StringBuilder();
			sb.Append(v.PatientClass);
			if (v.PatientType != null)
			{
				sb.Append(" - ");
				sb.Append(v.PatientType.Value);
			}
			if (v.AdmissionType != null)
			{
				sb.Append(" - ");
				sb.Append(v.AdmissionType.Value);
			}
			return sb.ToString();
		}
	}
}
