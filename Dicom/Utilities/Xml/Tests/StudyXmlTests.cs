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
using System.Text;
using System.Xml;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;
using System.Diagnostics;

namespace ClearCanvas.Dicom.Utilities.Xml.Tests
{
	[TestFixture]
	public class StudyXmlTests : AbstractTest
	{
		private readonly string A = "Short Image Comments A";
		private readonly string B = "Short Image Comments B";
		private readonly string X = null;
		private readonly string L = CreateLongString(1050);

		private readonly List<string[]> _overlappingTagTestSymbols = new List<string[]>();
		private readonly List<string[]> _overlappingTagTestSets = new List<string[]>();

		public StudyXmlTests()
		{
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			CreateOverlappingTagTestSets();
		}

		private void CreateOverlappingTagTestSets()
		{
			string[] symbols = new string[] { "A", "B", "X", "L" };
			string[] values = new string[] { A, B, X, L };

			for (int i = 0; i < values.Length; ++i)
			{
				for (int j = 0; j < values.Length; ++j)
				{
					for (int k = 0; k < values.Length; ++k)
					{
						_overlappingTagTestSymbols.Add(new string[] { symbols[i], symbols[j], symbols[k] });
						//NOTE: some tests effectively get duplicated here, but it's much easier this way.
						_overlappingTagTestSets.Add(new string[] { values[i], values[j], values[k] });
					}
				}
			}
		}

