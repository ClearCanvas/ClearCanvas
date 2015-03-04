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

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class IdentityVoiLinearLutTests
	{
		[Test]
		public void TestConstruction()
		{
			IdentityVoiLinearLut lut = new IdentityVoiLinearLut();
			Assert.NotNull(lut);
		}

		[Test]
		public void Test1Bit()
		{
			const int bitDepth = 1;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void Test3Bit()
		{
			const int bitDepth = 3;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void Test4Bit()
		{
			const int bitDepth = 4;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void Test8Bit()
		{
			const int bitDepth = 8;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void Test12Bit()
		{
			const int bitDepth = 12;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void Test31Bit()
		{
			const int bitDepth = 31;
			TestIdentityLut(new IdentityVoiLinearLut(bitDepth, false), bitDepth);
		}

		[Test]
		public void TestLookupValues()
		{
			var lut = new IdentityVoiLinearLut(13, false);

			lut.AssertLookupValues(-1, 1 << 13);
		}

		private static void TestIdentityLut(IdentityVoiLinearLut lut, int bitDepth)
		{
			double max = Math.Pow(2, bitDepth) - 1;

			Assert.AreEqual(0, lut.MinInputValue, "Minimum Input Value is wrong!");
			Assert.AreEqual(max, lut.MaxInputValue, "Maximum Input Value is wrong!");
			Assert.AreEqual(0, lut.MinOutputValue, "Minimum Output Value is wrong!");
			Assert.AreEqual(max, lut.MaxOutputValue, "Maximum Output Value is wrong!");
			Assert.AreEqual((max + 1d)/2d, lut.WindowCenter, "Window Centre is wrong!");
			Assert.AreEqual(max + 1d, lut.WindowWidth, "Window Width is wrong!");

			if (bitDepth <= 8)
			{
				for (double n = 0; n <= max; n += 0.5)
				{
					Assert.AreEqual(n, lut[n], "LUT Value {0} is wrong!", n);
				}
			}
			else
			{
				Assert.AreEqual(0, lut[0], "LUT Value {0} is wrong!", 0);
				Assert.AreEqual(1, lut[1], "LUT Value {0} is wrong!", 1);
				Assert.AreEqual(max/8, lut[max/8], "LUT Value {0} is wrong!", max/8);
				Assert.AreEqual(max/4, lut[max/4], "LUT Value {0} is wrong!", max/4);
				Assert.AreEqual(max*3/8, lut[max*3/8], "LUT Value {0} is wrong!", max*3/8);
				Assert.AreEqual(max/2, lut[max/2], "LUT Value {0} is wrong!", max/2);
				Assert.AreEqual(max*5/8, lut[max*5/8], "LUT Value {0} is wrong!", max*5/8);
				Assert.AreEqual(max*3/4, lut[max*3/4], "LUT Value {0} is wrong!", max*3/4);
				Assert.AreEqual(max*7/8, lut[max*7/8], "LUT Value {0} is wrong!", max*7/8);
				Assert.AreEqual(max - 1, lut[max - 1], "LUT Value {0} is wrong!", max - 1);
				Assert.AreEqual(max, lut[max], "LUT Value {0} is wrong!", max);
			}
		}
	}
}

#endif