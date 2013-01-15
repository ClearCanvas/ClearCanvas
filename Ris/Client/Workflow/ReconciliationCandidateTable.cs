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

namespace ClearCanvas.Ris.Client.Workflow
{
    class ReconciliationCandidateTable : Table<ReconciliationCandidateTableEntry>
    {
        public ReconciliationCandidateTable()
        {
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, bool>(SR.ColumnAbbreviationReconciliation,
                   delegate(ReconciliationCandidateTableEntry item) { return item.Checked; },
                   delegate(ReconciliationCandidateTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnScore,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.Score.ToString(); }, 1.0f));
            this.Columns.Add(
                new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSite,
                    delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Mrn.AssigningAuthority.Code; }, 0.5f));
            this.Columns.Add(
               new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnMRN,
                   delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Mrn.Id; }, 1.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnName,
                  delegate(ReconciliationCandidateTableEntry item) { return PersonNameFormat.Format(item.ReconciliationCandidate.PatientProfile.Name); }, 2.0f));
            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnHealthcardNumber,
                  delegate(ReconciliationCandidateTableEntry item) { return HealthcardFormat.Format(item.ReconciliationCandidate.PatientProfile.Healthcard); }, 1.0f));

            DateTimeTableColumn<ReconciliationCandidateTableEntry> dateOfBirthColumn =
              new DateTimeTableColumn<ReconciliationCandidateTableEntry>(SR.ColumnDateOfBirth,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.DateOfBirth; }, 1.0f);
            this.Columns.Add(dateOfBirthColumn);

            this.Columns.Add(
              new TableColumn<ReconciliationCandidateTableEntry, string>(SR.ColumnSex,
                  delegate(ReconciliationCandidateTableEntry item) { return item.ReconciliationCandidate.PatientProfile.Sex.Value; }, 0.5f));
        }
    }
}
