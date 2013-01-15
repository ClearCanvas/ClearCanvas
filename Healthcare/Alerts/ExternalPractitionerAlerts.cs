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
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Alerts
{
	public abstract class ExternalPractitionerAlerts
	{
		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class IncompleteDataAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "IncompleteDataAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var reasons = new List<string>();

				TestName(entity.Name, ref reasons);

				if (string.IsNullOrEmpty(entity.LicenseNumber))
					reasons.Add(SR.AlertExternalPractitionerLicenseNumberMissing);

				if (string.IsNullOrEmpty(entity.BillingNumber))
					reasons.Add(SR.AlertExternalPractitionerBillingNumberMissing);

				return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
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
		}

		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class IncompleteContactPointDataAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "IncompleteContactPointDataAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var reasons = new List<string>();

				if (entity.ContactPoints == null || entity.ContactPoints.Count == 0)
				{
					reasons.Add(SR.AlertContactPointsMissing);
				}
				else
				{
					TestContactPoints(entity.ContactPoints, ref reasons);
				}

				return reasons.Count > 0 ? new AlertNotification(this.Id, reasons) : null;
			}

			private static void TestContactPoints(IEnumerable<ExternalPractitionerContactPoint> contactPoints, ref List<string> reasons)
			{
				var hasMissingWorkAddress = false;
				var hasMissingWorkTelephone = false;
				var hasMissingWorkFax = false;

				CollectionUtils.ForEach(contactPoints,
					delegate(ExternalPractitionerContactPoint cp)
						{
							// Test for at least one contact point that does not have work address
							if (hasMissingWorkAddress == false && !TestWorkAddress(cp.Addresses))
								hasMissingWorkAddress = true;

							// Test for at least one contact point that does not have work telephone
							if (hasMissingWorkTelephone == false && !TestWorkTelephone(cp.TelephoneNumbers))
								hasMissingWorkTelephone = true;

							// Test for at least one contact point that does not have work fax
							if (hasMissingWorkFax == false && !TestWorkFax(cp.TelephoneNumbers))
								hasMissingWorkFax = true;
						});

				if (hasMissingWorkAddress)
					reasons.Add(SR.AlertWorkAddressMissing);

				if (hasMissingWorkTelephone)
					reasons.Add(SR.AlertWorkTelephoneNumberMissing);

				if (hasMissingWorkFax)
					reasons.Add(SR.AlertWorkFaxNumberMissing);
			}

			private static bool TestWorkAddress(ICollection<Address> addresses)
			{
				if (addresses == null || addresses.Count == 0)
					return false;

				return CollectionUtils.Contains(addresses, a => a.Type == AddressType.B);
			}

			private static bool TestWorkTelephone(ICollection<TelephoneNumber> phoneNumbers)
			{
				if (phoneNumbers == null || phoneNumbers.Count == 0)
					return false;

				return CollectionUtils.Contains(phoneNumbers, tn => tn.Equipment == TelephoneEquipment.PH && tn.Use == TelephoneUse.WPN);
			}

			private static bool TestWorkFax(ICollection<TelephoneNumber> phoneNumbers)
			{
				if (phoneNumbers == null || phoneNumbers.Count == 0)
					return false;

				return CollectionUtils.Contains(phoneNumbers, tn => tn.Equipment == TelephoneEquipment.FX && tn.Use == TelephoneUse.WPN);
			}
		}

		[ExtensionOf(typeof(ExternalPractitionerAlertExtensionPoint))]
		public class PossibleDuplicateAlert : ExternalPractitionerAlertBase
		{
			public override string Id
			{
				get { return "PossibleDuplicateAlert"; }
			}

			public override AlertNotification Test(ExternalPractitioner entity, IPersistenceContext context)
			{
				var broker = context.GetBroker<IExternalPractitionerBroker>();
				var count = broker.CountMergeCandidates(entity);
				return count > 0 ? new AlertNotification(this.Id, new string[] { }) : null;
			}
		}
	}
}
