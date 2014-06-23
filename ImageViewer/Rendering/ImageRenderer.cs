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

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal unsafe class ImageRenderer
	{
		[ThreadStatic]
		private static int[] _finalLutBuffer;

		public static void Render(
			ImageGraphic imageGraphic,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel,
			Rectangle clientRectangle)
		{
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
				return;

			if (imageGraphic.SizeInBytes != imageGraphic.PixelData.Raw.Length)
				throw new InvalidOperationException(String.Format(SR.ExceptionIncorrectPixelDataSize, imageGraphic.SizeInBytes, imageGraphic.PixelData.Raw.Length));

#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			RectangleF srcViewableRectangle;
			Rectangle dstViewableRectangle;

			CalculateVisibleRectangles(imageGraphic, clientRectangle, out dstViewableRectangle, out srcViewableRectangle);

		    var grayGraphic = imageGraphic as GrayscaleImageGraphic;
		    ColorImageGraphic colorGraphic;
            if (grayGraphic != null)
			{
				RenderGrayscale(
                    grayGraphic,
					srcViewableRectangle,
					dstViewableRectangle,
					pDstPixelData,
					dstWidth,
					dstBytesPerPixel);
			}
            else if (null != (colorGraphic = imageGraphic as ColorImageGraphic))
			{
				RenderColor(
                    colorGraphic,
					srcViewableRectangle,
					dstViewableRectangle,
					pDstPixelData,
					dstWidth,
					dstBytesPerPixel);
			}
			else
			{
				throw new Exception("Unknown ImageGraphic.");
			}
#if DEBUG
			clock.Stop();
			PerformanceReportBroker.PublishReport("ImageRenderer", "Render", clock.Seconds);
#endif
		}

		private static void RenderGrayscale(
			GrayscaleImageGraphic image,
			RectangleF srcViewableRectangle,
			Rectangle dstViewableRectangle,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel)
		{
			fixed (byte* pSrcPixelData = image.PixelData.Raw)
			{
				if (image.InterpolationMode == InterpolationMode.Bilinear)
				{
					//TODO: if we actually supported >8 bit displays, the LUT part would work ...
					var outputLut = image.GetOutputLut(0, byte.MaxValue);
					int[] finalLutBuffer = ConstructFinalLut(outputLut, image.ColorMap, image.Invert);

					fixed (int* pFinalLutData = finalLutBuffer)
					{
						ImageInterpolatorBilinear.LutData lutData;
						lutData.Data = pFinalLutData;
						lutData.FirstMappedPixelData = outputLut.MinInputValue;
						lutData.Length = finalLutBuffer.Length;

						ImageInterpolatorBilinear.Interpolate(
							srcViewableRectangle,
							pSrcPixelData,
							image.Columns,
							image.Rows,
							image.BytesPerPixel,
							image.BitsStored,
							dstViewableRectangle,
							(byte*) pDstPixelData,
							dstWidth,
							dstBytesPerPixel,
							IsRotated(image),
							&lutData, //ok because it's a local variable in an unsafe method, therefore it's already fixed.
							false,
							false,
							image.IsSigned);
					}
				}
			}
		}

		private static void RenderColor(
			ColorImageGraphic image,
			RectangleF srcViewableRectangle,
			Rectangle dstViewableRectangle,
			IntPtr pDstPixelData,
			int dstWidth,
			int dstBytesPerPixel)
		{
			fixed (byte* pSrcPixelData = image.PixelData.Raw)
			{
				if (image.InterpolationMode == InterpolationMode.Bilinear)
				{
					int srcBytesPerPixel = 4;

					if (image.VoiLutsEnabled)
					{
						int[] finalLutBuffer = ConstructFinalLut(image.OutputLut, image.Invert);
						fixed (int* pFinalLutData = finalLutBuffer)
						{
							ImageInterpolatorBilinear.LutData lutData;
							lutData.Data = pFinalLutData;
							lutData.FirstMappedPixelData = image.OutputLut.MinInputValue;
							lutData.Length = finalLutBuffer.Length;

							ImageInterpolatorBilinear.Interpolate(
								srcViewableRectangle,
								pSrcPixelData,
								image.Columns,
								image.Rows,
								srcBytesPerPixel,
								32,
								dstViewableRectangle,
								(byte*) pDstPixelData,
								dstWidth,
								dstBytesPerPixel,
								IsRotated(image),
								&lutData, //ok because it's a local variable in an unsafe method, therefore it's already fixed.
								true,
								false,
								false);
						}
					}
					else
					{
						ImageInterpolatorBilinear.Interpolate(
							srcViewableRectangle,
							pSrcPixelData,
							image.Columns,
							image.Rows,
							srcBytesPerPixel,
							32,
							dstViewableRectangle,
							(byte*) pDstPixelData,
							dstWidth,
							dstBytesPerPixel,
							IsRotated(image),
							null,
							true,
							false,
							false);
					}
				}
			}
		}

		private static bool IsRotated(ImageGraphic imageGraphic)
		{
			float m12 = imageGraphic.SpatialTransform.CumulativeTransform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		//internal for unit testing only.
		internal static void CalculateVisibleRectangles(
			ImageGraphic imageGraphic,
			Rectangle clientRectangle,
			out Rectangle dstVisibleRectangle,
			out RectangleF srcVisibleRectangle)
		{
			Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);
			RectangleF dstRectangle = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);

			// Find the intersection between the drawable client rectangle and
			// the transformed destination rectangle
			dstRectangle = RectangleUtilities.RoundInflate(dstRectangle);
			dstRectangle = RectangleUtilities.Intersect(clientRectangle, dstRectangle);

			if (dstRectangle.IsEmpty)
			{
				dstVisibleRectangle = Rectangle.Empty;
				srcVisibleRectangle = RectangleF.Empty;
				return;
			}

			// Figure out what portion of the image is visible in source coordinates
			srcVisibleRectangle = imageGraphic.SpatialTransform.ConvertToSource(dstRectangle);
			//dstRectangle is already rounded, this is just a conversion to Rectangle.
			dstVisibleRectangle = Rectangle.Round(dstRectangle);
		}

		private static int[] ConstructFinalLut(IComposedLut outputLut, IColorMap colorMap, bool invert)
		{
#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			colorMap.MinInputValue = outputLut.MinOutputValue;
			colorMap.MaxInputValue = outputLut.MaxOutputValue;

			int[] outputLutData = outputLut.Data;
			int[] colorMapData = colorMap.Data;

			if (_finalLutBuffer == null || _finalLutBuffer.Length != outputLutData.Length)
				_finalLutBuffer = new int[outputLutData.Length];

			int numberOfEntries = _finalLutBuffer.Length;

			fixed (int* pOutputLutData = outputLutData)
			fixed (int* pColorMapData = colorMapData)
			fixed (int* pFinalLutData = _finalLutBuffer)
			{
				int* pOutputLut = pOutputLutData;
				int* pFinalLut = pFinalLutData;

				if (!invert)
				{
					int firstColorMappedPixelValue = colorMap.FirstMappedPixelValue;
					for (int i = 0; i < numberOfEntries; ++i)
						*pFinalLut++ = pColorMapData[*pOutputLut++ - firstColorMappedPixelValue];
				}
				else
				{
					int lastColorMappedPixelValue = colorMap.FirstMappedPixelValue + colorMapData.Length - 1;
					for (int i = 0; i < numberOfEntries; ++i)
						*pFinalLut++ = pColorMapData[lastColorMappedPixelValue - *pOutputLut++];
				}
			}

#if DEBUG
			clock.Stop();
			PerformanceReportBroker.PublishReport("ImageRenderer", "ConstructFinalLut", clock.Seconds);
#endif
			return _finalLutBuffer;
		}

		private static int[] ConstructFinalLut(IComposedLut outputLut, bool invert)
		{
#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			int[] outputLutData = outputLut.Data;

			if (_finalLutBuffer == null || _finalLutBuffer.Length != outputLutData.Length)
				_finalLutBuffer = new int[outputLutData.Length];

			int numberOfEntries = _finalLutBuffer.Length;

			fixed (int* pOutputLutData = outputLutData)
			fixed (int* pFinalLutData = _finalLutBuffer)
			{
				int* pOutputLut = pOutputLutData;
				int* pFinalLut = pFinalLutData;

				if (!invert)
				{
					int firstColorMappedPixelValue = outputLut.MinOutputValue;
					for (int i = 0; i < numberOfEntries; ++i)
						*pFinalLut++ = *pOutputLut++ - firstColorMappedPixelValue;
				}
				else
				{
					int lastColorMappedPixelValue = outputLut.MaxOutputValue;
					for (int i = 0; i < numberOfEntries; ++i)
						*pFinalLut++ = lastColorMappedPixelValue - *pOutputLut++;
				}
			}

#if DEBUG
			clock.Stop();
			PerformanceReportBroker.PublishReport("ImageRenderer", "ConstructFinalLut", clock.Seconds);
#endif
			return _finalLutBuffer;
		}
	}
}