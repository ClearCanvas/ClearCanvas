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

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Specimen Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.2 (Table C.7-2a)</remarks>
	public class SpecimenSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenSequence"/> class.
		/// </summary>
		public SpecimenSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public SpecimenSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of SpecimenIdentifier in the underlying collection. Type 2.
		/// </summary>
		public string SpecimenIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.SpecimenIdentifier].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SpecimenTypeCodeSequence in the underlying collection. Type 2C.
		/// </summary>
		public SpecimenTypeCodeSequence SpecimenTypeCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SpecimenTypeCodeSequence(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.SpecimenTypeCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of SlideIdentifier in the underlying collection. Type 2C.
		/// </summary>
		public string SlideIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.SlideIdentifierRetired].SetString(0, value);
			}
		}
	}
}