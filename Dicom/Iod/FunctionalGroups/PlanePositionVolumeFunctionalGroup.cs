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
	/// Plane Position (Volume) Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.21 (Table C.7.6.16.2.21-1)</remarks>
	public class PlanePositionVolumeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionVolumeFunctionalGroup"/> class.
		/// </summary>
		public PlanePositionVolumeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionVolumeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionVolumeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PlanePositionVolumeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.ImagePositionVolume; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PlanePositionVolumeSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionVolumeSequenceItem PlanePositionVolumeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionVolumeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PlanePositionVolumeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionVolumeSequence];
				if (value == null)
				{
					const string msg = "PlanePositionVolumeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PlanePositionVolumeSequence in the underlying collection. Type 1.
		/// </summary>
		public PlanePositionVolumeSequenceItem CreatePlanePositionVolumeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PlanePositionVolumeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PlanePositionVolumeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PlanePositionVolumeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Plane Position (Volume) Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.21 (Table C.7.6.16.2.21-1)</remarks>
	public class PlanePositionVolumeSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionVolumeSequenceItem"/> class.
		/// </summary>
		public PlanePositionVolumeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlanePositionVolumeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PlanePositionVolumeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ImagePositionVolume in the underlying collection. Type 1.
		/// </summary>
		public double[] ImagePositionVolume
		{
			get
			{
				var result = new double[3];
				if (DicomAttributeProvider[DicomTags.ImagePositionVolume].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImagePositionVolume].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImagePositionVolume].TryGetFloat64(2, out result[2]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 3)
				{
					const string msg = "ImagePositionVolume is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ImagePositionVolume].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImagePositionVolume].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImagePositionVolume].SetFloat64(2, value[2]);
			}
		}
	}
}