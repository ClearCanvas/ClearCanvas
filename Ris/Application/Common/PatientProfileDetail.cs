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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class PatientProfileDetail : DataContractBase
	{
		public PatientProfileDetail()
		{
			this.Mrn = new CompositeIdentifierDetail();
			this.Healthcard = new HealthcardDetail();
			this.Addresses = new List<AddressDetail>();
			this.ContactPersons = new List<ContactPersonDetail>();
			this.EmailAddresses = new List<EmailAddressDetail>();
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Notes = new List<PatientNoteDetail>();
			this.Attachments = new List<AttachmentSummary>();
			this.Allergies = new List<PatientAllergyDetail>();
			this.Name = new PersonNameDetail();
		}

		[DataMember]
		public EntityRef PatientRef;

		[DataMember]
		public EntityRef PatientProfileRef;

		[DataMember]
		public CompositeIdentifierDetail Mrn;

		[DataMember]
		public HealthcardDetail Healthcard;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public DateTime? DateOfBirth;

		[DataMember]
		public EnumValueInfo Sex;

		[DataMember]
		public EnumValueInfo PrimaryLanguage;

		[DataMember]
		public EnumValueInfo Religion;

		[DataMember]
		public bool DeathIndicator;

		[DataMember]
		public DateTime? TimeOfDeath;

		[DataMember]
		public AddressDetail CurrentHomeAddress;

		[DataMember]
		public AddressDetail CurrentWorkAddress;

		[DataMember]
		public TelephoneDetail CurrentHomePhone;

		[DataMember]
		public TelephoneDetail CurrentWorkPhone;

		[DataMember]
		public string BillingInformation;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<EmailAddressDetail> EmailAddresses;

		[DataMember]
		public List<ContactPersonDetail> ContactPersons;

		[DataMember]
		public List<PatientNoteDetail> Notes;

		[DataMember]
		public List<AttachmentSummary> Attachments;

		[DataMember]
		public List<PatientAllergyDetail> Allergies;

		public PatientProfileSummary GetSummary()
		{
			return new PatientProfileSummary
			       	{
			       		DateOfBirth = this.DateOfBirth,
			       		Healthcard = this.Healthcard,
			       		Mrn = this.Mrn,
			       		Name = this.Name,
			       		PatientProfileRef = this.PatientProfileRef,
			       		PatientRef = this.PatientRef,
			       		Sex = this.Sex
			       	};
		}
	}
}
