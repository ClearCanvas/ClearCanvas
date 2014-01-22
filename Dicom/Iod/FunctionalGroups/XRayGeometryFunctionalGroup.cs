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
	/// X-Ray Geometry Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.14 (Table C.8.19.6-14)</remarks>
	public class XRayGeometryFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayGeometryFunctionalGroup"/> class.
		/// </summary>
		public XRayGeometryFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayGeometryFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayGeometryFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.XRayGeometrySequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.DistanceSourceToIsocenter;
				yield return DicomTags.DistanceSourceToDetector;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of XRayGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public XRayGeometrySequenceItem XRayGeometrySequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRayGeometrySequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new XRayGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRayGeometrySequence];
				if (value == null)
				{
					const string msg = "XRayGeometrySequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the XRayGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public XRayGeometrySequenceItem CreateXRayGeometrySequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.XRayGeometrySequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new XRayGeometrySequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new XRayGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// X-Ray Geometry Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.14 (Table C.8.19.6-14)</remarks>
	public class XRayGeometrySequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayGeometrySequenceItem"/> class.
		/// </summary>
		public XRayGeometrySequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayGeometrySequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayGeometrySequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DistanceSourceToIsocenter in the underlying collection. Type 1.
		/// </summary>
		public double DistanceSourceToIsocenter
		{
			get { return DicomAttributeProvider[DicomTags.DistanceSourceToIsocenter].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.DistanceSourceToIsocenter].SetFloat64(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of DistanceSourceToDetector in the underlying collection. Type 1.
		/// </summary>
		public double DistanceSourceToDetector
		{
			get { return DicomAttributeProvider[DicomTags.DistanceSourceToDetector].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.DistanceSourceToDetector].SetFloat64(0, value); }
		}
	}
}