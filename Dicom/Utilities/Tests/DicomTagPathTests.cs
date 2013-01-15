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

using System.Collections.Generic;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.Dicom.Utilities.Tests
{
	internal class PathTest : DicomTagPath
	{
		public PathTest()
			: base()
		{ 
		}

		public void SetPath(string path)
		{
			base.Path = path;
		}

		public void SetPath(IList<DicomTag> tags)
		{
			base.TagsInPath = tags;
		}
	}

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
			DicomTagPath path = new PathTest();
			((PathTest)path).SetPath("(0010,0010)");

			Assert.AreEqual(path, "(0010,0010)"); 
			Assert.AreEqual(path, (uint)0x00100010);
			Assert.AreEqual(path, NewDicomTag(0x00100010));
			Assert.IsTrue(path.Equals("(0010,0010)"));
			Assert.IsTrue(path.Equals((uint)0x00100010));
			Assert.IsTrue(path.Equals(NewDicomTag(0x00100010)));

			((PathTest)path).SetPath("(0054,0220)\\(0054,0222)");
			Assert.AreEqual(path, "(0054,0220)\\(0054,0222)"); 
			Assert.IsFalse(path.Equals("(0054,0220)"));
			Assert.IsFalse(path.Equals((uint)0x00540220));
			Assert.IsFalse(path.Equals(NewDicomTag(0x00540220)));

			((PathTest)path).SetPath("(0054,0220)\\(0054,0222)\\(0010,0022)");
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0022)");

			path = new PathTest();
			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00540220) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)");

			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)");

			((PathTest)path).SetPath(new List<DicomTag>(new DicomTag[] { NewDicomTag(0x00540220), NewDicomTag(0x00540222), NewDicomTag(0x00100022) }));
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0022)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222 });
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)");

			path = new DicomTagPath(new uint[] { 0x00540220, 0x00540222, 0x00100010 });
			Assert.AreEqual(path.ToString(), "(0054,0220)\\(0054,0222)\\(0010,0010)");
		}

		private DicomTag NewDicomTag(ushort group, ushort element)
		{
			return new DicomTag(DicomTag.GetTagValue(group, element), "Throwaway Tag", "ThrowawayTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}

		private DicomTag NewDicomTag(uint tag)
		{
			return new DicomTag(tag, "Throwaway Tag", "ThrowawayTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}
    }
}

#endif