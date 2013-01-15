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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	internal delegate void NotifyListChangedDelegate();

	internal class ListObserver<T> : IDisposable where T : class
	{
		private readonly ObservableList<T> _list;
		private readonly NotifyListChangedDelegate _callback;
		private bool _suppressChangedEvent = false;

		public ListObserver(ObservableList<T> list, NotifyListChangedDelegate callback)
		{
			_callback = callback;
			_list = list;
			SuppressChangedEvent = false;
		}

		public bool SuppressChangedEvent
		{
			get { return _suppressChangedEvent; }
			set
			{
				if (_suppressChangedEvent == value)
					return;

				_suppressChangedEvent = value;
				if (_suppressChangedEvent)
				{
					_list.ItemAdded -= OnChanged;
					_list.ItemRemoved -= OnChanged;
					_list.ItemChanged -= OnChanged;
				}
				else
				{
					_list.ItemAdded += OnChanged;
					_list.ItemRemoved += OnChanged;
					_list.ItemChanged += OnChanged;
				}
			}
		}

		private void OnChanged(object sender, ListEventArgs<T> e)
		{
			_callback();
		}

		#region IDisposable Members

		public void Dispose()
		{
			SuppressChangedEvent = true;
		}

		#endregion
	}
}
