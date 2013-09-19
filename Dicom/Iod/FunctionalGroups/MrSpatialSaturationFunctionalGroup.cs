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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// MR Spatial Saturation Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.11 (Table C.8-98)</remarks>
	public class MrSpatialSaturationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpatialSaturationFunctionalGroup"/> class.
		/// </summary>
		public MrSpatialSaturationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpatialSaturationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpatialSaturationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrSpatialSaturationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.SlabThickness;
				yield return DicomTags.SlabOrientation;
				yield return DicomTags.MidSlabPosition;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrSpatialSaturationSequence in the underlying collection. Type 2.
		/// </summary>
		public MrSpatialSaturationSequenceItem[] MrSpatialSaturationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpatialSaturationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new MrSpatialSaturationSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new MrSpatialSaturationSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.MrSpatialSaturationSequence].SetNullValue();
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.MrSpatialSaturationSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a MrSpatialSaturationSequence item. Does not modify the MrSpatialSaturationSequence in the underlying collection.
		/// </summary>
		public MrSpatialSaturationSequenceItem CreateMrSpatialSaturationSequenceItem()
		{
			var iodBase = new MrSpatialSaturationSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// MR Spatial Saturation Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.11 (Table C.8-98)</remarks>
	public class MrSpatialSaturationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpatialSaturationSequenceItem"/> class.
		/// </summary>
		public MrSpatialSaturationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpatialSaturationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpatialSaturationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}