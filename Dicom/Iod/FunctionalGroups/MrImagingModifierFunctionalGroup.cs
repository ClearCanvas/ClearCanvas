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
	/// MR Imaging Modifier Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.6 (Table C.8-93)</remarks>
	public class MrImagingModifierFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrImagingModifierFunctionalGroup"/> class.
		/// </summary>
		public MrImagingModifierFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrImagingModifierFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrImagingModifierFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrImagingModifierSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.MagnetizationTransfer;
				yield return DicomTags.BloodSignalNulling;
				yield return DicomTags.Tagging;
				yield return DicomTags.TagSpacingFirstDimension;
				yield return DicomTags.TagSpacingSecondDimension;
				yield return DicomTags.TagAngleFirstAxis;
				yield return DicomTags.TagAngleSecondAxis;
				yield return DicomTags.TagThickness;
				yield return DicomTags.TaggingDelay;
				yield return DicomTags.TransmitterFrequency;
				yield return DicomTags.PixelBandwidth;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrImagingModifierSequence in the underlying collection. Type 1.
		/// </summary>
		public MrImagingModifierSequenceItem MrImagingModifierSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrImagingModifierSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrImagingModifierSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrImagingModifierSequence];
				if (value == null)
				{
					const string msg = "MrImagingModifierSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrImagingModifierSequence in the underlying collection. Type 1.
		/// </summary>
		public MrImagingModifierSequenceItem CreateMrImagingModifierSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrImagingModifierSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrImagingModifierSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrImagingModifierSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Imaging Modifier Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.6 (Table C.8-93)</remarks>
	public class MrImagingModifierSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrImagingModifierSequenceItem"/> class.
		/// </summary>
		public MrImagingModifierSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrImagingModifierSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrImagingModifierSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}