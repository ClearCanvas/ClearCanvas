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
	/// MR Modifier Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.5 (Table C.8-92)</remarks>
	public class MrModifierFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrModifierFunctionalGroup"/> class.
		/// </summary>
		public MrModifierFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrModifierFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrModifierFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrModifierSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.InversionRecovery;
				yield return DicomTags.InversionTimes;
				yield return DicomTags.FlowCompensation;
				yield return DicomTags.FlowCompensationDirection;
				yield return DicomTags.Spoiling;
				yield return DicomTags.T2Preparation;
				yield return DicomTags.SpectrallySelectedExcitation;
				yield return DicomTags.SpatialPreSaturation;
				yield return DicomTags.PartialFourier;
				yield return DicomTags.PartialFourierDirection;
				yield return DicomTags.ParallelAcquisition;
				yield return DicomTags.ParallelAcquisitionTechnique;
				yield return DicomTags.ParallelReductionFactorInPlane;
				yield return DicomTags.ParallelReductionFactorOutOfPlane;
				yield return DicomTags.ParallelReductionFactorSecondInPlane;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrModifierSequence in the underlying collection. Type 1.
		/// </summary>
		public MrModifierSequenceItem MrModifierSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrModifierSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrModifierSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrModifierSequence];
				if (value == null)
				{
					const string msg = "MrModifierSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrModifierSequence in the underlying collection. Type 1.
		/// </summary>
		public MrModifierSequenceItem CreateMrModifierSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrModifierSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrModifierSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrModifierSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Modifier Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.5 (Table C.8-92)</remarks>
	public class MrModifierSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrModifierSequenceItem"/> class.
		/// </summary>
		public MrModifierSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrModifierSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrModifierSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}