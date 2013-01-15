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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ModalityProcedureStepSummary : ProcedureStepSummary
	{
		public ModalityProcedureStepSummary(ProcedureStepSummary ps, string description)
			: base(ps.ProcedureStepRef, ps.ProcedureStepName, ps.State, ps.StartTime, ps.EndTime, ps.Modality, ps.Procedure)
		{
			this.Description = description;
		}

		public ModalityProcedureStepSummary(
			EntityRef procedureStepRef,
			string procedureStepName,
			EnumValueInfo state,
			DateTime? startTime,
			DateTime? endTime,
			ModalitySummary modality,
			ProcedureSummary procedure,
			String description)
			: base(procedureStepRef, procedureStepName, state, startTime, endTime, modality, procedure)
		{
			this.Description = description;
		}

		[DataMember]
		public string Description;
	}
}
