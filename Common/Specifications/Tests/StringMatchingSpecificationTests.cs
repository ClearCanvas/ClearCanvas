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
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class StringMatchingSpecificationTests : TestsBase
	{
		[Test]
		public void Test_StartsWith()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
		}

		[Test]
		public void Test_StartsWith_StrictCasing()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
		}

		[Test]
		public void Test_StartsWith_IgnoreCasing()
		{
			var s = new StartsWithSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
			Assert.IsTrue(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("FOOa").Success);
		}

		[Test]
		public void Test_StartsWith_NullMatches()
		{
			var s = new StartsWithSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_StartsWith_InvalidType()
		{
			var s = new StartsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		public void Test_EndsWith()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("Fooa").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
		}

		[Test]
		public void Test_EndsWith_StrictCasing()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("AFoo").Success);
		}

		[Test]
		public void Test_EndsWith_IgnoreCasing()
		{
			var s = new EndsWithSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
		}

		[Test]
		public void Test_EndsWith_NullMatches()
		{
			var s = new EndsWithSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_EndsWith_InvalidType()
		{
			var s = new EndsWithSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		public void Test_Contains()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("aFooa").Success);
		}

		[Test]
		public void Test_Contains_StrictCasing()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsFalse(s.Test("afoo").Success);
			Assert.IsFalse(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("Fooa").Success);
			Assert.IsTrue(s.Test("aFooa").Success);
		}

		[Test]
		public void Test_Contains_IgnoreCasing()
		{
			var s = new ContainsSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("afoo").Success);
			Assert.IsTrue(s.Test("fooa").Success);
			Assert.IsTrue(s.Test("aFoo").Success);
			Assert.IsTrue(s.Test("FOOa").Success);
		}

		[Test]
		public void Test_Contains_NullMatches()
		{
			var s = new ContainsSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_Contains_InvalidType()
		{
			var s = new ContainsSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}
	}
}

#endif
