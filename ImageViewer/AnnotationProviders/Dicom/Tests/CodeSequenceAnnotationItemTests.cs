#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	[TestFixture]
	internal class CodeSequenceAnnotationItemTests
	{
		[Test]
		public void TestFormatNil()
		{
			var dataset = new DicomAttributeCollection();
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "1");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "2");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "3");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "4");

			dataset[DicomTags.PatientBreedCodeSequence].SetNullValue();
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "5");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "6");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "7");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "8");

			dataset[DicomTags.PatientBreedDescription].SetNullValue();
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "9");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "10");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "11");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "12");

			dataset[DicomTags.PatientBreedCodeSequence].SetEmptyValue();
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "13");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "14");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "15");
			Assert.IsEmpty(CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "16");
		}

		[Test]
		public void TestFormatNoDescription()
		{
			var item0 = new DicomSequenceItem();
			item0[DicomTags.CodeValue].SetStringValue("CODEVALUE");
			item0[DicomTags.CodeMeaning].SetStringValue("Code Meaning");
			item0[DicomTags.CodingSchemeDesignator].SetStringValue("DCM");

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item0);
			dataset[DicomTags.PatientBreedDescription].SetNullValue();

			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "1");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "2");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "3");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "4");

			item0[DicomTags.CodeMeaning].SetEmptyValue();
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "5");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "6");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "7");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "8");
		}

		[Test]
		public void TestFormatWithDescription()
		{
			var item0 = new DicomSequenceItem();
			item0[DicomTags.CodeValue].SetStringValue("CODEVALUE");
			item0[DicomTags.CodeMeaning].SetStringValue("Code Meaning");
			item0[DicomTags.CodingSchemeDesignator].SetStringValue("DCM");

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item0);
			dataset[DicomTags.PatientBreedDescription].SetStringValue("Description");

			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "1");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "2");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "3");
			Assert.AreEqual("Code Meaning", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "4");

			item0[DicomTags.CodeMeaning].SetEmptyValue();
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "5");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "6");
			Assert.AreEqual("Description", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "7");
			Assert.AreEqual("Description", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "8");

			dataset[DicomTags.PatientBreedDescription].SetNullValue();
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "9");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "10");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "11");
			Assert.AreEqual("CODEVALUE (DCM)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "12");
		}

		[Test]
		public void TestFormatSingleItem()
		{
			var item0 = new DicomSequenceItem();
			item0[DicomTags.CodeValue].SetStringValue("CODE1");
			item0[DicomTags.CodeMeaning].SetStringValue("Code Meaning 1");
			item0[DicomTags.CodingSchemeDesignator].SetStringValue("DCM1");

			var item1 = new DicomSequenceItem();
			item1[DicomTags.CodeValue].SetStringValue("CODE2");
			item1[DicomTags.CodeMeaning].SetStringValue("Code Meaning 2");
			item1[DicomTags.CodingSchemeDesignator].SetStringValue("DCM2");

			var item2 = new DicomSequenceItem();
			item2[DicomTags.CodeValue].SetStringValue("CODE3");
			item2[DicomTags.CodeMeaning].SetStringValue("Code Meaning 3");
			item2[DicomTags.CodingSchemeDesignator].SetStringValue("DCM3");

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item0);
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item1);
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item2);
			dataset[DicomTags.PatientBreedDescription].SetStringValue("Description (CODE1, CODE2, CODE3)");

			Assert.AreEqual("Code Meaning 1", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "1");
			Assert.AreEqual("Code Meaning 1", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "2");

			item0[DicomTags.CodeMeaning].SetEmptyValue();
			Assert.AreEqual("CODE1 (DCM1)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "3");
			Assert.AreEqual("Description (CODE1, CODE2, CODE3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "4");

			dataset[DicomTags.PatientBreedDescription].SetNullValue();
			Assert.AreEqual("CODE1 (DCM1)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null, false), "5");
			Assert.AreEqual("CODE1 (DCM1)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription, false), "6");
		}

		[Test]
		public void TestFormatMultipleItems()
		{
			var item0 = new DicomSequenceItem();
			item0[DicomTags.CodeValue].SetStringValue("CODE1");
			item0[DicomTags.CodeMeaning].SetStringValue("Code Meaning 1");
			item0[DicomTags.CodingSchemeDesignator].SetStringValue("DCM1");

			var item1 = new DicomSequenceItem();
			item1[DicomTags.CodeValue].SetStringValue("CODE2");
			item1[DicomTags.CodeMeaning].SetStringValue("Code Meaning 2");
			item1[DicomTags.CodingSchemeDesignator].SetStringValue("DCM2");

			var item2 = new DicomSequenceItem();
			item2[DicomTags.CodeValue].SetStringValue("CODE3");
			item2[DicomTags.CodeMeaning].SetStringValue("Code Meaning 3");
			item2[DicomTags.CodingSchemeDesignator].SetStringValue("DCM3");

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item0);
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item1);
			dataset[DicomTags.PatientBreedCodeSequence].AddSequenceItem(item2);
			dataset[DicomTags.PatientBreedDescription].SetStringValue("Description (CODE1, CODE2, CODE3)");

			Assert.AreEqual(@"Code Meaning 1\Code Meaning 2\Code Meaning 3", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "1");
			Assert.AreEqual(@"Code Meaning 1\Code Meaning 2\Code Meaning 3", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "2");

			item1[DicomTags.CodeMeaning].SetEmptyValue();
			Assert.AreEqual(@"Code Meaning 1\CODE2 (DCM2)\Code Meaning 3", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "3");
			Assert.AreEqual(@"Code Meaning 1\CODE2 (DCM2)\Code Meaning 3", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "4");

			item2[DicomTags.CodeMeaning].SetNullValue();
			Assert.AreEqual(@"Code Meaning 1\CODE2 (DCM2)\CODE3 (DCM3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "5");
			Assert.AreEqual(@"Code Meaning 1\CODE2 (DCM2)\CODE3 (DCM3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "6");

			item0[DicomTags.CodeMeaning].SetEmptyValue();
			Assert.AreEqual(@"CODE1 (DCM1)\CODE2 (DCM2)\CODE3 (DCM3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "7");
			Assert.AreEqual("Description (CODE1, CODE2, CODE3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "8");

			dataset[DicomTags.PatientBreedDescription].SetNullValue();
			Assert.AreEqual(@"CODE1 (DCM1)\CODE2 (DCM2)\CODE3 (DCM3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, null), "9");
			Assert.AreEqual(@"CODE1 (DCM1)\CODE2 (DCM2)\CODE3 (DCM3)", CodeSequenceAnnotationItem.FormatCodeSequence(dataset, DicomTags.PatientBreedCodeSequence, DicomTags.PatientBreedDescription), "10");
		}
	}
}

#endif