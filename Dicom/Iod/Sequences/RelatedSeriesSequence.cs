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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// RelatedSeries Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.3.1 (Table C.7-5a)</remarks>
	public class RelatedSeriesSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RelatedSeriesSequence"/> class.
		/// </summary>
		public RelatedSeriesSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelatedSeriesSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public RelatedSeriesSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of StudyInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "StudyInstanceUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SeriesInstanceUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PurposeOfReferenceCodeSequence in the underlying collection. Type 2.
		/// </summary>
		public CodeSequenceMacro[] PurposeOfReferenceCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				CodeSequenceMacro[] result = new CodeSequenceMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CodeSequenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence].SetNullValue();
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence].Values = result;
			}
		}
	}
}