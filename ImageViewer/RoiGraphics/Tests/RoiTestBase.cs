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
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	public abstract class RoiTestBase<T> : RoiTestBase
	{
		/// <summary>
		/// Tests the <see cref="Roi.Contains(System.Drawing.PointF)"/> method for a given shape. The image is used to provide a basis for the coordinate space.
		/// </summary>
		protected void TestRoiContains(ImageKey key, T shapeData, string name)
		{
			using (IPresentationImage image = GetImage(key))
			{
				IImageSopProvider provider = (IImageSopProvider) image;
				using (Bitmap bmp = new Bitmap(provider.Frame.Columns, provider.Frame.Rows))
				{
					using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
					{
						g.Clear(Color.Black);

						// baseline ROI using GDI's GraphicsPath
						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							AddShapeToGraphicsPath(graphicsPath, shapeData);
							g.FillPath(Brushes.Blue, graphicsPath);
						}
					}

					// simulates ROIs that an end-user would create using graphics constructed by the tools
					Roi userRoi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
					userRoi.PixelData.ForEachPixel(delegate(int i, int x, int y, int pixelIndex)
					                               	{
					                               		if (userRoi.Contains(x, y))
					                               			bmp.SetPixel(x, y, ShiftColor(bmp.GetPixel(x, y), true, false, false));
					                               	});

					// simulates ROIs that a programmer might create using the SDK directly, rather than via graphics
					Roi sdkRoi = CreateRoiFromImage(image, shapeData);
					sdkRoi.PixelData.ForEachPixel(delegate(int i, int x, int y, int pixelIndex)
					                              	{
					                              		if (sdkRoi.Contains(x, y))
					                              			bmp.SetPixel(x, y, ShiftColor(bmp.GetPixel(x, y), false, true, false));
					                              	});

					string filename = string.Format("{0}.{1}.containment.test.png", name, this.ShapeName).TrimStart('.');
					bmp.Save(filename);
					Trace.WriteLine(string.Format("Pixel containment map has been dumped to {0}.", filename));

					int totalCount = 0;
					int errorCount = 0;
					for (int y = 0; y < bmp.Height; y++)
					{
						for (int x = 0; x < bmp.Width; x++)
						{
							Color c = bmp.GetPixel(x, y);
							if (c.R > 0 || c.G > 0 || c.B > 0)
							{
								totalCount++;
								if (c.R < 255 || c.G < 255 || c.B < 255)
									errorCount++;
							}
						}
					}

					if (errorCount > 0)
					{
						WriteLine("The pixel containment test results are not perfect. {0} differences were found out of {1} pixels.", errorCount, totalCount);
						WriteLine("The image should be mostly a white shape on a black background.");
						WriteLine("Any coloured areas indicate locations where Roi.Contains(...) did not return perfectly coincident results.");
						WriteLine("There will invariably be such areas along shape boundaries, but the shape should be white overall.");
						WriteLine("Red channel is painted by user mode ROI. Green channel is painted by SDK. Blue channel is painted by GDI+.");
					}
					Assert.AreEqual(0, errorCount, 0.01*totalCount, "Automated pixel containment test failed. Please review the test output manually.");
				}
			}
		}

		/// <summary>
		/// Tests the <see cref="Roi.GetPixelCoordinates"/>, <see cref="Roi.GetRawPixelValues"/> and <see cref="Roi.GetPixelValues"/> method output
		/// for any given ROI shape on a 100x100 test image.
		/// </summary>
		/// <remarks>
		/// The pixel data and modality LUT is computed according to a built-in algorithm and automatically verified.
		/// </remarks>
		protected void TestRoiContainedPixels(T shapeData, IEnumerable<PointF> expectedPixels, bool traceLists, string testId)
		{
			// use the provided list of expected pixel coordinates to determine the expected raw values according to the algorithm
			var expectedRawValues = CollectionUtils.Map<PointF, int>(expectedPixels, p => (int) (p.X + p.Y)).AsReadOnly();

			// use the computed list of expected raw values to determine the expected post modality LUT values according to the algorithm
			var expectedValues = CollectionUtils.Map<int, int>(expectedRawValues, x => 3*x + 100).AsReadOnly();

			using (var image = new GrayscalePresentationImage(100, 100, 8, 8, 7, false, false, 3, 100, 1, 1, 1, 1, () =>
			                                                                                                       	{
			                                                                                                       		var buffer = new byte[10000];
			                                                                                                       		for (int i = 0; i < 10000; i++)
			                                                                                                       			buffer[i] = (byte) ((i%100) + (i/100));
			                                                                                                       		return buffer;
			                                                                                                       	}))
			{
				var roi = CreateRoiFromImage(image, shapeData);
				var error = false;

				// enumerate the pixel coordinates and compare with the expected list of coordinates
				var actualPixels = new List<PointF>(roi.GetPixelCoordinates()).AsReadOnly();
				try
				{
					var list = new List<PointF>(actualPixels);
					foreach (var expectedPixel in expectedPixels)
						Assert.IsTrue(list.Remove(expectedPixel), "{0}: The expected pixel {1} was not returned by the enumerator.", testId, expectedPixel);
					Assert.IsTrue(list.Count == 0, "{0}: The enumerator returned unexpected pixels.", testId);
				}
				catch (Exception)
				{
					error = true;
					throw;
				}
				finally
				{
					if (traceLists || error)
					{
						Trace.WriteLine(string.Format(" Expected Pixels: {0}", Format(expectedPixels)));
						Trace.WriteLine(string.Format("   Actual Pixels: {0}", Format(actualPixels)));
					}
				}

				// enumerate the raw values and compare with the expected list of raw values
				var actualRawValues = new List<int>(roi.GetRawPixelValues()).AsReadOnly();
				try
				{
					var list = new List<int>(actualRawValues);
					foreach (var expectedRawValue in expectedRawValues)
						Assert.IsTrue(list.Remove(expectedRawValue), "{0}: The expected raw value {1} was not returned by the enumerator.", testId, expectedRawValue);
					Assert.IsTrue(list.Count == 0, "{0}: The enumerator returned unexpected raw values.", testId);
				}
				catch (Exception)
				{
					error = true;
					throw;
				}
				finally
				{
					if (traceLists || error)
					{
						Trace.WriteLine(string.Format(" Expected Raw Values: {0}", Format(expectedRawValues)));
						Trace.WriteLine(string.Format("   Actual Raw Values: {0}", Format(actualRawValues)));
					}
				}

				// enumerate the post modality LUT values and compare with the expected list of values
				var actualValues = roi.GetPixelValues().Select(v => (int) Math.Round(v)).ToList().AsReadOnly();
				try
				{
					var list = new List<int>(actualValues);
					foreach (var expectedValue in expectedValues)
						Assert.IsTrue(list.Remove(expectedValue), "{0}: The expected value {1} was not returned by the enumerator.", testId, expectedValue);
					Assert.IsTrue(list.Count == 0, "{0}: The enumerator returned unexpected values.", testId);
				}
				catch (Exception)
				{
					error = true;
					throw;
				}
				finally
				{
					if (traceLists || error)
					{
						Trace.WriteLine(string.Format(" Expected Values: {0}", Format(expectedValues)));
						Trace.WriteLine(string.Format("   Actual Values: {0}", Format(actualValues)));
					}
				}
			}
		}

		/// <summary>
		/// Tests the length measurement for a specific shape on a specific image.
		/// </summary>
		protected void TestRoiLengthMeasurement(ImageKey key, T shapeData, double expectedLength)
		{
			TestRoiLengthMeasurement(key, shapeData, expectedLength, 0.25, Units.Pixels);
		}

		/// <summary>
		/// Tests the length measurement for a specific shape on a specific image.
		/// </summary>
		protected void TestRoiLengthMeasurement(ImageKey key, T shapeData, double expectedLength, double toleranceLength, Units lengthUnits)
		{
			string description = string.Format("{0} on {1}", shapeData, key);
			using (IPresentationImage image = GetImage(key))
			{
				// simulates ROIs that an end-user would create using graphics constructed by the tools
				Roi userRoi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
				PerformRoiLengthMeasurements(userRoi, expectedLength, toleranceLength, "user", description, lengthUnits);

				// simulates ROIs that a programmer might create using the SDK directly, rather than via graphics
				Roi sdkRoi = CreateRoiFromImage(image, shapeData);
				PerformRoiLengthMeasurements(sdkRoi, expectedLength, toleranceLength, "SDK", description, lengthUnits);
			}
		}

		private void PerformRoiLengthMeasurements(Roi roi, double expectedLength, double toleranceLength, string mode, string shapeData, Units lengthUnits)
		{
			WriteLine("Testing {1}-MODE {2} ROI using {0}", shapeData, mode.ToUpperInvariant(), this.ShapeName);

			double measuredLength = 0;

			if (roi is IRoiLengthProvider)
			{
				((IRoiLengthProvider) roi).Units = lengthUnits;
				measuredLength = ((IRoiLengthProvider) roi).Length;
			}

			if (this.VerboseOutput)
			{
				WriteLine("Expecting  \u2113={0:f0}", expectedLength);
				WriteLine("Actual     \u2113={0:f0}", measuredLength);
			}

			double errorLength = Math.Abs(expectedLength - measuredLength);

			WriteLine("Errors    \u0394\u2113={0:f2}", errorLength);

			Assert.AreEqual(expectedLength, measuredLength, toleranceLength, string.Format("\u0394length exceeds absolute tolerance of {0:f0}", toleranceLength));
		}

		/// <summary>
		/// Tests the area, mean and standard deviation calculations for a specific shape on a specific image.
		/// </summary>
		protected void TestRoiStatsCalculations(ImageKey key, T shapeData, double expectedPerimeter, double expectedArea, double expectedMean, double expectedSigma)
		{
			this.TestRoiStatsCalculations(key, shapeData, expectedPerimeter, expectedArea, expectedMean, expectedSigma, Units.Pixels);
		}

		/// <summary>
		/// Tests the area, mean and standard deviation calculations for a specific shape on a specific image.
		/// </summary>
		protected void TestRoiStatsCalculations(ImageKey key, T shapeData, double expectedPerimeter, double expectedArea, double expectedMean, double expectedSigma, Units areaPerimeterUnits)
		{
			string description = string.Format("{0} on {1}", shapeData, key);
			using (IPresentationImage image = GetImage(key))
			{
				// simulates ROIs that an end-user would create using graphics constructed by the tools
				Roi userRoi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
				PerformRoiStatsCalculations(userRoi, expectedPerimeter, expectedArea, expectedMean, expectedSigma, "user", description, areaPerimeterUnits);

				// simulates ROIs that a programmer might create using the SDK directly, rather than via graphics
				Roi sdkRoi = CreateRoiFromImage(image, shapeData);
				PerformRoiStatsCalculations(sdkRoi, expectedPerimeter, expectedArea, expectedMean, expectedSigma, "SDK", description, areaPerimeterUnits);
			}
		}

		private void PerformRoiStatsCalculations(Roi roi, double expectedPerimeter, double expectedArea, double expectedMean, double expectedSigma, string mode, string shapeData, Units areaPerimeterUnits)
		{
			WriteLine("Testing {1}-MODE {2} ROI using {0}", shapeData, mode.ToUpperInvariant(), this.ShapeName);

			RoiStatistics roiStats = RoiStatistics.Calculate(roi);
			double measuredArea = 0;
			double measuredMean = roiStats.Mean;
			double measuredSigma = roiStats.StandardDeviation;

			if (roi is IRoiAreaProvider)
			{
				((IRoiAreaProvider) roi).Units = areaPerimeterUnits;
				measuredArea = ((IRoiAreaProvider) roi).Area;
			}

			if (this.VerboseOutput)
			{
				WriteLine("Expecting  A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", expectedArea, expectedMean, expectedSigma);
				WriteLine("Actual     A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", measuredArea, measuredMean, measuredSigma);
			}

			double errorArea = Math.Abs(expectedArea - measuredArea);
			double errorMean = Math.Abs(expectedMean - measuredMean);
			double errorSigma = Math.Abs(expectedSigma - measuredSigma);

			WriteLine("Errors    \u0394A={0:f2} \u0394\u03BC={1:f2} \u0394\u03C3={2:f2}", errorArea, errorMean, errorSigma);

			double areaToleranceFactor = 0.01;
			if (expectedPerimeter/expectedArea > 0.25)
			{
				WriteLine("High Perimeter-to-Area Ratio (P={0:f3})", expectedPerimeter);
				areaToleranceFactor = 0.05;
			}

			double toleranceArea = areaToleranceFactor*expectedPerimeter;
			double toleranceMean = 2.0;
			double toleranceSigma = 2.0;

			Assert.AreEqual(expectedArea, measuredArea, toleranceArea, string.Format("\u0394area exceeds tolerance of {0:p0} of ROI perimeter", areaToleranceFactor));
			Assert.AreEqual(expectedMean, measuredMean, toleranceMean, string.Format("\u0394mean exceeds tolerance of 1% of pixel value range"));
			Assert.AreEqual(expectedSigma, measuredSigma, toleranceSigma, string.Format("\u0394stdev exceeds tolerance of 1% of pixel value range"));
		}

		/// <summary>
		/// Tests the statistics calculation for consistency over a number of trial runs using the same shape.
		/// </summary>
		protected void TestRoiStatsCalculationConsistency()
		{
			const int samples = 100;
			const int gridSize = 4;
			foreach (ImageKey imageKey in Enum.GetValues(typeof (ImageKey)))
			{
				WriteLine("Testing on Image {0}", imageKey.ToString());
				using (IPresentationImage image = GetImage(imageKey))
				{
					IImageSopProvider provider = (IImageSopProvider) image;
					IOverlayGraphicsProvider overlayGraphicsProvider = (IOverlayGraphicsProvider) image;
					int rows = provider.Frame.Rows;
					int cols = provider.Frame.Columns;

					for (int r = rows/gridSize/2; r < rows; r += rows/gridSize)
					{
						for (int c = cols/gridSize/2; c < cols; c += cols/gridSize)
						{
							if (this.VerboseOutput)
								WriteLine("Checking {0} core samples for consistent results at location {1}", samples, new PointF(c, r));

							T shapeData = CreateCoreSampleShape(new PointF(c, r), rows, cols);
							double expectedArea = 0, expectedMean = 0, expectedSigma = 0;
							for (int n = 0; n < samples; n++)
							{
								Roi userRoi = CreateRoiFromGraphic(overlayGraphicsProvider, shapeData);
								RoiStatistics stats = RoiStatistics.Calculate(userRoi);
								if (n == 0)
								{
									if (userRoi is IRoiAreaProvider)
										expectedArea = ((IRoiAreaProvider) userRoi).Area;
									expectedMean = stats.Mean;
									expectedSigma = stats.StandardDeviation;

									if (this.VerboseOutput)
										WriteLine("First sample reported A={0:f0}  \u03BC={1:f3}  \u03C3={2:f3}", expectedArea, expectedMean, expectedSigma);

									continue;
								}

								// very strict tolerance. performing the calculation the first time should yield the same result as the next hundred times.
								if (userRoi is IRoiAreaProvider)
									Assert.AreEqual(expectedArea, ((IRoiAreaProvider) userRoi).Area, double.Epsilon, "Area calculation consistency fail.");
								Assert.AreEqual(expectedMean, stats.Mean, double.Epsilon, "Mean calculation consistency fail.");
								Assert.AreEqual(expectedSigma, stats.StandardDeviation, double.Epsilon, "Stdev calculation consistency fail.");
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the name of the shape covered by this test.
		/// </summary>
		protected abstract string ShapeName { get; }

		/// <summary>
		/// Gets a value indicating whether or not output should be verbose.
		/// </summary>
		protected virtual bool VerboseOutput
		{
			get { return true; }
		}

		protected abstract T CreateCoreSampleShape(PointF location, int imageRows, int imageCols);
		protected abstract Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, T shapeData);
		protected abstract Roi CreateRoiFromImage(IPresentationImage image, T shapeData);
		protected abstract void AddShapeToGraphicsPath(GraphicsPath graphicsPath, T shapeData);

		private static string Format<TItem>(IEnumerable<TItem> enumerable)
		{
			var builder = new StringBuilder();
			foreach (var item in enumerable)
			{
				builder.Append(item.ToString());
				builder.Append("; ");
			}
			if (builder.Length == 0)
				return string.Empty;
			return builder.ToString(0, builder.Length - 2);
		}

		private static Color ShiftColor(Color original, bool shiftRed, bool shiftGreen, bool shiftBlue)
		{
			return Color.FromArgb(shiftRed ? 255 : (int) original.R, shiftGreen ? 255 : (int) original.G, shiftBlue ? 255 : (int) original.B);
		}

		[TestFixtureSetUp]
		public void TestSetup()
		{
			AssertTestImagePath();
		}
	}

	public abstract class RoiTestBase
	{
		private const string _testImagePathFormat = @"TestImages\{0}.dcm";

		protected static void WriteLine(string message, params object[] args)
		{
			if (args != null && args.Length > 0)
				message = string.Format(message, args);
			Trace.WriteLine(message);
		}

		protected static IPresentationImage GetImage(ImageKey key)
		{
			string filename = string.Format(_testImagePathFormat, key.ToString().ToLower());
			try
			{
				LocalSopDataSource dataSource = new LocalSopDataSource(filename);
				ImageSop imageSop = new ImageSop(dataSource);
				IPresentationImage theOne = null;
				foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
				{
					if (theOne == null)
					{
						theOne = image;
						continue;
					}
					image.Dispose();
				}
				imageSop.Dispose();
				return theOne;
			}
			catch (Exception ex)
			{
				throw new FileNotFoundException("Unable to load requested test image. Please check that the assembly has been built.", filename, ex);
			}
		}

		protected static void AssertTestImagePath()
		{
			foreach (ImageKey imageKey in Enum.GetValues(typeof (ImageKey)))
			{
				string filename = string.Format(_testImagePathFormat, imageKey.ToString());
				filename = Path.GetFullPath(filename);
				string directoryName = Path.GetDirectoryName(filename);

				if (!File.Exists(filename))
				{
					string message = String.Format(
						@"The required test image {0} is missing. " +
						@"Please copy the contents of <TrunkPath>\ImageViewer\RoiGraphics\Tests\Images " +
						@"to {1} in order to execute these tests.", filename, directoryName);

					Trace.WriteLine(message);
					Assert.Fail(message);
				}
			}
		}

		protected enum ImageKey
		{
			Simple01,
			Simple02,
			Simple03,
			Simple04,
			Simple05,
			Simple06,
			Complex01,
			Complex02,
			Complex03,
			Complex04,
			Complex05,
			Complex06,
			Complex07,
			Complex08,
			Complex09,
			Complex10,
			Complex11,
			Complex12,
			Real01,
			Real02,
			Real03,
			Real04,
			Real05,
			Real06,
			Aspect01,
			Aspect02,
			Aspect03,
			Aspect04,
			Aspect05,
			Aspect06,
			Aspect07,
			Aspect08,
			Aspect09,
			Aspect10,
			Aspect11,
			Aspect12,
		}
	}
}

#endif