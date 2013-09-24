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
	/// XA/XRF Frame Pixel Data Properties Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.4 (Table C.8.19.6-4)</remarks>
	public class XaXrfFramePixelDataPropertiesFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XaXrfFramePixelDataPropertiesFunctionalGroup"/> class.
		/// </summary>
		public XaXrfFramePixelDataPropertiesFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XaXrfFramePixelDataPropertiesFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XaXrfFramePixelDataPropertiesFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FramePixelDataPropertiesSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				yield return DicomTags.PixelIntensityRelationship;
				yield return DicomTags.PixelIntensityRelationshipSign;
				yield return DicomTags.ImagerPixelSpacing;
				yield return DicomTags.PixelDataAreaOriginRelativeToFov;
				yield return DicomTags.PixelDataAreaRotationAngleRelativeToFov;
				yield return DicomTags.GeometricalProperties;
				yield return DicomTags.GeometricMaximumDistortion;
				yield return DicomTags.ImageProcessingApplied;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FramePixelDataPropertiesSequence in the underlying collection. Type 1.
		/// </summary>
		public FramePixelDataPropertiesSequenceItem FramePixelDataPropertiesSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FramePixelDataPropertiesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FramePixelDataPropertiesSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FramePixelDataPropertiesSequence];
				if (value == null)
				{
					const string msg = "FramePixelDataPropertiesSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FramePixelDataPropertiesSequence in the underlying collection. Type 1.
		/// </summary>
		public FramePixelDataPropertiesSequenceItem CreateFramePixelDataPropertiesSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FramePixelDataPropertiesSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FramePixelDataPropertiesSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FramePixelDataPropertiesSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Frame Pixel Data Properties Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.4 (Table C.8.19.6-4)</remarks>
	public class FramePixelDataPropertiesSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelDataPropertiesSequenceItem"/> class.
		/// </summary>
		public FramePixelDataPropertiesSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelDataPropertiesSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FramePixelDataPropertiesSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}