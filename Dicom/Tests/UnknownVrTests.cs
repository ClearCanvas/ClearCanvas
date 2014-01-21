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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.IO;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class UnknownVrTests : AbstractTest
	{
        private void SetupPrivateGroups(List<DicomTag> tagList, DicomAttributeCollection dataSet)
        {
            DicomTag privateCreatorTag = new DicomTag(0x00090010, "PrivateCreator", "PrivateCreator", DicomVr.LOvr, false, 1, 1,
                                                      false);
            dataSet[privateCreatorTag].SetStringValue("ClearCanvasGroup9");

            DicomTag privateTagFL = new DicomTag(0x00091020, "PrivateFL", "PrivateFL", DicomVr.FLvr, true, 1, 10, false);
            DicomTag privateTagLO = new DicomTag(0x00091021, "PrivateLO", "PrivateLO", DicomVr.LOvr, true, 1, 10, false);
            DicomTag privateTagAE = new DicomTag(0x00091022, "PrivateAE", "PrivateAE", DicomVr.AEvr, true, 1, 10, false);
            DicomTag privateTagAS = new DicomTag(0x00091023, "PrivateAS", "PrivateAS", DicomVr.ASvr, true, 1, 10, false);
            DicomTag privateTagAT = new DicomTag(0x00091024, "PrivateAT", "PrivateAT", DicomVr.ATvr, true, 1, 10, false);
            DicomTag privateTagCS = new DicomTag(0x00091025, "PrivateCS", "PrivateCS", DicomVr.CSvr, true, 1, 10, false);
            DicomTag privateTagDA = new DicomTag(0x00091026, "PrivateDA", "PrivateDA", DicomVr.DAvr, true, 1, 10, false);
            DicomTag privateTagDS = new DicomTag(0x00091027, "PrivateDS", "PrivateDS", DicomVr.DSvr, true, 1, 10, false);
            DicomTag privateTagDT = new DicomTag(0x00091028, "PrivateDT", "PrivateDT", DicomVr.DTvr, true, 1, 10, false);
            DicomTag privateTagFD = new DicomTag(0x00091029, "PrivateFD", "PrivateFD", DicomVr.FDvr, true, 1, 10, false);
            DicomTag privateTagIS = new DicomTag(0x00091030, "PrivateIS", "PrivateIS", DicomVr.ISvr, true, 1, 10, false);
            DicomTag privateTagLT = new DicomTag(0x00091031, "PrivateLT", "PrivateLT", DicomVr.LTvr, true, 1, 1, false);
            DicomTag privateTagOB = new DicomTag(0x00091032, "PrivateOB", "PrivateOB", DicomVr.OBvr, true, 1, 1, false);
            DicomTag privateTagOF = new DicomTag(0x00091033, "PrivateOF", "PrivateOF", DicomVr.OFvr, true, 1, 1, false);
            DicomTag privateTagOW = new DicomTag(0x00091034, "PrivateOW", "PrivateOW", DicomVr.OWvr, true, 1, 1, false);
            DicomTag privateTagPN = new DicomTag(0x00091035, "PrivatePN", "PrivatePN", DicomVr.PNvr, true, 1, 10, false);
            DicomTag privateTagSH = new DicomTag(0x00091036, "PrivateSH", "PrivateSH", DicomVr.SHvr, true, 1, 10, false);
            DicomTag privateTagSL = new DicomTag(0x00091037, "PrivateSL", "PrivateSL", DicomVr.SLvr, true, 1, 10, false);
            DicomTag privateTagSQ = new DicomTag(0x00091038, "PrivateSQ", "PrivateSQ", DicomVr.SQvr, true, 1, 10, false);
            DicomTag privateTagSS = new DicomTag(0x00091039, "PrivateSS", "PrivateSS", DicomVr.SSvr, true, 1, 10, false);
            DicomTag privateTagST = new DicomTag(0x00091040, "PrivateST", "PrivateST", DicomVr.STvr, true, 1, 10, false);
            DicomTag privateTagTM = new DicomTag(0x00091041, "PrivateTM", "PrivateTM", DicomVr.TMvr, true, 1, 10, false);
            DicomTag privateTagUI = new DicomTag(0x00091042, "PrivateUI", "PrivateUI", DicomVr.UIvr, true, 1, 10, false);
            DicomTag privateTagUL = new DicomTag(0x00091043, "PrivateUI", "PrivateUI", DicomVr.ULvr, true, 1, 10, false);
            DicomTag privateTagUS = new DicomTag(0x00091044, "PrivateUS", "PrivateUS", DicomVr.USvr, true, 1, 10, false);
            DicomTag privateTagUT = new DicomTag(0x00091045, "PrivateUT", "PrivateUT", DicomVr.UTvr, true, 1, 1, false);
            DicomTag privateTagOD = new DicomTag(0x00091046, "PrivateOD", "PrivateOD", DicomVr.ODvr, true, 1, 1, false);

            dataSet[privateTagFL].AppendFloat32(1.1f);
            dataSet[privateTagFL].AppendFloat32(1.1123132f);
            tagList.Add(privateTagFL);
            dataSet[privateTagLO].AppendString("Test");
            dataSet[privateTagLO].AppendString("Test Me 2");
            tagList.Add(privateTagLO);
            dataSet[privateTagAE].AppendString("TESTAE1");
            dataSet[privateTagAE].AppendString("TESTAE2");
            tagList.Add(privateTagAE);
            dataSet[privateTagAS].AppendString("003Y");
            dataSet[privateTagAS].AppendString("003D");
            tagList.Add(privateTagAS);
            dataSet[privateTagAT].AppendUInt32(DicomTags.ZoomFactor);
            dataSet[privateTagAT].AppendUInt32(DicomTags.PhotometricInterpretation);
            tagList.Add(privateTagAT);
            dataSet[privateTagCS].AppendString("CODE1");
            dataSet[privateTagCS].AppendString("CODE2");
            tagList.Add(privateTagCS);
            dataSet[privateTagDA].AppendDateTime(DateTime.Now);
            dataSet[privateTagDA].AppendDateTime(DateTime.Now);
            tagList.Add(privateTagDA);
            dataSet[privateTagDS].AppendFloat64(1.12351234124124f);
            dataSet[privateTagDS].AppendFloat64(-12312312312.1231f);
            tagList.Add(privateTagDS);
            dataSet[privateTagDT].AppendDateTime(DateTime.Now);
            dataSet[privateTagDT].AppendDateTime(DateTime.Now);
            tagList.Add(privateTagDT);
            dataSet[privateTagFD].AppendFloat64(1.112312d);
            dataSet[privateTagFD].AppendFloat64(-11123.13211d);
            tagList.Add(privateTagFD);
            dataSet[privateTagIS].AppendString("123456789");
            dataSet[privateTagIS].AppendString("123456789876");
            tagList.Add(privateTagIS);
            dataSet[privateTagLT].SetStringValue("Now is the time for all good men to come to the aide of their country.");
            tagList.Add(privateTagLT);
            dataSet[privateTagPN].AppendString("Last^First^Middle^Post");
            dataSet[privateTagPN].AppendString("W^Steven^R^Test");
            tagList.Add(privateTagPN);
            dataSet[privateTagST].SetStringValue("Now is the time for all good men to come to the aide of their country.");
            tagList.Add(privateTagST);
            dataSet[privateTagSH].AppendString("Short text 1");
            dataSet[privateTagSH].AppendString("Short text 2");
            dataSet[privateTagSH].AppendString("Short text 3");
            dataSet[privateTagSH].AppendString("Short text 4");
            tagList.Add(privateTagSH);
            dataSet[privateTagSL].AppendInt32(1024);
            dataSet[privateTagSL].AppendInt32(2048);
            dataSet[privateTagSL].AppendInt32(4096);
            dataSet[privateTagSL].AppendInt32(8192);
            dataSet[privateTagSL].AppendInt32(-1024);
            dataSet[privateTagSL].AppendInt32(-2048);
            dataSet[privateTagSL].AppendInt32(-4096);
            dataSet[privateTagSL].AppendInt32(-8192);
            tagList.Add(privateTagSL);
            dataSet[privateTagSS].AppendInt16(-1024);
            dataSet[privateTagSS].AppendInt16(512);
            dataSet[privateTagSS].AppendInt16(-256);
            dataSet[privateTagSS].AppendInt16(128);
            tagList.Add(privateTagSS);
            dataSet[privateTagTM].AppendDateTime(DateTime.Now);
            dataSet[privateTagTM].AppendDateTime(DateTime.Now);
            tagList.Add(privateTagTM);
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            dataSet[privateTagUI].AppendUid(DicomUid.GenerateUid());
            tagList.Add(privateTagUI);
            dataSet[privateTagUL].AppendUInt32(128);
            dataSet[privateTagUL].AppendUInt32(1024);
            dataSet[privateTagUL].AppendUInt32(16384);
            dataSet[privateTagUL].AppendUInt32(123123123);
            tagList.Add(privateTagUL);
            dataSet[privateTagUS].AppendUInt16(128);
            dataSet[privateTagUS].AppendUInt16(64);
            dataSet[privateTagUS].AppendUInt16(256);
            tagList.Add(privateTagUS);
            dataSet[privateTagUT].SetStringValue("A man, a plan, a canal, panama.");
            tagList.Add(privateTagUT);

            dataSet[privateTagOB].Values = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            tagList.Add(privateTagOB);

            dataSet[privateTagOW].Values = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            tagList.Add(privateTagOW);

            dataSet[privateTagOF].Values = new[] { 1.1111f, 2.222f, 3.3333f, 44444.444f, 5555.555f, 66666.66666f, 123123.123123f };
            tagList.Add(privateTagOF);

            dataSet[privateTagOD].Values = new[] { 1.11111111111111, 2.22222222222, 3333333333.3333, 44444.44444444444, 5555.555555555555, 66666.666666666666, 123123.123123123123123123 };
            tagList.Add(privateTagOD);

            DicomSequenceItem item = new DicomSequenceItem();
            item[DicomTags.PhotometricInterpretation].AppendString("MONOCHROME1");
            item[DicomTags.Rows].AppendUInt16(256);
            item[DicomTags.Columns].AppendUInt16(256);
            item[DicomTags.BitsAllocated].AppendUInt16(8);
            item[DicomTags.BitsStored].AppendUInt16(8);
            item[DicomTags.HighBit].AppendUInt16(7);
            item[DicomTags.PixelData].Values = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            dataSet[privateTagSQ].AddSequenceItem(item);
            // SQ Attribute UN parsing does not work right now, don't add to the list.
            //tagList.Add(privateTagSQ);

        }

	    [Test]
		public void PrivateAttributeTest()
		{

			DicomFile file = new DicomFile("LittleEndianPrivateReadFileTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);
            dataSet[DicomTags.StudyDescription].SetNullValue();

            List<DicomTag> tagList = new List<DicomTag>();

	        SetupPrivateGroups(tagList, dataSet);
			
			SetupMetaInfo(file);


			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;

			DicomReadOptions readOptions = DicomReadOptions.Default;

			// Use ExplicitLengthSequence to force SQ attributes to UN VR when they're read back in
            Assert.IsTrue(file.Save(DicomWriteOptions.ExplicitLengthSequence));

			DicomFile newFile = new DicomFile(file.Filename);

			newFile.Load(readOptions);

			Assert.IsTrue(newFile.DataSet[DicomTags.StudyDescription].IsNull);

			Assert.AreNotEqual(file.DataSet.Equals(newFile.DataSet), true);

			foreach (DicomTag tag in tagList)
			{
				DicomAttributeUN unAttrib = newFile.DataSet[tag] as DicomAttributeUN;
				Assert.IsNotNull(unAttrib, String.Format("UN VR Attribute is not null for tag {0}",tag));

				ByteBuffer bb = unAttrib.GetByteBuffer(TransferSyntax.ImplicitVrLittleEndian,
														 newFile.DataSet[DicomTags.SpecificCharacterSet].ToString());
				Assert.IsNotNull(bb, String.Format("ByteBuffer not null for tag: {0}", tag));

				DicomAttribute validAttrib = tag.VR.CreateDicomAttribute(tag, bb);
				Assert.IsNotNull(validAttrib);

				Assert.IsTrue(validAttrib.Equals(file.DataSet[tag]), String.Format("Attributes equal for tag {0}", tag));
			}
		}

        [Test]
        public void StandardAttributeTest()
        {
            DicomFile file = new DicomFile("UNTest.dcm");

            DicomAttributeCollection dataSet = file.DataSet;
            SetupMetaInfo(file);
            SetupMR(dataSet);


            dataSet[DicomTags.RetrieveAeTitle].SetStringValue("TESTAE");
            dataSet[DicomTags.RetrieveAeTitle].AppendString("TESTAE2");
            dataSet[DicomTags.RetrieveAeTitle].AppendString("TESTAE3");
            dataSet[DicomTags.RetrieveAeTitle].AppendString("TESTAE4");
            dataSet[DicomTags.InstitutionAddress].SetStringValue("1224 Milwaukee Ave."); 
            dataSet[DicomTags.TimeRange].SetFloat32(0,1.111111f);
            dataSet[DicomTags.TimeRange].AppendFloat32(2.222222f);
            dataSet[DicomTags.RecommendedDisplayFrameRateInFloat].AppendFloat32(2.222222f);
            dataSet[DicomTags.ReferencePixelX0].AppendInt32(101010);
            dataSet[DicomTags.VerticesOfThePolygonalExposureControlSensingRegion].AppendInt16(324);
            dataSet[DicomTags.VerticesOfThePolygonalExposureControlSensingRegion].AppendInt16(111);
            dataSet[DicomTags.VerticesOfThePolygonalExposureControlSensingRegion].AppendInt16(1234);
            dataSet[DicomTags.DimensionIndexValues].AppendInt32(123456);
            dataSet[DicomTags.DimensionIndexValues].AppendInt32(789);
            dataSet[DicomTags.DimensionIndexValues].AppendInt32(98765);
            dataSet[DicomTags.PixelDataProviderUrl].SetStringValue("http://www.clearcanvas.ca");
            dataSet[DicomTags.EncapsulatedDocument].Values = new byte[] {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07};
            dataSet[DicomTags.VectorGridData].Values = new[] { 1234f, .01010f, 31231.31231f, 41414.4141414f };

            DicomAttributeCollection originalDataSet = file.DataSet.Copy();
            DicomAttributeCollection originalMetaInfo = file.MetaInfo.Copy();
            DicomFile originalFile = new DicomFile("", originalMetaInfo, originalDataSet);

            // Force a sampling of tags to UN
            ConvertAttributeToUN(dataSet, DicomTags.RetrieveAeTitle); // AE
            ConvertAttributeToUN(dataSet, DicomTags.PatientsAge);  // AS
            ConvertAttributeToUN(dataSet, DicomTags.TimeRange); // FD
            ConvertAttributeToUN(dataSet, DicomTags.RecommendedDisplayFrameRateInFloat); // FL
            ConvertAttributeToUN(dataSet, DicomTags.Modality); // CS
            ConvertAttributeToUN(dataSet, DicomTags.SeriesDate);  // DA
            ConvertAttributeToUN(dataSet, DicomTags.PatientsWeight); // DS
            ConvertAttributeToUN(dataSet, DicomTags.AcquisitionDatetime);  // DT
            ConvertAttributeToUN(dataSet, DicomTags.EchoTrainLength); // IS
            ConvertAttributeToUN(dataSet, DicomTags.Manufacturer); // LO
            ConvertAttributeToUN(dataSet, DicomTags.ImageComments); // LT
            ConvertAttributeToUN(dataSet, DicomTags.EncapsulatedDocument); // OB
            ConvertAttributeToUN(dataSet, DicomTags.VectorGridData); // OF            
            ConvertAttributeToUN(dataSet, DicomTags.ReferringPhysiciansName); // PN
            ConvertAttributeToUN(dataSet, DicomTags.AccessionNumber);  // SH
            ConvertAttributeToUN(dataSet, DicomTags.ReferencePixelX0);  // SL
            ConvertAttributeToUN(dataSet, DicomTags.VerticesOfThePolygonalExposureControlSensingRegion);  // SS
            ConvertAttributeToUN(dataSet, DicomTags.InstitutionAddress); // ST
            ConvertAttributeToUN(dataSet, DicomTags.SeriesTime);  // TM
            ConvertAttributeToUN(dataSet, DicomTags.StudyInstanceUid); // UI
            ConvertAttributeToUN(dataSet, DicomTags.DimensionIndexValues); // UL
            ConvertAttributeToUN(dataSet, DicomTags.SamplesPerPixel); // US
            ConvertAttributeToUN(dataSet, DicomTags.PixelDataProviderUrl); // UT

            // Explicit Big
            file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;
            file.Save();
            DicomFile newFile = new DicomFile(file.Filename);
            newFile.Load(DicomReadOptions.UseDictionaryForExplicitUN);

            List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();
            bool compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);


            // Explicit Little
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            file.Save();
            newFile = new DicomFile(file.Filename);
            newFile.Load(DicomReadOptions.UseDictionaryForExplicitUN);

            results = new List<DicomAttributeComparisonResult>();
            compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);

            // Implicit Little
            file.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
            file.Save();
            newFile = new DicomFile(file.Filename);
            newFile.Load(DicomReadOptions.UseDictionaryForExplicitUN);

            results = new List<DicomAttributeComparisonResult>();
            compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);

        }

        [Test]
        public void SQUNTests()
        {
            uint fakeTag = 0x00880199; // IconImageSequence is 0x00880200
            DicomTag fakeSQTag = new DicomTag(fakeTag, "Fake Icon", "FakeIcon", DicomVr.SQvr, false, 1, 1, false);

            if (DicomTagDictionary.TagDictionary.ContainsKey(fakeTag))
                DicomTagDictionary.TagDictionary.Remove(fakeTag);
            DicomTagDictionary.TagDictionary.Add(fakeTag, fakeSQTag);

            DicomFile file = new DicomFile("SQUNTest.dcm");
            DicomAttributeCollection dataSet = file.DataSet;
            SetupMetaInfo(file);
            SetupMR(dataSet);

            DicomSequenceItem item = new DicomSequenceItem();
            item[DicomTags.PhotometricInterpretation].AppendString("MONOCHROME1");
            item[DicomTags.Rows].AppendUInt16(4);
            item[DicomTags.Columns].AppendUInt16(4);
            item[DicomTags.BitsAllocated].AppendUInt16(8);
            item[DicomTags.BitsStored].AppendUInt16(8);
            item[DicomTags.HighBit].AppendUInt16(7);
            item[DicomTags.PixelData].Values = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };
            dataSet[fakeSQTag].AddSequenceItem(item);

            // Save the file
            DicomFile originalFile = new DicomFile("", file.MetaInfo.Copy(), file.DataSet.Copy());

            DicomTagDictionary.TagDictionary.Remove(fakeTag);

            List<DicomAttributeComparisonResult> results = new List<DicomAttributeComparisonResult>();
            bool compare;
            DicomReadOptions readOptions = DicomReadOptions.Default;



            // Little Endian Tests
            file.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;

            // Implicit Little Endian, No tag in the Dictionary, Explicit Length SQ
            Assert.IsTrue(file.Save(DicomWriteOptions.ExplicitLengthSequence), "UN File Save");
            DicomFile newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            Assert.AreNotEqual(originalFile.DataSet.Equals(newFile.DataSet), true);
            DicomFile saveUNfile = newFile;

            // Implicit Little Endian, No tag in the Dictionary, Implicit Length SQ
            // Parser knows its a SQ from implicit length SQ
            Assert.IsTrue(file.Save(DicomWriteOptions.ExplicitLengthSequenceItem), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);


            // Implicit Little Endian, No tag in the Dictionary, Implicit Length SQ
            // Parser knows its a SQ from implicit length SQ
            Assert.IsTrue(file.Save(DicomWriteOptions.None), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = originalFile.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);


            file = saveUNfile;
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            // Explicit Little Endian, No tag in the dictionary, UN SQ
            // Parser keeps tag as UN
            Assert.IsTrue(file.Save(DicomWriteOptions.None), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = file.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
            Assert.AreNotEqual(originalFile.DataSet.Equals(newFile.DataSet), true);


            file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

            // Explicit Big Endian, No tag in the dictionary, UN SQ
            // Parser keeps tag as UN
            Assert.IsTrue(file.Save(DicomWriteOptions.None), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = file.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
            Assert.AreNotEqual(originalFile.DataSet.Equals(newFile.DataSet), true);

        
            // Now add the tag into thedictionary
            DicomTagDictionary.TagDictionary.Add(fakeTag, fakeSQTag);
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            // Note, if we add parsing of SQ UN tags, the result of this test would change
            // Explicit Little Endian, Tag in the dictionary, UN SQ
            // Parser keeps tag as UN
            Assert.IsTrue(file.Save(DicomWriteOptions.None), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = file.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
            Assert.AreNotEqual(originalFile.DataSet.Equals(newFile.DataSet), true);


            file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

            // Note, if we add parsing of SQ UN tags, the result of this test would change
            // Explicit Big Endian, Tag in the dictionary, UN SQ
            // Parser keeps tag as UN
            Assert.IsTrue(file.Save(DicomWriteOptions.None), "UN File Save");
            newFile = new DicomFile(file.Filename);
            newFile.Load(readOptions);
            results = new List<DicomAttributeComparisonResult>();
            compare = file.DataSet.Equals(newFile.DataSet, ref results);
            Assert.IsTrue(compare, results.Count > 0 ? CollectionUtils.FirstElement(results).Details : string.Empty);
            Assert.AreNotEqual(originalFile.DataSet.Equals(newFile.DataSet), true);

        }
	}
}

#endif