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

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A simple 3D size class.
	/// </summary>
	/// <remarks>
	/// The Size3D class is immutable.
	/// </remarks>
	public sealed class Size3D : IEquatable<Size3D>
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _depth;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Size3D(int width, int height, int depth)
		{
			_width = width;
			_height = height;
			_depth = depth;
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		public Size3D(Size3D src)
		{
			_width = src.Width;
			_height = src.Height;
			_depth = src.Depth;
		}

		/// <summary>
		/// Gets the width
		/// </summary>
		public int Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets the height
		/// </summary>
		public int Height
		{
			get { return _height; }
		}

		/// <summary>
		/// Gets the depth
		/// </summary>
		public int Depth
		{
			get { return _depth; }
		}

		/// <summary>
		/// Returns the width * height * depth
		/// </summary>
		public int Volume
		{
			get { return Width*Height*Depth; }
		}

		public override int GetHashCode()
		{
			return 0x4175BF53 ^ _width.GetHashCode() ^ _height.GetHashCode() ^ _depth.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Size3D && Equals((Size3D) obj);
		}

		/// <summary>
		/// Gets whether or not this object equals <paramref name="other"/>.
		/// </summary>
		public bool Equals(Size3D other)
		{
			if (other == null)
				return false;

			return (Width == other.Width && Height == other.Height && Depth == other.Depth);
		}

		public override string ToString()
		{
			return string.Format(@"(Width={0}, Height={1}, Depth={2})", _width, _height, _depth);
		}
	}
}