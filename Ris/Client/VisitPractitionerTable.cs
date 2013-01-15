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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class VisitPractitionerTable : Table<VisitPractitionerDetail>
    {
        public VisitPractitionerTable()
        {
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnRole,
                delegate(VisitPractitionerDetail vp)
                {
                    return vp.Role.Value;
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnPractitioner,
                delegate(VisitPractitionerDetail vp)
                {
                    return PersonNameFormat.Format(vp.Practitioner.Name);
                },
                2.5f));
			this.Columns.Add(new DateTimeTableColumn<VisitPractitionerDetail>(
                SR.ColumnStartTime,
                delegate(VisitPractitionerDetail vp) { return vp.StartTime; },
                0.8f));
			this.Columns.Add(new DateTimeTableColumn<VisitPractitionerDetail>(
                SR.ColumnEndTime,
                delegate(VisitPractitionerDetail vp) { return vp.EndTime; },
                0.8f));
        }
    }
}
