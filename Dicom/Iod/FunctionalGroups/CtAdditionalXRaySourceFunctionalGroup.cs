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
	/// CT Additional X-Ray Source Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.11 (Table C.8-126b)</remarks>
	public class CtAdditionalXRaySourceFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtAdditionalXRaySourceFunctionalGroup"/> class.
		/// </summary>
		public CtAdditionalXRaySourceFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtAdditionalXRaySourceFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtAdditionalXRaySourceFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtAdditionalXRaySourceSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.Kvp;
				yield return DicomTags.XRayTubeCurrentInMa;
				yield return DicomTags.DataCollectionDiameter;
				yield return DicomTags.FocalSpots;
				yield return DicomTags.FilterType;
				yield return DicomTags.FilterMaterial;
				yield return DicomTags.ExposureInMas;
				yield return DicomTags.EnergyWeightingFactor;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtAdditionalXRaySourceSequence in the underlying collection. Type 1.
		/// </summary>
		public CtAdditionalXRaySourceSequenceItem[] CtAdditionalXRaySourceSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtAdditionalXRaySourceSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new CtAdditionalXRaySourceSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CtAdditionalXRaySourceSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "CtAdditionalXRaySourceSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.CtAdditionalXRaySourceSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a CtAdditionalXRaySourceSequence item. Does not modify the CtAdditionalXRaySourceSequence in the underlying collection.
		/// </summary>
		public CtAdditionalXRaySourceSequenceItem CreateCtAdditionalXRaySourceSequenceItem()
		{
			var iodBase = new CtAdditionalXRaySourceSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// CT Additional X-Ray Source Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.11 (Table C.8-126b)</remarks>
	public class CtAdditionalXRaySourceSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtAdditionalXRaySourceSequenceItem"/> class.
		/// </summary>
		public CtAdditionalXRaySourceSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtAdditionalXRaySourceSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtAdditionalXRaySourceSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}