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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public static class AlphaColorMapFactory
	{
	    // TODO (CR Apr 2013): I'm not sure this is necessary, and it causes basically a permanent memory leak.
		private static readonly Dictionary<ICachedColorMapKey, CacheItem> _cache = new Dictionary<ICachedColorMapKey, CacheItem>();
		private static readonly object _syncLock = new object();

		public static IColorMap GetColorMap(string colorMapName, byte alpha, bool thresholding)
		{
			return new CachedColorMapProxy(new AlphaColorMapKey(colorMapName, alpha, thresholding));
		}

		public static IColorMap GetColorMap(IColorMap colorMap, byte alpha, bool thresholding)
		{
			return new CachedColorMapProxy(new CustomAlphaColorMapKey(colorMap, alpha, thresholding));
		}

		private static CacheItem GetColorMapCacheItem(ICachedColorMapKey key)
		{
			lock (_syncLock)
			{
				if (_cache.ContainsKey(key))
					return _cache[key];
				CacheItem item = new CacheItem(key);
				_cache[key] = item;
				return item;
			}
		}

		#region ICachedColorMapKey Interface

		private interface ICachedColorMapKey
		{
			IColorMap CreateColorMap();
		}

		#endregion

		#region AlphaColorMapKey Class

		[Cloneable(true)]
		private class AlphaColorMapKey : ICachedColorMapKey, IEquatable<AlphaColorMapKey>
		{
			private readonly string _colorMapName;
			private readonly byte _alpha;
			private readonly bool _thresholding;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private AlphaColorMapKey() {}

			public AlphaColorMapKey(string colorMapName, byte alpha, bool thresholding)
				: this()
			{
				_colorMapName = colorMapName;
				_alpha = alpha;
				_thresholding = thresholding;
			}

			public IColorMap CreateColorMap()
			{
				IColorMap baseColorMap;
				if (_colorMapName == HotIronColorMapFactory.ColorMapName)
				{
					baseColorMap = new HotIronColorMapFactory().Create();
				}
				else
				{
					using (LutFactory lutFactory = LutFactory.Create())
					{
						baseColorMap = lutFactory.GetColorMap(_colorMapName);
					}
				}

				return new AlphaColorMap(baseColorMap, _alpha, _thresholding);
			}

			public override int GetHashCode()
			{
				return 0x15BDF4E1 ^ _colorMapName.GetHashCode() ^ _alpha.GetHashCode() ^ _thresholding.GetHashCode();
			}

			public bool Equals(AlphaColorMapKey other)
			{
				return _colorMapName.Equals(other._colorMapName) && _alpha.Equals(other._alpha) && _thresholding.Equals(other._thresholding);
			}

			public override bool Equals(object obj)
			{
				if (obj is AlphaColorMapKey)
					return Equals((AlphaColorMapKey) obj);
				return false;
			}

			public override string ToString()
			{
				return String.Format("{0}[alpha={1},thresholding={2}]", _colorMapName, _alpha, _thresholding ? 1 : 0);
			}
		}

		#endregion

		#region CustomAlphaColorMapKey Class

		[Cloneable(true)]
		private class CustomAlphaColorMapKey : ICachedColorMapKey, IEquatable<CustomAlphaColorMapKey>
		{
			private readonly IColorMap _colorMap;
			private readonly byte _alpha;
			private readonly bool _thresholding;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private CustomAlphaColorMapKey() {}

			public CustomAlphaColorMapKey(IColorMap colorMap, byte alpha, bool thresholding)
				: this()
			{
				_colorMap = colorMap;
				_alpha = alpha;
				_thresholding = thresholding;
			}

			public IColorMap CreateColorMap()
			{
				return new AlphaColorMap(_colorMap, _alpha, _thresholding);
			}

			public override int GetHashCode()
			{
				return 0x15BDF4E1 ^ _colorMap.GetHashCode() ^ _alpha.GetHashCode() ^ _thresholding.GetHashCode();
			}

			public bool Equals(CustomAlphaColorMapKey other)
			{
				return _colorMap.Equals(other._colorMap) && _alpha.Equals(other._alpha) && _thresholding.Equals(other._thresholding);
			}

			public override bool Equals(object obj)
			{
				if (obj is CustomAlphaColorMapKey)
					return Equals((CustomAlphaColorMapKey) obj);
				return false;
			}

			public override string ToString()
			{
				return String.Format("Custom[real={0},alpha={1},thresholding={2}]", _colorMap, _alpha, _thresholding ? 1 : 0);
			}
		}

		#endregion

		#region CachedColorMapProxy Class

		[Cloneable(true)]
		private class CachedColorMapProxy : ColorMapBase
		{
			private readonly ICachedColorMapKey _colorMapKey;
			private int _minInputValue;
			private int _maxInputValue;

			[CloneIgnore]
			private CacheItem _cacheItem;

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private CachedColorMapProxy() {}

			public CachedColorMapProxy(ICachedColorMapKey colorMapKey)
				: this()
			{
				_colorMapKey = colorMapKey;
			}

			private IColorMap RealColorMap
			{
				get
				{
					if (_cacheItem == null)
						_cacheItem = GetColorMapCacheItem(new FullColorMapKey(_colorMapKey, _minInputValue, _maxInputValue));
					return _cacheItem.RealColorMap;
				}
			}

			public override int MinInputValue
			{
				get { return _minInputValue; }
				set
				{
					if (value == _minInputValue)
						return;

					_cacheItem = null;
					_minInputValue = value;
					OnLutChanged();
				}
			}

			public override int MaxInputValue
			{
				get { return _maxInputValue; }
				set
				{
					if (value == _maxInputValue)
						return;

					_cacheItem = null;
					_maxInputValue = value;
					OnLutChanged();
				}
			}

			public override int this[int index]
			{
				get { return RealColorMap[index]; }
				protected set { throw new InvalidOperationException("The color map data cannot be altered."); }
			}

			public override string GetKey()
			{
				return RealColorMap.GetKey();
			}

			public override string GetDescription()
			{
				return RealColorMap.GetDescription();
			}

			#region IDataLut Members

			public override int FirstMappedPixelValue
			{
				get { return RealColorMap.FirstMappedPixelValue; }
			}

			public override int[] Data
			{
				get { return RealColorMap.Data; }
			}

			#endregion

			#region IMemorable Members

			public override object CreateMemento()
			{
				//no state to remember, but we do want to remove the reference to the 'real lut'.  It will be recreated later.
				_cacheItem = null;
				return base.CreateMemento();
			}

			#endregion

			#region FullColorMapKey<T> Class

			[Cloneable(true)]
			private class FullColorMapKey : ICachedColorMapKey, IEquatable<FullColorMapKey>
			{
				private readonly ICachedColorMapKey _colorMapKey;
				private readonly int _minInputValue;
				private readonly int _maxInputValue;

				/// <summary>
				/// Cloning constructor.
				/// </summary>
				private FullColorMapKey() {}

				public FullColorMapKey(ICachedColorMapKey colorMapKey, int minInputValue, int maxInputValue)
					: this()
				{
					_colorMapKey = colorMapKey;
					_minInputValue = minInputValue;
					_maxInputValue = maxInputValue;
				}

				public IColorMap CreateColorMap()
				{
					var colorMap = _colorMapKey.CreateColorMap();
					colorMap.MinInputValue = _minInputValue;
					colorMap.MaxInputValue = _maxInputValue;
					return colorMap;
				}

				public override int GetHashCode()
				{
					return 0x152D6351 ^ _colorMapKey.GetHashCode() ^ _minInputValue.GetHashCode() ^ _maxInputValue.GetHashCode();
				}

				public bool Equals(FullColorMapKey other)
				{
					return _colorMapKey.Equals(other._colorMapKey) && _minInputValue.Equals(other._minInputValue) && _maxInputValue.Equals(other._maxInputValue);
				}

				public override bool Equals(object obj)
				{
					if (obj is FullColorMapKey)
						return Equals((FullColorMapKey) obj);
					return false;
				}

				public override string ToString()
				{
					return string.Format("{0}[RangeIn:{1},{2}]", _colorMapKey, _minInputValue, _maxInputValue);
				}
			}

			#endregion
		}

		#endregion

		#region CacheItem Class

		private class CacheItem : ILargeObjectContainer
		{
			private readonly object _syncLock = new object();
			private readonly LargeObjectContainerData _largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) {RegenerationCost = LargeObjectContainerData.PresetGeneratedData};
			private volatile IColorMap _realColorMap;

			private readonly ICachedColorMapKey _key;

			internal CacheItem(ICachedColorMapKey key)
			{
				_key = key;
			}

			public override string ToString()
			{
				return _key.ToString();
			}

			public IColorMap RealColorMap
			{
				get
				{
					IColorMap realLut = _realColorMap;
					if (realLut != null)
						return realLut;

					lock (_syncLock)
					{
						if (_realColorMap != null)
							return _realColorMap;

						var source = _key.CreateColorMap();
						_realColorMap = new SimpleColorMap(source.MinInputValue, source.Data, source.GetKey(), source.GetDescription());

						//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
						_largeObjectData.UpdateLastAccessTime();
						_largeObjectData.BytesHeldCount = _realColorMap.Data.Length*sizeof (int);
						_largeObjectData.LargeObjectCount = 1;

						MemoryManager.Add(this);
						Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);
						return _realColorMap;
					}
				}
			}

			public void Unload()
			{
				if (_realColorMap == null)
					return;

				lock (_syncLock)
				{
					if (_realColorMap == null)
						return;
					_realColorMap = null;

					Diagnostics.OnLargeObjectReleased(_largeObjectData.BytesHeldCount);
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

			#endregion
		}

		#endregion
	}
}