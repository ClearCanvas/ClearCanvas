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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class FloatComparerTest
	{
		private enum CompareResult
		{
			AreEqual = 0,
			LessThan = -1,
			GreaterThan = 1
		}

		[Test(Description = "Test that the ordinary laws of math are being upheld.")]
		public void TestSanity()
		{
			// AreEqual
			{
				float x = 1.0f;
				float y = 2.0f;
				float z = x + y;

				Assert.IsTrue(FloatComparer.AreEqual(z, 3.0f));
			}

			// IsGreaterThan
			{
				float x = 1.001f;
				float y = 1.0f;

				Assert.IsTrue(FloatComparer.IsGreaterThan(x, y));
			}

			// IsLessThan
			{
				float x = 1.001f;
				float y = 1.0f;

				Assert.IsTrue(FloatComparer.IsLessThan(y, x));
			}
		}

		[Test]
		public void TestSinglePrecisionCompare()
		{
			// basic tests to ensure that ordinary math still works as expected
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, 0f, 0f, 0);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, 1.0f + 2.0f, 3.0f, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, 1.001f, 1.0f, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, 1.0f, 1.001f, 0);

			// tests to show that we can compare imprecise representations
			Assert.IsFalse(0.1f == 1f/10f);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, 0.1f, 1f/10f, 0);

			// tests application of significand tolerance 1
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, 1, 1.2345679f, 1.2345678f, 0);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, 1, 1.2345679f, 1.2345678f, 1);
			AssertSinglePrecisionCompare(CompareResult.LessThan, -1, 1.2345678f, 1.2345679f, 0);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, -1, 1.2345678f, 1.2345679f, 1);

			// tests application of significand tolerance 100
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, 100, 1.2345678f, 1.23455584f, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, 100, 1.2345678f, 1.23455584f, 99);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, 100, 1.2345678f, 1.23455584f, 100);
			AssertSinglePrecisionCompare(CompareResult.LessThan, -100, 1.2345678f, 1.23457968f, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, -100, 1.2345678f, 1.23457968f, 99);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, -100, 1.2345678f, 1.23457968f, 100);

			// tests that basic relationship holds for numbers differing only in sign
			AssertSinglePrecisionCompare(CompareResult.AreEqual, 2, float.Epsilon, -float.Epsilon, 2);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, 1.2345678f, -1.2345678f, 100);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, -1.2345678f, 1.2345678f, 100);

			// tests that basic relationship holds for numbers differing only in exponent
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, 1.2345678e8f, 1.2345678e-8f, 100);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, 1.2345678e-8f, 1.2345678e8f, 100);

			// tests that NaN and Infinity values are not compared as equal to real numbers
			const int maxTolerance = 4*1024*1024 - 1;
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, 1.2345678f, float.NaN, maxTolerance);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, 1.2345678f, float.PositiveInfinity, maxTolerance);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, 1.2345678f, float.NegativeInfinity, maxTolerance);

			// tests that basic relationships between NaN and Infinity values still hold
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, float.NaN, float.NaN, 100);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, float.PositiveInfinity, float.PositiveInfinity, 100);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.PositiveInfinity, float.NegativeInfinity, 100);
			AssertSinglePrecisionCompare(CompareResult.AreEqual, null, float.NegativeInfinity, float.NegativeInfinity, 100);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, float.NegativeInfinity, float.PositiveInfinity, 100);

			// tests that basic relationships between Max, Min and Epsilon still hold
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.MaxValue, float.MinValue, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.MaxValue, float.Epsilon, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.MaxValue, -float.Epsilon, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, float.MinValue, float.MaxValue, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, float.MinValue, float.Epsilon, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, float.MinValue, -float.Epsilon, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, float.Epsilon, float.MaxValue, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.Epsilon, float.MinValue, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, float.Epsilon, -float.Epsilon, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, -float.Epsilon, float.MaxValue, 0);
			AssertSinglePrecisionCompare(CompareResult.GreaterThan, null, -float.Epsilon, float.MinValue, 0);
			AssertSinglePrecisionCompare(CompareResult.LessThan, null, -float.Epsilon, float.Epsilon, 0);
		}

		[Test]
		public void TestDoublePrecisionCompare()
		{
			// basic tests to ensure that ordinary math still works as expected
			AssertDoublePrecisionCompare(CompareResult.AreEqual, null, 0, 0, 0);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, null, 1.0 + 2.0, 3.0, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, 1.001, 1.0, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, 1.0, 1.001, 0);

			// tests application of significand tolerance 1
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, 1, 1.234567890123456, 1.2345678901234559, 0);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, 1, 1.234567890123456, 1.2345678901234559, 1);
			AssertDoublePrecisionCompare(CompareResult.LessThan, -1, 1.234567890123456, 1.2345678901234563, 0);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, -1, 1.234567890123456, 1.2345678901234563, 1);

			// tests application of significand tolerance 100
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, 100, 1.234567890123456, 1.2345678901234338, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, 100, 1.234567890123456, 1.2345678901234338, 99);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, 100, 1.234567890123456, 1.2345678901234338, 100);
			AssertDoublePrecisionCompare(CompareResult.LessThan, -100, 1.234567890123456, 1.2345678901234782, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, -100, 1.234567890123456, 1.2345678901234782, 99);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, -100, 1.234567890123456, 1.2345678901234782, 100);

			// tests that basic relationship holds for numbers differing only in sign
			AssertDoublePrecisionCompare(CompareResult.AreEqual, 2, double.Epsilon, -double.Epsilon, 2);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, 1.234567890123456, -1.234567890123456, 100);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, -1.234567890123456, 1.234567890123456, 100);

			// tests that basic relationship holds for numbers differing only in exponent
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, 1.234567890123456e8, 1.234567890123456e-8, 100);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, 1.234567890123456e-8, 1.234567890123456e8, 100);

			// tests that NaN and Infinity values are not compared as equal to real numbers
			const long maxTolerance = 2L*1024*1024*1024*1024*1024 - 1;
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, 1.234567890123456, double.NaN, maxTolerance);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, 1.234567890123456, double.PositiveInfinity, maxTolerance);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, 1.234567890123456, double.NegativeInfinity, maxTolerance);

			// tests that basic relationships between NaN and Infinity values still hold
			AssertDoublePrecisionCompare(CompareResult.AreEqual, null, double.NaN, double.NaN, 100);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, null, double.PositiveInfinity, double.PositiveInfinity, 100);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.PositiveInfinity, double.NegativeInfinity, 100);
			AssertDoublePrecisionCompare(CompareResult.AreEqual, null, double.NegativeInfinity, double.NegativeInfinity, 100);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, double.NegativeInfinity, double.PositiveInfinity, 100);

			// tests that basic relationships between Max, Min and Epsilon still hold
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.MaxValue, double.MinValue, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.MaxValue, double.Epsilon, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.MaxValue, -double.Epsilon, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, double.MinValue, double.MaxValue, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, double.MinValue, double.Epsilon, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, double.MinValue, -double.Epsilon, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, double.Epsilon, double.MaxValue, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.Epsilon, double.MinValue, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, double.Epsilon, -double.Epsilon, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, -double.Epsilon, double.MaxValue, 0);
			AssertDoublePrecisionCompare(CompareResult.GreaterThan, null, -double.Epsilon, double.MinValue, 0);
			AssertDoublePrecisionCompare(CompareResult.LessThan, null, -double.Epsilon, double.Epsilon, 0);
		}

		[Test]
		public void TestInvalidTolerance()
		{
			AssertException<ArgumentOutOfRangeException>(() => FloatComparer.Compare(0f, 0f, -1));
			AssertException<ArgumentOutOfRangeException>(() => FloatComparer.Compare(0f, 0f, 4*1024*1024));
			AssertException<ArgumentOutOfRangeException>(() => FloatComparer.Compare(0.0, 0.0, -1L));
			AssertException<ArgumentOutOfRangeException>(() => FloatComparer.Compare(0.0, 0.0, 2L*1024*1024*1024*1024*1024));
		}

		[Test]
		public void TestConsistency()
		{
			AssertSinglePrecisionConsistency(CompareResult.AreEqual, 1.2345678e-8f, 1.2345678e-8f, "Testing for consistency across different methods");
			AssertSinglePrecisionConsistency(CompareResult.LessThan, 0, 1.2345678e-8f, "Testing for consistency across different methods");
			AssertSinglePrecisionConsistency(CompareResult.GreaterThan, 0, -1.2345678e8f, "Testing for consistency across different methods");

			AssertDoublePrecisionConsistency(CompareResult.AreEqual, 1.234567890123456e-8, 1.234567890123456e-8, "Testing for consistency across different methods");
			AssertDoublePrecisionConsistency(CompareResult.LessThan, 0, 1.234567890123456e-8, "Testing for consistency across different methods");
			AssertDoublePrecisionConsistency(CompareResult.GreaterThan, 0, -1.234567890123456e8, "Testing for consistency across different methods");
		}

		private static void AssertSinglePrecisionCompare(CompareResult expectedResult, int? expectedDifference, float x, float y, int tolerance)
		{
			int actualDifference;
			var actualResult = FloatComparer.Compare(x, y, tolerance, out actualDifference);

			Assert.AreEqual((int) expectedResult, actualResult, "Comparing {0:r} with {1:r} (\u00B1 {2} ULPs) [result]", x, y, tolerance);

			if (expectedDifference.HasValue)
				Assert.AreEqual(expectedDifference.Value, actualDifference, "Comparing {0:r} with {1:r} (\u00B1 {2} ULPs) [\u0394ULPs]", x, y, tolerance);
		}

		private static void AssertDoublePrecisionCompare(CompareResult expectedResult, long? expectedDifference, double x, double y, long tolerance)
		{
			long actualDifference;
			var actualResult = FloatComparer.Compare(x, y, tolerance, out actualDifference);

			Assert.AreEqual((int) expectedResult, actualResult, "Comparing {0:r} with {1:r} (\u00B1 {2} ULPs) [result]", x, y, tolerance);

			if (expectedDifference.HasValue)
				Assert.AreEqual(expectedDifference.Value, actualDifference, "Comparing {0:r} with {1:r} (\u00B1 {2} ULPs) [\u0394ULPs]", x, y, tolerance);
		}

		private static void AssertSinglePrecisionConsistency(CompareResult expectedResult, float x, float y, string message, params object[] args)
		{
			var expectedAreEqual = expectedResult == CompareResult.AreEqual;
			var expectedIsGreaterThan = expectedResult == CompareResult.GreaterThan;
			var expectedIsLessThan = expectedResult == CompareResult.LessThan;

			var actualResult = FloatComparer.Compare(x, y, 100);
			var actualAreEqual = FloatComparer.AreEqual(x, y);
			var actualIsGreaterThan = FloatComparer.IsGreaterThan(x, y);
			var actualIsLessThan = FloatComparer.IsLessThan(x, y);

			Assert.AreEqual((int) expectedResult, actualResult, message + " [Compare/float]", args);
			Assert.AreEqual(expectedAreEqual, actualAreEqual, message + " [AreEqual/float]", args);
			Assert.AreEqual(expectedIsGreaterThan, actualIsGreaterThan, message + " [IsGreaterThan/float]", args);
			Assert.AreEqual(expectedIsLessThan, actualIsLessThan, message + " [IsLessThan/float]", args);
		}

		private static void AssertDoublePrecisionConsistency(CompareResult expectedResult, double x, double y, string message, params object[] args)
		{
			var expectedAreEqual = expectedResult == CompareResult.AreEqual;
			var expectedIsGreaterThan = expectedResult == CompareResult.GreaterThan;
			var expectedIsLessThan = expectedResult == CompareResult.LessThan;

			var actualResult = FloatComparer.Compare(x, y, 100);
			var actualAreEqual = FloatComparer.AreEqual(x, y);
			var actualIsGreaterThan = FloatComparer.IsGreaterThan(x, y);
			var actualIsLessThan = FloatComparer.IsLessThan(x, y);

			Assert.AreEqual((int) expectedResult, actualResult, message + " [Compare/double]", args);
			Assert.AreEqual(expectedAreEqual, actualAreEqual, message + " [AreEqual/double]", args);
			Assert.AreEqual(expectedIsGreaterThan, actualIsGreaterThan, message + " [IsGreaterThan/double]", args);
			Assert.AreEqual(expectedIsLessThan, actualIsLessThan, message + " [IsLessThan/double]", args);
		}

		private delegate void AssertExceptionCallback();

		private static void AssertException<T>(AssertExceptionCallback callback)
			where T : Exception
		{
			try
			{
				callback.Invoke();
				Assert.Fail("Expected exception {0} was not thrown", typeof (T).FullName);
			}
			catch (T) {}
		}
	}
}

#endif