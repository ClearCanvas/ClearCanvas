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

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// SourceImage Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.1 (Table C.7-9)</remarks>
	public class SourceImageSequence : ImageSopInstanceReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceImageSequence"/> class.
		/// </summary>
		public SourceImageSequence() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SourceImageSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public SourceImageSequence(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PurposeOfReferenceCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro PurposeOfReferenceCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence];
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PurposeOfReferenceCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro CreatePurposeOfReferenceCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CodeSequenceMacro(dicomSequenceItem);
				return sequenceType;
			}
			return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of SpatialLocationsPreserved in the underlying collection. Type 3.
		/// </summary>
		public SpatialLocationsPreserved SpatialLocationsPreserved
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.SpatialLocationsPreserved].GetString(0, string.Empty), SpatialLocationsPreserved.None); }
			set
			{
				if (value == SpatialLocationsPreserved.None)
				{
					DicomAttributeProvider[DicomTags.SpatialLocationsPreserved] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.SpatialLocationsPreserved], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientOrientation in the underlying collection. Type 1C.
		/// </summary>
		public string PatientOrientation
		{
			get { return DicomAttributeProvider[DicomTags.PatientOrientation].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.PatientOrientation] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PatientOrientation].SetString(0, value);
			}
		}
	}

	/// <summary>
	/// Enumerated values for the <see cref="ClearCanvas.Dicom.DicomTags.SpatialLocationsPreserved"/> attribute describing
	/// the extent to which the spatial locations of all pixels are preserved during the processing of the source image that
	/// resulted in the current image.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.1 (Table C.7-9)</remarks>
	public enum SpatialLocationsPreserved
	{
// ReSharper disable InconsistentNaming

		/// <summary>
		/// Represents the null value.
		/// </summary>
		None,

		/// <summary>
		/// YES
		/// </summary>
		Yes,

		/// <summary>
		/// NO
		/// </summary>
		No,

		/// <summary>
		/// REORIENTED_ONLY
		/// </summary>
		Reoriented_Only

// ReSharper restore InconsistentNaming
	}
}