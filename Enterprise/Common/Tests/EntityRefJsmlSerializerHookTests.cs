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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;
using System;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.Tests
{
	[TestFixture]
	public class EntityRefJsmlSerializerHookTests
	{
		public enum TestEnum { Enum1, Enum2 }

		/// <summary>
		/// This data contract is for testing EntityRef within a data contract.  Also test when a data contract has all its properties as null.
		/// </summary>
		[DataContract]
		class TestContract1 : DataContractBase
		{
			[DataMember]
			public EntityRef EntityRef;

			public string Jsml
			{
				get
				{
					var builder = new StringBuilder();

					if (this.EntityRef != null)
					{
						builder.Append("<Tag type=\"hash\">");
						builder.Append("\r\n  ");
						builder.AppendFormat("<EntityRef>{0}</EntityRef>", this.EntityRef.Serialize());
						builder.Append("\r\n</Tag>");
					}
					else
					{
						builder.Append("<Tag type=\"hash\" />");
					}

					return builder.ToString();
				}
			}

			public string LegacyJsml
			{
				get { return this.Jsml.Replace("type=\"hash\"", "hash=\"true\"").Replace("type=\"array\"", "array=\"true\""); }
			}

			public override bool Equals(object obj)
			{
				var other = obj as TestContract1;
				return Equals(this.EntityRef, other.EntityRef);
			}

			public override int GetHashCode()
			{
				return 0;	// this isn't used by unit tests, but we want to silence the compiler warning
			}
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			var map = new Dictionary<Type, Type>{{typeof (JsmlSerializerHookExtensionPoint), typeof (EntityRefJsmlSerializerHook)}};
			Platform.SetExtensionFactory(new UnitTestExtensionFactory(map));
		}

		[Test]
		public void Test_EntityRef()
		{
			var guidEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0");
			SerializeHelper(guidEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0</Tag>");
			DeserializeHelper(guidEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:0</Tag>");

			var stringEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1");
			SerializeHelper(stringEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1</Tag>");
			DeserializeHelper(stringEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:S:0fa8fdae-4678-40d7-bc54-9ca700e646d9:1</Tag>");

			var intEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2");
			SerializeHelper(intEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2</Tag>");
			DeserializeHelper(intEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:I:123456:2</Tag>");

			var longEntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3");
			SerializeHelper(longEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3</Tag>");
			DeserializeHelper(longEntityRef, "<Tag>ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:L:12345678901234567:3</Tag>");

			// Null EntityRef is acceptable.
			DeserializeHelper<EntityRef>(null, "");
		}

		[Test]
		[ExpectedException(typeof(SerializationException))]
		public void Test_EntityRef_EmptyTag()
		{
			// We always expect EntityRef jsml to contain something.  An empty EntityRef is not allowed.
			JsmlSerializer.Deserialize<EntityRef>("<Tag />");
		}

		[Test]
		public void Test_DataContract_containing_EntityRef()
		{
			var contract1 = new TestContract1();
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.LegacyJsml);

			contract1.EntityRef = new EntityRef("ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:2");
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.LegacyJsml);
		}

		#region Private helpers

		private static void SerializeHelper(object input, string expectedJsml)
		{
			var jsml = JsmlSerializer.Serialize(input, "Tag");
			Assert.AreEqual(expectedJsml, jsml);
		}

		private static void DeserializeHelper<T>(T expectedValue, string jsml)
		{
			var value = JsmlSerializer.Deserialize<T>(jsml);

			if (null == expectedValue)
			{
				Assert.IsNull(value);
			}
			else if (expectedValue is ICollection)
			{
				Assert.IsTrue(CollectionUtils.Equal((ICollection)expectedValue, (ICollection)value, true));
			}
			else
			{
				Assert.IsTrue(expectedValue.Equals(value));
			}
		}

		#endregion
	}
}

#endif