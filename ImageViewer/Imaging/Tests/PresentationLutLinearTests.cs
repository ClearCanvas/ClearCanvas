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

using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class PresentationLutLinearTests
	{
		private const double _tolerance = 1e-5;

		[Test]
		public void TestIdentity()
		{
			var lut = new PresentationLutLinear() as IPresentationLut;
			lut.MinInputValue = 0;
			lut.MaxInputValue = 255;
			lut.MinOutputValue = 0;
			lut.MaxOutputValue = 255;

			Assert.AreEqual(0, lut[-1]);
			for (int i = 0; i < 256; ++i)
				Assert.AreEqual(i, lut[i]);

			Assert.AreEqual(255, lut[256]);
		}

		[Test]
		public void TestIdentityNegative()
		{
			var lut = new PresentationLutLinear() as IPresentationLut;
			lut.MinInputValue = -255;
			lut.MaxInputValue = 255;
			lut.MinOutputValue = -255;
			lut.MaxOutputValue = 255;

			Assert.AreEqual(-255, lut[-256]);
			Assert.AreEqual(-128, lut[-128]);
			Assert.AreEqual(0, lut[0]);
			Assert.AreEqual(128, lut[128]);
			Assert.AreEqual(255, lut[255]);
			Assert.AreEqual(255, lut[256]);
		}

		[Test]
		public void TestShrinkInputRange()
		{
			var lut = new PresentationLutLinear() as IPresentationLut;
			lut.MinInputValue = -32768;
			lut.MaxInputValue = 32767;
			lut.MinOutputValue = 0;
			lut.MaxOutputValue = 255;

			Assert.AreEqual(0, lut[-32768]);
			Assert.AreEqual(128, lut[0]);
			Assert.AreEqual(255, lut[32767]);
		}

		[Test]
		public void TestGrowInputRange()
		{
			var lut = new PresentationLutLinear() as IPresentationLut;
			lut.MinInputValue = -10;
			lut.MaxInputValue = 20;
			lut.MinOutputValue = 0;
			lut.MaxOutputValue = 255;

			Assert.AreEqual(0, lut[-10]);
			Assert.AreEqual(128, lut[5]);
			Assert.AreEqual(255, lut[20]);
		}

		[Test]
		public void TestPipelineUnsigned16()
		{
			var composer = new LutComposer(16, false);
			composer.ModalityLut = new ModalityLutLinear(16, false, 1, 0);
			var voiLut = new BasicVoiLutLinear(65536, 32768);
			composer.VoiLut = voiLut;

			Assert.AreEqual(0, voiLut[0], _tolerance);
			Assert.AreEqual(32768, voiLut[32768], _tolerance);
			Assert.AreEqual(65535, voiLut[65535], _tolerance);

			Assert.AreEqual(0, voiLut[-1], _tolerance);
			Assert.AreEqual(65535, voiLut[65536], _tolerance);

			var output = composer.GetOutputLut(0, byte.MaxValue);
			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(128, output[32768]);
			Assert.AreEqual(255, output[65535]);

			//Make sure the output of the grayscale color map works with all the different "display" output ranges.
			var colorMap = LutFactory.Create().GetGrayscaleColorMap();
			colorMap.MinInputValue = output.MinOutputValue;
			colorMap.MaxInputValue = output.MaxOutputValue;
			Assert.AreEqual(0, 0x000000FF & colorMap[output[0]]);
			Assert.AreEqual(128, 0x000000FF & colorMap[output[32768]]);
			Assert.AreEqual(255, 0x000000FF & colorMap[output[65535]]);

			//10-bit display
			output = composer.GetOutputLut(0, 1023);
			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(512, output[32768]);
			Assert.AreEqual(1023, output[65535]);

			colorMap.MinInputValue = output.MinOutputValue;
			colorMap.MaxInputValue = output.MaxOutputValue;
			Assert.AreEqual(0, 0x000000FF & colorMap[output[0]]);
			Assert.AreEqual(128, 0x000000FF & colorMap[output[32768]]);
			Assert.AreEqual(255, 0x000000FF & colorMap[output[65535]]);

			//Theoretical 12-bit display with signed output
			output = composer.GetOutputLut(-2048, 2047);
			Assert.AreEqual(-2048, output[0]);
			Assert.AreEqual(0, output[32768]);
			Assert.AreEqual(2047, output[65535]);

			colorMap.MinInputValue = output.MinOutputValue;
			colorMap.MaxInputValue = output.MaxOutputValue;
			Assert.AreEqual(0, 0x000000FF & colorMap[output[0]]);
			Assert.AreEqual(128, 0x000000FF & colorMap[output[32768]]);
			Assert.AreEqual(255, 0x000000FF & colorMap[output[65535]]);
		}

		[Test]
		public void TestPipelineSigned12()
		{
			var composer = new LutComposer(12, true);
			composer.ModalityLut = new ModalityLutLinear(12, true, 1, 0);
			var voiLut = new BasicVoiLutLinear(4096, 0);
			composer.VoiLut = voiLut;

			Assert.AreEqual(-2048, voiLut[-2048], _tolerance);
			Assert.AreEqual(0, voiLut[0], _tolerance);
			Assert.AreEqual(2047, voiLut[2047], _tolerance);

			var output = composer.GetOutputLut(0, byte.MaxValue);
			Assert.AreEqual(0, output[-2048]);
			Assert.AreEqual(128, output[0]);
			Assert.AreEqual(255, output[2047]);

			//10-bit display
			output = composer.GetOutputLut(0, 1023);
			Assert.AreEqual(0, output[-2048]);
			Assert.AreEqual(512, output[0]);
			Assert.AreEqual(1023, output[2047]);

			//Theoretical 12-bit display with signed output
			output = composer.GetOutputLut(-2048, 2047);
			Assert.AreEqual(-2048, output[-2048]);
			Assert.AreEqual(0, output[0]);
			Assert.AreEqual(2047, output[2047]);
		}

		[Test]
		public void TestLookupValues()
		{
			var lut = new PresentationLutLinear(0, 255);

			lut.AssertLookupValues(-1, 256);
		}
	}
}

#endif