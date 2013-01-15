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

using System;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Converts between colour spaces.
	/// </summary>
	public unsafe static class ColorSpaceConverter
	{
		private delegate int YbrToRgb(int y, int b, int r);

		#region Public methods

		/// <summary>
		/// Converts a YBR_FULL value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFullToRgb(int y, int b, int r)
		{
			// |r|   | 1.0000  0.0000  1.4020 | | y       |
			// |g| = | 1.0000 -0.3441 -0.7141 | | b - 128 |
			// |b|   | 1.0000  1.7720 -0.0000 | | r - 128 |

			int alpha = 0xff;
			int red = (int)(y + 1.4020 * (r - 128) + 0.5);
			int green = (int)(y - 0.3441 * (b - 128) - 0.7141 * (r - 128) + 0.5);
			int blue = (int)(y + 1.7720 * (b - 128) + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_FULL422 value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFull422ToRgb(int y, int b, int r)
		{
			return YbrFullToRgb(y, b, r);
		}

		/// <summary>
		/// Converts a YBR_PARTIAL422 value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrPartial422ToRgb(int y, int b, int r)
		{
			// |r|   |  1.1644  0.0000  1.5960 | | y - 16  |
			// |g| = |  1.1644 -0.3917 -0.8130 | | b - 128 |
			// |b|   |  1.1644  2.0173  0.0000 | | r - 128 |

			int alpha = 0xff;
			int red = (int)(1.1644 * (y - 16) + 1.5960 * (r - 128) + 0.5);
			int green = (int)(1.1644 * (y - 16) - 0.3917 * (b - 128) - 0.8130 * (r - 128) + 0.5);
			int blue = (int)(1.1644 * (y - 16) + 2.0173 * (b - 128) + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_ICT value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrIctToRgb(int y, int b, int r)
		{
			// |r|   |  1.00000  0.00000  1.40200 | | y |
			// |g| = |  1.00000 -0.34412 -0.71414 | | b |
			// |b|   |  1.00000  1.77200  0.00000 | | r |

			int alpha = 0xff;
			int red = (int)(y + 1.40200 * r + 0.5);
			int green = (int)(y - 0.34412 * b - 0.71414 * r + 0.5);
			int blue = (int)(y + 1.77200 * b + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);
			
			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_RCT value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrRctToRgb(int y, int b, int r)
		{
			int alpha = 0xff;
			int green = y - (r + b) / 4;
			int red = r + green;
			int blue = b + green;

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);
			
			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts pixel data of a particular photometric interpretation
		/// to ARGB.
		/// </summary>
		/// <param name="photometricInterpretation">The <see cref="PhotometricInterpretation"/> of <paramref name="srcPixelData"/>.</param>
		/// <param name="planarConfiguration">The planar configuration of <paramref name="srcPixelData"/>.</param>
		/// <param name="srcPixelData">The input pixel data to be converted.</param>
		/// <param name="argbPixelData">The converted output pixel data in ARGB format.</param>
		/// <remarks>
		/// Only RGB and YBR variants can be converted.  For PALETTE COLOR, use <see cref="ToArgb(int,bool,byte[],byte[],IDataLut)"/>.
		/// </remarks>
		public static void ToArgb(
			PhotometricInterpretation photometricInterpretation,
			int planarConfiguration,
			byte[] srcPixelData,
			byte[] argbPixelData)
		{
			int sizeInPixels = argbPixelData.Length / 4;

			if (photometricInterpretation == PhotometricInterpretation.Monochrome1 ||
				photometricInterpretation == PhotometricInterpretation.Monochrome2 ||
				photometricInterpretation == PhotometricInterpretation.PaletteColor ||
				photometricInterpretation == PhotometricInterpretation.Unknown)
				throw new Exception("Invalid photometric interpretation.  Must be either RGB or a YBR variant.");

			if (photometricInterpretation == PhotometricInterpretation.Rgb)
			{
				if (planarConfiguration == 0)
					RgbTripletToArgb(srcPixelData, argbPixelData, sizeInPixels);
				else
					RgbPlanarToArgb(srcPixelData, argbPixelData, sizeInPixels);
			}
			else
			{
				if (planarConfiguration == 0)
					YbrTripletToArgb(srcPixelData, argbPixelData, sizeInPixels, photometricInterpretation);
				else
					YbrPlanarToArgb(srcPixelData, argbPixelData, sizeInPixels, photometricInterpretation);
			}
		}

		/// <summary>
		/// Converts indexed pixel data to ARGB using the specified colour map.
		/// </summary>
		/// <param name="bitsAllocated">Number of bits allocated in <paramref name="srcPixelData"/>.</param>
		/// <param name="isSigned">Indicates whether <paramref name="srcPixelData"/> is signed.</param>
		/// <param name="srcPixelData">The input pixel data to be converted.</param>
		/// <param name="argbPixelData">The converted output pixel data in ARGB format.</param>
		/// <param name="map">The colour map to be used.</param>
		/// <remarks>
		/// Internally, this method is used to convert PALETTE COLOR images to ARGB.
		/// </remarks>
		public static void ToArgb(
			int bitsAllocated,
			bool isSigned, 
			byte[] srcPixelData, 
			byte[] argbPixelData, 
			IColorMap map)
		{
			int sizeInPixels = argbPixelData.Length/4;
			int firstPixelMapped = map.MinInputValue;

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					fixed (int* pColorMap = map.Data)
					{
						int dst = 0;

						if (bitsAllocated == 8)
						{
							if (isSigned)
							{
								// 8-bit signed
								for (int i = 0; i < sizeInPixels; i++)
								{
									int value = pColorMap[((sbyte*)pSrcPixelData)[i] - firstPixelMapped];
									pArgbPixelData[dst] = Color.FromArgb(value).B;
									pArgbPixelData[dst + 1] = Color.FromArgb(value).G;
									pArgbPixelData[dst + 2] = Color.FromArgb(value).R;
									pArgbPixelData[dst + 3] = Color.FromArgb(value).A;

									dst += 4;
								}
							}
							else
							{
								// 8-bit unsigned
								for (int i = 0; i < sizeInPixels; i++)
								{
									int value = pColorMap[pSrcPixelData[i] - firstPixelMapped];
									pArgbPixelData[dst] = Color.FromArgb(value).B;
									pArgbPixelData[dst + 1] = Color.FromArgb(value).G;
									pArgbPixelData[dst + 2] = Color.FromArgb(value).R;
									pArgbPixelData[dst + 3] = Color.FromArgb(value).A;

									dst += 4;
								}
							}
						}
						else
						{
							if (isSigned)
							{
								// 16-bit signed
								for (int i = 0; i < sizeInPixels; i++)
								{
									int value = pColorMap[((short*)pSrcPixelData)[i] - firstPixelMapped];
									pArgbPixelData[dst] = Color.FromArgb(value).B;
									pArgbPixelData[dst + 1] = Color.FromArgb(value).G;
									pArgbPixelData[dst + 2] = Color.FromArgb(value).R;
									pArgbPixelData[dst + 3] = Color.FromArgb(value).A;

									dst += 4;
								}
							}
							else
							{
								// 16-bit unsinged
								for (int i = 0; i < sizeInPixels; i++)
								{
									int value = pColorMap[((ushort*)pSrcPixelData)[i] - firstPixelMapped];
									pArgbPixelData[dst] = Color.FromArgb(value).B;
									pArgbPixelData[dst + 1] = Color.FromArgb(value).G;
									pArgbPixelData[dst + 2] = Color.FromArgb(value).R;
									pArgbPixelData[dst + 3] = Color.FromArgb(value).A;

									dst += 4;
								}
							}
						}
					}
				}
			}
		}

		#endregion

		#region Private methods

		private static void Limit(ref int color)
		{
			if (color < 0)
				color = 0;
			else if (color > 255)
				color = 255;
		}

		#region RGB to ARGB

		private static void RgbTripletToArgb(byte[] rgbPixelData, byte[] argbPixelData, int sizeInPixels)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					for (int i = 0; i < sizeInPixels; i++)
					{
						pArgbPixelData[dst] = pRgbPixelData[src + 2];
						pArgbPixelData[dst + 1] = pRgbPixelData[src + 1];
						pArgbPixelData[dst + 2] = pRgbPixelData[src];
						pArgbPixelData[dst + 3] = 0xff;

						src += 3;
						dst += 4;
					}
				}
			}
		}

		private static void RgbPlanarToArgb(byte[] rgbPixelData, byte[] argbPixelData, int sizeInPixels)
		{
			fixed (byte* pRgbPixelData = rgbPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int greenOffset = sizeInPixels;
					int blueOffset = sizeInPixels * 2;

					for (int i = 0; i < sizeInPixels; i++)
					{
						pArgbPixelData[dst] = pRgbPixelData[src + blueOffset];
						pArgbPixelData[dst + 1] = pRgbPixelData[src + greenOffset];
						pArgbPixelData[dst + 2] = pRgbPixelData[src];
						pArgbPixelData[dst + 3] = 0xff;

						src += 1;
						dst += 4;
					}
				}
			}
		}

		#endregion

		#region YBR to ARGB

		private static void YbrTripletToArgb(
			byte[] ybrPixelData,
			byte[] argbPixelData,
			int sizeInPixels,
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < sizeInPixels; i++)
					{
						int rgb = converter(
							pYbrPixelData[src],
							pYbrPixelData[src + 1],
							pYbrPixelData[src + 2]);

						pArgbPixelData[dst] = Color.FromArgb(rgb).B;
						pArgbPixelData[dst + 1] = Color.FromArgb(rgb).G;
						pArgbPixelData[dst + 2] = Color.FromArgb(rgb).R;
						pArgbPixelData[dst + 3] = 0xff;

						src += 3;
						dst += 4;
					}
				}
			}
		}

		private static void YbrPlanarToArgb(
			byte[] ybrPixelData,
			byte[] argbPixelData,
			int sizeInPixels, 
			PhotometricInterpretation photometricInterpretation)
		{
			fixed (byte* pYbrPixelData = ybrPixelData)
			{
				fixed (byte* pArgbPixelData = argbPixelData)
				{
					int src = 0;
					int dst = 0;

					int bOffset = sizeInPixels;
					int rOffset = sizeInPixels*2;

					YbrToRgb converter = GetYbrToRgbConverter(photometricInterpretation);

					for (int i = 0; i < sizeInPixels; i++)
					{
						int rgb = converter(
							pYbrPixelData[src],
							pYbrPixelData[src + bOffset],
							pYbrPixelData[src + rOffset]);

						pArgbPixelData[dst] = Color.FromArgb(rgb).B;
						pArgbPixelData[dst + 1] = Color.FromArgb(rgb).G;
						pArgbPixelData[dst + 2] = Color.FromArgb(rgb).R;
						pArgbPixelData[dst + 3] = 0xff;

						src += 1;
						dst += 4;
					}
				}
			}
		}

		#endregion

		private static YbrToRgb GetYbrToRgbConverter(PhotometricInterpretation photometricInterpretation)
		{
			YbrToRgb converter;

			if (photometricInterpretation == PhotometricInterpretation.YbrFull)
				converter = new YbrToRgb(YbrFullToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrFull422)
				converter = new YbrToRgb(YbrFull422ToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrIct)
				converter = new YbrToRgb(YbrIctToRgb);
			else if (photometricInterpretation == PhotometricInterpretation.YbrPartial422)
				converter = new YbrToRgb(YbrPartial422ToRgb);
			else
				converter = new YbrToRgb(YbrRctToRgb);

			return converter;
		}

		#endregion
	}
}
