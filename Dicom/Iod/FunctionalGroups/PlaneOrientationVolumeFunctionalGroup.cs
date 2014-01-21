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
	/// Plane Orientation (Volume) Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.22 (Table C.7.6.16.2.22-1)</remarks>
	public class PlaneOrientationVolumeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationVolumeFunctionalGroup"/> class.
		/// </summary>
		public PlaneOrientationVolumeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationVolumeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlaneOrientationVolumeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PlaneOrientationVolumeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.ImageOrientationVolume; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PlaneOrientationVolumeSequence in the underlying collection. Type 1.
		/// </summary>
		public PlaneOrientationVolumeSequenceItem PlaneOrientationVolumeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationVolumeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PlaneOrientationVolumeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationVolumeSequence];
				if (value == null)
				{
					const string msg = "PlaneOrientationVolumeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PlaneOrientationVolumeSequence in the underlying collection. Type 1.
		/// </summary>
		public PlaneOrientationVolumeSequenceItem CreatePlaneOrientationVolumeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PlaneOrientationVolumeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PlaneOrientationVolumeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PlaneOrientationVolumeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Plane Orientation (Volume) Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.22 (Table C.7.6.16.2.22-1)</remarks>
	public class PlaneOrientationVolumeSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationVolumeSequenceItem"/> class.
		/// </summary>
		public PlaneOrientationVolumeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaneOrientationVolumeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlaneOrientationVolumeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ImageOrientationVolume in the underlying collection. Type 1.
		/// </summary>
		public double[] ImageOrientationVolume
		{
			get
			{
				var result = new double[6];
				if (DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(2, out result[2])
				    && DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(3, out result[3])
				    && DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(4, out result[4])
				    && DicomAttributeProvider[DicomTags.ImageOrientationVolume].TryGetFloat64(5, out result[5]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 6)
				{
					const string msg = "ImageOrientationVolume is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(2, value[2]);
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(3, value[3]);
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(4, value[4]);
				DicomAttributeProvider[DicomTags.ImageOrientationVolume].SetFloat64(5, value[5]);
			}
		}
	}
}