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
using System.Linq;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.Dicom.Utilities.Tests
{
    [TestFixture]
    public class DicomTagPathTests
    {
        [TestFixtureSetUp]
        public void Init()
        {
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
        }

		[Test]
		public void Test()
		{
            var path = new DicomTagPath("(0010,0010)");

			Assert.AreEqual(path, "(0010,0010)"); 
			Assert.AreEqual(path, (uint)0x00100010);
			Assert.AreEqual(path, NewDicomTag(0x00100010));
			Assert.IsTrue(path.Equals("(0010,0010)"));
			Assert.IsTrue(path.Equals(0x00100010));
			Assert.IsTrue(path.Equals(NewDicomTag(0x00100010)));

            //Test old separator - "backslash"
            path = new DicomTagPath(@"(0054,0220)\(0054,0222)");

			path = new DicomTagPath("(0054,0220)/(0054,0222)");
			Assert.AreEqual(path, "(0054,0220)/(0054,0222)"); 
			Assert.IsFalse(path.Equals("(0054,0220)"));
			Assert.IsFalse(path.Equals(0x00540220));
			Assert.IsFalse(path.Equals(NewDicomTag(0x00540220)));

			path = new DicomTagPath("(0054,0220)/(0054,0222)/(0010,0022)");
			Assert.AreEqual(path.ToString(), "(0054,0220)/(0054,0222)/(0010,0022)");

			path = new DicomTagPath((new[] { NewDicomTag(0x00540220) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)");

			path = new DicomTagPath((new[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)/(0054,0222)");

			path = new DicomTagPath((new[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222), NewDicomTag(0x00100022) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)/(0054,0222)/(0010,0022)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222 });
			Assert.AreEqual(path.ToString(), "(0054,0220)/(0054,0222)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222, 0x00100010 });
			Assert.AreEqual(path.ToString(), "(0054,0220)/(0054,0222)/(0010,0010)");
		}

        [Test]
        public void TestSetVr()
        {
            var path = new DicomTagPath(new[] { DicomTags.ViewCodeSequence, DicomTags.CodeMeaning });
            Assert.AreEqual(DicomVr.SQvr, path.TagsInPath[0].VR);
            Assert.AreNotEqual(DicomVr.UNvr, path.TagsInPath[1].VR);

            path = new DicomTagPath(new[] { DicomTags.ViewCodeSequence, DicomTags.CodeMeaning }, DicomVr.UNvr);
            Assert.AreEqual(DicomVr.SQvr, path.TagsInPath[0].VR);
            Assert.AreEqual(DicomVr.UNvr, path.TagsInPath[1].VR);
        }

        [Test]
        public void TestGetAttributes()
        {
            var collection = new DicomAttributeCollection();
            collection[DicomTags.PatientId].SetStringValue("Test^Patient");
            var viewCodeSequence1 = new DicomSequenceItem();
            var viewCodeSequence2 = new DicomSequenceItem();
            collection[DicomTags.ViewCodeSequence].Values = new[] {viewCodeSequence1, viewCodeSequence2};
            viewCodeSequence1[DicomTags.CodeMeaning].SetStringValue("It's");
            viewCodeSequence2[DicomTags.CodeMeaning].SetStringValue("The Bomb");

            var path = new DicomTagPath(new[] { DicomTags.PatientId});
            var attributes = path.SelectAttributesFrom(collection).ToList();
            Assert.AreEqual(1, attributes.Count);
            Assert.AreEqual(attributes[0].ToString(), "Test^Patient");

            path = new DicomTagPath(new[] { DicomTags.PatientsName });
            attributes = path.SelectAttributesFrom(collection).ToList();
            Assert.AreEqual(1, attributes.Count);
            Assert.IsTrue(attributes[0].IsEmpty);

            path = new DicomTagPath(new[] { DicomTags.ViewCodeSequence, DicomTags.CodeMeaning });
            attributes = path.SelectAttributesFrom(collection).ToList();
            Assert.AreEqual(2, attributes.Count);
            Assert.AreEqual(attributes[0].ToString(), "It's");
            Assert.AreEqual(attributes[1].ToString(), "The Bomb");

            attributes[1].SetEmptyValue();
            attributes = path.SelectAttributesFrom(collection).ToList();
            Assert.AreEqual(2, attributes.Count);
            Assert.AreEqual(attributes[0].ToString(), "It's");
            Assert.IsTrue(attributes[1].IsEmpty);

            path = new DicomTagPath(new[] { DicomTags.AcquisitionDeviceTypeCodeSequence, DicomTags.CodeMeaning });
            attributes = path.SelectAttributesFrom(collection, true).ToList();
            Assert.AreEqual(1, attributes.Count);
            Assert.IsTrue(attributes[0].IsEmpty);
        }

        [Test]
        public void TestAdd()
        {
            var tag = DicomTagPath.Nil;
            tag = tag + DicomTags.PatientId;
            Assert.AreEqual(tag, NewDicomTag(DicomTags.PatientId));

            tag = tag + DicomTagPath.Nil;
            Assert.AreEqual(tag, NewDicomTag(DicomTags.PatientId));
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void AssertBadPath1()
        {
            var tag = new DicomTagPath(DicomTags.PatientId);
            tag = tag + DicomTags.PatientsName;
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void AssertBadPath2()
        {
            var tag = new DicomTagPath(DicomTags.ViewCodeSequence);
            try
            {
                tag = tag + DicomTags.CodeMeaning;
            }
            catch (Exception)
            {
                Assert.Fail("This is a valid path");
            }

            tag += DicomTags.PatientId;
        }

		private DicomTag NewDicomTag(uint tag)
		{
		    return DicomTagPath.NewTag(tag, null);
		}
    }
}

#endif