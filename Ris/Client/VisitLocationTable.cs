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
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class VisitLocationTable : Table<VisitLocationDetail>
	{
		public VisitLocationTable()
		{
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnRole, vl => vl.Role.Value, 0.8f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnLocation, FormatVisitLocation, 2.5f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnRoom, vl => vl.Room, 0.2f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnBed, vl => vl.Bed, 0.2f));
			this.Columns.Add(new DateTimeTableColumn<VisitLocationDetail>(
				SR.ColumnStartTime, vl => vl.StartTime, 0.8f));
			this.Columns.Add(new DateTimeTableColumn<VisitLocationDetail>(
				SR.ColumnEndTime, vl => vl.EndTime, 0.8f));
		}

		private static string FormatVisitLocation(VisitLocationDetail vl)
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}", vl.Bed, vl.Room, vl.Location.Floor, vl.Location.Building, vl.Location.Facility.Name);
		}
	}
}
