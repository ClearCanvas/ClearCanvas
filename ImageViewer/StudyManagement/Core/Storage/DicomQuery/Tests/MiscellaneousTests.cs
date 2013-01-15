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
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class MiscellaneousTests
    {
        [Test]
        public void TestDicomTagPathGetAttribute_NoCreate()
        {
            var collection = new DicomAttributeCollection();
            var path = new DicomTagPath(DicomTags.PatientId);

            //PatientID IS empty
            Assert.IsNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetNullValue();
            //PatientID NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetStringValue("PatientId");
            collection[DicomTags.PatientsName].SetStringValue("PatientsName");
            //PatientID NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));
            
            path = new DicomTagPath(DicomTags.PatientsName);
            //PatientsName NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            //ViewCodeSequence IS empty
            path = new DicomTagPath(DicomTags.ViewCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));
            
            var sequence1 = new DicomSequenceItem();
            collection[DicomTags.ViewCodeSequence].AddSequenceItem(sequence1);
            //ViewCodeSequence NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            path += DicomTags.CodeMeaning;
            //ViewCodeSequence/CodeMeaning IS empty
            Assert.IsNull(path.GetAttribute(collection));
            sequence1[DicomTags.CodeMeaning].SetNullValue();
            //ViewCodeSequence/CodeMeaning NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            //ViewCodeSequence/ConceptNameCodeSequence IS empty
            path = new DicomTagPath(DicomTags.ViewCodeSequence, DicomTags.ConceptNameCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));

            var sequence2 = new DicomSequenceItem();
            sequence1[DicomTags.ConceptNameCodeSequence].AddSequenceItem(sequence2);

            //ViewCodeSequence/ConceptNameCodeSequence NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            path += DicomTags.CodeValue;
            //ViewCodeSequence/ConceptNameCodeSequence/CodeValue IS empty
            Assert.IsNull(path.GetAttribute(collection));
            
            sequence2[DicomTags.CodeValue].SetStringValue("Code");
            //ViewCodeSequence/ConceptNameCodeSequence/CodeValue IS empty
            Assert.IsNotNull(path.GetAttribute(collection));
        }

        [Test]
        public void TestDicomTagPathGetAttribute_Create()
        {
            var collection = new DicomAttributeCollection();

            var path = new DicomTagPath(DicomTags.ViewCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));

            path += DicomTags.CodeMeaning;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));

            path = path.UpOne() + DicomTags.ConceptNameCodeSequence;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));

            path += DicomTags.CodeValue;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));
        }
    }
}

#endif