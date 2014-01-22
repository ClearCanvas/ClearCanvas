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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Utilities.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Vtk.Utilities.Tests
{
	[TestFixture]
	internal class SlabProjectionTests
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			SlabProjection.ForceCodeJit();
		}

		[Test]
		public void TestParallelProjectionWorkDivision()
		{
			SlabProjection.TestParallelJobDivision(13, 4);
			SlabProjection.TestParallelJobDivision(12, 4);
			SlabProjection.TestParallelJobDivision(11, 4);

			SlabProjection.TestParallelJobDivision(513, 4);
			SlabProjection.TestParallelJobDivision(512, 4);
			SlabProjection.TestParallelJobDivision(511, 4);

			SlabProjection.TestParallelJobDivision(9, 13);
			SlabProjection.TestParallelJobDivision(8, 13);
			SlabProjection.TestParallelJobDivision(7, 13);

			SlabProjection.TestParallelJobDivision(512*512 + 1, 13);
			SlabProjection.TestParallelJobDivision(512*512, 13);
			SlabProjection.TestParallelJobDivision(512*512 - 1, 13);
		}

		[Test]
		public unsafe void TestMaximumIntensityProjection()
		{
			const int pixels = 512*512;
			const int subsamples = 11;

			var rng = new PseudoRandom(0x2DB8498F);
			var slabData = new ushort[pixels*subsamples];
			for (var n = 0; n < slabData.Length; ++n)
				slabData[n] = (ushort) rng.Next(ushort.MinValue, ushort.MaxValue);

			var expectedResults = new ushort[pixels];
			for (var p = 0; p < pixels; ++p)
				expectedResults[p] = Enumerable.Range(0, subsamples).Select(s => slabData[s*pixels + p]).Max();

			var actualResults = new ushort[pixels];
			var projectedData = new byte[pixels*sizeof (ushort)];
			fixed (ushort* pSlabData = slabData)
			{
				SlabProjection.AggregateSlabMaximumIntensity((IntPtr) pSlabData, projectedData, subsamples, pixels, 2, false);
				Buffer.BlockCopy(projectedData, 0, actualResults, 0, projectedData.Length);
			}

			Assert.AreEqual(expectedResults, actualResults);
		}

		[Test]
		public unsafe void TestMinimumIntensityProjection()
		{
			const int pixels = 512*512;
			const int subsamples = 11;

			var rng = new PseudoRandom(0x2DB8498F);
			var slabData = new ushort[pixels*subsamples];
			for (var n = 0; n < slabData.Length; ++n)
				slabData[n] = (ushort) rng.Next(ushort.MinValue, ushort.MaxValue);

			var expectedResults = new ushort[pixels];
			for (var p = 0; p < pixels; ++p)
				expectedResults[p] = Enumerable.Range(0, subsamples).Select(s => slabData[s*pixels + p]).Min();

			var actualResults = new ushort[pixels];
			var projectedData = new byte[pixels*sizeof (ushort)];
			fixed (ushort* pSlabData = slabData)
			{
				SlabProjection.AggregateSlabMinimumIntensity((IntPtr) pSlabData, projectedData, subsamples, pixels, 2, false);
				Buffer.BlockCopy(projectedData, 0, actualResults, 0, projectedData.Length);
			}

			Assert.AreEqual(expectedResults, actualResults);
		}

		[Test]
		public unsafe void TestAverageIntensityProjection()
		{
			const int pixels = 512*512;
			const int subsamples = 11;

			var rng = new PseudoRandom(0x2DB8498F);
			var slabData = new ushort[pixels*subsamples];
			for (var n = 0; n < slabData.Length; ++n)
				slabData[n] = (ushort) rng.Next(ushort.MinValue, ushort.MaxValue);

			var expectedResults = new ushort[pixels];
			for (var p = 0; p < pixels; ++p)
				expectedResults[p] = (ushort) Math.Round(Enumerable.Range(0, subsamples).Select(s => slabData[s*pixels + p]).Average(v => v));

			SlabProjection.ForceCodeJit();

			var actualResults = new ushort[pixels];
			var projectedData = new byte[pixels*sizeof (ushort)];
			fixed (ushort* pSlabData = slabData)
			{
				SlabProjection.AggregateSlabAverageIntensity((IntPtr) pSlabData, projectedData, subsamples, pixels, 2, false);
				Buffer.BlockCopy(projectedData, 0, actualResults, 0, projectedData.Length);
			}

			Assert.AreEqual(expectedResults, actualResults);
		}
	}
}

namespace ClearCanvas.ImageViewer.Vtk.Utilities
{
// ReSharper disable RedundantAssignment

	internal partial class SlabProjection
	{
		internal static bool ReportStats { get; set; }

		internal static unsafe void ForceCodeJit()
		{
			const int pixels = 512*512;
			const int subsamples = 11;

			ReportStats = false;
			try
			{
				// perform a dummy run to ensure the parallelization code is JITed
				var projectedData = new byte[pixels*sizeof (ushort)];
				var slabData = new ushort[pixels*subsamples];
				fixed (ushort* pSlabData = slabData)
				{
					AggregateSlabAverageIntensity((IntPtr) pSlabData, projectedData, subsamples, pixels, 2, false);
					AggregateSlabMaximumIntensity((IntPtr) pSlabData, projectedData, subsamples, pixels, 2, false);
				}
			}
			finally
			{
				ReportStats = true;
			}
		}

		internal static void TestParallelJobDivision(int subsamplePixels, int maxThreads)
		{
			var syncLock = new object();
			var actual = new List<int>();

			AggregateSlabByIntensity((a, b, c, d, blockOffset, blockCount) =>
			                         	{
			                         		lock (syncLock)
			                         		{
			                         			actual.AddRange(Enumerable.Range(blockOffset, blockCount));
			                         		}
			                         	}, IntPtr.Zero, null, 0, subsamplePixels, maxThreads);

			actual.Sort();

			var expected = new List<int>(Enumerable.Range(0, subsamplePixels));

			Assert.AreEqual(expected, actual, "total pixel count = {0}, thread jobs = {1}", subsamplePixels, maxThreads);
		}

		static partial void StartClock(ref CodeClock codeClock)
		{
			codeClock = new CodeClock();
			codeClock.Start();
		}

		static partial void StopClock(CodeClock codeClock, string method, int pixels, int subsamples)
		{
			codeClock.Stop();

			var message = string.Format("Projection method '{0}' took {1:f3} [ms] to process {2} pixels with {3} subsamples", method, codeClock.Seconds*1000, pixels, subsamples);
			Platform.Log(LogLevel.Debug, message);

			if (ReportStats) Console.WriteLine(message);
		}
	}

// ReSharper restore RedundantAssignment
}

#endif