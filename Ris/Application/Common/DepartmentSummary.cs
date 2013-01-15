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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class DepartmentSummary : DataContractBase, IEquatable<DepartmentSummary>
	{
		public DepartmentSummary()
		{
		}

		public DepartmentSummary(EntityRef departmentRef, string id, string name, string facilityCode, string facilityName, bool deactivated)
		{
			this.DepartmentRef = departmentRef;
			this.Id = id;
			this.Name = name;
			this.FacilityCode = facilityCode;
			this.FacilityName = facilityName;
			this.Deactivated = deactivated;
		}

		[DataMember]
		public EntityRef DepartmentRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public string FacilityCode;

		[DataMember]
		public string FacilityName;

		[DataMember]
		public bool Deactivated;

		public bool Equals(DepartmentSummary other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.DepartmentRef, DepartmentRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as DepartmentSummary);
		}

		public override int GetHashCode()
		{
			return (DepartmentRef != null ? DepartmentRef.GetHashCode() : 0);
		}
	}
}