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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistItemSummary : WorklistItemSummaryBase
    {
        public ReportingWorklistItemSummary(
            EntityRef procedureStepRef,
            EntityRef procedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            EntityRef reportRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string procedureName,
            bool procedurePortable,
            bool hasErrors,
            EnumValueInfo procedureLaterality,
            string procedureStepName,
            DateTime? time,
            EnumValueInfo activityStatus,
            int reportPartIndex)
            : base(
                procedureStepRef,
                procedureRef,
                orderRef,
                patientRef,
                profileRef,
                mrn,
                name,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                procedureName,
                procedurePortable,
                procedureLaterality,
                procedureStepName,
                time
            )
        {
            this.ReportRef = reportRef;
            this.ActivityStatus = activityStatus;
            this.ReportPartIndex = reportPartIndex;
            this.HasErrors = hasErrors;
        }

        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ActivityStatus;

        [DataMember]
        public int ReportPartIndex;

        [DataMember]
        public bool HasErrors;

        /// <summary>
        /// Gets a value indicating if this worklist item refers to an addendum.
        /// </summary>
        public bool IsAddendumStep
        {
            get { return this.ReportPartIndex > 0; }
        }
    }
}
