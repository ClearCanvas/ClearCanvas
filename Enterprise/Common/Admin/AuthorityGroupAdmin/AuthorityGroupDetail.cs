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
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
    public class AuthorityGroupDetail : DataContractBase
    {
        public AuthorityGroupDetail(EntityRef authorityGroupRef, string name, string description, bool builtIn, bool dataGroup, List<AuthorityTokenSummary> authorityTokens)
        {
            AuthorityGroupRef = authorityGroupRef;
            Name = name;
            AuthorityTokens = authorityTokens;
            Description = description;
        	BuiltIn = builtIn;
            DataGroup = dataGroup;
        }

        public AuthorityGroupDetail()
        {
            AuthorityTokens = new List<AuthorityTokenSummary>();
        }

        [DataMember]
        public EntityRef AuthorityGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public List<AuthorityTokenSummary> AuthorityTokens;

        [DataMember]
        public string Description;

		[DataMember]
		public bool BuiltIn;

		[DataMember]
        public bool DataGroup;

    	public AuthorityGroupSummary GetSummary()
    	{
    		return new AuthorityGroupSummary(AuthorityGroupRef, Name, Description, BuiltIn, DataGroup);
    	}
    }
}
