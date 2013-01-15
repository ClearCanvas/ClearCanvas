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

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using System.ComponentModel;
using NUnit.Framework;


namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class ItemCollectionTests
	{
		class Foo
		{
			public Foo(string name)
			{
				Name = name;
			}

			public string Name { get; set; }
		}

		[Test]
		public void Test_FindInsertionPoint_non_existant_item()
		{
			var items = new ItemCollection<Foo>();
			items.AddRange(new []{ new Foo("b"), new Foo("b"),  new Foo("d")});

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("a"), comparison));
			Assert.AreEqual(2, items.FindInsertionPoint(new Foo("c"), comparison));
			Assert.AreEqual(3, items.FindInsertionPoint(new Foo("e"), comparison));
		}

		[Test]
		public void Test_FindInsertionPoint_non_existing_item()
		{
			var items = new ItemCollection<Foo>();
			items.AddRange(new[] { new Foo("b"), new Foo("b"), new Foo("d") });

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			var b = items.FindInsertionPoint(new Foo("b"), comparison);
			Assert.IsTrue(b == 0 || b == 1);

			var d = items.FindInsertionPoint(new Foo("d"), comparison);
			Assert.IsTrue(d == 2);
		}

		[Test]
		public void Test_FindInsertionPoint_empty_list()
		{
			var items = new ItemCollection<Foo>();

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			// insertion point is always zero
			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("a"), comparison));
			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("d"), comparison));
		}
	}
}

#endif