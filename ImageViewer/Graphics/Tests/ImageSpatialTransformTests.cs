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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics;
using NUnit.Framework;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class ImageSpatialTransformTests
	{
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
			transform.ScaleToFit = false;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
		}

		[Test]
		public void Scale()
		{
			var transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(transform.ScaleX, 2.0f);
			Assert.AreEqual(transform.ScaleY, 2.0f);
		}

		[Test]
		public void ScaleIsotropicPixel()
		{
			var transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(768, 1024), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelSpacing2To1()
		{
			var transform = CreateTransform(1, 2, 0, 0);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(768, 2048), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelSpacing1To2()
		{
			var transform = CreateTransform(2, 1, 0, 0);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(1536, 1024), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelSpacing4To3()
		{
			var transform = CreateTransform(0.3, 0.4, 0, 0);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2 + 2/3f, transform.Transform.Elements[3], 0.000001f);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(768, 1365 + 1/3f), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelSpacing3To4()
		{
			var transform = CreateTransform(0.4, 0.3, 0, 0);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2 + 2/3f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(1024, 1024), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio2To1()
		{
			var transform = CreateTransform(0, 0, 1, 2);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(4.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(768, 2048), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio1To2()
		{
			var transform = CreateTransform(0, 0, 2, 1);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(4.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(1536, 1024), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio4To3()
		{
			var transform = CreateTransform(0, 0, 3, 4);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2 + 2/3f, transform.Transform.Elements[3], 0.000001f);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(768, 1365 + 1/3f), screenSize);
		}

		[Test]
		public void ScaleAnisotropicPixelAspectRatio3To4()
		{
			var transform = CreateTransform(0, 0, 4, 3);
			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			// this tests the conversion to destination of (1,1)_src
			// the adopted convention is that the image is upsampled to normalize the pixel aspect ratio before the scale is applied
			// i.e. both a 250x333 image at 0.3\0.4 and a 333x250 image at 0.4\0.3 are normalized to 333x333 images, which is taken to be scale 1.0
			Assert.AreEqual(2 + 2/3f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreEqual(new SizeF(1024, 1024), screenSize);
		}

		[Test]
		public void FlipY()
		{
			ImageSpatialTransform transform = CreateTransform();
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
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
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
			ImageSpatialTransform transform = CreateTransform();
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
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;
			transform.RotationXY = 90;

			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[0]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(1.0f - transform.Transform.Elements[1]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(-1.0f - transform.Transform.Elements[2]) < 1.0E-05);
			Assert.IsTrue(Math.Abs(0.0f - transform.Transform.Elements[3]) < 1.0E-05);
		}

		[Test]
		public void Rotate2()
		{
			ImageSpatialTransform transform = CreateTransform();
			transform.ScaleToFit = false;

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
			ImageSpatialTransform transform = CreateTransform();
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
		public void ScaleToFit()
		{
			ImageSpatialTransform transform = CreateTransform();

			transform.ScaleToFit = false;
			transform.Scale = 2.0f;

			Assert.AreEqual(2.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(2.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(2.0f, transform.Scale);
			Assert.AreEqual(2.0f, transform.ScaleX);
			Assert.AreEqual(2.0f, transform.ScaleY);

			transform.ScaleToFit = true;

			Assert.AreEqual(1.0f, transform.Transform.Elements[0]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[1]);
			Assert.AreEqual(0.0f, transform.Transform.Elements[2]);
			Assert.AreEqual(1.0f, transform.Transform.Elements[3]);
			Assert.AreEqual(1.0f, transform.Scale);
			Assert.AreEqual(1.0f, transform.ScaleX);
			Assert.AreEqual(1.0f, transform.ScaleY);
		}

		[Test]
		public void ScaleToFitIsotropicPixel()
		{
			var transform = CreateTransform();
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 768, 1024);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(2.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(2.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(0.75f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelSpacing2To1()
		{
			var transform = CreateTransform(0.1, 0.2, 0, 0);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 768, 2048);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(2.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(4.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(0.375f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelSpacing1To2()
		{
			var transform = CreateTransform(0.2, 0.1, 0, 0);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1536, 1024);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(4.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(2.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(1.5f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelSpacing4To3()
		{
			var transform = CreateTransform(0.3, 0.4, 0, 0);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1152, 2048);
			Assert.AreEqual(3.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(3.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(4.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(0.5625f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelSpacing3To4()
		{
			var transform = CreateTransform(0.4, 0.3, 0, 0);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1024, 1024);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(2 + 2/3f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(2.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(1f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelAspectRatio2To1()
		{
			var transform = CreateTransform(0, 0, 0.1, 0.2);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 768, 2048);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(2.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(4.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(0.375f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelAspectRatio1To2()
		{
			var transform = CreateTransform(0, 0, 0.2, 0.1);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1536, 1024);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(4.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(2.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(1.5f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelAspectRatio4To3()
		{
			var transform = CreateTransform(0, 0, 0.3, 0.4);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1152, 2048);
			Assert.AreEqual(3.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(3.0f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(4.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(0.5625f, screenSize);
		}

		[Test]
		public void ScaleToFitAnisotropicPixelAspectRatio3To4()
		{
			var transform = CreateTransform(0, 0, 0.4, 0.3);
			transform.ScaleToFit = true;

			transform.ClientRectangle = new Rectangle(0, 0, 1024, 1024);
			Assert.AreEqual(2.0f, transform.Scale, 0.001f, "wrong normalized scale");
			Assert.AreEqual(2 + 2/3f, transform.ScaleX, 0.001f, "wrong scale along X");
			Assert.AreEqual(2.0f, transform.ScaleY, 0.001f, "wrong scale along Y");

			var screenSize = transform.ConvertToDestination(new SizeF(384, 512));
			AssertAreSimilar(1f, screenSize);
		}

		[Test]
		public void TileFittingIsotropicPixel()
		{
			var transform = CreateTransform();
			transform.ScaleToFit = true;

			// test a client area with exactly the same image aspect ratio
			Trace.WriteLine(string.Format("same image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 192, 256);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(0, 0, 192, 256), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a different image aspect ratio
			Trace.WriteLine(string.Format("different image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 256, 192);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(56, 0, 144, 192), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a rotated image aspect ratio, and the image is rotated
			Trace.WriteLine(string.Format("rotated image aspect ratio, rotated image"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 256, 192);
				transform.RotationXY = 90;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(256, 0, -256, 192), screenRect);
				AssertScaleIsConsistent(transform);
			}
		}

		[Test]
		public void TileFittingAnisotropicPixel2To1()
		{
			var transform = CreateTransform(0.1, 0.2, 0, 0);
			transform.ScaleToFit = true;

			// test a client area with exactly the same image aspect ratio
			Trace.WriteLine(string.Format("same image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 192, 512);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(0, 0, 192, 512), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a different image aspect ratio
			Trace.WriteLine(string.Format("different image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 512, 192);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(220, 0, 72, 192), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a rotated image aspect ratio, and the image is rotated
			Trace.WriteLine(string.Format("rotated image aspect ratio, rotated image"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 512, 192);
				transform.RotationXY = 90;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(512, 0, -512, 192), screenRect);
				AssertScaleIsConsistent(transform);
			}
		}

		[Test]
		public void TileFittingAnisotropicPixel1To2()
		{
			var transform = CreateTransform(0.2, 0.1, 0, 0);
			transform.ScaleToFit = true;

			// test a client area with exactly the same image aspect ratio
			Trace.WriteLine(string.Format("same image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 384, 256);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(0, 0, 384, 256), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a different image aspect ratio
			Trace.WriteLine(string.Format("different image aspect ratio"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 256, 384);
				transform.RotationXY = 0;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(0, 106 + 2/3f, 256, 170 + 2/3f), screenRect);
				AssertScaleIsConsistent(transform);
			}

			// test a client area with a rotated image aspect ratio, and the image is rotated
			Trace.WriteLine(string.Format("rotated image aspect ratio, rotated image"), "UNIT_TESTS");
			{
				transform.ClientRectangle = new Rectangle(0, 0, 256, 384);
				transform.RotationXY = 90;
				var screenRect = transform.ConvertToDestination(new Rectangle(0, 0, 384, 512));
				AssertAreEqual(new RectangleF(256, 0, -256, 384), screenRect);
				AssertScaleIsConsistent(transform);
			}
		}

		[Test]
		public void SourceToDestinationRoundtrip()
		{
			// be sure to covert back and forth
			CompositeImageGraphic graphic = new CompositeImageGraphic(3062, 3732);
			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(6, 6, 493, 626);

			PointF srcPt1 = new Point(100, 200);
			PointF dstPt = transform.ConvertToDestination(srcPt1);

			PointF srcPt2 = transform.ConvertToSource(dstPt);
			Assert.IsTrue(FloatComparer.AreEqual(srcPt1, srcPt2));
		}

		[Test]
		public void DestinationToSourceRoundtrip()
		{
			// be sure to covert back and forth
			CompositeImageGraphic graphic = new CompositeImageGraphic(3062, 3732);
			ImageSpatialTransform transform = (ImageSpatialTransform) graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(6, 6, 493, 626);

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

		[Test]
		public void CumulativeTransform()
		{
			//this will cause a non-1:1 scale in y
			CompositeImageGraphic composite = new CompositeImageGraphic(512, 384, 0, 0, 3, 4);
			ImageSpatialTransform transform = (ImageSpatialTransform)composite.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);
			transform.ScaleToFit = false;
			transform.Scale = 2;

			CompositeGraphic graphic = new CompositeGraphic();
			composite.Graphics.Add(graphic);
			graphic.SpatialTransform.RotationXY = 30;

			Assert.AreEqual(graphic.SpatialTransform.Transform.Elements[0], 0.8660, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.Transform.Elements[1], 0.5, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.Transform.Elements[2], -0.5, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.Transform.Elements[3], 0.866, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.CumulativeTransform.Elements[0], 1.7321, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.CumulativeTransform.Elements[1], 1.3333, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.CumulativeTransform.Elements[2], -1, 0.0001F);
			Assert.AreEqual(graphic.SpatialTransform.CumulativeTransform.Elements[3], 2.3094, 0.0001F);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void TestRotationConstraints()
		{
			SynchronizationContext oldContext = SynchronizationContext.Current;
			if (oldContext == null)
				SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			try
			{
				CompositeGraphic sceneGraph = CreateTestSceneGraph();
				CompositeImageGraphic imageComposite = (CompositeImageGraphic) sceneGraph.Graphics[0];
				ImageGraphic image = (ImageGraphic) imageComposite.Graphics[0];
				CompositeGraphic primitiveOwner = (CompositeGraphic) imageComposite.Graphics[1];
				Graphic primitive = (Graphic) primitiveOwner.Graphics[0];

				RoiGraphic roiGraphic = (RoiGraphic) imageComposite.Graphics[2];

				try
				{
					sceneGraph.SpatialTransform.RotationXY = 90;
					imageComposite.SpatialTransform.RotationXY = 90;
					primitiveOwner.SpatialTransform.RotationXY = 10;
					primitive.SpatialTransform.RotationXY = -20;
				}
				catch (Exception)
				{
					Assert.Fail("These operations should not throw an exception!");
				}

				Matrix cumulativeTransform;
				try
				{
					imageComposite.SpatialTransform.RotationXY = 30;
					//should throw; no non-90 degree rotations allowed on an image
					cumulativeTransform = image.SpatialTransform.CumulativeTransform;
					Assert.Fail("expected exception not thrown!");
				}
				catch (ArgumentException)
				{
					imageComposite.SpatialTransform.RotationXY = 90;
				}

				roiGraphic.SpatialTransform.RotationXY = 100;
				//should throw; no rotation allowed on a roi
				cumulativeTransform = roiGraphic.SpatialTransform.CumulativeTransform;
			}
			finally
			{
				if (oldContext != SynchronizationContext.Current)
					SynchronizationContext.SetSynchronizationContext(oldContext);
			}
		}

		private static CompositeGraphic CreateTestSceneGraph()
		{
			CompositeGraphic sceneGraph = new CompositeGraphic();
			ImageSpatialTransform imageTransform = CreateTransform();

			sceneGraph.Graphics.Add(imageTransform.OwnerGraphic);

			CompositeGraphic composite = new CompositeGraphic();
			Graphic leaf = new LinePrimitive();
			composite.Graphics.Add(leaf);
			((CompositeImageGraphic)imageTransform.OwnerGraphic).Graphics.Add(composite);

			RoiGraphic roiGraphic = new RoiGraphic(new EllipsePrimitive());
			((CompositeImageGraphic)imageTransform.OwnerGraphic).Graphics.Add(roiGraphic);

			return sceneGraph;
		}

		/// <summary>
		/// Creates a spatial transform for a 384x512 image (i.e. 512 rows, 384 columns).
		/// </summary>
		private static ImageSpatialTransform CreateTransform()
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384);
			GrayscaleImageGraphic image = new GrayscaleImageGraphic(512, 384);
			graphic.Graphics.Add(image);

			ImageSpatialTransform transform = (ImageSpatialTransform)graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);
			return transform;
		}

		/// <summary>
		/// Creates a spatial transform for a 384x512 image (i.e. 512 rows, 384 columns) with the specified pixel spacing and/or pixel aspect ratio.
		/// </summary>
		private static ImageSpatialTransform CreateTransform(double pixelSpacingX, double pixelSpacingY, double pixelAspectRatioX, double pixelAspectRatioY)
		{
			CompositeImageGraphic graphic = new CompositeImageGraphic(512, 384, pixelSpacingX, pixelSpacingY, pixelAspectRatioX, pixelAspectRatioY);
			GrayscaleImageGraphic image = new GrayscaleImageGraphic(512, 384);
			graphic.Graphics.Add(image);

			ImageSpatialTransform transform = (ImageSpatialTransform) graphic.SpatialTransform;
			transform.ClientRectangle = new Rectangle(0, 0, 384, 512);
			return transform;
		}

		/// <summary>
		/// Clones a spatial transform.
		/// </summary>
		private static T CloneTransform<T>(T transform) where T : SpatialTransform
		{
			return (T) ((IGraphic) CloneBuilder.Clone(transform.OwnerGraphic)).SpatialTransform;
		}

		/// <summary>
		/// Asserts that the <paramref name="actual"/> dimensions are equal to the <paramref name="expected"/> dimensions.
		/// </summary>
		private static void AssertAreEqual(SizeF expected, SizeF actual)
		{
			try
			{
				const float tolerance = 0.001f;
				Assert.AreEqual(expected.Width, actual.Width, tolerance, "wrong dimensions: width");
				Assert.AreEqual(expected.Height, actual.Height, tolerance, "wrong dimensions: height");
			}
			catch (Exception)
			{
				Trace.WriteLine(string.Format("Expected: {0}", expected), "UNIT_TESTS");
				Trace.WriteLine(string.Format("Actual: {0}", actual), "UNIT_TESTS");
				throw;
			}
		}

		/// <summary>
		/// Asserts that the <paramref name="actual"/> rectangle is equal to the <paramref name="expected"/> rectangle.
		/// </summary>
		private static void AssertAreEqual(RectangleF expected, RectangleF actual)
		{
			try
			{
				const float tolerance = 0.001f;
				Assert.AreEqual(expected.X, actual.X, tolerance, "wrong rectangle: locX");
				Assert.AreEqual(expected.Y, actual.Y, tolerance, "wrong rectangle: locY");
				Assert.AreEqual(expected.Width, actual.Width, tolerance, "wrong rectangle: width");
				Assert.AreEqual(expected.Height, actual.Height, tolerance, "wrong rectangle: height");
			}
			catch (Exception)
			{
				Trace.WriteLine(string.Format("Expected: {0}", expected), "UNIT_TESTS");
				Trace.WriteLine(string.Format("Actual: {0}", actual), "UNIT_TESTS");
				throw;
			}
		}

		/// <summary>
		/// Asserts that the <paramref name="actual"/> dimensions have the <paramref name="expected"/> aspect ratio.
		/// </summary>
		private static void AssertAreSimilar(float expected, SizeF actual)
		{
			const float tolerance = 0.001f;
			Assert.AreEqual(expected, actual.Width/actual.Height, tolerance, "wrong image aspect ratio");
		}

		/// <summary>
		/// Asserts that the resulting scale computed by ScaleToFit is consistent with ScaleXY.
		/// </summary>
		private static void AssertScaleIsConsistent(ImageSpatialTransform transform)
		{
			const float tolerance = 0.001f;

			var transformToFit = CloneTransform(transform);
			transformToFit.ScaleToFit = true;

			var transformXy = CloneTransform(transform);
			transformXy.ScaleToFit = false;
			transformXy.Scale = transform.Scale;

			var screenSizeToFit = transformToFit.ConvertToDestination(new SizeF(384, 512));
			var screenSizeXy = transformXy.ConvertToDestination(new SizeF(384, 512));
			try
			{
				Assert.AreEqual(screenSizeXy.Width, screenSizeToFit.Width, tolerance, "ScaleToFit and ScaleXY produce different dimensions at ZOOM={0:f6}: width", transform.Scale);
				Assert.AreEqual(screenSizeXy.Height, screenSizeToFit.Height, tolerance, "ScaleToFit and ScaleXY produce different dimensions at ZOOM={0:f6}: height", transform.Scale);
			}
			catch (Exception)
			{
				Trace.WriteLine(string.Format("ScaleXY: {0}", screenSizeXy), "UNIT_TESTS");
				Trace.WriteLine(string.Format("ScaleToFit: {0}", screenSizeToFit), "UNIT_TESTS");
				throw;
			}
		}
	}
}

#endif