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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// ExternalPractitionerContactPointEditorComponent class
	/// </summary>
	public class ExternalPractitionerContactPointEditorComponent : NavigatorComponentContainer
	{
		private readonly ExternalPractitionerContactPointDetail _contactPointDetail;

		private ExternalPractitionerContactPointDetailsEditorComponent _detailsEditor;
		private AddressesSummaryComponent _addressesSummary;
		private PhoneNumbersSummaryComponent _phoneNumbersSummary;
		private EmailAddressesSummaryComponent _emailAddressesSummary;

		private readonly List<EnumValueInfo> _addressTypeChoices;
		private readonly List<EnumValueInfo> _phoneTypeChoices;
		private readonly List<EnumValueInfo> _resultCommunicationModeChoices;
		private readonly List<EnumValueInfo> _informationAuthorityChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointEditorComponent(
			ExternalPractitionerContactPointDetail contactPoint,
			List<EnumValueInfo> addressTypeChoices,
			List<EnumValueInfo> phoneTypeChoices,
			List<EnumValueInfo> resultCommunicationModeChoices,
			List<EnumValueInfo> informationAuthorityChoices)
		{
			_contactPointDetail = contactPoint;
			_addressTypeChoices = addressTypeChoices;
			_phoneTypeChoices = phoneTypeChoices;
			_resultCommunicationModeChoices = resultCommunicationModeChoices;
			_informationAuthorityChoices = informationAuthorityChoices;
		}

		public override void Start()
		{
			this.Pages.Add(new NavigatorPage("NodeContactPoint", _detailsEditor = new ExternalPractitionerContactPointDetailsEditorComponent(_contactPointDetail, _resultCommunicationModeChoices, _informationAuthorityChoices)));
			this.Pages.Add(new NavigatorPage("NodeContactPoint/NodeAddresses", _addressesSummary = new AddressesSummaryComponent(_addressTypeChoices)));
			this.Pages.Add(new NavigatorPage("NodeContactPoint/NodePhoneNumbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(_phoneTypeChoices)));
			this.Pages.Add(new NavigatorPage("NodeContactPoint/NodeEmailAddresses", _emailAddressesSummary = new EmailAddressesSummaryComponent()));

			_addressesSummary.SetModifiedOnListChange = true;
			_phoneNumbersSummary.SetModifiedOnListChange = true;
			_emailAddressesSummary.SetModifiedOnListChange = true;

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_addressesSummary.Subject = _contactPointDetail.Addresses;
			_phoneNumbersSummary.Subject = _contactPointDetail.TelephoneNumbers;
			_emailAddressesSummary.Subject = _contactPointDetail.EmailAddresses;

			base.Start();
		}
	}
}
