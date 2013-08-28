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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
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
			private ICachedVolumeReference _cachedVolumeReference;
			private readonly object _lockSync = new object();
			private volatile int _lockCount;

			public AsyncVolumeSliceSopFrameData(int frameNumber, AsyncVolumeSliceSopDataSource parent)
				: base(frameNumber, parent)
			{
				if (Parent.Slice.VolumeReference is ICachedVolumeReference)
				{
					_cachedVolumeReference = (ICachedVolumeReference) Parent.Slice.VolumeReference.Clone();
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && _cachedVolumeReference != null)
				{
					_cachedVolumeReference.Dispose();
					_cachedVolumeReference = null;
				}

				base.Dispose(disposing);
			}

			public new AsyncVolumeSliceSopDataSource Parent
			{
				get { return (AsyncVolumeSliceSopDataSource) base.Parent; }
			}

			protected override RegenerationCost RegenerationCost
			{
				get { return (Parent.Slice.VolumeReference is ICachedVolumeReference && !((ICachedVolumeReference) Parent.Slice.VolumeReference).IsLoaded) ? RegenerationCost.High : base.RegenerationCost; }
				set { base.RegenerationCost = value; }
			}

			protected override bool IsReadySynchronously()
			{
				return _cachedVolumeReference == null || _cachedVolumeReference.IsLoaded;
			}

			protected override void LockSource()
			{
				if (_cachedVolumeReference != null)
				{
					lock (_lockSync)
					{
						if (_lockCount++ == 0)
							_cachedVolumeReference.Lock();
					}
				}
			}

			protected override void UnlockSource()
			{
				if (_cachedVolumeReference != null)
				{
					lock (_lockSync)
					{
						if (--_lockCount == 0)
							_cachedVolumeReference.Unlock();
					}
				}
			}

			private volatile ICachedVolumeReference _asyncVolumeLoaderReference;

			protected override void AsyncCreateNormalizedPixelData(Action<byte[], Exception> onComplete)
			{
				// only ICachedVolumeReference provides for dynamic loading of the volume - a standard IVolumeReference is a hard reference to a loaded volume
				if (_cachedVolumeReference == null)
				{
					onComplete.Invoke(Parent.Slice.GetPixelData(), null);
				}
				else if (_asyncVolumeLoaderReference == null)
				{
					var volume = _asyncVolumeLoaderReference = (ICachedVolumeReference) _cachedVolumeReference.Clone();
					volume.Lock();
					if (!volume.IsLoaded)
					{
						volume.ProgressChanged += (s, e) => UpdateProgress((int) volume.Progress, null);
						InitializeProgress((int) volume.Progress);

						var task = volume.LoadAsync();
						if (task == null)
						{
							var pixelData = Parent.Slice.GetPixelData();
							onComplete.Invoke(pixelData, null);
							return;
						}
						task.ContinueWith(t =>
						                  	{
						                  		try
						                  		{
						                  			if (t.IsFaulted && t.Exception != null)
						                  			{
						                  				onComplete.Invoke(null, t.Exception.Flatten().InnerExceptions.FirstOrDefault());
						                  			}
						                  			else
						                  			{
						                  				var pixelData = Parent.Slice.GetPixelData();
						                  				UpdateProgress(100, pixelData);
						                  				onComplete.Invoke(pixelData, null);
						                  			}
						                  		}
						                  		catch (Exception ex)
						                  		{
						                  			onComplete.Invoke(null, ex);
						                  		}
						                  		finally
						                  		{
						                  			volume.Unlock();
						                  			volume.Dispose();

						                  			_asyncVolumeLoaderReference = null;
						                  		}
						                  	});
					}
				}
			}

			protected override byte[] SyncCreateNormalizedPixelData()
			{
				// only ICachedVolumeReference provides for dynamic loading of the volume - a standard IVolumeReference is a hard reference to a loaded volume
				if (_cachedVolumeReference == null)
				{
					return Parent.Slice.GetPixelData();
				}
				else
				{
					using (var volume = (ICachedVolumeReference) _cachedVolumeReference.Clone())
					{
						volume.Lock();
						return Parent.Slice.GetPixelData();
					}
				}
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
			{
				throw new NotSupportedException("MPR Volumes do not support overlays");
			}
		}
	}
}