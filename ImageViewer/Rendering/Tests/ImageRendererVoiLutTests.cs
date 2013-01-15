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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
	[TestFixture]
	public class ImageRendererVoiLutTests
	{
		[Test]
		public void TestRGBKCorners()
		{
			// needs a high tolerance due to sharp edges (comparing LUTted and interpolated vs UN-LUTted and UN-interpolated)
			ExecuteIdentityLUTTest<ColorImageGraphic>(TestPattern.CreateRGBKCorners, 25.6, "RGBKCorners");
		}

		[Test]
		public void TestGraydient()
		{
			ExecuteIdentityLUTTest<GrayscaleImageGraphic>(TestPattern.CreateGraydient, 5, "Graydient");
		}

		[Test]
		public void TestCheckerboard()
		{
			// needs a very high tolerance due to many sharp edges (comparing LUTted and interpolated vs UN-LUTted and UN-interpolated)
			ExecuteIdentityLUTTest<GrayscaleImageGraphic>(TestPattern.CreateCheckerboard, 50, "Checkerboard");
		}

		private delegate T CreateLuttableImageGraphicDelegate<T>(Size size) where T : ImageGraphic, IVoiLutProvider;

		private static void ExecuteIdentityLUTTest<T>(CreateLuttableImageGraphicDelegate<T> @delegate, double tolerance, string testName) where T : ImageGraphic, IVoiLutProvider
		{
			Size size = new Size(128, 128);
			T control = @delegate(size);
			try
			{
				control.VoiLutManager.Enabled = false;
			}
			catch (InvalidOperationException) { }

			T withLut = @delegate(size);
			try
			{
				control.VoiLutManager.Enabled = true;
			}
			catch (InvalidOperationException) { }
			withLut.VoiLutManager.InstallVoiLut(new IdentityVoiLinearLut());

			Statistics stats = RenderAndDiff(control, withLut, string.Format("{0}.Lut0.bmp", testName), string.Format("{0}.Lut1.bmp", testName));
			Trace.WriteLine(string.Format("DIFF STATS {0}", stats));
			Assert.IsTrue(stats.IsEqualTo(0, tolerance), string.Format("Identity LUT test failed: TestPattern={0}", testName));

			control.Dispose();
			withLut.Dispose();
		}

		private static Statistics RenderAndDiff(ImageGraphic control, ImageGraphic test, string controlName, string testName)
		{
			List<int> diffs = new List<int>();

			using (Bitmap controlBitmap = new Bitmap(control.Columns, control.Rows))
			{
				using (Bitmap testBitmap = Render(test))
				{
					for (int x = 0; x < control.Columns; x++)
					{
						for (int y = 0; y < control.Rows; y++)
						{
							if (control is ColorImageGraphic)
							{
								Color controlColor = Color.FromArgb(control.PixelData.GetPixel(x, y));
								Color testColor = testBitmap.GetPixel(x, y);

								diffs.Add(Math.Abs(controlColor.R - testColor.R));
								diffs.Add(Math.Abs(controlColor.G - testColor.G));
								diffs.Add(Math.Abs(controlColor.B - testColor.B));

								controlBitmap.SetPixel(x, y, controlColor);
							}
							else if (control is GrayscaleImageGraphic)
							{
								int range = 1 << ((GrayscaleImageGraphic) control).BitsStored;
								int offset = ((GrayscaleImageGraphic) control).IsSigned ? -(range >> 1) : 0;
								int v = (int) (256f*(control.PixelData.GetPixel(x, y) - offset)/range);

								Color controlColor = Color.FromArgb(255, v, v, v);
								Color testColor = testBitmap.GetPixel(x, y);

								diffs.Add(Math.Abs(controlColor.R - testColor.R));
								diffs.Add(Math.Abs(controlColor.G - testColor.G));
								diffs.Add(Math.Abs(controlColor.B - testColor.B));

								controlBitmap.SetPixel(x, y, controlColor);
							}
							else
							{
								Assert.Ignore("Unable to render and diff images of type {0}", control.GetType().FullName);
							}
						}
					}

					if (!string.IsNullOrEmpty(testName))
						testBitmap.Save(testName);
				}

				if (!string.IsNullOrEmpty(controlName))
					controlBitmap.Save(controlName);
			}

			return new Statistics(diffs);
		}

		private static Bitmap Render(ImageGraphic x)
		{
			Bitmap bmp = new Bitmap(x.Columns, x.Rows);
			Rectangle rect = new Rectangle(Point.Empty, bmp.Size);

			BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			try
			{
				ImageRenderer.Render(x, bitmapData.Scan0, bitmapData.Width, 4, rect);
			}
			finally
			{
				bmp.UnlockBits(bitmapData);
			}

			return bmp;
		}
	}
}

#endif