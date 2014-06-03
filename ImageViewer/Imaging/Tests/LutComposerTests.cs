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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class LutComposerTests
	{
		private const double _tolerance = 1e-5;
		private LutFactory _lutFactory;

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
			MemoryManager.Enabled = false;
			_lutFactory = LutFactory.Create();
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			_lutFactory.Dispose();
		}

		[Test]
		public void ComposeUnsigned8()
		{
			const int bitsStored = 8;
			const bool isSigned = false;
			const double windowWidth = 128;
			const double windowLevel = 74;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 10;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(0, modalityLut.MinInputValue);
			Assert.AreEqual(255, modalityLut.MaxInputValue);
			Assert.AreEqual(10, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(137.5, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(10, modalityLut[0], _tolerance);
			Assert.AreEqual(137.5, modalityLut[255], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(10, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(137.5, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(10, voiLut.MinOutputValue);
			Assert.AreEqual(137.5, voiLut.MaxOutputValue);
			Assert.AreEqual(10, voiLut[10], _tolerance);
			Assert.AreEqual(137.5, voiLut[138], _tolerance);
			Assert.AreEqual(73.24803, voiLut[73], _tolerance);

			Assert.AreEqual(0, lutComposer.MinInputValue);
			Assert.AreEqual(255, lutComposer.MaxInputValue);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));

			Assert.AreEqual(10, output.MinOutputValue);
			Assert.AreEqual(138, output.MaxOutputValue);

			Assert.AreEqual(10, output[0], _tolerance);
			Assert.AreEqual(138, output[255], _tolerance);
			Assert.AreEqual(74, output[127], _tolerance);
		}

		[Test]
		public void ComposeUnsigned12()
		{
			const int bitsStored = 12;
			const bool isSigned = false;
			const double windowWidth = 2800;
			const double windowLevel = 1600;
			const double rescaleSlope = 0.683760684;
			const double rescaleIntercept = 200;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(0, modalityLut.MinInputValue);
			Assert.AreEqual(4095, modalityLut.MaxInputValue);
			Assert.AreEqual(200, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(3000, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(200, modalityLut[0], _tolerance);
			Assert.AreEqual(3000, modalityLut[4095], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(200, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(3000, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(200, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(3000, voiLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(200, voiLut[200], _tolerance);
			Assert.AreEqual(3000, voiLut[3000], _tolerance);
			Assert.AreEqual(1600.50018, voiLut[1600], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));

			Assert.AreEqual(200, output.Data[0]);
			Assert.AreEqual(3000, output.Data[4095]);
			Assert.AreEqual(1601, output.Data[2048]);
		}

		[Test]
		public void ComposeUnsigned16()
		{
			const int bitsStored = 16;
			const bool isSigned = false;
			const double windowWidth = 350;
			const double windowLevel = 40;
			const double rescaleSlope = 1;
			const double rescaleIntercept = -1024;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(0, modalityLut.MinInputValue);
			Assert.AreEqual(65535, modalityLut.MaxInputValue);
			Assert.AreEqual(-1024, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(64511, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-1024, modalityLut[0], _tolerance);
			Assert.AreEqual(64511, modalityLut[65535], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear(
				modalityLut.MinOutputValue,
				modalityLut.MaxOutputValue);

			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(-1024, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(64511, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-1024, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(64511, voiLut.MaxOutputValue, _tolerance);

			// Left Window
			Assert.AreEqual(-1024, voiLut[-135], _tolerance);

			// Right Window
			Assert.AreEqual(64511, voiLut[215], _tolerance);

			// Window center
			// 31837 is correct according to DICOM: See PS 3.3 C.11.2.1.2 for the calculation.
			// Although you might think it should be 31744 (65535/2 - 1024), it is not.
			Assert.AreEqual(31837.38968, voiLut[40], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));

			Assert.AreEqual(-1024, output.Data[0]);
			Assert.AreEqual(64511, output.Data[65535]);
			Assert.AreEqual(31837, output.Data[1064]);
		}

		[Test]
		public void ComposeSigned8()
		{
			// Use case:  Window width is 1
			const int bitsStored = 8;
			const bool isSigned = true;
			const double windowWidth = 1;
			const double windowLevel = 0;
			const double rescaleSlope = 0.5;
			const double rescaleIntercept = 10;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(-128, modalityLut.MinInputValue);
			Assert.AreEqual(127, modalityLut.MaxInputValue);
			Assert.AreEqual(-54, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(73.5, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-54, modalityLut[-128], _tolerance);
			Assert.AreEqual(73.5, modalityLut[127], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(-54, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(73.5, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-54, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(73.5, voiLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-54, voiLut[-1], _tolerance);
			Assert.AreEqual(73.5, voiLut[0], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));

			Assert.AreEqual(-54, output.Data[0]);
			Assert.AreEqual(-54, output.Data[106]);
			Assert.AreEqual(-54, output.Data[107]); // stored pixel -21 which, if you don't round off the modality LUT, is actually -0.5 and therefore has a VOI value of -54
			Assert.AreEqual(74, output.Data[108]);
		}

		[Test]
		public void ComposeSigned12()
		{
			const int bitsStored = 12;
			const bool isSigned = true;
			const double windowWidth = 16384;
			const double windowLevel = 4096;
			const double rescaleSlope = 1.0;
			const double rescaleIntercept = 0;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(-2048, modalityLut.MinInputValue);
			Assert.AreEqual(2047, modalityLut.MaxInputValue);
			Assert.AreEqual(-2048, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(2047, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-2048, modalityLut[-2048], _tolerance);
			Assert.AreEqual(2047, modalityLut[2047], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(-2048, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(2047, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-2048, voiLut.MinOutputValue);
			Assert.AreEqual(2047, voiLut.MaxOutputValue);

			Assert.AreEqual(-1535.84380, voiLut[-2047], _tolerance);
			Assert.AreEqual(-1024.18751, voiLut[0], _tolerance);
			Assert.AreEqual(-512.53122, voiLut[2047], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));

			Assert.AreEqual(-1536, output[-2047]);
			Assert.AreEqual(-1024, output[0]);
			Assert.AreEqual(-513, output[2047]);

			Assert.AreEqual(voiLut.MinOutputValue, output.MinInputValue);
			Assert.AreEqual(voiLut.MaxOutputValue, output.MaxInputValue);
			Assert.AreEqual(voiLut.MinOutputValue, output.MinOutputValue);
			Assert.AreEqual(voiLut.MaxOutputValue, output.MaxOutputValue);

			//This test is a little different from the others, it tests the output using a grayscale color map.
			var colorMap = _lutFactory.GetGrayscaleColorMap();
			colorMap.MaxInputValue = output.MaxOutputValue;
			colorMap.MinInputValue = output.MinOutputValue;

			Assert.AreEqual(32, 0x000000ff & colorMap[output.Data[0]]);
			Assert.AreEqual(64, 0x000000ff & colorMap[output.Data[2048]]);
			Assert.AreEqual(96, 0x000000ff & colorMap[output.Data[4095]]);
		}

		[Test]
		public void ComposeSigned16()
		{
			const int bitsStored = 16;
			const bool isSigned = true;
			const double windowWidth = 350;
			const double windowLevel = 40;
			const double rescaleSlope = 1;
			const double rescaleIntercept = -1024;

			IModalityLut modalityLut = _lutFactory.GetModalityLutLinear(
				bitsStored,
				isSigned,
				rescaleSlope,
				rescaleIntercept);

			Assert.AreEqual(-32768, modalityLut.MinInputValue);
			Assert.AreEqual(32767, modalityLut.MaxInputValue);
			Assert.AreEqual(-33792, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(31743, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-33792, modalityLut[-32768], _tolerance);
			Assert.AreEqual(31743, modalityLut[32767], _tolerance);

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLut;
			lutComposer.VoiLut = voiLut;

			Assert.AreEqual(-33792, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(31743, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-33792, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(31743, voiLut.MaxOutputValue, _tolerance);

			// Left Window
			Assert.AreEqual(-33792, voiLut[-135], _tolerance);
			// Right Window
			Assert.AreEqual(31743, voiLut[215], _tolerance);
			// Window center
			Assert.AreEqual(-930.610315, voiLut[40], _tolerance);
		}

		[Test]
		public void ComposeSignedSubnormalModalityLut()
		{
			const int bitsStored = 15;
			const bool isSigned = true;
			const double windowWidth = 32768;
			const double windowLevel = 0;
			const double rescaleSlope = 2.7182818284590452353602874713527e-6;
			const double rescaleIntercept = 0;

			var modalityLut = _lutFactory.GetModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
			var normalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var voiLut = new BasicVoiLutLinear {WindowWidth = windowWidth, WindowCenter = windowLevel};
			var lutComposer = new LutComposer(bitsStored, isSigned) {ModalityLut = modalityLut, VoiLut = voiLut, NormalizationLut = normalizationLut};

			Assert.AreEqual(-16384, modalityLut.MinInputValue);
			Assert.AreEqual(16383, modalityLut.MaxInputValue);
			Assert.AreEqual(-0.044536329477473, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(0.044533611195644536, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-0.044536329477473, modalityLut[-16384], _tolerance);
			Assert.AreEqual(0.044533611195644536, modalityLut[16383], _tolerance);

			Assert.AreEqual(-0.044536329477473, normalizationLut.MinInputValue, _tolerance);
			Assert.AreEqual(0.044533611195644536, normalizationLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-16384, normalizationLut.MinOutputValue, _tolerance);
			Assert.AreEqual(16383, normalizationLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-16384, normalizationLut[-0.044536329477473], _tolerance);
			Assert.AreEqual(16383, normalizationLut[0.044533611195644536], _tolerance);

			Assert.AreEqual(-16384, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(16383, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(-16384, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(16383, voiLut.MaxOutputValue, _tolerance);

			Assert.AreEqual(-13543, voiLut[-13543], _tolerance);
			Assert.AreEqual(12564, voiLut[12564], _tolerance);
			Assert.AreEqual(-4074, voiLut[-4074], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));
			Assert.AreEqual(-13543, output.Data[-13543 + 16384]);
			Assert.AreEqual(12564, output.Data[12564 + 16384]);
			Assert.AreEqual(-4074, output.Data[-4074 + 16384]);
		}

		[Test]
		public void ComposeUnsignedSubnormalModalityLut()
		{
			const int bitsStored = 13;
			const bool isSigned = false;
			const double windowWidth = 8192;
			const double windowLevel = 4096;
			const double rescaleSlope = 3.1415926535897932384626433832795e-6;
			const double rescaleIntercept = -10;

			var modalityLut = _lutFactory.GetModalityLutLinear(bitsStored, isSigned, rescaleSlope, rescaleIntercept);
			var normalizationLut = new NormalizationLutLinear(rescaleSlope, rescaleIntercept);
			var voiLut = new BasicVoiLutLinear {WindowWidth = windowWidth, WindowCenter = windowLevel};
			var lutComposer = new LutComposer(bitsStored, isSigned) {ModalityLut = modalityLut, VoiLut = voiLut, NormalizationLut = normalizationLut};

			Assert.AreEqual(0, modalityLut.MinInputValue);
			Assert.AreEqual(8191, modalityLut.MaxInputValue);
			Assert.AreEqual(-10, modalityLut.MinOutputValue, _tolerance);
			Assert.AreEqual(-9.9742672145744464, modalityLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(-10, modalityLut[0], _tolerance);
			Assert.AreEqual(-9.9742672145744464, modalityLut[8191], _tolerance);

			Assert.AreEqual(-10, normalizationLut.MinInputValue, _tolerance);
			Assert.AreEqual(-9.9742672145744464, normalizationLut.MaxInputValue, _tolerance);
			Assert.AreEqual(0, normalizationLut.MinOutputValue, _tolerance);
			Assert.AreEqual(8191, normalizationLut.MaxOutputValue, _tolerance);
			Assert.AreEqual(0, normalizationLut[-10], _tolerance);
			Assert.AreEqual(8191, normalizationLut[-9.9742672145744464], _tolerance);

			Assert.AreEqual(0, voiLut.MinInputValue, _tolerance);
			Assert.AreEqual(8191, voiLut.MaxInputValue, _tolerance);
			Assert.AreEqual(0, voiLut.MinOutputValue, _tolerance);
			Assert.AreEqual(8191, voiLut.MaxOutputValue, _tolerance);

			Assert.AreEqual(1543, voiLut[1543], _tolerance);
			Assert.AreEqual(5164, voiLut[5164], _tolerance);
			Assert.AreEqual(7074, voiLut[7074], _tolerance);

			//For this, we want the output range to be the same as the VOI.
			var output = lutComposer.GetOutputLut((int) Math.Round(voiLut.MinOutputValue), (int) Math.Round(voiLut.MaxOutputValue));
			Assert.AreEqual(1543, output.Data[1543]);
			Assert.AreEqual(5164, output.Data[5164]);
			Assert.AreEqual(7074, output.Data[7074]);
		}

		[Test]
		public void ComposeSingleLut()
		{
			const double windowWidth = 350;
			const double windowLevel = 40;

			BasicVoiLutLinear voiLut = new BasicVoiLutLinear();
			voiLut.MinInputValue = 0;
			voiLut.MaxInputValue = 4095;

			voiLut.WindowWidth = windowWidth;
			voiLut.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(0, 4095);
			lutComposer.VoiLut = voiLut;

			var output = lutComposer.GetOutputLut(0, 255);
			Assert.IsNotNull(output);
		}

		[Test]
		public void TestEquivalentLuts()
		{
			var composer1 = new LutComposer
			                	{
			                		ModalityLut = new ModalityLutLinear(16, true, 1, 0),
			                		VoiLut = new IdentityVoiLinearLut(16, true)
			                	};
			var composer2 = new LutComposer
			                	{
			                		ModalityLut = new ModalityLutLinear(16, false, 1, -32768),
			                		VoiLut = new IdentityVoiLinearLut(16, false)
			                	};

			//Compare 2 composed LUTs whose output should be the same.
			var output1 = composer1.GetOutputLut(0, 255);
			var output2 = composer2.GetOutputLut(0, 255);

			Assert.AreEqual(0, output1[-32768]);
			Assert.AreEqual(128, output1[0]);
			Assert.AreEqual(255, output1[32767]);

			Assert.AreEqual(0, output2[0]);
			Assert.AreEqual(128, output2[32768]);
			Assert.AreEqual(255, output2[65535]);

			Assert.AreEqual(output1.Data.Length, output2.Data.Length);
			for (int i = 0; i < output1.Data.Length; ++i)
				Assert.AreEqual(output1.Data[i], output2.Data[i]);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void NoLutsAdded()
		{
			LutComposer lutComposer = new LutComposer();
			var output = lutComposer.GetOutputLut(0, 255);
			Assert.IsNull(output);
		}
	}
}

#endif