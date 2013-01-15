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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class AddressAssembler
    {
        public AddressDetail CreateAddressDetail(Address address, IPersistenceContext context)
        {
            if (address == null)
                return null;

            AddressDetail addressDetail = new AddressDetail();

            addressDetail.Street = address.Street;
            addressDetail.Unit = address.Unit;
            addressDetail.City = address.City;
            addressDetail.Province = address.Province;
            addressDetail.PostalCode = address.PostalCode;
            addressDetail.Country = address.Country;

            addressDetail.Type = EnumUtils.GetEnumValueInfo(address.Type, context);

            addressDetail.ValidRangeFrom = address.ValidRange.From;
            addressDetail.ValidRangeUntil = address.ValidRange.Until;

            return addressDetail;
        }

        public Address CreateAddress(AddressDetail addressDetail)
        {
            if (addressDetail == null)
                return null;

            Address newAddress = new Address();

            newAddress.Street = addressDetail.Street;
            newAddress.Unit = addressDetail.Unit;
            newAddress.City = addressDetail.City;
            newAddress.Province = addressDetail.Province;
            newAddress.PostalCode = addressDetail.PostalCode;
            newAddress.Country = addressDetail.Country;
            newAddress.Type = EnumUtils.GetEnumValue<AddressType>(addressDetail.Type);
            newAddress.ValidRange.From = addressDetail.ValidRangeFrom;
            newAddress.ValidRange.Until = addressDetail.ValidRangeUntil;

            return newAddress;
        }
    }
}
