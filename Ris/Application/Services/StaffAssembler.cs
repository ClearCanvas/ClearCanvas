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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class StaffAssembler
	{
		public StaffSummary CreateStaffSummary(Staff staff, IPersistenceContext context)
		{
			if (staff == null)
				return null;

			return new StaffSummary(staff.GetRef(), staff.Id,
				EnumUtils.GetEnumValueInfo(staff.Type),
				new PersonNameAssembler().CreatePersonNameDetail(staff.Name),
				staff.Deactivated);
		}

		public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
		{
			PersonNameAssembler assembler = new PersonNameAssembler();
			StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
			EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
			TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
			AddressAssembler addressAssembler = new AddressAssembler();

			return new StaffDetail(
				staff.GetRef(),
				staff.Id,
				EnumUtils.GetEnumValueInfo(staff.Type),
				assembler.CreatePersonNameDetail(staff.Name),
				EnumUtils.GetEnumValueInfo(staff.Sex, context),
				staff.Title,
				staff.LicenseNumber,
				staff.BillingNumber,
				CollectionUtils.Map<TelephoneNumber, TelephoneDetail>(
					staff.TelephoneNumbers,
					delegate(TelephoneNumber tn) { return telephoneAssembler.CreateTelephoneDetail(tn, context); }),
				CollectionUtils.Map<Address, AddressDetail>(
					staff.Addresses,
					delegate(Address a) { return addressAssembler.CreateAddressDetail(a, context); }),
				CollectionUtils.Map<EmailAddress, EmailAddressDetail>(
					staff.EmailAddresses,
					delegate(EmailAddress ea) { return emailAssembler.CreateEmailAddressDetail(ea, context); }),
				CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
					staff.Groups,
					delegate(StaffGroup group) { return groupAssembler.CreateSummary(group); }),
				ExtendedPropertyUtils.Copy(staff.ExtendedProperties),
				staff.Deactivated,
				staff.UserName);
		}

		public void UpdateStaff(StaffDetail detail, Staff staff, bool updateElectiveGroups, bool updateNonElectiveGroups, IPersistenceContext context)
		{
			PersonNameAssembler assembler = new PersonNameAssembler();
			EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
			TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
			AddressAssembler addressAssembler = new AddressAssembler();

			staff.Id = detail.StaffId;
			staff.Type = EnumUtils.GetEnumValue<StaffTypeEnum>(detail.StaffType, context);
			assembler.UpdatePersonName(detail.Name, staff.Name);
			staff.Sex = EnumUtils.GetEnumValue<Sex>(detail.Sex);
			staff.Title = detail.Title;
			staff.LicenseNumber = detail.LicenseNumber;
			staff.BillingNumber = detail.BillingNumber;
			staff.Deactivated = detail.Deactivated;
			staff.UserName = detail.UserName;

			staff.TelephoneNumbers.Clear();
			if (detail.TelephoneNumbers != null)
			{
				foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
				{
					staff.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneNumber(phoneDetail));
				}
			}

			staff.Addresses.Clear();
			if (detail.Addresses != null)
			{
				foreach (AddressDetail addressDetail in detail.Addresses)
				{
					staff.Addresses.Add(addressAssembler.CreateAddress(addressDetail));
				}
			}

			staff.EmailAddresses.Clear();
			if (detail.EmailAddresses != null)
			{
				foreach (EmailAddressDetail emailAddressDetail in detail.EmailAddresses)
				{
					staff.EmailAddresses.Add(emailAssembler.CreateEmailAddress(emailAddressDetail));
				}
			}

			ExtendedPropertyUtils.Update(staff.ExtendedProperties, detail.ExtendedProperties);

			if (updateElectiveGroups)
			{
				// update elective groups
				UpdateStaffGroups(detail, staff,
					delegate(StaffGroupSummary summary) { return summary.IsElective; },
					delegate(StaffGroup group) { return group.Elective; },
					context);
			}

			if (updateNonElectiveGroups)
			{
				// update non-elective groups
				UpdateStaffGroups(detail, staff,
					delegate(StaffGroupSummary summary) { return !summary.IsElective; },
					delegate(StaffGroup group) { return !group.Elective; },
					context);
			}
		}

		private static void UpdateStaffGroups(StaffDetail detail, Staff staff, Predicate<StaffGroupSummary> p1, Predicate<StaffGroup> p2,
			IPersistenceContext context)
		{
			// create a helper to sync staff group membership
			CollectionSynchronizeHelper<StaffGroup, StaffGroupSummary> helper =
				new CollectionSynchronizeHelper<StaffGroup, StaffGroupSummary>(
					delegate(StaffGroup group, StaffGroupSummary summary)
					{
						return group.GetRef().Equals(summary.StaffGroupRef, true);
					},
					delegate(StaffGroupSummary groupSummary, ICollection<StaffGroup> groups)
					{
						StaffGroup group = context.Load<StaffGroup>(groupSummary.StaffGroupRef, EntityLoadFlags.Proxy);
						group.AddMember(staff);
					},
					delegate
					{
						// do nothing
					},
					delegate(StaffGroup group, ICollection<StaffGroup> groups)
					{
						group.RemoveMember(staff);
					}
				);

			helper.Synchronize(
				CollectionUtils.Select(staff.Groups, p2),
				CollectionUtils.Select(detail.Groups, p1));
		}
	}
}
