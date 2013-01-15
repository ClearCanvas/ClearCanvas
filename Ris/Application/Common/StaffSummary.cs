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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class StaffSummary : DataContractBase, ICloneable, IEquatable<StaffSummary>
    {
        public StaffSummary(EntityRef staffRef, string staffId, EnumValueInfo staffType, PersonNameDetail personNameDetail, bool deactivated)
        {
            this.StaffRef = staffRef;
            this.StaffId = staffId;
            this.StaffType = staffType;
            this.Name = personNameDetail;
        	this.Deactivated = deactivated;
        }

        public StaffSummary()
        {
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public EnumValueInfo StaffType;

        [DataMember]
        public string StaffId;

        [DataMember]
        public PersonNameDetail Name;

		[DataMember]
		public bool Deactivated;

		public override string ToString()
        {
            return this.Name.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
        	return new StaffSummary(this.StaffRef, this.StaffId, this.StaffType, this.Name, this.Deactivated);
        }

        #endregion

    	public bool Equals(StaffSummary staffSummary)
    	{
    		if (staffSummary == null) return false;
    		return Equals(StaffRef, staffSummary.StaffRef);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as StaffSummary);
    	}

    	public override int GetHashCode()
    	{
    		return StaffRef != null ? StaffRef.GetHashCode() : 0;
    	}
    }
}
