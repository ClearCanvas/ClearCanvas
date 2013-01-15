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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;
using System.Text;

namespace ClearCanvas.Ris.Client
{
	public class VisitListTable : Table<VisitListItem>
	{
		public VisitListTable()
			: base(2)
		{
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitNumber, visitListItem => VisitNumberFormat.Format(visitListItem.VisitNumber), 1.0f));

			//Visit type description
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitType, FormatVisitType, 1));

			//status
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitStatus, visitListItem => visitListItem.VisitStatus.Value, 1.0f));

			//admit date/time
			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnAdmitDateTime, visitListItem => visitListItem.AdmitTime, 1.0f));

			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnDischargeDateTime, visitListItem => visitListItem.DischargeTime, 1.0f));
		}

		private static string FormatVisitType(VisitListItem item)
		{
			var sb = new StringBuilder();
			sb.Append(item.PatientClass.Value);
			if(item.PatientType != null)
			{
				sb.Append(" - ");
				sb.Append(item.PatientType.Value);
			}
			if (item.AdmissionType != null)
			{
				sb.Append(" - ");
				sb.Append(item.AdmissionType.Value);
			}

			return sb.ToString();
		}
	}
}
