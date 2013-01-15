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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow
{
    public static class PatientReconciliation
    {
        public static bool ShowReconciliationDialog(EntityRef targetProfile, IDesktopWindow window)
        {
            IList<ReconciliationCandidate> candidates = null;
            IList<PatientProfileSummary> reconciledProfiles = null;

            Platform.GetService<IPatientReconciliationService>(
                delegate(IPatientReconciliationService service)
                {
                    ListPatientReconciliationMatchesResponse response =
                        service.ListPatientReconciliationMatches(new ListPatientReconciliationMatchesRequest(targetProfile));

                    candidates = response.MatchCandidates;
                    reconciledProfiles = response.ReconciledProfiles;
                });

            if (candidates.Count > 0)
            {
                ReconciliationComponent component = new ReconciliationComponent(targetProfile, reconciledProfiles, candidates);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    window,
                    component,
                    SR.TitlePatientReconciliation);
                return exitCode == ApplicationComponentExitCode.Accepted;
            }
            else
            {
                window.ShowMessageBox(SR.MessageNoReconciliationCandidate, MessageBoxActions.Ok);
                return false;
            }
        }
    }
}
