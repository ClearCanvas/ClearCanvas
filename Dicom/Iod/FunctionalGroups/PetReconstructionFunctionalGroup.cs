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
	/// PET Reconstruction Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.6 (Table C.8.22-17)</remarks>
	public class PetReconstructionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetReconstructionFunctionalGroup"/> class.
		/// </summary>
		public PetReconstructionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetReconstructionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetReconstructionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PetReconstructionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ReconstructionType;
				yield return DicomTags.ReconstructionAlgorithm;
				yield return DicomTags.IterativeReconstructionMethod;
				yield return DicomTags.NumberOfIterations;
				yield return DicomTags.NumberOfSubsets;
				yield return DicomTags.ReconstructionDiameter;
				yield return DicomTags.ReconstructionFieldOfView;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PetReconstructionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetReconstructionSequenceItem PetReconstructionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetReconstructionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PetReconstructionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetReconstructionSequence];
				if (value == null)
				{
					const string msg = "PetReconstructionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PetReconstructionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetReconstructionSequenceItem CreatePetReconstructionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PetReconstructionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PetReconstructionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PetReconstructionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// PET Reconstruction Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.6 (Table C.8.22-17)</remarks>
	public class PetReconstructionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetReconstructionSequenceItem"/> class.
		/// </summary>
		public PetReconstructionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetReconstructionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetReconstructionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}