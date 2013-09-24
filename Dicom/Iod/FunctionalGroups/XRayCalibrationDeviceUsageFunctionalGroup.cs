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
	/// X-Ray Calibration Device Usage Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.6 (Table C.8.19.6-6)</remarks>
	public class XRayCalibrationDeviceUsageFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayCalibrationDeviceUsageFunctionalGroup"/> class.
		/// </summary>
		public XRayCalibrationDeviceUsageFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayCalibrationDeviceUsageFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayCalibrationDeviceUsageFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CalibrationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.CalibrationImage; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CalibrationSequence in the underlying collection. Type 1.
		/// </summary>
		public CalibrationSequenceItem CalibrationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CalibrationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CalibrationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CalibrationSequence];
				if (value == null)
				{
					const string msg = "CalibrationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CalibrationSequence in the underlying collection. Type 1.
		/// </summary>
		public CalibrationSequenceItem CreateCalibrationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CalibrationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CalibrationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CalibrationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Calibration Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.6 (Table C.8.19.6-6)</remarks>
	public class CalibrationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalibrationSequenceItem"/> class.
		/// </summary>
		public CalibrationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CalibrationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CalibrationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of CalibrationImage in the underlying collection. Type 1.
		/// </summary>
		public string CalibrationImage
		{
			get { return DicomAttributeProvider[DicomTags.CalibrationImage].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "CalibrationImage is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.CalibrationImage].SetString(0, value);
			}
		}
	}
}