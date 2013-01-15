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

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PersonNameDetail : DataContractBase, ICloneable
    {
        public PersonNameDetail(string familyName, string givenName, string middleName, string prefix, string suffix, string degree)
        {
            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.MiddleName = middleName;
            this.Prefix = prefix;
            this.Suffix = suffix;
            this.Degree = degree;
        }

        public PersonNameDetail()
        {
        }

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;

        [DataMember]
        public string MiddleName;

        [DataMember]
        public string Prefix;

        [DataMember]
        public string Suffix;

        [DataMember]
        public string Degree;

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.FamilyName, this.GivenName);
        }

        #region ICloneable Members

        public object Clone()
        {
        	return new PersonNameDetail(this.FamilyName, this.GivenName, this.MiddleName,
				this.Prefix, this.Suffix, this.Degree);
        }

        #endregion
    }
}
