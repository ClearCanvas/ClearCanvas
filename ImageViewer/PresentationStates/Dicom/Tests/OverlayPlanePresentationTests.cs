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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	[TestFixture]
	public class OverlayPlanePresentationTests
	{
		private static GeneratedOverlayTestImages _testImages;

		[TestFixtureSetUp]
		public void Init()
		{
			_testImages = new GeneratedOverlayTestImages();
		}

		[Test]
		public void TestSanity()
		{
			// make sure that the functions that supporting testing here can actually tell when something is wrong
			var file = _testImages.ImageDataOverlay;
			using (var images = CreateImages(file))
			{
				AssertFrameIdentity(1, 1, images[0], "assertion 1 yielded false negative");

				try
				{
					AssertFrameIdentity(0, 1, images[0], "2");
					Assert.Fail("assertion 2 yielded false positive");
				}
				catch (Exception)
				{
					// expected
				}

				try
				{
					AssertFrameIdentity(1, 5, images[0], "3");
					Assert.Fail("assertion 3 yielded false positive");
				}
				catch (Exception)
				{
					// expected
				}

				try
				{
					AssertFrameIdentity(1, null, images[0], "4");
					Assert.Fail("assertion 4 yielded false positive");
				}
				catch (Exception)
				{
					// expected
				}
			}
		}

		[Test]
		public void TestMultiframeImageEmbeddedOverlay()
		{
			var file = _testImages.MultiframeImageEmbeddedOverlay;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should be exactly 1-to-1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with embedded overlay image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlay()
		{
			var file = _testImages.MultiframeImageDataOverlay;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should be exactly 1-to-1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with data overlay image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayDifferentSize()
		{
			var file = _testImages.MultiframeImageDataOverlayDifferentSize;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should be exactly 1-to-1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with different size data overlay image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayNotMultiframe()
		{
			var file = _testImages.MultiframeImageDataOverlayNotMultiframe;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should all have overlay #1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, 1, images[n], "multiframe image with non-multiframe data overlay image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayLowSubrangeImplicitOrigin()
		{
			var file = _testImages.MultiframeImageDataOverlayLowSubrangeImplicitOrigin;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should all have 1-to-1 overlays
				for (int n = 0; n < 7; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with data overlays on subrange 1-7 (encoded as implicit origin) image #{0}", n + 1);

				// these frames should not have overlays
				for (int n = 7; n < 17; n++)
					AssertFrameIdentity(n + 1, null, images[n], "multiframe image with data overlays on subrange 1-7 (encoded as implicit origin) image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayLowSubrange()
		{
			var file = _testImages.MultiframeImageDataOverlayLowSubrange;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should all have 1-to-1 overlays
				for (int n = 0; n < 7; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with data overlays on subrange 1-7 image #{0}", n + 1);

				// these frames should not have overlays
				for (int n = 7; n < 17; n++)
					AssertFrameIdentity(n + 1, null, images[n], "multiframe image with data overlays on subrange 1-7 image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayMidSubrange()
		{
			var file = _testImages.MultiframeImageDataOverlayMidSubrange;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should all have 1-to-1 overlays
				for (int n = 5; n < 12; n++)
					AssertFrameIdentity(n + 1, n + 1 - 5, images[n], "multiframe image with data overlays on subrange 6-12 image #{0}", n + 1);

				// these frames should not have overlays
				for (int n = 12; n < 17; n++)
					AssertFrameIdentity(n + 1, null, images[n], "multiframe image with data overlays on subrange 6-12 image #{0}", n + 1);

				// these frames should not have overlays
				for (int n = 0; n < 5; n++)
					AssertFrameIdentity(n + 1, null, images[n], "multiframe image with data overlays on subrange 6-12 image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayHighSubrange()
		{
			var file = _testImages.MultiframeImageDataOverlayHighSubrange;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should all have 1-to-1 overlays
				for (int n = 10; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1 - 10, images[n], "multiframe image with data overlays on subrange 11-17 image #{0}", n + 1);

				// these frames should not have overlays
				for (int n = 0; n < 10; n++)
					AssertFrameIdentity(n + 1, null, images[n], "multiframe image with data overlays on subrange 11-17 image #{0}", n + 1);
			}
		}

		[Test]
		public void TestImageEmbeddedOverlay()
		{
			var file = _testImages.ImageEmbeddedOverlay;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should be exactly 1-to-1
				AssertFrameIdentity(1, 1, images[0], "image with embedded overlay image #{0}", 1);
			}
		}

		[Test]
		public void TestImageDataOverlay()
		{
			var file = _testImages.ImageDataOverlay;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should be exactly 1-to-1
				AssertFrameIdentity(1, 1, images[0], "image with data overlay image #{0}", 1);
			}
		}

		[Test]
		public void TestImageDataOverlayDifferentSize()
		{
			var file = _testImages.ImageDataOverlayDifferentSize;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should be exactly 1-to-1
				AssertFrameIdentity(1, 1, images[0], "image with different size data overlay image #{0}", 1);
			}
		}

		[Test]
		public void TestImageDataOverlayMultiframe()
		{
			var file = _testImages.ImageDataOverlayMultiframe;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should not have an overlay displayed
				AssertFrameIdentity(1, null, images[0], "image with multiframe data overlay image #{0}", 1);
			}

			// should check that there is an error displayed
		}

		[Test]
		public void TestImageEmbeddedOverlay8Bit()
		{
			var file = _testImages.ImageEmbeddedOverlay8Bit;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should be exactly 1-to-1
				AssertFrameIdentity(1, 1, images[0], "image with embedded overlay image #{0}", 1);
			}
		}

		[Test]
		public void TestImageDataOverlayOWAttribute()
		{
			var file = _testImages.ImageDataOverlayOWAttribute;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(1, images.Count, "should be exactly 1 frames in this instance");

				// that frames should be exactly 1-to-1
				AssertFrameIdentity(1, 1, images[0], "image with data overlay image #{0}", 1);
			}
		}

		[Test]
		public void TestMultiframeImageEmbeddedOverlay8Bit()
		{
			var file = _testImages.MultiframeImageEmbeddedOverlay8Bit;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should be exactly 1-to-1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with embedded overlay image #{0}", n + 1);
			}
		}

		[Test]
		public void TestMultiframeImageDataOverlayOWAttribute()
		{
			var file = _testImages.MultiframeImageDataOverlayOWAttribute;
			using (var images = CreateImages(file))
			{
				Assert.AreEqual(17, images.Count, "should be exactly 17 frames in this instance");

				// these frames should be exactly 1-to-1
				for (int n = 0; n < 17; n++)
					AssertFrameIdentity(n + 1, n + 1, images[n], "multiframe image with data overlay image #{0}", n + 1);
			}
		}

		private static void AssertFrameIdentity(int expectedImageFrameNumber, int? expectedOverlayFrameNumber, IPresentationImage image, string message, params object[] args)
		{
			int actualImageFrameNumber;
			int? actualOverlayFrameNumber;

			IdentifyPresentationImageFrames(image, out actualImageFrameNumber, out actualOverlayFrameNumber);

			Assert.AreEqual(expectedImageFrameNumber, actualImageFrameNumber, message + " (IMAGE frame number)", args);
			Assert.AreEqual(expectedOverlayFrameNumber, actualOverlayFrameNumber, message + " (OVERLAY frame number)", args);
		}

		private static void IdentifyPresentationImageFrames(IPresentationImage image, out int imageFrameNumber, out int? overlayFrameNumber)
		{
			var overlayColor = Color.Red;
			var imageColor = Color.White;

			// force the overlays to show in our chosen colour
			PresentationState.DicomDefault.Deserialize(image);
			var dps = DicomGraphicsPlane.GetDicomGraphicsPlane((IDicomPresentationImage) image, true);
			foreach (var overlay in dps.ImageOverlays)
				overlay.Color = overlayColor;

			var sopProvider = (IImageSopProvider) image;
			using (var dump = image.DrawToBitmap(sopProvider.Frame.Columns, sopProvider.Frame.Rows))
			{
				// identify the frame number encoded in the image
				imageFrameNumber = 0;
				imageFrameNumber += AreEqual(Sample(dump, 95, 205, 8, 8), imageColor) ? 0x10 : 0;
				imageFrameNumber += AreEqual(Sample(dump, 113, 205, 8, 8), imageColor) ? 0x08 : 0;
				imageFrameNumber += AreEqual(Sample(dump, 130, 205, 8, 8), imageColor) ? 0x04 : 0;
				imageFrameNumber += AreEqual(Sample(dump, 148, 205, 8, 8), imageColor) ? 0x02 : 0;
				imageFrameNumber += AreEqual(Sample(dump, 166, 205, 8, 8), imageColor) ? 0x01 : 0;

				// check if overlay positioning blocks are in the right place
				if (!AreEqual(Sample(dump, 187, 73, 8, 8), overlayColor) || !AreEqual(Sample(dump, 74, 182, 8, 8), overlayColor))
				{
					overlayFrameNumber = null;
					return;
				}

				// identify the frame number encoded in the overlay
				overlayFrameNumber = 0;
				overlayFrameNumber += AreEqual(Sample(dump, 95, 182, 8, 8), overlayColor) ? 0x10 : 0;
				overlayFrameNumber += AreEqual(Sample(dump, 113, 182, 8, 8), overlayColor) ? 0x08 : 0;
				overlayFrameNumber += AreEqual(Sample(dump, 130, 182, 8, 8), overlayColor) ? 0x04 : 0;
				overlayFrameNumber += AreEqual(Sample(dump, 148, 182, 8, 8), overlayColor) ? 0x02 : 0;
				overlayFrameNumber += AreEqual(Sample(dump, 166, 182, 8, 8), overlayColor) ? 0x01 : 0;
			}
		}

		private static DisposableList<IPresentationImage> CreateImages(DicomFile dicomFile)
		{
			using (var dataSource = new LocalSopDataSource(dicomFile))
			{
				using (var sop = new ImageSop(dataSource))
				{
					return new DisposableList<IPresentationImage>(PresentationImageFactory.Create(sop));
				}
			}
		}

		private class DisposableList<T> : List<T>, IDisposable
			where T : IDisposable
		{
			public DisposableList(IEnumerable<T> collection)
				: base(collection) {}

			public void Dispose()
			{
				foreach (var item in this)
					item.Dispose();
			}
		}

		private static bool AreEqual(Color a, Color b)
		{
			var c = Color.FromArgb(Math.Abs(a.R - b.R), Math.Abs(a.G - b.G), Math.Abs(a.B - b.B));
			return c.GetBrightness() < 0.01f;
		}

		private static Color Sample(Bitmap image, int x, int y, int width, int height)
		{
			float count = width*height;
			float r = 0;
			float g = 0;
			float b = 0;
			for (int dx = -width/2; dx < width/2; dx++)
			{
				for (int dy = -height/2; dy < height/2; dy++)
				{
					var c = image.GetPixel(dx + x, dy + y);
					r += c.R/count;
					g += c.G/count;
					b += c.B/count;
				}
			}
			return Color.FromArgb((int) r, (int) g, (int) b);
		}
	}
}

#endif