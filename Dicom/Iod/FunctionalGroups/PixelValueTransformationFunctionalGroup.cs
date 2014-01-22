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
	/// Pixel Value Transformation Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.9 (Table C.7.6.16-10)</remarks>
	public class PixelValueTransformationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelValueTransformationFunctionalGroup"/> class.
		/// </summary>
		public PixelValueTransformationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelValueTransformationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelValueTransformationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
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

		public override void InitializeAttributes() {}

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
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PixelValueTransformationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PixelValueTransformationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Pixel Value Transformation Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.9 (Table C.7.6.16-10)</remarks>
	public class PixelValueTransformationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelValueTransformationSequenceItem"/> class.
		/// </summary>
		public PixelValueTransformationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelValueTransformationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelValueTransformationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of RescaleIntercept in the underlying collection. Type 1.
		/// </summary>
		public double RescaleIntercept
		{
			get { return DicomAttributeProvider[DicomTags.RescaleIntercept].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.RescaleIntercept].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RescaleSlope in the underlying collection. Type 1.
		/// </summary>
		public double RescaleSlope
		{
			get { return DicomAttributeProvider[DicomTags.RescaleSlope].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.RescaleSlope].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RescaleType in the underlying collection. Type 1.
		/// </summary>
		public string RescaleType
		{
			get { return DicomAttributeProvider[DicomTags.RescaleType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "RescaleType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.RescaleType].SetString(0, value);
			}
		}
	}
}