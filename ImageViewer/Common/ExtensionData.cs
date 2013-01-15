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

		public object this[Type key]
		{
			get
			{
				object value;
				return _data.TryGetValue(key, out value) ? value : null;
			}
			set { _data[key] = value; }
		}
	}
}