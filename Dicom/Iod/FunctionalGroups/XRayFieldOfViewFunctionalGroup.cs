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
	/// X-Ray Field of View Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.2 (Table C.8.19.6-2)</remarks>
	public class XRayFieldOfViewFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayFieldOfViewFunctionalGroup"/> class.
		/// </summary>
		public XRayFieldOfViewFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayFieldOfViewFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayFieldOfViewFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FieldOfViewSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FieldOfViewShape;
				yield return DicomTags.FieldOfViewDimensionsInFloat;
				yield return DicomTags.FieldOfViewOrigin;
				yield return DicomTags.FieldOfViewRotation;
				yield return DicomTags.FieldOfViewHorizontalFlip;
				yield return DicomTags.FieldOfViewDescription;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FieldOfViewSequence in the underlying collection. Type 1.
		/// </summary>
		public FieldOfViewSequenceItem FieldOfViewSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FieldOfViewSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FieldOfViewSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FieldOfViewSequence];
				if (value == null)
				{
					const string msg = "FieldOfViewSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FieldOfViewSequence in the underlying collection. Type 1.
		/// </summary>
		public FieldOfViewSequenceItem CreateFieldOfViewSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FieldOfViewSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FieldOfViewSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FieldOfViewSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Field of View Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.2 (Table C.8.19.6-2)</remarks>
	public class FieldOfViewSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FieldOfViewSequenceItem"/> class.
		/// </summary>
		public FieldOfViewSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldOfViewSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FieldOfViewSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}