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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using NUnit.Framework;
using System;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class Vector3DTests
	{
		public Vector3DTests()
		{
		}

		[Test]
		public void TestAdd()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(6F, 9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(-1.6F, -2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));
		}

		[Test]
		public void TestSubtract()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-1.6F, 2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(6F, -9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));
		}

		[Test]
		public void TestMultiply()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D result = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F*v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestDivide()
		{
			Vector3D result = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v1 = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F / v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestNormalize()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Assert.IsTrue(FloatComparer.AreEqual(v1.Magnitude, 9.8392072851F));

			Vector3D normalized = v1.Normalize();
			Assert.IsTrue(FloatComparer.AreEqual(normalized.Magnitude, 1.0F));
		}

		[Test]
		public void TestDot()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			
			Assert.IsTrue(FloatComparer.AreEqual(v1.Dot(v2), 16.13F));
		}

		[Test]
		public void TestCross()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(-3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-52.39F, -37.14F, -15.04F);

			Assert.IsTrue(Vector3D.AreEqual(v1.Cross(v2), result));
		}

		[Test]
		public void TestAngleBetween()
		{
			const float halfPi = (float)Math.PI/2;
			const float tolerance = 1e-5F;
			var v1 = Vector3D.xUnit;
			var v2 = Vector3D.yUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v2 = -Vector3D.yUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v2 = Vector3D.zUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v2 = -Vector3D.zUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v1 = Vector3D.yUnit;
			v2 = Vector3D.zUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v2 = -Vector3D.zUnit;
			Assert.AreEqual(halfPi, v1.GetAngleBetween(v2), tolerance);

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			const float result = 1.32374F; //75.845 degrees.
			Assert.AreEqual(result, v1.GetAngleBetween(v2), tolerance);

			v1 = v2 = Vector3D.xUnit;
			Assert.AreEqual(0, v1.GetAngleBetween(v2), tolerance);

		}

		[Test]
		public void TestIsOrthogonalTo()
		{
			const float tolerance = 1e-5F;
			
			Vector3D v1 = Vector3D.xUnit;
			Vector3D v2 = Vector3D.yUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));
			v2 = -Vector3D.yUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));

			v2 = Vector3D.zUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));
			v2 = -Vector3D.zUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));

			v1 = Vector3D.yUnit;
			v2 = Vector3D.zUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));
			v2 = -Vector3D.zUnit;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, tolerance));

			v1 = Vector3D.xUnit;
			v2 = Vector3D.xUnit;
			Assert.IsFalse(v1.IsOrthogonalTo(v2, tolerance));
			v2 = -Vector3D.xUnit;
			Assert.IsFalse(v1.IsOrthogonalTo(v2, tolerance));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Assert.IsFalse(v1.IsOrthogonalTo(v2, tolerance));

			//Angle between is 75.845 degrees; "big" tolerance is 14.156
			const float bigTolerance = 0.24706F;
			Assert.IsTrue(v1.IsOrthogonalTo(v2, bigTolerance));
		}

		[Test]
		public void TestIsParallelTo()
		{
			const float tolerance = 1e-5F;

			Vector3D v1 = Vector3D.xUnit;
			Vector3D v2 = Vector3D.yUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));
			v2 = -Vector3D.yUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));

			v2 = Vector3D.zUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));
			v2 = -Vector3D.zUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));

			v1 = Vector3D.yUnit;
			v2 = Vector3D.zUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));
			v2 = -Vector3D.zUnit;
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));

			v1 = Vector3D.xUnit;
			v2 = Vector3D.xUnit;
			Assert.IsTrue(v1.IsParallelTo(v2, tolerance));
			v2 = -Vector3D.xUnit;
			Assert.IsTrue(v1.IsParallelTo(v2, tolerance));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Assert.IsFalse(v1.IsParallelTo(v2, tolerance));
		}

		[Test]
		public void TestLinePlaneIntersection()
		{
			Vector3D planeNormal = new Vector3D(1, 1, 1);
			Vector3D pointInPlane = new Vector3D(1, 1, 1);

			Vector3D point1 = new Vector3D(0.5F, 0.5F, 0.5F);
			Vector3D point2 = new Vector3D(1.5F, 1.5F, 1.5F);

			Vector3D expected = new Vector3D(1, 1, 1);
			Vector3D intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);

			// line segment intersects plane
			Assert.IsTrue(Vector3D.AreEqual(expected, intersection), "line-plane intersection is not what is expected!");

			// infinite line intersects plane
			point2 = -point2;
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.IsTrue(Vector3D.AreEqual(expected, intersection), "line-plane intersection is not what is expected!");

			// line segment does not intersect plane
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// line is in plane (no intersection)
			point1 = new Vector3D(1, 0, 0);
			point2 = new Vector3D(0, 0, 1);
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// line is in plane (no intersection)
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// intersection at infinity (sort of)
			point1 = new Vector3D(1, 0, 0);
			point2 = new Vector3D(0, 0, 0.99999991F);
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, true);
			Assert.AreEqual(intersection, null, "line-plane intersection is not what is expected!");

			// intersection at infinity (sort of), line segment does not intersect
			intersection = Vector3D.GetLinePlaneIntersection(planeNormal, pointInPlane, point1, point2, false);
			Assert.AreNotEqual(intersection, null, "line-plane intersection is not what is expected!");
		}
	}
}

#endif