		[Test]
		public void TestSopClass()
		{
			List<DicomFile> images = SetupImages(4);

			string seriesUid = images[0].DataSet[DicomTags.SeriesInstanceUid].ToString();

			images[0].MediaStorageSopClassUid = SopClass.EnhancedCtImageStorageUid;
			images[1].MediaStorageSopClassUid = SopClass.EnhancedMrImageStorageUid;
			images[2].MediaStorageSopClassUid = SopClass.EnhancedSrStorageUid;
			images[3].MediaStorageSopClassUid = SopClass.EnhancedXaImageStorageUid;
		
			StudyXml xml = new StudyXml();
			foreach (DicomFile file in images)
				xml.AddFile(file);
	
			var doc = xml.GetMemento(new StudyXmlOutputSettings());

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedCtImageStorageUid);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedMrImageStorageUid);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedSrStorageUid);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedXaImageStorageUid);

			xml = new StudyXml();
			xml.SetMemento(doc);

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedCtImageStorageUid);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedMrImageStorageUid);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedSrStorageUid);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].SopClass.Uid, SopClass.EnhancedXaImageStorageUid);
		}

		[Test]
		public void TestTransferSyntax()
		{
			List<DicomFile> images = SetupImages(4);

			string seriesUid = images[0].DataSet[DicomTags.SeriesInstanceUid].ToString();

			images[0].TransferSyntax = TransferSyntax.Jpeg2000ImageCompression;
			images[1].TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
			images[2].TransferSyntax = TransferSyntax.ExplicitVrBigEndian;
			images[3].TransferSyntax = TransferSyntax.Jpeg2000ImageCompressionLosslessOnly;

			StudyXml xml = new StudyXml();
			foreach (DicomFile file in images)
				xml.AddFile(file);

			var doc = xml.GetMemento(new StudyXmlOutputSettings());

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompression);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrBigEndian);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);

			xml = new StudyXml();
			xml.SetMemento(doc);

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompression);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrBigEndian);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
		}

		[Test]
		public void TestChangeTransferSyntax()
		{
			List<DicomFile> images = SetupImages(4);
			foreach (DicomFile file in images)
				file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			string seriesUid = images[0].DataSet[DicomTags.SeriesInstanceUid].ToString();

			StudyXml xml = new StudyXml();
			foreach (DicomFile file in images)
				xml.AddFile(file);

			var doc = xml.GetMemento(new StudyXmlOutputSettings());

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);

			xml = new StudyXml();
			xml.SetMemento(doc);

			images[1].TransferSyntax = TransferSyntax.Jpeg2000ImageCompressionLosslessOnly;
			images[3].TransferSyntax = TransferSyntax.Jpeg2000ImageCompressionLosslessOnly;

			xml.AddFile(images[1]);
			xml.AddFile(images[3]);

			doc = xml.GetMemento(new StudyXmlOutputSettings());
			xml = new StudyXml();
			xml.SetMemento(doc);

			Assert.AreEqual(xml[seriesUid][images[0].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[1].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
			Assert.AreEqual(xml[seriesUid][images[2].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.ExplicitVrLittleEndian);
			Assert.AreEqual(xml[seriesUid][images[3].DataSet[DicomTags.SopInstanceUid]].TransferSyntax, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
		}

		[Test]
		public void TestEqualAfterSerialization()
		{
			foreach (string[] testSet in _overlappingTagTestSets)
			{
				List<DicomFile> images = SetupImages(testSet.Length);
				SetTestAttribute(images, testSet);

				StudyXml xml = new StudyXml();

				foreach (DicomFile file in images)
					xml.AddFile(file);

				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				settings.MaxTagLength = 1024;
				var doc = xml.GetMemento(settings);

				List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
				int i = 0;
				foreach (DicomFile file in images)
					ValidateEqualExceptExclusions(file.DataSet, dataSets[i++], DicomTags.ImageComments, DicomTags.PixelData);

				xml = new StudyXml();
				xml.SetMemento(doc);

				dataSets = GetInstanceXmlDataSets(xml);
				i = 0;
				foreach (DicomFile file in images)
					ValidateEqualExceptExclusions(file.DataSet, dataSets[i++], DicomTags.ImageComments, DicomTags.PixelData);
			}
		}

		[Test]
		public void TestExclusionsImmediatelyAfterSerialization()
		{
			//NOTE: previously, this test failed because the excluded tags were not added to the
			//xml collection until after it had been deserialized at least once from the xml.
			foreach (string[] testSet in _overlappingTagTestSets)
			{
				List<DicomFile> images = SetupImages(testSet.Length);
				SetTestAttribute(images, testSet);

				StudyXml xml = new StudyXml();

				foreach (DicomFile file in images)
					xml.AddFile(file);

				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				settings.MaxTagLength = 1024;
				var doc = xml.GetMemento(settings);

				List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
				ValidateSimpleDataSets(testSet, dataSets, settings);

				//do a little extra validation, what the hay.
				xml = new StudyXml();
				xml.SetMemento(doc);

				dataSets = GetInstanceXmlDataSets(xml);
				ValidateSimpleDataSets(testSet, dataSets, settings);
			}
		}

		[Test]
		public void TestBaseInstanceTagsPastEnd()
		{
			//NOTE: previously, this test failed because, during xml serialization, only the
			//instance's own attributes were iterated over; if there were tags in the base instance
			//that went past the end of the individual instances, no 'EmptyAttributes' got added
			//to the xml and there would be extra attributes in the instances on deserialization.
			List<DicomFile> images = SetupImages(2);
			DicomFile smallFile = new DicomFile(null);
			images.Add(smallFile);
			base.SetupMetaInfo(smallFile);

			DicomAttributeCollection theSet = smallFile.DataSet;
			theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
			theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
			theSet[DicomTags.InstanceCreationDate].SetStringValue("20070618");
			theSet[DicomTags.InstanceCreationTime].SetStringValue("133600");
			theSet[DicomTags.SopClassUid].SetStringValue(SopClass.MrImageStorageUid);
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.StudyDate].SetStringValue("20070618");
			theSet[DicomTags.StudyTime].SetStringValue("133600");
			theSet[DicomTags.SeriesDate].SetStringValue("20070618");
			theSet[DicomTags.SeriesTime].SetStringValue("133700");
			theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
			theSet[DicomTags.Modality].SetStringValue("MR");
			theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.ManufacturersModelName].SetNullValue();
			theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
			theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
			theSet[DicomTags.StudyDescription].SetStringValue("HEART");
			theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");

			theSet[DicomTags.StudyInstanceUid].SetStringValue(images[0].DataSet[DicomTags.StudyInstanceUid].ToString());
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(images[0].DataSet[DicomTags.SeriesInstanceUid].ToString());
			theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().ToString());

			StudyXml xml = new StudyXml();

			foreach (DicomFile file in images)
				xml.AddFile(file);

			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			var doc = xml.GetMemento(settings);

			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
			int i = 0;
			foreach (DicomFile file in images)
				ValidateEqualExceptExclusions(file.DataSet, dataSets[i++], DicomTags.ImageComments, DicomTags.PixelData);

			xml = new StudyXml();
			xml.SetMemento(doc);

			dataSets = GetInstanceXmlDataSets(xml);
			i = 0;
			foreach (DicomFile file in images)
				ValidateEqualExceptExclusions(file.DataSet, dataSets[i++], DicomTags.ImageComments, DicomTags.PixelData);
		}

		[Test]
		public void TestExcludePrivateTags()
		{
			List<DicomFile> images = SetupImages(2);
			StudyXml xml = new StudyXml();
			DicomTag privateTag =
				new DicomTag(0x00210010, "Private Tag", "Private Tag", DicomVr.LTvr, false, 1, uint.MaxValue, false);
			
			foreach (DicomFile file in images)
			{
				file.DataSet[privateTag].SetStringValue("My Private Tag");
				xml.AddFile(file);
			}

			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			var doc = xml.GetMemento(settings);

			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
			foreach (InstanceXmlDicomAttributeCollection dataSet in dataSets)
				Assert.IsFalse(dataSet.Contains(privateTag));

			xml = new StudyXml();
			xml.SetMemento(doc);

			dataSets = GetInstanceXmlDataSets(xml);
			foreach (InstanceXmlDicomAttributeCollection dataSet in dataSets)
				Assert.IsFalse(dataSet.Contains(privateTag));
		}

		[Test]
		public void TestExcludeBinaryTags()
		{
			List<DicomFile> images = SetupImages(3);
			images[2].DataSet[DicomTags.SpectroscopyData].Values = new float[6];

			StudyXml xml = new StudyXml();
			foreach (DicomFile file in images)
			{
				file.DataSet[DicomTags.RedPaletteColorLookupTableData].Values = new byte[256];
				file.DataSet[DicomTags.GreenPaletteColorLookupTableData].Values = new byte[256];
				file.DataSet[DicomTags.BluePaletteColorLookupTableData].Values = new byte[256];
				xml.AddFile(file);
			}

			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			settings.MaxTagLength = 100;
			var doc = xml.GetMemento(settings);

			//SaveStudyXml(doc, @"C:\stewart\testxml.xml");
			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
			foreach (InstanceXmlDicomAttributeCollection dataSet in dataSets)
			{
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.RedPaletteColorLookupTableData));
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.GreenPaletteColorLookupTableData));
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.BluePaletteColorLookupTableData));
			}

			//This attribute has a short value, so it should not be excluded.
			Assert.IsFalse(dataSets[2].IsTagExcluded(DicomTags.SpectroscopyData));

			xml = new StudyXml();
			xml.SetMemento(doc);

			dataSets = GetInstanceXmlDataSets(xml);
			foreach (InstanceXmlDicomAttributeCollection dataSet in dataSets)
			{
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.RedPaletteColorLookupTableData));
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.GreenPaletteColorLookupTableData));
				Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.BluePaletteColorLookupTableData));
			}

			//This attribute has a short value, so it should not be excluded.
			Assert.IsFalse(dataSets[2].IsTagExcluded(DicomTags.SpectroscopyData));
		}

		[Test]
		public void TestMultipleSerializations()
		{
			List<DicomFile> images = SetupImages(4);

			StudyXmlMemento doc = null;
			StudyXml xml;
			int i;
			for(i = 0; i < images.Count; ++i)
			{
				xml = new StudyXml();

				if (doc != null)
					xml.SetMemento(doc);

				xml.AddFile(images[i]);
				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				doc = xml.GetMemento(settings);
			}

			xml = new StudyXml();
			xml.SetMemento(doc);
			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(xml);
			i = 0;
			foreach (DicomFile file in images)
				ValidateEqualExceptExclusions(file.DataSet, dataSets[i++], DicomTags.ImageComments, DicomTags.PixelData);
		}

		#region Simple Tag Tests

		[Test]
		public void TestOverlappingTagSimple()
		{
			int i = 0;
			foreach (string[] testSet in _overlappingTagTestSets)
			{
				string[] symbols = _overlappingTagTestSymbols[i++];

				try
				{
					TestOverlappingTag(testSet, false); //no exclusion
				}
				catch
				{
					Trace.WriteLine(String.Format("Test {0}{1}{2} (NO exclusion) failed!", symbols[0], symbols[1], symbols[2]));
					throw;
				}

				try
				{
					TestOverlappingTag(testSet, true); //no exclusion
				}
				catch
				{
					Trace.WriteLine(String.Format("Test {0}{1}{2} (WITH exclusion) failed!", symbols[0], symbols[1], symbols[2]));
					throw;
				}
			}
		}

		//[Test]
		public void TestOverlappingTagPreviousFailures()
		{
			string[] values;

			//This test failed because A was serialized to the base instance, but the fact that L
			//was excluded was not serialized.
			//values = new string[] { A, A, L };
			//TestSameTag(values, true);

			//This test failed because B was serialized to the base instance, but the fact that L
			//was excluded was not serialized.
			//values = new string[] { B, B, L };
			//TestSameTag(values, true);

			//this test failed because the excluded attribute got serialized to the base instance xml,
			//but then was not re-added to the instances upon deserialization.  A gets serialized to
			//the third instance properly.
			//values = new string[] { L, L, A };
			//TestSameTag(values, true);

			//this test failed for the same reason as previous.
			//values = new string[] { L, L, B };
			//TestSameTag(values, true);

			//failed for similar reason as previous; X (empty) does not get serialized.
			values = new string[] { L, L, X };
			TestOverlappingTag(values, true);

			//failed for similar reason as previous; exclusion serialzed to base instance, but not
			//deserialized properly.
			//values = new string[] { L, L, L };
			//TestSameTag(values, true);
		}

		private void TestOverlappingTag(string[] values, bool setMaxTagLength)
		{
			List<DicomFile> images = SetupImages(values.Length);
			SetTestAttribute(images, values);

			StudyXml newStudyXml;
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			if (setMaxTagLength)
				settings.MaxTagLength = 1024;

			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(images, out newStudyXml, settings);

			ValidateSimpleDataSets(values, dataSets, settings);
		}

		private void SetTestAttribute(List<DicomFile> files, string[] values)
		{
			SetTestAttribute(files, values, DicomTags.ImageComments);
		}

		private void SetTestAttribute(List<DicomFile> files, string[] values, uint tag)
		{
			if (values.Length != files.Count)
				throw new ArgumentException("Test value and files lists must have same number of values.");

			for (int i = 0; i < values.Length; ++i)
			{
				if (values[i] == null)
					files[i].DataSet[tag] = null;
				else
					files[i].DataSet[tag].SetStringValue(values[i]);
			}
		}

		private void ValidateEqualExceptExclusions(DicomAttributeCollection collection1, DicomAttributeCollection collection2, params uint[] ignoreTags)
		{
			IEnumerator<DicomAttribute> iterator1 = collection1.GetEnumerator();
			IEnumerator<DicomAttribute> iterator2 = collection2.GetEnumerator();

			while(iterator1.MoveNext())
			{
				bool ignored = false;
				foreach (uint ignoreTag in ignoreTags)
				{
					if (ignoreTag == iterator1.Current.Tag.TagValue)
					{
						ignored = true;
						break;
					}
				}

				if (ignored)
					continue;

				DicomAttribute attribute = iterator1.Current;

				if (collection1 is InstanceXmlDicomAttributeCollection)
				{
					if (((InstanceXmlDicomAttributeCollection)collection1).ExcludedTags.Contains(attribute.Tag))
						continue;
				}

				if (collection2 is InstanceXmlDicomAttributeCollection)
				{
					if (((InstanceXmlDicomAttributeCollection)collection2).ExcludedTags.Contains(attribute.Tag))
						continue;
				}

				while (true)
				{
					if (!iterator2.MoveNext())
						throw new Exception("Tag exists in collection 1, but not collection 2.");
					else if (iterator2.Current.Tag == iterator1.Current.Tag)
						break;
					else
					{
						ignored = false;
						foreach (uint ignoreTag in ignoreTags)
						{
							if (ignoreTag == iterator2.Current.Tag.TagValue)
							{
								ignored = true;
								break;
							}
						}

						if (ignored)
							continue;
						else if (!iterator2.Current.IsEmpty)
							throw new Exception("Tag exists in collection 2, but not collection 1.");
					}
				}

				DicomAttribute attribute2 = iterator2.Current;
				if (attribute2.IsEmpty && attribute.IsEmpty)
					continue;
				
				if (!attribute.Equals(attribute2))
					throw new Exception("Attributes should be equal.");
			}

			while(iterator2.MoveNext())
			{
				bool ignored = false;
				foreach (uint ignoreTag in ignoreTags)
				{
					if (ignoreTag == iterator2.Current.Tag.TagValue)
					{
						ignored = true;
						break;
					}
				}

				if (ignored)
					continue;
				else if (!iterator2.Current.IsEmpty)
					throw new Exception("Tag exists in collection 2, but not collection 1.");
			}
		}

		private void ValidateSimpleDataSets(string[] values, List<InstanceXmlDicomAttributeCollection> dataSets, StudyXmlOutputSettings settings)
		{
			if (values.Length != dataSets.Count)
				throw new ArgumentException("Test value and data set lists must have same number of values.");

			for (int i = 0; i < dataSets.Count; ++i)
			{
				string originalValue = values[i];
				InstanceXmlDicomAttributeCollection dataSet = dataSets[i];

				if (originalValue == null)
				{
					Assert.IsFalse(dataSet.IsTagExcluded(DicomTags.ImageComments), "Tag should NOT be excluded.");
					Assert.IsTrue(dataSet[DicomTags.ImageComments].IsEmpty, "Tag should be empty.");
				}
				else if (originalValue.Length > (int)settings.MaxTagLength)
				{
					Assert.IsTrue(dataSet.IsTagExcluded(DicomTags.ImageComments), "Tag should be excluded.");
					Assert.AreNotEqual(dataSet[DicomTags.ImageComments].ToString(), originalValue, "Value doesn't match original.");
				}
				else
				{
					Assert.IsFalse(dataSet.IsTagExcluded(DicomTags.ImageComments), "Tag should not be excluded.");
					Assert.AreEqual(dataSet[DicomTags.ImageComments].ToString(), originalValue, "Value doesn't match original.");
				}
			}
		}

		#endregion

		#region Sequence Tests

		private enum SqId
		{
			A,
			B,
			X
		}

		[Test]
		public void TestOverlappingSequence()
		{
			string[] symbols = new string[] { "A", "B", "X" };
			SqId[] allValues = new SqId[] { SqId.A, SqId.B, SqId.X };

			for (int i = 0; i < allValues.Length; ++i)
			{
				for (int j = 0; j < allValues.Length; ++j)
				{
					for (int k = 0; k < allValues.Length; ++k)
					{
						//NOTE: some tests effectively get duplicated here, but it's much easier this way.
						SqId[] test = new SqId[] { allValues[i], allValues[j], allValues[k] };

						try
						{
							TestOverlappingSequence(test, false); //no exclusion
						}
						catch
						{
							Trace.WriteLine(String.Format("Test {0}{1}{2} (NO exclusion) failed!", symbols[i], symbols[j], symbols[k]));
							throw;
						}

						try
						{
							TestOverlappingSequence(test, true); //no exclusion
						}
						catch
						{
							Trace.WriteLine(String.Format("Test {0}{1}{2} (WITH exclusion) failed!", symbols[i], symbols[j], symbols[k]));
							throw;
						}
					}
				}
			}
		}

		//[Test]
		public void TestOverlappingSequencePreviousFailure()
		{
			SqId[] test;

			//many of the tests failed because sequence items were not of type InstanceXmlDicomSequenceItem on
			//deserialization and hence can't have any excluded tags.  (InstanceXmlDicomSequenceItem was
			//missing some copy functionality).
			test = new SqId[] { SqId.A, SqId.A, SqId.B };
			TestOverlappingSequence(test, true);
		}

		private void TestOverlappingSequence(SqId[] sequenceIds, bool setMaxTagLength)
		{
			List<DicomFile> images = SetupImagesWithVoiLutSequenceA(sequenceIds.Length);

			for (int i = 0; i < sequenceIds.Length; ++i)
			{
				DicomFile image = images[i];
				SqId sequenceId = sequenceIds[i];
				if (sequenceId == SqId.X)
					image.DataSet[DicomTags.VoiLutSequence] = null;
				else if (sequenceId == SqId.B)
					MakeVoiLutSequenceB(image.DataSet);
			}

			StudyXml newStudyXml;
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			if (setMaxTagLength)
				settings.MaxTagLength = 1024;

			List<InstanceXmlDicomAttributeCollection> dataSets = GetInstanceXmlDataSets(images, out newStudyXml, settings);

			for (int i = 0; i < dataSets.Count; ++i)
			{
				InstanceXmlDicomAttributeCollection dataSet = dataSets[i];
				SqId sequenceId = sequenceIds[i];

				DicomAttribute sequence = dataSets[i][DicomTags.VoiLutSequence];
				DicomSequenceItem[] items = (DicomSequenceItem[])sequence.Values;

				if (sequenceId == SqId.X)
				{
					Assert.IsTrue(sequence.IsEmpty, "Tag should be empty.");
					Assert.IsFalse(dataSet.HasExcludedTags(true), "There should be no excluded tags.");
				}
				else
				{
					Assert.IsFalse(sequence.IsEmpty, "Tag should NOT be empty.");
					if (!setMaxTagLength)
					{
						Assert.AreEqual(sequence, images[i].DataSet[DicomTags.VoiLutSequence], "Tag doesn't match original.");
						Assert.IsFalse(dataSet.HasExcludedTags(true), "There should be no excluded tags.");
					}
					else
					{
						foreach (InstanceXmlDicomSequenceItem item in items)
							Assert.IsTrue(item.IsTagExcluded(DicomTags.LutData), "Tag should be excluded.");
					}

					if (sequenceId == SqId.B)
						Assert.AreEqual(items.Length, 2, "Sequence length should be 2.");
					else
						Assert.AreEqual(items.Length, 3, "Sequence length should be 3.");
				}
			}
		}
		
		#endregion

		[Test]
		public void TestSameSequenceWithExclusionsMultipleSerializations()
		{
			List<DicomFile> images = SetupImagesWithVoiLutSequenceA(3);

			StudyXml newStudyXml;
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			settings.MaxTagLength = 1024;

			List<DicomFile> first2Images = new List<DicomFile>();
			first2Images.AddRange(images.GetRange(0, 2));

			List<InstanceXmlDicomAttributeCollection> dataSets1 = GetInstanceXmlDataSets(first2Images, out newStudyXml, settings);
			for (int i = 0; i < first2Images.Count; ++i)
			{
				DicomAttributeSQ sequence = (DicomAttributeSQ)dataSets1[i][DicomTags.VoiLutSequence];
				for (int j = 0; j < sequence.Count; ++j)
					Assert.IsTrue(((InstanceXmlDicomSequenceItem)sequence[j]).ExcludedTags[0].TagValue == DicomTags.LutData);
			}

			Assert.AreEqual(dataSets1[0][DicomTags.VoiLutSequence], dataSets1[1][DicomTags.VoiLutSequence]);

			newStudyXml.AddFile(images[2]);
			var document = newStudyXml.GetMemento(settings);
			//SaveStudyXml(document, @"c:\stewart\studyxml3.xml");

			newStudyXml = new StudyXml();
			newStudyXml.SetMemento(document);

			List<InstanceXmlDicomAttributeCollection> dataSets2 = GetInstanceXmlDataSets(newStudyXml);
			for (int i = 0; i < first2Images.Count; ++i)
			{
				DicomAttributeSQ sequence = (DicomAttributeSQ)dataSets2[i][DicomTags.VoiLutSequence];
				for (int j = 0; j < sequence.Count; ++j)
					Assert.IsTrue(((InstanceXmlDicomSequenceItem)sequence[j]).ExcludedTags[0].TagValue == DicomTags.LutData);
			}

			Assert.AreEqual(dataSets1[0][DicomTags.VoiLutSequence], dataSets2[0][DicomTags.VoiLutSequence]);
			Assert.AreEqual(dataSets1[1][DicomTags.VoiLutSequence], dataSets2[1][DicomTags.VoiLutSequence]);

			Assert.AreEqual(dataSets2[0][DicomTags.VoiLutSequence], dataSets2[1][DicomTags.VoiLutSequence]);
			Assert.AreEqual(dataSets2[0][DicomTags.VoiLutSequence], dataSets2[2][DicomTags.VoiLutSequence]);
		}

		[Test]
		public void TestBaseInstanceExclusionAfterSerialization()
		{
			foreach (string[] testSet in _overlappingTagTestSets)
			{
				List<DicomFile> images = SetupImages(testSet.Length);
				SetTestAttribute(images, testSet);

				StudyXml xml = new StudyXml();

				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				xml = new StudyXml();

				xml.AddFile(images[0]);
				xml.AddFile(images[1]);

				var doc = xml.GetMemento(settings);

				settings.MaxTagLength = 1024;

				xml.AddFile(images[2]); //re-add
				doc = xml.GetMemento(settings);

				xml = new StudyXml();
				xml.SetMemento(doc);
				doc = xml.GetMemento(settings);
				xml.AddFile(images[2]); //re-add
				doc = xml.GetMemento(settings);

				xml = new StudyXml();
				xml.SetMemento(doc);
				xml.AddFile(images[1]); //re-add
				doc = xml.GetMemento(settings);
			}
		}

		private List<DicomFile> SetupImagesWithVoiLutSequenceA(int number)
		{
			List<DicomFile> images = SetupImages(number);

			for (int i = 0; i < images.Count; ++i)
			{
				DicomSequenceItem item = new DicomSequenceItem();
				images[i].DataSet[DicomTags.VoiLutSequence].AddSequenceItem(item);

				item[DicomTags.LutDescriptor].SetStringValue(@"16384\0\16");
				item[DicomTags.LutExplanation].SetStringValue("NORMAL");
				item[DicomTags.LutData].Values = GetLutData(1);

				item = new DicomSequenceItem();
				images[i].DataSet[DicomTags.VoiLutSequence].AddSequenceItem(item);

				item[DicomTags.LutDescriptor].SetStringValue(@"16384\0\16");
				item[DicomTags.LutExplanation].SetStringValue("HARDER");
				item[DicomTags.LutData].Values = GetLutData(2);

				item = new DicomSequenceItem();
				images[i].DataSet[DicomTags.VoiLutSequence].AddSequenceItem(item);

				item[DicomTags.LutDescriptor].SetStringValue(@"16384\0\16");
				item[DicomTags.LutExplanation].SetStringValue("SOFTER");
				item[DicomTags.LutData].Values = GetLutData(3);
			}

			return images;
		}

		private static void MakeVoiLutSequenceB(DicomAttributeCollection dataSet)
		{
			DicomAttribute sequence = dataSet[DicomTags.VoiLutSequence];
			DicomSequenceItem[] items = (DicomSequenceItem[])sequence.Values;
			DicomSequenceItem[] newItems = new DicomSequenceItem[items.Length - 1];
			newItems[0] = items[0];
			newItems[1] = items[2];
			sequence.Values = newItems;
		}

		private List<DicomFile> SetupImages(int number)
		{
			List<DicomFile> images = new List<DicomFile>();

			DicomFile image1 = new DicomFile();
			SetupMR(image1.DataSet);
			SetupMetaInfo(image1);
			string studyInstanceUid = image1.DataSet[DicomTags.StudyInstanceUid].ToString();
			string seriesInstanceUid = image1.DataSet[DicomTags.SeriesInstanceUid].ToString();
			image1.DataSet[DicomTags.SopInstanceUid].SetStringValue("1");

			images.Add(image1);

			for (int i = 1; i < number; ++i)
			{
				DicomFile image = new DicomFile();
				SetupMetaInfo(image);
				SetupMR(image.DataSet);
				image.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				image.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				image.DataSet[DicomTags.SopInstanceUid].SetStringValue((i+1).ToString());

				images.Add(image);
			}

			return images;
		}

		private static List<InstanceXmlDicomAttributeCollection> GetInstanceXmlDataSets(IEnumerable<DicomFile> images, out StudyXml newStudyXml, StudyXmlOutputSettings settings)
		{
			StudyXml xml = new StudyXml();
			foreach (DicomFile image in images)
				xml.AddFile(image);

			var doc = xml.GetMemento(settings);
			//SaveStudyXml(doc, @"c:\stewart\LastStudyXml.xml");

			newStudyXml = new StudyXml();
			newStudyXml.SetMemento(doc);

			doc = newStudyXml.GetMemento(settings);
			//SaveStudyXml(doc, @"c:\stewart\LastStudyXml2.xml");

			return GetInstanceXmlDataSets(newStudyXml);
		}

		private static List<InstanceXmlDicomAttributeCollection> GetInstanceXmlDataSets(StudyXml xml)
		{
			List<InstanceXmlDicomAttributeCollection> dataSets = new List<InstanceXmlDicomAttributeCollection>();
			foreach (SeriesXml series in xml)
			{
				foreach (InstanceXml instanceXml in series)
				{
					dataSets.Add((InstanceXmlDicomAttributeCollection)instanceXml.Collection);
				}
			}

			return dataSets;
		}

		private static void SaveStudyXml(XmlDocument doc, string fileName)
		{
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 3;
				doc.Save(writer);
			}
		}

		private static ushort[] GetLutData(ushort values)
		{
			ushort[] data = new ushort[16384];
			for (int i = 0; i < data.Length; ++i)
				data[i] = values;

			return data;
		}

		private static string CreateLongString(int minLength)
		{
			StringBuilder builder = new StringBuilder();
			while(builder.Length < minLength)
				builder.Append("test");

			return builder.ToString();
		}
	}
}

#endif