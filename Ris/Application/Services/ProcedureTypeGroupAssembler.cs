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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProcedureTypeGroupAssembler
    {
        public ProcedureTypeGroupSummary GetProcedureTypeGroupSummary(ProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            EnumValueInfo category = GetCategoryEnumValueInfo(rptGroup.GetType());
            return new ProcedureTypeGroupSummary(rptGroup.GetRef(), rptGroup.Name, rptGroup.Description, category);
        }

        public ProcedureTypeGroupDetail GetProcedureTypeGroupDetail(ProcedureTypeGroup rptGroup, IPersistenceContext context)
        {
            ProcedureTypeGroupDetail detail = new ProcedureTypeGroupDetail();

            detail.Name = rptGroup.Name;
            detail.Description = rptGroup.Description;
            detail.Category = GetCategoryEnumValueInfo(rptGroup.GetType());

            ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
            detail.ProcedureTypes = CollectionUtils.Map<ProcedureType, ProcedureTypeSummary, List<ProcedureTypeSummary>>(
                rptGroup.ProcedureTypes,
                delegate (ProcedureType rpt)
                    {
                        return assembler.CreateSummary(rpt);
                    });

            return detail;
        }

        public EnumValueInfo GetCategoryEnumValueInfo(Type groupClass)
        {
            // this is a bit hokey but avoids having to modify the client code that is expecting an EnumValueInfo
            return new EnumValueInfo(groupClass.AssemblyQualifiedName, TerminologyTranslator.Translate(groupClass));
        }

        public void UpdateProcedureTypeGroup(ProcedureTypeGroup group, ProcedureTypeGroupDetail detail, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;
            
            group.ProcedureTypes.Clear();
            detail.ProcedureTypes.ForEach(
                delegate(ProcedureTypeSummary summary)
                    {
                        group.ProcedureTypes.Add(context.Load<ProcedureType>(summary.ProcedureTypeRef));
                    });
        }
    }
}