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
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class NoteCategoryTable : Table<PatientNoteCategorySummary>
    {
        public NoteCategoryTable()
        {
            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnSeverity,
                delegate(PatientNoteCategorySummary category) { return category.Severity.Value; },
                0.2f));

            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnCategory,
                delegate(PatientNoteCategorySummary category) { return category.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnDescription,
                delegate(PatientNoteCategorySummary category) { return category.Description; },
                1.0f));
        }
    }
}
