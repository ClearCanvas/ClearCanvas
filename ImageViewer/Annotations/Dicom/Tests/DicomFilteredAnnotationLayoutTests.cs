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

using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{
	[TestFixture]
	internal class DicomFilteredAnnotationLayoutTests
	{
		[Test]
		public void TestFilterMatchingCatchAll()
		{
			var layout = new DicomFilteredAnnotationLayout("dummy", "dummy");

			Assert.IsTrue(layout.IsMatch(new[]
			                             	{
			                             		new KeyValuePair<string, string>("k1", ""),
			                             		new KeyValuePair<string, string>("k2", ""),
			                             		new KeyValuePair<string, string>("k3", ""),
			                             		new KeyValuePair<string, string>("k4", ""),
			                             		new KeyValuePair<string, string>("k5", "")
			                             	}), "");
		}

		[Test]
		public void TestFilterMatchingOne()
		{
			var layout = new DicomFilteredAnnotationLayout("dummy", "dummy");
			layout.Filters.Add(new KeyValuePair<string, string>("k1", "v1"));

			Assert.IsTrue(layout.IsMatch(new[]
			                             	{
			                             		new KeyValuePair<string, string>("k1", "v1")
			                             	}), "v1");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "v2")
			                              	}), "v2");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "")
			                              	}), "<empty>");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", null)
			                              	}), "<null>");
		}

		[Test]
		public void TestFilterMatchingMultiple()
		{
			var layout = new DicomFilteredAnnotationLayout("dummy", "dummy");
			layout.Filters.Add(new KeyValuePair<string, string>("k1", "v1"));
			layout.Filters.Add(new KeyValuePair<string, string>("k2", "v2"));
			layout.Filters.Add(new KeyValuePair<string, string>("k3", ""));
			layout.Filters.Add(new KeyValuePair<string, string>("k4", null));

			Assert.IsTrue(layout.IsMatch(new[]
			                             	{
			                             		new KeyValuePair<string, string>("k1", "v1"),
			                             		new KeyValuePair<string, string>("k2", "v2"),
			                             		new KeyValuePair<string, string>("k3", ""),
			                             		new KeyValuePair<string, string>("k4", null)
			                             	}), "all match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "asdf"),
			                              		new KeyValuePair<string, string>("k2", "v2"),
			                              		new KeyValuePair<string, string>("k3", ""),
			                              		new KeyValuePair<string, string>("k4", null)
			                              	}), "k1 not match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "v1"),
			                              		new KeyValuePair<string, string>("k2", "asdf"),
			                              		new KeyValuePair<string, string>("k3", ""),
			                              		new KeyValuePair<string, string>("k4", null)
			                              	}), "k2 not match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "v1"),
			                              		new KeyValuePair<string, string>("k2", "v2"),
			                              		new KeyValuePair<string, string>("k3", "asdf"),
			                              		new KeyValuePair<string, string>("k4", null)
			                              	}), "k3 not match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "v1"),
			                              		new KeyValuePair<string, string>("k2", "v2"),
			                              		new KeyValuePair<string, string>("k3", ""),
			                              		new KeyValuePair<string, string>("k4", "asdf")
			                              	}), "k4 not match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "asdf"),
			                              		new KeyValuePair<string, string>("k2", "v2"),
			                              		new KeyValuePair<string, string>("k3", "asdf"),
			                              		new KeyValuePair<string, string>("k4", "asdf")
			                              	}), "some not match");

			Assert.IsFalse(layout.IsMatch(new[]
			                              	{
			                              		new KeyValuePair<string, string>("k1", "v1"),
			                              		new KeyValuePair<string, string>("k3", ""),
			                              		new KeyValuePair<string, string>("k4", null)
			                              	}), "some not present");
		}
	}
}

#endif