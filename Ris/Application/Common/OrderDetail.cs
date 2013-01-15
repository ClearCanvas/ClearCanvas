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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderDetail : DataContractBase
    {
        public OrderDetail()
        {
            this.Procedures = new List<ProcedureDetail>();
        }

        [DataMember] 
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public VisitDetail Visit;

        [DataMember]
        public string PlacerNumber;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;

        [DataMember]
        public DateTime? EnteredTime;

		[DataMember]
		public StaffSummary EnteredBy;

		[DataMember]
		public string EnteredComment;

		[DataMember]
        public DateTime? SchedulingRequestTime;

        [DataMember]
        public ExternalPractitionerSummary OrderingPractitioner;

        [DataMember]
        public FacilitySummary OrderingFacility;

        [DataMember]
        public string ReasonForStudy;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo CancelReason;

		[DataMember]
		public StaffSummary CancelledBy;

		[DataMember]
		public string CancelComment;

		[DataMember]
        public List<ProcedureDetail> Procedures;

        [DataMember]
        public List<OrderNoteSummary> Notes;

		[DataMember]
		public List<AttachmentSummary> Attachments;

		[DataMember]
		public List<ResultRecipientDetail> ResultRecipients;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;
	}
}
