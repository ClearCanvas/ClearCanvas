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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Codec.Tests
{
	[TestFixture]
	public class CodecTest : AbstractCodecTest
	{
		[Test]
		public void Test()
		{
			DicomFile file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 1);
			file.Filename = "Monochrome1TestPattern.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            file.Save();

			file = CreateFile(256, 256, "MONOCHROME2", 14, 16, true, 1);
			file.Filename = "Monochrome2TestPattern.dcm";
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            file.Save();

			file = CreateFile(256, 256, "RGB", 8, 8, false, 1);
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			file.Filename = "RgbColorTestPattern.dcm";
			file.Save();

			file = CreateFile(256, 256, "YBR_FULL", 8, 8, false, 1);
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			file.Filename = "YbrColorTestPattern.dcm";
			file.Save();

		}
	}
	public class AbstractCodecTest : AbstractTest
	{
        public void SetupMRWithUNVR(DicomAttributeCollection theSet)
        {
            SetupMR(theSet);

            theSet[DicomTags.LossyImageCompressionMethod].SetStringValue("ISO_15444_1");

            ConvertAttributeToUN(theSet, DicomTags.LossyImageCompressionMethod);
        }

        public void SetupMRWithOverlay(DicomAttributeCollection theSet)
        {
            SetupMR(theSet);

            OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(theSet);
            DicomUncompressedPixelData pd = new DicomUncompressedPixelData(theSet);
            OverlayPlane overlay = overlayIod[0];

            // Embedded overlays are retired in dicom, just doing it for testing purposes
            theSet[DicomTags.OverlayBitPosition].SetInt32(0, pd.HighBit + 1);
            overlay.OverlayBitsAllocated = 1;
            overlay.OverlayColumns = pd.ImageWidth;
            overlay.OverlayRows = pd.ImageHeight;
            overlay.OverlayOrigin = new Point(0, 0);
            overlay.OverlayType = OverlayType.R;
        }

	    public static void LosslessImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);
	        saveCopy.Filename = "SCLosslessUncompressed.dcm";
            saveCopy.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);
            newFile.Filename = "SCLosslessUncompressedPostCompression.dcm";
            newFile.Save(DicomWriteOptions.ExplicitLengthSequence);

	        string failureDescription;
	        bool result = Compare(DicomPixelData.CreateFrom(newFile),
	                              DicomPixelData.CreateFrom(saveCopy), out failureDescription);
	        Assert.IsTrue(result, failureDescription);

			List<DicomAttributeComparisonResult> list = new List<DicomAttributeComparisonResult>();
			result = newFile.DataSet.Equals(saveCopy.DataSet, ref list);

			StringBuilder sb = new StringBuilder();
			foreach (DicomAttributeComparisonResult compareResult in list)
				sb.AppendFormat("Comparison Failure: {0}, ", compareResult.Details);

			Assert.IsTrue(result,sb.ToString());
		}

		public static void LosslessImageTestWithConversion(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

            string failureDescription;
            bool result = Compare(DicomPixelData.CreateFrom(newFile),
                                  DicomPixelData.CreateFrom(saveCopy), out failureDescription);
            Assert.IsTrue(result, failureDescription);

			Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));
		}

        public static void LosslessImageTestWithBitsAllocatedConversion(TransferSyntax syntax, DicomFile theFile)
        {
            if (File.Exists(theFile.Filename))
                File.Delete(theFile.Filename);

            DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

            theFile.ChangeTransferSyntax(syntax);

            theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

            DicomFile newFile = new DicomFile(theFile.Filename);

            newFile.Load(DicomReadOptions.Default);

            newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

            string failureDescription;
            var newPd = DicomPixelData.CreateFrom(newFile);
            var oldPd = DicomPixelData.CreateFrom(saveCopy);

            bool result = Compare(newPd, oldPd, out failureDescription);
            Assert.IsFalse(result, failureDescription);

            Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));

            DicomAttributeCollection newDataSet = newFile.DataSet.Copy(true, true, true);
            DicomAttributeCollection oldDataSet = theFile.DataSet.Copy(true, true, true);

            oldDataSet.RemoveAttribute(DicomTags.BitsAllocated);
            newDataSet.RemoveAttribute(DicomTags.BitsAllocated);
            oldDataSet.RemoveAttribute(DicomTags.PixelData);
            newDataSet.RemoveAttribute(DicomTags.PixelData);

            var results = new List<DicomAttributeComparisonResult>();

            bool check = oldDataSet.Equals(newDataSet, ref results);
            Assert.IsTrue(check, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);

            for (int i = 0; i < oldPd.NumberOfFrames; i++)
            {
                var frame = oldPd.GetFrame(i);
                var convertedFrame = DicomUncompressedPixelData.ToggleBitDepth(frame, frame.Length,
                                                                              oldPd.UncompressedFrameSize,
                                                                              oldPd.BitsStored, oldPd.BitsAllocated);
                var newFrame = newPd.GetFrame(i);

                int pixelsVarying = 0;
                decimal totalVariation = 0.0m;
                for (int j = 0; j < convertedFrame.Length; j++)
                    if (convertedFrame[j] != newFrame[j])
                    {
                        pixelsVarying++;
                        totalVariation += Math.Abs(convertedFrame[i] - newFrame[i]);
                    }

                if (pixelsVarying > 0)
                {
                    Assert.Fail(String.Format(
                        "Tag (7fe0,0010) Pixel Data: {0} of {1} pixels varying, average difference: {2}",
                        pixelsVarying, convertedFrame.Length, totalVariation/pixelsVarying));
                }
            }
        }

		public static void LossyImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));

			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.DerivationDescription));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompression));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompressionMethod));
			Assert.IsTrue(newFile.DataSet.Contains(DicomTags.LossyImageCompressionRatio));

			Assert.IsFalse(newFile.DataSet[DicomTags.DerivationDescription].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompression].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionMethod].IsEmpty);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionRatio].IsEmpty);

			Assert.IsFalse(newFile.DataSet[DicomTags.DerivationDescription].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompression].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionMethod].IsNull);
			Assert.IsFalse(newFile.DataSet[DicomTags.LossyImageCompressionRatio].IsNull);

			// Make copies of datasets, delete the tags that don't match, and ensure the same
			DicomAttributeCollection newDataSet = newFile.DataSet.Copy(true, true, true);
			DicomAttributeCollection oldDataSet = theFile.DataSet.Copy(true, true, true);

			oldDataSet.RemoveAttribute(DicomTags.PixelData);
			newDataSet.RemoveAttribute(DicomTags.PixelData);
			oldDataSet.RemoveAttribute(DicomTags.DerivationDescription);
			newDataSet.RemoveAttribute(DicomTags.DerivationDescription);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompression);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompression);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompressionMethod);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompressionMethod);
			oldDataSet.RemoveAttribute(DicomTags.LossyImageCompressionRatio);
			newDataSet.RemoveAttribute(DicomTags.LossyImageCompressionRatio);
			oldDataSet.RemoveAttribute(DicomTags.PhotometricInterpretation);
			newDataSet.RemoveAttribute(DicomTags.PhotometricInterpretation);

			List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();

			bool check = oldDataSet.Equals(newDataSet, ref results);
			Assert.IsTrue(check, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
		}

		public static void ExpectedFailureTest(TransferSyntax syntax, DicomFile theFile)
		{
			try
			{
				theFile.ChangeTransferSyntax(syntax);
			}
			catch (DicomCodecUnsupportedSopException)
			{
				return;
			}

			Assert.IsTrue(false, "Unexpected successful compression of object.");
		}

        public static bool Compare(DicomPixelData pixels1, DicomPixelData pixels2, out string failureDescription)
        {
            failureDescription = string.Empty;
            if (pixels1.BitsAllocated != pixels2.BitsAllocated)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Bits Allocated varies: {0} , {1}", pixels1.BitsAllocated, pixels2.BitsAllocated);
                return false;
            }

            if (pixels1.BitsStored != pixels2.BitsStored)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Bits Stored varies: {0} , {1}", pixels1.BitsStored, pixels2.BitsStored);
                return false;
            }

            if (pixels1.ImageHeight != pixels2.ImageHeight)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Rows varies: {0} , {1}", pixels1.ImageHeight, pixels2.ImageHeight);
                return false;
            }

            if (pixels1.ImageWidth != pixels2.ImageWidth)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Columns varies: {0} , {1}", pixels1.ImageWidth, pixels2.ImageWidth);
                return false;
            }

            if (pixels1.SamplesPerPixel != pixels2.SamplesPerPixel)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Samples per pixel varies: {0} , {1}", pixels1.SamplesPerPixel, pixels2.SamplesPerPixel);
                return false;
            }

            if (pixels1.NumberOfFrames != pixels2.NumberOfFrames)
            {
                failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: Number of frames varies: {0} , {1}", pixels1.NumberOfFrames, pixels2.NumberOfFrames);
                return false;
            }

            long pixelsVarying = 0;
            long totalVariation = 0;

            int pixels = pixels1.ImageHeight * pixels1.ImageWidth * pixels1.SamplesPerPixel;

            if (pixels1.BitsAllocated == 8)
            {
                for (int frame = 0; frame < pixels1.NumberOfFrames; frame++)
                {
                    byte[] pixel1 = pixels1.GetFrame(frame);
                    byte[] pixel2 = pixels2.GetFrame(frame);

                    if (pixels1.HighBit != pixels2.HighBit)
                    {
                        // Justify the pixel, if needed
                        if (DicomUncompressedPixelData.RightAlign(pixel1, pixels1.BitsAllocated, pixels1.BitsStored,
                                                                  pixels1.HighBit))
                        {
                            //pixels1.HighBit = (ushort)(pixels1.BitsStored - 1);
                            DicomUncompressedPixelData.ZeroUnusedBits(pixel1, pixels1.BitsAllocated, pixels1.BitsStored,
                                                                      pixels1.BitsStored - 1);
                        }
                        if (DicomUncompressedPixelData.RightAlign(pixel2, pixels2.BitsAllocated, pixels2.BitsStored,
                                                                  pixels2.HighBit))
                        {
                            //pixels2.HighBit = (ushort)(pixels2.BitsStored - 1);
                            DicomUncompressedPixelData.ZeroUnusedBits(pixel2, pixels2.BitsAllocated, pixels2.BitsStored,
                                                                      pixels2.BitsStored - 1);
                        }
                    }

                    int[] intPixels1 = pixels1.IsSigned
                                           ? Convert8BitSigned(pixel1, pixels, (DicomUncompressedPixelData)pixels1)
                                           : Convert8BitUnsigned(pixel1, pixels, (DicomUncompressedPixelData)pixels1);

                    int[] intPixels2 = pixels2.IsSigned
                                           ? Convert8BitSigned(pixel2, pixels, (DicomUncompressedPixelData)pixels2)
                                           : Convert8BitUnsigned(pixel2, pixels, (DicomUncompressedPixelData)pixels2);


                    for (int i = 0; i < pixels; i++)
                        if (intPixels1[i] != intPixels2[i])
                        {
                            pixelsVarying++;
                            totalVariation += Math.Abs(intPixels1[i] - intPixels2[i]);
                        }
                }
                if (pixelsVarying > 0)
                {
                    failureDescription = String.Format(
                            "Tag (7fe0,0010) Pixel Data: {0} of {1} pixels varying, average difference: {2}",
                            pixelsVarying, pixels * pixels1.NumberOfFrames, totalVariation / pixelsVarying);
                    return false;
                }
            }
            else
            {
                for (int frame = 0; frame < pixels1.NumberOfFrames; frame++)
                {
                    byte[] pixel1 = pixels1.GetFrame(frame);
                    byte[] pixel2 = pixels2.GetFrame(frame);

                    if (pixels1.HighBit != pixels2.HighBit)
                    {
                        // Justify the pixel, if needed
                        if (DicomUncompressedPixelData.RightAlign(pixel1, pixels1.BitsAllocated, pixels1.BitsStored,
                                                              pixels1.HighBit))
                        {
                            //pixels1.HighBit = (ushort) (pixels1.BitsStored - 1);
                            DicomUncompressedPixelData.ZeroUnusedBits(pixel1, pixels1.BitsAllocated, pixels1.BitsStored,
                                                                     pixels1.BitsStored - 1);
                        }
                        if (DicomUncompressedPixelData.RightAlign(pixel2, pixels2.BitsAllocated, pixels2.BitsStored,
                                                              pixels2.HighBit))
                        {
                            //pixels2.HighBit = (ushort)(pixels2.BitsStored - 1);

                            DicomUncompressedPixelData.ZeroUnusedBits(pixel2, pixels2.BitsAllocated, pixels2.BitsStored,
                                                    pixels2.BitsStored - 1);
                        }
                    }


                    int[] intPixels1 = pixels1.IsSigned
                                           ? Convert16BitSigned(pixel1, pixels, (DicomUncompressedPixelData)pixels1)
                                           : Convert16BitUnsigned(pixel1, pixels, (DicomUncompressedPixelData)pixels1);

                    int[] intPixels2 = pixels2.IsSigned
                                           ? Convert16BitSigned(pixel2, pixels, (DicomUncompressedPixelData)pixels2)
                                           : Convert16BitUnsigned(pixel2, pixels, (DicomUncompressedPixelData)pixels2);


                    for (int i = 0; i < pixels; i++)
                        if (intPixels1[i] != intPixels2[i])
                        {
                            pixelsVarying++;
                            totalVariation += Math.Abs(intPixels1[i] - intPixels2[i]);
                        }
                }


                if (pixelsVarying > 0)
                {
                    failureDescription = String.Format("Tag (7fe0,0010) Pixel Data: {0} of {1} pixels varying, average difference: {2}", pixelsVarying, pixels, totalVariation / pixelsVarying);
                    return false;
                }
            }

            return true;
        }

        public static int[] Convert8BitSigned(byte[] byteInputData, int pixels, DicomUncompressedPixelData pd)
        {
            int shiftBits = 32 - pd.BitsStored;
            bool bPixelRescale = !string.IsNullOrEmpty(pd.RescaleSlope) &&
                                                      !string.IsNullOrEmpty(pd.RescaleIntercept)
                                                      &&
                                                      (pd.DecimalRescaleSlope != 1m ||
                                                       pd.DecimalRescaleIntercept != 0m);
            byte pixelMask = 0x00;
            for (int x = 0; x < pd.BitsStored; x++)
                pixelMask = (byte)((pixelMask << 1) | 0x0001);

            int[] intPixels = new int[pixels];
            for (int pixelCount = 0; pixelCount < byteInputData.Length; pixelCount++)
            {
                intPixels[pixelCount] = byteInputData[pixelCount] & pixelMask;

                intPixels[pixelCount] = (((intPixels[pixelCount] << shiftBits)) >> shiftBits);

                if (bPixelRescale)
                    intPixels[pixelCount] = intPixels[pixelCount] * (int)pd.DecimalRescaleSlope +
                                    (int)pd.DecimalRescaleIntercept;
            }
            return intPixels;
        }

        public static int[] Convert8BitUnsigned(byte[] byteInputData, int pixels, DicomUncompressedPixelData pd)
        {
            bool bPixelRescale = !string.IsNullOrEmpty(pd.RescaleSlope) &&
                                                       !string.IsNullOrEmpty(pd.RescaleIntercept)
                                                       &&
                                                       (pd.DecimalRescaleSlope != 1m ||
                                                        pd.DecimalRescaleIntercept != 0m);
            byte pixelMask = 0x00;
            for (int x = 0; x < pd.BitsStored; x++)
                pixelMask = (byte)((pixelMask << 1) | 0x0001);

            int[] intPixels = new int[pixels];

            for (int pixelCount = 0; pixelCount < byteInputData.Length; pixelCount++)
            {
                intPixels[pixelCount] = (byte)(byteInputData[pixelCount] & pixelMask);

                if (bPixelRescale)
                    intPixels[pixelCount] = intPixels[pixelCount] * (int)pd.DecimalRescaleSlope +
                                    (int)pd.DecimalRescaleIntercept;
            }

            return intPixels;
        }

        public static int[] Convert16BitSigned(byte[] byteInputData, int pixels, DicomUncompressedPixelData pd)
        {
            int shiftBits = 32 - pd.BitsStored;

            int[] intPixels = new int[pixels];
            for (int byteArrayCount = 0, pixelCount = 0;
                 byteArrayCount < byteInputData.Length;
                 byteArrayCount += 2, pixelCount++)
            {
                intPixels[pixelCount] = BitConverter.ToInt16(byteInputData, byteArrayCount);

                intPixels[pixelCount] = (intPixels[pixelCount] << shiftBits) >> shiftBits;
            }

            return intPixels;
        }

        public static int[] Convert16BitUnsigned(byte[] byteInputData, int pixels, DicomUncompressedPixelData pd)
        {
            bool bPixelRescale = !string.IsNullOrEmpty(pd.RescaleSlope) &&
                                                   !string.IsNullOrEmpty(pd.RescaleIntercept)
                                                   &&
                                                   (pd.DecimalRescaleSlope != 1m ||
                                                    pd.DecimalRescaleIntercept != 0m);

            ushort pixelMask = 0x0000;
            for (int x = 0; x < pd.BitsStored; x++)
                pixelMask = (ushort)((pixelMask << 1) | 0x0001);
            int[] intPixels = new int[pixels];

            for (int byteArrayCount = 0, pixelCount = 0; byteArrayCount < byteInputData.Length; byteArrayCount += 2, pixelCount++)
            {
                intPixels[pixelCount] = BitConverter.ToUInt16(byteInputData, byteArrayCount);

                intPixels[pixelCount] = (ushort)(intPixels[pixelCount] & pixelMask);
                if (bPixelRescale)
                    intPixels[pixelCount] = intPixels[pixelCount] * (int)pd.DecimalRescaleSlope +
                                    (int)pd.DecimalRescaleIntercept;
            }
            return intPixels;
        }
	}
}

#endif