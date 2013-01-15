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

using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class DiagnosticServiceAssembler
	{
		public DiagnosticServiceSummary CreateSummary(DiagnosticService diagnosticService)
		{
			return new DiagnosticServiceSummary(
				diagnosticService.GetRef(),
				diagnosticService.Id,
				diagnosticService.Name,
				diagnosticService.Deactivated);
		}

		public DiagnosticServiceDetail CreateDetail(DiagnosticService diagnosticService)
		{
			var rptAssembler = new ProcedureTypeAssembler();
			return new DiagnosticServiceDetail(
				diagnosticService.GetRef(),
				diagnosticService.Id,
				diagnosticService.Name,
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(diagnosticService.ProcedureTypes, rptAssembler.CreateSummary),
				diagnosticService.Deactivated);
		}

		public DiagnosticServicePlanDetail CreatePlanDetail(DiagnosticService diagnosticService, IPersistenceContext context)
		{
			var rptAssembler = new ProcedureTypeAssembler();
			return new DiagnosticServicePlanDetail(
				diagnosticService.GetRef(),
				diagnosticService.Id,
				diagnosticService.Name,
				diagnosticService.ProcedureTypes.Select(rpType => rptAssembler.CreateDetail(rpType, context)).ToList()
				);
		}

		public void UpdateDiagnosticService(DiagnosticService ds, DiagnosticServiceDetail detail, IPersistenceContext context)
		{
			ds.Id = detail.Id;
			ds.Name = detail.Name;
			ds.Deactivated = detail.Deactivated;

			ds.ProcedureTypes.Clear();
			ds.ProcedureTypes.AddAll(
				detail.ProcedureTypes.Select(pt => context.Load<ProcedureType>(pt.ProcedureTypeRef, EntityLoadFlags.Proxy)).ToList());
		}
	}
}
