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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	[Cloneable]
	public sealed class ExtensionData : IDisposable
	{
		private readonly IDictionary<Type, object> _data;

		public ExtensionData()
		{
			_data = new Dictionary<Type, object>();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		private ExtensionData(ExtensionData source, ICloningContext context)
		{
			_data = new Dictionary<Type, object>();
			foreach (var sourceData in source._data)
			{
				var valueClone = CloneBuilder.Clone(sourceData.Value);
				if (valueClone != null)
					_data[sourceData.Key] = valueClone;
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
			}
			catch (Exception e)
			{
				// shouldn't throw anything inside Dispose
				Platform.Log(LogLevel.Error, e);
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var disposable in _data.Values.OfType<IDisposable>())
					disposable.Dispose();
				_data.Clear();
			}
		}

		/// <summary>
		/// Gets or sets the data with the specified type key.
		/// </summary>
		/// <remarks>
		/// Typically, the type key will be either the type of the data or the type of the calling class responsible for the data.
		/// In general, the type key should never be a common type (such as <see cref="String"/>, <see cref="Int32"/> or <see cref="Boolean"/>)
		/// since it would not be a particularly unique key unless extra care is taken to limit the context of the extension data).
		/// </remarks>
		/// <param name="key">The type key of the data.</param>
		public object this[Type key]
		{
			get
			{
				object value;
				return _data.TryGetValue(key, out value) ? value : null;
			}
			set { _data[key] = value; }
		}

		/// <summary>
		/// Gets the data where the type of the data also acts as its own type key.
		/// </summary>
		/// <typeparam name="T">The type key of the data.</typeparam>
		/// <returns>The data.</returns>
		public T Get<T>()
			where T : class
		{
			return (T) this[typeof (T)];
		}

		/// <summary>
		/// Gets the data where the type of the data also acts as its own type key. If necessary, the data will be instantiated and set.
		/// </summary>
		/// <typeparam name="T">The type key of the data.</typeparam>
		/// <returns>The data.</returns>
		public T GetOrCreate<T>()
			where T : class, new()
		{
			return (T) this[typeof (T)] ?? ((T) (this[typeof (T)] = new T()));
		}

		/// <summary>
		/// Gets the data where the type of the data also acts as its own type key.
		/// </summary>
		/// <typeparam name="T">The type key of the data.</typeparam>
		/// <param name="value">The data.</param>
		public void Set<T>(T value)
			where T : class
		{
			this[typeof (T)] = value;
		}
	}
}