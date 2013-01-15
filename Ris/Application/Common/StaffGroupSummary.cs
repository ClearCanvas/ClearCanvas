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
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffGroupSummary : DataContractBase, IEquatable<StaffGroupSummary>
    {
        public StaffGroupSummary(EntityRef groupRef, string name, string description, bool isElective, bool deactivated)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
        	this.IsElective = isElective;
        	this.Deactivated = deactivated;
        }

        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        public StaffGroupSummary()
        {
        }

        [DataMember]
        public EntityRef StaffGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

		[DataMember]
		public bool IsElective;

		[DataMember]
		public bool Deactivated;


    	public bool Equals(StaffGroupSummary staffGroupSummary)
    	{
    		if (staffGroupSummary == null) return false;
    		return Equals(StaffGroupRef, staffGroupSummary.StaffGroupRef);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as StaffGroupSummary);
    	}

    	public override int GetHashCode()
    	{
    		return StaffGroupRef != null ? StaffGroupRef.GetHashCode() : 0;
    	}
    }
}
