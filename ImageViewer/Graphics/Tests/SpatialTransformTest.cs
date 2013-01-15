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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class SpatialTransformTest
	{
		public SpatialTransformTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void DefaultTransform()
		{
			var transform = CreateTransform();

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void Scale()
		{
			var transform = CreateTransform();
			transform.Scale = 2.0f;

			Assert.AreEqual(transform.ScaleX, 2.0f);
			Assert.AreEqual(transform.ScaleY, 2.0f);

			// this tests the conversion to destination of (1,1)_src
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var actual = transform.ConvertToDestination(new SizeF(384, 512));
			Assert.AreEqual(768, actual.Width, 0.001f);
			Assert.AreEqual(1024, actual.Height, 0.001f);
		}

		[Test]
		public void FlipY()
		{
			var transform = CreateTransform();
			transform.FlipY = true;

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void FlipX()
		{
			var transform = CreateTransform();
			transform.FlipX = true;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void FlipXY()
		{
			var transform = CreateTransform();
			transform.FlipX = true;
			transform.FlipY = true;

			Assert.AreEqual(-1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(-1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(transform.Scale, 1.0f);
			Assert.AreEqual(transform.ScaleX, 1.0f);
			Assert.AreEqual(transform.ScaleY, 1.0f);
		}

		[Test]
		public void Rotate1()
		{
			var transform = CreateTransform();
			transform.RotationXY = 90;

			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[0]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(1.0f - transform.Transform.Elements[1]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(-1.0f - transform.Transform.Elements[2]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[3]) < 1.0E-05);
		}

		[Test]
		public void Rotate2()
		{
			var transform = CreateTransform();

			transform.RotationXY = 0;
			Assert.AreEqual(0, transform.RotationXY);
			transform.RotationXY = 90;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = 360;
			Assert.AreEqual(0, transform.RotationXY);
			transform.RotationXY = 450;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = -90;
			Assert.AreEqual(270, transform.RotationXY);
			transform.RotationXY = -270;
			Assert.AreEqual(90, transform.RotationXY);
			transform.RotationXY = -450;
			Assert.AreEqual(270, transform.RotationXY);
		}

		[Test]
		public void Translate()
		{
			var transform = CreateTransform();
			transform.TranslationX = 10;
			transform.TranslationY = 20;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(10.0f, transform.Transform.Elements[4]);
			Assert.AreEqual(20.0f, transform.Transform.Elements[5]);
		}

		[Test]
		public void SourceToDestinationRoundtrip()
		{
			// be sure to covert back and forth
			var transform = CreateTransform();

			PointF srcPt1 = new Point(100, 200);
			PointF dstPt = transform.ConvertToDestination(srcPt1);

			PointF srcPt2 = transform.ConvertToSource(dstPt);
			Assert.IsTrue(FloatComparer.AreEqual(srcPt1, srcPt2));
		}

		[Test]
		public void DestinationToSourceRoundtrip()
		{
			// be sure to covert back and forth
			var transform = CreateTransform();

			PointF dstPt1 = new Point(100, 200);
			PointF srcPt = transform.ConvertToSource(dstPt1);

			PointF dstPt2 = transform.ConvertToDestination(srcPt);
			Assert.IsTrue(FloatComparer.AreEqual(dstPt1, dstPt2));
		}

		[Test]
		public void CumulativeScale()
		{
			CompositeGraphic sceneGraph = new CompositeGraphic();

			CompositeGraphic graphic1 = new CompositeGraphic();
			graphic1.SpatialTransform.Scale = 2.0f;
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);

			CompositeGraphic graphic2 = new CompositeGraphic();
			graphic2.SpatialTransform.Scale = 3.0f;
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 3.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 3.0f);
			
			CompositeGraphic graphic3 = new CompositeGraphic();
			graphic3.SpatialTransform.Scale = 4.0f;
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 4.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 4.0f);

			graphic1.Graphics.Add(graphic2);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);

			graphic2.Graphics.Add(graphic3);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 24.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 24.0f);

			sceneGraph.Graphics.Add(graphic1);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeScale, 2.0f);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], 2.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeScale, 6.0f);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 6.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeScale, 24.0f);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 24.0f);
		}

		[Test]
		public void CumulativeRotation()
		{
			CompositeGraphic sceneGraph = new CompositeGraphic();
			CompositeGraphic graphic1 = new CompositeGraphic();
			CompositeGraphic graphic2 = new CompositeGraphic();
			CompositeGraphic graphic3 = new CompositeGraphic();

			sceneGraph.Graphics.Add(graphic1);
			graphic1.Graphics.Add(graphic2);
			graphic1.Graphics.Add(graphic3);

			sceneGraph.SpatialTransform.RotationXY = 90;
			graphic1.SpatialTransform.RotationXY = 90;
			graphic2.SpatialTransform.RotationXY = 100;
			graphic3.SpatialTransform.RotationXY = -270;

			//90
			Assert.AreEqual(sceneGraph.SpatialTransform.Transform.Elements[0], 0, 0.0001F);
			Assert.AreEqual(sceneGraph.SpatialTransform.Transform.Elements[1], 1, 0.0001F);
			Assert.AreEqual(sceneGraph.SpatialTransform.Transform.Elements[2], -1, 0.0001F);
			Assert.AreEqual(sceneGraph.SpatialTransform.Transform.Elements[3], 0, 0.0001F);

			//90 + 90
			Assert.AreEqual(graphic1.SpatialTransform.Transform.Elements[0], 0, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.Transform.Elements[1], 1, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.Transform.Elements[2], -1, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.Transform.Elements[3], 0, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[0], -1, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[1], 0, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[2], 0, 0.0001F);
			Assert.AreEqual(graphic1.SpatialTransform.CumulativeTransform.Elements[3], -1, 0.0001F);

			//90 + 90 - 270 (+ 360)
			Assert.AreEqual(graphic3.SpatialTransform.Transform.Elements[0], 0, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.Transform.Elements[1], 1, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.Transform.Elements[2], -1, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.Transform.Elements[3], 0, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[0], 0, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[1], -1, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[2], 1, 0.0001F);
			Assert.AreEqual(graphic3.SpatialTransform.CumulativeTransform.Elements[3], 0, 0.0001F);

			//90 + 90 + 100
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[0], -0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[1], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[2], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[3], -0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[1], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[2], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[3], 0.1736, 0.0001F);

			graphic2.SpatialTransform.FlipX = true;
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[0], -0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[1], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[2], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[3], 0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], 0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[1], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[2], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[3], -0.1736, 0.0001F);

			graphic2.SpatialTransform.FlipY = true;
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[0], 0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[1], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[2], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.Transform.Elements[3], 0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[0], -0.1736, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[1], 0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[2], -0.9848, 0.0001F);
			Assert.AreEqual(graphic2.SpatialTransform.CumulativeTransform.Elements[3], -0.1736, 0.0001F);
		}

		private static SpatialTransform CreateTransform()
		{
			return new CompositeGraphic().SpatialTransform;
		}

		//[Test]
		//public void QuadrantTransformSameImageSize()
		//{
		//    SpatialTransform transform = new SpatialTransform();
		//    transform.SourceRectangle = new Rectangle(0, 0, 7, 11);
		//    transform.DestinationRectangle = new Rectangle(0, 0, 7, 11);

		//    int srcWidth = transform.SourceRectangle.Width;
		//    int srcHeight = transform.SourceRectangle.Height;
		//    int dstWidth = transform.DestinationRectangle.Width;
		//    int dstHeight = transform.DestinationRectangle.Height;
		//    float oneQuarter = 0.25f;
		//    float threeQuarters = 0.75f;

		//    PointF destinationPoint = new PointF((dstWidth * oneQuarter), (dstHeight * oneQuarter));
		//    PointF sourcePointExpected = new PointF((srcWidth * oneQuarter), (srcHeight * oneQuarter));
		//    PointF SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    PointF destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = true;

		//    sourcePointExpected.X = (srcWidth * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = false;
		//    transform.FlipVertical = true;

		//    sourcePointExpected.X = (srcWidth * oneQuarter);
		//    sourcePointExpected.Y = (srcHeight * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);

		//    transform.FlipHorizontal = true;
		//    transform.FlipVertical = true;

		//    sourcePointExpected.X = (srcWidth * threeQuarters);
		//    sourcePointExpected.Y = (srcHeight * threeQuarters);
		//    SourcePointCalculated = transform.ConvertToSource(destinationPoint);
		//    destinationPointBackCalculated = transform.ConvertToDestination(SourcePointCalculated);
		//    Assert.IsTrue(sourcePointExpected == SourcePointCalculated);
		//    Assert.IsTrue(destinationPointBackCalculated == destinationPoint);
		//}

		//[Test]
		//public void TransformVectorsTest()
		//{
		//    PointF[] originalVectors = new PointF[] { new PointF(1, 1), new PointF(1, -1), new PointF(-1, 1), new PointF(-1, -1) };

		//    SpatialTransform transform = new SpatialTransform();
		//    transform.SourceRectangle = new Rectangle(0, 0, 10, 10);
		//    transform.DestinationRectangle = new Rectangle(0, 0, 15, 25);

		//    PointF[] testVectors;
		//    PointF[] resultVectors;

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(1.5F, 1.5F), new PointF(1.5F, -1.5F), new PointF(-1.5F, 1.5F), new PointF(-1.5F, -1.5F) };
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-1.5F, 1.5F), new PointF(-1.5F, -1.5F), new PointF(1.5F, 1.5F), new PointF(1.5F, -1.5F) };
		//    transform.FlipHorizontal = true;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-1.5F, -1.5F), new PointF(-1.5F, 1.5F), new PointF(1.5F, -1.5F), new PointF(1.5F, 1.5F) };
		//    transform.FlipVertical = true;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-3F, -3F), new PointF(-3F, 3F), new PointF(3F, -3F), new PointF(3F, 3F) };
		//    transform.ScaleToFit = false;
		//    transform.Scale = 3F;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    testVectors = (PointF[])originalVectors.Clone();
		//    resultVectors = new PointF[] { new PointF(-3F, 3F), new PointF(3F, 3F), new PointF(-3F, -3F), new PointF(3F, -3F) };
		//    transform.Rotation = -90;
		//    transform.ConvertVectorsToDestination(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));

		//    resultVectors = (PointF[])originalVectors.Clone();
		//    transform.ConvertVectorsToSource(testVectors);
		//    Assert.IsTrue(VerifyTestVectors(testVectors, resultVectors));
		//}

		//private bool VerifyTestVectors(PointF[] transformed, PointF[] expected)
		//{
		//    for (int i = 0; i < transformed.Length; ++i)
		//    {
		//        double xDifference = Math.Abs(transformed[i].X - expected[i].X);
		//        double yDifference = Math.Abs(transformed[i].Y - expected[i].Y);

		//        if (xDifference > 0.001 || yDifference > 0.001)
		//            return false;
		//    }

		//    return true;
		//}

	}
}

#endif