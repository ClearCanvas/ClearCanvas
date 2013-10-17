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
using System.ComponentModel;
using System.Globalization;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// Stores a set of six float-point numbers that represent the location and dimensions of a rectangular cuboid.
	/// </summary>
	/// <remarks>
	/// This class uses a right-handed convention for axes - the direction of the third axis is determined by the
	/// cross product of the left-to-right vector and the top-to-bottom vectors, which yields the vector front-to-back.
	/// </remarks>
	public sealed class Rectangle3D : IEquatable<Rectangle3D>
	{
		/// <summary>
		/// Represents an instance of the <see cref="Rectangle3D"/> class with its members uninitialized.
		/// </summary>
		public static readonly Rectangle3D Empty = new Rectangle3D();

		private readonly Vector3D _location;
		private readonly Vector3D _dimensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle3D"/> class with a location at the origin and zero dimensions.
		/// </summary>
		public Rectangle3D()
			: this(null, null) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle3D"/> class with the specified location and dimensions.
		/// </summary>
		/// <param name="x">The x-coordinate of the front-upper-left corner of the rectangular cuboid.</param>
		/// <param name="y">The y-coordinate of the front-upper-left corner of the rectangular cuboid.</param>
		/// <param name="z">The z-coordinate of the front-upper-left corner of the rectangular cuboid.</param>
		/// <param name="width">The width of the rectangular cuboid.</param>
		/// <param name="height">The height of the rectangular cuboid.</param>
		/// <param name="depth">The depth of the rectangular cuboid.</param>
		public Rectangle3D(float x, float y, float z, float width, float height, float depth)
			: this(new Vector3D(x, y, z), new Vector3D(width, height, depth)) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle3D"/> class with the specified location and dimensions.
		/// </summary>
		/// <param name="location">A <see cref="Vector3D"/> that represents the front-upper-left corner of the rectangular cuboid.</param>
		/// <param name="dimensions">A <see cref="Vector3D"/> that represents the dimensions of the rectangular cuboid.</param>
		public Rectangle3D(Vector3D location, Vector3D dimensions)
		{
			_location = location ?? Vector3D.Null;
			_dimensions = dimensions ?? Vector3D.Null;
		}

		/// <summary>
		/// Tests whether the <see cref="Width"/>, <see cref="Height"/> or <see cref="Depth"/> property of this <see cref="Rectangle3D"/> has a value of zero.
		/// </summary>
		[Browsable(false)]
		public bool IsEmpty
		{
// ReSharper disable CompareOfFloatsByEqualityOperator
			get { return _dimensions.X == 0F || _dimensions.Y == 0F || _dimensions.Z == 0F; }
// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Gets the x-coordinate of the front-upper-left corner of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float X
		{
			get { return _location.X; }
		}

		/// <summary>
		/// Gets the y-coordinate of the front-upper-left corner of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float Y
		{
			get { return _location.Y; }
		}

		/// <summary>
		/// Gets the z-coordinate of the front-upper-left corner of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float Z
		{
			get { return _location.Z; }
		}

		/// <summary>
		/// Gets the width of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float Width
		{
			get { return _dimensions.X; }
		}

		/// <summary>
		/// Gets the height of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float Height
		{
			get { return _dimensions.Y; }
		}

		/// <summary>
		/// Gets the depth of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		public float Depth
		{
			get { return _dimensions.Z; }
		}

		/// <summary>
		/// Gets the x-coordinate of the left face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Left
		{
			get { return _location.X; }
		}

		/// <summary>
		/// Gets the y-coordinate of the top face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Top
		{
			get { return _location.Y; }
		}

		/// <summary>
		/// Gets the z-coordinate of the front face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Front
		{
			get { return _location.Z; }
		}

		/// <summary>
		/// Gets the x-coordinate of the right face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Right
		{
			get { return _location.X + _dimensions.X; }
		}

		/// <summary>
		/// Gets the y-coordinate of the bottom face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Bottom
		{
			get { return _location.Y + _dimensions.Y; }
		}

		/// <summary>
		/// Gets the z-coordinate of the back face of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public float Back
		{
			get { return _location.Z + _dimensions.Z; }
		}

		/// <summary>
		/// Gets the coordinates of the front-upper-left corner of this <see cref="Rectangle3D"/> structure.
		/// </summary>
		[Browsable(false)]
		public Vector3D Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Gets the size of this <see cref="Rectangle3D"/>.
		/// </summary>
		[Browsable(false)]
		public Vector3D Size
		{
			get { return _dimensions; }
		}

		/// <summary>
		/// Determines if the specified point is contained within this <see cref="Rectangle3D"/> structure.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(Vector3D point)
		{
			return point != null ? Contains(point.X, point.Y, point.Z) : Contains(0, 0, 0);
		}

		/// <summary>
		/// Determines if the specified point is contained within this <see cref="Rectangle3D"/> structure.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public bool Contains(float x, float y, float z)
		{
			return Left <= x && x < Right && Top <= y && y < Bottom && Front <= z && z < Back;
		}

		/// <summary>
		/// Determines if the rectangular cuboid region represented by <paramref name="rectangle"/> is entirely contained within this <see cref="Rectangle3D"/> structure.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <returns></returns>
		public bool Contains(Rectangle3D rectangle)
		{
			if (rectangle == null) rectangle = Empty;
			return Left <= rectangle.Left && rectangle.Right <= Right && Top <= rectangle.Top && rectangle.Bottom <= Bottom && Front <= rectangle.Front && rectangle.Back <= Back;
		}

		/// <summary>
		/// Computes a <see cref="Rectangle3D"/> enlarged by the specified amount.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Rectangle3D Inflate(Vector3D size)
		{
			return size != null ? Inflate(size.X, size.Y, size.Z) : Inflate(0, 0, 0);
		}

		/// <summary>
		/// Computes a <see cref="Rectangle3D"/> enlarged by the specified amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public Rectangle3D Inflate(float x, float y, float z)
		{
			return new Rectangle3D(X - x, Y - y, Z - z, Width + 2*x, Height + 2*y, Depth + 2*z);
		}

		/// <summary>
		/// Computes the intersection of this <see cref="Rectangle3D"/> and the <paramref name="rectangle"/>.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <returns></returns>
		public Rectangle3D Intersect(Rectangle3D rectangle)
		{
			return Intersect(this, rectangle);
		}

		/// <summary>
		/// Determines if this rectangle intersects with <paramref name="rect"/>.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public bool IntersectsWith(Rectangle3D rect)
		{
			return rect.Left < Right && Left < rect.Right && rect.Top < Bottom && Top < rect.Bottom && rect.Front < Back && Front < rect.Back;
		}

		/// <summary>
		/// Computes a <see cref="Rectangle3D"/> whose location is adjusted by the specified amount.
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		public Rectangle3D Offset(Vector3D offset)
		{
			return offset != null ? Offset(offset.X, offset.Y, offset.Z) : Inflate(0, 0, 0);
		}

		/// <summary>
		/// Computes a <see cref="Rectangle3D"/> whose location is adjusted by the specified amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public Rectangle3D Offset(float x, float y, float z)
		{
			return new Rectangle3D(X + x, Y + y, Z + z, Width, Height, Depth);
		}

		/// <summary>
		/// Computes an equivalent <see cref="Rectangle3D"/> whose dimensions are all positive.
		/// </summary>
		/// <returns></returns>
		public Rectangle3D ToPositiveRectangle()
		{
			var left = Left;
			var top = Top;
			var front = Front;
			var width = Width;
			var height = Height;
			var depth = Depth;

			if (width < 0)
			{
				left += width;
				width = -width;
			}

			if (height < 0)
			{
				top += height;
				height = -height;
			}

			if (depth < 0)
			{
				front += depth;
				depth = -depth;
			}

			return new Rectangle3D(left, top, front, width, height, depth);
		}

		public override int GetHashCode()
		{
			return 0x6EB17567 ^ _dimensions.GetHashCode() ^ _location.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle3D && Equals((Rectangle3D) obj);
		}

		public bool Equals(Rectangle3D other)
		{
			if (other == null) return false;
			return _dimensions.Equals(other._dimensions) && _location.Equals(other._location);
		}

		public override string ToString()
		{
			return string.Concat(new[]
			                     	{
			                     		"{X=", X.ToString(CultureInfo.CurrentCulture),
			                     		",Y=", Y.ToString(CultureInfo.CurrentCulture),
			                     		",Z=", Z.ToString(CultureInfo.CurrentCulture),
			                     		",Width=", Width.ToString(CultureInfo.CurrentCulture),
			                     		",Height=", Height.ToString(CultureInfo.CurrentCulture),
			                     		",Depth=", Depth.ToString(CultureInfo.CurrentCulture),
			                     		"}"
			                     	});
		}

		/// <summary>
		/// Returns a <see cref="Rectangle3D"/> that represents the intersection of two rectangular cuboids. If there is no intersection, an empty <see cref="Rectangle3D"/> is returned.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rectangle3D Intersect(Rectangle3D a, Rectangle3D b)
		{
			if (a == null) a = Empty;
			if (b == null) b = Empty;
			var left = Math.Max(a.Left, b.Left);
			var right = Math.Min(a.Right, b.Right);
			var top = Math.Max(a.Top, b.Top);
			var bottom = Math.Min(a.Bottom, b.Bottom);
			var front = Math.Max(a.Front, b.Front);
			var back = Math.Min(a.Back, b.Back);
			return right >= left && back >= front ? new Rectangle3D(left, top, front, right - left, bottom - top, back - front) : Empty;
		}

		/// <summary>
		/// Creates the smallest possible third rectangular cuboid that can contain both of two rectangular cuboids that form a union.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rectangle3D Union(Rectangle3D a, Rectangle3D b)
		{
			if (a == null) a = Empty;
			if (b == null) b = Empty;
			var left = Math.Min(a.Left, b.Left);
			var right = Math.Max(a.Right, b.Right);
			var top = Math.Min(a.Top, b.Top);
			var bottom = Math.Max(a.Bottom, b.Bottom);
			var front = Math.Min(a.Front, b.Front);
			var back = Math.Max(a.Back, b.Back);
			return new Rectangle3D(left, top, front, right - left, bottom - top, back - front);
		}

// ReSharper disable InconsistentNaming

		/// <summary>
		/// Creates a <see cref="Rectangle3D"/> with front-upper-left corner and back-lower-right corner at the specified locations.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="front"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="back"></param>
		/// <returns></returns>
		public static Rectangle3D FromLTFRBB(float left, float top, float front, float right, float bottom, float back)
		{
			return new Rectangle3D(left, top, front, right - left, bottom - top, back - front);
		}

		/// <summary>
		/// Creates a <see cref="Rectangle3D"/> with front-upper-left corner and back-lower-right corner at the specified locations.
		/// </summary>
		/// <param name="leftTopFront"></param>
		/// <param name="rightBottomBack"></param>
		/// <returns></returns>
		public static Rectangle3D FromLTFRBB(Vector3D leftTopFront, Vector3D rightBottomBack)
		{
			return new Rectangle3D(leftTopFront, rightBottomBack - leftTopFront);
		}

// ReSharper restore InconsistentNaming

		/// <summary>
		/// Tests whether two <see cref="Rectangle3D"/> instances have equal location and dimensions.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator ==(Rectangle3D x, Rectangle3D y)
		{
			return ReferenceEquals(x, y) || (!ReferenceEquals(x, null) && x.Equals(y));
		}

		/// <summary>
		/// Tests whether two <see cref="Rectangle3D"/> instances differ in location or dimensions.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator !=(Rectangle3D x, Rectangle3D y)
		{
			return !(x == y);
		}
	}
}