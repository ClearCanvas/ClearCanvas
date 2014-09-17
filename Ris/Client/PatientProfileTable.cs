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
    public class PatientProfileTable : Table<PatientProfileSummary>
    {
        public PatientProfileTable()
        {
            this.Columns.Add(
               new TableColumn<PatientProfileSummary, string>(SR.ColumnMRN,
                   delegate(PatientProfileSummary profile) { return MrnFormat.Format(profile.Mrn); }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnPatientName,
                  delegate(PatientProfileSummary profile) { return PersonNameFormat.Format(profile.Name); }, 2.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnHealthcardNumber,
                  delegate(PatientProfileSummary profile) { return HealthcardFormat.Format(profile.Healthcard); }, 1.0f));
            this.Columns.Add(
              new DateTableColumn<PatientProfileSummary>(SR.ColumnDateOfBirth,
                  delegate(PatientProfileSummary profile) { return profile.DateOfBirth; }, 1.0f));
            this.Columns.Add(
              new TableColumn<PatientProfileSummary, string>(SR.ColumnSex,
                  delegate(PatientProfileSummary profile) { return profile.Sex.Value; }, 0.5f));
        }
    }
}
