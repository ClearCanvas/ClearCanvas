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
	/// PET Position Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.4 (Table C.8.22-14)</remarks>
	public class PetPositionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetPositionFunctionalGroup"/> class.
		/// </summary>
		public PetPositionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetPositionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetPositionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PetPositionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.TablePosition;
				yield return DicomTags.DataCollectionCenterPatient;
				yield return DicomTags.ReconstructionTargetCenterPatient;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PetPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetPositionSequenceItem PetPositionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetPositionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PetPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetPositionSequence];
				if (value == null)
				{
					const string msg = "PetPositionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PetPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetPositionSequenceItem CreatePetPositionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PetPositionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PetPositionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PetPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// PET Position Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.4 (Table C.8.22-14)</remarks>
	public class PetPositionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetPositionSequenceItem"/> class.
		/// </summary>
		public PetPositionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetPositionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetPositionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}