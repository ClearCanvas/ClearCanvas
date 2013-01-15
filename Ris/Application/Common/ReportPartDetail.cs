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
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ReportPartDetail : DataContractBase
    {
        public ReportPartDetail(
            EntityRef reportPartRef,
            int index,
            bool isAddendum,
            EnumValueInfo status,
            DateTime creationTime,
            DateTime? preliminaryTime,
            DateTime? completedTime,
            DateTime? cancelledTime,
            StaffSummary supervisor,
            StaffSummary interpretedBy,
            StaffSummary transcribedBy,
            StaffSummary transcriptionSupervisor,
            StaffSummary verifiedBy,
            EnumValueInfo transcriptionRejectReason,
            Dictionary<string, string> extendedProperties)
        {
            this.ReportPartRef = reportPartRef;
            this.Index = index;
            this.IsAddendum = isAddendum;
            this.Status = status;
            this.CreationTime = creationTime;
            this.PreliminaryTime = preliminaryTime;
            this.CompletedTime = completedTime;
            this.CancelledTime = cancelledTime;
            this.Supervisor = supervisor;
            this.InterpretedBy = interpretedBy;
            this.TranscribedBy = transcribedBy;
            this.TranscriptionSupervisor = transcriptionSupervisor;
            this.VerifiedBy = verifiedBy;
            this.TranscriptionRejectReason = transcriptionRejectReason;
            this.ExtendedProperties = extendedProperties;
        }

        public static string ReportContentKey = "ReportContent";

        [DataMember]
        public EntityRef ReportPartRef;

        [DataMember]
        public int Index;

        [DataMember]
        public bool IsAddendum;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public DateTime CreationTime;

        [DataMember]
        public DateTime? PreliminaryTime;

        [DataMember]
        public DateTime? CompletedTime;

        [DataMember]
        public DateTime? CancelledTime;

        [DataMember]
        public StaffSummary Supervisor;

        [DataMember]
        public StaffSummary InterpretedBy;

        [DataMember]
        public StaffSummary TranscribedBy;

        [DataMember]
        public StaffSummary TranscriptionSupervisor;

        [DataMember]
        public StaffSummary VerifiedBy;

        [DataMember]
        public EnumValueInfo TranscriptionRejectReason;

        [DataMember]
        public Dictionary<string, string> ExtendedProperties;

        public string Content
        {
            get
            {
                if (ExtendedProperties == null || !ExtendedProperties.ContainsKey(ReportContentKey))
                    return null;

                return ExtendedProperties[ReportContentKey];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (ExtendedProperties != null && ExtendedProperties.ContainsKey(ReportContentKey))
                    {
                        ExtendedProperties.Remove(ReportContentKey);
                    }
                }
                else
                {
                    if (ExtendedProperties == null)
                        ExtendedProperties = new Dictionary<string, string>();

                    ExtendedProperties[ReportContentKey] = value;
                }
            }
        }
    }
}