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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Staff")]
	public class StaffImex : XmlEntityImex<Staff, StaffImex.StaffData>
	{
		[DataContract]
		public class StaffData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string FamilyName;

			[DataMember]
			public string GivenName;

			[DataMember]
			public string MiddleName;

			[DataMember]
			public string Sex;

			[DataMember]
			public string Title;

			[DataMember]
			public string StaffType;

			[DataMember]
			public string UserName;

			[DataMember]
			public string LicenseNumber;

			[DataMember]
			public string BillingNumber;

			[DataMember]
			public List<TelephoneNumberData> TelephoneNumbers;

			[DataMember]
			public List<AddressData> Addresses;

			[DataMember]
			public List<EmailAddressData> EmailAddresses;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;
		}


		#region Overrides

		protected override IList<Staff> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			StaffSearchCriteria where = new StaffSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IStaffBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override StaffData Export(Staff entity, IReadContext context)
		{
			StaffData data = new StaffData();
			data.Deactivated = entity.Deactivated;
			data.Id = entity.Id;
			data.StaffType = entity.Type.Code;
			data.Title = entity.Title;
			data.FamilyName = entity.Name.FamilyName;
			data.GivenName = entity.Name.GivenName;
			data.MiddleName = entity.Name.MiddleName;
			data.Sex = entity.Sex.ToString();
			data.UserName = entity.UserName;
			data.LicenseNumber = entity.LicenseNumber;
			data.BillingNumber = entity.BillingNumber;

			data.TelephoneNumbers = CollectionUtils.Map<TelephoneNumber, TelephoneNumberData>(entity.TelephoneNumbers,
				delegate(TelephoneNumber tn) { return new TelephoneNumberData(tn); });
			data.Addresses = CollectionUtils.Map<Address, AddressData>(entity.Addresses,
				delegate(Address a) { return new AddressData(a); });
			data.EmailAddresses = CollectionUtils.Map<EmailAddress, EmailAddressData>(entity.EmailAddresses,
				delegate(EmailAddress a) { return new EmailAddressData(a); });

			data.ExtendedProperties = ExtendedPropertyUtils.Copy(entity.ExtendedProperties);

			return data;
		}

		protected override void Import(StaffData data, IUpdateContext context)
		{
			Staff staff = GetStaff(data.Id, context);
			staff.Deactivated = data.Deactivated;
			staff.Type = context.GetBroker<IEnumBroker>().Find<StaffTypeEnum>(data.StaffType);
			staff.Title = data.Title;
			staff.Name.FamilyName = data.FamilyName;
			staff.Name.GivenName = data.GivenName;
			staff.Name.MiddleName = data.MiddleName;

			staff.Sex = string.IsNullOrEmpty(data.Sex) == false
				? (Sex)Enum.Parse(typeof(Sex), data.Sex)
				: Sex.U;

			staff.UserName = data.UserName;
			staff.LicenseNumber = data.LicenseNumber;
			staff.BillingNumber = data.BillingNumber;

			if (data.TelephoneNumbers != null)
			{
				staff.TelephoneNumbers.Clear();
				foreach (TelephoneNumberData phoneDetail in data.TelephoneNumbers)
				{
					staff.TelephoneNumbers.Add(phoneDetail.CreateTelephoneNumber());
				}
			}

			if (data.Addresses != null)
			{
				staff.Addresses.Clear();
				foreach (AddressData address in data.Addresses)
				{
					staff.Addresses.Add(address.CreateAddress());
				}
			}

			if (data.EmailAddresses != null)
			{
				staff.EmailAddresses.Clear();
				foreach (EmailAddressData addressDetail in data.EmailAddresses)
				{
					staff.EmailAddresses.Add(addressDetail.CreateEmailAddress());
				}
			}

			ExtendedPropertyUtils.Update(staff.ExtendedProperties, data.ExtendedProperties);
		}

		#endregion


		private Staff GetStaff(string id, IPersistenceContext context)
		{
			Staff staff = null;

			try
			{
				StaffSearchCriteria criteria = new StaffSearchCriteria();
				criteria.Id.EqualTo(id);

				IStaffBroker broker = context.GetBroker<IStaffBroker>();
				staff = broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				staff = new Staff();

				// need to populate required fields before we can lock (use dummy values)
				staff.Id = id;
				staff.Name.FamilyName = "";
				staff.Name.GivenName = "";
				staff.Sex = Sex.U;

				context.Lock(staff, DirtyState.New);
			}
			return staff;
		}
	}
}
