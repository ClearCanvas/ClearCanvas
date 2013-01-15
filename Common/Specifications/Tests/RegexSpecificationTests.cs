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
	public class RegexSpecificationTests : TestsBase
	{
		[Test]
		public void Test_StrictCasing()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
		}

		[Test]
		public void Test_IgnoreCasing()
		{
			RegexSpecification s = new RegexSpecification("Foo", true, false);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);
			Assert.IsTrue(s.Test("FOO").Success);
		}

		[Test]
		public void Test_NullMatches()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, true);
			Assert.IsFalse(s.Test("a").Success);
			Assert.IsFalse(s.Test("foo").Success);
			Assert.IsTrue(s.Test("Foo").Success);

			Assert.IsTrue(s.Test(null).Success);

			//TODO: it would seem that this test ought to succeed - consider changing this behaviour
			Assert.IsFalse(s.Test("").Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_InvalidType()
		{
			RegexSpecification s = new RegexSpecification("Foo", false, false);
			Assert.IsFalse(s.Test(1).Success);
		}
	}
}

#endif
