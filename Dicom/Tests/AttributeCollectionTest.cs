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

#pragma warning disable 1591, 0168

#if UNIT_TESTS

using NUnit.Framework;


namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class AttributeCollectionTest : AbstractTest
	{
        [Test]
        public void TestFile()
        {
            string filename = "OutOfRange.dcm";

            DicomFile file = new DicomFile(filename, new DicomAttributeCollection(), new DicomAttributeCollection());
            SetupMR(file.DataSet);
            SetupMetaInfo(file);

            DicomTag tag = new DicomTag(0x00030010, "Test", "TestBad", DicomVr.LOvr, false, 1, 1, false);

            file.DataSet[tag].SetStringValue("Test");

            file.Save(filename);

            file = new DicomFile(filename);

            file.DataSet.IgnoreOutOfRangeTags = true;

            file.Load();

            Assert.IsNotNull(file.DataSet.GetAttribute(tag));

            file = new DicomFile(filename);

            file.DataSet.IgnoreOutOfRangeTags = false;

            try
            {
                file.Load();
                Assert.Fail("file.Load failed");
            }
            catch (DicomException)
            {
            }
        }

	    [Test]
		public void TestIgnoreOutOfRangeTags()
		{
			DicomFile file = new DicomFile();
			DicomAttributeCollection collection = file.DataSet;

			SetupMR(collection);
			collection.IgnoreOutOfRangeTags = true;

			DicomTag tag = new DicomTag(0x00030010, "Test", "TestBad", DicomVr.STvr, false, 1, 1, false);

			Assert.IsNotNull(collection.GetAttribute(tag));
			Assert.IsNotNull(collection[tag]);
			Assert.IsNotNull(collection[tag.TagValue]);

			collection.IgnoreOutOfRangeTags = false;

			try
			{
				collection.GetAttribute(tag);
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}

			try
			{
				DicomAttribute attrib = collection[tag];
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}

			try
			{
				DicomAttribute attrib = collection[tag.TagValue];
				Assert.Fail("DicomAttributeCollection.GetAttribute failed");
			}
			catch (DicomException)
			{
			}
		}
	}
}

#endif