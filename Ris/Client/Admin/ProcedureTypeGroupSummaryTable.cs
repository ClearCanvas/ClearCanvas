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

namespace ClearCanvas.Ris.Client.Admin
{
    public class ProcedureTypeGroupSummaryTable : Table<ProcedureTypeGroupSummary>
    {
        public ProcedureTypeGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnName,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnCategory,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Category.Value; },
                0.5f));

            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnDescription,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Description; },
                1.5f));
        }
    }
}