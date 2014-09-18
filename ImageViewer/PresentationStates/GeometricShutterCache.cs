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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Imaging;
using System.Diagnostics;
using System.Text;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	internal class GeometricShutterCache
	{
		public interface ICacheItem
		{
			ColorPixelData PixelData { get; }
		}

		private class Key : IEquatable<Key>
		{
			private string _description;

			public Key(IList<GeometricShutter> shutters, Rectangle imageRectangle, Color fillColor)
			{
				shutters = CollectionUtils.Map(shutters, (GeometricShutter shutter) => shutter.Clone());
				Shutters = new ReadOnlyCollection<GeometricShutter>(shutters);
				ImageRectangle = imageRectangle;
				FillColor = fillColor;
			}

			public readonly IList<GeometricShutter> Shutters;
			public readonly Rectangle ImageRectangle;
			public readonly Color FillColor;

			public override int GetHashCode()
			{
				int hash = 0x5808A899;
				foreach (GeometricShutter shutter in Shutters)
					hash ^= shutter.GetHashCode();

				hash ^= ImageRectangle.GetHashCode();
				hash ^= FillColor.GetHashCode();
				return hash;
			}

			public override bool Equals(object obj)
			{
				if (obj is Key)
					return Equals((Key)obj);

				return false;
			}

			#region IEquatable<Key> Members

			public bool Equals(Key other)
			{
				if (other.Shutters.Count != Shutters.Count)
					return false;

				for (int i = 0; i < Shutters.Count; ++i)
				{
					//could do these in any order,really, but that complicates things.
					if (!Shutters[i].Equals(other.Shutters[i]))
						return false;
				}

				return other.ImageRectangle.Equals(ImageRectangle) && other.FillColor.Equals(FillColor);
			}

			#endregion

			public override string ToString()
			{
				if (_description != null)
					return _description;

				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("Shutters (count={0}, rect={1}, fill={2})", Shutters.Count, ImageRectangle, FillColor);
				foreach (GeometricShutter shutter in Shutters)
				{
					builder.AppendLine();
					builder.AppendFormat("\t{0}", shutter);
				}

				_description = builder.ToString();
				return _description;
			}
		}

		private class Item : ICacheItem, ILargeObjectContainer
		{
			private readonly Key _key;
			private readonly ColorPixelData _pixelData;

			private readonly LargeObjectContainerData _largeObjectData;
			private readonly object _syncLock = new object();
			private volatile byte[] _buffer;

			~Item()
			{
				_containsDeadItems = true;
			}

			public Item(Key key)
			{
				_key = key;
				_pixelData = new ColorPixelData(key.ImageRectangle.Height, key.ImageRectangle.Width, GetPixelData);
				_largeObjectData = new LargeObjectContainerData(Guid.NewGuid()) { RegenerationCost = LargeObjectContainerData.PresetGeneratedData };
			}

			public ColorPixelData PixelData
			{
				get { return _pixelData; }
			}

			private byte[] GetPixelData()
			{
				_largeObjectData.UpdateLastAccessTime();

				byte[] buffer = _buffer;
				if (buffer != null)
					return buffer;

				lock (_syncLock)
				{
					buffer = _buffer;
					if (buffer == null)
					{
						buffer = _buffer = CreateShutter(_key);
						_largeObjectData.BytesHeldCount = buffer.Length;
						_largeObjectData.LargeObjectCount = 1;
						MemoryManager.Add(this);

						//Trace.WriteLine(String.Format("Loading: {0}", _key), "Memory");
						//Trace.WriteLine("", "Memory");
					}
				}

				return buffer;
			}

			#region ILargeObjectContainer Members

			public Guid Identifier
			{
				get { return _largeObjectData.Identifier; }
			}

			public int LargeObjectCount
			{
				get { return _largeObjectData.LargeObjectCount; }
			}

			public long BytesHeldCount
			{
				get { return _largeObjectData.BytesHeldCount; }
			}

			public DateTime LastAccessTime
			{
				get { return _largeObjectData.LastAccessTime; }
			}

			public RegenerationCost RegenerationCost
			{
				get { return _largeObjectData.RegenerationCost; }
			}

			public bool IsLocked
			{
				get { return false; }
			}

			public void Lock()
			{
			}

			public void Unlock()
			{
			}

			public void Unload()
			{
				if (_buffer == null)
					return;

				lock (_syncLock)
				{
					if (_buffer == null)
						return;

					_buffer = null;
					_largeObjectData.BytesHeldCount = 0;
					_largeObjectData.LargeObjectCount = 0;
					MemoryManager.Remove(this);
					//Trace.WriteLine(String.Format("Unloading: {0}", _key), "Memory");
				}
			}

			#endregion

			public override int GetHashCode()
			{
				return _key.GetHashCode();
			}

			public override string ToString()
			{
				return _key.ToString();
			}
		}

		private static readonly object _cacheSyncLock = new object();
		private static readonly Dictionary<Key, WeakReference> _cache = new Dictionary<Key, WeakReference>();
		private static volatile bool _containsDeadItems = false;

		public static ICacheItem GetCacheItem(IList<GeometricShutter> shutters, Rectangle imageRectangle, Color fillColor)
		{
			Key key = new Key(shutters, imageRectangle, fillColor);

			//Trace.WriteLine("", "Memory");

			lock (_cacheSyncLock)
			{
				if (_containsDeadItems)
				{
					try { CleanupDeadItems(); }
					finally { _containsDeadItems = false; }
				}

				Item item;
				WeakReference cacheItem;
				if (_cache.TryGetValue(key, out cacheItem))
				{
					//Trace.WriteLine(String.Format("Exists: {0}", key), "Memory");

					try
					{
						item = cacheItem.Target as Item;
						if (item != null)
						{
							//Trace.WriteLine(String.Format("Live: {0}", key), "Memory");
							return item;
						}

						//Trace.WriteLine(String.Format("Dead: {0}", key), "Memory");
					}
					catch (InvalidOperationException)
					{
						//Trace.WriteLine(String.Format("Dead: {0}", key), "Memory");
					}
				}

				//Trace.WriteLine(String.Format("New:{0} ", key), "Memory");

				item = new Item(key);
				_cache[key] = new WeakReference(item);
				return item;
			}
		}

		private static void CleanupDeadItems()
		{
			List<Key> keysToRemove = new List<Key>();

			foreach(KeyValuePair<Key, WeakReference> cacheItem in _cache)
			{
				try
				{
					Item item = cacheItem.Value.Target as Item;
					if (item != null)
						continue;

					keysToRemove.Add(cacheItem.Key);
				}
				catch (InvalidOperationException)
				{
					keysToRemove.Add(cacheItem.Key);
				}
			}

			if (keysToRemove.Count > 0)
				Trace.WriteLine(String.Format("Removing {0} dead shutters from the cache", keysToRemove.Count), "Memory");

			foreach (Key key in keysToRemove)
				_cache.Remove(key);
		}

		private static byte[] CreateShutter(Key key)
		{
			return CreateShutter(key.Shutters, key.ImageRectangle, key.FillColor);
		}

		private static byte[] CreateShutter(IList<GeometricShutter> shutters, Rectangle imageRectangle, Color fillColor)
		{
			int stride = imageRectangle.Width * 4;
			int size = imageRectangle.Height * stride;
			byte[] buffer = MemoryManager.Allocate<byte>(size);

			GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			try
			{
				using (Bitmap bitmap = new Bitmap(imageRectangle.Width, imageRectangle.Height, stride, PixelFormat.Format32bppPArgb, bufferHandle.AddrOfPinnedObject()))
				{
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
					{
						graphics.Clear(Color.FromArgb(0, Color.Black));
						using (Brush brush = new SolidBrush(fillColor))
						{
							foreach (GeometricShutter shutter in shutters)
							{
								using (GraphicsPath path = new GraphicsPath())
								{
									path.FillMode = FillMode.Alternate;
									path.AddRectangle(imageRectangle);
									shutter.AddToGraphicsPath(path);
									path.CloseFigure();
									graphics.FillPath(brush, path);
								}
							}
						}
					}

					//NOTE: we are not doing this properly according to Dicom.  We should be rendering
					//to a 16-bit image so we can set the 16-bit p-value.
					return buffer;
				}
			}
			finally
			{
				bufferHandle.Free();
			}
		}
	}
}