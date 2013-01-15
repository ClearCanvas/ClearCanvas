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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("ExternalPractitioner")]
	public class ExternalPractitionerImex : XmlEntityImex<ExternalPractitioner, ExternalPractitionerImex.ExternalPractitionerData>
	{
		[DataContract]
		public class ExternalPractitionerData : ReferenceEntityDataBase
		{
			[DataMember]
			public string FamilyName;

			[DataMember]
			public string GivenName;

			[DataMember]
			public string MiddleName;

			[DataMember]
			public string LicenseNumber;

			[DataMember]
			public string BillingNumber;

			/// <summary>
			/// The IsVerified property is exported and is used when importing a new practitioner.  It is not imported for existing practitioner.
			/// </summary>
			[DataMember]
			public bool IsVerified;

			/// <summary>
			/// The LastVerifiedTime is exported and is used when importing a new practitioner.  It is not imported for existing practitioner
			/// </summary>
			[DataMember]
			public DateTime? LastVerifiedTime;

			/// <summary>
			/// The LastEditedTime is exported and is used when importing a new practitioner.  It is not imported for existing practitioner
			/// </summary>
			[DataMember]
			public DateTime? LastEditedTime;

			[DataMember]
			public List<ExternalPractitionerContactPointData> ContactPoints;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;
		}

		[DataContract]
		public class ExternalPractitionerContactPointData
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string PreferredResultCommunicationMode;

			[DataMember]
			public string InformationAuthority;

			[DataMember]
			public bool IsDefaultContactPoint;

			[DataMember]
			public List<TelephoneNumberData> TelephoneNumbers;

			[DataMember]
			public List<AddressData> Addresses;

			[DataMember]
			public List<EmailAddressData> EmailAddresses;
		}

		#region Overrides

		protected override IList<ExternalPractitioner> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new ExternalPractitionerSearchCriteria();
			where.Name.FamilyName.SortAsc(0);
			where.Name.GivenName.SortAsc(1);
			where.Name.MiddleName.SortAsc(2);
			where.LicenseNumber.SortAsc(3);
			where.BillingNumber.SortAsc(4);

			return context.GetBroker<IExternalPractitionerBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override ExternalPractitionerData Export(ExternalPractitioner entity, IReadContext context)
		{
			var data = new ExternalPractitionerData
				{
					Deactivated = entity.Deactivated,
					FamilyName = entity.Name.FamilyName,
					GivenName = entity.Name.GivenName,
					MiddleName = entity.Name.MiddleName,
					LicenseNumber = entity.LicenseNumber,
					BillingNumber = entity.BillingNumber,
					IsVerified = entity.IsVerified,
					LastVerifiedTime = entity.LastVerifiedTime,
					LastEditedTime = entity.LastEditedTime,
					ContactPoints = CollectionUtils.Map(entity.ContactPoints,
						(ExternalPractitionerContactPoint cp) => new ExternalPractitionerContactPointData
							{
								Name = cp.Name,
								IsDefaultContactPoint = cp.IsDefaultContactPoint,
								PreferredResultCommunicationMode = cp.PreferredResultCommunicationMode.ToString(),
								InformationAuthority = cp.InformationAuthority == null ? null : cp.InformationAuthority.Code,
								Description = cp.Description,
								Addresses = CollectionUtils.Map(cp.Addresses, (Address a) => new AddressData(a)),
								TelephoneNumbers = CollectionUtils.Map(cp.TelephoneNumbers, (TelephoneNumber tn) => new TelephoneNumberData(tn)),
								EmailAddresses = CollectionUtils.Map(cp.EmailAddresses, (EmailAddress a) => new EmailAddressData(a))
							}),
					ExtendedProperties = ExtendedPropertyUtils.Copy(entity.ExtendedProperties)
				};

			return data;
		}

		protected override void Import(ExternalPractitionerData data, IUpdateContext context)
		{
			var prac = LoadExternalPractitioner(
				data.LicenseNumber,
				data.BillingNumber,
				context);

			var name = new PersonName(data.FamilyName, data.GivenName, data.MiddleName, null, null, null);
			if (prac == null)
			{
				// Creating a new practitioenr:  Import
				prac = new ExternalPractitioner(name, 
					data.LicenseNumber, 
					data.BillingNumber, 
					data.IsVerified, 
					data.LastVerifiedTime, 
					data.LastEditedTime, 
					new HashedSet<ExternalPractitionerContactPoint>(),
					new Dictionary<string, string>(),
					null);
				context.Lock(prac, DirtyState.New);
			}
			else
			{
				prac.Name = name;
				prac.MarkEdited();
			}

			prac.MarkDeactivated(data.Deactivated);

			if (data.ContactPoints != null)
			{
				foreach (var cpData in data.ContactPoints)
				{
					var cp = CollectionUtils.SelectFirst(prac.ContactPoints, p => p.Name == cpData.Name) ?? new ExternalPractitionerContactPoint(prac);
					UpdateExternalPractitionerContactPoint(cpData, cp, context);
				}
			}

			ExtendedPropertyUtils.Update(prac.ExtendedProperties, data.ExtendedProperties);
		}

		#endregion

		private static void UpdateExternalPractitionerContactPoint(ExternalPractitionerContactPointData data, ExternalPractitionerContactPoint cp, IUpdateContext context)
		{
			cp.Name = data.Name;
			cp.Description = data.Description;
			cp.IsDefaultContactPoint = data.IsDefaultContactPoint;
			cp.PreferredResultCommunicationMode = (ResultCommunicationMode) Enum.Parse(typeof(ResultCommunicationMode), data.PreferredResultCommunicationMode);
			cp.InformationAuthority = string.IsNullOrEmpty(data.InformationAuthority) ? null : context.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(data.InformationAuthority);

			if (data.TelephoneNumbers != null)
			{
				cp.TelephoneNumbers.Clear();
				foreach (var phoneDetail in data.TelephoneNumbers)
				{
					cp.TelephoneNumbers.Add(phoneDetail.CreateTelephoneNumber());
				}
			}

			if (data.Addresses != null)
			{
				cp.Addresses.Clear();
				foreach (var addressDetail in data.Addresses)
				{
					cp.Addresses.Add(addressDetail.CreateAddress());
				}
			}

			if (data.EmailAddresses != null)
			{
				cp.EmailAddresses.Clear();
				foreach (var addressDetail in data.EmailAddresses)
				{
					cp.EmailAddresses.Add(addressDetail.CreateEmailAddress());
				}
			}
		}

		private static ExternalPractitioner LoadExternalPractitioner(string licenseNumber, string billingNumber, IPersistenceContext context)
		{
			ExternalPractitioner prac = null;

			// if either license or billing number are supplied, check for an existing practitioner
			if (!string.IsNullOrEmpty(licenseNumber) || !string.IsNullOrEmpty(billingNumber))
			{
				var criteria = new ExternalPractitionerSearchCriteria();
				if(!string.IsNullOrEmpty(licenseNumber))
					criteria.LicenseNumber.EqualTo(licenseNumber);
				if (!string.IsNullOrEmpty(billingNumber))
					criteria.BillingNumber.EqualTo(billingNumber);

				var broker = context.GetBroker<IExternalPractitionerBroker>();
				prac = CollectionUtils.FirstElement(broker.Find(criteria));
			}

			return prac;
		}
	}
}
