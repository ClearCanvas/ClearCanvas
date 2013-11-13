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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Tests;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tools.Measurement.Tests
{
	[TestFixture]
	public class CalibrationTest : RoiTestBase
	{
		[Test]
		public void TestCalibrationIsotropicPixels()
		{
			// shouldn't actually matter whether or not the image was already calibrated
			foreach (bool uncalibrated in new[] {true, false})
			{
				// calibrates a horizontal 25 pixel line to be 10 mm
				TestCalibration("ISO", uncalibrated, new PointF(50, 253), new PointF(75, 253), 10, 0.4, 0.4);

				// calibrates a vertical 25 pixel line to be 10 mm
				TestCalibration("ISO", uncalibrated, new PointF(253, 50), new PointF(253, 75), 10, 0.4, 0.4);

				// calibrates a diagonal sqrt(25*25*2) pixel line to be sqrt(2)*10 mm
				TestCalibration("ISO", uncalibrated, new PointF(50, 50), new PointF(75, 75), 14.142, 0.4, 0.4);
			}
		}

		[Test]
		public void TestCalibrationAnisotropicPixels4To3()
		{
			// shouldn't actually matter whether or not the image was already calibrated
			foreach (bool uncalibrated in new[] {true, false})
			{
				// calibrates a horizontal line
				TestCalibration("4:3", uncalibrated, new PointF(33, 253), new PointF(66, 253), 10, 0.4, 0.3);

				// calibrates a vertical line
				TestCalibration("4:3", uncalibrated, new PointF(337, 50), new PointF(337, 75), 10, 0.4, 0.3);

				// calibrates a diagonal line [sqrt(133^2+173^2) pixels] to be 80mm (the edge of the triangle).
				TestCalibration("4:3", uncalibrated, new PointF(33, 225), new PointF(166, 52), 80, 0.4, 0.3);
			}
		}

		[Test]
		public void TestCalibrationAnisotropicPixels3To4()
		{
			// shouldn't actually matter whether or not the image was already calibrated
			foreach (bool uncalibrated in new[] {true, false})
			{
				// calibrates a horizontal line
				TestCalibration("3:4", uncalibrated, new PointF(50, 336), new PointF(75, 336), 10, 0.3, 0.4);

				// calibrates a vertical line
				TestCalibration("3:4", uncalibrated, new PointF(253, 67), new PointF(253, 100), 10, 0.3, 0.4);

				// calibrates a diagonal line [sqrt(100^2+231^2) pixels] to be 80mm (the edge of the triangle).
				TestCalibration("3:4", uncalibrated, new PointF(124, 68), new PointF(224, 299), 80, 0.3, 0.4);
			}
		}

		private static void TestCalibration(string pixelShape, bool uncalibrated, PointF pt1, PointF pt2, double calibrationValue, double expectedRowSpacing, double expectedColSpacing)
		{
			using (IPresentationImage image = GetCalibrationTestImage(pixelShape, uncalibrated))
			{
				Trace.WriteLine(string.Format("TEST {0} image with {1} pixels", uncalibrated ? "uncalibrated" : "calibrated", pixelShape));
				Trace.WriteLine(string.Format("calibrating [{0}, {1}] to {2} mm", pt1, pt2, calibrationValue));

				PolylineGraphic lineGraphic;
				IOverlayGraphicsProvider overlayGraphicsProvider = (IOverlayGraphicsProvider) image;
				overlayGraphicsProvider.OverlayGraphics.Add(new VerticesControlGraphic(lineGraphic = new PolylineGraphic()));
				lineGraphic.CoordinateSystem = CoordinateSystem.Source;
				lineGraphic.Points.Add(pt1);
				lineGraphic.Points.Add(pt2);
				lineGraphic.ResetCoordinateSystem();

				CalibrationTool.TestCalibration(calibrationValue, lineGraphic);

				IImageSopProvider imageSopProvider = (IImageSopProvider) image;

				Trace.WriteLine(string.Format("Pixel Spacing (Actual)/(Expected): ({0:F4}:{1:F4})/({2:F4}:{3:F4})",
				                              imageSopProvider.Frame.NormalizedPixelSpacing.Row, imageSopProvider.Frame.NormalizedPixelSpacing.Column,
				                              expectedRowSpacing, expectedColSpacing));

				float percentErrorRow = Math.Abs((float) ((imageSopProvider.Frame.NormalizedPixelSpacing.Row - expectedRowSpacing)/expectedRowSpacing*100F));
				float percentErrorCol = Math.Abs((float) ((imageSopProvider.Frame.NormalizedPixelSpacing.Column - expectedColSpacing)/expectedColSpacing*100F));

				Trace.WriteLine(String.Format("Percent Error (Row/Column): {0:F3}%/{1:F3}%", percentErrorRow, percentErrorCol));

				Assert.AreEqual(expectedColSpacing, imageSopProvider.Frame.NormalizedPixelSpacing.Column, 0.005, "Column Spacing appears to be wrong");
				Assert.AreEqual(expectedRowSpacing, imageSopProvider.Frame.NormalizedPixelSpacing.Row, 0.005, "Row Spacing appears to be wrong");

				Assert.IsTrue(percentErrorCol < 1.5, "Column Spacing appears to be wrong");
				Assert.IsTrue(percentErrorRow < 1.5, "Row Spacing appears to be wrong");
			}
		}

		[Test]
		public void TestComputationAnisotropicPixel()
		{
			double widthInPixels = 4;
			double heightInPixels = 3;
			double pixelAspectRatio = 2;
			double pixelSpacingWidth, pixelSpacingHeight;

			double testPixelSpacingWidth = 1;
			double testPixelSpacingHeight = testPixelSpacingWidth*pixelAspectRatio;
			double testWidthInMm = testPixelSpacingWidth*widthInPixels;
			double testHeightInMm = testPixelSpacingHeight*heightInPixels;
			double lengthInMm = Math.Sqrt(testWidthInMm*testWidthInMm + testHeightInMm*testHeightInMm);

			CalibrationTool.CalculatePixelSpacing(
				lengthInMm,
				widthInPixels,
				heightInPixels,
				pixelAspectRatio,
				out pixelSpacingWidth,
				out pixelSpacingHeight);

			Assert.AreEqual(1, pixelSpacingWidth, 1e-10);
			Assert.AreEqual(2, pixelSpacingHeight, 1e-10);
		}

		[Test]
		public void TestComputationIsotropicPixel()
		{
			double widthInPixels = 4;
			double heightInPixels = 3;
			double pixelAspectRatio = 1;
			double pixelSpacingWidth, pixelSpacingHeight;

			double testPixelSpacingWidth = 1;
			double testPixelSpacingHeight = testPixelSpacingWidth*pixelAspectRatio;
			double testWidthInMm = testPixelSpacingWidth*widthInPixels;
			double testHeightInMm = testPixelSpacingHeight*heightInPixels;
			double lengthInMm = Math.Sqrt(testWidthInMm*testWidthInMm + testHeightInMm*testHeightInMm);

			CalibrationTool.CalculatePixelSpacing(
				lengthInMm,
				widthInPixels,
				heightInPixels,
				pixelAspectRatio,
				out pixelSpacingWidth,
				out pixelSpacingHeight);

			Assert.AreEqual(1, pixelSpacingWidth, 1e-10);
			Assert.AreEqual(1, pixelSpacingHeight, 1e-10);
		}

		/// <summary>
		/// Gets a test image for use in <see cref="CalibrationTest"/>.
		/// </summary>
		/// <param name="pixelShape">Available pixel shapes are ISO, 4:3 and 3:4</param>
		/// <param name="uncalibrated">Whether or not the image should specify Pixel Spacing already.</param>
		internal static IPresentationImage GetCalibrationTestImage(string pixelShape, bool uncalibrated)
		{
			ImageKey imageKey;
			switch (pixelShape.ToLowerInvariant())
			{
				case "iso":
					imageKey = uncalibrated ? ImageKey.Aspect01 : ImageKey.Aspect03;
					break;
				case "4:3":
					imageKey = uncalibrated ? ImageKey.Aspect06 : ImageKey.Aspect07;
					break;
				case "3:4":
					imageKey = uncalibrated ? ImageKey.Aspect10 : ImageKey.Aspect11;
					break;
				default:
					throw new ArgumentException("Unsupported pixel shape");
			}
			return GetImage(imageKey);
		}
	}
}

#endif