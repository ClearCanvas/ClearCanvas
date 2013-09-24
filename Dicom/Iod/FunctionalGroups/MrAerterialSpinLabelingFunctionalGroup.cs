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
	/// MR Arterial Spin Labeling Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.14 (Table C.8-100b)</remarks>
	public class MrArterialSpinLabelingFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrArterialSpinLabelingFunctionalGroup"/> class.
		/// </summary>
		public MrArterialSpinLabelingFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrArterialSpinLabelingFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrArterialSpinLabelingFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrArterialSpinLabelingSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.AslTechniqueDescription;
				yield return DicomTags.AslContext;
				yield return DicomTags.AslSlabSequence;
				yield return DicomTags.AslCrusherFlag;
				yield return DicomTags.AslCrusherFlow;
				yield return DicomTags.AslCrusherDescription;
				yield return DicomTags.AslBolusCutOffFlag;
				yield return DicomTags.AslBolusCutOffTimingSequence;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrArterialSpinLabelingSequence in the underlying collection. Type 1.
		/// </summary>
		public MrArterialSpinLabelingSequenceItem[] MrArterialSpinLabelingSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrArterialSpinLabelingSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new MrArterialSpinLabelingSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new MrArterialSpinLabelingSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "MrArterialSpinLabelingSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.MrArterialSpinLabelingSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a MrArterialSpinLabelingSequence item. Does not modify the MrArterialSpinLabelingSequence in the underlying collection.
		/// </summary>
		public MrArterialSpinLabelingSequenceItem CreateMrArterialSpinLabelingSequenceItem()
		{
			var iodBase = new MrArterialSpinLabelingSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// MR Arterial Spin Labeling Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.14 (Table C.8-100b)</remarks>
	public class MrArterialSpinLabelingSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrArterialSpinLabelingSequenceItem"/> class.
		/// </summary>
		public MrArterialSpinLabelingSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrArterialSpinLabelingSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrArterialSpinLabelingSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}