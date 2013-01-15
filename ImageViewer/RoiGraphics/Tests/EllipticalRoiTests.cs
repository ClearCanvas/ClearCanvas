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
	public class EllipticalRoiTests : RoiTestBase<RectangleF>
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
			base.TestRoiContains(ImageKey.Simple01, rectangle, "anti_ellipse_1");

			RectangleF rectangle2 = new RectangleF(177, 79, -100, 100);
			base.TestRoiContains(ImageKey.Simple01, rectangle2, "anti_ellipse_2");

			RectangleF rectangle3 = new RectangleF(177, 179, -100, -100);
			base.TestRoiContains(ImageKey.Simple01, rectangle3, "anti_ellipse_3");
		}

		[Test]
		public void TestStatsCalculationSimple04()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(36, 100, 179, 90);
			base.TestRoiStatsCalculations(ImageKey.Simple04, rectangle, 138.206*Math.PI, 12653, 255, 0);
		}

		[Test]
		public void TestStatsCalculationSimple05()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(50, 75, 150, 150);
			base.TestRoiStatsCalculations(ImageKey.Simple05, rectangle, 150*Math.PI, 17671, 255, 0);
		}

		[Test]
		public void TestStatsCalculationSimple06()
		{
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(50, 75, 150, 150);
			base.TestRoiStatsCalculations(ImageKey.Simple06, rectangle, 150*Math.PI, 17671, 169.122, 59.527);
		}

		[Test]
		public void TestStatsCalculationIsometricPixelAspectRatio()
		{
			// inscribed ellipse within an equilateral triangle figure
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(67.333f, 109.667f, 115.333f, 115.333f);
			base.TestRoiStatsCalculations(ImageKey.Aspect01, rectangle, 115.333f*Math.PI, 10447.19, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Aspect02, rectangle, 115.333f*Math.PI, 10447.19, 255, 0);
			base.TestRoiStatsCalculations(ImageKey.Aspect03, rectangle, 14.493, 16.716, 255, 0, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect04, rectangle, 14.493, 16.716, 255, 0, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationAnisometricPixelAspectRatio4To3()
		{
			// inscribed ellipse within an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(89.777f, 109.667f, 153.777f, 115.333f);
			base.TestRoiStatsCalculations(ImageKey.Aspect05, rectangle, 192.15*Math.PI, 13929.59, 255, 1.25);
			base.TestRoiStatsCalculations(ImageKey.Aspect06, rectangle, 192.15*Math.PI, 13929.59, 255, 1.25);
			base.TestRoiStatsCalculations(ImageKey.Aspect07, rectangle, 14.493, 16.716, 255, 1.25, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect08, rectangle, 14.493, 16.716, 255, 1.25, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationAnisometricPixelAspectRatio3To4()
		{
			// inscribed ellipse within an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(67.333f, 146.222f, 115.333f, 153.777f);
			base.TestRoiStatsCalculations(ImageKey.Aspect09, rectangle, 192.15*Math.PI, 13929.59, 255, 2.5);
			base.TestRoiStatsCalculations(ImageKey.Aspect10, rectangle, 192.15*Math.PI, 13929.59, 255, 2.5);
			base.TestRoiStatsCalculations(ImageKey.Aspect11, rectangle, 14.493, 16.716, 255, 2.5, Units.Centimeters);
			base.TestRoiStatsCalculations(ImageKey.Aspect12, rectangle, 14.493, 16.716, 255, 2.5, Units.Centimeters);
		}

		[Test]
		public void TestStatsCalculationsConsistency()
		{
			base.TestRoiStatsCalculationConsistency();
		}

		protected override string ShapeName
		{
			get { return "ellipse"; }
		}

		protected override RectangleF CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			const int s = 10;
			return RectangleF.FromLTRB(location.X, location.Y, Math.Min(imageCols - 1, location.X + s), Math.Min(imageRows - 1, location.Y + s));
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, RectangleF shapeData)
		{
			EllipsePrimitive graphic = new EllipsePrimitive();
			overlayGraphics.OverlayGraphics.Add(graphic);
			graphic.CoordinateSystem = CoordinateSystem.Source;
			graphic.TopLeft = shapeData.Location;
			graphic.BottomRight = shapeData.Location + shapeData.Size;
			graphic.ResetCoordinateSystem();
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, RectangleF shapeData)
		{
			return new EllipticalRoi(shapeData, image);
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, RectangleF shapeData)
		{
			// we must do the positive rectangle conversion because the GDI GraphicsPath tests will fail on negative rectangles
			graphicsPath.AddEllipse(RectangleUtilities.ConvertToPositiveRectangle(shapeData));
		}
	}
}

#endif