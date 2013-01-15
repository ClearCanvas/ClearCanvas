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
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	[TestFixture]
	public class RectangularRoiTests : RoiTestBase<RectangleF>
	{
		[Test]
		public void TestContains()
		{
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiContains(ImageKey.Simple01, rectangle, null);
		}

		[Test]
		public void TestContainsAntiShapes()
		{
			RectangleF rectangle = new RectangleF(77, 179, 100, -100);
			base.TestRoiContains(ImageKey.Simple01, rectangle, "anti_rect_1");

			RectangleF rectangle2 = new RectangleF(177, 79, -100, 100);
			base.TestRoiContains(ImageKey.Simple01, rectangle2, "anti_rect_2");

			RectangleF rectangle3 = new RectangleF(177, 179, -100, -100);
			base.TestRoiContains(ImageKey.Simple01, rectangle3, "anti_rect_3");
		}

		[Test]
		public void TestStatsCalculationSimple01()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple01, rectangle, 400, 10000, 255, 0);
		}

		[Test]
		public void TestStatsCalculationSimple02()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple02, rectangle, 400, 10000, 128.3, 73.98);
		}

		[Test]
		public void TestStatsCalculationSimple03()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);
			base.TestRoiStatsCalculations(ImageKey.Simple03, rectangle, 400, 10000, 127.9, 52.05);
		}

		[Test]
		public void TestStatsCalculationIsometricPixelAspectRatio()
		{
			// inscribed square within an equilateral triangle figure
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(78.619f, 132.239f, 92.761f, 92.761f);
			base.TestRoiStatsCalculations(ImageKey.Aspect01, rectangle, 371.046, 8604.68, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Aspect02, rectangle, 371.046, 8604.68, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Aspect03, rectangle, 14.842, 13.767, 255, 0, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect04, rectangle, 14.842, 13.767, 255, 0, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationAnisometricPixelAspectRatio4To3()
		{
			// inscribed square within an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(104.826f, 132.239f, 123.682f, 92.761f);
			base.TestRoiStatsCalculations(ImageKey.Aspect05, rectangle, 432.89, 11472.87, 255, 1.25);
			base.TestRoiStatsCalculations(ImageKey.Aspect06, rectangle, 432.89, 11472.87, 255, 1.25);
			base.TestRoiStatsCalculations(ImageKey.Aspect07, rectangle, 14.842, 13.767, 255, 0, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect08, rectangle, 14.842, 13.767, 255, 0, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationAnisometricPixelAspectRatio3To4()
		{
			// inscribed ellipse within an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(78.619f, 176.318f, 92.761f, 123.682f);
			base.TestRoiStatsCalculations(ImageKey.Aspect09, rectangle, 432.89, 11472.87, 255, 7.00);
			base.TestRoiStatsCalculations(ImageKey.Aspect10, rectangle, 432.89, 11472.87, 255, 7.00);
			base.TestRoiStatsCalculations(ImageKey.Aspect11, rectangle, 14.842, 13.767, 255, 7.00, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect12, rectangle, 14.842, 13.767, 255, 7.00, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationsConsistency()
		{
			base.TestRoiStatsCalculationConsistency();
		}

		[Test]
		public void TestRoiContainedPixels()
		{
			TestRoiContainedPixels(new RectangleF(-5, -5, 7, 7), new[] {new PointF(0, 0), new PointF(0, 1), new PointF(1, 0), new PointF(1, 1)}, false, "Rect (+W,+H,-Loc)");
			TestRoiContainedPixels(new RectangleF(49, -5, 2, 7), new[] {new PointF(49, 0), new PointF(49, 1), new PointF(50, 0), new PointF(50, 1)}, false, "Rect (+W,+H,+-Loc)");
			TestRoiContainedPixels(new RectangleF(2, 49, -7, 2), new[] {new PointF(0, 49), new PointF(0, 50), new PointF(1, 49), new PointF(1, 50)}, false, "Rect (-W,+H)");
			TestRoiContainedPixels(new RectangleF(49, 105, 2, -7), new[] {new PointF(49, 98), new PointF(49, 99), new PointF(50, 98), new PointF(50, 99)}, false, "Rect (+W,-H)");
			TestRoiContainedPixels(new RectangleF(105, 51, -7, -2), new[] {new PointF(98, 49), new PointF(98, 50), new PointF(99, 49), new PointF(99, 50)}, false, "Rect (-W,-H)");
		}

		protected override RectangleF CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			const int s = 10;
			return RectangleF.FromLTRB(location.X, location.Y, Math.Min(imageCols - 1, location.X + s), Math.Min(imageRows - 1, location.Y + s));
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, RectangleF shapeData)
		{
			RectanglePrimitive graphic = new RectanglePrimitive();
			overlayGraphics.OverlayGraphics.Add(graphic);
			graphic.CoordinateSystem = CoordinateSystem.Source;
			graphic.TopLeft = shapeData.Location;
			graphic.BottomRight = shapeData.Location + shapeData.Size;
			graphic.ResetCoordinateSystem();
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, RectangleF shapeData)
		{
			return new RectangularRoi(shapeData, image);
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, RectangleF shapeData)
		{
			// we must do the positive rectangle conversion because the GDI GraphicsPath tests will fail on negative rectangles
			graphicsPath.AddRectangle(RectangleUtilities.ConvertToPositiveRectangle(shapeData));
		}

		protected override string ShapeName
		{
			get { return "rectangle"; }
		}
	}
}

#endif