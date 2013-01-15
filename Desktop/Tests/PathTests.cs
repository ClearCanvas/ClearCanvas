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
	public class PathTests
	{
		[Test]
		public void Test_Constructor_Literals()
		{
			var p = new Path("A/B/C");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("A", p.Segments[0].ResourceKey);
			Assert.AreEqual("A", p.Segments[0].LocalizedText);
			Assert.AreEqual("B", p.Segments[1].ResourceKey);
			Assert.AreEqual("B", p.Segments[1].LocalizedText);
			Assert.AreEqual("C", p.Segments[2].ResourceKey);
			Assert.AreEqual("C", p.Segments[2].LocalizedText);

			// test other constructor, with a null resolver
			p = new Path("A/B/C", null);
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("A", p.Segments[0].ResourceKey);
			Assert.AreEqual("A", p.Segments[0].LocalizedText);
			Assert.AreEqual("B", p.Segments[1].ResourceKey);
			Assert.AreEqual("B", p.Segments[1].LocalizedText);
			Assert.AreEqual("C", p.Segments[2].ResourceKey);
			Assert.AreEqual("C", p.Segments[2].LocalizedText);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor1_Null()
		{
			new Path((string)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor2_Null()
		{
			new Path(null, new ResourceResolver(this.GetType(), false));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor3_Null()
		{
			new Path((PathSegment)null);
		}

		[Test]
		public void Test_Constructor1_EmptyPath()
		{
			var p = new Path("");
			Assert.AreEqual(0, p.Segments.Count);
			Assert.AreEqual(Path.Empty, p);
		}

		[Test]
		public void Test_Constructor2_EmptyPath()
		{
			var p = new Path("", null);
			Assert.AreEqual(0, p.Segments.Count);
			Assert.AreEqual(Path.Empty, p);
		}

		[Test]
		public void Test_Constructor_SingleSegment()
		{
			var p = new Path("a");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_Constructor_PrecedingSeparator()
		{
			var p = new Path("/a");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_EmptySegments()
		{
			var p = new Path("a//b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());

			p = new Path("a///b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());

			p = new Path("/a///b/");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());
		}

		[Test]
		public void Test_TrailingSeparator()
		{
			var p = new Path("a/");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_EscapedSeparator()
		{
			var p = new Path("a'/b");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a/b", p.Segments[0].ResourceKey);

			p = new Path("a'//b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a/", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a'//b", p.ToString());

			p = new Path("a/'//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("/", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/'//b", p.ToString());

			p = new Path("a/''//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("'/", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/''//b", p.ToString());

			p = new Path("a/'/'/b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("//b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/'/'/b", p.ToString());

			p = new Path("a/'/'//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("//", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/'/'//b", p.ToString());
		}

	}
}

#endif
