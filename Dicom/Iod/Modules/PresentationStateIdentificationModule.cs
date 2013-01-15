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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PresentationStateIdentification Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.10 (Table C.11.10-1)</remarks>
	public class PresentationStateIdentificationModuleIod : IodBase, IContentIdentificationMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateIdentificationModuleIod"/> class.
		/// </summary>	
		public PresentationStateIdentificationModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateIdentificationModuleIod"/> class.
		/// </summary>
		public PresentationStateIdentificationModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets the dicom attribute collection as a dicom sequence item.
		/// </summary>
		/// <value>The dicom sequence item.</value>
		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { return base.DicomAttributeProvider as DicomSequenceItem; }
			set { base.DicomAttributeProvider = value; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes()
		{
			this.PresentationCreationDateTime = DateTime.Now;
			this.InstanceNumber = 1;
			this.ContentLabel = " ";
			this.ContentDescription = null;
			this.ContentCreatorsName = null;
			this.ContentCreatorsIdentificationCodeSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of PresentationCreationDate and PresentationCreationTime in the underlying collection.  Type 1.
		/// </summary>
		public DateTime? PresentationCreationDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.PresentationCreationDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.PresentationCreationTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
					throw new ArgumentNullException("value", "PresentationCreation is Type 1 Required.");
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.PresentationCreationDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.PresentationCreationTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 1.
		/// </summary>
		public int InstanceNumber {
			get { return base.DicomAttributeProvider[DicomTags.InstanceNumber].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ContentLabel in the underlying collection. Type 1.
		/// </summary>
		public string ContentLabel {
			get { return base.DicomAttributeProvider[DicomTags.ContentLabel].GetString(0, string.Empty); }
			set {
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "ContentLabel is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.ContentLabel].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentDescription in the underlying collection. Type 2.
		/// </summary>
		public string ContentDescription {
			get { return base.DicomAttributeProvider[DicomTags.ContentDescription].GetString(0, string.Empty); }
			set {
				if (string.IsNullOrEmpty(value)) {
					base.DicomAttributeProvider[DicomTags.ContentDescription].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentCreatorsName in the underlying collection. Type 2.
		/// </summary>
		public string ContentCreatorsName {
			get { return base.DicomAttributeProvider[DicomTags.ContentCreatorsName].GetString(0, string.Empty); }
			set {
				if (string.IsNullOrEmpty(value)) {
					base.DicomAttributeProvider[DicomTags.ContentCreatorsName].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentCreatorsName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentCreatorsIdentificationCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro ContentCreatorsIdentificationCodeSequence {
			get {
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0) {
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
			}
			set {
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence];
				if (value == null) {
					base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] { value.DicomSequenceItem };
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ContentCreatorsIdentificationCodeSequence;
				yield return DicomTags.ContentCreatorsName;
				yield return DicomTags.ContentDescription;
				yield return DicomTags.ContentLabel;
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.PresentationCreationDate;
				yield return DicomTags.PresentationCreationTime;
			}
		}
	}
}
