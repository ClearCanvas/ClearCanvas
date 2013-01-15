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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// ContentIdentification Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.9 (Table 10-12)</remarks>
	public interface IContentIdentificationMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 1.
		/// </summary>
		int InstanceNumber { get; set; }

		/// <summary>
		/// Gets or sets the value of ContentLabel in the underlying collection. Type 1.
		/// </summary>
		string ContentLabel { get; set; }

		/// <summary>
		/// Gets or sets the value of ContentDescription in the underlying collection. Type 2.
		/// </summary>
		string ContentDescription { get; set; }

		/// <summary>
		/// Gets or sets the value of ContentCreatorsName in the underlying collection. Type 2.
		/// </summary>
		string ContentCreatorsName { get; set; }

		/// <summary>
		/// Gets or sets the value of ContentCreatorsIdentificationCodeSequence in the underlying collection. Type 3.
		/// </summary>
		PersonIdentificationMacro ContentCreatorsIdentificationCodeSequence { get; set; }
	}

	/// <summary>
	/// ContentIdentification Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.9 (Table 10-12)</remarks>
	internal class ContentIdentificationMacro : SequenceIodBase, IContentIdentificationMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentIdentificationMacro"/> class.
		/// </summary>
		public ContentIdentificationMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentIdentificationMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ContentIdentificationMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 1.
		/// </summary>
		public int InstanceNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.InstanceNumber].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ContentLabel in the underlying collection. Type 1.
		/// </summary>
		public string ContentLabel
		{
			get { return base.DicomAttributeProvider[DicomTags.ContentLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "ContentLabel is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.ContentLabel].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentDescription in the underlying collection. Type 2.
		/// </summary>
		public string ContentDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.ContentDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ContentDescription].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentCreatorsName in the underlying collection. Type 2.
		/// </summary>
		public string ContentCreatorsName
		{
			get { return base.DicomAttributeProvider[DicomTags.ContentCreatorsName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ContentCreatorsName].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentCreatorsName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentCreatorsIdentificationCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro ContentCreatorsIdentificationCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ContentCreatorsIdentificationCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}
	}
}