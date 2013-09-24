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
	/// CT Acquisition Details Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.3 (Table C.8-119)</remarks>
	public class CtAcquisitionDetailsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtAcquisitionDetailsFunctionalGroup"/> class.
		/// </summary>
		public CtAcquisitionDetailsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtAcquisitionDetailsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtAcquisitionDetailsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtAcquisitionDetailsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.RotationDirection;
				yield return DicomTags.RevolutionTime;
				yield return DicomTags.SingleCollimationWidth;
				yield return DicomTags.TotalCollimationWidth;
				yield return DicomTags.TableHeight;
				yield return DicomTags.GantryDetectorTilt;
				yield return DicomTags.DataCollectionDiameter;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtAcquisitionDetailsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtAcquisitionDetailsSequenceItem CtAcquisitionDetailsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtAcquisitionDetailsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtAcquisitionDetailsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtAcquisitionDetailsSequence];
				if (value == null)
				{
					const string msg = "CtAcquisitionDetailsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtAcquisitionDetailsSequence in the underlying collection. Type 1.
		/// </summary>
		public CtAcquisitionDetailsSequenceItem CreateCtAcquisitionDetailsSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtAcquisitionDetailsSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtAcquisitionDetailsSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtAcquisitionDetailsSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Acquisition Details Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.3 (Table C.8-119)</remarks>
	public class CtAcquisitionDetailsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtAcquisitionDetailsSequenceItem"/> class.
		/// </summary>
		public CtAcquisitionDetailsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtAcquisitionDetailsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtAcquisitionDetailsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}