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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProcedurePlanAssembler
    {
        public ProcedurePlanDetail CreateProcedurePlanSummary(Order order, IPersistenceContext context)
        {
            ProcedurePlanDetail detail = new ProcedurePlanDetail();

            ProcedureAssembler assembler = new ProcedureAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();

            detail.OrderRef = order.GetRef();
            detail.Procedures = CollectionUtils.Map<Procedure, ProcedureDetail>(
                order.Procedures,
                delegate(Procedure rp)
                {
                	return assembler.CreateProcedureDetail(
                		rp,
                		delegate(ProcedureStep ps) { return ps.Is<ModalityProcedureStep>(); }, // only MPS are relevant here
                		false,
                		context);
                });
            detail.DiagnosticServiceSummary =
                new DiagnosticServiceSummary(order.DiagnosticService.GetRef(), order.DiagnosticService.Id, order.DiagnosticService.Name, order.DiagnosticService.Deactivated);


            return detail;
        }
    }
}
