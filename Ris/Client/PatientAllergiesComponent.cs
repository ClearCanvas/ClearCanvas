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
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PatientAllergiesComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class AllergyContext : DataContractBase
		{
			public AllergyContext()
			{
				this.Allergies = new List<PatientAllergyDetail>();
				this.AllergenTypeChoices = new List<EnumValueInfo>();
				this.SeverityChoices = new List<EnumValueInfo>();
				this.SensitivityTypeChoices = new List<EnumValueInfo>();
				this.PersonRelationshipTypeChoices = new List<EnumValueInfo>();
			}

			[DataMember]
			public List<PatientAllergyDetail> Allergies;

			[DataMember]
			public List<EnumValueInfo> AllergenTypeChoices;

			[DataMember]
			public List<EnumValueInfo> SeverityChoices;

			[DataMember]
			public List<EnumValueInfo> SensitivityTypeChoices;

			[DataMember]
			public List<EnumValueInfo> PersonRelationshipTypeChoices;
		}

		private readonly bool _readOnly;
		private readonly AllergyContext _context;

		/// <summary>
		/// Constructor for readonly allergies information.
		/// </summary>
		public PatientAllergiesComponent()
		{
			_readOnly = true;
			_context = new AllergyContext();
		}

		/// <summary>
		/// Constructor for editing allergies.
		/// </summary>
		/// <param name="allergenTypeChoices"></param>
		/// <param name="severityChoices"></param>
		/// <param name="sensitivityTypeChoices"></param>
		/// <param name="personRelationshipTypeChoices"></param>
		public PatientAllergiesComponent(
			List<EnumValueInfo> allergenTypeChoices,
			List<EnumValueInfo> severityChoices,
			List<EnumValueInfo> sensitivityTypeChoices,
			List<EnumValueInfo> personRelationshipTypeChoices)
		{
			_readOnly = false;
			_context = new AllergyContext
				{
					AllergenTypeChoices = allergenTypeChoices,
					SeverityChoices = severityChoices,
					SensitivityTypeChoices = sensitivityTypeChoices,
					PersonRelationshipTypeChoices = personRelationshipTypeChoices
				};
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.BiographyAllergyDetailPageUrl);
			base.Start();
		}

		public List<PatientAllergyDetail> Allergies
		{
			get { return _context.Allergies; }
			set
			{
				_context.Allergies = value;
				NotifyAllPropertiesChanged();
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected override string GetTag(string tag)
		{
			if (string.Equals("ReadOnly", tag))
			{
				return _readOnly ? "true" : "false";
			}

			return base.GetTag(tag);
		}

		protected override void SetTag(string tag, string data)
		{
			if (string.Equals("Allergies", tag))
			{
				this.Allergies.Clear();
				this.Allergies.AddRange(JsmlSerializer.Deserialize<List<PatientAllergyDetail>>(data));
				return;
			}

			base.SetTag(tag, data);
		}
	}
}
