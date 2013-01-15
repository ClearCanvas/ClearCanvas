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

using System.Globalization;
using NUnit.Framework;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ColorMapTests
	{
		public ColorMapTests()
		{
		}

		[Test]
		public void Test12Unsigned()
		{
			GrayscaleColorMap colorMap = new GrayscaleColorMap();
			colorMap.MinInputValue = 0;
			colorMap.MaxInputValue = 4095;
			
			Assert.IsTrue(colorMap.Length == 4096);

			Color color = Color.FromArgb(colorMap.Data[0]);
			Assert.IsTrue(255 == color.A && 0 == color.R && color.R == color.G && color.G == color.B);

			color = Color.FromArgb(colorMap.Data[2047]);
			Assert.IsTrue(127 == color.R && color.R == color.G && color.G == color.B);

			color = Color.FromArgb(colorMap.Data[4095]);
			Assert.IsTrue(255 == color.R && color.R == color.G && color.G == color.B);
		}

		public void Test12Signed()
		{
			GrayscaleColorMap colorMap = new GrayscaleColorMap();
			colorMap.MinInputValue = -2048;
			colorMap.MaxInputValue = 2047;

			Assert.IsTrue(colorMap.Length == 4096);

			Assert.AreEqual(colorMap.Data[0], colorMap[-2048]);
			Color color = Color.FromArgb(colorMap.Data[0]);
			Assert.IsTrue(255 == color.A && 0 == color.R && color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[2048], colorMap[0]);
			color = Color.FromArgb(colorMap.Data[2048]);
			Assert.IsTrue(127 == color.R && color.R == color.G && color.G == color.B);

			Assert.AreEqual(colorMap.Data[4095], colorMap[2047]);
			color = Color.FromArgb(colorMap.Data[4095]);
			Assert.IsTrue(255 == color.R && color.R == color.G && color.G == color.B);
		}
	}
}

#endif