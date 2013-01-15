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
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class ModalityLUTLinearTest
	{
		public ModalityLUTLinearTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void Unsigned1()
		{
			int bitsStored = 1;
			bool isSigned = false;
			double rescaleSlope = 1;
			double rescaleIntercept = 100;

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
			int bitsStored = 1;
			bool isSigned = true;
			double rescaleSlope = 1;
			double rescaleIntercept = 100;

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
			int bitsStored = 12;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

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
			int bitsStored = 12;
			bool isSigned = true;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

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
		[ExpectedException(typeof(DicomDataException))]
		public void BitsStoredInvalid()
		{
			int bitsStored = 0;
			bool isSigned = false;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 100;

			ModalityLutLinear lut = new ModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);
		}
	}
}

#endif