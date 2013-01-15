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

using NUnit.Framework;

namespace ClearCanvas.Enterprise.Core.Tests
{
	[TestFixture]
	public class SearchCriteriaTests
	{
		public class Foo
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public Bar Bar { get; set; }
		}

		public class Bar
		{
			public string Description { get; set; }
		}


		class ConcreteSearchCriteria : SearchCriteria
		{
			internal ConcreteSearchCriteria(string key)
				:base(key)
			{
			}

			internal ConcreteSearchCriteria()
			{
			}

			internal ConcreteSearchCriteria(SearchCriteria other)
				:base(other)
			{
			}

			public override object Clone()
			{
				return new ConcreteSearchCriteria(this);
			}
		}

		class ConstantSearchCondition : SearchConditionBase
		{
			private readonly bool _result;

			public ConstantSearchCondition(bool result)
			{
				_result = result;
			}

			protected internal override bool IsSatisfiedBy(object value)
			{
				return _result;
			}

			public override object Clone()
			{
				return new ConstantSearchCondition(_result);
			}

			public override bool IsEmpty
			{
				get { return false; }
			}
		}

		class EqualitySearchCondition : SearchConditionBase
		{
			private readonly object _value;

			public EqualitySearchCondition(object value)
			{
				_value = value;
			}

			protected internal override bool IsSatisfiedBy(object value)
			{
				return Equals(value, _value);
			}

			public override object Clone()
			{
				return new EqualitySearchCondition(_value);
			}

			public override bool IsEmpty
			{
				get { return false; }
			}
		}


		[Test]
		public void Test_Constructor_Default()
		{
			var sc = new ConcreteSearchCriteria();
			Assert.IsTrue(sc.IsEmpty);
			Assert.AreEqual(null, sc.GetKey());
		}

		[Test]
		public void Test_Constructor_Key()
		{
			var sc = new ConcreteSearchCriteria("foo");
			Assert.IsTrue(sc.IsEmpty);
			Assert.AreEqual("foo", sc.GetKey());
		}

		[Test]
		public void Test_Constructor_Copy()
		{
			var sc = new ConcreteSearchCriteria("foo");
			Assert.IsTrue(sc.IsEmpty);
			Assert.AreEqual("foo", sc.GetKey());

			var sub = new ConcreteSearchCriteria("baz");
			sc.SubCriteria.Add("baz", sub);

			var copy = new ConcreteSearchCriteria(sc);
			Assert.AreEqual("foo", copy.GetKey());
			Assert.AreEqual(1, copy.SubCriteria.Count);
			Assert.IsTrue(copy.SubCriteria.ContainsKey("baz"));

			// check that the sub-criteria was actually cloned, not just copied
			Assert.IsFalse(ReferenceEquals(sc.SubCriteria["baz"], copy.SubCriteria["baz"]));
		}

		[Test]
		public void Test_IsEmpty()
		{
			var sc = new ConcreteSearchCriteria();
			Assert.IsTrue(sc.IsEmpty);

			var baz = new ConcreteSearchCriteria("baz");
			sc.SubCriteria.Add("baz", baz);

			// should still be empty, because baz is empty
			Assert.IsTrue(sc.IsEmpty);

			baz.SubCriteria.Add("moo", new ConstantSearchCondition(true));

			// now it should be false, because baz has a non-empty sub condition
			Assert.IsFalse(sc.IsEmpty);
		}


