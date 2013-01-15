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

namespace ClearCanvas.Ris.Client
{
	public class OrderListTable : Table<OrderListItem>
	{
		public OrderListTable()
			: base(3)
		{
			this.Columns.Add(new DateTableColumn<OrderListItem>(SR.ColumnCreatedOn, order => order.EnteredTime, 0.5f));
			this.Columns.Add(new DateTimeTableColumn<OrderListItem>(SR.ColumnScheduledFor, order => order.OrderScheduledStartTime, 0.75f));
			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnImagingService, order => order.DiagnosticService.Name, 1.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnStatus,
				order => order.OrderStatus.Code == "SC" && order.OrderScheduledStartTime == null
					? SR.MessageUnscheduled
					: order.OrderStatus.Value,
				0.5f));

			this.Columns.Add(new TableColumn<OrderListItem, string>(
				SR.ColumnMoreInfo,
				order => string.Format(SR.FormatMoreInfo,
					AccessionFormat.Format(order.AccessionNumber),
					PersonNameFormat.Format(order.OrderingPractitioner.Name),
					order.OrderingFacility.Name),
				1));

			this.Columns.Add(new TableColumn<OrderListItem, string>(SR.ColumnIndication, order => string.Format(SR.FormatIndication, order.ReasonForStudy), 2));
		}
	}
}
