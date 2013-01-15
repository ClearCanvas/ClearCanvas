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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class CompositeLUTTest
	{
		private const double _tolerance = 1e-5;
		private LutFactory _lutFactory;

		public CompositeLUTTest()
		{
		}

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
			int bitsStored = 8;
			bool isSigned = false;
			double windowWidth = 128;
			double windowLevel = 74;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(255, modalityLUT.MaxInputValue);
			Assert.AreEqual(10, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(137.5, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(10, modalityLUT[0], _tolerance);
			Assert.AreEqual(137.5, modalityLUT[255], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(10, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(137.5, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(10, voiLUT.MinOutputValue);
			Assert.AreEqual(138, voiLUT.MaxOutputValue);
			Assert.AreEqual(10, voiLUT[10]);
			Assert.AreEqual(138, voiLUT[138]);
			Assert.AreEqual(73, voiLUT[73]);

			Assert.AreEqual(0, lutComposer.MinInputValue);
			Assert.AreEqual(255, lutComposer.MaxInputValue);
			Assert.AreEqual(10, lutComposer.MinOutputValue);
			Assert.AreEqual(138, lutComposer.MaxOutputValue);

			Assert.AreEqual(10, lutComposer[0]);
			Assert.AreEqual(138, lutComposer[255]);
			Assert.AreEqual(74, lutComposer[127]);
		}

		[Test]
		public void ComposeUnsigned12()
		{
			int bitsStored = 12;
			bool isSigned = false;
			double windowWidth = 2800;
			double windowLevel = 1600;
			double rescaleSlope = 0.683760684;
			double rescaleIntercept = 200;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(4095, modalityLUT.MaxInputValue);
			Assert.AreEqual(200, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(3000, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(200, modalityLUT[0], _tolerance);
			Assert.AreEqual(3000, modalityLUT[4095], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(200, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(3000, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(200, voiLUT.MinOutputValue);
			Assert.AreEqual(3000, voiLUT.MaxOutputValue);
			Assert.AreEqual(200, voiLUT[200]);
			Assert.AreEqual(3000, voiLUT[3000]);
			Assert.AreEqual(1601, voiLUT[1600]);

			Assert.AreEqual(200, lutComposer.Data[0]);
			Assert.AreEqual(3000, lutComposer.Data[4095]);
			Assert.AreEqual(1601, lutComposer.Data[2048]);
		}

		[Test]
		public void ComposeUnsigned16()
		{
			int bitsStored = 16;
			bool isSigned = false;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(0, modalityLUT.MinInputValue);
			Assert.AreEqual(65535, modalityLUT.MaxInputValue);
			Assert.AreEqual(-1024, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(64511, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(-1024, modalityLUT[0], _tolerance);
			Assert.AreEqual(64511, modalityLUT[65535], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear(
				modalityLUT.MinOutputValue,
				modalityLUT.MaxOutputValue);

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(-1024, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(64511, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(-1024, voiLUT.MinOutputValue);
			Assert.AreEqual(64511, voiLUT.MaxOutputValue);

			// Left Window
			Assert.AreEqual(-1024, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(64511, voiLUT[215]);
			// Window center
			// 31837 is correct according to DICOM: See PS 3.3 C.11.2.1.2 for the calculation.
			// Although you might think it should be 31744 (65535/2 - 1024), it is not.
			Assert.AreEqual(31837, voiLUT[40]);
			
			Assert.AreEqual(-1024, lutComposer.Data[0]);
			Assert.AreEqual(64511, lutComposer.Data[65535]);
			Assert.AreEqual(31837, lutComposer.Data[1064]);
		}

		[Test]
		public void ComposeSigned8()
		{
			// Use case:  Window width is 1
			int bitsStored = 8;
			bool isSigned = true;
			double windowWidth = 1;
			double windowLevel = 0;
			double rescaleSlope = 0.5;
			double rescaleIntercept = 10;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-128, modalityLUT.MinInputValue);
			Assert.AreEqual(127, modalityLUT.MaxInputValue);
			Assert.AreEqual(-54, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(73.5, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(-54, modalityLUT[-128], _tolerance);
			Assert.AreEqual(73.5, modalityLUT[127], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(-54, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(73.5, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(-54, voiLUT.MinOutputValue);
			Assert.AreEqual(74, voiLUT.MaxOutputValue);
			Assert.AreEqual(-54, voiLUT[-1]);
			Assert.AreEqual(74, voiLUT[0]);

			Assert.AreEqual(-54, lutComposer.Data[0]);
			Assert.AreEqual(-54, lutComposer.Data[106]);
			Assert.AreEqual(-54, lutComposer.Data[107]); // stored pixel -21 which, if you don't round off the modality LUT, is actually -0.5 and therefore has a VOI value of -54
			Assert.AreEqual(74, lutComposer.Data[108]);
		}

		[Test]
		public void ComposeSigned12()
		{
			int bitsStored = 12;
			bool isSigned = true;
			double windowWidth = 16384;
			double windowLevel = 4096;
			double rescaleSlope = 1.0;
			double rescaleIntercept = 0;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-2048, modalityLUT.MinInputValue);
			Assert.AreEqual(2047, modalityLUT.MaxInputValue);
			Assert.AreEqual(-2048, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(2047, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(-2048, modalityLUT[-2048], _tolerance);
			Assert.AreEqual(2047, modalityLUT[2047], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(-2048, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(2047, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(-2048, voiLUT.MinOutputValue);
			Assert.AreEqual(2047, voiLUT.MaxOutputValue);
			Assert.AreEqual(-1536, voiLUT[-2047]);
			Assert.AreEqual(-1024, voiLUT[0]);
			Assert.AreEqual(-513, voiLUT[2047]);

			//This test is a little different from the others, it tests the output using a grayscale color map.
			var colorMap = _lutFactory.GetGrayscaleColorMap();
			colorMap.MaxInputValue = lutComposer.MaxOutputValue;
			colorMap.MinInputValue = lutComposer.MinOutputValue;
			Assert.AreEqual(31, 0x000000ff & colorMap[lutComposer.Data[0]]);
			Assert.AreEqual(63, 0x000000ff & colorMap[lutComposer.Data[2048]]);
			Assert.AreEqual(95, 0x000000ff & colorMap[lutComposer.Data[4095]]);
		}

		[Test]
		public void ComposeSigned16()
		{
			int bitsStored = 16;
			bool isSigned = true;
			double windowWidth = 350;
			double windowLevel = 40;
			double rescaleSlope = 1;
			double rescaleIntercept = -1024;

			IModalityLut modalityLUT = _lutFactory.GetModalityLutLinear(
				bitsStored, 
				isSigned, 
				rescaleSlope, 
				rescaleIntercept);

			Assert.AreEqual(-32768, modalityLUT.MinInputValue);
			Assert.AreEqual(32767, modalityLUT.MaxInputValue);
			Assert.AreEqual(-33792, modalityLUT.MinOutputValue, _tolerance);
			Assert.AreEqual(31743, modalityLUT.MaxOutputValue, _tolerance);
			Assert.AreEqual(-33792, modalityLUT[-32768], _tolerance);
			Assert.AreEqual(31743, modalityLUT[32767], _tolerance);

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(bitsStored, isSigned);
			lutComposer.ModalityLut = modalityLUT;
			lutComposer.VoiLut = voiLUT;

			Assert.AreEqual(-33792, voiLUT.MinInputValue, _tolerance);
			Assert.AreEqual(31743, voiLUT.MaxInputValue, _tolerance);
			Assert.AreEqual(-33792, voiLUT.MinOutputValue);
			Assert.AreEqual(31743, voiLUT.MaxOutputValue);

			// Left Window
			Assert.AreEqual(-33792, voiLUT[-135]);
			// Right Window
			Assert.AreEqual(31743, voiLUT[215]);
			// Window center
			Assert.AreEqual(-931, voiLUT[40]);
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
			Assert.AreEqual(-16384, voiLut.MinOutputValue);
			Assert.AreEqual(16383, voiLut.MaxOutputValue);

			Assert.AreEqual(-13543, voiLut[-13543]);
			Assert.AreEqual(12564, voiLut[12564]);
			Assert.AreEqual(-4074, voiLut[-4074]);

			Assert.AreEqual(-13543, lutComposer.Data[-13543 + 16384]);
			Assert.AreEqual(12564, lutComposer.Data[12564 + 16384]);
			Assert.AreEqual(-4074, lutComposer.Data[-4074 + 16384]);
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
			Assert.AreEqual(0, voiLut.MinOutputValue);
			Assert.AreEqual(8191, voiLut.MaxOutputValue);

			Assert.AreEqual(1543, voiLut[1543]);
			Assert.AreEqual(5164, voiLut[5164]);
			Assert.AreEqual(7074, voiLut[7074]);

			Assert.AreEqual(1543, lutComposer.Data[1543]);
			Assert.AreEqual(5164, lutComposer.Data[5164]);
			Assert.AreEqual(7074, lutComposer.Data[7074]);
		}

		[Test]
		public void ComposeSingleLUT()
		{
			double windowWidth = 350;
			double windowLevel = 40;

			BasicVoiLutLinear voiLUT = new BasicVoiLutLinear();
			voiLUT.MinInputValue = 0;
			voiLUT.MaxInputValue = 4095;

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowLevel;

			LutComposer lutComposer = new LutComposer(0, 4095);
			lutComposer.VoiLut = voiLUT;
			int[] data = lutComposer.Data;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void NoLUTsAdded()
		{
			LutComposer lutComposer = new LutComposer();
			int[] data = lutComposer.Data;
		}
	}
}

#endif