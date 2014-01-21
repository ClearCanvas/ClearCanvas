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
	/// CT Exposure Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.8 (Table C.8-124)</remarks>
	public class CtExposureFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtExposureFunctionalGroup"/> class.
		/// </summary>
		public CtExposureFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtExposureFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtExposureFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtExposureSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ExposureTimeInMs;
				yield return DicomTags.XRayTubeCurrentInMa;
				yield return DicomTags.ExposureInMas;
				yield return DicomTags.ExposureModulationType;
				yield return DicomTags.EstimatedDoseSaving;
				yield return DicomTags.Ctdivol;
				yield return DicomTags.CtdiPhantomTypeCodeSequence;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtExposureSequence in the underlying collection. Type 1.
		/// </summary>
		public CtExposureSequenceItem CtExposureSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtExposureSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtExposureSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtExposureSequence];
				if (value == null)
				{
					const string msg = "CtExposureSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtExposureSequence in the underlying collection. Type 1.
		/// </summary>
		public CtExposureSequenceItem CreateCtExposureSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtExposureSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtExposureSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtExposureSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Exposure Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.8 (Table C.8-124)</remarks>
	public class CtExposureSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtExposureSequenceItem"/> class.
		/// </summary>
		public CtExposureSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtExposureSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtExposureSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}