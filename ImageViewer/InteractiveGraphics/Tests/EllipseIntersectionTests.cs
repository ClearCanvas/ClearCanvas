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

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Mathematics;
namespace ClearCanvas.ImageViewer.InteractiveGraphics.Tests
{
	[TestFixture]
	public class EllipseIntersectionTests
	{
		public EllipseIntersectionTests()
		{
		}

		private static void VerifyPointOnEllipse(float a, float b, PointF center, PointF intersection)
		{
			float x = intersection.X - center.X;
			float y = intersection.Y - center.Y;

			float rule = x * x / (a * a) + y * y / (b * b);
			Assert.IsTrue(FloatComparer.AreEqual(1F, rule), "Point is not on ellipse!");
		}
		
		[Test]
		public void SimpleTest()
		{
			//for a circle of radius=1, test point = 1,1, intesection is 1/sqrt(2), 1/sqrt(2)
			float a = 1F;
			float b = 1F;
			PointF center = new PointF(0, 0);
			float root2Inverse = 1F/(float)Math.Sqrt(2);
			PointF result = new PointF(root2Inverse, root2Inverse);
			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, new PointF(1, 1));

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestPointInside()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(-3F, -2F);
			PointF test = new PointF(-4F, -2.5F);
			PointF result = new PointF(-4.767767F, -2.883884F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestPointOutside()
		{
			float a = -2.5F;
			float b = -1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);
			PointF result = new PointF(4.767767F, 2.883884F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			VerifyPointOnEllipse(a, b, center, intersection);

			Assert.IsTrue(FloatComparer.AreEqual(result, intersection), "ellipse intersection point is not correct!");
		}


		[Test]
		public void TestPointNearZero()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(3.01F, 2.01F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);
			VerifyPointOnEllipse(a, b, center, intersection);
		}

		[Test]
		public void TestPointVeryCloseToZero()
		{
			float a = 2.5F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(3.0001F, 2.0001F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);
			
			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestMajorAxisZero()
		{
			float a = 0F;
			float b = 1.25F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}

		[Test]
		public void TestMinorAxisZero()
		{
			float a = 2.5F;
			float b = 0F;
			PointF center = new PointF(3F, 2F);
			PointF test = new PointF(13F, 7F);

			PointF intersection = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, test);

			//intersection is at center.
			Assert.IsTrue(FloatComparer.AreEqual(center, intersection), "ellipse intersection point is not correct!");
		}
	}
}

#endif