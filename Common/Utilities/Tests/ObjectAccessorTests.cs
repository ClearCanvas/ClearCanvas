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

using System;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	class ObjectAccessorTests
	{
		class Foo
		{
			public string _readOnlyPropBackingField;
			public string _writeOnlyPropBackingField;

			public Foo(
				string readOnlyPropBackingField = null,
				string writeOnlyPropBackingField = null,
				string refProp = null,
				int valueProp = 0,
				string publicProp = null,
				string semiPublicProp = null,
				string privateProp = null
			)
			{
				_readOnlyPropBackingField = readOnlyPropBackingField;
				_writeOnlyPropBackingField = writeOnlyPropBackingField;
				RefProp = refProp;
				ValueProp = valueProp;
				PublicProp = publicProp;
				SemiPublicProp = semiPublicProp;
				PrivateProp = privateProp;
			}

			public string RefProp { get; set; }
			public int ValueProp { get; set; }

			public string PublicProp { get; set; }
			public string SemiPublicProp { get; private set; }
			private string PrivateProp { get; set; }

			public string ReadOnlyProp
			{
				get { return _readOnlyPropBackingField; }
			}

			public string WriteOnlyProp
			{
				set { _writeOnlyPropBackingField = value; }
			}

			public string GetPrivatePropValue()
			{
				return PrivateProp;
			}
		}


		[Test]
		public void Test_get_nonexistent_property_returns_null()
		{
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("NonExistantProp");
			Assert.IsNull(propAccessor);
		}

		[Test]
		public void Test_reference_property()
		{
			var foo = new Foo(refProp: "red");
			var propAccessor = ObjectAccessor.For(typeof (Foo)).GetProperty("RefProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsTrue(propAccessor.IsReadable);
			Assert.IsTrue(propAccessor.IsWritable);

			var value = propAccessor.GetValue(foo);
			Assert.AreEqual("red", value);

			propAccessor.SetValue(foo, "blue");
			Assert.AreEqual("blue", foo.RefProp);
		}

		[Test]
		public void Test_value_property()
		{
			var foo = new Foo(valueProp: 12);
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("ValueProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsTrue(propAccessor.IsReadable);
			Assert.IsTrue(propAccessor.IsWritable);

			var value = propAccessor.GetValue(foo);
			Assert.AreEqual(12, value);

			propAccessor.SetValue(foo, 13);
			Assert.AreEqual(13, foo.ValueProp);
		}

		[Test]
		public void Test_public_property()
		{
			var foo = new Foo(publicProp: "red");
			var publicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("PublicProp", false);
			Assert.IsNotNull(publicAccessor);
			Assert.IsTrue(publicAccessor.IsReadable);
			Assert.IsTrue(publicAccessor.IsWritable);

			var value = publicAccessor.GetValue(foo);
			Assert.AreEqual("red", value);

			publicAccessor.SetValue(foo, "blue");
			Assert.AreEqual("blue", foo.PublicProp);

			var nonPublicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("PublicProp", true);
			Assert.IsNotNull(nonPublicAccessor);
			Assert.IsTrue(nonPublicAccessor.IsReadable);
			Assert.IsTrue(nonPublicAccessor.IsWritable);

			value = nonPublicAccessor.GetValue(foo);
			Assert.AreEqual("blue", value);

			nonPublicAccessor.SetValue(foo, "red");
			Assert.AreEqual("red", foo.PublicProp);
		}

		[Test]
		public void Test_semipublic_property()
		{
			var foo = new Foo(semiPublicProp: "red");
			var publicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("SemiPublicProp", false);
			Assert.IsNotNull(publicAccessor);
			Assert.IsTrue(publicAccessor.IsReadable);

			// note that the semi-public property is writable
			// we don't respect the private setter, since the PropertyAccessor as a whole
			// can really only be one or the other
			Assert.IsTrue(publicAccessor.IsWritable);

			var value = publicAccessor.GetValue(foo);
			Assert.AreEqual("red", value);

			publicAccessor.SetValue(foo, "blue");
			Assert.AreEqual("blue", foo.SemiPublicProp);

			var nonPublicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("SemiPublicProp", true);
			Assert.IsNotNull(nonPublicAccessor);
			Assert.IsTrue(nonPublicAccessor.IsReadable);
			Assert.IsTrue(nonPublicAccessor.IsWritable);

			value = nonPublicAccessor.GetValue(foo);
			Assert.AreEqual("blue", value);

			nonPublicAccessor.SetValue(foo, "red");
			Assert.AreEqual("red", foo.SemiPublicProp);
		}

		[Test]
		public void Test_private_property()
		{
			var foo = new Foo(privateProp: "red");
			var publicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("PrivateProp", false);
			Assert.IsNull(publicAccessor); // can't get public accessor to private property

			var nonPublicAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("PrivateProp", true);
			Assert.IsNotNull(nonPublicAccessor);
			Assert.IsTrue(nonPublicAccessor.IsReadable);
			Assert.IsTrue(nonPublicAccessor.IsWritable);

			var value = nonPublicAccessor.GetValue(foo);
			Assert.AreEqual("red", value);

			nonPublicAccessor.SetValue(foo, "blue");
			Assert.AreEqual("blue", foo.GetPrivatePropValue());
		}

		[Test]
		public void Test_readonly_property()
		{
			var foo = new Foo(readOnlyPropBackingField: "red");
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("ReadOnlyProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsTrue(propAccessor.IsReadable);
			Assert.IsFalse(propAccessor.IsWritable);

			var value = propAccessor.GetValue(foo);
			Assert.AreEqual("red", value);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_readonly_property_throws_on_write_attempt()
		{
			var foo = new Foo(readOnlyPropBackingField: "red");
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("ReadOnlyProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsTrue(propAccessor.IsReadable);
			Assert.IsFalse(propAccessor.IsWritable);

			propAccessor.SetValue(foo, "blue");
		}

		[Test]
		public void Test_writeonly_property()
		{
			var foo = new Foo(writeOnlyPropBackingField: "red");
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("WriteOnlyProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsFalse(propAccessor.IsReadable);
			Assert.IsTrue(propAccessor.IsWritable);

			propAccessor.SetValue(foo, "blue");
			Assert.AreEqual("blue", foo._writeOnlyPropBackingField);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_writeonly_property_throws_on_read_attempt()
		{
			var foo = new Foo(writeOnlyPropBackingField: "red");
			var propAccessor = ObjectAccessor.For(typeof(Foo)).GetProperty("WriteOnlyProp");
			Assert.IsNotNull(propAccessor);
			Assert.IsFalse(propAccessor.IsReadable);
			Assert.IsTrue(propAccessor.IsWritable);

			propAccessor.GetValue(foo);
		}
	}
}

#endif
