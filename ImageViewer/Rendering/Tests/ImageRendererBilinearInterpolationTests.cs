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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering.Tests
{
	[TestFixture]
	public class ImageRendererBilinearInterpolationTests
	{
		enum ImageTypes { Mono8, Mono16, Mono8Signed, Mono16Signed, Rgb };

		ImageGraphic _image;
		CompositeImageGraphic _containerGraphic;

		int _srcWidth, _srcHeight;
		int _dstWidth, _dstHeight;
		bool _flipHorizontal;
		bool _flipVertical;
		int _rotation;
		float _scale;
		bool _scaleToFit;
		int _translationX;
		int _translationY;

		bool _trace = false;
		bool _tracePhantom = false;
		bool _traceBitmap = false;

		static readonly float _oneQuarter = 0.25F;
		static readonly float _threeQuarters = 0.75F;

		ImageTypes _sourceImageType;

		static readonly float _fixedScale = 128;
		static readonly int _fixedPrecision = 7;

		private bool IsColor()
		{
			return _image is ColorImageGraphic;
		}

		private ColorPixelData ColorPixelData()
		{
			return _image.PixelData as ColorPixelData;
		}

		private GrayscalePixelData GrayscalePixelData()
		{
			return _image.PixelData as GrayscalePixelData;
		}

		private static void Trace(string trace)
		{
			System.Diagnostics.Trace.Write(trace);
			Console.Write(trace);
		}

		private static void TraceLine(string trace)
		{
			System.Diagnostics.Trace.WriteLine(trace);
			Console.WriteLine(trace);
		}

		public ImageRendererBilinearInterpolationTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
			MemoryManager.Enabled = false;
			//_tracePhantom = true;
			//_traceBitmap = true;
			//_trace = true;
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestImageTwoByTwo()
		{
			//Test a 2x2 source image.  The results aren't so important here as it is for the Render call to *not* fail.
			_srcWidth = 2;
			_srcHeight = 2;
			_dstWidth = 11;
			_dstHeight = 17;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageArbitrarySizeLargerDestination1()
		{
			_srcWidth = 7;
			_srcHeight = 11;
			_dstWidth = 23;
			_dstHeight = 31;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageArbitrarySizeLargerDestination2()
		{
			_srcWidth = 23;
			_srcHeight = 31;
			_dstWidth = 41;
			_dstHeight = 53;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageArbitrarySizeLargerSource1()
		{
			_srcWidth = 23;
			_srcHeight = 31;
			_dstWidth = 7;
			_dstHeight = 11;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageArbitrarySizeLargerSource2()
		{
			_srcWidth = 41;
			_srcHeight = 53;
			_dstWidth = 23;
			_dstHeight = 31;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageSameSize1()
		{
			_srcWidth = 7;
			_srcHeight = 11;
			_dstWidth = 7;
			_dstHeight = 11;

			TestVariousPointAllMethods();
		}

		[Test]
		public void TestImageSameSize2()
		{
			_srcWidth = 23;
			_srcHeight = 31;
			_dstWidth = 23;
			_dstHeight = 31;

			TestVariousPointAllMethods();
		}

		private void TestVariousPointAllMethods()
		{
			TestAtCentreOfQuadrants();
			TestAtBoundaries();
			TestNearBoundaries();
		}

		private void TestAtCentreOfQuadrants()
		{
			Point[] arrayOfTestPoints = new Point[4];
        
			//quadrant 1
			arrayOfTestPoints[0].X = (int)(_dstWidth * _oneQuarter);
			arrayOfTestPoints[0].Y = (int)(_dstHeight * _oneQuarter);

			//quadrant 2
			arrayOfTestPoints[1].X = (int)(_dstWidth * _threeQuarters);
			arrayOfTestPoints[1].Y = (int)(_dstHeight * _oneQuarter);

			//quadrant 3
			arrayOfTestPoints[2].X = (int)(_dstWidth * _oneQuarter);
			arrayOfTestPoints[2].Y = (int)(_dstHeight * _threeQuarters);

			//quadrant 4
			arrayOfTestPoints[3].X = (int)(_dstWidth * _threeQuarters);
			arrayOfTestPoints[3].Y = (int)(_dstHeight * _threeQuarters);

			TestAllImageTypes(arrayOfTestPoints);
		}

		private void TestAtBoundaries()
		{
			Point[] arrayOfTestPoints = new Point[4];
        
			//top left
			arrayOfTestPoints[0].X = 0;
			arrayOfTestPoints[0].Y = 0;

			//bottom left
			arrayOfTestPoints[1].X = 0;
			arrayOfTestPoints[1].Y = _dstHeight - 1;

			//top right
			arrayOfTestPoints[2].X = _dstWidth - 1;
			arrayOfTestPoints[2].Y = 0;

			//bottom right
			arrayOfTestPoints[3].X = _dstWidth - 1;
			arrayOfTestPoints[3].Y = _dstHeight - 1;

			TestAllImageTypes(arrayOfTestPoints);
		}

		private void TestNearBoundaries()
		{
			Point[] arrayOfTestPoints = new Point[4];
        
			//top left
			arrayOfTestPoints[0].X = 1;
			arrayOfTestPoints[0].Y = 1;

			//bottom left
			arrayOfTestPoints[1].X = _dstWidth - 2;
			arrayOfTestPoints[1].Y = 1;

			//top right
			arrayOfTestPoints[2].X = 1;
			arrayOfTestPoints[2].Y = _dstHeight - 2;

			//bottom right
			arrayOfTestPoints[3].X = _dstWidth - 2;
			arrayOfTestPoints[3].Y = _dstHeight - 2;

			TestAllImageTypes(arrayOfTestPoints);
		}

		private void InitializeTransform()
		{
			_flipHorizontal = false;
			_flipVertical = false;
			_rotation = 0;
			_scale = 1.0F;
			_scaleToFit = true;
			_translationX = 0;
			_translationY = 0;
		}

		private void TestAllImageTypes(Point[] ArrayOfPoints)
		{
			_sourceImageType = ImageTypes.Mono16;
			InitializeTransform();
			TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

			_sourceImageType = ImageTypes.Mono8;
			InitializeTransform();
			TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

			_sourceImageType = ImageTypes.Mono8Signed;
			InitializeTransform();
			TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);

			_sourceImageType = ImageTypes.Mono16Signed;
			InitializeTransform();
			TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);
			
			_sourceImageType = ImageTypes.Rgb;
			InitializeTransform();
			TestImageVariousTranslationsScalesAndOrientations(ArrayOfPoints);
		}

		private void TestImageVariousTranslationsScalesAndOrientations(Point[] arrayOfTestPoints)
		{
			_translationX = 0;
			_translationY = 0;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = _dstWidth / 4;
			_translationY = 0;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = 0;
			_translationY = _dstHeight / 4;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);

			_translationX = _dstWidth / 4;
			_translationY = _dstHeight / 4;
			TestImageVariousScalesAndOrientations(arrayOfTestPoints);
		}

		private void TestImageVariousScalesAndOrientations(Point[] arrayOfTestPoints)
		{
			_scaleToFit = true;
			_scale = 1.0F; 
			TestImageVariousOrientations(arrayOfTestPoints);

			_scaleToFit = false;
			_scale = 1.0F;
			TestImageVariousOrientations(arrayOfTestPoints);

			_scale = 1.5F;
			TestImageVariousOrientations(arrayOfTestPoints);

			_scale = 2.0F;
			TestImageVariousOrientations(arrayOfTestPoints);
		}

		private void TestImageVariousOrientations(Point [] arrayOfTestPoints)
		{
			//there are 8 different possible (orthogonal) orientations.
			_flipHorizontal = false;
			_flipVertical = false;
			TestImage(arrayOfTestPoints);

			_flipHorizontal = true;
			_flipVertical = false;
			TestImage(arrayOfTestPoints);

			_flipHorizontal = false;
			_flipVertical = true;
			TestImage(arrayOfTestPoints);

			_flipHorizontal = true;
			_flipVertical = true;
			TestImage(arrayOfTestPoints);

			_rotation = 90;
			_flipHorizontal = false;
			_flipVertical = false;
			TestImage(arrayOfTestPoints);

			_rotation = 90;
			_flipHorizontal = false;
			_flipVertical = true;
			TestImage(arrayOfTestPoints);

			_rotation = 90;
			_flipHorizontal = true;
			_flipVertical = false;
			TestImage(arrayOfTestPoints);

			_rotation = 90;
			_flipHorizontal = true;
			_flipVertical = true;
			TestImage(arrayOfTestPoints);
		}

		private void TestImage(IEnumerable<Point> arrayOfTestPoints)
		{
			if (_trace)
			{
				string imageType;
				if (_sourceImageType == ImageTypes.Mono16)
					imageType = "Mono16";
				else if (_sourceImageType == ImageTypes.Mono8)
					imageType = "Mono8";
				else if (_sourceImageType == ImageTypes.Mono8Signed)
					imageType = "Mono8Signed";
				else if (_sourceImageType == ImageTypes.Mono16Signed)
					imageType = "Mono16Signed";
				else
					imageType = "Rgb";

				TraceLine("");
				TraceLine(imageType);
				TraceLine(String.Format("Scale (Fit/Scale): {0}/{1}", _scaleToFit, _scale));
				TraceLine(String.Format("Orientation(FH/FV/R): {0}/{1}/{2}", _flipHorizontal, _flipVertical, _rotation));
				TraceLine(String.Format("Translation: {0}, {1}", _translationX, _translationY));
			}

			CreateImageLayer(_sourceImageType);
			CreatePhantom();

			//render the image to a bitmap
			Bitmap dstBitmap = ImageRendererTestUtilities.RenderLayer(_image, _dstWidth, _dstHeight);
			if (_traceBitmap)
			{
				string strTraceBitmap = "Bitmap:\n";

				for (int y = 0; y < dstBitmap.Height; y++)
				{
					for (int x = 0; x < dstBitmap.Width; x++)
					{
						byte pixelValue = dstBitmap.GetPixel(x, y).R;
						strTraceBitmap += String.Format("{0}  ", (int)pixelValue);
					}

					strTraceBitmap += "\n";
				}

				TraceLine(strTraceBitmap);
			}

			foreach (Point dstPoint in arrayOfTestPoints)
			{
				// //The point of the unit test here is to do the same bilinear calculation as is done in the 
				// //actual interpolation code, but in a more reliable & simpler way (e.g. not using pointer
				// //arithmetic, offsets, rectangles, etc).
				Rectangle dstViewableRectangle;
				RectangleF srcViewableRectangle;
				ImageRenderer.CalculateVisibleRectangles(_image, new Rectangle(0, 0, _dstWidth, _dstHeight), out dstViewableRectangle, out srcViewableRectangle);

				byte dstValue = dstBitmap.GetPixel(dstPoint.X, dstPoint.Y).R; //just check the value of R.

				if (dstViewableRectangle.Contains(dstPoint))
				{
					PointF dstTestPoint = new PointF(dstPoint.X + 0.5F, dstPoint.Y + 0.5F);
					PointF srcTestPoint = _image.SpatialTransform.ConvertToSource(dstTestPoint);

					float tolerance = 1 / (8F * _fixedScale);

					// We can take advantage of the fact that we are using fixed point arithmetic to do the interpolation
					// because it essentially means that there is a discrete set of resulting values for a given 4-pixel
					// region (dx,dy between 0.0 -> 1.0 translates to 0/128 -> 128/128 in fixed point).
					// Because we use the SpatialTransform class to calculate the source coordinate in the C# world,
					// and 2 rectangles in the C++ world, we can't expect to get *exactly* the same answer in both cases.
					// Therefore, we calculate the values at source points offset by a tolerance much smaller than 1/128, 
					// so that if by chance the source coordinates are teetering on a fixed-point boundary (e.g. slight 
					// changes would result in dx or dy being off by +-1/128) then we can still be confident that the 
					// source coordinates the interpolator is calculating are reasonable, and thus the interpolated value
					// is correct, even if it's not exactly what we calculate in C# with zero-tolerance.
					// The tolerance used here is 1/(8*128) ~= 0.001.  When the test passes, this means that the C# and
					// C++ calculations of the source pixel coordinate (and thus, dx/dy and the interpolated value) agree to 
					// within one one-thousandth of a pixel.
					List<SizeF> offsets = new List<SizeF>();
					offsets.Add(new SizeF(0F, 0F));
					offsets.Add(new SizeF(-tolerance, -tolerance));
					offsets.Add(new SizeF(-tolerance, +tolerance));
					offsets.Add(new SizeF(+tolerance, -tolerance));
					offsets.Add(new SizeF(+tolerance, +tolerance));

					bool success = false;
					int i = 0;
					foreach (SizeF offset in offsets)
					{
						PointF srcPoint00 = PointF.Add(srcTestPoint, offset);
						byte backCalculateValue = PerformBilinearInterpolationAt(srcPoint00);

						if (_trace)
						{
							string strMessage = String.Format("Test Point #{0} ({1}, {2}): {3}, BackCalculated({4:F16}, {5:F16}): {6}\n",
															  ++i, dstTestPoint.X, dstTestPoint.Y, dstValue, srcPoint00.X, srcPoint00.Y,
							                                  backCalculateValue);

							TraceLine(strMessage);
						}

						if (backCalculateValue == dstValue)
						{
							success = true;
							break;
						}
					}

					Assert.IsTrue(success, "Failed for all test points within tolerance range.");
				}
				else
				{
					//The bilinear interpolation algorithm should not calculate any values outside the dstViewableRectangle.
					string strMessage = String.Format("Point outside rectangle ({0}, {1}) = {2}", dstPoint.X, dstPoint.Y, dstValue);
					
					if (_trace)
						TraceLine(strMessage);
					
					Assert.AreEqual(0, dstValue, strMessage);
				}
			}
		}

		private byte PerformBilinearInterpolationAt(PointF srcPoint00)
		{
			if (srcPoint00.Y < 0)
				srcPoint00.Y = 0;
			if (srcPoint00.X < 0)
				srcPoint00.X = 0;

			if (srcPoint00.X > (_srcWidth - 1.001F))
				srcPoint00.X = (_srcWidth - 1.001F);
			if (srcPoint00.Y > (_srcHeight - 1.001F))
				srcPoint00.Y = (_srcHeight - 1.001F);

			Point srcPointInt00 = new Point((int)srcPoint00.X, (int)srcPoint00.Y);

			float[,] arrayOfValues = new float[2, 2] { { 0, 0 }, { 0, 0 } };

			if (IsColor())
			{
				//Just test the R value, the calculation is done in exactly the same way 
				//for G & B, so if it's OK for the R channel it's OK for them too.

				//Get the 4 neighbour pixels for performing bilinear interpolation.
				arrayOfValues[0, 0] = (float)ColorPixelData().GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y).R;
				arrayOfValues[0, 1] = (float)ColorPixelData().GetPixelAsColor(srcPointInt00.X, srcPointInt00.Y + 1).R;
				arrayOfValues[1, 0] = (float)ColorPixelData().GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y).R;
				arrayOfValues[1, 1] = (float)ColorPixelData().GetPixelAsColor(srcPointInt00.X + 1, srcPointInt00.Y + 1).R;
			}
			else
			{
				if (_image.BitsPerPixel == 16)
				{
					//Get the 4 neighbour pixels for performing bilinear interpolation.
					arrayOfValues[0, 0] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X, srcPointInt00.Y);
					arrayOfValues[0, 1] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X, srcPointInt00.Y + 1);
					arrayOfValues[1, 0] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X + 1, srcPointInt00.Y);
					arrayOfValues[1, 1] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X + 1, srcPointInt00.Y + 1);
				}
				else if (_image.BitsPerPixel == 8)
				{
					//Get the 4 neighbour pixels for performing bilinear interpolation.
					arrayOfValues[0, 0] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X, srcPointInt00.Y);
					arrayOfValues[0, 1] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X, srcPointInt00.Y + 1);
					arrayOfValues[1, 0] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X + 1, srcPointInt00.Y);
					arrayOfValues[1, 1] = (float)GrayscalePixelData().GetPixel(srcPointInt00.X + 1, srcPointInt00.Y + 1);
				}
			}

			//TraceLine(String.Format("Pt: ({0}, {1})", srcPoint00.X, srcPoint00.Y));
			//TraceLine(String.Format("Values:\n{0}  {1}\n{2}  {3}", arrayOfValues[0, 0], arrayOfValues[1, 0], arrayOfValues[0, 1], arrayOfValues[1, 1]));

			//this actually performs the bilinear interpolation within the source image using 4 neighbour pixels.
			float dx = srcPoint00.X - (float)srcPointInt00.X;
			float dy = srcPoint00.Y - (float)srcPointInt00.Y;
			
			int dyFixed = (int)(dy * _fixedScale);
			int dxFixed = (int)(dx * _fixedScale);

			int yInterpolated1 = (((int)(arrayOfValues[0, 0])) << _fixedPrecision) + ((dyFixed * ((int)((arrayOfValues[0, 1] - arrayOfValues[0, 0])) << _fixedPrecision)) >> _fixedPrecision);
			int yInterpolated2 = (((int)(arrayOfValues[1, 0])) << _fixedPrecision) + ((dyFixed * ((int)((arrayOfValues[1, 1] - arrayOfValues[1, 0])) << _fixedPrecision)) >> _fixedPrecision);
			int interpolated = (yInterpolated1 + (((dxFixed) * (yInterpolated2 - yInterpolated1)) >> _fixedPrecision)) >> _fixedPrecision;

			//TraceLine(String.Format("Pt: ({0}, {1})", srcPoint00.X, srcPoint00.Y));
			//TraceLine(String.Format("Values:\n{0}  {1}\n{2}  {3}", arrayOfValues[0, 0], arrayOfValues[1, 0], arrayOfValues[0, 1], arrayOfValues[1, 1]));
			//TraceLine(String.Format("dx, dy = {0}, {1}", dx, dy));
			//TraceLine(String.Format("interpolated = {0}", interpolated));

			if (IsColor())
				return (byte)interpolated;

			//The image's LutComposer is private, so we just replicate it here and recalculate.
			GrayscaleImageGraphic graphic = (GrayscaleImageGraphic) _image;
			LutComposer composer = new LutComposer(graphic.BitsStored, graphic.IsSigned);
			composer.ModalityLut = graphic.ModalityLut;
			composer.VoiLut = graphic.VoiLut;
			var colorMap = new GrayscaleColorMap();

            //For this, we want the output range to be the same as the VOI.
            var output = composer.GetOutputLut(0, byte.MaxValue);
            colorMap.MaxInputValue = output.MaxOutputValue;
            colorMap.MinInputValue = output.MinOutputValue;

			return Color.FromArgb(colorMap[output[interpolated]]).R;
		}

		private void CreateImageLayer(ImageTypes imageType)
		{
			if (imageType == ImageTypes.Mono16)
				_image = new GrayscaleImageGraphic(_srcHeight, _srcWidth, 16, 16, 15, false, false, 1.9, 3, new byte[2 * _srcWidth * _srcHeight]);
			else if (imageType == ImageTypes.Mono8)
				_image = new GrayscaleImageGraphic(_srcHeight, _srcWidth, 8, 8, 7, false, false, 1.0, 0, new byte[_srcWidth * _srcHeight]);
			if (imageType == ImageTypes.Mono16Signed)
				_image = new GrayscaleImageGraphic(_srcHeight, _srcWidth, 16, 16, 15, true, false, 2.0, -630, new byte[2 * _srcWidth * _srcHeight]);
			else if (imageType == ImageTypes.Mono8Signed)
				_image = new GrayscaleImageGraphic(_srcHeight, _srcWidth, 8, 8, 7, true, false, 0.5, 4, new byte[_srcWidth * _srcHeight]);
			else
				_image = new ColorImageGraphic(_srcHeight, _srcWidth, new byte[4 * _srcWidth * _srcHeight]);

			if (_image is GrayscaleImageGraphic)
			{
				(_image as IColorMapInstaller).InstallColorMap(new GrayscaleColorMap());
			}

			_containerGraphic = new CompositeImageGraphic(_image.Rows, _image.Columns);
			_containerGraphic.Graphics.Add(_image);

			ImageSpatialTransform transform = (ImageSpatialTransform)_containerGraphic.SpatialTransform;
			transform.Initialize();
			transform.ClientRectangle = new Rectangle(0, 0, _dstWidth, _dstHeight);
			transform.ScaleToFit = _scaleToFit;
			transform.Scale = _scale;
			transform.FlipX = _flipHorizontal;
			transform.FlipY = _flipVertical;
			transform.RotationXY = _rotation;
			transform.TranslationX = _translationX;
			transform.TranslationY = _translationY;
		}

		private void CreatePhantom()
		{
			int max16 = 65535;
			int max8 = 255;
			int maxXPlusY;
			if (_srcHeight > 1 && _srcWidth > 1)
				maxXPlusY = _srcWidth + _srcHeight - 2;
			else
				maxXPlusY = 1;

			float scale16 = (float)max16 / maxXPlusY;
			float scale8 = (float)max8 / maxXPlusY;

			string strTracePhantom = "Phantom:\n";

			//fill the pixel data with values spanning the full possible range (using x + y as a base).
			for (int y = 0; y < _srcHeight; ++y) 
			{
				for (int x = 0; x < _srcWidth; ++x)
				{
					if (IsColor())
					{
						int value = (int)((x + y) * scale8);
						Color colour = Color.FromArgb(0xFF, value, value, value);
						ColorPixelData().SetPixel(x, y, colour);

						if (_tracePhantom)
							strTracePhantom += String.Format("{0}  ", value);
					}
					else
					{
						if (_sourceImageType == ImageTypes.Mono16)
						{
							ushort pixelValue = (ushort)((x + y) * scale16);
							GrayscalePixelData().SetPixel(x, y, pixelValue);
							if (_tracePhantom)
								strTracePhantom += String.Format("{0}  ", pixelValue);
						}
						else if (_sourceImageType == ImageTypes.Mono8)
						{
							byte pixelValue = (byte)((x + y) * scale8);
							GrayscalePixelData().SetPixel(x, y, pixelValue);
							if (_tracePhantom)
								strTracePhantom += String.Format("{0}  ", pixelValue);
						}
						else if (_sourceImageType == ImageTypes.Mono16Signed)
						{
							short pixelValue = (short)((x + y) * scale16 - 32767);
							GrayscalePixelData().SetPixel(x, y, pixelValue);
							if (_tracePhantom)
								strTracePhantom += String.Format("{0}  ", pixelValue);
						}
						else if (_sourceImageType == ImageTypes.Mono8Signed)
						{
							sbyte pixelValue = (sbyte)((x + y) * scale8 - 127);
							GrayscalePixelData().SetPixel(x, y, pixelValue);
							if (_tracePhantom)
								strTracePhantom += String.Format("{0}  ", pixelValue);
						}
					}
				}

				strTracePhantom += "\n";
			}

			if (_tracePhantom)
				TraceLine(strTracePhantom);
		}
	}
}

#endif
