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
    public class ContactPersonDetail : DataContractBase, ICloneable
    {
        public ContactPersonDetail(EnumValueInfo type, string name, string address, string homePhoneNumber, string businessPhoneNumber, EnumValueInfo relationship)
        {
            this.Type = type;
            this.Name = name;
            this.Address = address;
            this.HomePhoneNumber = homePhoneNumber;
            this.BusinessPhoneNumber = businessPhoneNumber;
            this.Relationship = relationship;
        }

        public ContactPersonDetail()
        {
        }
        
        [DataMember]
        public EnumValueInfo Type;

        [DataMember]
        public string Name;

        [DataMember]
        public string Address;

        [DataMember]
        public string HomePhoneNumber;

        [DataMember]
        public string BusinessPhoneNumber;

        [DataMember]
        public EnumValueInfo Relationship;

        #region ICloneable Members

        public object Clone()
        {
            ContactPersonDetail clone = new ContactPersonDetail();
            clone.Address = this.Address;
            clone.BusinessPhoneNumber = this.BusinessPhoneNumber;
            clone.HomePhoneNumber = this.HomePhoneNumber;
            clone.Name = this.Name;
            clone.Relationship = (EnumValueInfo)this.Relationship.Clone();
            clone.Type = (EnumValueInfo)this.Type.Clone();

            return clone;
        }

        #endregion
    }
}
