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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageBoxMemento : IEquatable<ImageBoxMemento>
	{
		private IDisplaySet _displaySet;
		private bool _displaySetLocked;
		private object _displaySetMemento;
		private int _rows;
		private int _columns;
		private int _topLeftPresentationImageIndex;
		private RectangleF _normalizedRectangle;
		private int _indexOfSelectedTile;

		public ImageBoxMemento(
			IDisplaySet displaySet,
			bool displaySetLocked,
			object displaySetMemento,
			int rows,
			int columns,
			int topLeftPresentationImageIndex,
			RectangleF normalizedRectangle,
			int indexOfSelectedTile)
		{
			// displaySet can be null, as that would correspond to an
			// empty imageBox
			Platform.CheckNonNegative(rows, "rows");
			Platform.CheckNonNegative(columns, "columns");
			Platform.CheckNonNegative(_topLeftPresentationImageIndex, "_topLeftPresentationImageIndex");

			_displaySet = displaySet;
			_displaySetLocked = displaySetLocked;
			_displaySetMemento = displaySetMemento;
			_rows = rows;
			_columns = columns;
			_topLeftPresentationImageIndex = topLeftPresentationImageIndex;
			_normalizedRectangle = normalizedRectangle;
			_indexOfSelectedTile = indexOfSelectedTile;
		}

		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
		}

		public bool DisplaySetLocked
		{
			get { return _displaySetLocked; }	
		}

		public object DisplaySetMemento
		{
			get { return _displaySetMemento; }	
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		public int TopLeftPresentationImageIndex
		{
			get { return _topLeftPresentationImageIndex; }
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
		}

		public int IndexOfSelectedTile
		{
			get { return _indexOfSelectedTile; }
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return false;

			if (obj is ImageBoxMemento)
				return this.Equals((ImageBoxMemento) obj);

			return false;
		}

		/// <summary>
		/// Gets a hash code for the object.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IEquatable<ImageBoxMemento> Members

		public bool Equals(ImageBoxMemento other)
		{
			if (other == null)
				return false;

			return DisplaySet == other.DisplaySet &&
				   DisplaySetLocked == other.DisplaySetLocked &&
				   Object.Equals(DisplaySetMemento, other.DisplaySetMemento) &&
			       Rows == other.Rows &&
			       Columns == other.Columns &&
			       TopLeftPresentationImageIndex == other.TopLeftPresentationImageIndex &&
			       IndexOfSelectedTile == other.IndexOfSelectedTile &&
			       NormalizedRectangle.Equals(other.NormalizedRectangle);
		}

		#endregion
	}
}
