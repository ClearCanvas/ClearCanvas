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
	/// Plane Position (Slide) Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.1 (Table C.8.12.6.1-1)</remarks>
	public class PlanePositionSlideFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSlideFunctionalGroup"/> class.
		/// </summary>
		public PlanePositionSlideFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSlideFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionSlideFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PlanePositionSlideSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ColumnPositionInTotalImagePixelMatrix;
				yield return DicomTags.RowPositionInTotalImagePixelMatrix;
				yield return DicomTags.XOffsetInSlideCoordinateSystem;
				yield return DicomTags.YOffsetInSlideCoordinateSystem;
				yield return DicomTags.ZOffsetInSlideCoordinateSystem;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PlanePositionSlideSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionSlideSequenceItem PlanePositionSlideSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSlideSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PlanePositionSlideSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSlideSequence];
				if (value == null)
				{
					const string msg = "PlanePositionSlideSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PlanePositionSlideSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionSlideSequenceItem CreatePlanePositionSlideSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSlideSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PlanePositionSlideSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PlanePositionSlideSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Plane Position (Slide) Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.1 (Table C.8.12.6.1-1)</remarks>
	public class PlanePositionSlideSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSlideSequenceItem"/> class.
		/// </summary>
		public PlanePositionSlideSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSlideSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionSlideSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ColumnPositionInTotalImagePixelMatrix in the underlying collection. Type 1.
		/// </summary>
		public int ColumnPositionInTotalImagePixelMatrix
		{
			get { return DicomAttributeProvider[DicomTags.ColumnPositionInTotalImagePixelMatrix].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.ColumnPositionInTotalImagePixelMatrix].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RowPositionInTotalImagePixelMatrix in the underlying collection. Type 1.
		/// </summary>
		public int RowPositionInTotalImagePixelMatrix
		{
			get { return DicomAttributeProvider[DicomTags.RowPositionInTotalImagePixelMatrix].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.RowPositionInTotalImagePixelMatrix].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of XOffsetInSlideCoordinateSystem in the underlying collection. Type 1.
		/// </summary>
		public double XOffsetInSlideCoordinateSystem
		{
			get { return DicomAttributeProvider[DicomTags.XOffsetInSlideCoordinateSystem].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.XOffsetInSlideCoordinateSystem].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of YOffsetInSlideCoordinateSystem in the underlying collection. Type 1.
		/// </summary>
		public double YOffsetInSlideCoordinateSystem
		{
			get { return DicomAttributeProvider[DicomTags.YOffsetInSlideCoordinateSystem].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.YOffsetInSlideCoordinateSystem].SetFloat64(0, value); }
		}
	}
}