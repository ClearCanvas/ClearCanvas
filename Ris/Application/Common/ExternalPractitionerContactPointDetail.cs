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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerContactPointDetail : DataContractBase, ICloneable
	{
		public ExternalPractitionerContactPointDetail(
			EntityRef contactPointRef, string name, string description, bool isDefaultContactPoint,
			EnumValueInfo preferredResultCommunicationMode, EnumValueInfo informationAuthority,
			List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails, List<EmailAddressDetail> emailAddressDetails,
			TelephoneDetail currentPhone, TelephoneDetail currentFax, AddressDetail currentAddress, EmailAddressDetail currentEmailAddress,
			ExternalPractitionerContactPointSummary mergeDestination, bool isMerged, bool deactivated)
		{
			this.ContactPointRef = contactPointRef;
			this.Name = name;
			this.Description = description;
			this.IsDefaultContactPoint = isDefaultContactPoint;
			this.PreferredResultCommunicationMode = preferredResultCommunicationMode;
			this.InformationAuthority = informationAuthority;
			this.TelephoneNumbers = phoneDetails;
			this.Addresses = addressDetails;
			this.EmailAddresses = emailAddressDetails;
			this.CurrentPhoneNumber = currentPhone;
			this.CurrentFaxNumber = currentFax;
			this.CurrentAddress = currentAddress;
			this.CurrentEmailAddress = currentEmailAddress;
			this.MergeDestination = mergeDestination;
			this.IsMerged = isMerged;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerContactPointDetail()
		{
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Addresses = new List<AddressDetail>();
			this.EmailAddresses = new List<EmailAddressDetail>();
		}

		[DataMember]
		public EntityRef ContactPointRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public bool IsDefaultContactPoint;

		[DataMember]
		public EnumValueInfo PreferredResultCommunicationMode;

		[DataMember]
		public EnumValueInfo InformationAuthority;

		[DataMember]
		public TelephoneDetail CurrentPhoneNumber;

		[DataMember]
		public TelephoneDetail CurrentFaxNumber;

		[DataMember]
		public AddressDetail CurrentAddress;

		[DataMember]
		public EmailAddressDetail CurrentEmailAddress;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public List<EmailAddressDetail> EmailAddresses;

		[DataMember]
		public ExternalPractitionerContactPointSummary MergeDestination;

		[DataMember]
		public bool IsMerged;

		[DataMember]
		public bool Deactivated;

		public ExternalPractitionerContactPointSummary GetSummary()
		{
			return new ExternalPractitionerContactPointSummary(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				this.IsMerged,
				this.Deactivated);
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerContactPointDetail(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				(EnumValueInfo)(this.PreferredResultCommunicationMode == null ? null : this.PreferredResultCommunicationMode.Clone()),
				(EnumValueInfo)(this.InformationAuthority == null ? null : this.InformationAuthority.Clone()),
				CollectionUtils.Map(this.TelephoneNumbers, (TelephoneDetail detail) => (TelephoneDetail) detail.Clone()),
				CollectionUtils.Map(this.Addresses, (AddressDetail detail) => (AddressDetail) detail.Clone()),
				CollectionUtils.Map(this.EmailAddresses, (EmailAddressDetail detail) => (EmailAddressDetail)detail.Clone()),
				(TelephoneDetail)(this.CurrentPhoneNumber == null ? null : this.CurrentPhoneNumber.Clone()),
				(TelephoneDetail) (this.CurrentFaxNumber == null ? null : this.CurrentFaxNumber.Clone()),
				(AddressDetail) (this.CurrentAddress == null ? null : this.CurrentAddress.Clone()),
				(EmailAddressDetail)(this.CurrentEmailAddress == null ? null : this.CurrentEmailAddress.Clone()),
				(ExternalPractitionerContactPointSummary)(this.MergeDestination == null ? null : this.MergeDestination.Clone()),
				this.IsMerged,
				this.Deactivated
			);
		}

		#endregion
	}
}
