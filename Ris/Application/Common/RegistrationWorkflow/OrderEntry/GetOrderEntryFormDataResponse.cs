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

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class GetOrderEntryFormDataResponse : DataContractBase
	{
		public GetOrderEntryFormDataResponse(
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<ModalitySummary> modalityChoices,
			List<EnumValueInfo> orderPriorityChoices,
			List<EnumValueInfo> cancelReasonChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
		{
			this.FacilityChoices = facilityChoices;
			this.DepartmentChoices = departmentChoices;
			this.ModalityChoices = modalityChoices;
			this.OrderPriorityChoices = orderPriorityChoices;
			this.CancelReasonChoices = cancelReasonChoices;
			this.LateralityChoices = lateralityChoices;
			this.SchedulingCodeChoices = schedulingCodeChoices;
		}

		[DataMember]
		public List<FacilitySummary> FacilityChoices;

		[DataMember]
		public List<DepartmentSummary> DepartmentChoices;

		[DataMember]
		public List<EnumValueInfo> OrderPriorityChoices;

		[DataMember]
		public List<EnumValueInfo> CancelReasonChoices;

		[DataMember]
		public List<EnumValueInfo> LateralityChoices;

		[DataMember]
		public List<EnumValueInfo> SchedulingCodeChoices;

		[DataMember]
		public List<ModalitySummary> ModalityChoices;
	}
}
