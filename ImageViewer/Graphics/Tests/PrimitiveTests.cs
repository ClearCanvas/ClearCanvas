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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class PrimitiveTests
	{
		private const float _dimensionTolerance = 0.0001f;

		[Test]
		public void TestRectangleGetRoi()
		{
			// test null rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(5, 5);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(5, 5, 0, 0), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test positive rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(0, 0);
				rect.BottomRight = new PointF(1, 1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(0, 0, 1, 1), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test inverted rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(-1, -1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(-1, -1, 6, 6), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test negative width rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(-1, 6);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(-1, 5, 6, 1), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}

			// test negative height rectangle
			{
				var rect = new RectanglePrimitive();
				rect.TopLeft = new PointF(5, 5);
				rect.BottomRight = new PointF(6, -1);
				var roi = rect.GetRoi();
				Assert.IsNotNull(roi, "RectanglePrimitive.GetRoi() should never return null (rect = {0})", rect.Rectangle);
				AssertAreEqual(new RectangleF(5, -1, 1, 6), roi.BoundingBox, _dimensionTolerance, "RectanglePrimitive.GetRoi() returned wrong ROI (rect = {0})", rect.Rectangle);
			}
		}

		[Test]
		public void TestEllipseGetRoi()
		{
			// test null ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(5, 5);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(5, 5, 0, 0), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test positive ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(0, 0);
				ellipse.BottomRight = new PointF(1, 1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(0, 0, 1, 1), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test inverted ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(-1, -1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(-1, -1, 6, 6), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test negative width ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(-1, 6);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(-1, 5, 6, 1), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}

			// test negative height ellipse
			{
				var ellipse = new EllipsePrimitive();
				ellipse.TopLeft = new PointF(5, 5);
				ellipse.BottomRight = new PointF(6, -1);
				var roi = ellipse.GetRoi();
				Assert.IsNotNull(roi, "EllipsePrimitive.GetRoi() should never return null (rect = {0})", ellipse.Rectangle);
				AssertAreEqual(new RectangleF(5, -1, 1, 6), roi.BoundingBox, _dimensionTolerance, "EllipsePrimitive.GetRoi() returned wrong ROI (ellipse = {0})", ellipse.Rectangle);
			}
		}

		private static void AssertAreEqual(RectangleF expected, RectangleF actual, float delta, string message, params object[] args)
		{
			try
			{
				Assert.AreEqual(expected.Left, actual.Left, delta, message, args);
				Assert.AreEqual(expected.Right, actual.Right, delta, message, args);
				Assert.AreEqual(expected.Width, actual.Width, delta, message, args);
				Assert.AreEqual(expected.Height, actual.Height, delta, message, args);
			}
			catch (Exception)
			{
				Console.WriteLine("\tExpected: {0}", expected);
				Console.WriteLine("\t  Actual: {0}", actual);
				throw;
			}
		}
	}
}

#endif