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
using System;

namespace ClearCanvas.Healthcare.Imex
{
	[DataContract]
	public class ReferenceEntityDataBase
	{
		[DataMember]
		public bool Deactivated;
	}

    [DataContract]
    public class TelephoneNumberData
    {
        public TelephoneNumberData()
        {
        }

        public TelephoneNumberData(TelephoneNumber tn)
        {
            this.CountryCode = tn.CountryCode;
            this.AreaCode = tn.AreaCode;
            this.Number = tn.Number;
            this.Ext = tn.Extension;
            this.Use = tn.Use.ToString();
            this.Equipment = tn.Equipment.ToString();

            if(tn.ValidRange != null)
            {
                this.ValidFrom = tn.ValidRange.From;
                this.ValidUntil = tn.ValidRange.Until;
            }
        }

        public TelephoneNumber CreateTelephoneNumber()
        {
            return new TelephoneNumber(
                this.CountryCode,
                this.AreaCode,
                this.Number,
                this.Ext,
                (TelephoneUse)Enum.Parse(typeof(TelephoneUse), this.Use),
                (TelephoneEquipment)Enum.Parse(typeof(TelephoneEquipment), this.Equipment),
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string CountryCode;

        [DataMember]
        public string AreaCode;

        [DataMember]
        public string Number;

        [DataMember]
        public string Ext;

        [DataMember]
        public string Use;

        [DataMember]
        public string Equipment;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

    [DataContract]
    public class AddressData
    {
        public AddressData()
        {
        }

        public AddressData(Address a)
        {
            this.Street = a.Street;
            this.Unit = a.Unit;
            this.City = a.City;
            this.Province = a.Province;
            this.PostalCode = a.PostalCode;
            this.Country = a.Country;
            this.AddressType = a.Type.ToString();

            if (a.ValidRange != null)
            {
                this.ValidFrom = a.ValidRange.From;
                this.ValidUntil = a.ValidRange.Until;
            }
        }

        public Address CreateAddress()
        {
            return new Address(
                this.Street,
                this.Unit,
                this.City,
                this.Province,
                this.PostalCode,
                this.Country,
                (AddressType)Enum.Parse(typeof(AddressType), this.AddressType),
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string Street;

        [DataMember]
        public string Unit;

        [DataMember]
        public string City;

        [DataMember]
        public string Province;

        [DataMember]
        public string PostalCode;

        [DataMember]
        public string Country;

        [DataMember]
        public string AddressType;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

    [DataContract]
    public class EmailAddressData
    {
        public EmailAddressData()
        {
        }

        public EmailAddressData(EmailAddress e)
        {
            this.EmailAddress = e.Address;
            if (e.ValidRange != null)
            {
                this.ValidFrom = e.ValidRange.From;
                this.ValidUntil = e.ValidRange.Until;
            }
        }

        public EmailAddress CreateEmailAddress()
        {
            return new EmailAddress(
                this.EmailAddress,
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string EmailAddress;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

}