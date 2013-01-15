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
    public class ProcedureTypeGroupSummary : DataContractBase, IEquatable<ProcedureTypeGroupSummary>
    {
        public ProcedureTypeGroupSummary(EntityRef entityRef, string name, string description, EnumValueInfo category)
        {
			ProcedureTypeGroupRef = entityRef;
            Name = name;
            Description = description;
            Category = category;
        }

        [DataMember]
        public EntityRef ProcedureTypeGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Category;


        public bool Equals(ProcedureTypeGroupSummary procedureTypeGroupSummary)
        {
            if (procedureTypeGroupSummary == null) return false;
            return Equals(this.ProcedureTypeGroupRef, procedureTypeGroupSummary.ProcedureTypeGroupRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ProcedureTypeGroupSummary);
        }

        public override int GetHashCode()
        {
        	return ProcedureTypeGroupRef.GetHashCode();
        }
    }
}