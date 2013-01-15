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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class StaffDetail : DataContractBase
	{
		public StaffDetail(EntityRef staffRef, string staffId, EnumValueInfo staffType,
			PersonNameDetail personNameDetail, EnumValueInfo sex,
			string title, string licenseNumber, string billingNumber,
			List<TelephoneDetail> telephoneNumbers, List<AddressDetail> addresses, List<EmailAddressDetail> emailAddresses,
			List<StaffGroupSummary> groups, Dictionary<string, string> extendedProperties, bool deactivated, string userName)
		{
			this.StaffRef = staffRef;
			this.StaffId = staffId;
			this.StaffType = staffType;
			this.Name = personNameDetail;
			this.Sex = sex;
			this.Title = title;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.TelephoneNumbers = telephoneNumbers;
			this.Addresses = addresses;
			this.EmailAddresses = emailAddresses;
			this.Groups = groups;
			this.ExtendedProperties = extendedProperties;
			this.Deactivated = deactivated;
			this.UserName = userName;
		}

		public StaffDetail()
		{
			this.Name = new PersonNameDetail();
			this.EmailAddresses = new List<EmailAddressDetail>();
			this.Addresses = new List<AddressDetail>();
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Groups = new List<StaffGroupSummary>();
			this.ExtendedProperties = new Dictionary<string, string>();
		}

		[DataMember]
		public EntityRef StaffRef;

		[DataMember]
		public string StaffId;

		[DataMember]
		public EnumValueInfo StaffType;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public EnumValueInfo Sex;

		[DataMember]
		public string Title;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public List<EmailAddressDetail> EmailAddresses;

		[DataMember]
		public List<StaffGroupSummary> Groups;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		[DataMember]
		public bool Deactivated;

		/// <summary>
		/// Name of associated user account.
		/// </summary>
		[DataMember]
		public string UserName;
	}
}
