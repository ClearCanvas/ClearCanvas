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

using System.Linq;
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	internal class VoiWindowTests
	{
		[Test]
		public void TestBasics()
		{
			var window1 = new VoiWindow(123.1, 456.2);
			Assert.AreEqual(123.1, window1.Width);
			Assert.AreEqual(456.2, window1.Center);
			Assert.AreEqual("123.10/456.20", window1.ToString());

			Assert.IsFalse(window1.Equals(new object()));

			var window2 = new VoiWindow(1.231, 456.2);
			Assert.AreNotEqual(window1, window2);

			var window3 = new VoiWindow(123.1, 4.562);
			Assert.AreNotEqual(window1, window3);

			var window4 = new VoiWindow(123.1, 456.2);
			Assert.AreEqual(window1, window4);
		}

		[Test]
		public void TestExplanations()
		{
			var window0 = new VoiWindow(123.1, 456.2);
			Assert.AreEqual(string.Empty, window0.Explanation);

			var window1 = new VoiWindow(123.1, 456.2, null);
			Assert.AreEqual(string.Empty, window1.Explanation);
			Assert.AreEqual(window1, window0);

			var window2 = new VoiWindow(123.1, 456.2, "explanation");
			Assert.AreEqual("explanation", window2.Explanation);
			Assert.AreEqual(window1, window2);

			var window3 = new VoiWindow(4132, 324, "explanation");
			Assert.AreNotEqual(window2, window3);
		}

		[Test]
		public void TestGetWindows()
		{
			var windows = new[]
			              	{
			              		new VoiWindow(1, 2),
			              		new VoiWindow(100, 200),
			              		new VoiWindow(100.1, 200.2),
			              		new VoiWindow(123.1, 456.2),
			              		new VoiWindow(123.567890123, -23.567890123)
			              	}.ToList();

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.WindowCenter].SetStringValue(@"2\200\200.2\456.2\-23.567890123");
			dataset[DicomTags.WindowWidth].SetStringValue(@"1\100\100.1\123.1\123.567890123");

			var actualWindows = VoiWindow.GetWindows(dataset);

			Assert.AreEqual(windows, actualWindows);
		}

		[Test]
		public void TestGetWindowsWithExplanations()
		{
			var windows = new[]
			              	{
			              		new VoiWindow(1, 2),
			              		new VoiWindow(100, 200),
			              		new VoiWindow(100.1, 200.2),
			              		new VoiWindow(123.1, 456.2),
			              		new VoiWindow(123.567890123, -23.567890123)
			              	};

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.WindowCenter].SetStringValue(@"2\200\200.2\456.2\-23.567890123");
			dataset[DicomTags.WindowWidth].SetStringValue(@"1\100\100.1\123.1\123.567890123");
			dataset[DicomTags.WindowCenterWidthExplanation].SetStringValue(@"bob\alice\eve\james");

			var actualWindows = VoiWindow.GetWindows(dataset).ToArray();

			Assert.AreEqual(windows, actualWindows);
			Assert.AreEqual("bob", actualWindows[0].Explanation);
			Assert.AreEqual("alice", actualWindows[1].Explanation);
			Assert.AreEqual("eve", actualWindows[2].Explanation);
			Assert.AreEqual("james", actualWindows[3].Explanation);
			Assert.AreEqual("", actualWindows[4].Explanation);
		}

		[Test]
		public void TestGetWindowsMissingCenters()
		{
			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.WindowCenter].SetEmptyValue();
			dataset[DicomTags.WindowWidth].SetStringValue(@"1\100\100.1\123.1");

			var windows = VoiWindow.GetWindows(dataset).ToArray();
			Assert.AreEqual(new VoiWindow[0], windows);
		}

		[Test]
		public void TestGetWindowsMissingWidths()
		{
			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.WindowCenter].SetStringValue(@"2\200\200.2\456.2\-23.567890123");
			dataset[DicomTags.WindowWidth].SetEmptyValue();

			var windows = VoiWindow.GetWindows(dataset).ToArray();
			Assert.AreEqual(new VoiWindow[0], windows);
		}

		[Test]
		public void TestGetWindowsMismatchedPairs()
		{
			var windows = new[]
			              	{
			              		new VoiWindow(1, 2),
			              		new VoiWindow(100, 200),
			              		new VoiWindow(100.1, 200.2),
			              		new VoiWindow(123.1, 456.2)
			              	};

			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.WindowCenter].SetStringValue(@"2\200\200.2\456.2\-23.567890123");
			dataset[DicomTags.WindowWidth].SetStringValue(@"1\100\100.1\123.1");

			var actualWindows = VoiWindow.GetWindows(dataset).ToArray();
			Assert.AreEqual(windows, actualWindows);
		}

		[Test]
		public void TestSetWindows()
		{
			var windows = new[]
			              	{
			              		new VoiWindow(1, 2),
			              		new VoiWindow(100, 200),
			              		new VoiWindow(100.1, 200.2),
			              		new VoiWindow(123.1, 456.2),
			              		new VoiWindow(123.567890123, -23.567890123)
			              	};

			var dataset = new DicomAttributeCollection();
			VoiWindow.SetWindows(windows, dataset);

			Assert.AreEqual(5, dataset[DicomTags.WindowCenter].Count);
			Assert.AreEqual(dataset[DicomTags.WindowCenter].ToString(), @"2\200\200.2\456.2\-23.567890123");

			Assert.AreEqual(5, dataset[DicomTags.WindowWidth].Count);
			Assert.AreEqual(dataset[DicomTags.WindowWidth].ToString(), @"1\100\100.1\123.1\123.567890123");
		}

		[Test]
		public void TestSetWindowsWithExplanations()
		{
			var windows = new[]
			              	{
			              		new VoiWindow(1, 2, "bob"),
			              		new VoiWindow(100, 200, "alice"),
			              		new VoiWindow(100.1, 200.2, "eve"),
			              		new VoiWindow(123.1, 456.2, "james"),
			              		new VoiWindow(123.567890123, -23.567890123)
			              	};

			var dataset = new DicomAttributeCollection();
			VoiWindow.SetWindows(windows, dataset);

			Assert.AreEqual(5, dataset[DicomTags.WindowCenter].Count);
			Assert.AreEqual(dataset[DicomTags.WindowCenter].ToString(), @"2\200\200.2\456.2\-23.567890123");

			Assert.AreEqual(5, dataset[DicomTags.WindowWidth].Count);
			Assert.AreEqual(dataset[DicomTags.WindowWidth].ToString(), @"1\100\100.1\123.1\123.567890123");

			Assert.AreEqual(5, dataset[DicomTags.WindowCenterWidthExplanation].Count);
			Assert.AreEqual(dataset[DicomTags.WindowCenterWidthExplanation].ToString(), @"bob\alice\eve\james\");
		}

		[Test]
		public void TestSetWindowsEmpty()
		{
			var windows = new VoiWindow[0];

			var dataset = new DicomAttributeCollection();
			VoiWindow.SetWindows(windows, dataset);

			Assert.IsTrue(dataset[DicomTags.WindowCenter].IsEmpty);
			Assert.IsTrue(dataset[DicomTags.WindowWidth].IsEmpty);
		}
	}
}

#endif