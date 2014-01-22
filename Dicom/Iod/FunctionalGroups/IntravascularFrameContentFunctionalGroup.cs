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
	/// Intravascular Frame Content Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.27.6.2 (Table C.8.27.6.2-1)</remarks>
	public class IntravascularFrameContentFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularFrameContentFunctionalGroup"/> class.
		/// </summary>
		public IntravascularFrameContentFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularFrameContentFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IntravascularFrameContentFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IntravascularFrameContentSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.IntravascularLongitudinalDistance;
				yield return DicomTags.SeamLineLocation;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of IntravascularFrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public IntravascularFrameContentSequenceItem IntravascularFrameContentSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularFrameContentSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new IntravascularFrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularFrameContentSequence];
				if (value == null)
				{
					const string msg = "IntravascularFrameContentSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the IntravascularFrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public IntravascularFrameContentSequenceItem CreateIntravascularFrameContentSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.IntravascularFrameContentSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new IntravascularFrameContentSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new IntravascularFrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Intravascular Frame Content Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.27.6.2 (Table C.8.27.6.2-1)</remarks>
	public class IntravascularFrameContentSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularFrameContentSequenceItem"/> class.
		/// </summary>
		public IntravascularFrameContentSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntravascularFrameContentSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IntravascularFrameContentSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}