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
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.VTK.Utilities
{
	internal static unsafe partial class SlabProjection
	{
		public static void AggregateSlabMaximumIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			if (bytesPerPixel != 2 || signed) throw new NotSupportedException();

			CodeClock cc = null;
			StartClock(ref cc);

			// initialize managed buffer with content of first subsample
			Marshal.Copy(pSlabData, pixelData, 0, pixelData.Length);

			fixed (byte* pPixelData = pixelData)
			{
				// skip one subsample worth of data, since it's already been copied to output buffer
				var pInput = ((ushort*) pSlabData) + subsamplePixels;
				for (var i = 1; i < subsamples; ++i)
				{
					// iterate across the subsample, comparing values and updating output buffer if value is greater than current
					var pOutput = (ushort*) pPixelData;
					for (var n = 0; n < subsamplePixels; ++n)
					{
						var value = *pInput++;
						if (value > *pOutput) *pOutput = value;
						++pOutput;
					}
				}
			}

			StopClock(cc, "Maximum", subsamplePixels, subsamples);
		}

		public static void AggregateSlabMinimumIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			if (bytesPerPixel != 2 || signed) throw new NotSupportedException();

			CodeClock cc = null;
			StartClock(ref cc);

			// initialize managed buffer with content of first subsample
			Marshal.Copy(pSlabData, pixelData, 0, pixelData.Length);

			fixed (byte* pPixelData = pixelData)
			{
				// skip one subsample worth of data, since it's already been copied to output buffer
				var pInput = ((ushort*) pSlabData) + subsamplePixels;
				for (var i = 1; i < subsamples; ++i)
				{
					// iterate across the subsample, comparing values and updating output buffer if value is lesser than current
					var pOutput = (ushort*) pPixelData;
					for (var n = 0; n < subsamplePixels; ++n)
					{
						var value = *pInput++;
						if (value < *pOutput) *pOutput = value;
						++pOutput;
					}
				}
			}

			StopClock(cc, "Minimum", subsamplePixels, subsamples);
		}

		public static void AggregateSlabAverageIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			if (bytesPerPixel != 2 || signed) throw new NotSupportedException();

			CodeClock cc = null;
			StartClock(ref cc);

			// allocate an unmanaged buffer to store the per-pixel sums
			var pSumData = Marshal.AllocHGlobal(sizeof (int)*subsamplePixels);
			try
			{
				// initialize sums buffer with content of first subsample
				var pSums = (int*) pSumData;
				{
					var pSubsample = (ushort*) pSlabData;
					for (var n = 0; n < subsamplePixels; ++n)
						*pSums++ = *pSubsample++;
				}

				// skip one subsample worth of data, since it's already been copied to output buffer
				var pInput = ((ushort*) pSlabData) + subsamplePixels;
				for (var i = 1; i < subsamples; ++i)
				{
					// iterate across the subsample, adding values into the sums buffer
					pSums = (int*) pSumData;
					for (var n = 0; n < subsamplePixels; ++n)
					{
						var value = *pInput++;
						*pSums += value;
						++pSums;
					}
				}

				fixed (byte* pPixelData = pixelData)
				{
					pSums = (int*) pSumData;
					var pOutput = (ushort*) pPixelData;
					for (var n = 0; n < subsamplePixels; ++n)
					{
						var value = *pSums++;
						*pOutput = (ushort) (1.0*value/subsamples + (value > 0 ? 0.5 : -0.5));
						++pOutput;
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal(pSumData);
			}

			StopClock(cc, "Average", subsamplePixels, subsamples);
		}

		static partial void StartClock(ref CodeClock codeClock);
		static partial void StopClock(CodeClock codeClock, string method, int pixels, int subsamples);
	}
}