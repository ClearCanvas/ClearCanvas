#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Codec.Tests;
using ClearCanvas.Dicom.Iod.Modules;
using NUnit.Framework;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.Dicom.Codec.Rle.Tests
{
	//TODO: this test won't work anymore because the codec registry uses extensions.
    [TestFixture]
    public class CodecTest : AbstractCodecTest
    {
		private class StubExtensionFactory : IExtensionFactory
		{
			#region IExtensionFactory Members

			public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
			{
				if (extensionPoint.GetType() == typeof(DicomCodecFactoryExtensionPoint))
					return new object[]{ new DicomRleCodecFactory() };

				return new object[0];
			}

			public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
			{
				return new ExtensionInfo[0];
			}

			#endregion
		}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new StubExtensionFactory());
			//HACK: for now, call the static constructor again, so it will repopulate the dictionary
			ConstructorInfo staticConstructor = typeof(DicomCodecRegistry).TypeInitializer;
			staticConstructor.Invoke(null, null);
		}

        [Test]
        public void RleTest()
        {
            DicomFile file = new DicomFile("RleCodecTest.dcm");

            SetupMR(file.DataSet);

            SetupMetaInfo(file);

            RleTest(file);

            file = new DicomFile("MultiframeRleCodecTest.dcm");

            SetupMultiframeXA(file.DataSet, 511, 511, 5);

            RleTest(file);


            file = new DicomFile("MultiframeRleCodecTest.dcm");

            SetupMultiframeXA(file.DataSet, 63, 63, 1);

            RleTest(file);

            file = new DicomFile("MultiframeRleCodecTest.dcm");

            SetupMultiframeXA(file.DataSet, 1024, 1024, 3);

            RleTest(file);

            file = new DicomFile("MultiframeRleCodecTest.dcm");

            SetupMultiframeXA(file.DataSet, 512, 512, 2);

            RleTest(file);
        }

        [Test]
        public void RleUNVRTest()
        {
            DicomFile file = new DicomFile("RleCodecUNTest.dcm");

            SetupMRWithUNVR(file.DataSet);

            SetupMetaInfo(file);

            file.Save();

            DicomFile newFile = new DicomFile(file.Filename);

            newFile.Load();


            RleTest(newFile);
        }


        [Test]
        public void RleOverlayTest()
        {
            DicomFile file = new DicomFile("RleCodecOverlayTest.dcm");

            SetupMRWithOverlay(file.DataSet);

            SetupMetaInfo(file);

            // Save a copy
            file.Save();


            // Load the file into new DicomFile object
            DicomFile newFile = new DicomFile(file.Filename);

            newFile.Load();

            OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(newFile.DataSet);
            OverlayPlane overlay = overlayIod[0];
            
            // SHould be no OverlayData tag
            Assert.IsNull(overlay.OverlayData, "Overlay should be in pixel data");

            // Overlay should be extracted out of pixel data here
            newFile.ChangeTransferSyntax(TransferSyntax.RleLossless);

            Assert.IsNotNull(overlay.OverlayData,"Overlay Data is not null");

            newFile.Save();

            // Load a new copy
            newFile = new DicomFile(file.Filename);

            newFile.Load();

            newFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

            newFile.Filename = "Output" + file.Filename;
            newFile.Save();

            List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();
            bool compare = file.DataSet.Equals(newFile.DataSet, ref results);

            // Shouldn't be the same, OverlayData tag should have been added
            Assert.IsFalse(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);                      
        }

        public void RleTest(DicomFile file)
        {
            // Make a copy of the source format
        	DicomAttributeCollection originalDataSet = file.DataSet.Copy();
            DicomAttributeCollection originalMetaInfo = file.MetaInfo.Copy();
            DicomFile originalFile = new DicomFile("", originalMetaInfo, originalDataSet);

            file.ChangeTransferSyntax(TransferSyntax.RleLossless);

            file.Save();

            DicomFile newFile = new DicomFile(file.Filename);

            newFile.Load();

            newFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

            newFile.Filename = "Output" + file.Filename;
            newFile.Save();

            List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();
            bool compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
        }

        [Test]
        public void PartialFrameTest()
        {
            DicomFile file = new DicomFile("RlePartialFrameTest.dcm");

            SetupMultiframeXA(file.DataSet, 511, 511, 7);

            file.ChangeTransferSyntax(TransferSyntax.RleLossless);

            file.Save();

            DicomFile newFile = new DicomFile(file.Filename);

            newFile.Load(DicomReadOptions.StorePixelDataReferences);

            DicomPixelData pd;

            if (!newFile.TransferSyntax.Encapsulated)
                pd = new DicomUncompressedPixelData(newFile);
            else if (newFile.TransferSyntax.Equals(TransferSyntax.RleLossless))
                pd = new DicomCompressedPixelData(newFile);
            else
                throw new DicomCodecException("Unsupported transfer syntax: " + newFile.TransferSyntax);

            for (int i=0; i< pd.NumberOfFrames; i++)
            {
                pd.GetFrame(i);
            }
        }

		[Test]
		public void LosslessMonochromeCodecTest()
		{
			DicomFile file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(256, 255, "MONOCHROME2", 16, 16, false, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(256, 255, "MONOCHROME2", 16, 16, true, 1);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 5);
			LosslessImageTest(TransferSyntax.RleLossless, file);

			file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 5);
			LosslessImageTest(TransferSyntax.RleLossless, file);

		}
		[Test]
		public void LosslessColorCodecTest()
		{
			TransferSyntax syntax = TransferSyntax.RleLossless;
			DicomFile file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
			LosslessImageTest(syntax, file);

			file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
			LosslessImageTest(syntax, file);

			file = CreateFile(256, 255, "RGB", 8, 8, false, 1);
			LosslessImageTest(syntax, file);

			file = CreateFile(256, 256, "RGB", 8, 8, false, 5);
			LosslessImageTest(syntax, file);

			file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
			LosslessImageTest(syntax, file);

			file = CreateFile(512, 512, "YBR_FULL", 8, 8, false, 1);
			LosslessImageTest(syntax, file);

			file = CreateFile(255, 255, "YBR_FULL", 8, 8, false, 5);
			LosslessImageTest(syntax, file);

		}
	}
}

#endif