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

#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class NormalizationLutLinearTests
	{
		[Test]
		public void TestTrivialRescale()
		{
			const double rescaleSlope = 1;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			//The composed LUT outputs "Presentation Values", so just trick it into being "identity".
			var output = composer.GetOutputLut(0, 4095);
			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestNormalRescale1()
		{
			const double rescaleSlope = 1.9;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var output = composer.GetOutputLut(0, 4095);

			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestNormalRescale2()
		{
			const double rescaleSlope = 0.9;
			const double rescaleIntercept = 406;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var output = composer.GetOutputLut(0, 4095);

			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestNormalRescale3()
		{
			const double rescaleSlope = 1;
			const double rescaleIntercept = -625;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var output = composer.GetOutputLut(0, 4095);

			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestSubnormalRescale1()
		{
			const double rescaleSlope = 1.5e-9;
			const double rescaleIntercept = 0;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var output = composer.GetOutputLut(0, 4095);

			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestSubnormalRescale2()
		{
			const double rescaleSlope = 1.5e-9;
			const double rescaleIntercept = 351;

			const int bitsStored = 12;
			const bool signed = false;

			var composer = new LutComposer(bitsStored, signed);
			composer.ModalityLut = new ModalityLutLinear(bitsStored, signed, rescaleSlope, rescaleIntercept);
			composer.NormalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var output = composer.GetOutputLut(0, 4095);

			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(101, output[101]);
			Assert.AreEqual(2047, output[2047]);
			Assert.AreEqual(2048, output[2048]);
			Assert.AreEqual(3993, output[3993]);
			Assert.AreEqual(4095, output[4095]);
		}

		[Test]
		public void TestLookupValues()
		{
			const double rescaleSlope = 1.5e-9;
			const double rescaleIntercept = 351;

			var lut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);

			lut.AssertLookupValues(-65536, 65536);
		}
	}
}

#endif