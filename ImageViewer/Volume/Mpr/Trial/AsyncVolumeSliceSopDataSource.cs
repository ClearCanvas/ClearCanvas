#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	internal class AsyncVolumeSliceSopDataSource : AsyncSopDataSource
	{
		/// <summary>
		/// Initializes a new <see cref="ISopDataSource"/> for the given volume slice.
		/// </summary>
		/// <param name="slice">A volume slice. This instance will be disposed when the <see cref="VolumeSliceSopDataSource"/> instance is disposed.</param>
		public AsyncVolumeSliceSopDataSource(VolumeSlice slice)
		{
			Slice = slice;

			DataSet = new DicomAttributeCollection();
			DataSet[DicomTags.Rows].SetInt32(0, Slice.Rows);
			DataSet[DicomTags.Columns].SetInt32(0, Slice.Colums);
			DataSet[DicomTags.NumberOfFrames].SetInt32(0, 1);
			DataSet[DicomTags.ImageOrientationPatient].SetStringValue(Slice.ImageOrientationPatient);
			DataSet[DicomTags.ImagePositionPatient].SetStringValue(Slice.ImagePositionPatient);
			DataSet[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);
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

		protected override AsyncSopFrameData CreateFrameData(int frameNumber)
		{
			Platform.CheckArgumentRange(frameNumber, 1, 1, "frameNumber");
			return new AsyncVolumeSliceSopFrameData(frameNumber, this);
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

		protected class AsyncVolumeSliceSopFrameData : AsyncSopFrameData
		{
			public AsyncVolumeSliceSopFrameData(int frameNumber, AsyncVolumeSliceSopDataSource parent)
				: base(frameNumber, parent) {}

			public new AsyncVolumeSliceSopDataSource Parent
			{
				get { return (AsyncVolumeSliceSopDataSource) base.Parent; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				// only ICachedVolumeReference provides for dynamic loading of the volume - a standard IVolumeReference is a hard reference to a loaded volume
				if (Parent.Slice.VolumeReference is ICachedVolumeReference)
				{
					using (var volume = (ICachedVolumeReference) Parent.Slice.VolumeReference.Clone())
					{
						volume.Lock();

						if (!volume.IsLoaded)
						{
							UpdateProgress(0, null);
							var ar = volume.LoadAsync();
							while (!ar.IsCompleted)
							{
								UpdateProgress(volume.Progress, null);
								ar.AsyncWaitHandle.WaitOne(100);
							}
						}

						UpdateProgress(100, null);
						var pixelData = Parent.Slice.GetPixelData();
						UpdateProgress(100, pixelData);

						volume.Unlock();

						return pixelData;
					}
				}
				return Parent.Slice.GetPixelData();
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
			{
				throw new NotSupportedException("MPR Volumes do not support overlays");
			}
		}
	}
}