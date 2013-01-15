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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alerts
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    class IncompleteDemographicDataAlert : PatientProfileAlertBase
    {
		public override string Id
		{
			get { return "IncompleteDemographicDataAlert"; }
		}

		public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
			var reasons = new List<string>();

        	TestName(profile.Name, ref reasons);
			TestAddresses(profile.Addresses, ref reasons);
			TestTelephoneNumbers(profile.TelephoneNumbers, ref reasons);

			var settings = new AlertsSettings();
			if(settings.MissingHealthcardInfoTriggersIncompleteDemographicDataAlert)
			{
				TestHealthcard(profile.Healthcard, ref reasons);
			}

            if (reasons.Count > 0)
                return new AlertNotification(this.Id, reasons);

            return null;
        }

		private static void TestName(PersonName name, ref List<string> reasons)
		{
			if (name == null)
			{
				reasons.Add(SR.AlertFamilyNameMissing);
				return;
			}

			if (string.IsNullOrEmpty(name.FamilyName))
				reasons.Add(SR.AlertFamilyNameMissing);

			if (string.IsNullOrEmpty(name.GivenName))
				reasons.Add(SR.AlertGivenNameMissing);
		}

		private static void TestHealthcard(HealthcardNumber healthcard, ref List<string> reasons)
		{
			if (healthcard == null)
			{
				reasons.Add(SR.AlertHealthcardMissing);
				return;
			}

			if (string.IsNullOrEmpty(healthcard.Id))
				reasons.Add(SR.AlertHealthcardIdMissing);

			if (healthcard.AssigningAuthority == null)
				reasons.Add(SR.AlertHealthcardAssigningAuthorityMissing);
		}

		private static void TestAddresses(ICollection<Address> addresses, ref List<string> reasons)
		{
			if (addresses == null || addresses.Count == 0)
			{
				reasons.Add(SR.AlertResidentialAddressMissing);
				return;
			}

			bool hasResidentialAddress = CollectionUtils.Contains(addresses,
				delegate(Address a) { return a.Type == AddressType.R; });

			if (!hasResidentialAddress)
				reasons.Add(SR.AlertResidentialAddressMissing);
		}

		private static void TestTelephoneNumbers(ICollection<TelephoneNumber> phoneNumbers, ref List<string> reasons)
		{
			if (phoneNumbers == null || phoneNumbers.Count == 0)
			{
				reasons.Add(SR.AlertResidentialTelephoneNumberMissing);
				return;
			}

			bool hasResidentialPhoneNumber = CollectionUtils.Contains(phoneNumbers,
				delegate(TelephoneNumber tn) { return tn.Equipment == TelephoneEquipment.PH && tn.Use == TelephoneUse.PRN; });

			if (!hasResidentialPhoneNumber)
				reasons.Add(SR.AlertResidentialTelephoneNumberMissing);
		}
	}
}
