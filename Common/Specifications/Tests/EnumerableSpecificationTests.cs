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
	public class EnumerableSpecificationTests : TestsBase
	{
		[Test]
		public void Test_All_Empty()
		{
			AllSpecification s1 = new AllSpecification(AlwaysFalse);
			Assert.IsTrue(s1.Test(new object[0]).Success);

			AllSpecification s2 = new AllSpecification(AlwaysTrue);
			Assert.IsTrue(s2.Test(new object[0]).Success);
		}

		[Test]
		public void Test_All_Normal()
		{
			AllSpecification s = new AllSpecification(new PredicateSpecification<int>(delegate(int i) { return i > 0; }));
			Assert.IsFalse(s.Test(new int[] { 0, 1, 2 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_All_InvalidType()
		{
			// cannot test a non-enumerable object
			AllSpecification s = new AllSpecification(AlwaysTrue);
			s.Test(new object());
		}

		[Test]
		public void Test_Any_Empty()
		{
			AnySpecification s1 = new AnySpecification(AlwaysFalse);
			Assert.IsFalse(s1.Test(new object[0]).Success);

			AnySpecification s2 = new AnySpecification(AlwaysTrue);
			Assert.IsFalse(s2.Test(new object[0]).Success);
		}

		[Test]
		public void Test_Any_Normal()
		{
			AnySpecification s = new AnySpecification(new PredicateSpecification<int>(delegate(int i) { return i > 0; }));
			Assert.IsFalse(s.Test(new int[] { 0, 0, 0 }).Success);
			Assert.IsTrue(s.Test(new int[] { 0, 0, 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 1, 1 }).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_Any_InvalidType()
		{
			// cannot test a non-enumerable object
			AnySpecification s = new AnySpecification(AlwaysTrue);
			s.Test(new object());
		}

	}
}

#endif
