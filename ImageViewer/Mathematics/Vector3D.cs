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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// Represents a three dimensional coordinate or vector.
	/// </summary>
	/// <remarks>
	/// This class is immutable. All necessary operators return the resulting output as a new instance.
	/// </remarks>
	public sealed class Vector3D : IEquatable<Vector3D>
	{
		private readonly float _x;
		private readonly float _y;
		private readonly float _z;

		/// <summary>
		/// Represents the Zero vector.
		/// </summary>
		public static readonly Vector3D Null = new Vector3D(0F, 0F, 0F);

// ReSharper disable InconsistentNaming

		/// <summary>
		/// Represents the unit vector in the direction of the positive X axis.
		/// </summary>
		public static readonly Vector3D xUnit = new Vector3D(1F, 0F, 0F);

		/// <summary>
		/// Represents the unit vector in the direction of the positive Y axis.
		/// </summary>
		public static readonly Vector3D yUnit = new Vector3D(0F, 1F, 0F);

		/// <summary>
		/// Represents the unit vector in the direction of the positive Z axis.
		/// </summary>
		public static readonly Vector3D zUnit = new Vector3D(0F, 0F, 1F);

// ReSharper restore InconsistentNaming

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3D"/>.
		/// </summary>
		/// <param name="x">The X component of the vector.</param>
		/// <param name="y">The Y component of the vector.</param>
		/// <param name="z">The Z component of the vector.</param>
		public Vector3D(float x, float y, float z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3D"/>.
		/// </summary>
		/// <param name="values">The components of the vector.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="values"/> does not have exactly 3 elements.</exception>
		public Vector3D(float[] values)
		{
			Platform.CheckForNullReference(values, "values");
			Platform.CheckTrue(values.Length == 3, "values must have exactly 3 elements");
			_x = values[0];
			_y = values[1];
			_z = values[2];
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Vector3D"/>.
		/// </summary>
		public Vector3D(Vector3D src)
		{
			_x = src.X;
			_y = src.Y;
			_z = src.Z;
		}

		/// <summary>
		/// Gets the specified component of the vector.
		/// </summary>
		/// <param name="index">The index of the component (X=0, Y=1, Z=2).</param>
		public float this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, 3, GetType());
				return index == 0 ? _x : (index == 1 ? _y : _z);
			}
		}

		/// <summary>
		/// Gets the X component of the vector.
		/// </summary>
		public float X
		{
			get { return _x; }
		}

		/// <summary>
		/// Gets the Y component of the vector.
		/// </summary>
		public float Y
		{
			get { return _y; }
		}

		/// <summary>
		/// Gets the Z component of the vector.
		/// </summary>
		public float Z
		{
			get { return _z; }
		}

		/// <summary>
		/// Gets whether or not this is a 'null' vector (all components are zero).
		/// </summary>
		public bool IsNull
		{
// ReSharper disable CompareOfFloatsByEqualityOperator
			get { return _x == 0F && _y == 0F && _z == 0F; }
// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Gets the magnitude of this vector.
		/// </summary>
		public float Magnitude
		{
			get { return (float) Math.Sqrt(_x*_x + _y*_y + _z*_z); }
		}

		/// <summary>
		/// Normalizes the vector, or makes it a unit vector.
		/// </summary>
		public Vector3D Normalize()
		{
			return this/Magnitude;
		}

		/// <summary>
		/// Gets the dot product (scalar product) of of this vector and <paramref name="right"/>.
		/// </summary>
		public float Dot(Vector3D right)
		{
			return _x*right.X + _y*right.Y + _z*right.Z;
		}

		/// <summary>
		/// Returns the cross product (vector product) of this vector and <paramref name="right"/>.
		/// </summary>
		public Vector3D Cross(Vector3D right)
		{
			float x = _y*right.Z - _z*right.Y;
			float y = -_x*right.Z + _z*right.X;
			float z = _x*right.Y - _y*right.X;

			return new Vector3D(x, y, z);
		}

		/// <summary>
		/// Determines whether or not this vector is parallel to <paramref name="other"/> within a certain <paramref name="angleToleranceRadians"/>.
		/// </summary>
		public bool IsParallelTo(Vector3D other, float angleToleranceRadians)
		{
			angleToleranceRadians = Math.Abs(angleToleranceRadians);
			float angle = GetAngleBetween(other);
			return FloatComparer.AreEqual(angle, 0, angleToleranceRadians) ||
			       FloatComparer.AreEqual(angle, (float) Math.PI, angleToleranceRadians);
		}

		/// <summary>
		/// Determines whether or not this vector is orthogonal to <paramref name="other"/> within a certain <paramref name="angleToleranceRadians"/>.
		/// </summary>
		public bool IsOrthogonalTo(Vector3D other, float angleToleranceRadians)
		{
			angleToleranceRadians = Math.Abs(angleToleranceRadians);
			float angle = GetAngleBetween(other);
			const float halfPi = (float) Math.PI/2;
			return FloatComparer.AreEqual(angle, halfPi, angleToleranceRadians);
		}

		/// <summary>
		/// Gets the angle between this vector and <paramref name="other"/> in radians.
		/// </summary>
		public float GetAngleBetween(Vector3D other)
		{
			Vector3D normal1 = Normalize();
			Vector3D normal2 = other.Normalize();

			// the vectors are already normalized, so we don't need to divide by the magnitudes.
			float dot = normal1.Dot(normal2);

			if (dot < -1F)
				dot = -1F;
			if (dot > 1F)
				dot = 1F;

			return Math.Abs((float) Math.Acos(dot));
		}

		/// <summary>
		/// Gets the components of the vector as an array (elements in order of X, Y and Z).
		/// </summary>
		public float[] ToArray()
		{
			return new[] {_x, _y, _z};
		}

		/// <summary>
		/// Returns a descriptive string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"({0}, {1}, {2})", _x, _y, _z);
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of <paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator *(float scale, Vector3D vector)
		{
			return vector*scale;
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of <paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator *(Vector3D vector, float scale)
		{
			return new Vector3D(vector.X*scale, vector.Y*scale, vector.Z*scale);
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of 1/<paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator /(float scale, Vector3D vector)
		{
			return vector/scale;
		}

		/// <summary>
		/// Scales <paramref name="vector"/> by a factor of 1/<paramref name="scale"/>.
		/// </summary>
		public static Vector3D operator /(Vector3D vector, float scale)
		{
			return new Vector3D(vector.X/scale, vector.Y/scale, vector.Z/scale);
		}

		/// <summary>
		/// Adds <paramref name="left"/> and <paramref name="right"/> together.
		/// </summary>
		public static Vector3D operator +(Vector3D left, Vector3D right)
		{
			return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		/// <summary>
		/// Subtracts <paramref name="right"/> from <paramref name="left"/>.
		/// </summary>
		public static Vector3D operator -(Vector3D left, Vector3D right)
		{
			return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		/// <summary>
		/// Returns the negative of the given vector.
		/// </summary>
		public static Vector3D operator -(Vector3D vector)
		{
			return -1*vector;
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a given tolerance (per vector component).
		/// </summary>
		public static bool AreEqual(Vector3D left, Vector3D right, float tolerance)
		{
			if (left == null || right == null)
				return ReferenceEquals(left, right);

			return FloatComparer.AreEqual(left.X, right.X, tolerance) &&
			       FloatComparer.AreEqual(left.Y, right.Y, tolerance) &&
			       FloatComparer.AreEqual(left.Z, right.Z, tolerance);
		}

		/// <summary>
		/// Gets whether or not <paramref name="left"/> is equal to <paramref name="right"/>, within a small tolerance (per vector component).
		/// </summary>
		public static bool AreEqual(Vector3D left, Vector3D right)
		{
			if (left == null || right == null)
				return ReferenceEquals(left, right);

			return FloatComparer.AreEqual(left.X, right.X) &&
			       FloatComparer.AreEqual(left.Y, right.Y) &&
			       FloatComparer.AreEqual(left.Z, right.Z);
		}

		/// <summary>
		/// Gets a hash code for the vector.
		/// </summary>
		public override int GetHashCode()
		{
			return -0x760F6FAB ^ _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
		}

		/// <summary>
		/// Gets whether or not this vector equals <paramref name="obj"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			return obj == this || Equals(obj as Vector3D);
		}

		/// <summary>
		/// Gets whether or not this vector equals <paramref name="other"/>.
		/// </summary>
		public bool Equals(Vector3D other)
		{
			if (other == null)
				return false;

// ReSharper disable CompareOfFloatsByEqualityOperator
			return (X == other.X && Y == other.Y && Z == other.Z);
// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Gets the specified <see cref="Size3D"/> value as a <see cref="Vector3D"/>.
		/// </summary>
		public static implicit operator Vector3D(Size3D size3D)
		{
			return new Vector3D(size3D.Width, size3D.Height, size3D.Depth);
		}

		#region Specialized Calculations

		/// <summary>
		/// Finds the intersection of the line segment defined by <paramref name="linePoint1"/> and
		/// <paramref name="linePoint2"/> with a plane described by it's normal (<paramref name="planeNormal"/>)
		/// and an arbitrary point in the plane (<paramref name="pointInPlane"/>).
		/// </summary>
		/// <param name="planeNormal">The normal vector of an arbitrary plane.</param>
		/// <param name="pointInPlane">A point in space that lies on the plane whose normal is <paramref name="planeNormal"/>.</param>
		/// <param name="linePoint1">The position vector of the start of the line.</param>
		/// <param name="linePoint2">The position vector of the end of the line.</param>
		/// <param name="isLineSegment">Specifies whether <paramref name="linePoint1"/> and <paramref name="linePoint2"/>
		/// define a line segment, or simply 2 points on an infinite line.</param>
		/// <returns>A position vector describing the point of intersection of the line with the plane, or null if the
		/// line and plane do not intersect.</returns>
		public static Vector3D GetLinePlaneIntersection(
			Vector3D planeNormal,
			Vector3D pointInPlane,
			Vector3D linePoint1,
			Vector3D linePoint2,
			bool isLineSegment)
		{
			if (AreEqual(planeNormal, Null))
				return null;

			Vector3D line = linePoint2 - linePoint1;
			Vector3D planeToLineStart = pointInPlane - linePoint1;

			float lineDotPlaneNormal = planeNormal.Dot(line);

			if (FloatComparer.AreEqual(0F, lineDotPlaneNormal))
				return null;

			float ratio = planeNormal.Dot(planeToLineStart)/lineDotPlaneNormal;

			if (isLineSegment && (ratio < 0F || ratio > 1F))
				return null;

			return linePoint1 + ratio*line;
		}

		#endregion
	}
}