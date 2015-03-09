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

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Tests
{
	public abstract class AbstractTest
	{
		public void ConvertAttributeToUN(DicomAttributeCollection theSet, uint tag)
		{
			ByteBuffer theData =
				theSet[tag].GetByteBuffer(TransferSyntax.ImplicitVrLittleEndian,
				                          theSet.SpecificCharacterSet);

			DicomTag baseTag = DicomTagDictionary.GetDicomTag(tag);
			DicomTag theTag = new DicomTag(tag,
			                               baseTag.Name, baseTag.VariableName, DicomVr.UNvr, baseTag.MultiVR, baseTag.VMLow, baseTag.VMHigh, baseTag.Retired);

			DicomAttribute unAttrib = DicomVr.UNvr.CreateDicomAttribute(theTag, theData);
			theSet[tag] = unAttrib;
		}

		public void SetupMetaInfo(DicomFile theFile)
		{
			DicomAttributeCollection theSet = theFile.MetaInfo;

			theSet[DicomTags.MediaStorageSopClassUid].SetStringValue(theFile.DataSet[DicomTags.SopClassUid].GetString(0, ""));
			theSet[DicomTags.MediaStorageSopInstanceUid].SetStringValue(theFile.DataSet[DicomTags.SopInstanceUid].GetString(0, ""));
			theFile.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			theSet[DicomTags.ImplementationClassUid].SetStringValue("1.1.1.1.1.11.1");
			theSet[DicomTags.ImplementationVersionName].SetStringValue("CC DICOM 1.0");
		}

		public IList<DicomAttributeCollection> SetupMRSeries(int seriesCount, int instancesPerSeries, string studyInstanceUid)
		{
			List<DicomAttributeCollection> instanceList = new List<DicomAttributeCollection>();

			DicomAttributeCollection baseCollection = new DicomAttributeCollection();

			SetupMR(baseCollection);

			baseCollection[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);

			int acquisitionNumber = 1;
			int instanceNumber = 100;

			float positionX = -61.7564f;
			float positionY = -212.04848f;
			float positionZ = -99.6208f;

			float orientation1 = 0.861f;
			float orientation2 = 0.492f;
			float orientation3 = 0.126f;
			float orientation4 = -0.2965f;

			for (int i = 0; i < seriesCount; i++)
			{
				string seriesInstanceUid = DicomUid.GenerateUid().UID;

				for (int j = 0; j < instancesPerSeries; j++)
				{
					string sopInstanceUid = DicomUid.GenerateUid().UID;
					DicomAttributeCollection instanceCollection = baseCollection.Copy();
					instanceCollection[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);
					instanceCollection[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);

					instanceCollection[DicomTags.SeriesNumber].SetStringValue((i + 1).ToString());
					instanceCollection[DicomTags.SeriesDescription].SetStringValue("Series" + (i + 1).ToString());

					instanceCollection[DicomTags.AcquisitionNumber].SetStringValue(acquisitionNumber++.ToString());
					instanceCollection[DicomTags.InstanceNumber].SetStringValue(instanceNumber++.ToString());

					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(0, positionX);
					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(1, positionY);
					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(2, positionZ);
					positionY += 0.1f;

					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(0, orientation1);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(1, orientation2);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(2, orientation3);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(2, orientation4);
					orientation2 += 0.01f;

					instanceList.Add(instanceCollection);
				}
			}

			return instanceList;
		}

		public void SetupKoForImage(DicomAttributeCollection theSet, DicomAttributeCollection source)
		{
			theSet[DicomTags.SpecificCharacterSet] = source[DicomTags.SpecificCharacterSet].Copy();
			theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
			theSet[DicomTags.InstanceCreationDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.InstanceCreationTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.SopClassUid].SetStringValue(SopClass.KeyObjectSelectionDocumentStorageUid);
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyDate] = source[DicomTags.StudyDate].Copy();
			theSet[DicomTags.StudyTime] = source[DicomTags.StudyTime].Copy();
			theSet[DicomTags.SeriesDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.SeriesTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.ContentDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.ContentTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.AccessionNumber] = source[DicomTags.AccessionNumber].Copy();
			theSet[DicomTags.Modality].SetStringValue("KO");
			theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.ManufacturersModelName].SetNullValue();
			theSet[DicomTags.InstitutionName] = source[DicomTags.InstitutionName].Copy();
			theSet[DicomTags.ReferringPhysiciansName] = source[DicomTags.ReferringPhysiciansName].Copy();
			theSet[DicomTags.StudyDescription] = source[DicomTags.StudyDescription].Copy();
			theSet[DicomTags.SeriesDescription].SetStringValue("Teaching Series");
			theSet[DicomTags.PatientsName] = source[DicomTags.PatientsName].Copy();
			theSet[DicomTags.PatientId] = source[DicomTags.PatientId].Copy();
			theSet[DicomTags.PatientsBirthDate] = source[DicomTags.PatientsBirthDate].Copy();
			theSet[DicomTags.PatientsSex] = source[DicomTags.PatientsSex].Copy();
			theSet[DicomTags.PatientsWeight] = source[DicomTags.PatientsWeight].Copy();
			theSet[DicomTags.StudyInstanceUid] = source[DicomTags.StudyInstanceUid].Copy();
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyId] = source[DicomTags.StudyId].Copy();
			theSet[DicomTags.SeriesNumber].SetStringValue("99");
			theSet[DicomTags.InstanceNumber].SetStringValue("1");

			theSet[DicomTags.ValueType].SetStringValue("CONTAINER");

			DicomSequenceItem item = new DicomSequenceItem();
			theSet[DicomTags.ConceptNameCodeSequence].AddSequenceItem(item);

			item[DicomTags.CodeValue].SetStringValue("113004");
			item[DicomTags.CodingSchemeDesignator].SetStringValue("DCM");
			item[DicomTags.CodeMeaning].SetStringValue("For Teaching");

			theSet[DicomTags.ContinuityOfContent].SetStringValue("SEPARATE");

			item = new DicomSequenceItem();
			theSet[DicomTags.CurrentRequestedProcedureEvidenceSequence].AddSequenceItem(item);

			DicomSequenceItem refSeriesItem = new DicomSequenceItem();
			item[DicomTags.ReferencedSeriesSequence].AddSequenceItem(refSeriesItem);
			refSeriesItem[DicomTags.SeriesInstanceUid] = source[DicomTags.SeriesInstanceUid].Copy();

			DicomSequenceItem refSopItem = new DicomSequenceItem();
			refSeriesItem[DicomTags.ReferencedSopSequence].AddSequenceItem(refSopItem);
			refSopItem[DicomTags.ReferencedSopClassUid].SetStringValue(source[DicomTags.SopClassUid].ToString());
			refSopItem[DicomTags.ReferencedSopInstanceUid].SetStringValue(source[DicomTags.SopInstanceUid].ToString());

			item[DicomTags.StudyInstanceUid] = source[DicomTags.StudyInstanceUid].Copy();

			item[DicomTags.RequestedProcedureId].SetStringValue("MR2R1234");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("MR2S1234");

			theSet[DicomTags.IdenticalDocumentsSequence].AddSequenceItem(new DicomSequenceItem(item.Copy(), true, true, true));

			DicomSequenceItem contentItem = new DicomSequenceItem();

			theSet[DicomTags.ContentSequence].AddSequenceItem(contentItem);

			contentItem[DicomTags.RelationshipType].SetStringValue("CONTAINS");
			contentItem[DicomTags.ValueType].SetStringValue("IMAGE");

			refSopItem = new DicomSequenceItem();
			contentItem[DicomTags.ReferencedSopSequence].AddSequenceItem(refSopItem);
			refSopItem[DicomTags.ReferencedSopClassUid].SetStringValue(source[DicomTags.SopClassUid].ToString());
			refSopItem[DicomTags.ReferencedSopInstanceUid].SetStringValue(source[DicomTags.SopInstanceUid].ToString());
			if (source.Contains(DicomTags.NumberOfFrames))
				refSopItem[DicomTags.ReferencedFrameNumber].SetStringValue("1");
			else
				refSopItem[DicomTags.ReferencedFrameNumber].SetNullValue();

			refSopItem[DicomTags.ReferencedSegmentNumber].SetNullValue();

			contentItem = new DicomSequenceItem();
			theSet[DicomTags.ContentSequence].AddSequenceItem(contentItem);

			contentItem[DicomTags.RelationshipType].SetStringValue("CONTAINS");
			contentItem[DicomTags.TextValue].SetStringValue("Teaching Images");

			item = new DicomSequenceItem();
			contentItem[DicomTags.ConceptNameCodeSequence].AddSequenceItem(item);
			item[DicomTags.CodeValue].SetStringValue("113012");
			item[DicomTags.CodingSchemeDesignator].SetStringValue("DCM");
			item[DicomTags.CodeMeaning].SetStringValue("Key Object Description");
		}

		public virtual void SetupMR(DicomAttributeCollection theSet)
		{
			DateTime studyTime = DateTime.Now;
			theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
			theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
			theSet[DicomTags.InstanceCreationDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.InstanceCreationTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.AcquisitionDatetime].SetDateTime(0, DateTime.Now);
			theSet[DicomTags.SopClassUid].SetStringValue(SopClass.MrImageStorageUid);
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.StudyTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
			theSet[DicomTags.Modality].SetStringValue("MR");
			theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.ManufacturersModelName].SetNullValue();
			theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
			theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
			theSet[DicomTags.StudyDescription].SetStringValue("HEART");
			theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
			theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
			theSet[DicomTags.PatientId].SetStringValue("ID123-45-9999");
			theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
			theSet[DicomTags.PatientsSex].SetStringValue("M");
			theSet[DicomTags.PatientsWeight].SetStringValue("70");
			theSet[DicomTags.PatientsSize].SetStringValue("10.000244140625");
			theSet[DicomTags.PatientsAge].SetStringValue("035Y");
			theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
			theSet[DicomTags.ScanOptions].SetStringValue("CG");
			theSet[DicomTags.MrAcquisitionType].SetStringValue("2D");
			theSet[DicomTags.SliceThickness].SetStringValue("10.000000");
			theSet[DicomTags.RepetitionTime].SetStringValue("857.142883");
			theSet[DicomTags.EchoTime].SetStringValue("8.712100");
			theSet[DicomTags.NumberOfAverages].SetStringValue("1");
			theSet[DicomTags.ImagingFrequency].SetStringValue("63.901150");
			theSet[DicomTags.ImagedNucleus].SetStringValue("1H");
			theSet[DicomTags.EchoNumbers].SetStringValue("1");
			theSet[DicomTags.MagneticFieldStrength].SetStringValue("1.500000");
			theSet[DicomTags.SpacingBetweenSlices].SetStringValue("10.00000");
			theSet[DicomTags.NumberOfPhaseEncodingSteps].SetStringValue("81");
			theSet[DicomTags.EchoTrainLength].SetStringValue("0");
			theSet[DicomTags.PercentSampling].SetStringValue("63.281250");
			theSet[DicomTags.PercentPhaseFieldOfView].SetStringValue("68.75000");
			theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
			theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
			theSet[DicomTags.ProtocolName].SetStringValue("2D EPI BH");
			theSet[DicomTags.TriggerTime].SetStringValue("14.000000");
			theSet[DicomTags.LowRRValue].SetStringValue("948");
			theSet[DicomTags.HighRRValue].SetStringValue("1178");
			theSet[DicomTags.IntervalsAcquired].SetStringValue("102");
			theSet[DicomTags.IntervalsRejected].SetStringValue("0");
			theSet[DicomTags.HeartRate].SetStringValue("56");
			theSet[DicomTags.ReceiveCoilName].SetStringValue("B");
			theSet[DicomTags.TransmitCoilName].SetStringValue("B");
			theSet[DicomTags.InPlanePhaseEncodingDirection].SetStringValue("COL");
			theSet[DicomTags.FlipAngle].SetStringValue("50.000000");
			theSet[DicomTags.PatientPosition].SetStringValue("HFS");
			theSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyId].SetStringValue("1933");
			theSet[DicomTags.SeriesNumber].SetStringValue("1");
			theSet[DicomTags.AcquisitionNumber].SetStringValue("7");
			theSet[DicomTags.InstanceNumber].SetStringValue("1");
			theSet[DicomTags.ImagePositionPatient].SetStringValue("-61.7564\\-212.04848\\-99.6208");
			theSet[DicomTags.ImageOrientationPatient].SetStringValue("0.861\\0.492\\0.126\\-0.2965");
			theSet[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.PositionReferenceIndicator].SetStringValue(null);
			theSet[DicomTags.ImageComments].SetStringValue("Test MR Image");
			theSet[DicomTags.SamplesPerPixel].SetStringValue("1");
			theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			theSet[DicomTags.Rows].SetStringValue("256");
			theSet[DicomTags.Columns].SetStringValue("256");
			theSet[DicomTags.PixelSpacing].SetStringValue("1.367188\\1.367188");
			theSet[DicomTags.BitsAllocated].SetStringValue("16");
			theSet[DicomTags.BitsStored].SetStringValue("12");
			theSet[DicomTags.HighBit].SetStringValue("11");
			theSet[DicomTags.PixelRepresentation].SetStringValue("0");
			theSet[DicomTags.WindowCenter].SetStringValue("238");
			theSet[DicomTags.WindowWidth].SetStringValue("471");
			theSet[DicomTags.RescaleSlope].SetStringValue("1.1234567890123");
			theSet[DicomTags.RescaleIntercept].SetStringValue("0.0123456789012");

			uint length = 256*256*2;

			DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

			byte[] pixelArray = new byte[length];

			for (uint i = 0; i < length; i += 2)
				pixelArray[i] = (byte) (i%255);

			pixels.Values = pixelArray;

			theSet[DicomTags.PixelData] = pixels;

			DicomSequenceItem item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("MRR1234");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("MRS1234");

			item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("MR2R1234");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("MR2S1234");

			DicomSequenceItem studyItem = new DicomSequenceItem();

			item[DicomTags.ReferencedStudySequence].AddSequenceItem(studyItem);

			studyItem[DicomTags.ReferencedSopClassUid].SetStringValue(SopClass.MrImageStorageUid);
			studyItem[DicomTags.ReferencedSopInstanceUid].SetStringValue("1.2.3.4.5.6.7.8.9");
		}

		public void CreateIconImageSequence(DicomAttributeCollection theSet)
		{
			var iconSequence = new DicomSequenceItem();

			theSet[DicomTags.IconImageSequence].SetEmptyValue();
			theSet[DicomTags.IconImageSequence].AddSequenceItem(iconSequence);

			iconSequence[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			iconSequence[DicomTags.SamplesPerPixel].SetInt32(0, 1);
			iconSequence[DicomTags.BitsStored].SetInt32(0, 12);
			iconSequence[DicomTags.BitsAllocated].SetInt32(0, 16);
			iconSequence[DicomTags.HighBit].SetInt32(0, 11);
			iconSequence[DicomTags.PixelRepresentation].SetInt32(0, 1);
			iconSequence[DicomTags.Rows].SetInt32(0, 64);
			iconSequence[DicomTags.Columns].SetInt32(0, 64);

			CreatePixelData(iconSequence);
		}

		public void SetupSecondaryCapture(DicomAttributeCollection theSet)
		{
			DateTime studyTime = DateTime.Now;
			theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
			theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\");
			theSet[DicomTags.InstanceCreationDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.InstanceCreationTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.StudyTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
			theSet[DicomTags.Modality].SetStringValue("MR");
			theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.ManufacturersModelName].SetNullValue();
			theSet[DicomTags.InstitutionName].SetStringValue("Toronto General Hospital");
			theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
			theSet[DicomTags.StudyDescription].SetStringValue("TEST");
			theSet[DicomTags.SeriesDescription].SetStringValue("TEST");
			theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
			theSet[DicomTags.PatientId].SetStringValue("ID123-45-9999");
			theSet[DicomTags.PatientsBirthDate].SetStringValue("19600102");
			theSet[DicomTags.PatientsSize].SetStringValue("10.000244140625");
			theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
			theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
			theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
			theSet[DicomTags.PatientPosition].SetStringValue("HFS");
			theSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyId].SetStringValue("1933");
			theSet[DicomTags.SeriesNumber].SetStringValue("1");
			theSet[DicomTags.InstanceNumber].SetStringValue("1");
			theSet[DicomTags.ImageComments].SetStringValue("Test SC Image");
			theSet[DicomTags.SamplesPerPixel].SetStringValue("1");
			theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			theSet[DicomTags.Rows].SetStringValue("256");
			theSet[DicomTags.Columns].SetStringValue("256");
			theSet[DicomTags.BitsAllocated].SetStringValue("16");
			theSet[DicomTags.BitsStored].SetStringValue("12");
			theSet[DicomTags.HighBit].SetStringValue("11");
			theSet[DicomTags.PixelRepresentation].SetStringValue("0");

			uint length = 256*256*2;

			DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

			byte[] pixelArray = new byte[length];

			for (uint i = 0; i < length; i += 2)
				pixelArray[i] = (byte) (i%255);

			pixels.Values = pixelArray;

			theSet[DicomTags.PixelData] = pixels;

			DicomSequenceItem item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("MRR1234");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("MRS1234");

			item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("MR2R1234");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("MR2S1234");

			DicomSequenceItem studyItem = new DicomSequenceItem();

			item[DicomTags.ReferencedStudySequence].AddSequenceItem(studyItem);

			studyItem[DicomTags.ReferencedSopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			studyItem[DicomTags.ReferencedSopInstanceUid].SetStringValue("1.2.3.4.5.6.7.8.9");
		}

		public void SetupMultiframeXA(DicomAttributeCollection theSet, uint rows, uint columns, uint frames)
		{
			DateTime studyTime = DateTime.Now;
			theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
			theSet[DicomTags.ImageType].SetStringValue("DERIVED\\SECONDARY\\SINGLE PLANE\\SINGLE A");
			theSet[DicomTags.InstanceCreationDate].SetStringValue(DateParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.InstanceCreationTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
			theSet[DicomTags.SopClassUid].SetStringValue(SopClass.XRayAngiographicImageStorageUid);
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.StudyTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesDate].SetStringValue(DateParser.ToDicomString(studyTime));
			theSet[DicomTags.SeriesTime].SetStringValue(TimeParser.ToDicomString(studyTime));
			theSet[DicomTags.AccessionNumber].SetStringValue("A4567");
			theSet[DicomTags.Modality].SetStringValue("XA");
			theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.InstitutionName].SetStringValue("KardiologieUniklinikHeidelberg");
			theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
			theSet[DicomTags.StudyDescription].SetStringValue("HEART");
			theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
			theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
			theSet[DicomTags.PatientId].SetStringValue("ID123-45-9999");
			theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
			theSet[DicomTags.PatientsSex].SetStringValue("M");
			theSet[DicomTags.PatientsWeight].SetStringValue("80");
			theSet[DicomTags.PatientsSize].SetStringValue("10.000244140625");
			theSet[DicomTags.Kvp].SetStringValue("80");
			theSet[DicomTags.ProtocolName].SetStringValue("25  FPS Koronarien");
			theSet[DicomTags.FrameTime].SetStringValue("40");
			theSet[DicomTags.FrameDelay].SetStringValue("0");
			theSet[DicomTags.DistanceSourceToDetector].SetStringValue("1018");
			theSet[DicomTags.ExposureTime].SetStringValue("7");
			theSet[DicomTags.XRayTubeCurrent].SetStringValue("815");
			theSet[DicomTags.RadiationSetting].SetStringValue("GR");
			theSet[DicomTags.IntensifierSize].SetStringValue("169.99998");
			theSet[DicomTags.PositionerMotion].SetStringValue("STATIC");
			theSet[DicomTags.PositionerPrimaryAngle].SetStringValue("-40.2999999");
			theSet[DicomTags.PositionerSecondaryAngle].SetStringValue("-15.5");
			theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
			theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
			theSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyId].SetStringValue("20021208125233");
			theSet[DicomTags.SeriesNumber].SetStringValue("1");
			theSet[DicomTags.AcquisitionNumber].SetStringValue("3");
			theSet[DicomTags.InstanceNumber].SetStringValue("13");
			theSet[DicomTags.PatientOrientation].SetNullValue();
			theSet[DicomTags.ImagesInAcquisition].SetStringValue("1");
			theSet[DicomTags.SamplesPerPixel].SetStringValue("1");
			theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			theSet[DicomTags.NumberOfFrames].SetUInt32(0, frames);
			theSet[DicomTags.Rows].SetUInt32(0, rows);
			theSet[DicomTags.Columns].SetUInt32(0, columns);
			theSet[DicomTags.FrameIncrementPointer].SetUInt32(0, DicomTags.FrameTime);
			theSet[DicomTags.BitsAllocated].SetStringValue("8");
			theSet[DicomTags.BitsStored].SetStringValue("8");
			theSet[DicomTags.HighBit].SetStringValue("7");
			theSet[DicomTags.PixelRepresentation].SetStringValue("0");
			theSet[DicomTags.WindowCenter].SetStringValue("128");
			theSet[DicomTags.WindowWidth].SetStringValue("204.8");
			theSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue("20080219");
			theSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue("143600");
			theSet[DicomTags.PerformedProcedureStepId].SetStringValue("UNKNOWN");

			// Null SQ Test
			theSet[DicomTags.ReferencedStudySequence].SetNullValue();

			// FL & FD tags for testing
			theSet[DicomTags.SlabThickness].SetFloat64(0, 0.1234567d);
			theSet[DicomTags.CalciumScoringMassFactorPatient].SetFloat32(0, 0.7654321f);

			uint length = rows*columns*frames;
			if (length%2 == 1)
				length++;
			DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

			byte[] pixelArray = new byte[length];
			pixelArray[length - 1] = 0x00; // Padding char

			for (uint frameCount = 0; frameCount < frames; frameCount++)
				for (uint i = frameCount*rows*columns, val = frameCount + 1; i < (frameCount + 1)*rows*columns; i++, val++)
					pixelArray[i] = (byte) (val%255);

			pixels.Values = pixelArray;

			theSet[DicomTags.PixelData] = pixels;

			DicomSequenceItem item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("XA123");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA1234");

			item = new DicomSequenceItem();
			theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

			item[DicomTags.RequestedProcedureId].SetStringValue("XA567");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA5678");
		}

		public DicomFile CreateFile(ushort rows, ushort columns, string photometricInterpretation, ushort bitsStored, ushort bitsAllocated, bool isSigned, ushort numberOfFrames)
		{
			DicomFile file = new DicomFile("SCTestImage.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupSecondaryCapture(dataSet);

			dataSet[DicomTags.PixelData] = null;

			DicomUncompressedPixelData pd = new DicomUncompressedPixelData(dataSet)
			                                {
				                                ImageWidth = columns,
				                                ImageHeight = rows,
				                                PhotometricInterpretation = photometricInterpretation,
				                                BitsAllocated = bitsAllocated,
				                                BitsStored = bitsStored,
				                                HighBit = (ushort) (bitsStored - 1),
				                                PixelRepresentation = (ushort) (isSigned ? 1 : 0),
				                                NumberOfFrames = numberOfFrames
			                                };

			CreatePixelData(pd);
			pd.UpdateAttributeCollection(dataSet);

			SetupMetaInfo(file);

			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			return file;
		}

		public DicomFile CreateUNFile(ushort rows, ushort columns, string photometricInterpretation, ushort bitsStored, ushort bitsAllocated, bool isSigned, ushort numberOfFrames)
		{
			DicomFile file = CreateFile(rows, columns, photometricInterpretation, bitsStored, bitsAllocated, isSigned,
			                            numberOfFrames);

			DicomAttributeCollection theSet = file.DataSet;

			theSet[DicomTags.LossyImageCompressionMethod].SetStringValue("ISO_15444_1");
			ConvertAttributeToUN(theSet, DicomTags.LossyImageCompressionMethod);

			return file;
		}

		protected static void CreatePixelData(DicomAttributeCollection dataSet)
		{
			dataSet[DicomTags.PixelData] = null;
			var pd = new DicomUncompressedPixelData(dataSet);
			CreatePixelData(pd);
			pd.UpdateAttributeCollection(dataSet);
		}

		protected static void CreatePixelData(DicomUncompressedPixelData pd)
		{
			var photometricInterpretation = pd.PhotometricInterpretation;
			if (photometricInterpretation.Equals("RGB")
			    || photometricInterpretation.Equals("YBR_FULL"))
			{
				pd.SamplesPerPixel = 3;
				pd.PlanarConfiguration = 1;
				CreateColorPixelData(pd);
			}
			else if (photometricInterpretation.Equals("MONOCHROME1")
			         || photometricInterpretation.Equals("MONOCHROME2"))
			{
				CreateMonochromePixelData(pd);
			}
			else
			{
				throw new DicomException("Unsupported Photometric Interpretation in CreateFile");
			}
		}

		protected static void CreateMonochromePixelData(DicomUncompressedPixelData pd)
		{
			int rows = pd.ImageHeight;
			int cols = pd.ImageWidth;

			int minValue = pd.IsSigned ? -(1 << (pd.BitsStored - 1)) : 0;
			int maxValue = (1 << pd.BitsStored) + minValue - 1;
			const ushort noOpMask = 0xFFFF;
			const byte noOpByteMask = 0xFF;
			var shortMask = (ushort) (noOpMask >> (pd.BitsAllocated - pd.BitsStored));
			var byteMask = (byte) (noOpByteMask >> (pd.BitsAllocated - pd.BitsStored));

			// Create a small block of pixels in the test pattern in an integer,
			// then copy/tile into the full size frame data

			int smallRows = (rows*3)/8;
			int smallColumns = rows/4;
			int stripSize = rows/16;

			int[] smallPixels = new int[smallRows*smallColumns];

			float slope = (float) (maxValue - minValue)/smallColumns;

			int pixelOffset = 0;
			for (int i = 0; i < smallRows; i++)
			{
				if (i < stripSize)
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = (int) ((j*slope) + minValue);
						pixelOffset++;
					}
				}
				else if (i > (smallRows - stripSize))
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = (int) (maxValue - (j*slope));
						pixelOffset++;
					}
				}
				else
				{
					int pixel = minValue + (int) ((i - stripSize)*slope);
					if (pixel < minValue) pixel = minValue + 1;
					if (pixel > maxValue) pixel = maxValue - 1;

					int start = (smallColumns/2) - (i - stripSize)/2;
					int end = (smallColumns/2) + (i - stripSize)/2;

					for (int j = 0; j < smallColumns; j++)
					{
						if (j < start)
							smallPixels[pixelOffset] = minValue;
						else if (j > end)
							smallPixels[pixelOffset] = maxValue;
						else
							smallPixels[pixelOffset] = pixel;

						pixelOffset++;
					}
				}
			}
			// Now create the actual frame
			for (int frame = 0; frame < pd.NumberOfFrames; frame++)
			{
				// Odd length frames are automatically dealt with by DicomUncompressedPixelData
				byte[] frameData = new byte[pd.UncompressedFrameSize];
				pixelOffset = 0;

				if (pd.BitsAllocated == 8)
				{
					if (pd.IsSigned)
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i%smallRows)*smallColumns;

							for (int j = 0; j < cols; j++)
							{
								frameData[pixelOffset] = (byte) ((sbyte) smallPixels[smallOffset + j%smallColumns]);
								frameData[pixelOffset] = (byte) (frameData[pixelOffset] & byteMask);
								pixelOffset++;
							}
						}
					}
					else
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i%smallRows)*smallColumns;

							for (int j = 0; j < cols; j++)
							{
								frameData[pixelOffset] = (byte) smallPixels[smallOffset + j%smallColumns];
								frameData[pixelOffset] = (byte) (frameData[pixelOffset] & byteMask);
								pixelOffset++;
							}
						}
					}
				}
				else
				{
					if (pd.IsSigned)
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i%smallRows)*smallColumns;

							for (int j = 0; j < cols; j++)
							{
								short pixel = (short) smallPixels[smallOffset + j%smallColumns];
								pixel = (short) (pixel & shortMask);

								frameData[pixelOffset] = (byte) (pixel & 0x00FF);
								pixelOffset++;
								frameData[pixelOffset] = (byte) ((pixel & 0xFF00) >> 8);
								pixelOffset++;
							}
						}
					}
					else
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i%smallRows)*smallColumns;

							for (int j = 0; j < cols; j++)
							{
								ushort pixel = (ushort) smallPixels[smallOffset + j%smallColumns];
								pixel &= shortMask;
								frameData[pixelOffset] = (byte) (pixel & 0x00FF);
								pixelOffset++;
								frameData[pixelOffset] = (byte) ((pixel & 0xFF00) >> 8);
								pixelOffset++;
							}
						}
					}
				}

				pd.AppendFrame(frameData);
			}
		}

		protected static void CreateColorPixelData(DicomUncompressedPixelData pd)
		{
			int rows = pd.ImageHeight;
			int cols = pd.ImageWidth;

			int minValue = pd.IsSigned ? -(1 << (pd.BitsStored - 1)) : 0;
			int maxValue = (1 << pd.BitsStored) + minValue - 1;
			const byte noOpByteMask = 0xFF;
			var byteMask = (byte) (noOpByteMask >> (pd.BitsAllocated - pd.BitsStored));

			// Create a small block of pixels in the test pattern in an integer,
			// then copy/tile into the full size frame data

			int smallRows = (rows*3)/8;
			int smallColumns = rows/4;
			int stripSize = rows/16;

			int[] smallPixels = new int[smallRows*smallColumns*3];

			float slope = (float) (maxValue - minValue)/smallColumns;

			int pixelOffset = 0;
			for (int i = 0; i < smallRows; i++)
			{
				if (i < stripSize)
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = (int) ((j*slope) + minValue);
						pixelOffset++;
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
					}
				}
				else if (i > (smallRows - stripSize))
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
						smallPixels[pixelOffset] = (int) (maxValue - (j*slope));
						pixelOffset++;
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
					}
				}
				else
				{
					int pixel = minValue + (int) ((i - stripSize)*slope);
					if (pixel < minValue) pixel = minValue + 1;
					if (pixel > maxValue) pixel = maxValue - 1;

					int start = (smallColumns/2) - (i - stripSize)/2;
					int end = (smallColumns/2) + (i - stripSize)/2;

					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
						smallPixels[pixelOffset] = 0;
						pixelOffset++;
						if (j < start)
							smallPixels[pixelOffset] = minValue;
						else if (j > end)
							smallPixels[pixelOffset] = maxValue;
						else
							smallPixels[pixelOffset] = pixel;

						pixelOffset++;
					}
				}
			}
			// Now create the actual frame
			for (int frame = 0; frame < pd.NumberOfFrames; frame++)
			{
				// Odd length frames are automatically dealt with by DicomUncompressedPixelData
				byte[] frameData = new byte[pd.UncompressedFrameSize];
				pixelOffset = 0;

				for (int i = 0; i < rows; i++)
				{
					int smallOffset = (i%smallRows)*smallColumns*3;

					for (int j = 0; j < cols*3; j++)
					{
						frameData[pixelOffset] = (byte) smallPixels[smallOffset + j%(smallColumns*3)];
						frameData[pixelOffset] &= byteMask;
						pixelOffset++;
					}
				}

				pd.AppendFrame(frameData);
			}
		}
	}
}