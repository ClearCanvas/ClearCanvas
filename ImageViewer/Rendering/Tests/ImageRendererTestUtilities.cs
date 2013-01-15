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

using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
	static class ImageRendererTestUtilities
	{
		public static Bitmap RenderLayer(ImageGraphic layer, int dstWidth, int dstHeight)
		{
			Bitmap bitmap = new Bitmap(dstWidth, dstHeight);
			Rectangle clientArea = new Rectangle(0, 0, dstWidth, dstHeight);

			BitmapData bitmapData = LockBitmap(bitmap);
			int bytesPerPixel = 4;
			ImageRenderer.Render(layer, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public static void VerifyMonochromePixelValue16(int x, int y, int expectedPixelValue16, Bitmap bitmap)
		{
			int expectedPixelValue8 = expectedPixelValue16 / 256;

			VerifyMonochromePixelValue8(x, y, expectedPixelValue8, bitmap);
		}

		public static void VerifyMonochromePixelValue8(int x, int y, int expectedPixelValue8, Bitmap bitmap)
		{
			Color expectedPixelColor = Color.FromArgb(expectedPixelValue8, expectedPixelValue8, expectedPixelValue8);

			VerifyRGBPixelValue(x, y, expectedPixelColor, bitmap);
		}

		public static void VerifyRGBPixelValue(int x, int y, Color expectedPixelColor, Bitmap bitmap)
		{
			Color actualPixelColor = bitmap.GetPixel(x, y);
			Assert.AreEqual(expectedPixelColor, actualPixelColor);
		}

		private static BitmapData LockBitmap(Bitmap bitmap)
		{
			BitmapData bitmapData = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadWrite,
				bitmap.PixelFormat);

			return bitmapData;
		}
	}
}

#endif