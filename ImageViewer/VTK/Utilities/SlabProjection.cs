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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Core.Functions;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Vtk.Utilities
{
	internal static partial class SlabProjection
	{
		public static void AggregateSlabIntensity(VolumeProjectionMode mode, IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			switch (mode)
			{
				case VolumeProjectionMode.Maximum:
					AggregateSlabMaximumIntensity(pSlabData, pixelData, subsamples, subsamplePixels, bytesPerPixel, signed);
					return;
				case VolumeProjectionMode.Minimum:
					AggregateSlabMinimumIntensity(pSlabData, pixelData, subsamples, subsamplePixels, bytesPerPixel, signed);
					return;
				case VolumeProjectionMode.Average:
				default:
					AggregateSlabAverageIntensity(pSlabData, pixelData, subsamples, subsamplePixels, bytesPerPixel, signed);
					return;
			}
		}

		public static void AggregateSlabMaximumIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			IntensityProjectionMethod method;
			switch (bytesPerPixel)
			{
				case 1:
					method = signed ? (IntensityProjectionMethod) MaximumIntensityProjectionS8 : MaximumIntensityProjectionU8;
					break;
				case 2:
					method = signed ? (IntensityProjectionMethod) MaximumIntensityProjectionS16 : MaximumIntensityProjectionU16;
					break;
				case 4:
					method = signed ? (IntensityProjectionMethod) MaximumIntensityProjectionS32 : MaximumIntensityProjectionU32;
					break;
				default:
					throw new NotSupportedException();
			}

			CodeClock cc = null;
			StartClock(ref cc);

			AggregateSlabByIntensity(method, pSlabData, pixelData, subsamples, subsamplePixels, ImageProcessingHelper.MaxParallelThreads);

			StopClock(cc, "Maximum", subsamplePixels, subsamples);
		}

		public static void AggregateSlabMinimumIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			IntensityProjectionMethod method;
			switch (bytesPerPixel)
			{
				case 1:
					method = signed ? (IntensityProjectionMethod) MinimumIntensityProjectionS8 : MinimumIntensityProjectionU8;
					break;
				case 2:
					method = signed ? (IntensityProjectionMethod) MinimumIntensityProjectionS16 : MinimumIntensityProjectionU16;
					break;
				case 4:
					method = signed ? (IntensityProjectionMethod) MinimumIntensityProjectionS32 : MinimumIntensityProjectionU32;
					break;
				default:
					throw new NotSupportedException();
			}

			CodeClock cc = null;
			StartClock(ref cc);

			AggregateSlabByIntensity(method, pSlabData, pixelData, subsamples, subsamplePixels, ImageProcessingHelper.MaxParallelThreads);

			StopClock(cc, "Minimum", subsamplePixels, subsamples);
		}

		public static void AggregateSlabAverageIntensity(IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int bytesPerPixel, bool signed)
		{
			IntensityProjectionMethod method;
			switch (bytesPerPixel)
			{
				case 1:
					method = signed ? (IntensityProjectionMethod) AverageIntensityProjectionS8 : AverageIntensityProjectionU8;
					break;
				case 2:
					method = signed ? (IntensityProjectionMethod) AverageIntensityProjectionS16 : AverageIntensityProjectionU16;
					break;
				case 4:
					method = signed ? (IntensityProjectionMethod) AverageIntensityProjectionS32 : AverageIntensityProjectionU32;
					break;
				default:
					throw new NotSupportedException();
			}

			CodeClock cc = null;
			StartClock(ref cc);

			AggregateSlabByIntensity(method, pSlabData, pixelData, subsamples, subsamplePixels, ImageProcessingHelper.MaxParallelThreads);

			StopClock(cc, "Average", subsamplePixels, subsamples);
		}

		private delegate void IntensityProjectionMethod(IntPtr pSlabData, IntPtr pixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount);

		private static unsafe void AggregateSlabByIntensity(IntensityProjectionMethod method, IntPtr pSlabData, byte[] pixelData, int subsamples, int subsamplePixels, int maxThreads)
		{
			fixed (byte* pPixelData = pixelData)
			{
				var ptrPixelData = new IntPtr(pPixelData);

				// only parallelize if requested, and there's enough work for multiple threads to do stuff (otherwise overhead would just kill performance)
				if (maxThreads > 1 && subsamplePixels > 32768*maxThreads)
				{
					// determine the number of pixels to process per thread job
					var jobSize = subsamplePixels/maxThreads;

					// first MAX_THREADS-1 jobs will have exactly JOB_SIZE pixels, and the last will be everything else (thus including any remainder caused by the determination of JOB_SIZE)
					var lastJob = maxThreads - 1;
					Enumerable.Range(0, maxThreads).Select(g => new KeyValuePair<int, int>(g*jobSize, g == lastJob ? subsamplePixels - lastJob*jobSize : jobSize))
						.AsParallel().WithDegreeOfParallelism(maxThreads)
						.ForAll(r => method(pSlabData, ptrPixelData, subsamples, subsamplePixels, r.Key, r.Value));
				}
				else
				{
					method(pSlabData, ptrPixelData, subsamples, subsamplePixels, 0, subsamplePixels);
				}
			}
		}

		#region Maximum Intensity Methods

		private static unsafe void MaximumIntensityProjectionU8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((byte*) pSlabData, (byte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MaximumIntensityProjectionS8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((sbyte*) pSlabData, (sbyte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MaximumIntensityProjectionU16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((ushort*) pSlabData, (ushort*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MaximumIntensityProjectionS16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((short*) pSlabData, (short*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MaximumIntensityProjectionU32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((uint*) pSlabData, (uint*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MaximumIntensityProjectionS32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MaximumIntensityProjection.ProjectOrthogonal((int*) pSlabData, (int*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		#endregion

		#region Minimum Intensity Methods

		private static unsafe void MinimumIntensityProjectionU8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((byte*) pSlabData, (byte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MinimumIntensityProjectionS8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((sbyte*) pSlabData, (sbyte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MinimumIntensityProjectionU16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((ushort*) pSlabData, (ushort*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MinimumIntensityProjectionS16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((short*) pSlabData, (short*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MinimumIntensityProjectionU32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((uint*) pSlabData, (uint*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void MinimumIntensityProjectionS32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			MinimumIntensityProjection.ProjectOrthogonal((int*) pSlabData, (int*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		#endregion

		#region Average Intensity Methods

		private static unsafe void AverageIntensityProjectionU8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((byte*) pSlabData, (byte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void AverageIntensityProjectionS8(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((sbyte*) pSlabData, (sbyte*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void AverageIntensityProjectionU16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((ushort*) pSlabData, (ushort*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void AverageIntensityProjectionS16(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((short*) pSlabData, (short*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void AverageIntensityProjectionU32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((uint*) pSlabData, (uint*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		private static unsafe void AverageIntensityProjectionS32(IntPtr pSlabData, IntPtr pPixelData, int subsamples, int subsamplePixels, int blockOffset, int blockCount)
		{
			AverageIntensityProjection.ProjectOrthogonal((int*) pSlabData, (int*) pPixelData, subsamples, subsamplePixels, blockOffset, blockCount);
		}

		#endregion

		static partial void StartClock(ref CodeClock codeClock);
		static partial void StopClock(CodeClock codeClock, string method, int pixels, int subsamples);
	}
}