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

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal sealed class AnnotationDataSourceContext<TSource>
		where TSource : class
	{
		private readonly object _syncroot = new object();
		private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
		private WeakReference _dataSourceReference;

		public event EventHandler<AnnotationDataSourceContextEventArgs<TSource>> DataSourceChanged;

		public TSource DataSource
		{
			get
			{
				lock (_syncroot)
				{
					if (_dataSourceReference != null)
					{
						if (_dataSourceReference.IsAlive)
							return (TSource) _dataSourceReference.Target;

						_dictionary.Clear();
						_dataSourceReference = null;

						EventsHelper.Fire(DataSourceChanged, this, new AnnotationDataSourceContextEventArgs<TSource>(this));
					}
				}
				return null;
			}
			set
			{
				if (ReferenceEquals(value, DataSource)) return;

				lock (_syncroot)
				{
					_dictionary.Clear();
					_dataSourceReference = value != null ? new WeakReference(value) : null;

					EventsHelper.Fire(DataSourceChanged, this, new AnnotationDataSourceContextEventArgs<TSource>(this));
				}
			}
		}

		public object this[string key]
		{
			get
			{
				lock (_syncroot)
				{
					object data;
					return _dictionary.TryGetValue(key, out data) ? data : null;
				}
			}
			set
			{
				lock (_syncroot)
				{
					_dictionary[key] = value;
				}
			}
		}

		public object GetData(TSource dataSource, string key)
		{
			DataSource = dataSource;
			return this[key];
		}

		public TData GetData<TData>(TSource dataSource, string key)
			where TData : class
		{
			DataSource = dataSource;
			return this[key] as TData;
		}
	}

	internal sealed class AnnotationDataSourceContextEventArgs<TSource> : EventArgs
		where TSource : class
	{
		private readonly AnnotationDataSourceContext<TSource> _owner;
		private readonly TSource _dataSource;

		internal AnnotationDataSourceContextEventArgs(AnnotationDataSourceContext<TSource> owner)
		{
			_owner = owner;
			_dataSource = owner.DataSource;
		}

		public TSource DataSource
		{
			get { return _dataSource; }
		}

		public object this[string key]
		{
			get { return _owner[key]; }
			set { _owner[key] = value; }
		}
	}
}