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

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
    [ExtensionOf(typeof (SampleRuleExtensionPoint))]
    public class GrantAccessSamples : SampleRuleBase
    {
        public GrantAccessSamples()
            : base("GrantAccessSample",
                   "Grant Access by Referring Physician",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccess.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
		}
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessSamplesTwoDocs : SampleRuleBase
    {
        public GrantAccessSamplesTwoDocs()
            : base("GrantAccessSamplesTwoDocs",
                   "Grant Access by Two Referring Physicians",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessTwoDocs.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessSourceAeSamples : SampleRuleBase
    {
        public GrantAccessSourceAeSamples()
            : base("GrantAccessSourceAeSample",
                   "Grant Access by Source AE Title",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessSourceAe.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }

    [ExtensionOf(typeof(SampleRuleExtensionPoint))]
    public class GrantAccessInstitutionSamples : SampleRuleBase
    {
        public GrantAccessInstitutionSamples()
            : base("GrantAccessInstitutionSamples",
                   "Grant Access by Institution Name",
                   ServerRuleTypeEnum.DataAccess,
                   "Sample_GrantAccessInstitution.xml")
        {
            ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
        }
    }
}