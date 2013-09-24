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
	/// Intravascular OCT Frame Content Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.27.6.3 (Table C.8.27.6.3-1)</remarks>
	public class IntravascularOctFrameContentFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularOctFrameContentFunctionalGroup"/> class.
		/// </summary>
		public IntravascularOctFrameContentFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularOctFrameContentFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IntravascularOctFrameContentFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IntravascularOctFrameContentSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.OctZOffsetCorrection;
				yield return DicomTags.SeamLineIndex;
				yield return DicomTags.NumberOfPaddedAlines;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of IntravascularOctFrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public IntravascularOctFrameContentSequenceItem IntravascularOctFrameContentSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularOctFrameContentSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new IntravascularOctFrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularOctFrameContentSequence];
				if (value == null)
				{
					const string msg = "IntravascularOctFrameContentSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the IntravascularOctFrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public IntravascularOctFrameContentSequenceItem CreateIntravascularOctFrameContentSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularOctFrameContentSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new IntravascularOctFrameContentSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new IntravascularOctFrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Intravascular OCT Frame Content Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.27.6.3 (Table C.8.27.6.3-1)</remarks>
	public class IntravascularOctFrameContentSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularOctFrameContentSequenceItem"/> class.
		/// </summary>
		public IntravascularOctFrameContentSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularOctFrameContentSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IntravascularOctFrameContentSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}