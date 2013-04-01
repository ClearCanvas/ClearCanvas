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
	public class InequalitySpecificationTests : TestsBase
	{
		[Test]
		public void Test_GreaterThan_Exclusive()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);

			// null is less than any other value
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_GreaterThan_Inclusive()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			s.Inclusive = true;

			Assert.IsFalse(s.Test(0).Success);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);

			// null is less than any other value
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_GreaterThan_CoerceTypes()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);

			Assert.IsFalse(s.Test(0.5).Success);
			Assert.IsTrue(s.Test(2.1).Success);

			// these will do string comparison, not numeric comparison
			Assert.IsFalse(s.Test("0.5").Success);
			Assert.IsTrue(s.Test("2.1").Success);

			// null is less than any other value
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_GreaterThan_Strict()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");
			s.Strict = true;

			// this should fail because in strict mode we don't do type coercion,
			// and IComparable throws an ArgumentException in this situation
			s.Test(0);
		}

		[Test]
		public void Test_LessThan_Exclusive()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsTrue(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsFalse(s.Test(2).Success);

			// null is less than any other value
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_LessThan_Inclusive()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			s.Inclusive = true;

			Assert.IsTrue(s.Test(0).Success);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsFalse(s.Test(2).Success);

			// null is less than any other value
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_LessThan_CoerceTypes()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsTrue(s.Test(0).Success);
			Assert.IsFalse(s.Test(2).Success);
			Assert.IsFalse(s.Test(1).Success);

			Assert.IsTrue(s.Test(0.5).Success);
			Assert.IsFalse(s.Test(2.1).Success);

			// these will do string comparison, not numeric comparison
			Assert.IsTrue(s.Test("0.5").Success);
			Assert.IsFalse(s.Test("2.1").Success);

			// null is less than any other value
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_LessThan_Strict()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");
			s.Strict = true;

			// this should fail because in strict mode we don't do type coercion,
			// and IComparable throws an ArgumentException in this situation
			s.Test(0);
		}
	}
}

#endif
