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

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class OrderListItem : VisitListItem
	{
		public OrderListItem()
		{
		}

		#region Order

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public string PlacerNumber;

		[DataMember]
		public string AccessionNumber;

		[DataMember]
		public DiagnosticServiceSummary DiagnosticService;

		[DataMember]
		public DateTime? EnteredTime;

		[DataMember]
		public DateTime? SchedulingRequestTime;

		[DataMember]
		public DateTime? OrderScheduledStartTime;

		[DataMember]
		public ExternalPractitionerSummary OrderingPractitioner;

		[DataMember]
		public FacilitySummary OrderingFacility;

		[DataMember]
		public string ReasonForStudy;

		[DataMember]
		public EnumValueInfo OrderPriority;

		[DataMember]
		public EnumValueInfo OrderStatus;

		[DataMember]
		public EnumValueInfo CancelReason;

		#endregion

		#region Procedure

		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public ProcedureTypeSummary ProcedureType;

		[DataMember]
		public DateTime? ProcedureScheduledStartTime;

		[DataMember]
		public EnumValueInfo ProcedureSchedulingCode;

		[DataMember]
		public DateTime? ProcedureCheckInTime;

		[DataMember]
		public DateTime? ProcedureCheckOutTime;

		[DataMember]
		public EnumValueInfo ProcedureStatus;

		[DataMember]
		public FacilitySummary ProcedurePerformingFacility;

		[DataMember]
		public bool ProcedurePortable;

		[DataMember]
		public EnumValueInfo ProcedureLaterality;

		#endregion
	}
}
