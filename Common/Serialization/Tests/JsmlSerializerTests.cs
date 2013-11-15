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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.Text;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Common.Serialization.Tests
{
	[TestFixture]
	public class JsmlSerializerTests
	{
		public enum TestEnum { Enum1, Enum2 }

		[DataContract]
		class TestContract1
		{
			[DataMember]
			public string Foo;

			public string Jsml
			{
				get
				{
					var builder = new StringBuilder();

					if (this.Foo != null)
					{
						builder.Append("<Tag type=\"hash\">");
						builder.Append("\r\n  ");
						builder.AppendFormat("<Foo>{0}</Foo>", this.Foo);
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
				return Equals(this.Foo, other.Foo);
			}

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
		}

		/// <summary>
		/// This class is for testing multiple data members.  Also test when a particular data member is filtered out by <see cref="JsmlSerializer.SerializeOptions"/>.
		/// </summary>
		[DataContract]
		class TestContract2
		{
			[DataMember]
			public Double Double;

			[DataMember]
			public bool Bool;

			[DataMember]
			public DateTime? NullableDateTime;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;

			public string Jsml
			{
				get { return GetJsml(false); }
			}

			public string LegacyJsml
			{
				get { return this.Jsml.Replace("type=\"hash\"", "hash=\"true\"").Replace("type=\"array\"", "array=\"true\""); }
			}

			public string GetJsml(bool filteredOutDouble)
			{
				var builder = new StringBuilder();
				builder.Append("<Tag type=\"hash\">");

				if (!filteredOutDouble)
				{
					builder.Append("\r\n  ");
					builder.AppendFormat("<Double>{0}</Double>", this.Double);
				}

				builder.Append("\r\n  ");
				builder.AppendFormat("<Bool>{0}</Bool>", this.Bool.ToString().ToLower());

				if (this.NullableDateTime != null)
				{
					builder.Append("\r\n  ");
					builder.AppendFormat("<NullableDateTime>{0}</NullableDateTime>", DateTimeUtils.FormatISO(this.NullableDateTime.Value));
				}

				if (this.ExtendedProperties != null)
				{
					builder.Append("\r\n  ");
					builder.Append("<ExtendedProperties type=\"hash\">");
					foreach (var kvp in this.ExtendedProperties)
					{
						builder.Append("\r\n    ");
						builder.AppendFormat("<{0}>{1}</{0}>", kvp.Key, kvp.Value);
					}
					builder.Append("\r\n  ");
					builder.Append("</ExtendedProperties>");
				}

				builder.Append("\r\n</Tag>");

				return builder.ToString();
			}

			public override bool Equals(object obj)
			{
				var other = obj as TestContract2;
				return Equals(this.Double, other.Double)
					&& Equals(this.Bool, other.Bool)
					&& Equals(this.NullableDateTime, other.NullableDateTime)
					&& (this.ExtendedProperties == null || other.ExtendedProperties == null
							? Equals(this.ExtendedProperties, other.ExtendedProperties)
							: CollectionUtils.Equal((ICollection)this.ExtendedProperties, other.ExtendedProperties, true));
			}

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
		}

		[DataContract]
		class TestContract3
		{
			// used by deserializer
			private TestContract3()
			{
			}

			public TestContract3(string value)
			{
				this.Value = value;
			}

			[DataMember]
			public string Value;
		}

		class TestPolyDataContractAttribute : PolymorphicDataContractAttribute
		{
			public TestPolyDataContractAttribute(string dataContractGuid) : base(dataContractGuid)
			{
			}
		}

		[DataContract]
		class TestContract4
		{
			[DataMember]
			public string Data { get; set; }

			[DataMember]
			public TestContract4 Child { get; set; }
		}

		[DataContract]
		[TestPolyDataContractAttribute("851f5c87-52d5-4543-b4ec-7fa4a820ee24")]
		class TestContract4_A : TestContract4
		{
			[DataMember]
			public string DataA { get; set; }
		}

		[DataContract]
		[TestPolyDataContractAttribute("6f406a66-f9a4-44e4-aceb-22374022dde8")]
		class TestContract4_B : TestContract4
		{
			[DataMember]
			public string DataB { get; set; }
		}

		[DataContract]
		public enum TestEnumWithDataContract { Enum1, Enum2 }

		[DataContract]
		class TestContract5
		{
			[DataMember]
			public TestEnumWithDataContract Data { get; set; }

			public override bool Equals(object obj)
			{
				var that = obj as TestContract5;
				return that != null && that.Data == this.Data;
			}

		    public bool Equals(TestContract5 other)
		    {
		        if (ReferenceEquals(null, other)) return false;
		        if (ReferenceEquals(this, other)) return true;
		        return Equals(other.Data, Data);
		    }

		    public override int GetHashCode()
		    {
		        return Data.GetHashCode();
		    }
		}

		[DataContract]
		class TestContract6
		{
			public List<string> Values { get; set; }

			public string[] Names { get; set; }
		}


		public JsmlSerializerTests()
		{
			Platform.SetExtensionFactory(new UnitTestExtensionFactory());
		}

		[TestFixtureSetUp]
		public void InitTest()
		{
			PolymorphicDataContractHook<TestPolyDataContractAttribute>.ClearKnownTypes();
		}

		[Test]
		public void Test_Null()
		{
			SerializeHelper(null, null);
			DeserializeHelper<object>(null, null);
			DeserializeHelper<object>(null, "");
		}

		[Test]
		public void Test_String()
		{
			// Empty string
			SerializeHelper("", "<Tag />");
			DeserializeHelper("", "<Tag />");
			DeserializeHelper("", "<Tag></Tag>");

			// Alphabets
			SerializeHelper("abcdefghijklmnopqrstuvwxyz", "<Tag>abcdefghijklmnopqrstuvwxyz</Tag>");
			DeserializeHelper("abcdefghijklmnopqrstuvwxyz", "<Tag>abcdefghijklmnopqrstuvwxyz</Tag>");

			// Numbers
			SerializeHelper("000", "<Tag>000</Tag>");
			DeserializeHelper("000", "<Tag>000</Tag>");
			SerializeHelper("1234567890", "<Tag>1234567890</Tag>");
			DeserializeHelper("1234567890", "<Tag>1234567890</Tag>");

			// Unusual characters
			SerializeHelper(@"`~!@#$%^*()-_=+[{]}\|;:,./?", @"<Tag>`~!@#$%^*()-_=+[{]}\|;:,./?</Tag>");
			DeserializeHelper(@"`~!@#$%^*()-_=+[{]}\|;:,./?", @"<Tag>`~!@#$%^*()-_=+[{]}\|;:,./?</Tag>");

			// Single and double quotes
			SerializeHelper("''", @"<Tag>''</Tag>");
			DeserializeHelper("''", @"<Tag>''</Tag>");
			SerializeHelper(@"""", @"<Tag>""</Tag>");
			DeserializeHelper(@"""", @"<Tag>""</Tag>");

			// Escaped characters
			SerializeHelper(@"&<>", @"<Tag>&amp;&lt;&gt;</Tag>");
			DeserializeHelper(@"&<>", @"<Tag>&amp;&lt;&gt;</Tag>");
		}

		[Test]
		public void Test_Guid()
		{

			var guid = Guid.NewGuid();
			var guidString = guid.ToString("N");
			var expectedJsml = "<Tag>" + guidString + "</Tag>";

			SerializeHelper(guid, expectedJsml);
			DeserializeHelper(guid, expectedJsml);
		}

		[Test]
		public void Test_CR_LF_Spaces()
		{
			// CR, LF, spaces between words
			SerializeHelper("ABC\r\nDEF", "<Tag>ABC\r\nDEF</Tag>");
			DeserializeHelper("ABC\r\nDEF", "<Tag>ABC\r\nDEF</Tag>");
			SerializeHelper("ABC\rDEF", "<Tag>ABC\rDEF</Tag>");
			DeserializeHelper("ABC\rDEF", "<Tag>ABC\rDEF</Tag>");
			SerializeHelper("ABC\nDEF", "<Tag>ABC\nDEF</Tag>");
			DeserializeHelper("ABC\nDEF", "<Tag>ABC\nDEF</Tag>");
			SerializeHelper(" ABC DEF ", "<Tag> ABC DEF </Tag>");
			DeserializeHelper(" ABC DEF ", "<Tag> ABC DEF </Tag>");

			// CR, LF, spaces before words
			SerializeHelper("\r\nDEF", "<Tag>\r\nDEF</Tag>");
			DeserializeHelper("\r\nDEF", "<Tag>\r\nDEF</Tag>");
			SerializeHelper("\rDEF", "<Tag>\rDEF</Tag>");
			DeserializeHelper("\rDEF", "<Tag>\rDEF</Tag>");
			SerializeHelper("\nDEF", "<Tag>\nDEF</Tag>");
			DeserializeHelper("\nDEF", "<Tag>\nDEF</Tag>");
			SerializeHelper("  DEF", "<Tag>  DEF</Tag>");
			DeserializeHelper("  DEF", "<Tag>  DEF</Tag>");

			// CR, LF, spaces after words
			SerializeHelper("ABC\r\n", "<Tag>ABC\r\n</Tag>");
			DeserializeHelper("ABC\r\n", "<Tag>ABC\r\n</Tag>");
			SerializeHelper("ABC\r", "<Tag>ABC\r</Tag>");
			DeserializeHelper("ABC\r", "<Tag>ABC\r</Tag>");
			SerializeHelper("ABC\n", "<Tag>ABC\n</Tag>");
			DeserializeHelper("ABC\n", "<Tag>ABC\n</Tag>");
			SerializeHelper("ABC  ", "<Tag>ABC  </Tag>");
			DeserializeHelper("ABC  ", "<Tag>ABC  </Tag>");

			// CR, LF, whitespace by themselves
			SerializeHelper("\r\n", "<Tag>\r\n</Tag>");
			DeserializeHelper("\r\n", "<Tag>\r\n</Tag>");
			SerializeHelper("\r\n\r\n", "<Tag>\r\n\r\n</Tag>");
			DeserializeHelper("\r\n\r\n", "<Tag>\r\n\r\n</Tag>");
			SerializeHelper(" ", "<Tag> </Tag>");
			DeserializeHelper(" ", "<Tag> </Tag>");
			SerializeHelper("  ", "<Tag>  </Tag>");
			DeserializeHelper("  ", "<Tag>  </Tag>");
			SerializeHelper(" \r\n", "<Tag> \r\n</Tag>");
			DeserializeHelper(" \r\n", "<Tag> \r\n</Tag>");
			SerializeHelper("  \r\n", "<Tag>  \r\n</Tag>");
			DeserializeHelper("  \r\n", "<Tag>  \r\n</Tag>");
		}

		[Test]
		public void Test_Int()
		{
			SerializeHelper(0, "<Tag>0</Tag>");
			DeserializeHelper(0, "<Tag>0</Tag>");

			SerializeHelper(00, "<Tag>0</Tag>");
			DeserializeHelper(00, "<Tag>0</Tag>");

			SerializeHelper(5, "<Tag>5</Tag>");
			DeserializeHelper(5, "<Tag>5</Tag>");

			SerializeHelper(99999, "<Tag>99999</Tag>");
			DeserializeHelper(99999, "<Tag>99999</Tag>");

			SerializeHelper(-1, "<Tag>-1</Tag>");
			DeserializeHelper(-1, "<Tag>-1</Tag>");

			SerializeHelper(-99999, "<Tag>-99999</Tag>");
			DeserializeHelper(-99999, "<Tag>-99999</Tag>");
		}

		[Test]
		public void Test_NullableInt()
		{
			int? n = null;
			SerializeHelper(n, null);
			DeserializeHelper(n, null);

			n = 0;
			SerializeHelper(n, "<Tag>0</Tag>");
			DeserializeHelper(n, "<Tag>0</Tag>");

			n = 1;
			SerializeHelper(n, "<Tag>1</Tag>");
			DeserializeHelper(n, "<Tag>1</Tag>");

			n = -1;
			SerializeHelper(n, "<Tag>-1</Tag>");
			DeserializeHelper(n, "<Tag>-1</Tag>");
		}

		[Test]
		public void Test_Double()
		{
			SerializeHelper(0.00, "<Tag>0</Tag>");
			DeserializeHelper(0.00, "<Tag>0</Tag>");

			SerializeHelper(5.1, "<Tag>5.1</Tag>");
			DeserializeHelper(5.1, "<Tag>5.1</Tag>");

			SerializeHelper(-5.1, "<Tag>-5.1</Tag>");
			DeserializeHelper(-5.1, "<Tag>-5.1</Tag>");

			SerializeHelper(0.99999999, "<Tag>0.99999999</Tag>");
			DeserializeHelper(0.99999999, "<Tag>0.99999999</Tag>");

			SerializeHelper(0.001, "<Tag>0.001</Tag>");
			DeserializeHelper(0.001, "<Tag>0.001</Tag>");

			// Scientific notations.  Note that the serialized is always capitalized and padded.  i.e. E-08 rather than E-8
			// Make sure the lower case and e-8 and e-08 can all be deserialized.
			SerializeHelper(0.000000015, "<Tag>1.5E-08</Tag>");
			SerializeHelper(1.5E-08, "<Tag>1.5E-08</Tag>");
			SerializeHelper(15E-09, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(0.000000015, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5E-08</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5e-8</Tag>");
			DeserializeHelper(1.5E-8, "<Tag>1.5e-08</Tag>");
		}

		[Test]
		public void Test_Double_French()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-CA");
				Test_Double();

				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
				Test_Double();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_Double_English()
		{
			var originalCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-CA");
				Test_Double();

				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
				Test_Double();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		[Test]
		public void Test_NullableDouble()
		{
			double? n = null;
			SerializeHelper(n, null);
			DeserializeHelper(n, null);

			n = 0;
			SerializeHelper(n, "<Tag>0</Tag>");
			DeserializeHelper(n, "<Tag>0</Tag>");
			DeserializeHelper(n, "<Tag>0.0</Tag>");
			DeserializeHelper(n, "<Tag>0.000</Tag>");

			n = 5.1;
			SerializeHelper(n, "<Tag>5.1</Tag>");
			DeserializeHelper(n, "<Tag>5.1</Tag>");
		}

		[Test]
		public void Test_Bool()
		{
			SerializeHelper(true, "<Tag>true</Tag>");
			DeserializeHelper(true, "<Tag>true</Tag>");

			SerializeHelper(false, "<Tag>false</Tag>");
			DeserializeHelper(false, "<Tag>false</Tag>");
		}

		[Test]
		public void Test_Bool_deserialize_case_insensitive()
		{
			DeserializeHelper(true, "<Tag>true</Tag>");
			DeserializeHelper(true, "<Tag>True</Tag>");
			DeserializeHelper(true, "<Tag>TRUE</Tag>");
			DeserializeHelper(true, "<Tag>trUE</Tag>");

			DeserializeHelper(false, "<Tag>false</Tag>");
			DeserializeHelper(false, "<Tag>False</Tag>");
			DeserializeHelper(false, "<Tag>FALSE</Tag>");
			DeserializeHelper(false, "<Tag>fAlSe</Tag>");
		}

		[Test]
		public void Test_Enum()
		{
			SerializeHelper(TestEnum.Enum1, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));
			DeserializeHelper(TestEnum.Enum1, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));

			SerializeHelper(TestEnum.Enum2, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
			DeserializeHelper(TestEnum.Enum2, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
		}

		[Test]
		public void Test_Enum_deserialize_case_insensitive()
		{
			DeserializeHelper(TestEnum.Enum1, "<Tag>enum1</Tag>");
			DeserializeHelper(TestEnum.Enum1, "<Tag>ENUM1</Tag>");
			DeserializeHelper(TestEnum.Enum1, "<Tag>eNUM1</Tag>");

			DeserializeHelper(TestEnum.Enum2, "<Tag>enum2</Tag>");
			DeserializeHelper(TestEnum.Enum2, "<Tag>ENUM2</Tag>");
			DeserializeHelper(TestEnum.Enum2, "<Tag>eNuM2</Tag>");
		}

		[Test]
		public void Test_NullableEnum()
		{
			TestEnum? v = null;
			SerializeHelper(v, null);
			DeserializeHelper(v, null);

			v = TestEnum.Enum1;
			SerializeHelper(v, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));
			DeserializeHelper(v, string.Format("<Tag>{0}</Tag>", TestEnum.Enum1));

			v = TestEnum.Enum2;
			SerializeHelper(v, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
			DeserializeHelper(v, string.Format("<Tag>{0}</Tag>", TestEnum.Enum2));
		}

		[Test]
		public void Test_DateTime()
		{
			var now = DateTime.Now;

			// Serializer do not support milliseconds and below.
			var nowWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			SerializeHelper(nowWithoutMilliseconds, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nowWithoutMilliseconds)));
			DeserializeHelper(nowWithoutMilliseconds, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nowWithoutMilliseconds)));
		}

		[Test]
		public void Test_NullableDateTime()
		{
			var now = DateTime.Now;
			var nowWithoutMilliseconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			var nullable = (DateTime?)nowWithoutMilliseconds;

			SerializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nullable.Value)));
			DeserializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatISO(nullable.Value)));
		}

		[Test]
		public void Test_TimeSpan()
		{
			var oneHour = new TimeSpan(1, 0, 0);

			SerializeHelper(oneHour, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatTimeSpan(oneHour)));
			DeserializeHelper(oneHour, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatTimeSpan(oneHour)));
		}

		[Test]
		public void Test_NullableTimeSpan()
		{
			var oneHour = new TimeSpan(1, 0, 0);
			TimeSpan? nullable = oneHour;

			SerializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatTimeSpan(nullable.Value)));
			DeserializeHelper(nullable, string.Format("<Tag>{0}</Tag>", DateTimeUtils.FormatTimeSpan(nullable.Value)));
		}

		[Test]
		public void Test_List()
		{
			var emptyList = new List<object>();
			SerializeHelper(emptyList, "<Tag type=\"array\" />");
			DeserializeHelper(emptyList, "<Tag type=\"array\" />");
			DeserializeHelper(emptyList, "<Tag type=\"array\"></Tag>");
			DeserializeHelper(emptyList, "<Tag array=\"true\" />");
			DeserializeHelper(emptyList, "<Tag array=\"true\"></Tag>");

			var stringList = new List<string> {"1", "2"};
			SerializeHelper(stringList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag type=\"array\"><item>1</item><item>2</item></Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\"><item>1</item><item>2</item></Tag>");

			var intList = new List<int> {1};
			SerializeHelper(intList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag type=\"array\"><item>1</item></Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\"><item>1</item></Tag>");

			var doubleList = new List<double> {1.0};
			SerializeHelper(doubleList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag type=\"array\"><item>1</item></Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\"><item>1</item></Tag>");

			var boolList = new List<bool> {true, false};
			SerializeHelper(boolList, "<Tag type=\"array\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag type=\"array\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag type=\"array\"><item>true</item><item>false</item></Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\"><item>true</item><item>false</item></Tag>");
		}

		[Test]
		public void Test_Array()
		{
			var emptyList = new object[0];
			SerializeHelper(emptyList, "<Tag type=\"array\" />");
			DeserializeHelper(emptyList, "<Tag type=\"array\" />");
			DeserializeHelper(emptyList, "<Tag type=\"array\"></Tag>");
			DeserializeHelper(emptyList, "<Tag array=\"true\" />");
			DeserializeHelper(emptyList, "<Tag array=\"true\"></Tag>");

			var stringList = new string[] { "1", "2" };
			SerializeHelper(stringList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag type=\"array\"><item>1</item><item>2</item></Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
			DeserializeHelper(stringList, "<Tag array=\"true\"><item>1</item><item>2</item></Tag>");

			var intList = new int[] { 1 };
			SerializeHelper(intList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag type=\"array\"><item>1</item></Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(intList, "<Tag array=\"true\"><item>1</item></Tag>");

			var doubleList = new double[] { 1.0 };
			SerializeHelper(doubleList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag type=\"array\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag type=\"array\"><item>1</item></Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\">\r\n  <item>1</item>\r\n</Tag>");
			DeserializeHelper(doubleList, "<Tag array=\"true\"><item>1</item></Tag>");

			var boolList = new bool[] { true, false };
			SerializeHelper(boolList, "<Tag type=\"array\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag type=\"array\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag type=\"array\"><item>true</item><item>false</item></Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\">\r\n  <item>true</item>\r\n  <item>false</item>\r\n</Tag>");
			DeserializeHelper(boolList, "<Tag array=\"true\"><item>true</item><item>false</item></Tag>");
		}

		[Test]
		public void Test_UntypedList_serialize()
		{
			var list = new ArrayList() { "1", "2" };
			SerializeHelper(list, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void Test_UntypedList_deserialize()
		{
			var list = new ArrayList() { "1", "2" };
			DeserializeHelper(list, "<Tag type=\"array\">\r\n  <item>1</item>\r\n  <item>2</item>\r\n</Tag>");
		}


		[Test]
		public void Test_Dictionary()
		{
			var emptyDictionary = new Dictionary<string, string>();
			SerializeHelper(emptyDictionary, "<Tag type=\"hash\" />");
			DeserializeHelper(emptyDictionary, "<Tag type=\"hash\" />");
			DeserializeHelper(emptyDictionary, "<Tag type=\"hash\"></Tag>");
			DeserializeHelper(emptyDictionary, "<Tag hash=\"true\" />");
			DeserializeHelper(emptyDictionary, "<Tag hash=\"true\"></Tag>");

			var strStrDictionary = new Dictionary<string, string> {{"key", "value"}};
			SerializeHelper(strStrDictionary, "<Tag type=\"hash\">\r\n  <key>value</key>\r\n</Tag>");
			DeserializeHelper(strStrDictionary, "<Tag type=\"hash\">\r\n  <key>value</key>\r\n</Tag>");
			DeserializeHelper(strStrDictionary, "<Tag type=\"hash\"><key>value</key></Tag>");
			DeserializeHelper(strStrDictionary, "<Tag hash=\"true\">\r\n  <key>value</key>\r\n</Tag>");
			DeserializeHelper(strStrDictionary, "<Tag hash=\"true\"><key>value</key></Tag>");

			var strIntDictionary = new Dictionary<string, int> {{"key", 5}};
			SerializeHelper(strIntDictionary, "<Tag type=\"hash\">\r\n  <key>5</key>\r\n</Tag>");
			DeserializeHelper(strIntDictionary, "<Tag type=\"hash\">\r\n  <key>5</key>\r\n</Tag>");
			DeserializeHelper(strIntDictionary, "<Tag type=\"hash\"><key>5</key></Tag>");
			DeserializeHelper(strIntDictionary, "<Tag hash=\"true\">\r\n  <key>5</key>\r\n</Tag>");
			DeserializeHelper(strIntDictionary, "<Tag hash=\"true\"><key>5</key></Tag>");
		}

		[Test]
		[ExpectedException(typeof(XmlException))]
		public void Test_Dictionary_InvalidKeyType()
		{
			// Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.
			var intStrDictionary = new Dictionary<int, string>();
			intStrDictionary[0] = "value";
			SerializeHelper(intStrDictionary, "<Tag type=\"hash\">\r\n  <0>value</0>\r\n</Tag>");
			DeserializeHelper(intStrDictionary, "<Tag type=\"hash\">\r\n  <0>value</0>\r\n</Tag>");
			DeserializeHelper(intStrDictionary, "<Tag hash=\"true\">\r\n  <0>value</0>\r\n</Tag>");
		}

		[Test]
		public void Test_DataContract()
		{
			var contract1 = new TestContract1();
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.LegacyJsml);

			contract1.Foo = "ClearCanvas.Healthcare.ExternalPractitioner, ClearCanvas.Healthcare:G:0fa8fdae-4678-40d7-bc54-9ca700e646d9:2";
			SerializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.Jsml);
			DeserializeHelper(contract1, contract1.LegacyJsml);

			var contract2 = new TestContract2();
			SerializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.LegacyJsml);

			var now = DateTime.Now;
			contract2.Double = 5.0;
			contract2.Bool = true;
			contract2.NullableDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			contract2.ExtendedProperties = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
			SerializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.Jsml);
			DeserializeHelper(contract2, contract2.LegacyJsml);
		}

		[Test]
		public void Test_DataContract_non_public_constructor()
		{
			var jsml = JsmlSerializer.Serialize(new TestContract3("foo"), "data");
			var obj = JsmlSerializer.Deserialize<TestContract3>(jsml);
			Assert.AreEqual("foo", obj.Value);
		}

		[Test]
		public void Test_SerializeOptions()
		{
			var now = DateTime.Now;
			var options = new JsmlSerializer.SerializeOptions { DataMemberTest = (m => AttributeUtils.HasAttribute<DataMemberAttribute>(m) && m.Name != "Double") }; 
			var contract2 = new TestContract2
					{
						Double = 5.0,
						Bool = true,
						NullableDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second),
						ExtendedProperties = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}}
					};

			var jsmlWithoutDouble = contract2.GetJsml(true);
			SerializeHelper(contract2, jsmlWithoutDouble, options);
		}

		[Test]
		public void Test_Polymorhpic_data_contracts()
		{
			// normally it isn't necessary to explicitly add subtypes, if those types are defined in *plugins*
			// but since Common is not a plugin, we need to explicitly add the types
			PolymorphicDataContractHook<TestPolyDataContractAttribute>.RegisterKnownType(typeof(TestContract4_A));
			PolymorphicDataContractHook<TestPolyDataContractAttribute>.RegisterKnownType(typeof(TestContract4_B));


			TestContract4 input, output;
			input = new TestContract4_A() {Data = "foo", DataA = "bar", Child = new TestContract4_B {Data = "x", DataB = "b"}};
			var serialized = JsmlSerializer.Serialize(input, "data",
				new JsmlSerializer.SerializeOptions { Hook = new PolymorphicDataContractHook<TestPolyDataContractAttribute>() });


			output = JsmlSerializer.Deserialize<TestContract4>(serialized,
				new JsmlSerializer.DeserializeOptions { Hook = new PolymorphicDataContractHook<TestPolyDataContractAttribute>() });

            Assert.IsInstanceOf(typeof(TestContract4_A), output);
			Assert.AreEqual("foo", output.Data);

			Assert.AreEqual("bar", ((TestContract4_A) output).DataA);
            Assert.IsInstanceOf(typeof(TestContract4_B), output.Child);
			Assert.AreEqual("x", output.Child.Data);
			Assert.AreEqual("b", ((TestContract4_B)output.Child).DataB);
		}

		[Test]
		public void Test_Enum_with_DataContract_attribute_processed_as_enum()
		{
			var value = new TestContract5() {Data = TestEnumWithDataContract.Enum2};
			SerializeHelper(value, "<Tag type=\"hash\">\r\n  <Data>Enum2</Data>\r\n</Tag>");
			DeserializeHelper(value, "<Tag type=\"hash\">\r\n  <Data>Enum2</Data>\r\n</Tag>");
		}

		#region Private helpers

		private static void SerializeHelper(object input, string expectedJsml)
		{
			SerializeHelper(input, expectedJsml, JsmlSerializer.SerializeOptions.Default);
		}

		private static void SerializeHelper(object input, string expectedJsml, JsmlSerializer.SerializeOptions options)
		{
			var jsml = JsmlSerializer.Serialize(input, "Tag", options);
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
				Assert.IsTrue(CollectionUtils.Equal((ICollection) expectedValue, (ICollection) value, true));
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