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

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class WorklistClassSummary : DataContractBase, IEquatable<WorklistClassSummary>
    {
        /// <summary>
        /// No-args constructor required by Oto scripts.
        /// </summary>
        public WorklistClassSummary()
        {
        }

        public WorklistClassSummary(string className, string displayName, string categoryName, string description, 
            string procedureTypeGroupClassName, string procedureTypeGroupClassDisplayName, 
            bool supportsReportingStaffRoleFilters)
        {
            ClassName = className;
            DisplayName = displayName;
            CategoryName = categoryName;
            ProcedureTypeGroupClassName = procedureTypeGroupClassName;
            Description = description;
            ProcedureTypeGroupClassDisplayName = procedureTypeGroupClassDisplayName;
            SupportsReportingStaffRoleFilters = supportsReportingStaffRoleFilters;
        }

        [DataMember]
        public string ClassName;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string CategoryName;

        [DataMember]
        public string Description;

        [DataMember]
        public string ProcedureTypeGroupClassName;

        [DataMember]
        public string ProcedureTypeGroupClassDisplayName;

        [DataMember]
        public bool SupportsReportingStaffRoleFilters;

        #region IEquatable overrides

        public bool Equals(WorklistClassSummary worklistClassSummary)
        {
            if (worklistClassSummary == null) return false;
            return Equals(ClassName, worklistClassSummary.ClassName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistClassSummary);
        }

        public override int GetHashCode()
        {
            return ClassName != null ? ClassName.GetHashCode() : 0;
        }

        #endregion
    }
}
