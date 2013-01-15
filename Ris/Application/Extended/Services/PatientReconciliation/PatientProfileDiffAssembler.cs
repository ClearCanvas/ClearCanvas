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

using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Application.Extended.Services.PatientReconciliation
{
    public class PatientProfileDiffAssembler
    {
        public PatientProfileDiff CreatePatientProfileDiff(PatientProfile left, PatientProfile right, IList<DiscrepancyTestResult> results)
        {
            var diff = new PatientProfileDiff();
            diff.LeftProfileAssigningAuthority = left.Mrn.AssigningAuthority.Code;
            diff.RightProfileAssigningAuthority = right.Mrn.AssigningAuthority.Code;

            diff.DateOfBirth = CreatePropertyDiff(PatientProfileDiscrepancy.DateOfBirth, results);
            diff.FamilyName = CreatePropertyDiff(PatientProfileDiscrepancy.FamilyName, results);
            diff.GivenName = CreatePropertyDiff(PatientProfileDiscrepancy.GivenName, results);
            diff.Healthcard = CreatePropertyDiff(PatientProfileDiscrepancy.Healthcard, results);
            diff.HomeAddress = CreatePropertyDiff(PatientProfileDiscrepancy.HomeAddress, results);
            diff.HomePhone = CreatePropertyDiff(PatientProfileDiscrepancy.HomePhone, results);
            diff.MiddleName = CreatePropertyDiff(PatientProfileDiscrepancy.MiddleName, results);
            diff.Sex = CreatePropertyDiff(PatientProfileDiscrepancy.Sex, results);
            diff.WorkAddress = CreatePropertyDiff(PatientProfileDiscrepancy.WorkAddress, results);
            diff.WorkPhone = CreatePropertyDiff(PatientProfileDiscrepancy.WorkPhone, results);

            return diff;
        }

        private PropertyDiff CreatePropertyDiff(PatientProfileDiscrepancy discrepancy, IList<DiscrepancyTestResult> results)
        {
            var result = CollectionUtils.SelectFirst<DiscrepancyTestResult>(results, r => r.Discrepancy == discrepancy);

            if (result != null)
            {
                var propDiff = new PropertyDiff();
                propDiff.IsDiscrepant = result.IsDiscrepant;
                propDiff.AlignedLeftValue = result.Diff.AlignedLeft;
                propDiff.AlignedRightValue = result.Diff.AlignedRight;
                propDiff.DiffMask = result.Diff.DiffMask;
                return propDiff;
            }
            return null;
        }
    }
}