		[Test]
		public void Test_IsSatisfiedBy_Empty()
		{
			var sc = new ConcreteSearchCriteria();
			Assert.IsTrue(sc.IsEmpty);

			// an empty criteria can be satisfied by any object, or null
			Assert.IsTrue(sc.IsSatisfiedBy(new object()));
			Assert.IsTrue(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_SingleCondition()
		{
			var sc = new ConcreteSearchCriteria();
			var foo = new Foo() {Name = "Bob"};
			var goo = new Foo() {Name = "Lena"};

			sc.SubCriteria["Name"] = new EqualitySearchCondition("Bob");
			Assert.IsTrue(sc.IsSatisfiedBy(foo));
			Assert.IsFalse(sc.IsSatisfiedBy(goo));

			sc.SubCriteria["Name"] = new EqualitySearchCondition("Lena");
			Assert.IsFalse(sc.IsSatisfiedBy(foo));
			Assert.IsTrue(sc.IsSatisfiedBy(goo));
		}

		[Test]
		public void Test_IsSatisfiedBy_SingleCondition_Null()
		{
			var sc = new ConcreteSearchCriteria();
			sc.SubCriteria["Name"] = new EqualitySearchCondition("Bob");

			sc.SubCriteria["Name"] = new EqualitySearchCondition("Bob");
			Assert.IsFalse(sc.IsSatisfiedBy(null));

			// due to null propagation, sc is satisfied
			// (because null is propagated to Name sub-criteria)
			sc.SubCriteria["Name"] = new EqualitySearchCondition(null);
			Assert.IsTrue(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_MultiCondition()
		{
			var foo = new Foo() { Name = "Bob", Age = 10};
			var hoo = new Foo() { Name = "Bob", Age = 12 };
			var goo = new Foo() { Name = "Lena", Age = 10 };

			var sc = new ConcreteSearchCriteria();
			sc.SubCriteria["Name"] = new EqualitySearchCondition("Bob");
			sc.SubCriteria["Age"] = new EqualitySearchCondition(10);

			Assert.IsFalse(sc.IsSatisfiedBy(hoo));
			Assert.IsFalse(sc.IsSatisfiedBy(goo));
			Assert.IsTrue(sc.IsSatisfiedBy(foo));

			sc.SubCriteria["Name"] = new EqualitySearchCondition("Lena");

			Assert.IsFalse(sc.IsSatisfiedBy(hoo));
			Assert.IsTrue(sc.IsSatisfiedBy(goo));
			Assert.IsFalse(sc.IsSatisfiedBy(foo));
		}

		[Test]
		public void Test_IsSatisfiedBy_MissingProperty()
		{
			var foo = new Foo();
			var sc = new ConcreteSearchCriteria();

			// sc is not satisfied because Foo does not contain a property called Moo,
			// even though the Moo condition is always true
			// (this is the current behaviour... should it throw an exception instead?)
			sc.SubCriteria["Moo"] = new ConstantSearchCondition(true);
			Assert.IsFalse(sc.IsSatisfiedBy(foo));

			// sc is not satisfied because Foo does not contain a property called Moo,
			// even though the Moo condition is equality with null
			// (this is the current behaviour... should it throw an exception instead?)
			sc.SubCriteria["Moo"] = new EqualitySearchCondition(null);
			Assert.IsFalse(sc.IsSatisfiedBy(foo));

			// all properties are "missing" from null, by definition
			// but in this case, sc is satisfied by null, due to null propagation
			// (because the Moo condition is equality with null)
			Assert.IsTrue(sc.IsSatisfiedBy(null));

			// all properties are "missing" from null, by definition
			// in this case, sc is not satisfied by null, due to null propagation
			// (because the Moo condition is equality with "Bob")
			sc.SubCriteria["Moo"] = new EqualitySearchCondition("Bob");
			Assert.IsFalse(sc.IsSatisfiedBy(null));

		}

		[Test]
		public void Test_IsSatisfiedBy_Recursive()
		{
			var foo = new Foo() {Bar = new Bar() {Description = "xag"} };
			var goo = new Foo() { Bar = new Bar() { Description = "yyy" } };
			var hoo = new Foo() { Bar = new Bar() };
			var ioo = new Foo();

			var sc = new ConcreteSearchCriteria();
			sc.SubCriteria["Bar"] = new ConcreteSearchCriteria();
			sc.SubCriteria["Bar"].SubCriteria["Description"] = new EqualitySearchCondition("xag");

			Assert.IsTrue(sc.IsSatisfiedBy(foo));
			Assert.IsFalse(sc.IsSatisfiedBy(goo));
			Assert.IsFalse(sc.IsSatisfiedBy(hoo));
			Assert.IsFalse(sc.IsSatisfiedBy(ioo));
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_NullPropagation()
		{
			var ioo = new Foo();

			var sc = new ConcreteSearchCriteria();
			sc.SubCriteria["Bar"] = new ConcreteSearchCriteria();
			sc.SubCriteria["Bar"].SubCriteria["Description"] = new EqualitySearchCondition(null);

			// null-valued Bar is propagated through the Description condition
			Assert.IsTrue(sc.IsSatisfiedBy(ioo));
			Assert.IsTrue(sc.IsSatisfiedBy(null));

			sc.SubCriteria["Bar"].SubCriteria["Description"] = new EqualitySearchCondition("Bob");

			// null-valued Bar is propagated through the Description condition
			Assert.IsFalse(sc.IsSatisfiedBy(ioo));
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}
	}
}

#endif
