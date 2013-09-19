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
	/// Identity Pixel Value Transformation Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.9b (Table C.7.6.16-10b)</remarks>
	public class IdentityPixelValueTransformationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IdentityPixelValueTransformationFunctionalGroup"/> class.
		/// </summary>
		public IdentityPixelValueTransformationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IdentityPixelValueTransformationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IdentityPixelValueTransformationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PixelValueTransformationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.RescaleIntercept;
				yield return DicomTags.RescaleSlope;
				yield return DicomTags.RescaleType;
			}
		}

		public override void InitializeAttributes()
		{
			CreatePixelValueTransformationSequence();
		}

		/// <summary>
		/// Gets or sets the value of PixelValueTransformationSequence in the underlying collection. Type 1.
		/// </summary>
		public PixelValueTransformationSequenceItem PixelValueTransformationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PixelValueTransformationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PixelValueTransformationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PixelValueTransformationSequence];
				if (value == null)
				{
					const string msg = "PixelValueTransformationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PixelValueTransformationSequence in the underlying collection. Type 1.
		/// </summary>
		public PixelValueTransformationSequenceItem CreatePixelValueTransformationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PixelValueTransformationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var sequenceType = new PixelValueTransformationSequenceItem {RescaleSlope = 1, RescaleIntercept = 0, RescaleType = "US"};
				dicomAttribute.Values = new[] {sequenceType.DicomSequenceItem};
				return sequenceType;
			}
			return new PixelValueTransformationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}