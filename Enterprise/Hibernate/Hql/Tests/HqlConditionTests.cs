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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Hibernate.Hql.Tests
{
    [TestFixture]
    public class HqlConditionTests
    {


		public HqlConditionTests()
        {
        }

        [Test]
        public void TestExpressionFactoryMethods()
        {
            AreEqual(new HqlCondition("x = ?", new object[] { 1 }), HqlCondition.EqualTo("x", 1));
			AreEqual(new HqlCondition("x <> ?", new object[] { 1 }), HqlCondition.NotEqualTo("x", 1));
			AreEqual(new HqlCondition("x like ?", new object[] { "foo" }), HqlCondition.Like("x", "foo"));
			AreEqual(new HqlCondition("x not like ?", new object[] { "foo" }), HqlCondition.NotLike("x", "foo"));
			AreEqual(new HqlCondition("x between ? and ?", new object[] { 1, 2 }), HqlCondition.Between("x", 1, 2));
			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.In("x", 1, 2, 3));
			AreEqual(new HqlCondition("x not in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.NotIn("x", 1, 2, 3));
			AreEqual(new HqlCondition("x < ?", new object[] { 1 }), HqlCondition.LessThan("x", 1));
			AreEqual(new HqlCondition("x <= ?", new object[] { 1 }), HqlCondition.LessThanOrEqual("x", 1));
			AreEqual(new HqlCondition("x > ?", new object[] { 1 }), HqlCondition.MoreThan("x", 1));
			AreEqual(new HqlCondition("x >= ?", new object[] { 1 }), HqlCondition.MoreThanOrEqual("x", 1));
			AreEqual(new HqlCondition("x is null", new object[] {}), HqlCondition.IsNull("x"));
			AreEqual(new HqlCondition("x is not null", new object[] {}), HqlCondition.IsNotNull("x"));
		}

		[Test]
		public void TestExpressionFromSearchCriteria()
		{
			AreEqual(new HqlCondition("x = ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.Equal, new object[] { 1 }));
			AreEqual(new HqlCondition("x <> ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.NotEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x like ?", new object[] { "foo" }), HqlCondition.GetCondition("x", SearchConditionTest.Like, new object[] { "foo" }));
			AreEqual(new HqlCondition("x not like ?", new object[] { "foo" }), HqlCondition.GetCondition("x", SearchConditionTest.NotLike, new object[] { "foo" }));
			AreEqual(new HqlCondition("x between ? and ?", new object[] { 1, 2 }), HqlCondition.GetCondition("x", SearchConditionTest.Between, new object[] { 1,2 }));
			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.GetCondition("x", SearchConditionTest.In, new object[] { 1,2,3 }));
			AreEqual(new HqlCondition("x not in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.GetCondition("x", SearchConditionTest.NotIn, new object[] { 1, 2, 3 }));
			AreEqual(new HqlCondition("x < ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.LessThan, new object[] { 1 }));
			AreEqual(new HqlCondition("x <= ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.LessThanOrEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x > ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.MoreThan, new object[] { 1 }));
			AreEqual(new HqlCondition("x >= ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.MoreThanOrEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x is null", new object[] { }), HqlCondition.GetCondition("x", SearchConditionTest.Null, new object[] { 1 }));
			AreEqual(new HqlCondition("x is not null", new object[] { }), HqlCondition.GetCondition("x", SearchConditionTest.NotNull, new object[] { 1 }));
		}

		[Test]
		public void TestInList()
		{
			List<int> numbers = new List<int>();
			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);

			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.In("x", numbers));
		}

		private static void AreEqual(HqlCondition c1, HqlCondition c2)
		{
			Assert.AreEqual(c1.Hql, c2.Hql);
			Assert.IsTrue(CollectionUtils.Equal<object>(c1.Parameters, c2.Parameters, true));
		}
    }
}

#endif
