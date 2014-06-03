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

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class MinMaxPixelCalculatedLutTests
	{
		[Test]
		public void TestSimple()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			GrayscalePixelData pixelData = new GrayscalePixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(24, lut.WindowWidth, "WindowWidth");
			Assert.AreEqual(12, lut.WindowCenter, "WindowCenter");

			lut.AssertLookupValues(-300, 300);
		}

		[Test]
		public void TestWithModalityLut()
		{
			byte[] data = new byte[25];
			for (byte x = 0; x < 25; ++x)
			{
				data[x] = x;
			}

			ModalityLutLinear modalityLut = new ModalityLutLinear(8, true, 1.0, -10);
			GrayscalePixelData pixelData = new GrayscalePixelData(5, 5, 8, 8, 7, false, data);
			MinMaxPixelCalculatedLinearLut lut = new MinMaxPixelCalculatedLinearLut(pixelData, modalityLut);
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;

			Assert.AreEqual(24, lut.WindowWidth, "WindowWidth");
			Assert.AreEqual(2, lut.WindowCenter, "WindowCenter");

			lut.AssertLookupValues(-300, 300);
		}
	}
}

#endif