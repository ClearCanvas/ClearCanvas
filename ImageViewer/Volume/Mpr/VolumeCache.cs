﻿#region License

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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Represents a cached MPR volume.
	/// </summary>
	public interface ICachedVolume
	{
		/// <summary>
		/// Gets a GUID uniquely identifying the cached MPR volume.
		/// </summary>
		/// <remarks>
		/// This identifier GUID will remain consistent for the same set of source frames, even if the volume is unloaded and reloaded by the <see cref="MemoryManager"/>.
		/// </remarks>
		Guid Identifier { get; }

		/// <summary>
		/// Gets the MPR volume, synchronously loading the volume if necessary.
		/// </summary>
		/// <remarks>
		/// Client code should not hold on to the <see cref="Mpr.Volume"/> instance returned by this property.
		/// If a long-term reference is desired, call and store the result from <see cref="CreateReference"/>,
		/// accessing the <see cref="ICachedVolumeReference.Volume"/> property as necessary.
		/// This is important, because the <see cref="MemoryManager"/> may decide to unload the actual volume at any time,
		/// and a direct reference to a specific <see cref="Mpr.Volume"/> can point to a disposed object if held on to
		/// for any significant period of time.
		/// </remarks>
		Volume Volume { get; }

		/// <summary>
		/// Creates a long-term reference to the cached MPR volume.
		/// </summary>
		/// <remarks>
		/// Calling code should ensure that the <see cref="ICachedVolumeReference"/> instance returned by this method is properly disposed.
		/// This will ensure that all resources held by the cache object, including the volume itself as well as the references to the source frames,
		/// can be properly released when no other cache references exist.
		/// </remarks>
		ICachedVolumeReference CreateReference();
	}

	/// <summary>
	/// Represents a reference to a cached MPR volume.
	/// </summary>
	public interface ICachedVolumeReference : ICachedVolume, IVolumeReference
	{
		/// <summary>
		/// Gets the MPR volume, synchronously loading the volume if necessary.
		/// </summary>
		/// <remarks>
		/// Client code should not hold on to the <see cref="Mpr.Volume"/> instance returned by this property.
		/// If a long-term reference is desired, call and store the result from <see cref="CreateReference"/>,
		/// accessing the <see cref="ICachedVolumeReference.Volume"/> property as necessary.
		/// This is important, because the <see cref="MemoryManager"/> may decide to unload the actual volume at any time,
		/// and a direct reference to a specific <see cref="Mpr.Volume"/> can point to a disposed object if held on to
		/// for any significant period of time.
		/// </remarks>
		new Volume Volume { get; }

		/// <summary>
		/// Creates a long-term reference to the cached MPR volume.
		/// </summary>
		/// <remarks>
		/// Calling code should ensure that the <see cref="ICachedVolumeReference"/> instance returned by this method is properly disposed.
		/// This will ensure that all resources held by the cache object, including the volume itself as well as the references to the source frames,
		/// can be properly released when no other cache references exist.
		/// </remarks>
		new ICachedVolumeReference CreateReference();

		/// <summary>
		/// Gets a value indicating whether or not the MPR volume is loaded.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Fired when the value of <see cref="Progress"/> changes.
		/// </summary>
		event EventHandler ProgressChanged;

		/// <summary>
		/// Gets a value between 0 and 100 indicating the loading progress of the MPR volume.
		/// </summary>
		float Progress { get; }

		/// <summary>
		/// Starts loading the MPR volume asynchronously.
		/// </summary>
		/// <param name="onProgress">Optionally specifies a callback method to be called periodically while the loading the volume.</param>
		/// <param name="onComplete">Optionally specifies a callback method to be called when the volume has been successfully loaded.</param>
		/// <param name="onError">Optionally specifies a callback method to be called if an exception was thrown while loading the volume.</param>
		/// <returns>Returns an <see cref="IAsyncResult"/> which can be used to wait for the MPR volume to finish loading.</returns>
		IAsyncResult LoadAsync(VolumeLoadProgressCallback onProgress = null, VolumeLoadCompletionCallback onComplete = null, VolumeLoadErrorCallback onError = null);

		/// <summary>
		/// Loads the MPR volume synchronously.
		/// </summary>
		/// <param name="onProgress">Optionally specifies a callback method to be called periodically while the loading the volume.</param>
		void Load(VolumeLoadProgressCallback onProgress = null);

		/// <summary>
		/// Locks the MPR volume, preventing it from being unloaded by memory management.
		/// </summary>
		/// <remarks>
		/// The lock will be automatically released if the reference is disposed.
		/// </remarks>
		void Lock();

		/// <summary>
		/// Unlocks the MPR volume, allowing memory management to unload it as necessary.
		/// </summary>
		void Unlock();
	}

	/// <summary>
	/// Represents the callback method to be called periodically while the loading the volume.
	/// </summary>
	/// <param name="volume">A reference to the volume being loaded.</param>
	/// <param name="completedOperations">The number of suboperations completed.</param>
	/// <param name="totalOperations">The total number of suboperations.</param>
	public delegate void VolumeLoadProgressCallback(ICachedVolume volume, int completedOperations, int totalOperations);

	/// <summary>
	/// Represents the callback method to be called when the volume has been successfully loaded.
	/// </summary>
	/// <param name="volume">A reference to the loaded volume.</param>
	public delegate void VolumeLoadCompletionCallback(ICachedVolume volume);

	/// <summary>
	/// Represents the callback method to be called if an exception was thrown while loading the volume.
	/// </summary>
	/// <param name="volume">A reference to the failed volume.</param>
	/// <param name="ex">The exception that was thrown.</param>
	public delegate void VolumeLoadErrorCallback(ICachedVolume volume, Exception ex);

	/// <summary>
	/// Implementation of a memory-managed MPR volume cache.
	/// </summary>
	/// <remarks>
	/// The <see cref="ICachedVolumeReference"/> items returned by this cache are container objects that hold the MPR volume,
	/// references to the source frames, as well as implement memory management. Direct references to this object
	/// (and the actual MPR Volume exposed by <see cref="ICachedVolumeReference.Volume"/>) should not be held on to by client code
	/// for any significant period of time, as the underlying instance of <see cref="Mpr.Volume"/> may be disposed of
	/// at any time by the <see cref="MemoryManager"/>. Instead, if a long-term reference is desired, create a reference
	/// with <see cref="ICachedVolumeReference.CreateReference"/>. When all outstanding references to the <see cref="ICachedVolumeReference"/>
	/// have been disposed, the item will itself be removed from the cache, releasing the references to the source frames
	/// as well as the <see cref="Mpr.Volume"/> instance.
	/// </remarks>
	public sealed class VolumeCache : IDisposable
	{
		/// <summary>
		/// Gets an instance of a <see cref="VolumeCache"/> whose lifetime is tied to a specific <see cref="IImageViewer"/> instance.
		/// </summary>
		public static VolumeCache GetInstance(IImageViewer imageViewer)
		{
			return imageViewer.GetVolumeCache();
		}

		private readonly object _syncRoot = new object();
		private Dictionary<CacheKey, CachedVolume> _cache;

		/// <summary>
		/// Initializes a new instance of <see cref="VolumeCache"/>.
		/// </summary>
		public VolumeCache()
		{
			_cache = new Dictionary<CacheKey, CachedVolume>();
		}

		/// <summary>
		/// Disposes the <see cref="VolumeCache"/>.
		/// </summary>
		public void Dispose()
		{
			// do not forcibly dispose the cached volumes, as things like clipboard items may still hold references to cached volumes
			_cache = null;
		}

		/// <summary>
		/// Creates a reference to a cached MPR volume based on the specified source display set.
		/// </summary>
		public ICachedVolumeReference GetVolumeReference(IDisplaySet displaySet)
		{
			return CreateVolumeCore(displaySet.PresentationImages.Cast<IImageSopProvider>().Select(i => i.Frame).ToList());
		}

		/// <summary>
		/// Creates a reference to a cached MPR volume based on the specified source frames.
		/// </summary>
		/// <param name="frames">References to the source frames from which to create an MPR volume. This method does not take ownership of the specified frame references.</param>
		public ICachedVolumeReference GetVolumeReference(IEnumerable<IFrameReference> frames)
		{
			return CreateVolumeCore(frames.Select(f => f.Frame).ToList());
		}

		/// <summary>
		/// Creates a reference to a cached MPR volume based on the specified source frames.
		/// </summary>
		public ICachedVolumeReference GetVolumeReference(IEnumerable<Frame> frames)
		{
			return CreateVolumeCore(frames.ToList());
		}

		private ICachedVolumeReference CreateVolumeCore(IList<Frame> frames)
		{
			var cacheKey = new CacheKey(frames);
			lock (_syncRoot)
			{
				CachedVolume cachedItem;
				if (!_cache.TryGetValue(cacheKey, out cachedItem) || cachedItem.IsDisposed)
					_cache[cacheKey] = cachedItem = new CachedVolume(this, cacheKey, frames);
				return cachedItem.CreateReference(); // always return a new counted reference to the cache item
			}
		}

		private void RemoveVolumeCore(CacheKey cacheKey, CachedVolume cachedItem)
		{
			lock (_syncRoot)
			{
				// double check identity of item being removed, in case it's already been recreated before the previous item's dispose finishes
				CachedVolume realItem;
				if (_cache.TryGetValue(cacheKey, out realItem) && ReferenceEquals(realItem, cachedItem))
					_cache.Remove(cacheKey);
			}
		}

		#region Unit Test Support

#if UNIT_TESTS

		public int Count
		{
			get { return _cache.Count; }
		}

		public bool IsCached(IDisplaySet displaySet)
		{
			return IsCached(displaySet.PresentationImages.Cast<IImageSopProvider>().Select(i => i.Frame).ToList());
		}

		public bool IsCached(IEnumerable<Frame> frames)
		{
			return _cache.ContainsKey(new CacheKey(frames.ToList()));
		}

#endif

		#endregion

		/// <summary>
		/// Cache item acting as a container for the volume and source frames.
		/// </summary>
		private class CachedVolume : ICachedVolume, ILargeObjectContainer
		{
			private readonly object _syncRoot = new object();
			private readonly CacheKey _cacheKey;
			private readonly VolumeCache _cacheOwner;
			private IList<IFrameReference> _frames;
			private IVolumeReference _volumeReference;
			private bool _isDisposed = false;

			private event EventHandler _progressChanged;
			private volatile float _progress = 0;

			public CachedVolume(VolumeCache cacheOwner, CacheKey cacheKey, IEnumerable<Frame> frames)
			{
				_cacheOwner = cacheOwner;
				_cacheKey = cacheKey;
				_frames = frames.Select(f => f.CreateTransientReference()).ToList();

				Volume.Validate(_frames);
			}

			/// <summary>
			/// Called when all references to the cached item are destroyed, and thus all held source frames and volume can be released.
			/// </summary>
			private void Dispose()
			{
				MemoryManager.Remove(this);

				if (_volumeReference != null)
				{
					_volumeReference.Dispose();
					_volumeReference = null;
				}

				if (_frames != null)
				{
					foreach (var frameReference in _frames)
						frameReference.Dispose();
					_frames.Clear();
					_frames = null;
				}
			}

			public Guid Identifier
			{
				get { return _cacheKey.Guid; }
			}

			public Volume Volume
			{
				get
				{
					AssertNotDisposed();
					_largeObjectContainerData.UpdateLastAccessTime();
					return LoadCore(null);
				}
			}

			private bool IsLoaded
			{
				get
				{
					AssertNotDisposed();
					return _volumeReference != null;
				}
			}

			private void Load(VolumeLoadProgressCallback callback = null)
			{
				AssertNotDisposed();
				LoadCore(callback);
			}

			private float Progress
			{
				get { return _progress; }
				set
				{
					_progress = value;
					EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
				}
			}

			private Volume LoadCore(VolumeLoadProgressCallback callback)
			{
				if (_volumeReference != null) return _volumeReference.Volume;

				lock (_syncRoot)
				{
					if (_volumeReference != null) return _volumeReference.Volume;

					Progress = 0;

					using (var volume = Volume.Create(_frames, (n, total) =>
					                                           	{
					                                           		Progress = Math.Min(100f, 100f*n/total);
					                                           		if (callback != null) callback.Invoke(this, n, total);
					                                           	}))
					{
						_volumeReference = volume.CreateTransientReference();

						_largeObjectContainerData.LargeObjectCount = 1;
						_largeObjectContainerData.BytesHeldCount = 2*volume.SizeInVoxels;
						_largeObjectContainerData.UpdateLastAccessTime();
						MemoryManager.Add(this);
					}

					Progress = 100f;

					return _volumeReference.Volume;
				}
			}

			public void Unload()
			{
				AssertNotDisposed();

				if (_volumeReference == null) return;

				lock (_syncRoot)
				{
					if (_volumeReference == null) return;

					Progress = 0;

					MemoryManager.Remove(this);
					_largeObjectContainerData.LargeObjectCount = 0;
					_largeObjectContainerData.BytesHeldCount = 0;

					// in general, this would be the only transient reference to the volume
					// we can't stop external code from calling CreateTransientReference() too
					// but if they did, the volume wouldn't really release here anyway, so that external code would still work
					_volumeReference.Dispose();
					_volumeReference = null;
				}
			}

			private void AssertNotDisposed()
			{
				if (_isDisposed)
					throw new ObjectDisposedException(typeof (CachedVolume).FullName, "Cached volume has already been disposed!");
			}

			#region Asynchronous Loader

			private readonly object _backgroundLoadSyncRoot = new object();
			private Action _backgroundLoadMethod;
			private IAsyncResult _backgroundLoadMethodAsyncResult;

			private IAsyncResult LoadAsync(VolumeLoadProgressCallback onProgress = null, VolumeLoadCompletionCallback onComplete = null, VolumeLoadErrorCallback onError = null)
			{
				AssertNotDisposed();

				if (_volumeReference != null) return null;
				if (_backgroundLoadMethod != null) return _backgroundLoadMethodAsyncResult;

				lock (_backgroundLoadSyncRoot)
				{
					if (_volumeReference != null) return null;
					if (_backgroundLoadMethod != null) return _backgroundLoadMethodAsyncResult;

					_backgroundLoadMethod = () =>
					                        	{
					                        		try
					                        		{
					                        			LoadCore(onProgress);

					                        			if (onComplete != null)
					                        				onComplete.Invoke(this);
					                        		}
					                        		catch (Exception ex)
					                        		{
					                        			if (onError != null)
					                        				onError.Invoke(this, ex);
					                        			else
					                        				Platform.Log(LogLevel.Debug, ex, "Unhandled exception thrown in asynchronous volume loader");
					                        		}
					                        	};
					return _backgroundLoadMethodAsyncResult = _backgroundLoadMethod.BeginInvoke(ar =>
					                                                                            	{
					                                                                            		_backgroundLoadMethod.EndInvoke(ar);
					                                                                            		_backgroundLoadMethod = null;
					                                                                            		_backgroundLoadMethodAsyncResult = null;
					                                                                            	}, null);
				}
			}

			#endregion

			#region CachedVolume References

			private readonly object _referenceSyncRoot = new object();
			private int _referenceCount = 0;

			public ICachedVolumeReference CreateReference()
			{
				AssertNotDisposed();
				return new CachedVolumeReference(this);
			}

			public bool IsDisposed
			{
				get
				{
					lock (_referenceSyncRoot)
					{
						return _isDisposed;
					}
				}
			}

			private void IncrementReferenceCount()
			{
				lock (_referenceSyncRoot)
				{
					++_referenceCount;
				}
			}

			private void DecrementReferenceCount()
			{
				lock (_referenceSyncRoot)
				{
					--_referenceCount;

					if (_referenceCount == 0)
					{
						// we don't want to block too long especially if the calling code is disposing the LAST reference
						// so just mark this as disposed and queue up a task to actually perform uncaching and disposal
						// no one else should have a reference to this anyway, and the cache checks the disposed property too
						_isDisposed = true;
						ThreadPool.QueueUserWorkItem(s =>
						                             	{
						                             		_cacheOwner.RemoveVolumeCore(_cacheKey, this);
						                             		Dispose();
						                             	}, null);
					}
				}
			}

			private class CachedVolumeReference : ICachedVolumeReference
			{
				private CachedVolume _cachedVolume;
				private bool _locked;

				public CachedVolumeReference(CachedVolume cachedVolume)
				{
					cachedVolume.IncrementReferenceCount();
					_cachedVolume = cachedVolume;
					_cachedVolume._progressChanged += CachedVolumeOnProgressChanged;
				}

				public void Dispose()
				{
					if (_cachedVolume != null)
					{
						if (_locked) _cachedVolume.Unlock();
						_cachedVolume._progressChanged -= CachedVolumeOnProgressChanged;
						_cachedVolume.DecrementReferenceCount();
						_cachedVolume = null;
					}
				}

				private void CachedVolumeOnProgressChanged(object sender, EventArgs eventArgs)
				{
					EventsHelper.Fire(ProgressChanged, this, eventArgs);
				}

				public Guid Identifier
				{
					get { return _cachedVolume.Identifier; }
				}

				public Volume Volume
				{
					get { return _cachedVolume.Volume; }
				}

				public event EventHandler ProgressChanged;

				public float Progress
				{
					get { return _cachedVolume.Progress; }
				}

				public bool IsLoaded
				{
					get { return _cachedVolume.IsLoaded; }
				}

				public IAsyncResult LoadAsync(VolumeLoadProgressCallback onProgress = null, VolumeLoadCompletionCallback onComplete = null, VolumeLoadErrorCallback onError = null)
				{
					return _cachedVolume.LoadAsync(onProgress, onComplete, onError);
				}

				public void Load(VolumeLoadProgressCallback callback = null)
				{
					_cachedVolume.Load(callback);
				}

				public void Lock()
				{
					if (!_locked) _cachedVolume.Lock();
					_locked = true;
				}

				public void Unlock()
				{
					if (_locked) _cachedVolume.Unlock();
					_locked = false;
				}

				public ICachedVolumeReference CreateReference()
				{
					return new CachedVolumeReference(_cachedVolume);
				}

				IVolumeReference IVolumeReference.Clone()
				{
					return CreateReference();
				}

				public override int GetHashCode()
				{
					return 0;
				}

				private bool Equals(CachedVolumeReference other)
				{
					return ReferenceEquals(_cachedVolume, other._cachedVolume);
				}

				public override bool Equals(object obj)
				{
					return obj is CachedVolumeReference && Equals((CachedVolumeReference) obj);
				}
			}

			#endregion

			#region Implementation of ILargeObjectContainer

			private readonly LargeObjectContainerData _largeObjectContainerData = new LargeObjectContainerData(Guid.NewGuid()) {RegenerationCost = RegenerationCost.Medium};

			Guid ILargeObjectContainer.Identifier
			{
				get { return _largeObjectContainerData.Identifier; }
			}

			public int LargeObjectCount
			{
				get { return _largeObjectContainerData.LargeObjectCount; }
			}

			public long BytesHeldCount
			{
				get { return _largeObjectContainerData.BytesHeldCount; }
			}

			public DateTime LastAccessTime
			{
				get { return _largeObjectContainerData.LastAccessTime; }
			}

			public RegenerationCost RegenerationCost
			{
				get { return _largeObjectContainerData.RegenerationCost; }
			}

			public bool IsLocked
			{
				get { return _largeObjectContainerData.IsLocked; }
			}

			public void Lock()
			{
				_largeObjectContainerData.Lock();
			}

			public void Unlock()
			{
				_largeObjectContainerData.Unlock();
			}

			#endregion
		}

		private struct CacheKey : IEquatable<CacheKey>
		{
			private readonly Guid _hash;

			public CacheKey(IList<Frame> frames)
				: this()
			{
				using (var md5 = new MD5CryptoServiceProvider())
				using (var stream = new MemoryStream())
				using (var writer = new StreamWriter(stream))
				{
					var firstFrame = frames.FirstOrDefault();
					if (firstFrame != null)
					{
						writer.WriteLine(firstFrame.ParentImageSop.PatientId);
						writer.WriteLine(firstFrame.ParentImageSop.PatientsName);
						writer.WriteLine(firstFrame.ParentImageSop.StudyInstanceUid);
						writer.WriteLine(firstFrame.ParentImageSop.SeriesInstanceUid);
						foreach (var f in frames)
							writer.WriteLine("{0}:{1}", f.SopInstanceUid, f.FrameNumber);
					}

					stream.Position = 0;
					_hash = new Guid(md5.ComputeHash(stream));
				}
			}

			public Guid Guid
			{
				get { return _hash; }
			}

			public override int GetHashCode()
			{
				return 0x3351E935 ^ _hash.GetHashCode();
			}

			public bool Equals(CacheKey other)
			{
				return _hash.Equals(other._hash);
			}

			public override bool Equals(object obj)
			{
				return obj is CacheKey && Equals((CacheKey) obj);
			}

			public override string ToString()
			{
				return _hash.ToString();
			}
		}
	}
}