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
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    public class ProtocolEditorProcedurePlanSummaryTable : Table<ProtocolEditorProcedurePlanSummaryTableItem>
    {
        public ProtocolEditorProcedurePlanSummaryTable()
        {
            ITableColumn sortColumn = new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                "Procedure Description",
                delegate(ProtocolEditorProcedurePlanSummaryTableItem item) { return ProcedureFormat.Format(item.ProcedureDetail); },
                0.5f);

            this.Columns.Add(sortColumn);

            this.Columns.Add(new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                                 "Protocol Status",
                                 delegate(ProtocolEditorProcedurePlanSummaryTableItem item)
                                 {
                                     return item.ProtocolDetail.Status.Value;
                                 },
                                 0.5f));

            this.Sort(new TableSortParams(sortColumn, true));
        }
    }
}