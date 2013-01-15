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
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public sealed class StoredLayoutSortByModality : IComparer<StoredLayout>
	{
		public StoredLayoutSortByModality()
		{ 
		}

		#region IComparer<StoredLayoutConfiguration> Members

		public int Compare(StoredLayout x, StoredLayout y)
		{
			return x.Modality.CompareTo(y.Modality);
		}

		#endregion
	}

	public sealed class StoredLayout : INotifyPropertyChanged
	{
		private readonly string _modality;
		private int _imageBoxRows;
		private int _imageBoxColumns;
		private int _tileRows;
		private int _tileColumns;

		private event PropertyChangedEventHandler _propertyChanged;

		internal StoredLayout(string modality, int imageBoxRows, int imageBoxColumns, int tileRows, int tileColumns)
		{
			_modality = modality ?? "";
			_imageBoxRows = imageBoxRows;
			_imageBoxColumns = imageBoxColumns;
			_tileRows = tileRows;
			_tileColumns = tileColumns;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion

		public int ImageBoxRows
		{
			get { return _imageBoxRows; }
			set
			{
				if (_imageBoxRows == value)
					return;

				_imageBoxRows = Math.Max(value, 1);
				_imageBoxRows = Math.Min(_imageBoxRows, LayoutSettings.MaximumImageBoxRows);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("ImageBoxRows"));
			}
		}

		public int ImageBoxColumns
		{
			get { return _imageBoxColumns; }
			set
			{
				if (_imageBoxColumns == value)
					return;

				_imageBoxColumns = Math.Max(value, 1);
				_imageBoxColumns = Math.Min(_imageBoxColumns, LayoutSettings.MaximumImageBoxColumns);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("ImageBoxColumns"));
			}
		}

		public int TileRows
		{
			get { return _tileRows; }
			set
			{
				if (_tileRows == value)
					return;

				_tileRows = Math.Max(value, 1);
				_tileRows = Math.Min(_tileRows, LayoutSettings.MaximumTileRows);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("TileRows"));
			}
		}

		public int TileColumns
		{
			get { return _tileColumns; }
			set
			{
				if (_tileColumns == value)
					return;

				_tileColumns = Math.Max(value, 1);
				_tileColumns = Math.Min(_tileColumns, LayoutSettings.MaximumTileColumns);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("TileColumns"));
			}
		}
	
		public string Modality
		{
			get { return _modality; }
		}

		public string Text
		{
			get
			{
				return (this.IsDefault) ? SR.LabelDefault : _modality;
			}
		}

		public bool IsDefault
		{
			get { return String.IsNullOrEmpty(_modality); }
		}
	}
}
