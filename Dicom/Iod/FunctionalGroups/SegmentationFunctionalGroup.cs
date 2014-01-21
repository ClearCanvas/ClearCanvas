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

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Segmentation Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.20.3 (Table C.8.20-3)</remarks>
	public class SegmentationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SegmentationFunctionalGroup"/> class.
		/// </summary>
		public SegmentationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SegmentationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public SegmentationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.SegmentIdentificationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.ReferencedSegmentNumber; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of SegmentIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public SegmentIdentificationSequenceItem SegmentIdentificationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SegmentIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new SegmentIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SegmentIdentificationSequence];
				if (value == null)
				{
					const string msg = "SegmentIdentificationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the SegmentIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public SegmentIdentificationSequenceItem CreateSegmentIdentificationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.SegmentIdentificationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new SegmentIdentificationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new SegmentIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Segment Identification Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.20.3 (Table C.8.20-3)</remarks>
	public class SegmentIdentificationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SegmentIdentificationSequenceItem"/> class.
		/// </summary>
		public SegmentIdentificationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SegmentIdentificationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public SegmentIdentificationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ReferencedSegmentNumber in the underlying collection. Type 1.
		/// </summary>
		public int ReferencedSegmentNumber
		{
			get { return DicomAttributeProvider[DicomTags.ReferencedSegmentNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.ReferencedSegmentNumber].SetInt32(0, value); }
		}
	}
}