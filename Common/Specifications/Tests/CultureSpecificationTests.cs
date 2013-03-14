#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class CultureSpecificationTests : TestsBase
	{
		[Test]
		public void Test_CoerceTypes_Invariant()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			try
			{
				Test_Equal_CoerceTypes();
				Test_NotEqual_CoerceTypes();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_CoerceTypes_English_US()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			try
			{
				Test_Equal_CoerceTypes();
				Test_NotEqual_CoerceTypes();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_CoerceTypes_English_Canada()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-CA");
			try
			{
				Test_Equal_CoerceTypes();
				Test_NotEqual_CoerceTypes();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_CoerceTypes_French_France()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
			try
			{
				Test_Equal_CoerceTypes();
				Test_NotEqual_CoerceTypes();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_CoerceTypes_French_Canada()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-CA");
			try
			{
				Test_Equal_CoerceTypes();
				Test_NotEqual_CoerceTypes();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		private static void Test_Equal_CoerceTypes()
		{
			EqualSpecification s = new EqualSpecification();
			s.RefValueExpression = new ConstantExpression("1.0");

			Assert.IsTrue(s.Test(1.0).Success);
			Assert.IsTrue(s.Test("1.0").Success);

			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(0.0).Success);
			Assert.IsFalse(s.Test("0.0").Success);
		}

		private static void Test_NotEqual_CoerceTypes()
		{
			NotEqualSpecification s = new NotEqualSpecification();
			s.RefValueExpression = new ConstantExpression("1.0");

			Assert.IsFalse(s.Test(1.0).Success);
			Assert.IsFalse(s.Test("1.0").Success);

			Assert.IsTrue(s.Test(null).Success);
			Assert.IsTrue(s.Test(0.0).Success);
			Assert.IsTrue(s.Test("0.0").Success);
		}
	}
}

#endif