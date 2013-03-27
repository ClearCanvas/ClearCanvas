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
using System.Threading.Tasks;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Base implementation of a <see cref="SopDataSource"/> with pixel data that must be loaded and refined asynchronously.
	/// </summary>
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public abstract class AsyncSopDataSource : SopDataSource
	{
		/// <summary>
		/// Gets a lock object suitable for synchronizing access to the data source.
		/// </summary>
		protected readonly object SyncLock = new object();

		private volatile ISopFrameData[] _frameData;

		/// <summary>
		/// Constructs a new <see cref="AsyncSopDataSource"/>.
		/// </summary>
		protected AsyncSopDataSource() {}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image.
		/// </summary>
		public override bool IsImage
		{
			get { return _frameData != null || ImageSop.IsSupportedSopClass(SopClassUid); }
		}

		/// <summary>
		/// Called by the base class to create a new <see cref="AsyncSopFrameData"/> containing the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>A new <see cref="AsyncSopFrameData"/> containing the data for a particular frame in the SOP instance.</returns>
		protected abstract AsyncSopFrameData CreateFrameData(int frameNumber);

		/// <summary>
		/// Gets the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>An <see cref="ISopFrameData"/> containing frame-specific data.</returns>
		protected override ISopFrameData GetFrameData(int frameNumber)
		{
			if (_frameData == null)
			{
				lock (SyncLock)
				{
					if (_frameData == null)
					{
						var frameData = new ISopFrameData[NumberOfFrames];
						for (int n = 0; n < frameData.Length; n++)
							frameData[n] = CreateFrameData(n + 1);

						_frameData = frameData;
					}
				}
			}

			return _frameData[frameNumber - 1];
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				ISopFrameData[] frameData;
				lock (SyncLock)
				{
					frameData = _frameData;
					_frameData = null;
				}

				if (frameData != null)
				{
					foreach (var frame in frameData)
						frame.Dispose();
				}
			}
		}

		#region AsyncSopFrameData class

		/// <summary>
		/// Base implementation of a <see cref="SopFrameData"/> with pixel data that must be loaded and refined asynchronously.
		/// </summary>
		protected abstract class AsyncSopFrameData : SopFrameData, IAsyncSopFrameData, ILargeObjectContainer
		{
			/// <summary>
			/// Gets a lock object suitable for synchronizing access to the frame data.
			/// </summary>
			protected readonly object SyncLock = new object();

			private readonly Dictionary<int, byte[]> _overlayData = new Dictionary<int, byte[]>(16);
			private volatile byte[] _pixelData = null;
			private volatile Task<byte[]> _task;

			private readonly LargeObjectContainerData _largeObjectContainerData = new LargeObjectContainerData(Guid.NewGuid());

			/// <summary>
			/// Constructs a new <see cref="AsyncSopFrameData"/>
			/// </summary>
			/// <param name="frameNumber">The 1-based number of this frame.</param>
			/// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
			protected AsyncSopFrameData(int frameNumber, AsyncSopDataSource parent)
				: this(frameNumber, parent, RegenerationCost.Medium) {}

			/// <summary>
			/// Constructs a new <see cref="AsyncSopFrameData"/>.
			/// </summary>
			/// <param name="frameNumber">The 1-based number of this frame.</param>
			/// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
			/// <param name="regenerationCost">The approximate cost to regenerate the pixel and/or overlay data.</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
			protected AsyncSopFrameData(int frameNumber, AsyncSopDataSource parent, RegenerationCost regenerationCost)
				: base(frameNumber, parent)
			{
				_largeObjectContainerData.RegenerationCost = regenerationCost;
			}

			/// <summary>
			/// Gets the parent <see cref="AsyncSopDataSource"/> to which this frame belongs.
			/// </summary>
			public new AsyncSopDataSource Parent
			{
				get { return (AsyncSopDataSource) base.Parent; }
			}

			protected byte[] PixelData
			{
				get { return _pixelData; }
				set { _pixelData = value; }
			}

			/// <summary>
			/// Gets or sets the approximate cost to regenerate the pixel and/or overlay data.
			/// </summary>
			protected RegenerationCost RegenerationCost
			{
				get { return _largeObjectContainerData.RegenerationCost; }
				set { _largeObjectContainerData.RegenerationCost = value; }
			}

			/// <summary>
			/// Gets pixel data in normalized form (8 or 16-bit grayscale, or ARGB).
			/// </summary>
			/// <returns></returns>
			/// <remarks>
			/// <i>Normalized</i> pixel data means that:
			/// <list type="Bullet">
			/// <item>
			/// <description>Grayscale pixel data has embedded overlays removed and each pixel value
			/// is padded so that it can be cast directly to the appropriate type (e.g. byte, sbyte, ushort, short).</description>
			/// </item>
			/// <item>
			/// <description>Colour pixel data is always converted into ARGB format.</description>
			/// </item>
			/// <item>
			/// <description>Pixel data is always uncompressed.</description>
			/// </item>
			/// </list>
			/// <para>
			/// Ensuring that the pixel data always meets the above criteria
			/// allows clients to easily consume pixel data without having
			/// to worry about the the multitude of DICOM photometric interpretations
			/// and transfer syntaxes.
			/// </para>
			/// <para>
			/// Pixel data is reloaded when this method is called after a 
			/// call to <see cref="ISopFrameData.Unload"/>.
			/// </para>
			/// </remarks>		
			public override byte[] GetNormalizedPixelData()
			{
				_largeObjectContainerData.UpdateLastAccessTime();

				var pixelData = _pixelData;
				if (pixelData == null && _task == null)
				{
					lock (SyncLock)
					{
						pixelData = _pixelData;
						if (pixelData == null && _task == null)
						{
							try
							{
								var task = _task = new Task<byte[]>(CreateNormalizedPixelData);
								task.ContinueWith(t =>
								                  	{
								                  		try
								                  		{
								                  			var pd = _pixelData = t.Result;
								                  			if (pd != null)
								                  			{
								                  				UpdateLargeObjectInfo();
								                  				MemoryManager.Add(this);
								                  				Diagnostics.OnLargeObjectAllocated(pd.Length);
								                  				UpdateProgress(100, true, pd);
								                  			}
								                  		}
								                  		catch (Exception ex)
								                  		{
								                  			Platform.Log(LogLevel.Error, ex, "Encountered error while asynchronously loading SOP frame data.");
								                  		}
								                  		_task = null;
								                  	});
								UpdateProgress(0, false, null);
								task.Start();
								task.Wait(150);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Error, ex, "Encountered error while asynchronously loading SOP frame data.");
							}
							pixelData = _pixelData;
						}
					}
				}

				return pixelData;
			}

			private void UpdateLargeObjectInfo()
			{
				if (_pixelData == null)
				{
					_largeObjectContainerData.LargeObjectCount = 0;
					_largeObjectContainerData.BytesHeldCount = 0;
				}
				else
				{
					_largeObjectContainerData.LargeObjectCount = 1;
					_largeObjectContainerData.BytesHeldCount = _pixelData.Length;
				}

				_largeObjectContainerData.LargeObjectCount += _overlayData.Count;
				foreach (var pair in _overlayData)
				{
					if (pair.Value != null)
						_largeObjectContainerData.BytesHeldCount += pair.Value.Length;
				}
			}

			/// <summary>
			/// Called by <see cref="GetNormalizedPixelData"/> to create a new byte buffer
			/// containing normalized pixel data for this frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <remarks>
			/// See <see cref="GetNormalizedPixelData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <returns>A new byte buffer containing the normalized pixel data.</returns>
			protected abstract byte[] CreateNormalizedPixelData();

			/// <summary>
			/// Gets the normalized overlay data buffer for a particular overlay group (8-bit grayscale).
			/// </summary>
			/// <remarks>
			/// <para>
			/// <i>Normalized</i> overlay data means that the 1-bit overlay pixel data is extracted and
			/// unpacked as necessary to form an 8-bit-per-pixel buffer with values of either 0 or 255.
			/// </para>
			/// <para>
			/// Ensuring that the overlay data always meets the above criteria allows clients to easily
			/// consume overlay data without having to worry about the storage of overlay data, whether
			/// embedded in unused bits of the pixel data or in a separate packed bits buffer.
			/// </para>
			/// <para>
			/// Overlay data is reloaded when this method is called after a call to <see cref="ISopFrameData.Unload"/>.
			/// The pixel data will also be reloaded if this method is called before
			/// <see cref="ISopFrameData.GetNormalizedPixelData"/> and there are overlays stored in unused bits of the
			/// pixel data.
			/// </para>
			/// </remarks>
			/// <param name="overlayNumber">The 1-based overlay plane number.</param>
			/// <returns>A byte buffer containing the normalized overlay pixel data.</returns>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="overlayNumber"/> is not a positive non-zero number.</exception>
			public override byte[] GetNormalizedOverlayData(int overlayNumber)
			{
				_largeObjectContainerData.UpdateLastAccessTime();

				if (overlayNumber < 1)
					throw new ArgumentOutOfRangeException("overlayNumber", overlayNumber, "Overlay number must be a positive, non-zero number.");

				lock (SyncLock)
				{
					byte[] data;
					if (!_overlayData.TryGetValue(overlayNumber, out data) || data == null)
					{
						_overlayData[overlayNumber] = data = CreateNormalizedOverlayData(overlayNumber);
						if (data != null)
						{
							UpdateLargeObjectInfo();
							MemoryManager.Add(this);
							Diagnostics.OnLargeObjectAllocated(data.Length);
						}
					}

					return data;
				}
			}

			/// <summary>
			/// Called by <see cref="GetNormalizedOverlayData"/> to create a new byte buffer containing normalized 
			/// overlay pixel data for a particular overlay plane.
			/// </summary>
			/// <remarks>
			/// See <see cref="GetNormalizedOverlayData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <param name="overlayNumber">The 1-based overlay plane number.</param>
			/// <returns>A new byte buffer containing the normalized overlay pixel data.</returns>
			protected abstract byte[] CreateNormalizedOverlayData(int overlayNumber);

			/// <summary>
			/// Unloads any cached byte buffers owned by this <see cref="ISopFrameData"/>.
			/// </summary>
			/// <remarks>
			/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
			/// Calling this method will not necessarily result in an immediate decrease in memory
			/// usage, since it merely releases the reference to the pixel data; it is up to the
			/// garbage collector to free the memory.  Calling <see cref="ISopFrameData.GetNormalizedPixelData"/>
			/// will reload the pixel data.
			/// </remarks>
			public override sealed void Unload()
			{
				lock (SyncLock)
				{
					_largeObjectContainerData.UpdateLastAccessTime();

					ReportLargeObjectsUnloaded();

					_pixelData = null;
					_overlayData.Clear();
					OnUnloaded();

					UpdateLargeObjectInfo();
					MemoryManager.Remove(this);
				}
			}

			/// <summary>
			/// Called by the base class when the cached byte buffers are being unloaded.
			/// </summary>
			protected virtual void OnUnloaded()
			{
				UpdateProgress(0, false, null);
			}

			/// <summary>
			/// Called by the base <see cref="SopFrameData"/> to release any owned resources.
			/// </summary>
			/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				if (disposing)
				{
					lock (SyncLock)
					{
						ReportLargeObjectsUnloaded();

						_pixelData = null;
						_overlayData.Clear();

						MemoryManager.Remove(this);
					}
				}
			}

			private void ReportLargeObjectsUnloaded()
			{
				if (_pixelData != null)
					Diagnostics.OnLargeObjectReleased(_pixelData.Length);

				foreach (var overlayData in _overlayData.Values)
				{
					if (overlayData != null)
						Diagnostics.OnLargeObjectReleased(overlayData.Length);
				}
			}

			protected void UpdateProgress(float progressPercent, byte[] pixelData)
			{
				// ensures that the Loaded event only fires once (beacuse we fire it automatically on task completion)
				UpdateProgress(progressPercent, false, pixelData);
			}

			private void UpdateProgress(float progressPercent, bool isComplete, byte[] pixelData)
			{
				ProgressPercent = Math.Min(100, Math.Max(0, progressPercent));
				IsLoaded = isComplete;

				var e = new AsyncPixelDataProgressEventArgs(progressPercent, isComplete, pixelData);
				EventsHelper.Fire(ProgressChanged, this, e);
				if (isComplete) EventsHelper.Fire(Loaded, this, e);
			}

			public float ProgressPercent { get; private set; }

			public event AsyncPixelDataProgressEventHandler ProgressChanged;

			public bool IsLoaded { get; private set; }

			public event AsyncPixelDataEventHandler Loaded;

			#region ILargeObjectContainer Members

			Guid ILargeObjectContainer.Identifier
			{
				get { return _largeObjectContainerData.Identifier; }
			}

			int ILargeObjectContainer.LargeObjectCount
			{
				get { return _largeObjectContainerData.LargeObjectCount; }
			}

			long ILargeObjectContainer.BytesHeldCount
			{
				get { return _largeObjectContainerData.BytesHeldCount; }
			}

			DateTime ILargeObjectContainer.LastAccessTime
			{
				get { return _largeObjectContainerData.LastAccessTime; }
			}

			RegenerationCost ILargeObjectContainer.RegenerationCost
			{
				get { return _largeObjectContainerData.RegenerationCost; }
			}

			bool ILargeObjectContainer.IsLocked
			{
				get { return _largeObjectContainerData.IsLocked; }
			}

			void ILargeObjectContainer.Lock()
			{
				_largeObjectContainerData.Lock();
			}

			void ILargeObjectContainer.Unlock()
			{
				_largeObjectContainerData.Unlock();
			}

			#endregion
		}

		#endregion
	}
}