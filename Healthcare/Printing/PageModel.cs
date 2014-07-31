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
using ClearCanvas.Enterprise.Core.Printing;

namespace ClearCanvas.Healthcare.Printing
{
	public abstract class PageModel : IPageModel
	{
		#region Facade classes

		/// <summary>
		/// Person name facade.
		/// </summary>
		public class NameFacade
		{
			private readonly PersonName _name;

			public NameFacade(PersonName name)
			{
				_name = name;
			}

			public string GivenName
			{
				get { return _name.GivenName; }
			}

			public string FamilyName
			{
				get { return _name.FamilyName; }
			}

			public string MiddleName
			{
				get { return _name.MiddleName; }
			}

			public override string ToString()
			{
				return _name.ToString();
			}
		}

		/// <summary>
		/// Patient identifier facade.
		/// </summary>
		public class IdentifierFacade
		{
			private readonly PatientIdentifier _identifier;

			public IdentifierFacade(PatientIdentifier identifier)
			{
				_identifier = identifier;
			}

			public string Id
			{
				get { return _identifier.Id; }
			}

			public override string ToString()
			{
				return _identifier.ToString();
			}
		}

		/// <summary>
		/// Healthcard facade.
		/// </summary>
		public class HealthcardFacade
		{
			private readonly HealthcardNumber _healthcard;

			public HealthcardFacade(HealthcardNumber healthcard)
			{
				_healthcard = healthcard;
			}

			public string Id
			{
				get { return _healthcard.Id; }
			}

			public string AssigningAuthority
			{

				get { return _healthcard.AssigningAuthority.Code; }
			}

			public override string ToString()
			{
				return string.IsNullOrEmpty(_healthcard.Id) ? string.Empty : _healthcard.ToString();
			}
		}

		/// <summary>
		/// Address facade.
		/// </summary>
		public class AddressFacade
		{
			private readonly Address _address;

			internal AddressFacade(Address address)
			{
				_address = address;
			}

			public string Line1
			{
				get { return string.IsNullOrEmpty(_address.Unit) ? _address.Street : string.Format("{0} - {1}", _address.Unit, _address.Street); }
			}

			public string Line2
			{
				get { return string.Format("{0} {1} {2}", _address.City, _address.Province, _address.PostalCode); }
			}

			public string Street
			{
				get { return _address.Street; }
			}

			public string Unit
			{
				get { return _address.Unit; }
			}

			public string City
			{
				get { return _address.City; }
			}

			public string Province
			{
				get { return _address.Province; }
			}

			public string PostalCode
			{
				get { return _address.PostalCode; }
			}

			public string Country
			{
				get { return _address.Country; }
			}

			public override string ToString()
			{
				return _address.ToString();
			}
		}

		/// <summary>
		/// Letterhead facade.
		/// </summary>
		public class LetterheadFacade
		{
			private readonly Letterhead _letterhead;
			private readonly AddressFacade _address;

			internal LetterheadFacade(string facilityCode)
			{
				_letterhead = LetterheadHelper.GetLetterheads(facilityCode);
				_address = new AddressFacade(_letterhead.Address);
			}

			public string FacilityTitle
			{
				get { return _letterhead.FacilityTitle; }
			}

			public string FacilitySubtitle
			{
				get { return _letterhead.FacilitySubtitle; }
			}

			public string LogoFile
			{
				get { return _letterhead.LogoFile; }
			}

			public AddressFacade Address
			{
				get { return _address; }
			}

			public string Phone
			{
				get { return _letterhead.Phone; }
			}

			public string Fax
			{
				get { return _letterhead.Fax; }
			}

			public string Email
			{
				get { return _letterhead.Email; }
			}
		}

		/// <summary>
		/// Patient facade.
		/// </summary>
		public class PatientFacade
		{
			private readonly PatientProfile _patientProfile;

			internal PatientFacade(PatientProfile patientProfile)
			{
				_patientProfile = patientProfile;
			}

			public NameFacade Name
			{
				get { return new NameFacade(_patientProfile.Name); }
			}

			public IdentifierFacade Mrn
			{
				get { return new IdentifierFacade(_patientProfile.Mrn); }
			}

			public HealthcardFacade Healthcard
			{
				get { return new HealthcardFacade(_patientProfile.Healthcard); }
			}

			public string BillingInformation
			{
				get { return _patientProfile.BillingInformation; }
			}

			public string DateOfBirth
			{
				get { return FormatDateOfBirth(_patientProfile.DateOfBirth); }
			}

			public override string ToString()
			{
				return string.Format("{0} ({1})", this.Name, this.Mrn);
			}

			private string FormatDateOfBirth(DateTime? dateOfBirth)
			{
				//todo: can we centralize formatting somewhere
				return dateOfBirth == null ? "" : dateOfBirth.Value.ToString("yyyy-MM-dd");
			}
		}

		#endregion

		private readonly Uri _templateUrl;

		protected PageModel(string templateUrl)
		{
			_templateUrl = new Uri(templateUrl);
		}

		/// <summary>
		/// Gets the URL of the template.
		/// </summary>
		public Uri TemplateUrl
		{
			get { return _templateUrl; }
		}

		/// <summary>
		/// Gets the set of variables accessible to the template.
		/// </summary>
		public abstract Dictionary<string, object> Variables { get; }
	}
}
