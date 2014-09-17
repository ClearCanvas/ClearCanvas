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
	internal class DataModalityLutTests
	{
		// ModalityLutLinear is tested in its own test class

		[Test]
		public void TestSimpleDataModalityLut()
		{
			const int bitsStored = 8;
			const bool isSigned = false;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 10;

			var modalityLutLinear = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			var simpleDataModalityLut =
				new SimpleDataModalityLut(modalityLutLinear.MinInputValue, modalityLutLinear.Data, modalityLutLinear.MinOutputValue, modalityLutLinear.MaxOutputValue, modalityLutLinear.GetKey(), modalityLutLinear.GetDescription());

			Assert.AreEqual(modalityLutLinear.MinInputValue, simpleDataModalityLut.MinInputValue, "MinInputValue");
			Assert.AreEqual(modalityLutLinear.MaxInputValue, simpleDataModalityLut.MaxInputValue, "MaxInputValue");
			Assert.AreEqual(modalityLutLinear.MinOutputValue, simpleDataModalityLut.MinOutputValue, "MinOutputValue");
			Assert.AreEqual(modalityLutLinear.MaxOutputValue, simpleDataModalityLut.MaxOutputValue, "MaxOutputValue");
			Assert.AreEqual(modalityLutLinear.GetKey(), simpleDataModalityLut.GetKey(), "Key");
			Assert.AreEqual(modalityLutLinear.GetDescription(), simpleDataModalityLut.GetDescription(), "Description");
			Assert.AreEqual(modalityLutLinear.Length, simpleDataModalityLut.Length, "Length");

			for (var n = -1; n < 256; ++n)
				Assert.AreEqual(modalityLutLinear[n], simpleDataModalityLut[n], "Value @{0}", n);

			simpleDataModalityLut.AssertLookupValues(-1, 256);
		}
	}
}

#endif