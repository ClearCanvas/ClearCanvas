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

#if UNIT_TESTS

using System;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Rendering;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class ImageGraphicTests
	{
		[Test]
		public void TestBasicGrayscaleImage8()
		{
			const int rows = 8;
			const int cols = 8;

			var pixelData = new byte[rows*cols];
			using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 8, 8, 7, false, false, 1, 0, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		[Test]
		public void TestInvalidPixelDataGrayscaleImage8()
		{
			const int rows = 8;
			const int cols = 8;

			try
			{
				var badPixelData = new byte[rows*cols - 10];
				using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 8, 8, 7, false, false, 1, 0, () => badPixelData))
				using (var presentationImage = new TestPresentationImage(imageGraphic))
				{
					presentationImage.DrawToBitmap(100, 100);
				}
				Assert.Fail("Expected RenderingException was not thrown");
			}
			catch (RenderingException ex)
			{
				Trace.WriteLine(string.Format("Rendering exception thrown: {0}", ex.UserMessage));
			}
		}

		[Test]
		public void TestBasicGrayscaleImage16()
		{
			const int rows = 8;
			const int cols = 8;

			var pixelData = new byte[rows*cols*2];
			using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 16, 16, 15, false, false, 1, 0, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		[Test]
		public void TestInvalidPixelDataGrayscaleImage16()
		{
			const int rows = 8;
			const int cols = 8;

			try
			{
				var badPixelData = new byte[rows*cols*2 - 10];
				using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 16, 16, 15, false, false, 1, 0, () => badPixelData))
				using (var presentationImage = new TestPresentationImage(imageGraphic))
				{
					presentationImage.DrawToBitmap(100, 100);
				}
				Assert.Fail("Expected RenderingException was not thrown");
			}
			catch (RenderingException ex)
			{
				Trace.WriteLine(string.Format("Rendering exception thrown: {0}", ex.UserMessage));
			}
		}

		[Test]
		public void TestBasicColorImage()
		{
			const int rows = 8;
			const int cols = 8;

			var pixelData = new byte[rows*cols*4];
			using (var imageGraphic = new ColorImageGraphic(rows, cols, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		[Test]
		public void TestInvalidPixelDataColorImage()
		{
			const int rows = 8;
			const int cols = 8;

			try
			{
				var badPixelData = new byte[rows*cols*4 - 10];
				using (var imageGraphic = new ColorImageGraphic(rows, cols, () => badPixelData))
				using (var presentationImage = new TestPresentationImage(imageGraphic))
				{
					presentationImage.DrawToBitmap(100, 100);
				}
				Assert.Fail("Expected RenderingException was not thrown");
			}
			catch (RenderingException ex)
			{
				Trace.WriteLine(string.Format("Rendering exception thrown: {0}", ex.UserMessage));
			}
		}

		[Test]
		public void TestMaximumPracticalGrayscaleImage8()
		{
			if (Assert64Bit()) return;

			// due to the 2GB limit for a single buffer and the current ImageViewer rendering infrastructure,
			// it is not possible to render the largest possible valid-DICOM 8-bit grayscale image.
			// these values test the maximum renderable image under the current infrastructure assuming a 64-bit machine
			const int rows = (1 << 16) - 1;
			const int cols = (1 << 15);

			var pixelData = new byte[rows*cols];
			using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 8, 8, 7, false, false, 1, 0, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		[Test]
		public void TestMaximumPracticalGrayscaleImage16()
		{
			if (Assert64Bit()) return;

			// due to the 2GB limit for a single buffer and the current ImageViewer rendering infrastructure,
			// it is not possible to render the largest possible valid-DICOM 16-bit grayscale image.
			// these values test the maximum renderable image under the current infrastructure assuming a 64-bit machine
			const int rows = (1 << 15) - 1;
			const int cols = (1 << 15);

			var pixelData = new byte[rows*cols*2];
			using (var imageGraphic = new GrayscaleImageGraphic(rows, cols, 16, 16, 15, false, false, 1, 0, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		[Test]
		public void TestMaximumPracticalColorImage()
		{
			if (Assert64Bit()) return;

			// due to the 2GB limit for a single buffer and the current ImageViewer rendering infrastructure,
			// it is not possible to render the largest possible valid-DICOM color image.
			// these values test the maximum renderable image under the current infrastructure assuming a 64-bit machine
			const int rows = (1 << 15) - 1;
			const int cols = (1 << 14);

			var pixelData = new byte[rows*cols*4];
			using (var imageGraphic = new ColorImageGraphic(rows, cols, () => pixelData))
			using (var presentationImage = new TestPresentationImage(imageGraphic))
			{
				presentationImage.DrawToBitmap(100, 100);
			}
		}

		private static bool Assert64Bit()
		{
			if (IntPtr.Size < 64)
			{
				Assert.Ignore("This test is only valid on a 64-bit machine");
				return true;
			}
			return false;
		}

		private class TestPresentationImage : BasicPresentationImage
		{
			public TestPresentationImage(ImageGraphic imageGraphic)
				: base(imageGraphic) {}

			public override IPresentationImage CreateFreshCopy()
			{
				throw new NotImplementedException();
			}
		}
	}
}

#endif