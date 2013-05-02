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

			path = new DicomTagPath("(0054,0220)\\(0054,0222)");
			Assert.AreEqual(path, "(0054,0220)\\(0054,0222)"); 
			Assert.IsFalse(path.Equals("(0054,0220)"));
			Assert.IsFalse(path.Equals(0x00540220));
			Assert.IsFalse(path.Equals(NewDicomTag(0x00540220)));

			path = new DicomTagPath("(0054,0220)\\(0054,0222)\\(0010,0022)");
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0022)");

			path = new DicomTagPath((new List<DicomTag>(new[] { NewDicomTag(0x00540220) })));
			Assert.AreEqual(path.ToString(), "(0054,0220)");

			path = new DicomTagPath((new List<DicomTag>(new[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222) })));
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)");

			path = new DicomTagPath((new List<DicomTag>(new[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222), NewDicomTag(0x00100022) })));
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0022)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222 });
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222, 0x00100010 });
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0010)");
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
			return new DicomTag(tag, "Throwaway Tag", "ThrowawayTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}
    }
}

#endif