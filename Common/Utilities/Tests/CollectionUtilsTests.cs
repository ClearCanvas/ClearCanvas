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
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
    [TestFixture]
    public class CollectionUtilsTests
    {
        class Foo : IComparable<Foo>
        {
            private int _num;

            public Foo(int num)
            {
                _num = num;
            }

            public int Number
            {
                get { return _num; }
            }

            #region IComparable<Foo> Members

            public int CompareTo(Foo other)
            {
                if (other == null)
                    return 1;
                return this.Number.CompareTo(other.Number);
            }

            #endregion
        }


        public CollectionUtilsTests()
        {

        }

        [Test]
        public void TestMinMaxWithValueType()
        {
            int[] numbers = new int[] { 3, 1, 2 };

            Assert.AreEqual(1, CollectionUtils.Min<int>(numbers, -1));
            Assert.AreEqual(3, CollectionUtils.Max<int>(numbers, -1));
        }

        [Test]
        public void TestMinMaxWithReferenceType()
        {
            string[] strings = new string[] { "b", "a", "d", "c" };

            Assert.AreEqual("a", CollectionUtils.Min<string>(strings));
            Assert.AreEqual("d", CollectionUtils.Max<string>(strings));

            string[] stringsWithNull = new string[] { null, "b", "a", "d", "c" };
            string[] stringsWithTwoNulls = new string[] { null, "b", "a", "d", null, "c" };

            // nulls are treated as less than any other value
            Assert.AreEqual(null, CollectionUtils.Min<string>(stringsWithNull));
            Assert.AreEqual("d", CollectionUtils.Max<string>(stringsWithNull));
            Assert.AreEqual(null, CollectionUtils.Min<string>(stringsWithTwoNulls));
            Assert.AreEqual("d", CollectionUtils.Max<string>(stringsWithTwoNulls));
        }

        [Test]
        public void TestMinMaxWithNullableType()
        {
            int?[] numbers = new int?[] { 3, 1, 2, null };

            // nulls are treated as less than any other value
            Assert.AreEqual(null, CollectionUtils.Min<int?>(numbers, null));
            Assert.AreEqual(3, CollectionUtils.Max<int?>(numbers, null));
        }

        [Test]
        public void TestMinMaxWithIComparableType()
        {
            Foo foo1 = new Foo(1);
            Foo foo2 = new Foo(2);
            Foo foo3 = new Foo(3);

            Foo[] foos = new Foo[] { foo3, foo1, foo2 };

            Assert.AreEqual(foo1, CollectionUtils.Min<Foo>(foos));
            Assert.AreEqual(foo3, CollectionUtils.Max<Foo>(foos));

            Foo[] foosWithNull = new Foo[] { foo3, foo1, foo2, null };

            // nulls are treated as less than any other value
            Assert.AreEqual(null, CollectionUtils.Min<Foo>(foosWithNull));
            Assert.AreEqual(foo3, CollectionUtils.Max<Foo>(foosWithNull));
        }

        [Test]
        public void TestMinMaxWithCustomComparison()
        {
            Foo foo1 = new Foo(1);
            Foo foo2 = new Foo(2);
            Foo foo3 = new Foo(3);

            Foo[] foosWithNull = new Foo[] { foo3, foo1, foo2, null };

            Assert.AreEqual(null, CollectionUtils.Min<Foo>(foosWithNull, null, CompareTreatNullAsMin));
            Assert.AreEqual(foo1, CollectionUtils.Min<Foo>(foosWithNull, null, CompareTreatNullAsMax));

            Assert.AreEqual(foo3, CollectionUtils.Max<Foo>(foosWithNull, null, CompareTreatNullAsMin));
            Assert.AreEqual(null, CollectionUtils.Max<Foo>(foosWithNull, null, CompareTreatNullAsMax));
        }

        /// <summary>
        /// Comparison that treats a null as less than any other value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CompareTreatNullAsMin(Foo x, Foo y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            return x.Number.CompareTo(y.Number);
        }

        /// <summary>
        /// Comparison that treats a null as greater than any other value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CompareTreatNullAsMax(Foo x, Foo y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            return x.Number.CompareTo(y.Number);
        }

		[Test]
		public void TestGroupBy()
		{
			string[] names = {"Ana", "Bill", "Cara", "Avalon", "Bonita"};

			Dictionary<char, List<string>> groups = 
				CollectionUtils.GroupBy<string, char>(names,
					delegate (string name) { return Char.ToLower(name[0]);});

			Assert.IsTrue(groups.ContainsKey('a'));
			Assert.IsTrue(groups.ContainsKey('b'));
			Assert.IsTrue(groups.ContainsKey('c'));

			Assert.AreEqual(2, groups['a'].Count);
			Assert.AreEqual(2, groups['b'].Count);
			Assert.AreEqual(1, groups['c'].Count);

			// note: each group list must contain items in order they first appeared
			Assert.AreEqual("Ana", groups['a'][0]);
			Assert.AreEqual("Avalon", groups['a'][1]);
			Assert.AreEqual("Bill", groups['b'][0]);
			Assert.AreEqual("Bonita", groups['b'][1]);
			Assert.AreEqual("Cara", groups['c'][0]);
		}
	}
}

#endif
