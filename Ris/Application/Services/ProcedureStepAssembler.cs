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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ModalityProcedureStepAssembler
	{
		public ModalityProcedureStepSummary CreateProcedureStepSummary(ModalityProcedureStep mps, IPersistenceContext context)
		{
			var psSummary = new ProcedureStepAssembler().CreateProcedureStepSummary(mps, context);
			return new ModalityProcedureStepSummary(psSummary, mps.Description);
		}
	}
	
	public class ProcedureStepAssembler
	{
		public ProcedureStepSummary CreateProcedureStepSummary(ProcedureStep ps, IPersistenceContext context)
		{
			var assembler = new ProcedureAssembler();
			var modalityAssembler = new ModalityAssembler();
			return new ProcedureStepSummary(
				ps.GetRef(),
				ps.Name,
				EnumUtils.GetEnumValueInfo(ps.State, context),
				ps.StartTime,
				ps.EndTime,
				ps.Is<ModalityProcedureStep>() ? modalityAssembler.CreateModalitySummary(ps.As<ModalityProcedureStep>().Modality) : null,
				assembler.CreateProcedureSummary(ps.Procedure, context));
		}

		public ProcedureStepDetail CreateProcedureStepDetail(ProcedureStep ps, IPersistenceContext context)
		{
			var staffAssembler = new StaffAssembler();
			var modalityAssembler = new ModalityAssembler();

			return new ProcedureStepDetail(
				ps.GetRef(),
				ps.Name,
				ps.GetClass().Name,
				ps.Is<ModalityProcedureStep>() ? ps.As<ModalityProcedureStep>().Description : null,
				EnumUtils.GetEnumValueInfo(ps.State, context),
				ps.CreationTime,
				ps.Scheduling == null ? null : ps.Scheduling.StartTime,
				ps.StartTime,
				ps.EndTime,
				ps.AssignedStaff == null ? null : staffAssembler.CreateStaffSummary(ps.AssignedStaff, context),
				ps.PerformingStaff == null ? null : staffAssembler.CreateStaffSummary(ps.PerformingStaff, context),
				ps.Is<ModalityProcedureStep>() ? modalityAssembler.CreateModalitySummary(ps.As<ModalityProcedureStep>().Modality) : null);
		}
	}
}
