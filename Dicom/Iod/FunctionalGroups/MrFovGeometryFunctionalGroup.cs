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
	/// MR FOV/Geometry Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.3 (Table C.8-90)</remarks>
	public class MrFovGeometryFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrFovGeometryFunctionalGroup"/> class.
		/// </summary>
		public MrFovGeometryFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrFovGeometryFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrFovGeometryFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrFovGeometrySequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.InPlanePhaseEncodingDirection;
				yield return DicomTags.MrAcquisitionFrequencyEncodingSteps;
				yield return DicomTags.MrAcquisitionPhaseEncodingStepsInPlane;
				yield return DicomTags.MrAcquisitionPhaseEncodingStepsOutOfPlane;
				yield return DicomTags.PercentSampling;
				yield return DicomTags.PercentPhaseFieldOfView;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrFovGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public MrFovGeometrySequenceItem MrFovGeometrySequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrFovGeometrySequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrFovGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrFovGeometrySequence];
				if (value == null)
				{
					const string msg = "MrFovGeometrySequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrFovGeometrySequence in the underlying collection. Type 1.
		/// </summary>
		public MrFovGeometrySequenceItem CreateMrFovGeometrySequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrFovGeometrySequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrFovGeometrySequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrFovGeometrySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR FOV/Geometry Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.3 (Table C.8-90)</remarks>
	public class MrFovGeometrySequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrFovGeometrySequenceItem"/> class.
		/// </summary>
		public MrFovGeometrySequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrFovGeometrySequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrFovGeometrySequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}