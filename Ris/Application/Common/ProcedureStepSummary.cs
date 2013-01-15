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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ProcedureStepSummary : DataContractBase
	{
		public ProcedureStepSummary(
			EntityRef procedureStepRef,
			string procedureStepName,
			EnumValueInfo state,
			DateTime? startTime,
			DateTime? endTime,
			ModalitySummary modality,
			ProcedureSummary procedure
			)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ProcedureStepName = procedureStepName;
			this.State = state;
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.Modality = modality;
			this.Procedure = procedure;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public string ProcedureStepName;

		[DataMember]
		public EnumValueInfo State;

		[DataMember]
		public DateTime? StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public ProcedureSummary Procedure;

		/// <summary>
		/// Specifies the modality of a MPS.  This field is null for other types of procedure step.
		/// </summary>
		[DataMember]
		public ModalitySummary Modality;

	}
}
