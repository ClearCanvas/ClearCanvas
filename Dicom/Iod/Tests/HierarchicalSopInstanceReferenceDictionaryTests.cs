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
using System.Linq;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	internal class HierarchicalSopInstanceReferenceDictionaryTests
	{
		[Test]
		public void TestCount()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			Assert.AreEqual(11, dictionary.Count);
		}

		[Test]
		public void TestAddReference()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			Assert.AreEqual(2, dictionary.ListStudies().Count, "study count");
			Assert.AreEqual(8, dictionary.ListSeries("study1").Count, "study1 series count");
			Assert.AreEqual(1, dictionary.ListSeries("study2").Count, "study2 series count");

			Assert.AreEqual(2, dictionary.ListSops("study1", "series1", "", "", "", "").Count, "study1 series1 sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series2", "", "", "", "").Count, "study1 series2 sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle1", "location1", "mediaid1", "mediauid1").Count, "study1 series1(1111) sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid1").Count, "study1 series1(2111) sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle1", "location2", "mediaid1", "mediauid1").Count, "study1 series1(1211) sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle1", "location1", "mediaid2", "mediauid1").Count, "study1 series1(1121) sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle1", "location1", "mediaid1", "mediauid2").Count, "study1 series1(1112) sop count");
			Assert.AreEqual(2, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid2").Count, "study1 series1(2112) sop count");

			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "", "", "", ""), "study1 series1 sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series2", "sopinst3", "", "", "", ""), "study1 series2 sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1"), "study1 series1(1111) sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1"), "study1 series1(2111) sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1"), "study1 series1(1211) sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1"), "study1 series1(1121) sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2"), "study1 series1(1112) sop check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop2 check");

			Assert.AreEqual(1, dictionary.ListSops("study2", "series3", "", "", "", "").Count, "study2 series3 sop count");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study2", "series3", "sopinst4", "", "", "", ""), "study2 series3 sop check");

			try
			{
				dictionary.AddReference("study1", "series1", "sopclass2", "sopinst1");
				Assert.Fail("Expected exception of type {0}", typeof (ArgumentException));
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestRemoveReference()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			Assert.IsFalse(dictionary.TryRemoveReference("study3", "series1", "sopclass1", "sopinst2"), "nonexistent study");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series9", "sopclass1", "sopinst2"), "nonexistent series");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst9"), "nonexistent sop");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass2", "sopinst1"), "nonexistent sop (class mismatch)");
			Assert.AreEqual(11, dictionary.Count, "count check");

			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1", "aetitle3", "location1", "mediaid1", "mediauid2"), "nonexistent series (aetitle mismatch");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location3", "mediaid1", "mediauid2"), "nonexistent series (location mismatch");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid3", "mediauid2"), "nonexistent series (mediaid mismatch");
			Assert.IsFalse(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid3"), "nonexistent series (medauid mismatch");
			Assert.AreEqual(11, dictionary.Count, "count check");

			Assert.IsTrue(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "remove 1 sop");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid2").Count, "study1 series1(2112) sop count");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop2 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst2", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual(10, dictionary.Count, "count check");

			Assert.IsTrue(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "remove 1 sop");
			Assert.AreEqual(0, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid2").Count, "study1 series1(2112) sop count");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop2 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst1", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst2", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual(9, dictionary.Count, "count check");

			Assert.IsTrue(dictionary.TryRemoveReference("study1", "series1", "sopclass1", "sopinst1"), "remove all series1");
			Assert.AreEqual(0, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid2").Count, "study1 series1(2112) sop count");
			Assert.AreEqual(0, dictionary.ListSops("study1", "series1", "aetitle1", "location1", "mediaid1", "mediauid2").Count, "study1 series1(1112) sop count");
			Assert.AreEqual(1, dictionary.ListSops("study1", "series1", "", "", "", "").Count, "study1 series1(1112) sop count");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual("sopclass1", dictionary.GetSopClass("study1", "series1", "sopinst2", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual(3, dictionary.Count, "count check");
		}

		[Test]
		public void TestContainsReference()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			Assert.IsFalse(dictionary.ContainsReference("study3", "series1", "sopclass1", "sopinst2"), "nonexistent study");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series9", "sopclass1", "sopinst2"), "nonexistent series");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst9"), "nonexistent sop");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass2", "sopinst1"), "nonexistent sop (class mismatch)");

			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1", "aetitle3", "location1", "mediaid1", "mediauid2"), "nonexistent series (aetitle mismatch");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location3", "mediaid1", "mediauid2"), "nonexistent series (location mismatch");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid3", "mediauid2"), "nonexistent series (mediaid mismatch");
			Assert.IsFalse(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid3"), "nonexistent series (medauid mismatch");

			Assert.IsTrue(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "contains sop1");
			Assert.IsTrue(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "contains sop2");
			Assert.IsTrue(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst1"), "contains any series1 sop1");
			Assert.IsTrue(dictionary.ContainsReference("study1", "series1", "sopclass1", "sopinst2"), "contains any series1 sop2");
		}

		[Test]
		public void TestClear()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			dictionary.Clear();
			Assert.AreEqual(0, dictionary.Count, "count check");

			Assert.AreEqual(0, dictionary.ListSops("study1", "series1", "aetitle2", "location1", "mediaid1", "mediauid2").Count, "study1 series1(2112) sop count");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop1 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2"), "study1 series1(2112) sop2 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst1", "", "", "", ""), "study1 series1 sop1 check");
			Assert.AreEqual(null, dictionary.GetSopClass("study1", "series1", "sopinst2", "", "", "", ""), "study1 series1 sop1 check");
		}

		[Test]
		public void TestToIod()
		{
			var dictionary = new HierarchicalSopInstanceReferenceDictionary();
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location2", "mediaid1", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid2", "mediauid1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle1", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst1");
			dictionary.AddReference("study1", "series1", "sopclass1", "sopinst2", "aetitle2", "location1", "mediaid1", "mediauid2");
			dictionary.AddReference("study1", "series2", "sopclass1", "sopinst3");
			dictionary.AddReference("study2", "series3", "sopclass1", "sopinst4");

			var studies = dictionary.ToList().OrderBy(s => s.StudyInstanceUid).ToList();
			Assert.AreEqual(2, studies.Count, "study count");
			Assert.AreEqual("study1", studies[0].StudyInstanceUid, "study1");
			Assert.AreEqual("study2", studies[1].StudyInstanceUid, "study2");

			var study1Series = studies[0].ReferencedSeriesSequence.OrderBy(s => s.SeriesInstanceUid).ThenBy(s => s.RetrieveAeTitle).ThenBy(s => s.RetrieveLocationUid).ThenBy(s => s.StorageMediaFileSetId).ThenBy(s => s.StorageMediaFileSetUid).ToList();
			Assert.AreEqual(8, study1Series.Count, "study1 series count");

			Assert.AreEqual("series1", study1Series[0].SeriesInstanceUid, "study1 series1");
			Assert.AreEqual("", study1Series[0].RetrieveAeTitle, "study1 series1");
			Assert.AreEqual("", study1Series[0].RetrieveLocationUid, "study1 series1");
			Assert.AreEqual("", study1Series[0].StorageMediaFileSetId, "study1 series1");
			Assert.AreEqual("", study1Series[0].StorageMediaFileSetUid, "study1 series1");

			Assert.AreEqual("series1", study1Series[1].SeriesInstanceUid, "study1 series1(1111)");
			Assert.AreEqual("aetitle1", study1Series[1].RetrieveAeTitle, "study1 series1(1111)");
			Assert.AreEqual("location1", study1Series[1].RetrieveLocationUid, "study1 series1(1111)");
			Assert.AreEqual("mediaid1", study1Series[1].StorageMediaFileSetId, "study1 series1(1111)");
			Assert.AreEqual("mediauid1", study1Series[1].StorageMediaFileSetUid, "study1 series1(1111)");

			Assert.AreEqual("series1", study1Series[2].SeriesInstanceUid, "study1 series1(1112)");
			Assert.AreEqual("aetitle1", study1Series[2].RetrieveAeTitle, "study1 series1(1112)");
			Assert.AreEqual("location1", study1Series[2].RetrieveLocationUid, "study1 series1(1112)");
			Assert.AreEqual("mediaid1", study1Series[2].StorageMediaFileSetId, "study1 series1(1112)");
			Assert.AreEqual("mediauid2", study1Series[2].StorageMediaFileSetUid, "study1 series1(1112)");

			Assert.AreEqual("series1", study1Series[3].SeriesInstanceUid, "study1 series1(1121)");
			Assert.AreEqual("aetitle1", study1Series[3].RetrieveAeTitle, "study1 series1(1121)");
			Assert.AreEqual("location1", study1Series[3].RetrieveLocationUid, "study1 series1(1121)");
			Assert.AreEqual("mediaid2", study1Series[3].StorageMediaFileSetId, "study1 series1(1121)");
			Assert.AreEqual("mediauid1", study1Series[3].StorageMediaFileSetUid, "study1 series1(1121)");

			Assert.AreEqual("series1", study1Series[4].SeriesInstanceUid, "study1 series1(1211)");
			Assert.AreEqual("aetitle1", study1Series[4].RetrieveAeTitle, "study1 series1(1211)");
			Assert.AreEqual("location2", study1Series[4].RetrieveLocationUid, "study1 series1(1211)");
			Assert.AreEqual("mediaid1", study1Series[4].StorageMediaFileSetId, "study1 series1(1211)");
			Assert.AreEqual("mediauid1", study1Series[4].StorageMediaFileSetUid, "study1 series1(1211)");

			Assert.AreEqual("series1", study1Series[5].SeriesInstanceUid, "study1 series1(2111)");
			Assert.AreEqual("aetitle2", study1Series[5].RetrieveAeTitle, "study1 series1(2111)");
			Assert.AreEqual("location1", study1Series[5].RetrieveLocationUid, "study1 series1(2111)");
			Assert.AreEqual("mediaid1", study1Series[5].StorageMediaFileSetId, "study1 series1(2111)");
			Assert.AreEqual("mediauid1", study1Series[5].StorageMediaFileSetUid, "study1 series1(2111)");

			Assert.AreEqual("series1", study1Series[6].SeriesInstanceUid, "study1 series1(2112)");
			Assert.AreEqual("aetitle2", study1Series[6].RetrieveAeTitle, "study1 series1(2112)");
			Assert.AreEqual("location1", study1Series[6].RetrieveLocationUid, "study1 series1(2112)");
			Assert.AreEqual("mediaid1", study1Series[6].StorageMediaFileSetId, "study1 series1(2112)");
			Assert.AreEqual("mediauid2", study1Series[6].StorageMediaFileSetUid, "study1 series1(2112)");

			Assert.AreEqual("series2", study1Series[7].SeriesInstanceUid, "study1 series2");
			Assert.AreEqual("", study1Series[7].RetrieveAeTitle, "study1 series2");
			Assert.AreEqual("", study1Series[7].RetrieveLocationUid, "study1 series2");
			Assert.AreEqual("", study1Series[7].StorageMediaFileSetId, "study1 series2");
			Assert.AreEqual("", study1Series[7].StorageMediaFileSetUid, "study1 series2");

			var study1Series1Sops = study1Series[0].ReferencedSopSequence.OrderBy(s => s.ReferencedSopInstanceUid).ToList();
			Assert.AreEqual(1, study1Series1Sops.Count, "study1 series1 sop count");
			Assert.AreEqual("sopinst1", study1Series1Sops[0].ReferencedSopInstanceUid, "study1 series1 sop1");
			Assert.AreEqual("sopclass1", study1Series1Sops[0].ReferencedSopClassUid, "study1 series1 sop1");

			var study1Series12112Sops = study1Series[6].ReferencedSopSequence.OrderBy(s => s.ReferencedSopInstanceUid).ToList();
			Assert.AreEqual(2, study1Series12112Sops.Count, "study1 series1(2112) sop count");
			Assert.AreEqual("sopinst1", study1Series12112Sops[0].ReferencedSopInstanceUid, "study1 series1(2112) sop1");
			Assert.AreEqual("sopclass1", study1Series12112Sops[0].ReferencedSopClassUid, "study1 series1(2112) sop1");
			Assert.AreEqual("sopinst2", study1Series12112Sops[1].ReferencedSopInstanceUid, "study1 series1(2112) sop2");
			Assert.AreEqual("sopclass1", study1Series12112Sops[1].ReferencedSopClassUid, "study1 series1(2112) sop2");
		}
	}
}

namespace ClearCanvas.Dicom.Iod
{
	partial class HierarchicalSopInstanceReferenceDictionary
	{
		internal IList<string> ListStudies()
		{
			return _dictionary.Keys.ToList();
		}

		internal IList<string> ListSeries(string studyInstanceUid)
		{
			if (_dictionary.ContainsKey(studyInstanceUid))
				return _dictionary[studyInstanceUid].Keys.Select(s => s.SeriesInstanceUid).ToList();
			return new string[0];
		}

		internal IList<string> ListSops(string studyInstanceUid, string seriesInstanceUid, string retrieveAe, string retrieveLocation, string mediaFileSetId, string mediaFileSetUid)
		{
			if (_dictionary.ContainsKey(studyInstanceUid))
			{
				var seriesKey = new SeriesKey(seriesInstanceUid, retrieveAe, retrieveLocation, mediaFileSetId, mediaFileSetUid);
				if (_dictionary[studyInstanceUid].ContainsKey(seriesKey))
					return _dictionary[studyInstanceUid][seriesKey].Keys.ToList();
			}
			return new string[0];
		}

		internal string GetSopClass(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, string retrieveAe, string retrieveLocation, string mediaFileSetId, string mediaFileSetUid)
		{
			if (_dictionary.ContainsKey(studyInstanceUid))
			{
				var seriesKey = new SeriesKey(seriesInstanceUid, retrieveAe, retrieveLocation, mediaFileSetId, mediaFileSetUid);
				if (_dictionary[studyInstanceUid].ContainsKey(seriesKey))
				{
					string sopClass;
					if (_dictionary[studyInstanceUid][seriesKey].TryGetValue(sopInstanceUid, out sopClass))
						return sopClass;
				}
			}
			return null;
		}
	}
}

#endif