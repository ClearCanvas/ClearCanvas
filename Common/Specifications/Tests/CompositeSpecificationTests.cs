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
	public class CompositeSpecificationTests : TestsBase
	{
		[Test]
		public void Test_And_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			AndSpecification s = new AndSpecification();
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllTrue()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllFalse()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_And_Mixed()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			OrSpecification s = new OrSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllTrue()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllFalse()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Mixed()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			NotSpecification s = new NotSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllTrue()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllFalse()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Mixed()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}
	}
}

#endif
