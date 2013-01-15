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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
	[DataContract]
	public class ProcedureRequisition : DataContractBase
	{
		/// <summary>
		/// Constructor for use by service to return requisition back to client for editing.
		/// </summary>
		public ProcedureRequisition(
			EntityRef procedureRef,
			ProcedureTypeSummary procedureType,
			string procedureNumber,
			DateTime? scheduledTime,
			int scheduledDuration,
			ModalitySummary modality,
			EnumValueInfo schedulingCode,
			FacilitySummary performingFacility,
			DepartmentSummary performingDepartment,
			EnumValueInfo laterality,
			bool portableModality,
			bool checkedIn,
			EnumValueInfo status,
			bool canModify,
			bool cancelled)
		{
			this.ProcedureRef = procedureRef;
			this.ProcedureType = procedureType;
			this.ProcedureNumber = procedureNumber;
			this.ScheduledTime = scheduledTime;
			this.ScheduledDuration = scheduledDuration;
			this.Modality = modality;
			this.SchedulingCode = schedulingCode;
			this.PerformingFacility = performingFacility;
			this.PerformingDepartment = performingDepartment;
			this.Laterality = laterality;
			this.PortableModality = portableModality;
			this.CheckedIn = checkedIn;
			this.Status = status;
			this.CanModify = canModify;
			this.Cancelled = cancelled;
		}

		/// <summary>
		/// Constructor for use by client when initially creating a requisition.
		/// </summary>
		public ProcedureRequisition(ProcedureTypeSummary procedureType, FacilitySummary facility)
		{
			this.ProcedureType = procedureType;
			this.PerformingFacility = facility;
			this.CanModify = true;  // can modify a new requisition
		}

		/// <summary>
		/// Procedure reference.  Will be set by the server. Clients should not set or modify this field.
		/// </summary>
		[DataMember]
		public EntityRef ProcedureRef;

		/// <summary>
		/// The procedure type. Required.
		/// </summary>
		[DataMember]
		public ProcedureTypeSummary ProcedureType;

		/// <summary>
		/// Procedure number.  Will be set by the server. Clients should not set or modify this field.
		/// </summary>
		[DataMember]
		public string ProcedureNumber;

		/// <summary>
		/// Time at which this procedure is scheduled to occur. May be null, indicating that
		/// the procedure is not yet scheduled for a specific time.
		/// </summary>
		[DataMember]
		public DateTime? ScheduledTime;

		/// <summary>
		/// The duration of the block of time which the procedure is expected to take, in minutes.
		/// </summary>
		[DataMember]
		public int ScheduledDuration;

		/// <summary>
		/// The modality on which the procedure is to be performed. Optional.
		/// If not specified, the procedure will be scheduled for its default modality.
		/// </summary>
		[DataMember]
		public ModalitySummary Modality;

		/// <summary>
		/// Indicates additional info about procedure scheduling via configurable codes.  Optional.
		/// </summary>
		[DataMember]
		public EnumValueInfo SchedulingCode;

		/// <summary>
		/// Status of this procedure, set by the server.
		/// </summary>
		[DataMember]
		public EnumValueInfo Status;

		/// <summary>
		/// Facility at which this procedure will be performed. Required.
		/// </summary>
		[DataMember]
		public FacilitySummary PerformingFacility;

		/// <summary>
		/// Department at which this procedure will be performed. Optional.
		/// </summary>
		[DataMember]
		public DepartmentSummary PerformingDepartment;

		/// <summary>
		/// Indicates whether this procedure is to be performed on a portable modality.
		/// </summary>
		[DataMember]
		public bool PortableModality;

		/// <summary>
		/// Laterality for this procedure.
		/// </summary>
		[DataMember]
		public EnumValueInfo Laterality;

		/// <summary>
		/// Set by the server to indicate whether this requested procedure can be modified
		/// during an order modification (e.g. it cannot be modified if it is already in-progress).
		/// </summary>
		[DataMember]
		public bool CanModify;

		/// <summary>
		/// Indicates if an existing procedure is checked in or not, and if a new procedure should be checked in upon creation.
		/// </summary>
		[DataMember]
		public bool CheckedIn;

		/// <summary>
		/// Set by the server if this procedure is cancelled, or by the client to indicate that the procedure should be cancelled.
		/// </summary>
		[DataMember]
		public bool Cancelled;
	}
}
