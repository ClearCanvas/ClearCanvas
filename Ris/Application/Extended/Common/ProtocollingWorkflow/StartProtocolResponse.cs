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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	[DataContract]
	public class StartProtocolResponse : DataContractBase
	{
		public StartProtocolResponse(
			EntityRef protocolAssignmentStepRef,
			EntityRef assignedStaffRef,
			bool protocolClaimed,
			List<OrderNoteDetail> protocolNotes,
			OrderDetail order)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.AssignedStaffRef = assignedStaffRef;
			this.ProtocolClaimed = protocolClaimed;
			this.ProtocolNotes = protocolNotes;
			this.Order = order;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public EntityRef AssignedStaffRef;

		[DataMember]
		public bool ProtocolClaimed;

		[DataMember]
		public List<OrderNoteDetail> ProtocolNotes;

		[DataMember]
		public OrderDetail Order;
	}
}