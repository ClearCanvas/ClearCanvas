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

using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Tests
{
	[TestFixture]
	internal class DateTimeParserTests
	{
		[Test]
		public void TestGetDateTimeAttributeValue()
		{
			Assert.AreEqual("20130701123456.098765", DateTimeParser.GetDateTimeAttributeValues("20130701", "123456.098765"));
			Assert.AreEqual("20130701123456.0987", DateTimeParser.GetDateTimeAttributeValues("20130701", "123456.0987"));
			Assert.AreEqual("20130701123456.0", DateTimeParser.GetDateTimeAttributeValues("20130701", "123456.0"));
			Assert.AreEqual("20130701123456", DateTimeParser.GetDateTimeAttributeValues("20130701", "123456"));
			Assert.AreEqual("201307011234", DateTimeParser.GetDateTimeAttributeValues("20130701", "1234"));
			Assert.AreEqual("2013070112", DateTimeParser.GetDateTimeAttributeValues("20130701", "12"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateTimeAttributeValues("20130701", ""));
			Assert.AreEqual("201307", DateTimeParser.GetDateTimeAttributeValues("201307", ""));
			Assert.AreEqual("2013", DateTimeParser.GetDateTimeAttributeValues("2013", ""));

			Assert.AreEqual("201307", DateTimeParser.GetDateTimeAttributeValues("201307", "123456.098765"));
			Assert.AreEqual("2013", DateTimeParser.GetDateTimeAttributeValues("2013", "123456.098765"));

			Assert.AreEqual("2013075", DateTimeParser.GetDateTimeAttributeValues("2013075", "123456.098765"));
			Assert.AreEqual("123", DateTimeParser.GetDateTimeAttributeValues("123", "123456.098765"));
			Assert.AreEqual("", DateTimeParser.GetDateTimeAttributeValues("", "123456.098765"));
		}

		[Test]
		public void TestGetDateAttributeValue()
		{
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456.098765+0800"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456.098765-05"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456.098765"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456.0987"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456.0"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701123456"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("201307011234"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("2013070112"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("20130701"));
			Assert.AreEqual("20130701", DateTimeParser.GetDateAttributeValues("201307"));
			Assert.AreEqual("20130101", DateTimeParser.GetDateAttributeValues("2013"));

			Assert.AreEqual("123", DateTimeParser.GetDateAttributeValues("123"));
			Assert.AreEqual("", DateTimeParser.GetDateAttributeValues(""));
		}

		[Test]
		public void TestGetTimeAttributeValue()
		{
			Assert.AreEqual("123456.098765", DateTimeParser.GetTimeAttributeValues("20130701123456.098765+0800"));
			Assert.AreEqual("123456.098765", DateTimeParser.GetTimeAttributeValues("20130701123456.098765-05"));
			Assert.AreEqual("123456.098765", DateTimeParser.GetTimeAttributeValues("20130701123456.098765"));
			Assert.AreEqual("123456.0987", DateTimeParser.GetTimeAttributeValues("20130701123456.0987"));
			Assert.AreEqual("123456.0", DateTimeParser.GetTimeAttributeValues("20130701123456.0"));
			Assert.AreEqual("123456", DateTimeParser.GetTimeAttributeValues("20130701123456"));
			Assert.AreEqual("1234", DateTimeParser.GetTimeAttributeValues("201307011234"));
			Assert.AreEqual("12", DateTimeParser.GetTimeAttributeValues("2013070112"));
			Assert.AreEqual("", DateTimeParser.GetTimeAttributeValues("20130701"));
			Assert.AreEqual("", DateTimeParser.GetTimeAttributeValues("201307"));
			Assert.AreEqual("", DateTimeParser.GetTimeAttributeValues("2013"));

			Assert.AreEqual("", DateTimeParser.GetTimeAttributeValues("123"));
			Assert.AreEqual("", DateTimeParser.GetTimeAttributeValues(""));
		}
	}
}

#endif