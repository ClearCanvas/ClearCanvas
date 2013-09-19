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
	/// PET Frame Correction Factors Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.5 (Table C.8.22-15)</remarks>
	public class PetFrameCorrectionFactorsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameCorrectionFactorsFunctionalGroup"/> class.
		/// </summary>
		public PetFrameCorrectionFactorsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameCorrectionFactorsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetFrameCorrectionFactorsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PetFrameCorrectionFactorsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.PrimaryPromptsCountsAccumulated;
				yield return DicomTags.SliceSensitivityFactor;
				yield return DicomTags.DecayFactor;
				yield return DicomTags.ScatterFractionFactor;
				yield return DicomTags.DeadTimeFactor;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PetFrameCorrectionFactorsSequence in the underlying collection. Type 1.
		/// </summary>
		public PetFrameCorrectionFactorsSequenceItem PetFrameCorrectionFactorsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameCorrectionFactorsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PetFrameCorrectionFactorsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameCorrectionFactorsSequence];
				if (value == null)
				{
					const string msg = "PetFrameCorrectionFactorsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PetFrameCorrectionFactorsSequence in the underlying collection. Type 1.
		/// </summary>
		public PetFrameCorrectionFactorsSequenceItem CreatePetFrameCorrectionFactorsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameCorrectionFactorsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PetFrameCorrectionFactorsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PetFrameCorrectionFactorsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// PET Frame Correction Factors Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.5 (Table C.8.22-15)</remarks>
	public class PetFrameCorrectionFactorsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameCorrectionFactorsSequenceItem"/> class.
		/// </summary>
		public PetFrameCorrectionFactorsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameCorrectionFactorsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetFrameCorrectionFactorsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}