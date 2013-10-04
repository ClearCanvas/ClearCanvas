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
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ColorMapTests
	{
		[Test]
		public void Test12Unsigned()
		{
		    var colorMap = new GrayscaleColorMap {MinInputValue = 0, MaxInputValue = 4095};

		    Assert.IsTrue(colorMap.Length == 4096);

			Color color = Color.FromArgb(colorMap.Data[0]);
		    Assert.AreEqual(255, color.A);
            Assert.AreEqual(0, color.R);
            Assert.IsTrue(color.R == color.G && color.G == color.B);

            color = Color.FromArgb(colorMap.Data[2048]);
            Assert.AreEqual(255, color.A);
            Assert.IsTrue(color.R == 128);
            Assert.IsTrue(color.R == color.G && color.G == color.B);

			color = Color.FromArgb(colorMap.Data[4095]);
            Assert.AreEqual(255, color.A);
            Assert.AreEqual(255, color.R);
            Assert.IsTrue(color.R == color.G && color.G == color.B);
        }

        [Test]
		public void Test12Signed()
		{
            var colorMap = new GrayscaleColorMap {MinInputValue = -2048, MaxInputValue = 2047};

            Assert.IsTrue(colorMap.Length == 4096);

			Assert.AreEqual(colorMap.Data[0], colorMap[-2048]);
            Color color = Color.FromArgb(colorMap.Data[0]);
            Assert.AreEqual(255, color.A);
            Assert.AreEqual(0, color.R);
            Assert.IsTrue(color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[2048], colorMap[0]);
            color = Color.FromArgb(colorMap.Data[2048]);
            Assert.AreEqual(255, color.A);
            Assert.IsTrue(color.R == 128);
            Assert.IsTrue(color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[4095], colorMap[2047]);
            color = Color.FromArgb(colorMap.Data[4095]);
            Assert.AreEqual(255, color.A);
            Assert.AreEqual(255, color.R);
            Assert.IsTrue(color.R == color.G && color.G == color.B);
        }

        [Test]
        public void Test8Range()
        {
            var colorMap = new GrayscaleColorMap {MinInputValue = 0, MaxInputValue = 255};

            Assert.AreEqual(colorMap.Data.Length, 256);

            //If the input is 0-255 (e.g. presentation LUT output range),
            //then the color map has to be a no-op.
            for (int i = 0; i < colorMap.Data.Length; ++i)
            {
                var value = 0xFF000000 | (i << 16) | (i << 8) | i;
                Assert.AreEqual((uint)value, (uint)colorMap.Data[i]);
            }
        }
	}
}

#endif