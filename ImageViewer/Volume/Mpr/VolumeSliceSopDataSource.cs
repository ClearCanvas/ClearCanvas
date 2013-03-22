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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class VolumeSliceSopDataSource : StandardSopDataSource
	{
		private DicomAttributeCollection _instance;
		private VolumeSlice _slice;

		public VolumeSliceSopDataSource(VolumeSlice slice)
		{
			_slice = slice;

			_instance = new DicomAttributeCollection();
			_instance[DicomTags.Rows].SetInt32(0, _slice.Rows);
			_instance[DicomTags.Columns].SetInt32(0, _slice.Colums);
			_instance[DicomTags.NumberOfFrames].SetInt32(0, 1);
			_instance[DicomTags.ImageOrientationPatient].SetStringValue(_slice.ImageOrientationPatient);
			_instance[DicomTags.ImagePositionPatient].SetStringValue(_slice.ImagePositionPatient);
			_instance[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);
		}

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock (SyncLock)
				{
					// the instance dataset should always override the prototype values from the sliceset
					// if the operation results in a new attribute being inserted, do it in the instance dataset
					DicomAttribute attribute;
					if (_instance.TryGetAttribute(tag, out attribute) || _slice.TryGetAttribute(tag, out attribute))
						return attribute;
					return _instance[tag];
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
					if (_instance.TryGetAttribute(tag, out attribute) || _slice.TryGetAttribute(tag, out attribute))
						return attribute;
					return _instance[tag];
				}
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				// the instance dataset should always override the prototype values from the sliceset
				return _instance.TryGetAttribute(tag, out attribute) || _slice.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				// the instance dataset should always override the prototype values from the sliceset
				return _instance.TryGetAttribute(tag, out attribute) || _slice.TryGetAttribute(tag, out attribute);
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
				if (_slice != null)
				{
					_slice.Dispose();
					_slice = null;
				}
				_instance = null;
			}
			base.Dispose(disposing);
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
				return Parent._slice.GetPixelData();
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
			{
				throw new NotSupportedException("MPR Volumes do not support overlays");
			}
		}
	}
}