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
	/// Optical Path Identification Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.2 (Table C.8.12.6.2-1)</remarks>
	public class OpticalPathIdentificationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpticalPathIdentificationFunctionalGroup"/> class.
		/// </summary>
		public OpticalPathIdentificationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpticalPathIdentificationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public OpticalPathIdentificationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.OpticalPathIdentificationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.OpticalPathIdentifier; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of OpticalPathIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public OpticalPathIdentificationSequenceItem OpticalPathIdentificationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.OpticalPathIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new OpticalPathIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.OpticalPathIdentificationSequence];
				if (value == null)
				{
					const string msg = "OpticalPathIdentificationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the OpticalPathIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public OpticalPathIdentificationSequenceItem CreateOpticalPathIdentificationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.OpticalPathIdentificationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new OpticalPathIdentificationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new OpticalPathIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Optical Path Identification Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.2 (Table C.8.12.6.2-1)</remarks>
	public class OpticalPathIdentificationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpticalPathIdentificationSequenceItem"/> class.
		/// </summary>
		public OpticalPathIdentificationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpticalPathIdentificationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public OpticalPathIdentificationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of OpticalPathIdentifier in the underlying collection. Type 1.
		/// </summary>
		public string OpticalPathIdentifier
		{
			get { return DicomAttributeProvider[DicomTags.OpticalPathIdentifier].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "OpticalPathIdentifier is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.OpticalPathIdentifier].SetString(0, value);
			}
		}
	}
}