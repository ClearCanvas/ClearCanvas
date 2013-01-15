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

using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageServer.Core.Edit.Test
{
    [TestFixture]
    public class SetDicomTagTest_UnicodeConversionNotAllowed : SetDicomTagTestBase
    {
        [TestFixtureSetUp]
        public override void Init()
        {
            base.Init();
            UnicodeAllowed = false;

        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test(Description="Test cases where the original is Latin1 and it's not necessary to convert to unicode")]
        public void TestOriginalIsLatin1()
        {
            
            // PN tag
            AssertTagValueChanged(DicomTags.PatientsName, "ABC", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.PatientsName, "ABCD", Latin1, Latin1);


            // SH tag
            AssertTagValueChanged(DicomTags.StudyId, "StudyID1", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.StudyId, "StudyID2", Latin1, Latin1);

            // AE , UI
            AssertTagValueChanged(DicomTags.RetrieveAeTitle, "AE-TITLE", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.RetrieveAeTitle, "AE-TITLE1", Latin1, Latin1);

            AssertTagValueChanged(DicomTags.StudyInstanceUid, "1.34", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.StudyInstanceUid, "1.345", Latin1, Latin1);
            
        }

        [Test(Description = "Test cases where the original is Latin1 and it's necessary to convert to unicode. Setting does not allow it")]
        public void TestOriginalIsLatin1_ConversionIsNeeded()
        {

            // PN tag
            AssertExceptionThrown(DicomTags.PatientsName, "我的名字", Latin1);
            AssertExceptionThrown(DicomTags.PatientsName, "我的名字是", Latin1);

            // SH tag
            AssertExceptionThrown(DicomTags.StudyId, "記錄ID", Latin1);
            AssertExceptionThrown(DicomTags.StudyId, "記錄ID1", Latin1);

        }

        [Test(Description = "Test cases where the original is UTF8. File should not be converted to Latin1 when updated")]
        public void TestOriginalIsUnicode()
        {
            // PN tag
            AssertTagValueChanged(DicomTags.PatientsName, "ABC", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.PatientsName, "ABCD", Utf8, Utf8);


            // SH tag
            AssertTagValueChanged(DicomTags.StudyId, "StudyID1", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.StudyId, "StudyID2", Utf8, Utf8);

            // AE , UI
            AssertTagValueChanged(DicomTags.RetrieveAeTitle, "AE-TITLE", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.RetrieveAeTitle, "AE-TITLE1", Utf8, Utf8);

            AssertTagValueChanged(DicomTags.StudyInstanceUid, "1.34", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.StudyInstanceUid, "1.345", Utf8, Utf8);

        }


       

        [Test(Description = "Tests where the values are set to null/empty string")]
        public void TestEmptyString()
        {
            // Original file is Latin1

            // PN tag
            AssertTagValueChanged(DicomTags.PatientsName, "", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.PatientsName, null, Latin1, Latin1);

            // SH tag
            AssertTagValueChanged(DicomTags.StudyId, "", Latin1, Latin1);
            AssertTagValueChanged(DicomTags.StudyId, null, Latin1, Latin1);

            // Original file is UTF8:
            // PN tag
            AssertTagValueChanged(DicomTags.PatientsName, "", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.PatientsName, null, Utf8, Utf8);
            // SH tag
            AssertTagValueChanged(DicomTags.StudyId, "", Utf8, Utf8);
            AssertTagValueChanged(DicomTags.StudyId, null, Utf8, Utf8);
        }

        [Test(Description = "Tests where extended characters are used in special tags which are not allowed")]
        public void TestSpecialTags()
        {

            // when original file is UTF8
            AssertExceptionThrown(DicomTags.RetrieveAeTitle, "記錄-TITLE1", Utf8);
            AssertExceptionThrown(DicomTags.StudyInstanceUid, "1.345記錄", Utf8);

            // when original file is Latin1
            AssertExceptionThrown(DicomTags.RetrieveAeTitle, "記錄-TITLE1", Latin1);
            AssertExceptionThrown(DicomTags.StudyInstanceUid, "1.345記錄", Latin1);

        }

    }
}
#endif