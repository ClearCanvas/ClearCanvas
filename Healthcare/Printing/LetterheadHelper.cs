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
using System.Linq;
using System.Xml;

namespace ClearCanvas.Healthcare.Printing
{
	class Letterhead
	{
		public Letterhead()
		{
			this.Address = new Address();
		}

		public string FacilityCode { get; set; }
		public string FacilityTitle { get; set; }
		public string FacilitySubtitle { get; set; }
		public string LogoFile { get; set; }
		public string Fax { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public Address Address { get; set; }
	}

	internal class LetterheadHelper
	{
		public static Letterhead GetLetterheads(string facilityCode)
		{
			var settings = new LetterheadSettings();

			var facilityLetterhead = new LetterheadHelper(settings.MultiFacilityLetterheadsXml).GetLetterhead(facilityCode);
			if (facilityLetterhead != null)
				return facilityLetterhead;

			return GetDefaultLetterhead();
		}

		public static Letterhead GetDefaultLetterhead()
		{
			var settings = new LetterheadSettings();
			return new Letterhead
			{
				FacilityTitle = settings.LetterheadFacilityTitle,
				FacilitySubtitle = settings.LetterheadFacilitySubtitle,
				LogoFile = settings.LetterheadLogoFile,
				Phone = settings.LetterheadPhone,
				Fax = settings.LetterheadFax,
				Email = settings.LetterheadEmail,
				Address = new Address(
					settings.LetterheadAddressStreet,
					settings.LetterheadAddressUnit,
					settings.LetterheadAddressCity,
					settings.LetterheadAddressProvince,
					settings.LetterheadAddressPostalCode,
					"", AddressType.B, null)
			};			
		}

		private readonly XmlDocument _xml;
		private readonly List<Letterhead> _letterheads; 
		private bool _parsed;

		public LetterheadHelper(XmlDocument xml)
		{
			_xml = xml;
			_letterheads = new List<Letterhead>();
		}

		public Letterhead GetLetterhead(string facilityCode)
		{
			ParseOnce();
			return _letterheads.SingleOrDefault(lh => Equals(lh.FacilityCode, facilityCode));
		}

		private void ParseOnce()
		{
			if (_parsed)
				return;

			var letterheadNodes = _xml.SelectNodes("letterheads/letterhead");
			if (letterheadNodes == null)
				return;

			foreach (var node in letterheadNodes.Cast<XmlElement>())
			{
				var lh = GetDefaultLetterhead();

				var facilityNode = node.GetElementsByTagName("facility").Cast<XmlElement>().FirstOrDefault();
				if (facilityNode != null)
				{
					lh.FacilityCode = facilityNode.GetAttribute("code");

					var attribute = facilityNode.GetAttribute("title");
					if (!string.IsNullOrEmpty(attribute))
						lh.FacilityTitle = attribute;

					attribute = facilityNode.GetAttribute("subtitle");
					if (!string.IsNullOrEmpty(attribute))
						lh.FacilitySubtitle = attribute;

					attribute = facilityNode.GetAttribute("logo");
					if (!string.IsNullOrEmpty(attribute))
						lh.LogoFile = attribute;
				}

				var phoneNode = node.GetElementsByTagName("phone").Cast<XmlElement>().FirstOrDefault();
				if (phoneNode != null && !string.IsNullOrEmpty(phoneNode.InnerText))
					lh.Phone = phoneNode.InnerText;

				var faxNode = node.GetElementsByTagName("fax").Cast<XmlElement>().FirstOrDefault();
				if (faxNode != null && !string.IsNullOrEmpty(faxNode.InnerText))
					lh.Fax = faxNode.InnerText;

				var emailNode = node.GetElementsByTagName("email").Cast<XmlElement>().FirstOrDefault();
				if (emailNode != null && !string.IsNullOrEmpty(emailNode.InnerText))
					lh.Email = emailNode.InnerText;

				var addressNode = node.GetElementsByTagName("address").Cast<XmlElement>().FirstOrDefault();
				if (addressNode != null)
				{
					var attribute = addressNode.GetAttribute("unit");
					if (!string.IsNullOrEmpty(attribute))
						lh.Address.Unit = attribute;

					attribute = addressNode.GetAttribute("street");
					if (!string.IsNullOrEmpty(attribute))
						lh.Address.Street = attribute;

					attribute = addressNode.GetAttribute("city");
					if (!string.IsNullOrEmpty(attribute))
						lh.Address.City = attribute;

					attribute = addressNode.GetAttribute("province");
					if (!string.IsNullOrEmpty(attribute))
						lh.Address.Province = attribute;

					attribute = addressNode.GetAttribute("postalCode");
					if (!string.IsNullOrEmpty(attribute))
						lh.Address.PostalCode = attribute;
				}

				_letterheads.Add(lh);
			}

			_parsed = true;
		}
	}
}
