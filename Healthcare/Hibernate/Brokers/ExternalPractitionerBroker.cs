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

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerBroker
	{
		#region IExternalPractitionerBroker Members

		public IList<ExternalPractitioner> GetMergeCandidates(ExternalPractitioner practitioner)
		{
			var criteria = GetMergeCandidatesCriteria(practitioner);
			return Find(criteria);
		}

		public int CountMergeCandidates(ExternalPractitioner practitioner)
		{
			var criteria = GetMergeCandidatesCriteria(practitioner);
			return (int) Count(criteria);
		}

		#endregion

		private static ExternalPractitionerSearchCriteria[] GetMergeCandidatesCriteria(ExternalPractitioner practitioner)
		{
			var criteria = new List<ExternalPractitionerSearchCriteria>();

			var baseCriteria = new ExternalPractitionerSearchCriteria();
			baseCriteria.NotEqualTo(practitioner);
			baseCriteria.Deactivated.EqualTo(false);

			var nameCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
			nameCriteria.Name.FamilyName.EqualTo(practitioner.Name.FamilyName);
			nameCriteria.Name.GivenName.EqualTo(practitioner.Name.GivenName);
			criteria.Add(nameCriteria);

			if (!string.IsNullOrEmpty(practitioner.LicenseNumber))
			{
				var licenseNumberCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
				licenseNumberCriteria.LicenseNumber.EqualTo(practitioner.LicenseNumber);
				criteria.Add(licenseNumberCriteria);
			}

			if (!string.IsNullOrEmpty(practitioner.BillingNumber))
			{
				var billingNumberCriteria = (ExternalPractitionerSearchCriteria)baseCriteria.Clone();
				billingNumberCriteria.BillingNumber.EqualTo(practitioner.BillingNumber);
				criteria.Add(billingNumberCriteria);
			}

			return criteria.ToArray();
		}
	}
}
