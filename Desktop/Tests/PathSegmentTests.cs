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
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class PathSegmentTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor_Null()
		{
			new PathSegment(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_Constructor_Empty()
		{
			new PathSegment("");
		}

		[Test]
		public void Test_Constructor_NullLocalized()
		{
			var p = new PathSegment("a", (string)null);
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual(null, p.LocalizedText);
		}

		[Test]
		public void Test_Constructor_EmptyLocalized()
		{
			var p = new PathSegment("a", "");
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual("", p.LocalizedText);
		}

		[Test]
		public void Test_NullResolver()
		{
			var p = new PathSegment("a", (IResourceResolver) null);
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual("a", p.LocalizedText);
		}
	}
}

#endif
