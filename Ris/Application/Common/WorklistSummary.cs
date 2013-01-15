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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class WorklistSummary : DataContractBase
    {
		public WorklistSummary()
		{

		}

        public WorklistSummary(EntityRef worklistRef, string displayName, string description,
			string className, string classCategoryName, string classDisplayName, StaffSummary ownerStaff, StaffGroupSummary ownerGroup)
        {
            WorklistRef = worklistRef;
            DisplayName = displayName;
            Description = description;
            ClassName = className;
			ClassCategoryName = classCategoryName;
			ClassDisplayName = classDisplayName;
			OwnerStaff = ownerStaff;
            OwnerGroup = ownerGroup;
        }

        public bool IsUserWorklist
        {
            get { return IsStaffOwned || IsGroupOwned; }
        }

        public bool IsStaffOwned
        {
            get { return OwnerStaff != null; }
        }

        public bool IsGroupOwned
        {
            get { return OwnerGroup != null; }
        }

        [DataMember]
        public EntityRef WorklistRef;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string Description;

		[DataMember]
        public string ClassName;

		[DataMember]
		public string ClassCategoryName;

		[DataMember]
		public string ClassDisplayName;

        [DataMember]
        public StaffSummary OwnerStaff;

        [DataMember]
        public StaffGroupSummary OwnerGroup;
    }
}
