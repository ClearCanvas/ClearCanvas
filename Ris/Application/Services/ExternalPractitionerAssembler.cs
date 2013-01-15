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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ExternalPractitionerAssembler
	{
		public ExternalPractitionerSummary CreateExternalPractitionerSummary(ExternalPractitioner prac, IPersistenceContext context)
		{
			var summary = new ExternalPractitionerSummary(
				prac.GetRef(),
				new PersonNameAssembler().CreatePersonNameDetail(prac.Name),
				prac.LicenseNumber,
				prac.BillingNumber,
				prac.IsVerified,
				prac.LastVerifiedTime,
				prac.LastEditedTime,
				prac.IsMerged,
				prac.Deactivated);

			return summary;
		}

		public ExternalPractitionerDetail CreateExternalPractitionerDetail(ExternalPractitioner prac, IPersistenceContext context)
		{
			var assembler = new PersonNameAssembler();

			var sortedContactPoints = CollectionUtils.Sort(prac.ContactPoints, (x, y) =>
				{
					if (ReferenceEquals(x, y)) return 0;
					if (x.IsDefaultContactPoint) return -1;
					if (y.IsDefaultContactPoint) return 1;
					return string.Compare(x.Name, y.Name);
				});

			var contactPointDetails = CollectionUtils.Map(
				sortedContactPoints,
				(ExternalPractitionerContactPoint cp) => CreateExternalPractitionerContactPointDetail(cp, context));

			var detail = new ExternalPractitionerDetail(
				prac.GetRef(),
				assembler.CreatePersonNameDetail(prac.Name),
				prac.LicenseNumber,
				prac.BillingNumber,
				prac.IsVerified,
				prac.LastVerifiedTime,
				prac.LastEditedTime,
				contactPointDetails,
				ExtendedPropertyUtils.Copy(prac.ExtendedProperties),
				CreateExternalPractitionerSummary(prac.GetUltimateMergeDestination(), context),
				prac.IsMerged,
				prac.Deactivated);

			return detail;
		}

		public void UpdateExternalPractitioner(ExternalPractitionerDetail detail, ExternalPractitioner prac, IPersistenceContext context)
		{
			// validate that only one contact point is specified as default
			var defaultPoints = CollectionUtils.Select(detail.ContactPoints, cp => cp.IsDefaultContactPoint);
			if(defaultPoints.Count > 1)
				throw new RequestValidationException(SR.ExceptionOneDefaultContactPoint);

			var assembler = new PersonNameAssembler();
			assembler.UpdatePersonName(detail.Name, prac.Name);

			prac.LicenseNumber = detail.LicenseNumber;
			prac.BillingNumber = detail.BillingNumber;
			prac.MarkDeactivated(detail.Deactivated);

			// update contact points collection
			var syncHelper = new CollectionSynchronizeHelper<ExternalPractitionerContactPoint, ExternalPractitionerContactPointDetail>(
					delegate (ExternalPractitionerContactPoint cp, ExternalPractitionerContactPointDetail cpDetail)
					{
						// ignore version in this comparison - deal with this issue in the update delegate
						return cp.GetRef().Equals(cpDetail.ContactPointRef, true);
					},
					delegate (ExternalPractitionerContactPointDetail cpDetail, ICollection<ExternalPractitionerContactPoint> cps)
					{
						// create a new contact point
						var cp = new ExternalPractitionerContactPoint(prac);
						UpdateExternalPractitionerContactPoint(cpDetail, cp, context);
						cps.Add(cp);
					},
					(cp, cpDetail, cps) => UpdateExternalPractitionerContactPoint(cpDetail, cp, context),
					(cp, cps) => cps.Remove(cp));

			syncHelper.Synchronize(prac.ContactPoints, detail.ContactPoints);

			ExtendedPropertyUtils.Update(prac.ExtendedProperties, detail.ExtendedProperties);
		}

		public ExternalPractitionerContactPointSummary CreateExternalPractitionerContactPointSummary(ExternalPractitionerContactPoint contactPoint)
		{
			return new ExternalPractitionerContactPointSummary(contactPoint.GetRef(),
				contactPoint.Name,
				contactPoint.Description,
				contactPoint.IsDefaultContactPoint,
				contactPoint.IsMerged,
				contactPoint.Deactivated);
		}

		public ExternalPractitionerContactPointDetail CreateExternalPractitionerContactPointDetail(ExternalPractitionerContactPoint contactPoint,
			IPersistenceContext context)
		{
			var telephoneNumberAssembler = new TelephoneNumberAssembler();
			var addressAssembler = new AddressAssembler();
			var emailAddressAssembler = new EmailAddressAssembler();

			var currentPhone = contactPoint.CurrentPhoneNumber;
			var currentFax = contactPoint.CurrentFaxNumber;
			var currentAddress = contactPoint.CurrentAddress;
			var currentEmailAddress = contactPoint.CurrentEmailAddress;

			return new ExternalPractitionerContactPointDetail(
				contactPoint.GetRef(),
				contactPoint.Name,
				contactPoint.Description,
				contactPoint.IsDefaultContactPoint,
				EnumUtils.GetEnumValueInfo(contactPoint.PreferredResultCommunicationMode, context),
				EnumUtils.GetEnumValueInfo(contactPoint.InformationAuthority),
				CollectionUtils.Map(contactPoint.TelephoneNumbers, (TelephoneNumber phone) => telephoneNumberAssembler.CreateTelephoneDetail(phone, context)),
				CollectionUtils.Map(contactPoint.Addresses, (Address address) => addressAssembler.CreateAddressDetail(address, context)),
				CollectionUtils.Map(contactPoint.EmailAddresses, (EmailAddress emailAddress) => emailAddressAssembler.CreateEmailAddressDetail(emailAddress, context)),
				currentPhone == null ? null : telephoneNumberAssembler.CreateTelephoneDetail(currentPhone, context),
				currentFax == null ? null : telephoneNumberAssembler.CreateTelephoneDetail(currentFax, context),
				currentAddress == null ? null : addressAssembler.CreateAddressDetail(currentAddress, context),
				currentEmailAddress == null ? null : emailAddressAssembler.CreateEmailAddressDetail(currentEmailAddress, context),
				CreateExternalPractitionerContactPointSummary(contactPoint.GetUltimateMergeDestination()),
				contactPoint.IsMerged,
				contactPoint.Deactivated);
		}

		public void UpdateExternalPractitionerContactPoint(ExternalPractitionerContactPointDetail detail, ExternalPractitionerContactPoint contactPoint, IPersistenceContext context)
		{
			contactPoint.Name = detail.Name;
			contactPoint.Description = detail.Description;
			contactPoint.IsDefaultContactPoint = detail.IsDefaultContactPoint;
			contactPoint.PreferredResultCommunicationMode = EnumUtils.GetEnumValue<ResultCommunicationMode>(detail.PreferredResultCommunicationMode);
			contactPoint.InformationAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.InformationAuthority, context);
			contactPoint.MarkDeactivated(detail.Deactivated);

			var phoneAssembler = new TelephoneNumberAssembler();
			var addressAssembler = new AddressAssembler();
			var emailAddressAssembler = new EmailAddressAssembler();

			contactPoint.TelephoneNumbers.Clear();
			if (detail.TelephoneNumbers != null)
			{
				foreach (var phoneDetail in detail.TelephoneNumbers)
				{
					contactPoint.TelephoneNumbers.Add(phoneAssembler.CreateTelephoneNumber(phoneDetail));
				}
			}

			contactPoint.Addresses.Clear();
			if (detail.Addresses != null)
			{
				foreach (var addressDetail in detail.Addresses)
				{
					contactPoint.Addresses.Add(addressAssembler.CreateAddress(addressDetail));
				}
			}

			contactPoint.EmailAddresses.Clear();
			if (detail.EmailAddresses != null)
			{
				foreach (var emailAddressDetail in detail.EmailAddresses)
				{
					contactPoint.EmailAddresses.Add(emailAddressAssembler.CreateEmailAddress(emailAddressDetail));
				}
			}
		}
	}
}
