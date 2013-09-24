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
	/// X-Ray Projection Pixel Calibration Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.9 (Table C.8.19.6-9)</remarks>
	public class XRayProjectionPixelCalibrationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayProjectionPixelCalibrationFunctionalGroup"/> class.
		/// </summary>
		public XRayProjectionPixelCalibrationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayProjectionPixelCalibrationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayProjectionPixelCalibrationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ProjectionPixelCalibrationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.DistanceObjectToTableTop;
				yield return DicomTags.ObjectPixelSpacingInCenterOfBeam;
				yield return DicomTags.TableHeight;
				yield return DicomTags.BeamAngle;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ProjectionPixelCalibrationSequence in the underlying collection. Type 1.
		/// </summary>
		public ProjectionPixelCalibrationSequenceItem ProjectionPixelCalibrationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ProjectionPixelCalibrationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new ProjectionPixelCalibrationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ProjectionPixelCalibrationSequence];
				if (value == null)
				{
					const string msg = "ProjectionPixelCalibrationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ProjectionPixelCalibrationSequence in the underlying collection. Type 1.
		/// </summary>
		public ProjectionPixelCalibrationSequenceItem CreateProjectionPixelCalibrationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.ProjectionPixelCalibrationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new ProjectionPixelCalibrationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new ProjectionPixelCalibrationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Projection Pixel Calibration Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.9 (Table C.8.19.6-9)</remarks>
	public class ProjectionPixelCalibrationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionPixelCalibrationSequenceItem"/> class.
		/// </summary>
		public ProjectionPixelCalibrationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionPixelCalibrationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ProjectionPixelCalibrationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}