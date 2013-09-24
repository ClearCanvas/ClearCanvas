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
	/// CT X-Ray Details Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.9 (Table C.8-125)</remarks>
	public class CtXRayDetailsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtXRayDetailsFunctionalGroup"/> class.
		/// </summary>
		public CtXRayDetailsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtXRayDetailsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtXRayDetailsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtXRayDetailsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.Kvp;
				yield return DicomTags.FocalSpots;
				yield return DicomTags.FilterType;
				yield return DicomTags.FilterMaterial;
				yield return DicomTags.CalciumScoringMassFactorPatient;
				yield return DicomTags.CalciumScoringMassFactorDevice;
				yield return DicomTags.EnergyWeightingFactor;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtXRayDetailsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtXRayDetailsSequenceItem CtXRayDetailsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtXRayDetailsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtXRayDetailsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtXRayDetailsSequence];
				if (value == null)
				{
					const string msg = "CtXRayDetailsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtXRayDetailsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtXRayDetailsSequenceItem CreateCtXRayDetailsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtXRayDetailsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtXRayDetailsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtXRayDetailsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT X-Ray Details Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.9 (Table C.8-125)</remarks>
	public class CtXRayDetailsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtXRayDetailsSequenceItem"/> class.
		/// </summary>
		public CtXRayDetailsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtXRayDetailsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtXRayDetailsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}