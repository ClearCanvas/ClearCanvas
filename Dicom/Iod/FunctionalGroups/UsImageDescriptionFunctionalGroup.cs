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
	/// US Image Description Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.24.6.1 (Table C.8.24.6.1-1)</remarks>
	public class UsImageDescriptionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UsImageDescriptionFunctionalGroup"/> class.
		/// </summary>
		public UsImageDescriptionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="UsImageDescriptionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public UsImageDescriptionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.UsImageDescriptionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				yield return DicomTags.VolumetricProperties;
				yield return DicomTags.VolumeBasedCalculationTechnique;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of UsImageDescriptionSequence in the underlying collection. Type 1.
		/// </summary>
		public UsImageDescriptionSequenceItem UsImageDescriptionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.UsImageDescriptionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new UsImageDescriptionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.UsImageDescriptionSequence];
				if (value == null)
				{
					const string msg = "UsImageDescriptionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the UsImageDescriptionSequence in the underlying collection. Type 1.
		/// </summary>
		public UsImageDescriptionSequenceItem CreateUsImageDescriptionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.UsImageDescriptionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new UsImageDescriptionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new UsImageDescriptionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// US Image Description Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.24.6.1 (Table C.8.24.6.1-1)</remarks>
	public class UsImageDescriptionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UsImageDescriptionSequenceItem"/> class.
		/// </summary>
		public UsImageDescriptionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="UsImageDescriptionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public UsImageDescriptionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}