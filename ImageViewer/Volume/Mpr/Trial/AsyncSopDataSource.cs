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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
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

			private readonly object _progressEventSyncLock = new object();
			private event AsyncPixelDataProgressEventHandler _progressChanged;
			private event AsyncPixelDataEventHandler _loaded;
			private event AsyncPixelDataFaultEventHandler _faulted;
			private volatile bool _isLoading;

			private readonly Dictionary<int, byte[]> _overlayData = new Dictionary<int, byte[]>(16);
			private volatile byte[] _pixelData = null;

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
			protected virtual RegenerationCost RegenerationCost
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
				if (pixelData == null && !_isLoading)
				{
					lock (SyncLock)
					{
						pixelData = _pixelData;
						if (pixelData == null && !_isLoading)
						{
							LockSource();
							try
							{
								if (IsReadySynchronously())
								{
									_pixelData = pixelData = SyncCreateNormalizedPixelData();
									_largeObjectContainerData.UpdateLastAccessTime();

									UpdateLargeObjectInfo();
									MemoryManager.Add(this);

									Diagnostics.OnLargeObjectAllocated(pixelData.Length);
									UpdateProgress(100, true, pixelData, null, false);

									return pixelData;
								}

								_isLoading = true;
								ProgressPercent = 0;
								IsLoaded = false;

								AsyncCreateNormalizedPixelData((pd, e) =>
								                               	{
								                               		try
								                               		{
								                               			_largeObjectContainerData.UpdateLastAccessTime();

								                               			if (pd != null)
								                               			{
								                               				lock (SyncLock)
								                               				{
								                               					Monitor.Pulse(SyncLock);
								                               					_isLoading = false;
								                               					_pixelData = pd;
								                               					UpdateLargeObjectInfo();
								                               					MemoryManager.Add(this);
								                               				}

								                               				Diagnostics.OnLargeObjectAllocated(pd.Length);
								                               			}

								                               			// we have to update progress no matter how it finished (success/failed)
								                               			UpdateProgress(100, true, pd, e, true);
								                               		}
								                               		catch (Exception ex)
								                               		{
								                               			UpdateProgress(100, true, pd, ex, true);
								                               			Platform.Log(LogLevel.Error, ex, "Encountered error while asynchronously loading SOP frame data.");
								                               		}
								                               	});
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Error, ex, "Encountered error while asynchronously loading SOP frame data.");
							}
							finally
							{
								UnlockSource();
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

			protected abstract byte[] SyncCreateNormalizedPixelData();

			/// <summary>
			/// Called by <see cref="GetNormalizedPixelData"/> to start asynchronously creating a new byte buffer
			/// containing normalized pixel data for this frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <remarks>
			/// See <see cref="GetNormalizedPixelData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <returns>A new byte buffer containing the normalized pixel data.</returns>
			protected abstract void AsyncCreateNormalizedPixelData(Action<byte[], Exception> onComplete);

			/// <summary>
			/// Called to determine whether or not the underlying source is ready synchronously
			/// and thus does can be executed on the UI thread.
			/// </summary>
			/// <returns></returns>
			protected virtual bool IsReadySynchronously()
			{
				return false;
			}

			/// <summary>
			/// Called to lock the underlying source, preventing its status from changing due to interaction from other threads.
			/// </summary>
			protected virtual void LockSource() {}

			/// <summary>
			/// Called to release the lock on the underlying source, thus once again allowing interaction from other threads.
			/// </summary>
			protected virtual void UnlockSource() {}

			public IDisposable AcquireLock()
			{
				return new Lock(this);
			}

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
				{
					const string msg = "Overlay number must be a positive, non-zero number.";
					throw new ArgumentOutOfRangeException("overlayNumber", overlayNumber, msg);
				}

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
				_largeObjectContainerData.UpdateLastAccessTime();

				lock (SyncLock)
				{
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
				InitializeProgress(0);
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

			protected void InitializeProgress(int progressPercent)
			{
				UpdateProgress(progressPercent, false, null, null, false);
			}

			protected void UpdateProgress(int progressPercent, byte[] pixelData)
			{
				// ensures that the Loaded event only fires once (beacuse we fire it automatically on task completion)
				UpdateProgress(progressPercent, false, pixelData, null, true);
			}

			private void UpdateProgress(int progressPercent, bool isComplete, byte[] pixelData, Exception exception, bool fireEvents)
			{
				if (!isComplete && progressPercent == ProgressPercent)
					return; // ignore this update request if progress hasn't changed and we're not notifying that loading is complete

				ProgressPercent = Math.Min(100, Math.Max(0, progressPercent));
				IsLoaded = isComplete;
				IsFaulted = isComplete && exception != null;
				LastException = exception;

				if (!fireEvents)
					return;

				lock (_progressEventSyncLock)
				{
					var e = new AsyncPixelDataProgressEventArgs(progressPercent, isComplete, pixelData);
					EventsHelper.Fire(_progressChanged, this, e);
					if (isComplete) EventsHelper.Fire(_loaded, this, e);
					if (isComplete && exception != null) EventsHelper.Fire(_faulted, this, new AsyncPixelDataFaultEventArgs(pixelData, exception));
				}
			}

			public Exception LastException { get; private set; }
			public int ProgressPercent { get; private set; }
			public bool IsLoaded { get; private set; }
			public bool IsFaulted { get; private set; }

			public event AsyncPixelDataProgressEventHandler ProgressChanged
			{
				add { lock (_progressEventSyncLock) _progressChanged += value; }
				remove { lock (_progressEventSyncLock) _progressChanged -= value; }
			}

			public event AsyncPixelDataEventHandler Loaded
			{
				add { lock (_progressEventSyncLock) _loaded += value; }
				remove { lock (_progressEventSyncLock) _loaded -= value; }
			}

			public event AsyncPixelDataFaultEventHandler Faulted
			{
				add { lock (_progressEventSyncLock) _faulted += value; }
				remove { lock (_progressEventSyncLock) _faulted -= value; }
			}

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
				get { return RegenerationCost; }
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

			#region Lock Class

			private class Lock : IDisposable
			{
				private AsyncSopFrameData _owner;

				public Lock(AsyncSopFrameData owner)
				{
					_owner = owner;
					_owner.LockSource();
				}

				public void Dispose()
				{
					if (_owner != null)
					{
						_owner.UnlockSource();
						_owner = null;
					}
				}
			}

			#endregion
		}

		#endregion
	}
}