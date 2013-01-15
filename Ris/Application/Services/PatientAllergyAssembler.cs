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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class PatientAllergyAssembler
	{
		class PatientAllergySynchronizeHelper : CollectionSynchronizeHelper<Allergy, PatientAllergyDetail>
		{
			private readonly PatientAllergyAssembler _assembler;
			private readonly IPersistenceContext _context;

			public PatientAllergySynchronizeHelper(PatientAllergyAssembler assembler, IPersistenceContext context)
				: base(true, true)
			{
				_assembler = assembler;
				_context = context;
			}

			protected override bool CompareItems(Allergy domainItem, PatientAllergyDetail sourceItem)
			{
				return _assembler.CompareAllergy(domainItem, sourceItem);
			}

			protected override void AddItem(PatientAllergyDetail sourceItem, ICollection<Allergy> domainList)
			{
				var allergy = _assembler.CreateAllergy(sourceItem, _context);
				domainList.Add(allergy);
			}

			protected override void UpdateItem(Allergy domainItem, PatientAllergyDetail sourceItem, ICollection<Allergy> domainList)
			{
				_assembler.UpdateAllergy(domainItem, sourceItem, _context);
			}

			protected override void RemoveItem(Allergy domainItem, ICollection<Allergy> domainList)
			{
				domainList.Remove(domainItem);
			}
		}

		public void Synchronize(IList<Allergy> domainList, IList<PatientAllergyDetail> sourceList, IPersistenceContext context)
		{
			var synchronizer = new PatientAllergySynchronizeHelper(this, context);
			synchronizer.Synchronize(domainList, sourceList);
		}

		public PatientAllergyDetail CreateAllergyDetail(Allergy allergy)
		{
			var nameAssembler = new PersonNameAssembler();

			return new PatientAllergyDetail(
				EnumUtils.GetEnumValueInfo(allergy.AllergenType),
				allergy.AllergenDescription,
				EnumUtils.GetEnumValueInfo(allergy.Severity),
				allergy.Reaction,
				EnumUtils.GetEnumValueInfo(allergy.SensitivityType),
				allergy.OnsetTime,
				allergy.ReportedTime,
				nameAssembler.CreatePersonNameDetail(allergy.Reporter),
				EnumUtils.GetEnumValueInfo(allergy.ReporterRelationshipType));
		}

		public Allergy CreateAllergy(PatientAllergyDetail detail, IPersistenceContext context)
		{
			var allergy = new Allergy();
			UpdateAllergy(allergy, detail, context);
			return allergy;
		}

		public void UpdateAllergy(Allergy allergy, PatientAllergyDetail source, IPersistenceContext context)
		{
			var nameAssembler = new PersonNameAssembler();

			allergy.AllergenType = EnumUtils.GetEnumValue<AllergyAllergenTypeEnum>(source.AllergenType, context);
			allergy.AllergenDescription = source.AllergenDescription;
			allergy.Severity = EnumUtils.GetEnumValue<AllergySeverityEnum>(source.Severity, context);
			allergy.Reaction = source.Reaction;
			allergy.SensitivityType = EnumUtils.GetEnumValue<AllergySensitivityTypeEnum>(source.SensitivityType, context);
			allergy.OnsetTime = source.OnsetTime;
			allergy.ReportedTime = source.ReportedTime;
			nameAssembler.UpdatePersonName(source.ReporterName, allergy.Reporter);
			allergy.ReporterRelationshipType = EnumUtils.GetEnumValue<PersonRelationshipTypeEnum>(source.ReporterRelationshipType, context);
		}

		public bool CompareAllergy(Allergy allergy, PatientAllergyDetail detail)
		{
			// Not sure how can we determine two allergies are equivalent... so the only way is to compare every field.
			return Equals(allergy.AllergenType.Code, detail.AllergenType)
				&& Equals(allergy.AllergenDescription, detail.AllergenDescription)
				&& Equals(allergy.Severity.Code, detail.Severity)
				&& Equals(allergy.Reaction, detail.Reaction)
				&& Equals(allergy.SensitivityType.Code, detail.SensitivityType.Code)
				&& Equals(allergy.OnsetTime, detail.OnsetTime)
				&& Equals(allergy.ReportedTime, detail.ReportedTime)
				&& Equals(allergy.Reporter, detail.ReporterName)
				&& Equals(allergy.ReporterRelationshipType.Code, detail.ReporterRelationshipType.Code);
		}
	}
}
