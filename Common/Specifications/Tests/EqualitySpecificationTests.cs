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
	public class EqualitySpecificationTests : TestsBase
	{
		enum Color
		{
			Red,
			Blue,
			Green
		}


		[Test]
		public void Test_Equal_ValueType()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Equal_ReferenceType()
		{
			object x = new object();
			object y = new object();

			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(x);

			Assert.IsTrue(s.Test(x).Success);
			Assert.IsFalse(s.Test(y).Success);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Equal_Null()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(null);

			Assert.IsTrue(s.Test(null).Success);
			Assert.IsFalse(s.Test(new object()).Success);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		public void Test_Equal_CoerceTypes()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsTrue(s.Test(1).Success);
			Assert.IsTrue(s.Test(1.0).Success);
			Assert.IsTrue(s.Test("1").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
		}

		[Test]
		// this test is related to bug #5909 - should be able to compare enums to strings
		public void Test_Equal_CoerceEnum()
		{
			EqualSpecification s = new EqualSpecification();

			s.RefValueExpression = new ConstantExpression("Blue");
			Assert.IsTrue(s.Test(Color.Blue).Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(Color.Red).Success);

			s.RefValueExpression = new ConstantExpression(Color.Red);
			Assert.IsTrue(s.Test(Color.Red).Success);
			Assert.IsTrue(s.Test("Red").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("Silver").Success);
			Assert.IsFalse(s.Test(Color.Blue).Success);
		}

		[Test]
		public void Test_Equal_Strict()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(1.0);
			s.Strict = true;

			// this should fail because in strict mode we don't do type coercion,
			// and Object.Equals(x, y) returns false when comparing different types
			Assert.IsFalse(s.Test(1).Success);
		}


		[Test]
		public void Test_NotEqual_ValueType()
		{
			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsTrue(s.Test(0).Success);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_NotEqual_ReferenceType()
		{
			object x = new object();
			object y = new object();

			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression(x);

			Assert.IsFalse(s.Test(x).Success);
			Assert.IsTrue(s.Test(y).Success);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_NotEqual_Null()
		{
			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression(null);

			Assert.IsFalse(s.Test(null).Success);
			Assert.IsTrue(s.Test(new object()).Success);
			Assert.IsTrue(s.Test(1).Success);
		}

		[Test]
		public void Test_NotEqual_CoerceTypes()
		{
			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsFalse(s.Test(1).Success);
			Assert.IsFalse(s.Test(1.0).Success);
			Assert.IsFalse(s.Test("1").Success);

			Assert.IsTrue(s.Test(null).Success);
			Assert.IsTrue(s.Test(0).Success);
			Assert.IsTrue(s.Test(0.0).Success);
			Assert.IsTrue(s.Test("0").Success);
		}

		[Test]
		public void Test_NotEqual_Strict()
		{
			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression(1.0);
			s.Strict = true;

			// this should pass because in strict mode we don't do type coercion,
			// and Object.Equals(x, y) returns false when comparing different types
			Assert.IsTrue(s.Test(1).Success);
		}

		[Test]
		public void Test_Equal_CoerceFloat1()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression("1.0");

			Assert.IsTrue(s.Test(1.0f).Success);
			Assert.IsTrue(s.Test(1.0d).Success);
			Assert.IsTrue(s.Test((decimal)1.0).Success);
			Assert.IsTrue(s.Test(1).Success);

			Assert.IsFalse(s.Test("1").Success);
			Assert.IsFalse(s.Test("1.00").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
			Assert.IsFalse(s.Test(0).Success);
		}

		[Test]
		public void Test_Equal_CoerceFloat2()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(1.0);

			Assert.IsTrue(s.Test(1).Success);
			Assert.IsTrue(s.Test(1.0).Success);
			Assert.IsTrue(s.Test("1").Success);
			Assert.IsTrue(s.Test("1.0").Success);
			Assert.IsTrue(s.Test("1.000").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
		}

		[Test]
		public void Test_Equal_CoerceFloat3()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(1);

			Assert.IsTrue(s.Test(1).Success);
			Assert.IsTrue(s.Test(1.0).Success);
			Assert.IsTrue(s.Test("1").Success);
			Assert.IsTrue(s.Test("1.0").Success);

			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
		}

		[Test]
		public void Test_Equal_CoerceFloat4()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression("1.1");

			Assert.IsTrue(s.Test(1.1f).Success);
			Assert.IsTrue(s.Test(1.1d).Success);
			Assert.IsTrue(s.Test((decimal)1.1).Success);
			Assert.IsTrue(s.Test("1.1").Success);

			Assert.IsFalse(s.Test(1).Success);
			Assert.IsFalse(s.Test("1").Success);
			Assert.IsFalse(s.Test("1.10").Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
			Assert.IsFalse(s.Test(0).Success);
		}

		[Test]
		public void Test_Equal_CoerceFloat5()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression(1.1);

			Assert.IsTrue(s.Test(1.1f).Success);
			Assert.IsTrue(s.Test(1.1d).Success);
			Assert.IsTrue(s.Test((decimal)1.1).Success);
			Assert.IsTrue(s.Test("1.1").Success);
			Assert.IsTrue(s.Test("1.10").Success);
			Assert.IsTrue(s.Test("1.100").Success);

			Assert.IsFalse(s.Test(1).Success);
			Assert.IsFalse(s.Test(1.0).Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0").Success);
		}

		[Test]
		public void Test_Equal_CoerceChar1()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression('x');

			Assert.IsTrue(s.Test('x').Success);
			Assert.IsTrue(s.Test("x").Success);

			Assert.IsFalse(s.Test('\0').Success);
			Assert.IsFalse(s.Test('y').Success);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("xy").Success);
			Assert.IsFalse(s.Test("yx").Success);
		}

		[Test]
		public void Test_Equal_CoerceChar2()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression("x");

			Assert.IsTrue(s.Test("x").Success);
			Assert.IsTrue(s.Test('x').Success);

			Assert.IsFalse(s.Test('\0').Success);
			Assert.IsFalse(s.Test('y').Success);
			Assert.IsFalse(s.Test("").Success);
			Assert.IsFalse(s.Test("xy").Success);
			Assert.IsFalse(s.Test("yx").Success);
		}

	}
}

#endif
