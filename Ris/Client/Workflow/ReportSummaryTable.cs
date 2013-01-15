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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportSummaryTable : Table<ReportSummary>
    {
        public ReportSummaryTable()
        {
            this.Columns.Add(new TableColumn<ReportSummary, string>("Accession Number",
                delegate(ReportSummary report) { return report.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Visit Number",
                delegate(ReportSummary report) { return VisitNumberFormat.Format(report.VisitNumber); }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Requested Procedure",
                delegate(ReportSummary report)
                {
                    return StringUtilities.Combine(report.Procedures, ", ",
                        delegate(RequestedProcedureSummary summary) { return summary.Type.Name; });
                }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Performed Location",
                delegate(ReportSummary report) { return "WHAT?"; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Performed Date",
                delegate(ReportSummary report) { return "WHAT?"; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Status",
                delegate(ReportSummary report) { return report.ReportStatus.Value; }));
        }
    }
}
