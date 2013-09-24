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
	/// Patient Orientation in Frame Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.15 (Table C.7.6.16-16)</remarks>
	public class PatientOrientationInFrameFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationInFrameFunctionalGroup"/> class.
		/// </summary>
		public PatientOrientationInFrameFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationInFrameFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PatientOrientationInFrameFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PatientOrientationInFrameSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.PatientOrientation; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PatientOrientationInFrameSequence in the underlying collection. Type 1.
		/// </summary>
		public PatientOrientationInFrameSequenceItem PatientOrientationInFrameSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationInFrameSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PatientOrientationInFrameSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationInFrameSequence];
				if (value == null)
				{
					const string msg = "PatientOrientationInFrameSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PatientOrientationInFrameSequence in the underlying collection. Type 1.
		/// </summary>
		public PatientOrientationInFrameSequenceItem CreatePatientOrientationInFrameSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationInFrameSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PatientOrientationInFrameSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PatientOrientationInFrameSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Patient Orientation in Frame Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.15 (Table C.7.6.16-16)</remarks>
	public class PatientOrientationInFrameSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationInFrameSequenceItem"/> class.
		/// </summary>
		public PatientOrientationInFrameSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationInFrameSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PatientOrientationInFrameSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PatientOrientation in the underlying collection. Type 1.
		/// </summary>
		public PatientOrientation PatientOrientation
		{
			get { return PatientOrientation.FromString(DicomAttributeProvider[DicomTags.PatientOrientation].ToString()); }
			set
			{
				if (value == null || value.IsEmpty)
				{
					const string msg = "PatientOrientation is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.PatientOrientation].SetStringValue(value.ToString());
			}
		}
	}
}