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
	/// X-Ray Exposure Control Sensing Regions Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.3 (Table C.8.19.6-3)</remarks>
	public class XRayExposureControlSensingRegionsFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayExposureControlSensingRegionsFunctionalGroup"/> class.
		/// </summary>
		public XRayExposureControlSensingRegionsFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayExposureControlSensingRegionsFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayExposureControlSensingRegionsFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ExposureControlSensingRegionsSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ExposureControlSensingRegionShape;
				yield return DicomTags.ExposureControlSensingRegionLeftVerticalEdge;
				yield return DicomTags.ExposureControlSensingRegionRightVerticalEdge;
				yield return DicomTags.ExposureControlSensingRegionUpperHorizontalEdge;
				yield return DicomTags.ExposureControlSensingRegionLowerHorizontalEdge;
				yield return DicomTags.CenterOfCircularExposureControlSensingRegion;
				yield return DicomTags.RadiusOfCircularExposureControlSensingRegion;
				yield return DicomTags.VerticesOfThePolygonalExposureControlSensingRegion;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ExposureControlSensingRegionsSequence in the underlying collection. Type 1.
		/// </summary>
		public ExposureControlSensingRegionsSequenceItem[] ExposureControlSensingRegionsSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ExposureControlSensingRegionsSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new ExposureControlSensingRegionsSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ExposureControlSensingRegionsSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "ExposureControlSensingRegionsSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ExposureControlSensingRegionsSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ExposureControlSensingRegionsSequence item. Does not modify the ExposureControlSensingRegionsSequence in the underlying collection.
		/// </summary>
		public ExposureControlSensingRegionsSequenceItem CreateExposureControlSensingRegionsSequenceItem()
		{
			var iodBase = new ExposureControlSensingRegionsSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Exposure Control Sensing Regions Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.3 (Table C.8.19.6-3)</remarks>
	public class ExposureControlSensingRegionsSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExposureControlSensingRegionsSequenceItem"/> class.
		/// </summary>
		public ExposureControlSensingRegionsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExposureControlSensingRegionsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ExposureControlSensingRegionsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}