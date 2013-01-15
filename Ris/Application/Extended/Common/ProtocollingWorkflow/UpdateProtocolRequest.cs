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
	/// <summary>
	/// Data contract for updating a protocol
	/// <remarks>
	/// The contract should only be populated via the constructor.  Specifically, the protocol's supervisor should only be set via either the 
	/// "protocol" parameter in the three argument constructor, or the "supervisorRef" parameter of the two argument constructor.  This criteria 
	/// is enforced by <see cref="IProtocollingWorkflowService"/> and will cause a <see cref="RequestValidationException"/> if not met.
	/// </remarks>
	/// </summary>
	[DataContract]
	public class UpdateProtocolRequest : DataContractBase
	{
		public UpdateProtocolRequest(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.Protocol = protocol;
			this.OrderNotes = orderNotes;
		}

		public UpdateProtocolRequest(EntityRef protocolAssignmentStepRef, EntityRef supervisorRef)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public ProtocolDetail Protocol;

		[DataMember]
		public List<OrderNoteDetail> OrderNotes;

		[DataMember]
		public EntityRef SupervisorRef;
	}
}