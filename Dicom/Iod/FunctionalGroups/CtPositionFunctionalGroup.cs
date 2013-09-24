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
	/// CT Position Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.5 (Table C.8-121)</remarks>
	public class CtPositionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtPositionFunctionalGroup"/> class.
		/// </summary>
		public CtPositionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtPositionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtPositionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtPositionSequence; }
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
		/// Gets or sets the value of CtPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public CtPositionSequenceItem CtPositionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtPositionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtPositionSequence];
				if (value == null)
				{
					const string msg = "CtPositionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public CtPositionSequenceItem CreateCtPositionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtPositionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtPositionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Position Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.5 (Table C.8-121)</remarks>
	public class CtPositionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtPositionSequenceItem"/> class.
		/// </summary>
		public CtPositionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtPositionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtPositionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}