#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	internal static class LutTestHelper
	{
		/// <summary>
		/// Generates pseudo-random integer LUT data.
		/// </summary>
		/// <param name="size">Number of entries in output LUT data.</param>
		/// <param name="rngSeed">Seed to use for RNG.</param>
		/// <returns>Generated LUT data.</returns>
		public static unsafe int[] GenerateRandomLutData(int size, int rngSeed)
		{
			var data = new int[size];
			fixed (int* pData = data)
				new PseudoRandom(rngSeed).NextBytes((IntPtr) pData, data.Length*sizeof (int));
			return data;
		}

		/// <summary>
		/// Asserts that the <see cref="IComposableLut.LookupValues"/> method produces the same results as performing individual lookups via <see cref="IComposableLut.this"/>.
		/// </summary>
		/// <param name="lut">The <see cref="IComposableLut"/> implementation under test.</param>
		/// <param name="minTestInputValue">Specifies the input test value range.</param>
		/// <param name="maxTestInputValue">Specifies the input test value range.</param>
		/// <param name="message">Message for NUnit Assertions.</param>
		/// <param name="args">Args for <paramref name="message"/>.</param>
		public static void AssertLookupValues(this IComposableLut lut, int minTestInputValue, int maxTestInputValue, string message = null, params object[] args)
		{
			const int countGuards = 9;
			const double baseGuard = 12.3456789;

			Platform.CheckTrue(minTestInputValue <= maxTestInputValue, "max test input value must be greater than or equal to min test input value");

			var count = maxTestInputValue - minTestInputValue + 1;
			var actualValues = new double[count + countGuards];
			var msg = !string.IsNullOrEmpty(message) ? " - " + string.Format(message, args) : null;

			// populate the array with test input values
			for (var i = 0; i < count; ++i)
				actualValues[i] = minTestInputValue + i;

			// populate end of array with guard values
			for (var i = 0; i < countGuards; ++i)
				actualValues[count + i] = baseGuard*i;

			// perform lookup (function under test)
			lut.LookupValues(actualValues, actualValues, count);

			// assert the result values
			for (var i = 0; i < count; ++i)
				Assert.AreEqual(lut[minTestInputValue + i], actualValues[i], "LookupValues output @{0} (Value {1}){2}", i, minTestInputValue + i, msg);

			// assert the guard values
			for (var i = 0; i < countGuards; ++i)
				Assert.AreEqual(baseGuard*i, actualValues[count + i], "LookupValues overruns allotted buffer @{0}{1}", count + i, msg);
		}
	}
}

#endif