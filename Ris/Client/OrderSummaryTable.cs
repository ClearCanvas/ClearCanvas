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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class OrderSummaryTable : Table<OrderSummary>
    {
        public OrderSummaryTable()
        {
            this.Columns.Add(new DateTimeTableColumn<OrderSummary>("Scheduled Requested For",
                delegate(OrderSummary order) { return order.SchedulingRequestTime; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnAccessionNumber,
                delegate(OrderSummary order) { return AccessionFormat.Format(order.AccessionNumber); }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnImagingService,
                delegate(OrderSummary order) { return order.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnPriority,
                delegate(OrderSummary order) { return order.OrderPriority.Value; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>(SR.ColumnStatus,
                delegate(OrderSummary order) { return order.OrderStatus.Value; }));

            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered by",
                delegate(OrderSummary order) { return PersonNameFormat.Format(order.OrderingPractitioner.Name); }));

            this.Columns.Add(new TableColumn<OrderSummary, string>("Ordered From",
                delegate(OrderSummary order) { return order.OrderingFacility; }));
            this.Columns.Add(new TableColumn<OrderSummary, string>("Reason for Study",
                delegate(OrderSummary order) { return order.ReasonForStudy; }));
			this.Columns.Add(new DateTableColumn<OrderSummary>(SR.ColumnCreatedOn,
                delegate(OrderSummary order) { return order.EnteredTime; }));

        }
    }
}
