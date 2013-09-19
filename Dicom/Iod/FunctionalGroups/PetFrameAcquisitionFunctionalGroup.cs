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
	/// PET Frame Acquisition Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.2 (Table C.8.22-11)</remarks>
	public class PetFrameAcquisitionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameAcquisitionFunctionalGroup"/> class.
		/// </summary>
		public PetFrameAcquisitionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameAcquisitionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetFrameAcquisitionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PetFrameAcquisitionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.TableHeight;
				yield return DicomTags.GantryDetectorTilt;
				yield return DicomTags.GantryDetectorSlew;
				yield return DicomTags.DataCollectionDiameter;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PetFrameAcquisitionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetFrameAcquisitionSequenceItem PetFrameAcquisitionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameAcquisitionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PetFrameAcquisitionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameAcquisitionSequence];
				if (value == null)
				{
					const string msg = "PetFrameAcquisitionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PetFrameAcquisitionSequence in the underlying collection. Type 1.
		/// </summary>
		public PetFrameAcquisitionSequenceItem CreatePetFrameAcquisitionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PetFrameAcquisitionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PetFrameAcquisitionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PetFrameAcquisitionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// PET Frame Acquisition Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.2 (Table C.8.22-11)</remarks>
	public class PetFrameAcquisitionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameAcquisitionSequenceItem"/> class.
		/// </summary>
		public PetFrameAcquisitionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetFrameAcquisitionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetFrameAcquisitionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}