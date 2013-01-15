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

using System.Xml;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProcedureTypeAssembler
	{
		public ProcedureTypeSummary CreateSummary(ProcedureType rpt)
		{
			return new ProcedureTypeSummary(rpt.GetRef(), rpt.Name, rpt.Id, rpt.DefaultDuration, rpt.Deactivated);
		}

		public ProcedureTypeDetail CreateDetail(ProcedureType procedureType, IPersistenceContext context)
		{
			if(procedureType.Plan.IsDefault)
			{
				var modalityAssembler = new ModalityAssembler();
				return new ProcedureTypeDetail(
					procedureType.GetRef(),
					procedureType.Id,
					procedureType.Name,
					procedureType.Plan.DefaultModality == null ? null : modalityAssembler.CreateModalitySummary(procedureType.Plan.DefaultModality),
					procedureType.DefaultDuration,
					procedureType.Deactivated);
			}

			return new ProcedureTypeDetail(
				procedureType.GetRef(),
				procedureType.Id,
				procedureType.Name,
				procedureType.BaseType == null ? null : CreateSummary(procedureType.BaseType),
				procedureType.Plan.ToString(),
				procedureType.DefaultDuration,
				procedureType.Deactivated);
		}

		public void UpdateProcedureType(ProcedureType procType, ProcedureTypeDetail detail, IPersistenceContext context)
		{
			procType.Id = detail.Id;
			procType.Name = detail.Name;
			procType.BaseType = detail.CustomProcedurePlan && detail.BaseType != null
									? context.Load<ProcedureType>(detail.BaseType.ProcedureTypeRef, EntityLoadFlags.Proxy)
									: null;
			procType.DefaultDuration = detail.DefaultDuration;
			procType.Deactivated = detail.Deactivated;

			try
			{
				if(detail.CustomProcedurePlan)
				{
					procType.Plan = new ProcedurePlan(detail.PlanXml);
				}
				else
				{
					var modality = context.Load<Modality>(detail.DefaultModality.ModalityRef);
					procType.Plan = ProcedurePlan.CreateDefaultPlan(detail.Name, modality);
				}
			}
			catch (XmlException e)
			{
				throw new RequestValidationException(string.Format("Procedure plan XML is invalid: {0}", e.Message));
			}
		}
	}
}
