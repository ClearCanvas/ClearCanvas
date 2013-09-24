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
	/// Plane Position (Patient) Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.3 (Table C.7.6.16-4)</remarks>
	public class PlanePositionPatientFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionPatientFunctionalGroup"/> class.
		/// </summary>
		public PlanePositionPatientFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionPatientFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionPatientFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PlanePositionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.ImagePositionPatient; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PlanePositionSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionSequenceItem PlanePositionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PlanePositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSequence];
				if (value == null)
				{
					const string msg = "PlanePositionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PlanePositionSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionSequenceItem CreatePlanePositionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PlanePositionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PlanePositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Plane Position Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.3 (Table C.7.6.16-4)</remarks>
	public class PlanePositionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSequenceItem"/> class.
		/// </summary>
		public PlanePositionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ImagePositionPatient in the underlying collection. Type 1C.
		/// </summary>
		public double[] ImagePositionPatient
		{
			get
			{
				var result = new double[3];
				if (DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(2, out result[2]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 3)
				{
					DicomAttributeProvider[DicomTags.ImagePositionPatient] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(2, value[2]);
			}
		}
	}
}