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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	[TestFixture]
	public class LinearRoiTests : RoiTestBase<LinearRoiTests.Line>
	{
		[Test]
		public void TestLengthMeasurementOnAntiShapes()
		{
			// A--B
			// |  |
			// D--C
			// various lengths on a square figure
			// these expected values were independently computed by hand
			RectangleF rectangle = new RectangleF(77, 79, 100, 100);

			Line sideAtoB = new Line(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Top);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideAtoB, 100);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideAtoB.Reverse(), 100);

			Line sideBtoC = new Line(rectangle.Right, rectangle.Top, rectangle.Right, rectangle.Bottom);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideBtoC, 100);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideBtoC.Reverse(), 100);

			Line sideCtoD = new Line(rectangle.Right, rectangle.Bottom, rectangle.Left, rectangle.Bottom);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideCtoD, 100);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideCtoD.Reverse(), 100);

			Line sideDtoA = new Line(rectangle.Left, rectangle.Bottom, rectangle.Left, rectangle.Top);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideDtoA, 100);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, sideDtoA.Reverse(), 100);

			Line diagonalAtoC = new Line(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, diagonalAtoC, 141.421);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, diagonalAtoC.Reverse(), 141.421);

			Line diagonalBtoD = new Line(rectangle.Right, rectangle.Top, rectangle.Left, rectangle.Bottom);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, diagonalBtoD, 141.421);
			base.TestRoiLengthMeasurement(ImageKey.Simple01, diagonalBtoD.Reverse(), 141.421);
		}

		[Test]
		public void TestLengthMeasurementIsometricPixelAspectRatio()
		{
			//    A
			// c / \ b
			//  B---C
			//    a
			// various lengths on an equilateral triangle figure
			// these expected values were independently computed by hand
			PolygonalRoiTests.PolygonShape triangleShape = PolygonalRoiTests.PolygonShape.Triangle;

			Line sideA = GetTriangleSide(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, sideA, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, sideA, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, sideA, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, sideA, 8, 0.01, Units.Centimeters);

			Line sideB = GetTriangleSide(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, sideB, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, sideB, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, sideB, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, sideB, 8, 0.01, Units.Centimeters);

			Line sideC = GetTriangleSide(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, sideC, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, sideC, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, sideC, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, sideC, 8, 0.01, Units.Centimeters);

			// the altitude of these triangles in pixels can be computed normally by dropping a line perpendicular to the selected base
			// when computing in centimetres (and thus adjusting for pixel aspect ratio), the altitude is actually the bisector in the
			// triangle before adjusting for pixel aspect ratio - because these triangles are supposed to be equilateral, in which case
			// the bisectors are also the altitudes.

			Line altitudeA = GetTriangleAltitude(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, altitudeA, 173);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, altitudeA, 173);
			Line bisectorA = GetTriangleBisector(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, bisectorA, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, bisectorA, 6.9241, 0.01, Units.Centimeters);

			Line altitudeB = GetTriangleAltitude(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, altitudeB, 173);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, altitudeB, 173);
			Line bisectorB = GetTriangleBisector(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, bisectorB, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, bisectorB, 6.9241, 0.01, Units.Centimeters);

			Line altitudeC = GetTriangleAltitude(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect01, altitudeC, 173);
			base.TestRoiLengthMeasurement(ImageKey.Aspect02, altitudeC, 173);
			Line bisectorC = GetTriangleBisector(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect03, bisectorC, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect04, bisectorC, 6.9241, 0.01, Units.Centimeters);
		}

		[Test]
		public void TestLengthMeasurementAnisometricPixelAspectRatio4To3()
		{
			//    A
			// c / \ b
			//  B---C
			//    a
			// various lengths on an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			PolygonalRoiTests.PolygonShape triangleShape = PolygonalRoiTests.PolygonShape.TriangleWide;

			Line sideA = GetTriangleSide(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, sideA, 267);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, sideA, 267);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, sideA, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, sideA, 8, 0.01, Units.Centimeters);

			Line sideB = GetTriangleSide(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, sideB, 218.52);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, sideB, 218.52);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, sideB, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, sideB, 8, 0.01, Units.Centimeters);

			Line sideC = GetTriangleSide(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, sideC, 218.52);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, sideC, 218.52);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, sideC, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, sideC, 8, 0.01, Units.Centimeters);

			// the altitude of these triangles in pixels can be computed normally by dropping a line perpendicular to the selected base
			// when computing in centimetres (and thus adjusting for pixel aspect ratio), the altitude is actually the bisector in the
			// triangle before adjusting for pixel aspect ratio - because these triangles are supposed to be equilateral, in which case
			// the bisectors are also the altitudes.

			Line altitudeA = GetTriangleAltitude(triangleShape, 0); // this is the altitude of the triangle without accounting for pixel aspect ratio
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, altitudeA, 173);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, altitudeA, 173);
			Line bisectorA = GetTriangleBisector(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, bisectorA, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, bisectorA, 6.9241, 0.01, Units.Centimeters);

			Line altitudeB = GetTriangleAltitude(triangleShape, 1); // this is the altitude of the triangle without accounting for pixel aspect ratio
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, altitudeB, 211.38);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, altitudeB, 211.38);
			Line bisectorB = GetTriangleBisector(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, bisectorB, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, bisectorB, 6.9241, 0.01, Units.Centimeters);

			Line altitudeC = GetTriangleAltitude(triangleShape, 2); // this is the altitude of the triangle without accounting for pixel aspect ratio
			base.TestRoiLengthMeasurement(ImageKey.Aspect05, altitudeC, 211.38);
			base.TestRoiLengthMeasurement(ImageKey.Aspect06, altitudeC, 211.38);
			Line bisectorC = GetTriangleBisector(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect07, bisectorC, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect08, bisectorC, 6.9241, 0.01, Units.Centimeters);
		}

		[Test]
		public void TestLengthMeasurementAnisometricPixelAspectRatio3To4()
		{
			//    A
			// c / \ b
			//  B---C
			//    a
			// various lengths on an "equilateral" triangle figure (equilateral when adjusted for pixel aspect ratio)
			// these expected values were independently computed by hand
			PolygonalRoiTests.PolygonShape triangleShape = PolygonalRoiTests.PolygonShape.TriangleNarrow;

			Line sideA = GetTriangleSide(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, sideA, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, sideA, 200);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, sideA, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, sideA, 8, 0.01, Units.Centimeters);

			Line sideB = GetTriangleSide(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, sideB, 251.72);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, sideB, 251.72);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, sideB, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, sideB, 8, 0.01, Units.Centimeters);

			Line sideC = GetTriangleSide(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, sideC, 251.72);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, sideC, 251.72);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, sideC, 8, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, sideC, 8, 0.01, Units.Centimeters);

			// the altitude of these triangles in pixels can be computed normally by dropping a line perpendicular to the selected base
			// when computing in centimetres (and thus adjusting for pixel aspect ratio), the altitude is actually the bisector in the
			// triangle before adjusting for pixel aspect ratio - because these triangles are supposed to be equilateral, in which case
			// the bisectors are also the altitudes.

			Line altitudeA = GetTriangleAltitude(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, altitudeA, 231);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, altitudeA, 231);
			Line bisectorA = GetTriangleBisector(triangleShape, 0);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, bisectorA, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, bisectorA, 6.9241, 0.01, Units.Centimeters);

			Line altitudeB = GetTriangleAltitude(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, altitudeB, 183.54);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, altitudeB, 183.54);
			Line bisectorB = GetTriangleBisector(triangleShape, 1);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, bisectorB, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, bisectorB, 6.9241, 0.01, Units.Centimeters);

			Line altitudeC = GetTriangleAltitude(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect09, altitudeC, 183.54);
			base.TestRoiLengthMeasurement(ImageKey.Aspect10, altitudeC, 183.54);
			Line bisectorC = GetTriangleBisector(triangleShape, 2);
			base.TestRoiLengthMeasurement(ImageKey.Aspect11, bisectorC, 6.9241, 0.01, Units.Centimeters);
			base.TestRoiLengthMeasurement(ImageKey.Aspect12, bisectorC, 6.9241, 0.01, Units.Centimeters);
		}

		private static Line GetTriangleSide(PolygonalRoiTests.PolygonShape triangleShape, int indexOppositeAngle)
		{
			return new Line(triangleShape[(indexOppositeAngle + 1)%3], triangleShape[(indexOppositeAngle + 2)%3]);
		}

		private static Line GetTriangleAltitude(PolygonalRoiTests.PolygonShape triangleShape, int indexAngle)
		{
			PointF point = PointF.Empty;
			Vector.DistanceFromPointToLine(triangleShape[indexAngle], triangleShape[(indexAngle + 1)%3], triangleShape[(indexAngle + 2)%3], ref point);
			return new Line(triangleShape[indexAngle], point);
		}

		private static Line GetTriangleBisector(PolygonalRoiTests.PolygonShape triangleShape, int indexAngle)
		{
			return new Line(triangleShape[indexAngle], Vector.Midpoint(triangleShape[(indexAngle + 1)%3], triangleShape[(indexAngle + 2)%3]));
		}

		protected override string ShapeName
		{
			get { return "line"; }
		}

		protected override Line CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			const float offset = 10;
			return new Line(location, location + new SizeF(offset, offset));
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, Line shapeData)
		{
			PolylineGraphic graphic = new PolylineGraphic();
			overlayGraphics.OverlayGraphics.Add(graphic);
			graphic.CoordinateSystem = CoordinateSystem.Source;
			graphic.Points.Add(shapeData.Point1);
			graphic.Points.Add(shapeData.Point2);
			graphic.ResetCoordinateSystem();
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, Line shapeData)
		{
			return new LinearRoi(shapeData, image);
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, Line shapeData)
		{
			graphicsPath.AddLine(shapeData.Point1, shapeData.Point2);
		}

		public struct Line : IEnumerable<PointF>
		{
			public readonly PointF Point1;
			public readonly PointF Point2;
			private readonly string _description;

			public Line(PointF pt1, PointF pt2)
			{
				this.Point1 = pt1;
				this.Point2 = pt2;
				_description = null;
			}

			public Line(float x1, float y1, float x2, float y2)
			{
				this.Point1 = new PointF(x1, y1);
				this.Point2 = new PointF(x2, y2);
				_description = null;
			}

			public Line Reverse()
			{
				return new Line(this.Point2, this.Point1);
			}

			public IEnumerator<PointF> GetEnumerator()
			{
				yield return this.Point1;
				yield return this.Point2;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public override string ToString()
			{
				if (string.IsNullOrEmpty(_description))
					return string.Format("[{0}, {1}]", this.Point1, this.Point2);
				return _description;
			}
		}
	}
}

#endif
