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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A factory for modality luts and color maps.
	/// </summary>
	public abstract class LutFactory : IDisposable
	{
		private LutFactory() {}

		#region Abstract Methods

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s that describe all the available color maps.
		/// </summary>
		public abstract IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }

		/// <summary>
		/// Factory method for grayscale color maps.
		/// </summary>
		public abstract IColorMap GetGrayscaleColorMap();

		/// <summary>
		/// Factory method that returns a new color map given the name of a <see cref="IColorMapFactory"/>.
		/// </summary>
		public abstract IColorMap GetColorMap(string name);

		/// <summary>
		/// Factory method for linear modality luts.
		/// </summary>
		public abstract IModalityLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept);

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		protected virtual void Dispose(bool disposing) {}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

		private class LutFactoryProxy : LutFactory
		{
			private bool _disposed;

			internal LutFactoryProxy()
			{
				_instance.OnProxyCreated();
			}

			~LutFactoryProxy()
			{
				Dispose(false);
			}

			#region ILutFactory Members

			public override IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get { return _instance.AvailableColorMaps; }
			}

			public override IColorMap GetGrayscaleColorMap()
			{
				return _instance.GetGrayscaleColorMap();
			}

			public override IColorMap GetColorMap(string name)
			{
				return _instance.GetColorMap(name);
			}

			public override IModalityLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
			{
				return _instance.GetModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
			}

			#endregion

			protected override void Dispose(bool disposing)
			{
				if (_disposed)
					return;

				_disposed = true;
				_instance.OnProxyDisposed();
			}
		}

		private abstract class CacheItem<TLut> : ILargeObjectContainer
			where TLut : class
		{
			private readonly LargeObjectContainerData _largeObjectData;

			private readonly object _syncLock = new object();
			private volatile TLut _realLut;

			protected CacheItem()
			{
				_largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) {RegenerationCost = LargeObjectContainerData.PresetGeneratedData};
			}

			protected abstract TLut CreateLut();
			protected abstract int GetSizeInBytes(TLut lut);

			public TLut RealLut
			{
				get
				{
					TLut realLut = _realLut;
					if (realLut != null)
						return realLut;

					lock (_syncLock)
					{
						if (_realLut != null)
							return _realLut;

						_realLut = CreateLut();

						//Trace.WriteLine(String.Format("Creating LUT: {0}", _realLut.GetKey()));

						//just use the creation time as the "last access time", otherwise it can get expensive when called in a tight loop.
						_largeObjectData.UpdateLastAccessTime();
						_largeObjectData.BytesHeldCount = GetSizeInBytes(_realLut);
						_largeObjectData.LargeObjectCount = 1;
						MemoryManager.Add(this);
						Diagnostics.OnLargeObjectAllocated(_largeObjectData.BytesHeldCount);

						return _realLut;
					}
				}
			}

			internal void Unload()
			{
				if (_realLut == null)
					return;

				lock (_syncLock)
				{
					if (_realLut == null)
						return;

					//Trace.WriteLine(String.Format("Unloading LUT: {0}", _realLut.GetKey()));

					_realLut = null;
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

			void ILargeObjectContainer.Unload()
			{
				Unload();
			}

			#endregion
		}

		#region Color Map Classes

		private class ColorMapKey : IEquatable<ColorMapKey>
		{
			internal ColorMapKey(string factoryName, int minInputValue, int maxInputValue)
			{
				FactoryName = factoryName;
				MinInputValue = minInputValue;
				MaxInputValue = maxInputValue;
			}

			public readonly string FactoryName;
			public readonly int MinInputValue;
			public readonly int MaxInputValue;

			public override int GetHashCode()
			{
				return FactoryName.GetHashCode() + 3*MinInputValue.GetHashCode() + 5*MaxInputValue.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is ColorMapKey)
					return Equals((ColorMapKey) obj);

				return false;
			}

			#region IEquatable<ColorMapKey> Members

			public bool Equals(ColorMapKey other)
			{
				return FactoryName.Equals(other.FactoryName) &&
				       MinInputValue.Equals(other.MinInputValue) &&
				       MaxInputValue.Equals(other.MaxInputValue);
			}

			#endregion

			public override string ToString()
			{
				return String.Format("{0}_{1}_{2}", FactoryName, MinInputValue, MaxInputValue);
			}
		}

		private class ColorMapCacheItem : CacheItem<IColorMap>
		{
			private readonly ColorMapKey _key;

			internal ColorMapCacheItem(ColorMapKey key) : base()
			{
				_key = key;
			}

			protected override IColorMap CreateLut()
			{
				IColorMap source = _colorMapFactories[_key.FactoryName].Create();
				source.MinInputValue = _key.MinInputValue;
				source.MaxInputValue = _key.MaxInputValue;

				return new SimpleColorMap(source.MinInputValue, source.Data, source.GetKey(), source.GetDescription());
			}

			protected override int GetSizeInBytes(IColorMap lut)
			{
				return lut.Data.Length*sizeof (int);
			}

			public override string ToString()
			{
				return _key.ToString();
			}
		}

		[Cloneable(true)]
		private class CachedColorMapProxy : ColorMapBase
		{
			private readonly string _factoryName;
			private int _minInputValue;
			private int _maxInputValue;

			[CloneIgnore]
			private ColorMapCacheItem _cacheItem;

			//For cloning.
			private CachedColorMapProxy() {}

			public CachedColorMapProxy(string factoryName)
			{
				_factoryName = factoryName;
			}

			private IColorMap RealColorMap
			{
				get
				{
					if (_cacheItem == null)
						_cacheItem = _instance.GetColorMapCacheItem(new ColorMapKey(_factoryName, _minInputValue, _maxInputValue));

					return _cacheItem.RealLut;
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
		}

		#endregion

		#region Modality Lut Classes

		private class CachedModalityLutLinear : SimpleDataModalityLut
		{
			public CachedModalityLutLinear(IDataModalityLut source)
				: base(source.MinInputValue, source.Data,
				       source.MinOutputValue, source.MaxOutputValue,
				       source.GetKey(), source.GetDescription()) {}
		}

		private class ModalityLutCacheItem : CacheItem<IDataModalityLut>
		{
			private readonly ModalityLutLinear _sourceLut;

			internal ModalityLutCacheItem(ModalityLutLinear sourceLut)
			{
				_sourceLut = sourceLut;
				_sourceLut.Clear();
			}

			protected override IDataModalityLut CreateLut()
			{
				CachedModalityLutLinear lut = new CachedModalityLutLinear(_sourceLut);
				_sourceLut.Clear();
				return lut;
			}

			protected override int GetSizeInBytes(IDataModalityLut lut)
			{
				return lut.Data.Length*sizeof (int);
			}

			public override string ToString()
			{
				return _sourceLut.GetKey();
			}
		}

		[Cloneable(true)]
		private class CachedModalityLutProxy : ComposableModalityLut
		{
			[CloneCopyReference]
			private readonly ModalityLutCacheItem _cacheItem;

			public CachedModalityLutProxy(ModalityLutCacheItem cacheItem)
			{
				_cacheItem = cacheItem;
			}

			//for cloning.
			private CachedModalityLutProxy() {}

			private IDataModalityLut RealLut
			{
				get { return _cacheItem.RealLut; }
			}

			public override int MinInputValue
			{
				get { return RealLut.MinInputValue; }
				set { }
			}

			public override int MaxInputValue
			{
				get { return RealLut.MaxInputValue; }
				set { }
			}

			public override double MinOutputValue
			{
				get { return RealLut.MinOutputValue; }
				protected set { }
			}

			public override double MaxOutputValue
			{
				get { return RealLut.MaxOutputValue; }
				protected set { }
			}

			public override double this[int index]
			{
				get { return RealLut[index]; }
			}

			protected override void LookupValues(double[] input, double[] output, int count)
			{
				RealLut.LookupValues(input, output, count);
			}

			public override string GetKey()
			{
				return RealLut.GetKey();
			}

			public override string GetDescription()
			{
				return RealLut.GetDescription();
			}
		}

		#endregion

		#region Private Fields

		private static readonly Dictionary<string, IColorMapFactory> _colorMapFactories;
		private static readonly List<IColorMapFactory> _sortedColorMapFactories;
		private static readonly SingletonLutFactory _instance = new SingletonLutFactory();

		#endregion

		#region Static

		static LutFactory()
		{
			_sortedColorMapFactories = CreateSortedColorMapFactories();

			_colorMapFactories = new Dictionary<string, IColorMapFactory>();
			foreach (IColorMapFactory colorMapFactory in _sortedColorMapFactories)
				_colorMapFactories[colorMapFactory.Name] = colorMapFactory;
		}

		private static List<IColorMapFactory> CreateSortedColorMapFactories()
		{
			List<IColorMapFactory> factories = new List<IColorMapFactory>();

			try
			{
				object[] extensions = new ColorMapFactoryExtensionPoint().CreateExtensions();
				foreach (IColorMapFactory factory in extensions)
				{
					if (String.IsNullOrEmpty(factory.Name))
						Platform.Log(LogLevel.Debug, "'{0}' must have a unique name", factory.GetType().FullName);
					else
						factories.Add(factory);
				}
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			factories = CollectionUtils.Sort(factories, (f1, f2) => (f1.Description ?? "").CompareTo(f2.Description ?? ""));

			factories.Insert(0, new GrayscaleColorMapFactory());
			return factories;
		}

		/// <summary>
		/// Calls <see cref="Create"/>.
		/// </summary>
		/// <remarks>This property has been deprecated; use <see cref="Create"/> instead.</remarks>
		[Obsolete("Use Create method instead.")]
		public static LutFactory NewInstance
		{
			get { return Create(); }
		}

		/// <summary>
		/// Creates a new factory instance.
		/// </summary>
		/// <remarks>
		/// You must dispose of the returned instance when you are done with it.
		/// </remarks>
		public static LutFactory Create()
		{
			return new LutFactoryProxy();
		}

		#endregion

		#region Singleton Instance

		private class SingletonLutFactory : LutFactory
		{
			private readonly object _syncLock = new object();
			private readonly Dictionary<ColorMapKey, ColorMapCacheItem> _colorMapCache;
			private readonly Dictionary<string, ModalityLutCacheItem> _modalityLutCache;
			private int _referenceCount;

			internal SingletonLutFactory()
			{
				_colorMapCache = new Dictionary<ColorMapKey, ColorMapCacheItem>();
				_modalityLutCache = new Dictionary<string, ModalityLutCacheItem>();
			}

			#region Color Maps

			/// <summary>
			/// Gets <see cref="ColorMapDescriptor"/>s that describe all the available color maps.
			/// </summary>
			public override IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get
				{
					//If there's only the default grayscale one, then don't return any (no point).
					if (_sortedColorMapFactories.Count == 1)
					{
						yield break;
					}
					else
					{
						foreach (IColorMapFactory factory in _sortedColorMapFactories)
							yield return ColorMapDescriptor.FromFactory(factory);
					}
				}
			}

			/// <summary>
			/// Factory method for grayscale color maps.
			/// </summary>
			public override IColorMap GetGrayscaleColorMap()
			{
				return GetColorMap(GrayscaleColorMapFactory.FactoryName);
			}

			/// <summary>
			/// Factory method that returns a new color map given the name of a <see cref="IColorMapFactory"/>.
			/// </summary>
			public override IColorMap GetColorMap(string name)
			{
				if (!_colorMapFactories.ContainsKey(name))
					throw new ArgumentException(String.Format("No Color Map factory extension exists with the name {0}.", name));

				return new CachedColorMapProxy(name);
			}

			internal ColorMapCacheItem GetColorMapCacheItem(ColorMapKey key)
			{
				lock (_syncLock)
				{
					if (_colorMapCache.ContainsKey(key))
						return _colorMapCache[key];

					ColorMapCacheItem item = new ColorMapCacheItem(key);
					_colorMapCache[key] = item;
					return item;
				}
			}

			#endregion

			#region Modality Luts

			/// <summary>
			/// Factory method for linear modality luts.
			/// </summary>
			public override IModalityLut GetModalityLutLinear(int bitsStored, bool isSigned, double rescaleSlope, double rescaleIntercept)
			{
				ModalityLutLinear modalityLut = new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
				string key = modalityLut.GetKey();

				lock (_syncLock)
				{
					if (_modalityLutCache.ContainsKey(key))
						return new CachedModalityLutProxy(_modalityLutCache[key]);

					ModalityLutCacheItem item = new ModalityLutCacheItem(modalityLut);
					_modalityLutCache[key] = item;
					return new CachedModalityLutProxy(item);
				}
			}

			#endregion

			internal void OnProxyCreated()
			{
				Interlocked.Increment(ref _referenceCount);
			}

			internal void OnProxyDisposed()
			{
				if (Interlocked.Decrement(ref _referenceCount) <= 0)
				{
					lock (_syncLock)
					{
						Thread.VolatileWrite(ref _referenceCount, 0); //force it to zero, just in case.

						foreach (ModalityLutCacheItem item in _modalityLutCache.Values)
							item.Unload();

						foreach (ColorMapCacheItem item in _colorMapCache.Values)
							item.Unload();

						_modalityLutCache.Clear();
						_colorMapCache.Clear();

						Trace.WriteLine("LutFactory cache is empty.");
					}
				}
			}

			#endregion
		}
	}
}