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
using System.Globalization;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
    [TestFixture]
    public class DicomFieldTests : AbstractTest
    {
        public class TestFields
        {
            [DicomField(DicomTags.SopClassUid, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SopClassUid = null;

            [DicomField(DicomTags.SopInstanceUid, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SOPInstanceUID = null;

            [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Default)]
            public DateTime StudyDate;

            [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Default)]
            public string AccessionNumber = null;

            [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Default)]
            public string Modality = null;

            [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Default)]
            public string StudyDescription = null;

            [DicomField(DicomTags.StudyInstanceUid, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid StudyInstanceUID = null;

            [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SeriesInstanceUID = null;

            [DicomField(DicomTags.StudyId, DefaultValue = DicomFieldDefault.Default)]
            public string StudyID = null;

            [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Default)]
            public string PatientsName = null;

            [DicomField(DicomTags.PatientId, DefaultValue = DicomFieldDefault.Default)]
            public string PatientID = null;

            [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Default)]
            public DateTime PatientsBirthDate;

            [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Default)]
            public string PatientsSex = null;

            [DicomField(DicomTags.Rows, DefaultValue = DicomFieldDefault.Default)]
            public ushort Rows = 0;

            [DicomField(DicomTags.Columns, DefaultValue = DicomFieldDefault.Default)]
            public ushort Columns = 0;

            [DicomField(DicomTags.PixelSpacing, DefaultValue = DicomFieldDefault.Default)]
            public float PixelSpacing = 0.0f;

            [DicomField(DicomTags.InstanceNumber, DefaultValue = DicomFieldDefault.Default)]
            public int InstanceNumber = 0;

            [DicomField(DicomTags.ImageType, DefaultValue = DicomFieldDefault.Default)]
            public string[] ImageType = null;

            [DicomField(DicomTags.ImagePositionPatient, DefaultValue = DicomFieldDefault.Default)]
            public float[] ImagePositionPatient = null;
            
        }

        [Test]
        public void FieldTest()
        {
            DicomAttributeCollection theSet = new DicomAttributeCollection();
            TestFields theFields = new TestFields();

            SetupMR(theSet);

            theSet.LoadDicomFields(theFields);

            Assert.IsTrue(theFields.AccessionNumber.Equals(theSet[DicomTags.AccessionNumber].GetString(0,"")), "Accession Numbers did not match!");
            Assert.IsTrue(theFields.SopClassUid.UID.Equals(theSet[DicomTags.SopClassUid].GetString(0, "")), "SOP Class UIDs did not match!");
            Assert.IsTrue(theFields.SOPInstanceUID.UID.Equals(theSet[DicomTags.SopInstanceUid].GetString(0, "")), "SOP Class UIDs did not match!");
			Assert.IsTrue(theFields.StudyDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture).Equals(theSet[DicomTags.StudyDate].GetString(0, "")));
            Assert.IsTrue(theFields.Modality.Equals(theSet[DicomTags.Modality].GetString(0, "")), "Modality did not match!");
            Assert.IsTrue(theFields.StudyDescription.Equals(theSet[DicomTags.StudyDescription].GetString(0, "")), "Study Description did not match!");
            Assert.IsTrue(theFields.StudyInstanceUID.UID.Equals(theSet[DicomTags.StudyInstanceUid].GetString(0, "")), "Study Instance UIDs did not match!");
            Assert.IsTrue(theFields.SeriesInstanceUID.UID.Equals(theSet[DicomTags.SeriesInstanceUid].GetString(0, "")), "Series Instance UIDs did not match!");
            Assert.IsTrue(theFields.StudyID.Equals(theSet[DicomTags.StudyId].GetString(0, "")), "StudyID did not match!");
            Assert.IsTrue(theFields.PatientsName.Equals(theSet[DicomTags.PatientsName].GetString(0, "")), "PatientsName did not match!");
            Assert.IsTrue(theFields.PatientID.Equals(theSet[DicomTags.PatientId].GetString(0, "")), "PatientID did not match!");
			Assert.IsTrue(theFields.PatientsBirthDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture).Equals(theSet[DicomTags.PatientsBirthDate].GetString(0, "")));
            Assert.IsTrue(theFields.PatientsSex.Equals(theSet[DicomTags.PatientsSex].GetString(0, "")), "PatientsSex did not match!");
            Assert.IsTrue(theFields.Rows == theSet[DicomTags.Rows].GetUInt16(0,0));
            Assert.IsTrue(theFields.Columns == theSet[DicomTags.Columns].GetUInt16(0,0));
            float floatValue;
            theSet[DicomTags.PixelSpacing].TryGetFloat32(0, out floatValue);
            Assert.IsTrue(theFields.PixelSpacing == floatValue);
            int intValue;
            theSet[DicomTags.InstanceNumber].TryGetInt32(0, out intValue);
            Assert.IsTrue(theFields.InstanceNumber == intValue);
            //Assert.IsTrue(string.Join("\\", theFields.ImageType).Equals(theSet[DicomTags.ImageType].ToString()));
        }

    }
}

#endif