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

#pragma warning disable 1591,0419,1574,1587, 649

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Anonymization.Tests
{
	internal class UidData
	{
		[DicomField(DicomTags.StudyInstanceUid)]
		public string StudyInstanceUid;

		[DicomField(DicomTags.SeriesInstanceUid)]
		public string SeriesInstanceUid;

		[DicomField(DicomTags.SopInstanceUid)]
		public string SopInstanceUid;

		[DicomField(DicomTags.ReferencedSopInstanceUid)]
		public string ReferencedSopInstanceUid;

		[DicomField(DicomTags.FrameOfReferenceUid)]
		public string FrameOfReferenceUid;

		[DicomField(DicomTags.SynchronizationFrameOfReferenceUid)]
		public string SynchronizationFrameOfReferenceUid;

		[DicomField(DicomTags.Uid)]
		public string Uid;

		[DicomField(DicomTags.ReferencedFrameOfReferenceUid)]
		public string ReferencedFrameOfReferenceUid;

		[DicomField(DicomTags.RelatedFrameOfReferenceUid)]
		public string RelatedFrameOfReferenceUid;

		[DicomField(DicomTags.InstanceCreatorUid)]
		public string InstanceCreatorUid;

		[DicomField(DicomTags.StorageMediaFileSetUid)]
		public string StorageMediaFileSetUid;

		private DicomAttributeSQ _referencedImageSequence;

		[DicomField(DicomTags.ReferencedImageSequence)]
		public DicomAttributeSQ ReferencedImageSequence
		{
			get { return _referencedImageSequence; }
			set { _referencedImageSequence = value != null ? (DicomAttributeSQ) value.Copy(true) : null; }
		}

		public string ReferencedImageSequenceInstanceCreatorUid
		{
			get
			{
				if (ReferencedImageSequence != null && !ReferencedImageSequence.IsEmpty && !ReferencedImageSequence.IsNull)
					return ((DicomSequenceItem[]) ReferencedImageSequence.Values)[0][DicomTags.InstanceCreatorUid].ToString();
				return null;
			}
		}
	}

	internal class DateData
	{
		[DicomField(DicomTags.InstanceCreationDate)]
		public string InstanceCreationDate;

		[DicomField(DicomTags.SeriesDate)]
		public string SeriesDate;

		[DicomField(DicomTags.ContentDate)]
		public string ContentDate;

		[DicomField(DicomTags.AcquisitionDatetime)]
		public string AcquisitionDatetime;
	}

	[TestFixture]
	public class AnonymizationTests : AbstractTest
	{
		private DicomFile _file;

		private UidData _originalUidData;
		private StudyData _originaStudyData;
		private SeriesData _originaSeriesData;
		private DateData _originalDateData;

		private UidData _anonymizedUidData;
		private StudyData _anonymizedStudyData;
		private SeriesData _anonymizedSeriesData;
		private DateData _anonymizedDateData;

		public AnonymizationTests() {}

		private DicomFile CreateTestFile()
		{
			DicomFile file = new DicomFile();

			SetupMultiframeXA(file.DataSet, 128, 128, 2);

			file.DataSet[DicomTags.PatientsName].SetStringValue("Doe^Jon^Mr");

			//NOTE: this is not intended to be a realistic dataset; we're just testing the anonymizer.

			//set fixed values for dates (SetupMultiframeXA uses today's date)
			file.DataSet[DicomTags.InstanceCreationDate].SetStringValue("20080219");
			file.DataSet[DicomTags.InstanceCreationTime].SetStringValue("143600");
			file.DataSet[DicomTags.StudyDate].SetStringValue("20080219");
			file.DataSet[DicomTags.StudyTime].SetStringValue("143600");
			file.DataSet[DicomTags.SeriesDate].SetStringValue("20080219");
			file.DataSet[DicomTags.SeriesTime].SetStringValue("143700");

			//add a couple of the dates that get adjusted.
			file.DataSet[DicomTags.ContentDate].SetStringValue("20080219");
			file.DataSet[DicomTags.AcquisitionDatetime].SetStringValue("20080219100406");

			//add a couple of the Uids that get remapped (ones that aren't already in the data set).
			file.DataSet[DicomTags.ReferencedSopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			file.DataSet[DicomTags.Uid].SetStringValue(DicomUid.GenerateUid().UID);

			//add a couple of the tags that get removed.
			file.DataSet[DicomTags.InstanceCreatorUid].SetStringValue(DicomUid.GenerateUid().UID);
			file.DataSet[DicomTags.StorageMediaFileSetUid].SetStringValue(DicomUid.GenerateUid().UID);

			//add a couple of the tags that get nulled.
			file.DataSet[DicomTags.StationName].SetStringValue("STATION1");
			file.DataSet[DicomTags.PatientComments].SetStringValue("Claustrophobic");

			//sequence
			DicomSequenceItem item = new DicomSequenceItem();
			file.DataSet[DicomTags.ReferencedImageSequence].AddSequenceItem(item);
			item[DicomTags.ReferencedSopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			item[DicomTags.ReferencedFrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			//will be removed
			item[DicomTags.InstanceCreatorUid].SetStringValue(DicomUid.GenerateUid().UID);

			//nested sequence; will be removed
			item[DicomTags.RequestAttributesSequence].AddSequenceItem(item = new DicomSequenceItem());
			item[DicomTags.RequestedProcedureId].SetStringValue("XA123");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA1234");
			return file;
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestStrict()
		{
			DicomFile file = CreateTestFile();
			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;

			try
			{
				//this ought to work.
				anonymizer.Anonymize(file);
			}
			catch (Exception)
			{
				Assert.Fail("Not strict - no exception expected.");
			}

			anonymizer = new DicomAnonymizer();
			Assert.IsTrue(anonymizer.ValidationOptions == ValidationOptions.Default); //strict by default

			//should throw.
			anonymizer.Anonymize(CreateTestFile());
		}

		[Test]
		public void TestSimple()
		{
			Initialize();

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;
			anonymizer.Anonymize(_file);

			AfterAnonymize(new StudyData(), new SeriesData());

			ValidateNullDates(_anonymizedDateData);
		}

		[Test]
		public void TestPrototypes()
		{
			Initialize();

			StudyData studyPrototype = new StudyData();
			studyPrototype.PatientId = "123";
			studyPrototype.PatientsBirthDateRaw = "19760810";
			studyPrototype.PatientsNameRaw = "Patient^Anonymous";
			studyPrototype.PatientsSex = "M";
			studyPrototype.StudyDateRaw = "20080220";
			studyPrototype.StudyDescription = "Test";
			studyPrototype.StudyId = "Test";

			SeriesData seriesPrototype = new SeriesData();
			seriesPrototype.SeriesDescription = "Series";
			seriesPrototype.ProtocolName = "Protocol";
			seriesPrototype.SeriesNumber = "1";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.SeriesDataPrototype = seriesPrototype;
			anonymizer.Anonymize(_file);

			AfterAnonymize(studyPrototype, seriesPrototype);

			//validate the adjusted dates.
			Assert.AreEqual("20080220", _anonymizedDateData.InstanceCreationDate, "Anonymized InstanceCreationDate doesn't match StudyDate");
			Assert.AreEqual("20080220", _anonymizedDateData.SeriesDate, "Anonymized SeriesDate doesn't match StudyDate");
			Assert.AreEqual("20080220", _anonymizedDateData.ContentDate, "Anonymized ContentDate doesn't match StudyDate");
			Assert.AreEqual("20080220100406", _anonymizedDateData.AcquisitionDatetime, "Anonymized AcquisitionDatetime doesn't match StudyDate/Time");
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidatePatientIdNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "123";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidatePatientIdNotEmpty()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientIdAllowEmpty()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEmptyPatientId;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidatePatientsNameNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "PATIENT^ANONYMOUS";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidatePatientsNameNotEmpty()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientsNameAllowEmpty()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEmptyPatientName;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidateAccessionNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.AccessionNumber].SetStringValue("1234");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.AccessionNumber = "1234";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidatePatientsBirthDateNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsBirthDate].SetStringValue("19760810");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsBirthDateRaw = "19760810";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientsBirthDateAllowEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsBirthDate].SetStringValue("19760810");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsBirthDateRaw = "19760810";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEqualBirthDate;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof (DicomAnonymizerValidationException))]
		public void TestValidateStudyIdNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.StudyId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.StudyId = "123";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void EnsureCorrectTagVr()
		{
			var failed = false;

			var list = DicomAnonymizer.UidTagsToRemap;
			foreach (var invalidTag in list.Select(DicomTagDictionary.GetDicomTag).Where(t => t.VR != DicomVr.UIvr))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} (VR={1}) is listed for RemappedUidTags", invalidTag, invalidTag.VR.Name));
			}

			list = DicomAnonymizer.DateTimeTagsToAdjust;
			foreach (var invalidTag in list.Select(DicomTagDictionary.GetDicomTag).Where(t => !(t.VR == DicomVr.DAvr || t.VR == DicomVr.DTvr || t.VR == DicomVr.TMvr)))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} (VR={1}) is listed for AdjustedDateTimeTags", invalidTag, invalidTag.VR.Name));
			}

			Assert.IsFalse(failed, "Test failed because one or more tags with the wrong VR are listed for actions requiring specific VR");
		}

		[Test]
		public void CheckSpecificTagActions()
		{
			// ContentSequence; // X (but required for key image support)
			// GraphicAnnotationSequence; // D (but required for presentation state support)
			// OverlayData; // X (but the overlay doesn't usually contain identifying information, and should really be supported through a clean pixels option)
			// ReferencedImageSequence; // X/Z/U*
			// ReferencedStudySequence; // X/Z
			// SourceImageSequence; // X/Z/U*
			var list = DicomAnonymizer.ListAllProcessedTags();

			Assert.False(list.Contains(DicomTags.ContentSequence), "ContentSequence should not be removed or nulled because it is used by key objects and SRs");
			Assert.False(list.Contains(DicomTags.GraphicAnnotationSequence), "GraphicAnnotationSequence should not be removed or nulled because it is used by presentation states");
			Assert.False(list.Contains(DicomTags.OverlayData), "OverlayData is not be removed or nulled because it should really be covered under a clean pixels option");
			Assert.False(list.Contains(DicomTags.ReferencedImageSequence), "ReferencedImageSequence should not be removed or nulled because the hierarchical relationships need to be kept");
			Assert.False(list.Contains(DicomTags.ReferencedStudySequence), "ReferencedStudySequence should not be removed or nulled because the hierarchical relationships need to be kept");
			Assert.False(list.Contains(DicomTags.SourceImageSequence), "SourceImageSequence should not be removed or nulled because the hierarchical relationships need to be kept");
		}

		[Test]
		public void PrintTagActions()
		{
			// prints a list of tags processed by the anonymizer, and the action taken, in a format suitable for easy comparison with the table in the standard
			var list = DicomAnonymizer.TagsToRemove.Select(t => new {Tag = t, Action = 'X', DicomTagDictionary.GetDicomTag(t).Name})
				.Union(DicomAnonymizer.TagsToNull.Select(t => new {Tag = t, Action = 'Z', DicomTagDictionary.GetDicomTag(t).Name}))
				.Union(DicomAnonymizer.UidTagsToRemap.Select(t => new {Tag = t, Action = 'U', DicomTagDictionary.GetDicomTag(t).Name}))
				.Union(DicomAnonymizer.DateTimeTagsToAdjust.Select(t => new {Tag = t, Action = 'D', DicomTagDictionary.GetDicomTag(t).Name}))
				.OrderBy(t => t.Name).ToList();

			foreach (var tag in list)
			{
				Debug.WriteLine(string.Format("{0,65} ({1:X4},{2:X4}) {3}", tag.Name.Trim(), (tag.Tag >> 16) & 0x0FFFF, tag.Tag & 0x0FFFF, tag.Action));
			}
		}

		[Test]
		public void PrintTagActions2()
		{
			// prints a list of tags processed by the anonymizer, and the action taken, in a CSV format suitable for spreadsheet applications
			// Hint: if you're using MS Excel, open the data through "Import Text" function (e.g. Data -> Import Text) to force the tag value column to Text mode
			var list = DicomAnonymizer.TagsToRemove.Select(t => new {Tag = t, Action = 'X', DicomTagDictionary.GetDicomTag(t).Name})
				.Union(DicomAnonymizer.TagsToNull.Select(t => new {Tag = t, Action = 'Z', DicomTagDictionary.GetDicomTag(t).Name}))
				.Union(DicomAnonymizer.UidTagsToRemap.Select(t => new {Tag = t, Action = 'U', DicomTagDictionary.GetDicomTag(t).Name}))
				.Union(DicomAnonymizer.DateTimeTagsToAdjust.Select(t => new {Tag = t, Action = 'D', DicomTagDictionary.GetDicomTag(t).Name}))
				.OrderBy(t => t.Name).ToList();

			foreach (var tag in list)
			{
				Debug.WriteLine(string.Format("\"{0}\",\"({1:X4},{2:X4})\",\"{3}\"", tag.Name.Trim(), (tag.Tag >> 16) & 0x0FFFF, tag.Tag & 0x0FFFF, tag.Action));
			}
		}

		[Test]
		public void EnsureUniqueTags()
		{
			var failed = false;

			var list = DicomAnonymizer.TagsToRemove;
			foreach (var duplicatedTag in list.GroupBy(t => t).SelectMany(t => t.Skip(1)).Select(DicomTagDictionary.GetDicomTag))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} is listed twice in RemovedTags", duplicatedTag));
			}

			list = DicomAnonymizer.TagsToNull;
			foreach (var duplicatedTag in list.GroupBy(t => t).SelectMany(t => t.Skip(1)).Select(DicomTagDictionary.GetDicomTag))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} is listed twice in NulledTags", duplicatedTag));
			}

			list = DicomAnonymizer.UidTagsToRemap;
			foreach (var duplicatedTag in list.GroupBy(t => t).SelectMany(t => t.Skip(1)).Select(DicomTagDictionary.GetDicomTag))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} is listed twice in RemappedUidTags", duplicatedTag));
			}

			list = DicomAnonymizer.DateTimeTagsToAdjust;
			foreach (var duplicatedTag in list.GroupBy(t => t).SelectMany(t => t.Skip(1)).Select(DicomTagDictionary.GetDicomTag))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} is listed twice in AdjustedDateTimeTags", duplicatedTag));
			}

			Assert.IsFalse(failed, "Test failed because one or more tags are duplicated in a single list");

			list = DicomAnonymizer.ListAllProcessedTags();
			foreach (var duplicatedTag in list.GroupBy(t => t).SelectMany(t => t.Skip(1)).Select(DicomTagDictionary.GetDicomTag))
			{
				failed = true;
				Debug.WriteLine(string.Format("The tag {0} is listed twice with different actions", duplicatedTag));
			}

			Assert.IsFalse(failed, "Test failed because one or more tags are duplicated with multiple conflicting actions");
		}

		[Test]
		public void TestFileMetaInformation()
		{
			Initialize();

			string oldUid = _file.DataSet[DicomTags.SopInstanceUid].ToString();

			StudyData studyPrototype = CreateStudyPrototype();

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);

			Assert.AreNotEqual(oldUid, _file.DataSet[DicomTags.SopInstanceUid].ToString(), "Patient Confidentiality Issue - SOP Instance Uid is not anonymized.");
			Assert.AreNotEqual(oldUid, _file.MetaInfo[DicomTags.MediaStorageSopInstanceUid].ToString(), "Patient Confidentiality Issue - Media Storage SOP Instance Uid is not anonymized.");
			Assert.AreNotEqual(oldUid, _file.MediaStorageSopInstanceUid, "Patient Confidentiality Issue - Media Storage SOP Instance Uid is not anonymized.");
			Assert.AreEqual(_file.DataSet[DicomTags.SopInstanceUid].ToString(), _file.MetaInfo[DicomTags.MediaStorageSopInstanceUid].ToString(), "MetaInfo Media Storage SOP Instance doesn't match DataSet SOP Instance.");
			Assert.AreEqual(_file.DataSet[DicomTags.SopClassUid].ToString(), _file.MetaInfo[DicomTags.MediaStorageSopClassUid].ToString(), "MetaInfo Media Storage SOP Class doesn't match DataSet SOP Class.");
		}

		[Test]
		public void TestRemappedReferencedSopUids()
		{
			// setup some files with unique "uids"
			DicomFile[] originals = new DicomFile[8];
			for (int n = 0; n < originals.Length; n++)
			{
				originals[n] = new DicomFile();
				originals[n].DataSet[DicomTags.StudyInstanceUid].SetStringValue((1000 + (n >> 2)).ToString());
				originals[n].DataSet[DicomTags.SeriesInstanceUid].SetStringValue((100 + (n >> 1)).ToString());
				originals[n].DataSet[DicomTags.SopInstanceUid].SetStringValue((10 + n).ToString());
				originals[n].DataSet[DicomTags.SopClassUid].SetStringValue((11111111111).ToString());
			}

			// setup up some cyclic and self references
			for (int n = 0; n < originals.Length; n++)
			{
				DicomSequenceItem sq;
				DicomFile n0File = originals[n];
				DicomFile n1File = originals[(n + 1)%originals.Length];
				DicomFile n2File = originals[(n + 2)%originals.Length];
				DicomFile n4File = originals[(n + 4)%originals.Length];

				n0File.DataSet[DicomTags.Uid].SetStringValue(n0File.DataSet[DicomTags.SopInstanceUid].ToString());
				n0File.DataSet[DicomTags.ReferencedSopInstanceUid].SetStringValue(n1File.DataSet[DicomTags.SopInstanceUid].ToString());

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(DicomUid.GenerateUid().UID);
				sq[DicomTags.TextString].SetStringValue("UID of something not in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n0File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of self");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n1File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n1File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n1File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of next file study in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n2File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 2nd next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n2File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 2nd next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n2File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 2nd next file study in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n4File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 4th next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n4File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 4th next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.Uid].SetStringValue(n4File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("UID of 4th next file study in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				sq[DicomTags.TextString].SetStringValue("Sop UID of something not in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n0File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Sop UID of self");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n1File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Sop UID of next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n2File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Sop UID of 2nd next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n4File.DataSet[DicomTags.SopInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Sop UID of 4th next file in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				sq[DicomTags.TextString].SetStringValue("Series UID of something not in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n0File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Series UID of self series");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n1File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Series UID of next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n2File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Series UID of 2nd next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n4File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Series UID of 4th next file series in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				sq[DicomTags.TextString].SetStringValue("Study UID of something not in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.StudyInstanceUid].SetStringValue(n0File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Study UID of self study");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.StudyInstanceUid].SetStringValue(n1File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Study UID of next file study in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.StudyInstanceUid].SetStringValue(n2File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Study UID of 2nd next file study in data set");

				n0File.DataSet[DicomTags.ContentSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.StudyInstanceUid].SetStringValue(n4File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.TextString].SetStringValue("Study UID of 4th next file study in data set");

				n0File.DataSet[DicomTags.ReferencedStudySequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.TextString].SetStringValue("A more typical hierarchical sop reference to self");
				sq[DicomTags.StudyInstanceUid].SetStringValue(n0File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.ReferencedSeriesSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n0File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.ReferencedSopSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n0File.DataSet[DicomTags.SopInstanceUid].ToString());

				n0File.DataSet[DicomTags.ReferencedStudySequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.TextString].SetStringValue("A more typical hierarchical sop reference to next");
				sq[DicomTags.StudyInstanceUid].SetStringValue(n1File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.ReferencedSeriesSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n1File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.ReferencedSopSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n1File.DataSet[DicomTags.SopInstanceUid].ToString());

				n0File.DataSet[DicomTags.ReferencedStudySequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.TextString].SetStringValue("A more typical hierarchical sop reference to 2nd next");
				sq[DicomTags.StudyInstanceUid].SetStringValue(n2File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.ReferencedSeriesSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n2File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.ReferencedSopSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n2File.DataSet[DicomTags.SopInstanceUid].ToString());

				n0File.DataSet[DicomTags.ReferencedStudySequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.TextString].SetStringValue("A more typical hierarchical sop reference to 4th next");
				sq[DicomTags.StudyInstanceUid].SetStringValue(n4File.DataSet[DicomTags.StudyInstanceUid].ToString());
				sq[DicomTags.ReferencedSeriesSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.SeriesInstanceUid].SetStringValue(n4File.DataSet[DicomTags.SeriesInstanceUid].ToString());
				sq[DicomTags.ReferencedSopSequence].AddSequenceItem(sq = new DicomSequenceItem());
				sq[DicomTags.ReferencedSopInstanceUid].SetStringValue(n4File.DataSet[DicomTags.SopInstanceUid].ToString());
			}

			// copy the originals and anonymize them
			DicomFile[] anonymized = new DicomFile[originals.Length];
			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;
			for (int n = 0; n < anonymized.Length; n++)
			{
				anonymized[n] = new DicomFile(string.Empty, originals[n].MetaInfo.Copy(true, true, true), originals[n].DataSet.Copy(true, true, true));
				anonymizer.Anonymize(anonymized[n]);
			}

			// generate validation dump strings - unique uids are mapped to sequential numbers
			string originalDumpValidationString = GenerateHierarchicalUidValidationDumpString(originals);
			string anonymizedDumpValidationString = GenerateHierarchicalUidValidationDumpString(anonymized);

			// if the hierarchical structure and pattern of uids is the same, then the relationships have been preserved.
			Assert.AreEqual(originalDumpValidationString, anonymizedDumpValidationString, "Relationships of anonymized data set differ compared to those of original data set.");
		}

		private static string GenerateHierarchicalUidValidationDumpString(DicomFile[] files)
		{
			Regex rgx = new Regex("[0-9A-Fa-f]{8}:UI=(\\[.*\\])");

			// generate a dump of all SQ and UI attributes in the original set
			StringBuilder sbOriginalDump = new StringBuilder();
			for (int n = 0; n < files.Length; n++)
			{
				sbOriginalDump.AppendLine("File " + n);
				sbOriginalDump.AppendLine(DumpHierarchicalDataSet(files[n].DataSet, 0, delegate(DicomAttribute a) { return a is DicomAttributeSQ || a is DicomAttributeUI; }));
			}

			// map unique UIDs to a sequential number
			Dictionary<string, int> uidValidationMap = new Dictionary<string, int>();
			int seed = 0;
			string validationDumpString = rgx.Replace(sbOriginalDump.ToString(),
			                                          delegate(Match m)
			                                          	{
			                                          		int value;
			                                          		string key = m.Groups[1].Value;
			                                          		if (string.IsNullOrEmpty(key))
			                                          			return m.Groups[0].Value;
			                                          		if (uidValidationMap.ContainsKey(key))
			                                          			value = uidValidationMap[key];
			                                          		else
			                                          			value = uidValidationMap[key] = seed++;
			                                          		return m.Groups[0].Value.Replace(key, value.ToString("X8"));
			                                          	});

			return validationDumpString;
		}

		private void Initialize()
		{
			_file = CreateTestFile();

			_originalUidData = new UidData();
			_originaStudyData = new StudyData();
			_originaSeriesData = new SeriesData();
			_originalDateData = new DateData();

			_file.DataSet.LoadDicomFields(_originalUidData);
			_file.DataSet.LoadDicomFields(_originaStudyData);
			_file.DataSet.LoadDicomFields(_originaSeriesData);
			_file.DataSet.LoadDicomFields(_originalDateData);
		}

		private void AfterAnonymize(StudyData studyPrototype, SeriesData seriesPrototype)
		{
			_anonymizedUidData = new UidData();
			_anonymizedStudyData = new StudyData();
			_anonymizedSeriesData = new SeriesData();
			_anonymizedDateData = new DateData();

			_file.DataSet.LoadDicomFields(_anonymizedUidData);
			_file.DataSet.LoadDicomFields(_anonymizedStudyData);
			_file.DataSet.LoadDicomFields(_anonymizedSeriesData);
			_file.DataSet.LoadDicomFields(_anonymizedDateData);

			ValidateRemovedTags(_file.DataSet);
			ValidateNulledAttributes(_file.DataSet);
			ValidateRemappedUids(_originalUidData, _anonymizedUidData);

			Assert.AreEqual(_anonymizedStudyData.PatientId, studyPrototype.PatientId);
			Assert.AreEqual(_anonymizedStudyData.PatientsNameRaw, studyPrototype.PatientsNameRaw);
			Assert.AreEqual(_anonymizedStudyData.PatientsBirthDateRaw, studyPrototype.PatientsBirthDateRaw);
			Assert.AreEqual(_anonymizedStudyData.PatientsSex, studyPrototype.PatientsSex);
			Assert.AreEqual(_anonymizedStudyData.AccessionNumber, studyPrototype.AccessionNumber);
			Assert.AreEqual(_anonymizedStudyData.StudyDateRaw, studyPrototype.StudyDateRaw);
			Assert.AreEqual(_anonymizedStudyData.StudyDescription, studyPrototype.StudyDescription);
			Assert.AreEqual(_anonymizedStudyData.StudyId, studyPrototype.StudyId);

			Assert.AreEqual(_anonymizedSeriesData.SeriesDescription, seriesPrototype.SeriesDescription);
			Assert.AreEqual(_anonymizedSeriesData.ProtocolName, seriesPrototype.ProtocolName);
			Assert.AreEqual(_anonymizedSeriesData.SeriesNumber, seriesPrototype.SeriesNumber);
		}

		private static StudyData CreateStudyPrototype()
		{
			StudyData studyPrototype = new StudyData();
			studyPrototype.AccessionNumber = "0x0A11BA5E";
			studyPrototype.PatientId = "216CA75";
			studyPrototype.PatientsBirthDate = DateTime.Now;
			studyPrototype.PatientsNameRaw = "PICARD^JEAN-LUC^^CPT.";
			studyPrototype.PatientsSex = "M";
			studyPrototype.StudyDate = DateTime.Now;
			studyPrototype.StudyDescription = "Description of a study prototype, anonymized";
			studyPrototype.StudyId = "STUDY158739";
			return studyPrototype;
		}

		private static void ValidateNullDates(DateData anonymizedDateData)
		{
			Assert.AreEqual(anonymizedDateData.InstanceCreationDate, "");
			Assert.AreEqual(anonymizedDateData.AcquisitionDatetime, "");
			Assert.AreEqual(anonymizedDateData.ContentDate, "");
			Assert.AreEqual(anonymizedDateData.SeriesDate, "");

			Trace.WriteLine("Validated Nulled Dates.");
		}

		private static void ValidateNulledAttributes(DicomAttributeCollection dataset)
		{
			//just test a couple
			Assert.AreEqual(dataset[DicomTags.StationName].ToString(), "");
			Assert.AreEqual(dataset[DicomTags.PatientComments].ToString(), "");

			Trace.WriteLine("Validated Nulled Attributes.");
		}

		private static void ValidateRemovedTags(DicomAttributeCollection dataset)
		{
			if (dataset.Contains(DicomTags.RequestAttributesSequence))
				throw new Exception("RequestAttributesSequence");

			DicomSequenceItem item = ((DicomSequenceItem[]) dataset[DicomTags.ReferencedImageSequence].Values)[0];
			if (item.Contains(DicomTags.RequestAttributesSequence))
				throw new Exception("RequestAttributesSequence");
		}

		private static void ValidateRemappedUids(UidData originalData, UidData anonymizedData)
		{
			if (originalData.StudyInstanceUid == anonymizedData.StudyInstanceUid)
				throw new Exception("StudyInstanceUid");

			if (originalData.SeriesInstanceUid == anonymizedData.SeriesInstanceUid)
				throw new Exception("SeriesInstanceUid");

			if (originalData.SopInstanceUid == anonymizedData.SopInstanceUid)
				throw new Exception("SopInstanceUid");

			if (!String.IsNullOrEmpty(originalData.ReferencedSopInstanceUid)
			    && originalData.ReferencedSopInstanceUid == anonymizedData.ReferencedSopInstanceUid)
				throw new Exception("ReferencedSopInstanceUid");

			if (!String.IsNullOrEmpty(originalData.FrameOfReferenceUid)
			    && originalData.FrameOfReferenceUid == anonymizedData.FrameOfReferenceUid)
				throw new Exception("FrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.SynchronizationFrameOfReferenceUid)
			    && originalData.SynchronizationFrameOfReferenceUid == anonymizedData.SynchronizationFrameOfReferenceUid)
				throw new Exception("SynchronizationFrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.Uid) && originalData.Uid == anonymizedData.Uid)
				throw new Exception("Uid");

			if (!String.IsNullOrEmpty(originalData.ReferencedFrameOfReferenceUid)
			    && originalData.ReferencedFrameOfReferenceUid == anonymizedData.ReferencedFrameOfReferenceUid)
				throw new Exception("ReferencedFrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.RelatedFrameOfReferenceUid)
			    && originalData.RelatedFrameOfReferenceUid == anonymizedData.RelatedFrameOfReferenceUid)
				throw new Exception("RelatedFrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.InstanceCreatorUid)
			    && originalData.InstanceCreatorUid == anonymizedData.InstanceCreatorUid)
				throw new Exception("InstanceCreatorUid");

			if (!String.IsNullOrEmpty(originalData.StorageMediaFileSetUid)
			    && originalData.StorageMediaFileSetUid == anonymizedData.StorageMediaFileSetUid)
				throw new Exception("StorageMediaFileSetUid");

			if (!String.IsNullOrEmpty(originalData.ReferencedImageSequenceInstanceCreatorUid)
			    && originalData.ReferencedImageSequenceInstanceCreatorUid == anonymizedData.ReferencedImageSequenceInstanceCreatorUid)
				throw new Exception("ReferencedImageSequence > InstanceCreatorUid");

			Trace.WriteLine("Validated Remapped Uids.");
		}

		#region Hierarchical Dump Code

		private static string DumpHierarchicalDataSet(DicomAttributeCollection dataset)
		{
			return DumpHierarchicalDataSet(dataset, 0, null);
		}

		private static string DumpHierarchicalDataSet(DicomAttributeCollection dataset, Predicate<DicomAttribute> filter)
		{
			return DumpHierarchicalDataSet(dataset, 0, filter);
		}

		private static string DumpHierarchicalDataSet(DicomAttributeCollection dataset, int level, Predicate<DicomAttribute> filter)
		{
			StringBuilder sb = new StringBuilder();
			string prefix = string.Join(">", new string[level + 1]);
			foreach (DicomAttribute attribute in dataset)
			{
				if (filter != null && !filter(attribute))
					continue;

				if (attribute is DicomAttributeSQ)
				{
					sb.AppendFormat("{0}{1}:SQ={2} items", prefix, attribute.Tag.HexString, attribute.Count);
					sb.AppendLine();
					int counter = 0;
					foreach (DicomSequenceItem item in ((DicomSequenceItem[]) attribute.Values))
					{
						sb.AppendFormat("{0}{1} SQ Item #{2}", prefix, attribute.Tag.HexString, counter++);
						sb.AppendLine();
						sb.AppendLine(DumpHierarchicalDataSet(item, level + 1, filter));
					}
				}
				else
				{
					sb.AppendFormat("{0}{1}:{3}=[{2}]", prefix, attribute.Tag.HexString, attribute.ToString(), attribute.Tag.VR.Name);
					sb.AppendLine();
				}
			}
			return sb.ToString().TrimEnd();
		}

		#endregion
	}
}

#endif