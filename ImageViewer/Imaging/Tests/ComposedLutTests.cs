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
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class ComposedLutTests
	{
		[Test]
		public void TestClassic16Bit()
		{
			const bool isSigned = false;
			const int bitsStored = 16;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.15, -100.80),
			           		new BasicVoiLutLinear(16794, 6920)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestClassic16BitSigned()
		{
			const bool isSigned = true;
			const int bitsStored = 16;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.15, -100.80),
			           		new BasicVoiLutLinear(16794, 0)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestClassic13Bit()
		{
			const bool isSigned = false;
			const int bitsStored = 13;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 2.15, -100),
			           		new BasicVoiLutLinear(4963, 6920)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestClassic13BitSigned()
		{
			const bool isSigned = true;
			const int bitsStored = 13;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 2.15, -100),
			           		new BasicVoiLutLinear(4963, 0)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestClassic8Bit()
		{
			const bool isSigned = false;
			const int bitsStored = 8;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.15, -100),
			           		new BasicVoiLutLinear(255, 127.5)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestClassic8BitSigned()
		{
			const bool isSigned = true;
			const int bitsStored = 8;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.15, -100),
			           		new BasicVoiLutLinear(255, 0)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestSubnormal15Bit()
		{
			const bool isSigned = false;
			const int bitsStored = 15;
			const double rescaleSlope = 0.000000153412;
			const double rescaleIntercept = 0;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept),
			           		new NormalizationLutLinear(rescaleSlope, rescaleIntercept),
			           		new BasicVoiLutLinear(12453, 6742)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void TestSubnormal15BitSigned()
		{
			const bool isSigned = true;
			const int bitsStored = 15;
			const double rescaleSlope = 0.000000153412;
			const double rescaleIntercept = 0;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept),
			           		new NormalizationLutLinear(rescaleSlope, rescaleIntercept),
			           		new BasicVoiLutLinear(12453, 6742)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void Test16BitDisplay16Bit()
		{
			const bool isSigned = false;
			const int bitsStored = 16;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.0145, -35767),
			           		new BasicVoiLutLinear(32743, 0),
			           		new PresentationLutLinear(0, 65535)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		[Test]
		public void Test16BitDisplay16BitSigned()
		{
			const bool isSigned = true;
			const int bitsStored = 16;

			var luts = new IComposableLut[]
			           	{
			           		new ModalityLutLinear(bitsStored, isSigned, 1.0145, 32767),
			           		new BasicVoiLutLinear(32743, 35314),
			           		new PresentationLutLinear(0, 65535)
			           	};
			var lutRange = SyncMinMaxValues(luts, bitsStored, isSigned);

			var composedLut = new ComposedLut(luts);

			AssertComposedLut(luts, composedLut, lutRange);
		}

		private static void AssertComposedLut(IComposableLut[] luts, ComposedLut composedLut, LutValueRange lutRange)
		{
			Assert.AreEqual(lutRange.MinInputValue, composedLut.MinInputValue, "MinInputValue");
			Assert.AreEqual(lutRange.MaxInputValue, composedLut.MaxInputValue, "MaxInputValue");
			Assert.AreEqual(lutRange.MinOutputValue, composedLut.MinOutputValue, "MinOutputValue");
			Assert.AreEqual(lutRange.MaxOutputValue, composedLut.MaxOutputValue, "MaxOutputValue");

			var data = composedLut.Data;
			Assert.IsNotNull(data, "Data");

			var lutCount = luts.Length;
			for (var i = -65536; i < 65535; ++i)
			{
				double value = i;
				for (var j = 0; j < lutCount; ++j)
					value = luts[j][value];

				var expectedValue = (int) Math.Round(value);
				Assert.AreEqual(expectedValue, composedLut[i], "LUT @{0}", i);

				if (i >= lutRange.MinInputValue && i <= lutRange.MaxInputValue)
				{
					Assert.AreEqual(expectedValue, data[i - lutRange.MinInputValue], "Data @{0} (Value {1})", i - lutRange.MinInputValue, i);
				}
			}
		}

		private static LutValueRange SyncMinMaxValues(IComposableLut[] luts, int bitsStored, bool isSigned)
		{
			var range = new LutValueRange();

			luts[0].MinInputValue = range.MinInputValue = DicomPixelData.GetMinPixelValue(bitsStored, isSigned);
			luts[0].MaxInputValue = range.MaxInputValue = DicomPixelData.GetMaxPixelValue(bitsStored, isSigned);

			var lutCount = luts.Length;
			for (var j = 1; j < lutCount; ++j)
			{
				luts[j].MinInputValue = luts[j - 1].MinOutputValue;
				luts[j].MaxInputValue = luts[j - 1].MaxOutputValue;
			}

			range.MinOutputValue = (int) Math.Round(luts[lutCount - 1].MinOutputValue);
			range.MaxOutputValue = (int) Math.Round(luts[lutCount - 1].MaxOutputValue);
			return range;
		}

		private struct LutValueRange
		{
			public int MinInputValue;
			public int MaxInputValue;

			public int MinOutputValue;
			public int MaxOutputValue;
		}
	}
}

#endif