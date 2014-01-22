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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageServer.Core.Edit.Test
{
    public abstract class SetDicomTagTestBase:AbstractTest
    {
        public const string Latin1 = "ISO_IR 100";
        public const string Utf8 = "ISO_IR 192";

        [TestFixtureSetUp]
        virtual public void Init()
        {
            DicomImplementation.UnitTest = true;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            DicomImplementation.UnitTest = false;
        }

        public bool UnicodeAllowed
        {
            get
            {
                return ImageServer.Common.Settings.Default.AllowedConvertToUnicodeOnEdit;
            }
            set
            {
                // make sure it's loaded
                var temp = ImageServer.Common.Settings.Default.AllowedConvertToUnicodeOnEdit;
                ImageServer.Common.Settings.Default.PropertyValues["AllowedConvertToUnicodeOnEdit"].PropertyValue = value;
            }
        }

        public void AssertTagValueChanged(uint tag, string valueToSet, string originalCharacterSet, string expectedNewCharacterSet)
        {
            DicomAttributeCollection dataset = new DicomAttributeCollection();
            SetupDataSet(dataset, originalCharacterSet);
            DicomFile file = new DicomFile("test", CreateMetaInfo(), dataset);

            Assert.AreEqual(originalCharacterSet, file.DataSet[DicomTags.SpecificCharacterSet].ToString());

            SetTagCommand cmd = new SetTagCommand(tag, valueToSet);

            Assert.AreEqual(cmd.CanSaveInUnicode, UnicodeAllowed, "SetTagCommand.CanSaveInUnicode returns an incorrect value");

			var sq = new OriginalAttributesSequence
			{
				ModifiedAttributesSequence = new DicomSequenceItem(),
				ModifyingSystem = ProductInformation.Component,
				ReasonForTheAttributeModification = "CORRECT",
				AttributeModificationDatetime = Platform.Time,
				SourceOfPreviousValues = file.SourceApplicationEntityTitle
			};

            Assert.IsTrue(cmd.Apply(file, sq), "SetTagCommand.Apply failed");

            var filename = string.Format("Test-{0}.dcm", DicomTagDictionary.GetDicomTag(tag).Name);
            Assert.IsTrue(file.Save(filename), "Unable to save dicom file");
            file = new DicomFile(filename);
            file.Load();

            if (valueToSet == null)
                Assert.AreEqual(string.Empty, file.DataSet[tag].ToString());
            else
                Assert.AreEqual(valueToSet, file.DataSet[tag].ToString());

            Assert.IsTrue(file.DataSet[DicomTags.SpecificCharacterSet].ToString().Equals(expectedNewCharacterSet));

            Delete(filename);

        }

        public void AssertExceptionThrown(uint tag, string valueToSet, string originalCharacterSet)
        {
            DicomAttributeCollection dataset = new DicomAttributeCollection();
            SetupDataSet(dataset, originalCharacterSet);
            DicomFile file = new DicomFile("test", CreateMetaInfo(), dataset);

            Assert.AreEqual(originalCharacterSet, file.DataSet[DicomTags.SpecificCharacterSet].ToString());
			
			var sq = new OriginalAttributesSequence
			{
				ModifiedAttributesSequence = new DicomSequenceItem(),
				ModifyingSystem = ProductInformation.Component,
				ReasonForTheAttributeModification = "CORRECT",
				AttributeModificationDatetime = Platform.Time,
				SourceOfPreviousValues = file.SourceApplicationEntityTitle
			};

            SetTagCommand cmd = new SetTagCommand(tag, valueToSet);
            try
            {
                cmd.Apply(file, sq);
                Assert.Fail("DicomCharacterSetException is expected");
            }
            catch(DicomCharacterSetException)
            {
                // good
                return;
            }
            catch(Exception)
            {
                Assert.Fail("DicomCharacterSetException is expected");
            }
        }


        private void Delete(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public DicomAttributeCollection CreateMetaInfo()
        {
            DicomAttributeCollection theSet = new DicomAttributeCollection();
            return theSet;

        }

        public void SetupDataSet(DicomAttributeCollection theSet, string characterSet)
        {
            DateTime studyTime = DateTime.Now;
            theSet[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);
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

            uint length = 256 * 256 * 2;

            DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i += 2)
                pixelArray[i] = (byte)(i % 255);

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
    }
}

#endif