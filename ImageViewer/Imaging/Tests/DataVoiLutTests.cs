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

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class DataVoiLutTests
	{
		[Test]
		public void TestBasicDataVoiLut()
		{
			const int rngSeed = 0x0D924561;
			const int firstMappedPixelValue = -5;
			const int lastMappedPixelValue = 1243;

			var lutData = LutTestHelper.GenerateRandomLutData(lastMappedPixelValue - firstMappedPixelValue + 1, rngSeed);

			var dataVoiLut = new TestLut(lutData, firstMappedPixelValue, rngSeed.ToString("X8"));
			var composableLut = (IComposableLut) dataVoiLut;

			for (var n = 0; n < lutData.Length; ++n)
			{
				Assert.AreEqual(lutData[n], dataVoiLut[n + firstMappedPixelValue], "DataLut[@{0}]", n);
				Assert.AreEqual(lutData[n], composableLut[n + firstMappedPixelValue], "IComposableLut[@{0}]", n);
			}

			dataVoiLut.AssertLookupValues(-10, 2000);
		}

		[Test]
		public void TestAdjustableDataVoiLut()
		{
			const int rngSeed = 0x0D924561;
			const int firstMappedPixelValue = -5;
			const int lastMappedPixelValue = 1243;
			const double windowCenter = 12345;
			const double windowWidth = 65536;
			const int minOutputValue = int.MinValue;
			const int maxOutputValue = int.MaxValue;

			var lutData = LutTestHelper.GenerateRandomLutData(lastMappedPixelValue - firstMappedPixelValue + 1, rngSeed);

			var dataVoiLut = new AdjustableDataLut(new SimpleDataLut(firstMappedPixelValue, lutData, minOutputValue, maxOutputValue, rngSeed.ToString("X8"), rngSeed.ToString("X8")));
			var voiLut = (IBasicVoiLutLinear) dataVoiLut;

			// do not perform exact floating point assertion, as the AdjustableDataLut function is very likely to produce floating point error
			for (var n = 0; n < lutData.Length; ++n)
			{
				Assert.AreEqual(lutData[n], dataVoiLut[n + firstMappedPixelValue], 0.5, "AdjustableDataLut[@{0}]", n);
			}

			dataVoiLut.AssertLookupValues(-10, 2000);

			voiLut.WindowCenter = windowCenter;
			voiLut.WindowWidth = windowWidth;

			for (var n = 0; n < lutData.Length; ++n)
			{
				const double centerLessHalf = windowCenter - 0.5;
				const double windowLessOne = windowWidth - 1;
				const double halfWidth = windowLessOne/2;
				const double outputRange = ((double) maxOutputValue - minOutputValue);

				var input = lutData[n];

				double expected;
				if (input <= centerLessHalf - halfWidth)
					expected = minOutputValue;
				else if (input > centerLessHalf + halfWidth)
					expected = maxOutputValue;
				else
					expected = ((input - centerLessHalf)/windowLessOne + 0.5)*outputRange + minOutputValue;

				Assert.AreEqual(expected, dataVoiLut[n + firstMappedPixelValue], 0.5, "After Adjust - AdjustableDataLut[@{0}]", n);
			}

			dataVoiLut.AssertLookupValues(-10, 2000);
		}

		private class TestLut : DataVoiLut
		{
			private readonly string _key;
			private readonly int[] _data;
			private readonly int _firstMappedPixelValue;

			public TestLut(int[] data, int firstMappedPixelValue, string key)
			{
				_firstMappedPixelValue = firstMappedPixelValue;
				_data = data;
				_key = key;
			}

			public override int FirstMappedPixelValue
			{
				get { return _firstMappedPixelValue; }
			}

			public override int LastMappedPixelValue
			{
				get { return _firstMappedPixelValue + _data.Length - 1; }
			}

			public override int[] Data
			{
				get { return _data; }
			}

			public override string GetKey()
			{
				return _key;
			}

			public override string GetDescription()
			{
				return _key;
			}
		}
	}
}

#endif