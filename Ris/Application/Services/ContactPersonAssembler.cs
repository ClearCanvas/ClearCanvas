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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ContactPersonAssembler
    {
        public ContactPersonDetail CreateContactPersonDetail(ContactPerson cp)
        {
            ContactPersonDetail detail = new ContactPersonDetail();

            detail.Name = cp.Name;
            detail.Address = cp.Address;
            detail.HomePhoneNumber = cp.HomePhone;
            detail.BusinessPhoneNumber = cp.BusinessPhone;
            detail.Relationship = EnumUtils.GetEnumValueInfo(cp.Relationship);
            detail.Type = EnumUtils.GetEnumValueInfo(cp.Type);

            return detail;
        }

        public ContactPerson CreateContactPerson(ContactPersonDetail detail, IPersistenceContext context)
        {
            ContactPerson cp = new ContactPerson();

            cp.Name = detail.Name;
            cp.Address = detail.Address;
            cp.HomePhone = detail.HomePhoneNumber;
            cp.BusinessPhone = detail.BusinessPhoneNumber;
            cp.Relationship = EnumUtils.GetEnumValue<ContactPersonRelationshipEnum>(detail.Relationship, context);
            cp.Type = EnumUtils.GetEnumValue<ContactPersonTypeEnum>(detail.Type, context);

            return cp;
        }
    }
}
