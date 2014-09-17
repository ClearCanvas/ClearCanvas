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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class VolumeSliceSopDataSource : StandardSopDataSource
	{
		/// <summary>
		/// Initializes a new <see cref="ISopDataSource"/> for the given volume slice.
		/// </summary>
		/// <param name="slice">A volume slice. This instance will be disposed when the <see cref="VolumeSliceSopDataSource"/> instance is disposed.</param>
		public VolumeSliceSopDataSource(VolumeSlice slice)
		{
			Slice = slice;
		    DataSet = new DicomAttributeCollection() {ValidateVrLengths = false, ValidateVrValues = false};
			FillDataSet(DataSet, slice);
		}

		public VolumeSlice Slice { get; private set; }

		public IDicomAttributeProvider DataSet { get; private set; }

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock (SyncLock)
				{
					// the instance dataset should always override the prototype values from the sliceset
					// if the operation results in a new attribute being inserted, do it in the instance dataset
					DicomAttribute attribute;
					if (DataSet.TryGetAttribute(tag, out attribute) || Slice.TryGetAttribute(tag, out attribute))
						return attribute;
					return DataSet[tag];
				}
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				lock (SyncLock)
				{
					// the instance dataset should always override the prototype values from the sliceset
					// if the operation results in a new attribute being inserted, do it in the instance dataset
					DicomAttribute attribute;
					if (DataSet.TryGetAttribute(tag, out attribute) || Slice.TryGetAttribute(tag, out attribute))
						return attribute;
					return DataSet[tag];
				}
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				// the instance dataset should always override the prototype values from the sliceset
				return DataSet.TryGetAttribute(tag, out attribute) || Slice.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				// the instance dataset should always override the prototype values from the sliceset
				return DataSet.TryGetAttribute(tag, out attribute) || Slice.TryGetAttribute(tag, out attribute);
			}
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			Platform.CheckArgumentRange(frameNumber, 1, 1, "frameNumber");
			return new VolumeSliceSopFrameData(frameNumber, this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Slice != null)
				{
					Slice.Dispose();
					Slice = null;
				}
				DataSet = null;
			}
			base.Dispose(disposing);
		}

		internal static void FillDataSet(IDicomAttributeProvider dataSet, VolumeSlice slice)
		{
			// generate values for SC Equipment Module
			var scEquipment = new ScEquipmentModuleIod(dataSet);
			scEquipment.ConversionType = @"WSD";
			scEquipment.SecondaryCaptureDeviceManufacturer = @"ClearCanvas Inc.";
			scEquipment.SecondaryCaptureDeviceManufacturersModelName = ProductInformation.GetName(false, false);
			scEquipment.SecondaryCaptureDeviceSoftwareVersions = new[] {ProductInformation.GetVersion(true, true, true, true)};

			// generate values for the General Image Module
			dataSet[DicomTags.ImageType].SetStringValue(@"DERIVED\SECONDARY");
			dataSet[DicomTags.DerivationDescription].SetStringValue(@"Multiplanar Reformatting");
			dataSet[DicomTags.DerivationCodeSequence].Values = new[] {ImageDerivationContextGroup.MultiplanarReformatting.AsDicomSequenceItem()};

			// update the Image Plane Module
			dataSet[DicomTags.PixelSpacing].SetStringValue(slice.PixelSpacing);
			dataSet[DicomTags.ImageOrientationPatient].SetStringValue(slice.ImageOrientationPatient);
			dataSet[DicomTags.ImagePositionPatient].SetStringValue(slice.ImagePositionPatient);
			dataSet[DicomTags.SliceThickness].SetStringValue(slice.SliceThickness);

			// update the spacing between slices, even though it's only part of modality-specific modules
			dataSet[DicomTags.SpacingBetweenSlices].SetStringValue(slice.SpacingBetweenSlices);

			// update the Image Pixel Module
			dataSet[DicomTags.Rows].SetInt32(0, slice.Rows);
			dataSet[DicomTags.Columns].SetInt32(0, slice.Columns);

			// generate values for Multi-Frame Module
			dataSet[DicomTags.NumberOfFrames].SetInt32(0, 1);

			// generate values for SOP Common Module
			dataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			dataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
		}

		protected class VolumeSliceSopFrameData : StandardSopFrameData
		{
			public VolumeSliceSopFrameData(int frameNumber, VolumeSliceSopDataSource parent)
				: base(frameNumber, parent) {}

			public new VolumeSliceSopDataSource Parent
			{
				get { return (VolumeSliceSopDataSource) base.Parent; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				return Parent.Slice.GetPixelData();
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
			{
				throw new NotSupportedException("MPR Volumes do not support overlays");
			}
		}
	}
}