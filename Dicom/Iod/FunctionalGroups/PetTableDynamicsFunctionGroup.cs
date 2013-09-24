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
	/// PET Table Dynamics Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.7 (Table C.8.22-18)</remarks>
	public class PetTableDynamicsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetTableDynamicsFunctionalGroup"/> class.
		/// </summary>
		public PetTableDynamicsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetTableDynamicsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetTableDynamicsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PetTableDynamicsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.TableSpeed; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PetTableDynamicsSequence in the underlying collection. Type 1.
		/// </summary>
		public PetTableDynamicsSequenceItem PetTableDynamicsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetTableDynamicsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PetTableDynamicsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PetTableDynamicsSequence];
				if (value == null)
				{
					const string msg = "PetTableDynamicsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PetTableDynamicsSequence in the underlying collection. Type 1.
		/// </summary>
		public PetTableDynamicsSequenceItem CreatePetTableDynamicsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PetTableDynamicsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PetTableDynamicsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PetTableDynamicsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// PET Table Dynamics Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.22.5.7 (Table C.8.22-18)</remarks>
	public class PetTableDynamicsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PetTableDynamicsSequenceItem"/> class.
		/// </summary>
		public PetTableDynamicsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PetTableDynamicsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PetTableDynamicsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}