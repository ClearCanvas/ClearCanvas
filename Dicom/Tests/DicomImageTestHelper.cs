#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Tests
{
	public static class DicomImageTestHelper
	{
		/// <summary>
		/// Creates an in-memory DICOM image
		/// </summary>
		/// <returns></returns>
		internal static DicomFile CreateDicomImage(int rows = 20, int columns = 30, bool bitsAllocated16 = true, bool signed = false, int numberOfFrames = 1, Endian endian = Endian.Little, bool useOB = false)
		{
			var sopClassUid = bitsAllocated16 ? SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid : SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid;
			var sopInstanceUid = DicomUid.GenerateUid().UID;

			var dcf = new DicomFile();
			dcf.MediaStorageSopClassUid = sopClassUid;
			dcf.MediaStorageSopInstanceUid = sopInstanceUid;
			dcf.TransferSyntax = endian == Endian.Little ? TransferSyntax.ExplicitVrLittleEndian : TransferSyntax.ExplicitVrBigEndian;
			dcf.DataSet[DicomTags.PatientId].SetStringValue("TEST");
			dcf.DataSet[DicomTags.PatientsName].SetStringValue("TEST");
			dcf.DataSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.StudyId].SetStringValue("TEST");
			dcf.DataSet[DicomTags.StudyDescription].SetStringValue("TEST");
			dcf.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.SeriesNumber].SetInt32(0, 1);
			dcf.DataSet[DicomTags.SeriesDescription].SetStringValue("TEST");
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(sopClassUid);
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);
			dcf.DataSet[DicomTags.InstanceNumber].SetInt32(0, 1);

			var frameSize = rows*columns*(bitsAllocated16 ? 2 : 1);
			var data = new byte[numberOfFrames*frameSize];
			for (var n = 0; n < numberOfFrames; ++n)
			{
				var value = (byte) (0xFF & (n + 0x80));

				for (var k = 0; k < frameSize; ++k)
					data[n*frameSize + k] = (byte) ((value + k)%255);
			}

			var pixelDataTag = DicomTagDictionary.GetDicomTag(DicomTags.PixelData);
			pixelDataTag = new DicomTag(pixelDataTag.TagValue, pixelDataTag.Name, pixelDataTag.VariableName, useOB ? DicomVr.OBvr : DicomVr.OWvr, pixelDataTag.MultiVR, pixelDataTag.VMLow, pixelDataTag.VMHigh, pixelDataTag.Retired);

			dcf.DataSet[DicomTags.PhotometricInterpretation].SetStringValue(PhotometricInterpretation.Monochrome2.Code);
			dcf.DataSet[DicomTags.SamplesPerPixel].SetInt32(0, 1);
			dcf.DataSet[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);
			dcf.DataSet[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated16 ? 16 : 8);
			dcf.DataSet[DicomTags.BitsStored].SetInt32(0, bitsAllocated16 ? 16 : 8);
			dcf.DataSet[DicomTags.HighBit].SetInt32(0, bitsAllocated16 ? 15 : 7);
			dcf.DataSet[DicomTags.Rows].SetInt32(0, rows);
			dcf.DataSet[DicomTags.Columns].SetInt32(0, columns);
			dcf.DataSet[DicomTags.NumberOfFrames].SetInt32(0, numberOfFrames);
			dcf.DataSet[pixelDataTag].Values = data;

			return dcf;
		}
	}
}

#endif