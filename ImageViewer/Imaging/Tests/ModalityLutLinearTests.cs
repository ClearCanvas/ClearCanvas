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

using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class ModalityLutLinearTests
	{
		[Test]
		public void Unsigned1()
		{
			const int bitsStored = 1;
			const bool isSigned = false;
			const double rescaleSlope = 1;
			const double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(2, lut.Length);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(101, lut[1]);
		}

		[Test]
		public void Signed1()
		{
			const int bitsStored = 1;
			const bool isSigned = true;
			const double rescaleSlope = 1;
			const double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(2, lut.Length);
			Assert.AreEqual(99, lut[-1]);
			Assert.AreEqual(100, lut[0]);
		}

		[Test]
		public void Unsigned12()
		{
			const int bitsStored = 12;
			const bool isSigned = false;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(4096, lut.Length);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(1123.5, lut[2047]);
			Assert.AreEqual(2147.5, lut[4095]);
		}

		[Test]
		public void Signed12()
		{
			const int bitsStored = 12;
			const bool isSigned = true;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(4096, lut.Length);
			Assert.AreEqual(-924, lut[-2048]);
			Assert.AreEqual(100, lut[0]);
			Assert.AreEqual(1123.5, lut[2047]);
		}

		[Test]
		[ExpectedException(typeof (DicomDataException))]
		public void BitsStoredInvalid()
		{
			const int bitsStored = 0;
			const bool isSigned = false;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.IsNull(lut);
		}

		[Test]
		public void TestLookupValues()
		{
			var lut = new ModalityLutLinear(15, true, 1.2345, -67.890);

			lut.AssertLookupValues(short.MinValue - 1, short.MaxValue + 1);
		}
	}
}

#endif