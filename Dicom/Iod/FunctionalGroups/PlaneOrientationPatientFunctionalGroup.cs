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
	/// Plane Orientation (Patient) Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.4 (Table C.7.6.16-5)</remarks>
	public class PlaneOrientationPatientFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationPatientFunctionalGroup"/> class.
		/// </summary>
		public PlaneOrientationPatientFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationPatientFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlaneOrientationPatientFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PlaneOrientationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.ImageOrientationPatient; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PlaneOrientationSequence in the underlying collection. Type 1.
		/// </summary>
		public PlaneOrientationSequenceItem PlaneOrientationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PlaneOrientationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationSequence];
				if (value == null)
				{
					const string msg = "PlaneOrientationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PlaneOrientationSequence in the underlying collection. Type 1.
		/// </summary>
		public PlaneOrientationSequenceItem CreatePlaneOrientationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PlaneOrientationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PlaneOrientationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Plane Orientation Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.4 (Table C.7.6.16-5)</remarks>
	public class PlaneOrientationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationSequenceItem"/> class.
		/// </summary>
		public PlaneOrientationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlaneOrientationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ImageOrientationPatient in the underlying collection. Type 1C.
		/// </summary>
		public double[] ImageOrientationPatient
		{
			get
			{
				var result = new double[6];
				if (DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(2, out result[2])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(3, out result[3])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(4, out result[4])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(5, out result[5]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 6)
				{
					DicomAttributeProvider[DicomTags.ImageOrientationPatient] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(2, value[2]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(3, value[3]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(4, value[4]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(5, value[5]);
			}
		}
	}
}