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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents the pixel aspect ratio of an image.
	/// </summary>
	public class PixelAspectRatio : IEquatable<PixelAspectRatio>
	{
		#region Private Members

		private int _row;
		private int _column;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public PixelAspectRatio(int row, int column)
		{
			_row = row;
			_column = column;
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected PixelAspectRatio() {}

		#region Public Properties

		/// <summary>
		/// Gets whether or not this object represents a null value.
		/// </summary>
		public bool IsNull
		{
			get { return _row == 0 || _column == 0; }
		}

		//TODO: change this to Vertical/Horizontal, as specified in DICOM PS3.3 C.7.6.3.1.7?

		/// <summary>
		/// Gets the row (vertical) component of the ratio.
		/// </summary>
		public virtual int Row
		{
			get { return _row; }
			protected set { _row = value; }
		}

		/// <summary>
		/// Gets the column (horizontal) component of the ratio.
		/// </summary>
		public virtual int Column
		{
			get { return _column; }
			protected set { _column = value; }
		}

		/// <summary>
		/// Gets the pixel aspect ratio as a floating point value, or zero if <see cref="IsNull"/> is true.
		/// </summary>
		/// <remarks>
		/// The aspect ratio of a pixel is defined as the ratio of it's vertical and horizontal
		/// size(s), or <see cref="Row"/> divided by <see cref="Column"/>.
		/// </remarks>
		public float Value
		{
			get
			{
				if (IsNull)
					return 0;

				return Row/(float) Column;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets a string suitable for direct insertion into a <see cref="DicomAttributeMultiValueText"/> attribute.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0}\{1}", _row, _column);
		}

		/// <summary>
		/// Creates a <see cref="PixelAspectRatio"/> object from a dicom multi-valued string.
		/// </summary>
		/// <returns>
		/// Null if there are not exactly 2 parsed values in the input string.
		/// </returns>
		public static PixelAspectRatio FromString(string multiValuedString)
		{
			if (string.IsNullOrEmpty(multiValuedString)) return null;

			int[] values;
			if (DicomStringHelper.TryGetIntArray(multiValuedString, out values) && values.Length == 2)
				return new PixelAspectRatio(values[0], values[1]);
			return null;
		}

		#region IEquatable<PixelAspectRatio> Members

		public bool Equals(PixelAspectRatio other)
		{
			return other != null && _row == other._row && _column == other._column;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return obj != null && Equals(obj as PixelAspectRatio);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			return -0x463A3C83;
		}

		#endregion
	}
}