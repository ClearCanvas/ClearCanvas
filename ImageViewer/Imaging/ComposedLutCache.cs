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
using System.Diagnostics;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal static class ComposedLutCache
	{
		#region Cached Lut

		public interface ICachedLut : IComposedLut, IDisposable {}

		public static ICachedLut GetLut(LutCollection sourceLuts)
		{
			Platform.CheckForNullReference(sourceLuts, "sourceLuts");
			return new CachedLutProxy(sourceLuts);
		}

		private class CachedLutProxy : ICachedLut
		{
			private readonly LutCollection _sourceLuts;
			private CacheItemProxy _cacheItemProxy;

			internal CachedLutProxy(LutCollection sourceLuts)
			{
				_sourceLuts = sourceLuts;
			}

			#region Cache Item Proxy

			private CacheItemProxy CacheItemProxy
			{
				get
				{
					_sourceLuts.SyncMinMaxValues();
					_sourceLuts.Validate();

					if (_cacheItemProxy == null)
					{
						_cacheItemProxy = CreateItemProxy(_sourceLuts);
					}
					else if (_cacheItemProxy.Key != GetKey(_sourceLuts))
					{
						//Trace.WriteLine("Detected cache item key != lut collection key", "LUT");
						_cacheItemProxy.Dispose();
						_cacheItemProxy = CreateItemProxy(_sourceLuts);
					}

					return _cacheItemProxy;
				}
			}

			private void DisposeCacheItemProxy()
			{
				if (_cacheItemProxy != null)
				{
					_cacheItemProxy.Dispose();
					_cacheItemProxy = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					DisposeCacheItemProxy();
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion

			#endregion

			#region IComposedLut Members

			private IComposedLut RealLut
			{
				get { return CacheItemProxy.GetLut(_sourceLuts); }
			}

			public int[] Data
			{
				get { return RealLut.Data; }
			}

			#endregion

			#region ILut Members

			public int MinInputValue
			{
				get { return RealLut.MinInputValue; }
			}

			public int MaxInputValue
			{
				get { return RealLut.MaxInputValue; }
			}

			public int MinOutputValue
			{
				get { return RealLut.MinOutputValue; }
			}

			public int MaxOutputValue
			{
				get { return RealLut.MaxOutputValue; }
			}

			public int this[int index]
			{
				get { return RealLut[index]; }
			}

			#endregion
		}

		#endregion

		#region Cache Item

		private interface ICacheItem : IDisposable
		{
			string Key { get; }
			IComposedLut GetLut(LutCollection sourceLuts);
		}

		#region Cache Item Proxy

		private class CacheItemProxy : ICacheItem
		{
			private ReferenceCountedObjectWrapper<CacheItem> _wrapper;
			private CacheItem _cacheItem;

			internal CacheItemProxy(ReferenceCountedObjectWrapper<CacheItem> wrapper)
			{
				_wrapper = wrapper;
				_cacheItem = _wrapper.Item;
				_wrapper.IncrementReferenceCount();
			}

			internal ReferenceCountedObjectWrapper<CacheItem> Wrapper
			{
				get { return _wrapper; }
			}

			#region IItem Members

			public string Key
			{
				get { return _cacheItem.Key; }
			}

			public IComposedLut GetLut(LutCollection sourceLuts)
			{
				return _cacheItem.GetLut(sourceLuts);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					if (_wrapper == null)
						return;

					_wrapper.DecrementReferenceCount();
					OnProxyDisposed(this);
					_wrapper = null;
					_cacheItem = null;

					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion
		}

		#endregion

		#region Cache Item

		private class CacheItem : ICacheItem, ILargeObjectContainer
		{
			private readonly LargeObjectContainerData _largeObjectData;
			private readonly string _key;
			private readonly BufferCache<int> _bufferCache = SharedBufferCache;
			private readonly BufferCache<double> _doubleBufferCache = SharedDoubleBufferCache;

			private readonly object _syncLock = new object();
			private volatile ComposedLut _realComposedLut;

			internal CacheItem(string key)
			{
				_key = key;
				_largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) {RegenerationCost = LargeObjectContainerData.PresetGeneratedData};
			}

			~CacheItem()
			{
				_containsDeadItems = true;
			}

			public string Key
			{
				get { return _key; }
			}

			public IComposedLut GetLut(LutCollection sourceLuts)
			{
				IComposedLut lut = _realComposedLut;
				if (lut != null)
					return lut;

				lock (_syncLock)
				{
					if (_realComposedLut != null)
						return _realComposedLut;

					//Trace.WriteLine(String.Format("Creating Composed Lut '{0}'", Key), "LUT");

					_realComposedLut = new ComposedLut(sourceLuts.ToArray(), _bufferCache, _doubleBufferCache);

					//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
					_largeObjectData.UpdateLastAccessTime();
					_largeObjectData.BytesHeldCount = _realComposedLut.Data.Length*sizeof (int);
					_largeObjectData.LargeObjectCount = 1;
					MemoryManager.Add(this);
					Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);

					return _realComposedLut;
				}
			}

			private void Unload(bool disposing)
			{
				if (_realComposedLut == null)
					return;

				lock (_syncLock)
				{
					if (_realComposedLut == null)
						return;

					Diagnostics.OnLargeObjectReleased(_largeObjectData.BytesHeldCount);
					//We can't return a buffer to the pool unless we're certain it's not
					//being used anywhere else, which means this cache item must be
					//being disposed.
					if (disposing)
						_bufferCache.Return(_realComposedLut.Data);

					_realComposedLut = null;
					_largeObjectData.BytesHeldCount = 0;
					_largeObjectData.LargeObjectCount = 0;
					MemoryManager.Remove(this);
				}
			}

			#region ILargeObjectContainer Members

			Guid ILargeObjectContainer.Identifier
			{
				get { return _largeObjectData.Identifier; }
			}

			int ILargeObjectContainer.LargeObjectCount
			{
				get { return _largeObjectData.LargeObjectCount; }
			}

			long ILargeObjectContainer.BytesHeldCount
			{
				get { return _largeObjectData.BytesHeldCount; }
			}

			DateTime ILargeObjectContainer.LastAccessTime
			{
				get { return _largeObjectData.LastAccessTime; }
			}

			RegenerationCost ILargeObjectContainer.RegenerationCost
			{
				get { return _largeObjectData.RegenerationCost; }
			}

			bool ILargeObjectContainer.IsLocked
			{
				get { return _largeObjectData.IsLocked; }
			}

			void ILargeObjectContainer.Lock()
			{
				_largeObjectData.Lock();
			}

			void ILargeObjectContainer.Unlock()
			{
				_largeObjectData.Unlock();
			}

			void ILargeObjectContainer.Unload()
			{
				//if (_realComposedLut != null)
				//    Trace.WriteLine(String.Format("Memory Manager unloading Composed Lut '{0}'", Key), "LUT");

				Unload(false);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					//if (_realComposedLut != null) 
					//    Trace.WriteLine(String.Format("Dispose unloading Composed Lut '{0}'", Key), "LUT");

					Unload(true);
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}

			#endregion
		}

		#endregion

		#region Private Fields

		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, WeakReference> _cache = new Dictionary<string, WeakReference>();
		private static volatile bool _containsDeadItems;

		#endregion

		#region Shared Buffer Cache

		private static readonly object _bufferCacheLock = new object();
		private static volatile WeakReference _sharedBufferCache;
		private static volatile WeakReference _sharedDoubleBufferCache;

		private static BufferCache<int> SharedBufferCache
		{
			get
			{
				BufferCache<int> bufferCache = GetSharedBufferCache();
				if (bufferCache == null)
				{
					lock (_bufferCacheLock)
					{
						bufferCache = GetSharedBufferCache();
						if (bufferCache == null)
						{
							//Trace.WriteLine("Creating new ComposedLut.SharedBufferCache", "LUT");
							bufferCache = new BufferCache<int>(10, true);
							_sharedBufferCache = new WeakReference(bufferCache);
						}
					}
				}

				return bufferCache;
			}
		}

		private static BufferCache<int> GetSharedBufferCache()
		{
			if (_sharedBufferCache == null)
				return null;

			BufferCache<int> bufferCache;
			try
			{
				bufferCache = _sharedBufferCache.Target as BufferCache<int>;
			}
			catch (InvalidOperationException)
			{
				bufferCache = null;
			}

			return bufferCache;
		}

		private static BufferCache<double> SharedDoubleBufferCache
		{
			get
			{
				BufferCache<double> bufferCache = GetSharedDoubleBufferCache();
				if (bufferCache == null)
				{
					lock (_bufferCacheLock)
					{
						bufferCache = GetSharedDoubleBufferCache();
						if (bufferCache == null)
						{
							//Trace.WriteLine("Creating new ComposedLut.SharedBufferCache", "LUT");
							bufferCache = new BufferCache<double>(10, true);
							_sharedDoubleBufferCache = new WeakReference(bufferCache);
						}
					}
				}

				return bufferCache;
			}
		}

		private static BufferCache<double> GetSharedDoubleBufferCache()
		{
			if (_sharedDoubleBufferCache == null)
				return null;

			BufferCache<double> bufferCache;
			try
			{
				bufferCache = _sharedDoubleBufferCache.Target as BufferCache<double>;
			}
			catch (InvalidOperationException)
			{
				bufferCache = null;
			}

			return bufferCache;
		}

		#endregion

		#region Private Helper Methods

		private static string GetKey(IEnumerable<IComposableLut> luts)
		{
			return StringUtilities.Combine(luts, "/", lut => lut.GetKey());
		}

		private static void CleanupDeadItems()
		{
			if (!_containsDeadItems)
				return;

			_containsDeadItems = false;

			List<string> deadObjectKeys = new List<string>();
			foreach (KeyValuePair<string, WeakReference> pair in _cache)
			{
				try
				{
					if (!pair.Value.IsAlive || pair.Value.Target == null)
						deadObjectKeys.Add(pair.Key);
				}
				catch (InvalidOperationException)
				{
					deadObjectKeys.Add(pair.Key);
				}
			}

			foreach (string deadObjectKey in deadObjectKeys)
				_cache.Remove(deadObjectKey);

			if (_cache.Count == 0)
				Trace.WriteLine("The composed lut cache is empty.", "LUT");
		}

		private static void OnProxyDisposed(CacheItemProxy cacheItemProxy)
		{
			ReferenceCountedObjectWrapper<CacheItem> wrapper = cacheItemProxy.Wrapper;
			if (!wrapper.IsReferenceCountAboveZero())
			{
				lock (_syncLock)
				{
					//The count could have gone back up
					if (wrapper.IsReferenceCountAboveZero())
						return;

					CacheItem cacheItem = wrapper.Item;
					_cache.Remove(cacheItem.Key);
					cacheItem.Dispose();

					if (_cache.Count == 0)
						Trace.WriteLine("The composed lut cache is empty.", "LUT");
				}
			}
		}

		private static CacheItemProxy CreateItemProxy(IEnumerable<IComposableLut> luts)
		{
			string key = GetKey(luts);

			lock (_syncLock)
			{
				CleanupDeadItems();

				ReferenceCountedObjectWrapper<CacheItem> wrapper;
				if (!_cache.ContainsKey(key))
				{
					wrapper = new ReferenceCountedObjectWrapper<CacheItem>(new CacheItem(key));
					_cache[key] = new WeakReference(wrapper);
				}
				else
				{
					WeakReference reference = _cache[key];
					try
					{
						wrapper = reference.Target as ReferenceCountedObjectWrapper<CacheItem>;
					}
					catch (InvalidOperationException)
					{
						wrapper = null;
					}

					if (wrapper == null)
					{
						wrapper = new ReferenceCountedObjectWrapper<CacheItem>(new CacheItem(key));
						_cache[key] = new WeakReference(wrapper);
					}
				}

				return new CacheItemProxy(wrapper);
			}
		}

		#endregion

		#endregion
	}
}