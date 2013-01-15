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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProcedureAssembler
	{
		/// <summary>
		/// Creates the most verbose possible procedure detail.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public ProcedureDetail CreateProcedureDetail(Procedure rp, IPersistenceContext context)
		{
			return CreateProcedureDetail(rp, delegate { return true; }, true, context);
		}

		/// <summary>
		/// Creates procedure detail optionally including specified data.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="procedureStepFilter"></param>
		/// <param name="includeProtocol"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public ProcedureDetail CreateProcedureDetail(
			Procedure rp,
			Predicate<ProcedureStep> procedureStepFilter,
			bool includeProtocol,
			IPersistenceContext context)
		{
			var detail = new ProcedureDetail
							{
								ProcedureRef = rp.GetRef(),
								Status = EnumUtils.GetEnumValueInfo(rp.Status, context),
								Type = new ProcedureTypeAssembler().CreateSummary(rp.Type),
								ScheduledStartTime = rp.ScheduledStartTime,
								SchedulingCode = EnumUtils.GetEnumValueInfo(rp.SchedulingCode),
								StartTime = rp.StartTime,
								EndTime = rp.EndTime,
								CheckInTime = rp.ProcedureCheckIn.CheckInTime,
								CheckOutTime = rp.ProcedureCheckIn.CheckOutTime,
								PerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility),
								PerformingDepartment = rp.PerformingDepartment == null ? null : new DepartmentAssembler().CreateSummary(rp.PerformingDepartment, context),
								Laterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context),
								ImageAvailability = EnumUtils.GetEnumValueInfo(rp.ImageAvailability, context),
								Portable = rp.Portable,
								StudyInstanceUid = rp.StudyInstanceUID
							};

			var includedSteps = CollectionUtils.Select(rp.GetWorkflowHistory(), procedureStepFilter);
			if (includedSteps.Count > 0)
			{
				var procedureStepAssembler = new ProcedureStepAssembler();
				detail.ProcedureSteps = CollectionUtils.Map(
					includedSteps,
					(ProcedureStep ps) => procedureStepAssembler.CreateProcedureStepDetail(ps, context));
			}

			// the Protocol may be null, if this procedure has not been protocolled
			if (includeProtocol && rp.ActiveProtocol != null)
			{
				var protocolAssembler = new ProtocolAssembler();
				detail.Protocol = protocolAssembler.CreateProtocolDetail(rp.ActiveProtocol, context);
			}

			return detail;
		}

		public ProcedureSummary CreateProcedureSummary(Procedure rp, IPersistenceContext context)
		{
			var rptAssembler = new ProcedureTypeAssembler();
			var summary = new ProcedureSummary
							{
								OrderRef = rp.Order.GetRef(),
								ProcedureRef = rp.GetRef(),
								ScheduledStartTime = rp.ScheduledStartTime,
								SchedulingCode = EnumUtils.GetEnumValueInfo(rp.SchedulingCode),
								PerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility),
								Type = rptAssembler.CreateSummary(rp.Type),
								Laterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context),
								Portable = rp.Portable,
								StudyInstanceUid = rp.StudyInstanceUID
							};

			return summary;
		}
	}
}
