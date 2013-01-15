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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Patient Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public class PatientModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientModuleIod"/> class.
		/// </summary>	
		public PatientModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientModuleIod"/> class.
		/// </summary>
		public PatientModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.BreedRegistrationSequence = null;
			this.DeIdentificationMethod = null;
			this.DeIdentificationMethodCodeSequence = null;
			this.EthnicGroup = null;
			this.IssuerOfPatientId = null;
			this.OtherPatientIds = null;
			this.OtherPatientIdsSequence = null;
			this.OtherPatientNames = null;
			this.PatientBreedCodeSequence = null;
			this.PatientBreedDescription = null;
			this.PatientComments = null;
			this.PatientId = null;
			this.PatientIdentityRemoved = PatientIdentityRemoved.Unknown;
			this.PatientsBirthDateTime = null;
			this.PatientsName = null;
			this.PatientSpeciesCodeSequence = null;
			this.PatientSpeciesDescription = null;
			this.PatientsSex = PatientsSex.None;
			this.ReferencedPatientSequence = null;
			this.ResponsibleOrganization = null;
			this.ResponsiblePerson = null;
			this.ResponsiblePersonRole = ResponsiblePersonRole.None;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (this.BreedRegistrationSequence == null
			    && string.IsNullOrEmpty(this.DeIdentificationMethod)
			    && this.DeIdentificationMethodCodeSequence == null
			    && string.IsNullOrEmpty(this.EthnicGroup)
			    && string.IsNullOrEmpty(this.IssuerOfPatientId)
			    && string.IsNullOrEmpty(this.OtherPatientIds)
			    && this.OtherPatientIdsSequence == null
			    && string.IsNullOrEmpty(this.OtherPatientNames)
			    && this.PatientBreedCodeSequence == null
			    && string.IsNullOrEmpty(this.PatientBreedDescription)
			    && string.IsNullOrEmpty(this.PatientComments)
			    && string.IsNullOrEmpty(this.PatientId)
			    && this.PatientIdentityRemoved == PatientIdentityRemoved.Unknown
			    && !this.PatientsBirthDateTime.HasValue
			    && string.IsNullOrEmpty(this.PatientsName)
			    && this.PatientSpeciesCodeSequence == null
			    && string.IsNullOrEmpty(this.PatientSpeciesDescription)
			    && this.PatientsSex == PatientsSex.None
			    && this.ReferencedPatientSequence == null
			    && string.IsNullOrEmpty(this.ResponsibleOrganization)
			    && string.IsNullOrEmpty(this.ResponsiblePerson)
			    && this.ResponsiblePersonRole == ResponsiblePersonRole.None)
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of PatientsName in the underlying collection. Type 2.
		/// </summary>
		public string PatientsName
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientsName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientsName].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientsName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientId in the underlying collection. Type 2.
		/// </summary>
		public string PatientId
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientId].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of IssuerOfPatientId in the underlying collection. Type 3.
		/// </summary>
		public string IssuerOfPatientId
		{
			get { return base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.IssuerOfPatientId] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsBirthDate and PatientsBirthTime in the underlying collection.  Type 2.
		/// </summary>
		public DateTime? PatientsBirthDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.PatientsBirthDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.PatientsBirthTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.PatientsBirthDate].SetNullValue();
					base.DicomAttributeProvider[DicomTags.PatientsBirthTime].SetNullValue();
					return;
				}
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.PatientsBirthDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.PatientsBirthTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsSex in the underlying collection. Type 2.
		/// </summary>
		public PatientsSex PatientsSex
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.PatientsSex].GetString(0, string.Empty), PatientsSex.None); }
			set
			{
				if (value == PatientsSex.None)
				{
					base.DicomAttributeProvider[DicomTags.PatientsSex].SetNullValue();
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PatientsSex], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedPatientSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedPatientSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ReferencedPatientSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedPatientSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				ISopInstanceReferenceMacro sequenceType = new SopInstanceReferenceMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of OtherPatientIds in the underlying collection. Type 3.
		/// </summary>
		public string OtherPatientIds
		{
			get { return base.DicomAttributeProvider[DicomTags.OtherPatientIds].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OtherPatientIds] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OtherPatientIds].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OtherPatientIdsSequence in the underlying collection. Type 3.
		/// </summary>
		public OtherPatientIdsSequence[] OtherPatientIdsSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.OtherPatientIdsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				OtherPatientIdsSequence[] result = new OtherPatientIdsSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new OtherPatientIdsSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.OtherPatientIdsSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.OtherPatientIdsSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of OtherPatientNames in the underlying collection. Type 3.
		/// </summary>
		public string OtherPatientNames
		{
			get { return base.DicomAttributeProvider[DicomTags.OtherPatientNames].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OtherPatientNames] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OtherPatientNames].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of EthnicGroup in the underlying collection. Type 3.
		/// </summary>
		public string EthnicGroup
		{
			get { return base.DicomAttributeProvider[DicomTags.EthnicGroup].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.EthnicGroup] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.EthnicGroup].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientComments in the underlying collection. Type 3.
		/// </summary>
		public string PatientComments
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientComments].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientComments] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientComments].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientSpeciesDescription in the underlying collection. Type 1C.
		/// </summary>
		public string PatientSpeciesDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientSpeciesDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientSpeciesDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientSpeciesDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientSpeciesCodeSequence in the underlying collection. Type 1C.
		/// </summary>
		public Species PatientSpeciesCodeSequence
		{
			get
			{

				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PatientSpeciesCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				var dicomSequenceItem = ((DicomSequenceItem[])dicomAttribute.Values)[0];
				return new Species(new CodeSequenceMacro(dicomSequenceItem));
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.PatientSpeciesCodeSequence] = null;
					return;
				}

				var dicomAttribute = base.DicomAttributeProvider[DicomTags.PatientSpeciesCodeSequence];

				var sequenceItem = new CodeSequenceMacro();
				value.WriteToCodeSequence(sequenceItem);

				dicomAttribute.Values = new DicomSequenceItem[] { sequenceItem.DicomSequenceItem };
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientBreedDescription in the underlying collection. Type 2C.
		/// </summary>
		public string PatientBreedDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientBreedDescription].GetString(0, string.Empty); }
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.PatientBreedDescription].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientBreedDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientBreedCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public Breed[] PatientBreedCodeSequence
		{
			get
			{

				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PatientBreedCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				var results = new Breed[dicomAttribute.Count];
				var sequenceItems = (DicomSequenceItem[])dicomAttribute.Values;
				for (var n = 0; n < sequenceItems.Length; n++)
				{
					results[n] = new Breed(new CodeSequenceMacro(sequenceItems[n]));
				}

				return results;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.PatientBreedCodeSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (var n = 0; n < value.Length; n++)
				{
					var codeSequence = new CodeSequenceMacro();
					value[n].WriteToCodeSequence(codeSequence);
					result[n] = codeSequence.DicomSequenceItem;
				}

				base.DicomAttributeProvider[DicomTags.PatientBreedCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of BreedRegistrationSequence in the underlying collection. Type 3.
		/// </summary>
		public BreedRegistrationSequence[] BreedRegistrationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.BreedRegistrationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				BreedRegistrationSequence[] result = new BreedRegistrationSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new BreedRegistrationSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.BreedRegistrationSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.BreedRegistrationSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of ResponsiblePerson in the underlying collection. Type 2C.
		/// </summary>
		public string ResponsiblePerson
		{
			get { return base.DicomAttributeProvider[DicomTags.ResponsiblePerson].GetString(0, string.Empty); }
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ResponsiblePerson] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ResponsiblePerson].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ResponsiblePersonRole in the underlying collection. Type 1C.
		/// </summary>
		public ResponsiblePersonRole ResponsiblePersonRole
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.ResponsiblePersonRole].GetString(0, string.Empty), ResponsiblePersonRole.None); }
			set
			{
				if (value == ResponsiblePersonRole.None)
				{
					base.DicomAttributeProvider[DicomTags.ResponsiblePersonRole] = null;
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ResponsiblePersonRole], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ResponsibleOrganization in the underlying collection. Type 2C.
		/// </summary>
		public string ResponsibleOrganization
		{
			get { return base.DicomAttributeProvider[DicomTags.ResponsibleOrganization].GetString(0, string.Empty); }
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ResponsibleOrganization] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ResponsibleOrganization].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientIdentityRemoved in the underlying collection. Type 3.
		/// </summary>
		public PatientIdentityRemoved PatientIdentityRemoved
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.PatientIdentityRemoved].GetString(0, string.Empty), PatientIdentityRemoved.Unknown); }
			set
			{
				if (value == PatientIdentityRemoved.Unknown)
				{
					base.DicomAttributeProvider[DicomTags.PatientIdentityRemoved] = null;
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PatientIdentityRemoved], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeIdentificationMethod in the underlying collection. Type 1C.
		/// </summary>
		public string DeIdentificationMethod
		{
			get { return base.DicomAttributeProvider[DicomTags.DeIdentificationMethod].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.DeIdentificationMethod] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.DeIdentificationMethod].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeIdentificationMethodCodeSequence in the underlying collection. Type 1C.
		/// </summary>
		public DeIdentificationMethodCodeSequence[] DeIdentificationMethodCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.DeIdentificationMethodCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				DeIdentificationMethodCodeSequence[] result = new DeIdentificationMethodCodeSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new DeIdentificationMethodCodeSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.DeIdentificationMethodCodeSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.DeIdentificationMethodCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.BreedRegistrationSequence;
				yield return DicomTags.DeIdentificationMethod;
				yield return DicomTags.DeIdentificationMethodCodeSequence;
				yield return DicomTags.EthnicGroup;
				yield return DicomTags.IssuerOfPatientId;
				yield return DicomTags.OtherPatientIds;
				yield return DicomTags.OtherPatientIdsSequence;
				yield return DicomTags.OtherPatientNames;
				yield return DicomTags.PatientBreedCodeSequence;
				yield return DicomTags.PatientBreedDescription;
				yield return DicomTags.PatientComments;
				yield return DicomTags.PatientId;
				yield return DicomTags.PatientIdentityRemoved;
				yield return DicomTags.PatientsBirthDate;
				yield return DicomTags.PatientsBirthTime;
				yield return DicomTags.PatientsName;
				yield return DicomTags.PatientSpeciesCodeSequence;
				yield return DicomTags.PatientSpeciesDescription;
				yield return DicomTags.PatientsSex;
				yield return DicomTags.ReferencedPatientSequence;
				yield return DicomTags.ResponsibleOrganization;
				yield return DicomTags.ResponsiblePerson;
				yield return DicomTags.ResponsiblePersonRole;
			}
		}
	}
}