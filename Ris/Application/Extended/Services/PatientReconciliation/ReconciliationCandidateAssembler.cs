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

using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.PatientReconciliation
{
    public class ReconciliationCandidateAssembler
    {
        public ReconciliationCandidate CreateReconciliationCandidate(PatientProfileMatch profileMatch, IPersistenceContext context)
        {
            var rc = new ReconciliationCandidate();

            var profileAssembler = new PatientProfileAssembler();
            rc.PatientProfile = profileAssembler.CreatePatientProfileSummary(profileMatch.PatientProfile, context);
            switch (profileMatch.Score)
            {
                case PatientProfileMatch.ScoreValue.High:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.High;
                    break;
                case PatientProfileMatch.ScoreValue.Moderate:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.Moderate;
                    break;
                case PatientProfileMatch.ScoreValue.Low:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.Low;
                    break;
            }
            return rc;
        }
    }
}
