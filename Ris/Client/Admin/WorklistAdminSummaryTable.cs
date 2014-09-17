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
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Admin
{
    public class WorklistAdminSummaryTable : Table<WorklistAdminSummary>
    {
        public WorklistAdminSummaryTable()
        {
            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>(SR.ColumnName,
                delegate(WorklistAdminSummary summary) { return summary.DisplayName; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>(SR.ColumnClass,
                delegate(WorklistAdminSummary summary)
                {
                     return string.Format("{0} - {1}", summary.WorklistClass.CategoryName, summary.WorklistClass.DisplayName);
                },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>(SR.ColumnDescription,
                delegate(WorklistAdminSummary summary) { return summary.Description; },
                1.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>(SR.ColumnOwner,
                delegate(WorklistAdminSummary summary)
                {
                    if (summary.OwnerStaff != null)
                        return PersonNameFormat.Format(summary.OwnerStaff.Name);
                    else if (summary.OwnerGroup != null)
                        return summary.OwnerGroup.Name;
                    else
                        return "";
                },
                1.0f));
        }
    }
}