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
using System.Globalization;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class DateTimeTests
	{
		[TestFixtureSetUp]
		public void Init()
		{ 
		
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestDateParser()
		{
			//Valid date. In DICOM, it's 8 bytes fixed ... period.
			DateTime date;
			bool returnValue = DateParser.Parse("20060607", out date);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(date, new DateTime(2006, 6, 7));
			
			// Valid date according to OLD Dicom Standard (pre 3.0).
			// Recommended by Dicom that we still support it, but we're choosing not to.
			returnValue = DateParser.Parse("2006.06.07", out date);
			Assert.IsFalse(returnValue);

			//Invalid dates
			returnValue = DateParser.Parse("a0060607", out date);
			Assert.IsFalse(returnValue);

			returnValue = DateParser.Parse("2006067", out date);
			Assert.IsFalse(returnValue);

            returnValue = DateParser.Parse("200606", out date);
            Assert.IsFalse(returnValue);

            returnValue = DateParser.Parse("2006", out date);
            Assert.IsFalse(returnValue);

			returnValue = DateParser.Parse(String.Empty, out date);
			Assert.IsFalse(returnValue);

			returnValue = DateParser.Parse(null, out date);
			Assert.IsFalse(returnValue);
		}

		[Test]
		public void TestTimeParser()
		{
			//According to Dicom, any part of the time that is omitted is zero.
			//These are all valid time formats.
			DateTime time;
			DateTime testTime;
			bool returnValue = TimeParser.Parse("000000", out time);
			Assert.IsTrue(returnValue);
			testTime = new DateTime(1, 1, 1, 0, 0, 0);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("22", out time);
			Assert.IsTrue(returnValue);
			testTime = new DateTime(1, 1, 1, 22, 0, 0);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("2217", out time);
			Assert.IsTrue(returnValue);
			testTime = new DateTime(1, 1, 1, 22, 17, 0);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721", out time);
			Assert.IsTrue(returnValue);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			//zero fraction
			returnValue = TimeParser.Parse("221721.000000", out time);
			Assert.IsTrue(returnValue);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			//maximum fraction defined by Dicom
			returnValue = TimeParser.Parse("221721.999999", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(9999990); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			//arbitrary length fractions
			returnValue = TimeParser.Parse("221721.1", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1000000); 
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721.12", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1200000);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721.123", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1230000);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721.1234", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1234000);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721.12345", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1234500);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("221721.123456", out time);
			testTime = new DateTime(1, 1, 1, 22, 17, 21);
			testTime = testTime.AddTicks(1234560);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(time.TimeOfDay, testTime.TimeOfDay);

			returnValue = TimeParser.Parse("240000", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("236000", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("230060", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("a21721.12345", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("22a721.12345", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("2217a1.12345", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse("221721.a2345", out time);
			Assert.IsFalse(returnValue);

			returnValue = TimeParser.Parse(String.Empty, out time);
			Assert.IsFalse(returnValue);
		
			returnValue = TimeParser.Parse(null, out time);
			Assert.IsFalse(returnValue);
		}

		[Test]
		public void TestDateTimeParser_Parse()
		{
			DateTime dateTime;
			DateTime testDateTime;
		    bool returnValue;
            returnValue = DateTimeParser.Parse("2006", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 01, 01, 0, 0, 0);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.Parse("200606", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 01, 0, 0, 0);
            Assert.AreEqual(dateTime, testDateTime);
            
            returnValue = DateTimeParser.Parse("20060607", out dateTime);
			Assert.IsTrue(returnValue);
			testDateTime = new DateTime(2006, 06, 07, 0, 0, 0);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("2006060722", out dateTime);
			Assert.IsTrue(returnValue);
			testDateTime = new DateTime(2006, 06, 07, 22, 0, 0);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("200606072217", out dateTime);
			Assert.IsTrue(returnValue);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 0);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721", out dateTime);
			Assert.IsTrue(returnValue);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			Assert.AreEqual(dateTime, testDateTime);

			//zero fraction
			returnValue = DateTimeParser.Parse("20060607221721.000000", out dateTime);
			Assert.IsTrue(returnValue);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			Assert.AreEqual(dateTime, testDateTime);

			//maximum fraction defined by Dicom
			returnValue = DateTimeParser.Parse("20060607221721.999999", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(9999990); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue); 
			Assert.AreEqual(dateTime, testDateTime);

			//arbitrary length fractions
			returnValue = DateTimeParser.Parse("20060607221721.1", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1000000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.12", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1200000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1230000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.1234", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1234000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.12345", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1234500); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123456", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
			testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123456+05", out dateTime);
			testDateTime = new DateTime(2006, 06, 08, 3, 17, 21);
			testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123456-05", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 17, 17, 21);
			testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123456+0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 08, 3, 31, 21);
			testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123456-0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 17, 3, 21);
			testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721.123-05", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 17, 17, 21);
			testDateTime = testDateTime.AddTicks(1230000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607221721+0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 08, 3, 31, 21);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("200606072217+0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 08, 3, 31, 0);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("2006060722+0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 08, 3, 14, 0);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607+0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 07, 5, 14, 0);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("20060607-0514", out dateTime);
			testDateTime = new DateTime(2006, 06, 06, 18, 46, 0);
			Assert.IsTrue(returnValue);
			Assert.AreEqual(dateTime, testDateTime);

			returnValue = DateTimeParser.Parse("2006-0514", out dateTime);
			Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.Parse("200606-0514", out dateTime);
            Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("a0060607221721.12345", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("20060607a21721.12345", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("20060607221721.a2345", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("20060607221721.12345r0517", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("20060607221721.12345-a517", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse("20060607221721.12345-05a7", out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse(String.Empty, out dateTime);
			Assert.IsFalse(returnValue);

			returnValue = DateTimeParser.Parse(null, out dateTime);
			Assert.IsFalse(returnValue);
		}

        [Test]
        public void TestDateTimeParser_ParseDateAndTime()
		{
            DateTime dateTime;
            DateTime testDateTime;
            bool returnValue;
            returnValue = DateTimeParser.ParseDateAndTime(null, "2006", null, out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "200606", null, out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", null, out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 07, 0, 0, 0);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "22", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 07, 22, 0, 0);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "2217", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 0);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            Assert.AreEqual(dateTime, testDateTime);

            //zero fraction
            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.000000", out dateTime);
            Assert.IsTrue(returnValue);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            Assert.AreEqual(dateTime, testDateTime);

            //maximum fraction defined by Dicom
            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.999999", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(9999990); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            //arbitrary length fractions
            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.1", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1000000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.12", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1200000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.123", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1230000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.1234", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1234000); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.12345", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1234500); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.123456", out dateTime);
            testDateTime = new DateTime(2006, 06, 07, 22, 17, 21);
            testDateTime = testDateTime.AddTicks(1234560); //ticks are in units of 100 nano-seconds (1e9 * 100 = 1e7)
            Assert.IsTrue(returnValue);
            Assert.AreEqual(dateTime, testDateTime);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.123456+05", out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "a0060607", "221721.12345", out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "a21721.12345", out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.a2345", out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.ParseDateAndTime(null, "20060607", "221721.12345r0517", out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.Parse(String.Empty, out dateTime);
            Assert.IsFalse(returnValue);

            returnValue = DateTimeParser.Parse(null, out dateTime);
            Assert.IsFalse(returnValue);

		}

	    [Test]
		public void TestDateRangeParser()
		{
			string[,] tests = 
			{
				{ "20070101","20070101", "", "False" },
				{ "20070101-", "20070101", "", "True" },
				{ "-20070101", "", "20070101", "True" },
				{ "20070101-20070101", "20070101", "20070101", "True" },
				{ "20070101-20070202", "20070101", "20070202", "True" },

				//from date is after to
				{ "20070102-20070101", "", "", "exception" },

				//bad format, extra '-'
				{ "20070101-20070101-", "", "", "exception" },
				{ "-20070101-20070101", "", "", "exception" },

				//bad format, missing '-'
				{ "2007010120070101", "", "", "exception" },
				
				//bad format, missing character
				{ "20070101-2007010", "", "", "exception" },

				//bad format, bad characters
				{ "2007010a-20070101", "", "", "exception" }
			};

			for (int i = 0; i < tests.Length / 4; ++i)
			{
				string dateRange = tests[i, 0];
				string expectedFromDate = tests[i, 1];
				string expectedToDate = tests[i, 2];
				string expectedResult = tests[i, 3];

				bool isRange;

				try
				{
					DateTime? from, to;

					DateRangeHelper.Parse(dateRange, out from, out to, out isRange);
					if (expectedResult == "exception")
						Assert.Fail("expected an exception");

					if (expectedFromDate == "")
						Assert.IsNull(from);
					else
						Assert.AreEqual(((DateTime)from).ToString(DateParser.DicomDateFormat), expectedFromDate);
					
					if (expectedToDate == "")
						Assert.IsNull(to);
					else
						Assert.AreEqual(((DateTime)to).ToString(DateParser.DicomDateFormat), expectedToDate);

					Assert.AreEqual(isRange.ToString(), expectedResult);

				}
				catch (Exception e)
				{
					if (expectedResult != "exception")
						Assert.Fail(e.Message);
				}

				try
				{
					string fromString, toString;

					DateRangeHelper.Parse(dateRange, out fromString, out toString, out isRange);
					if (expectedResult == "exception")
						Assert.Fail("expected an exception");

					Assert.AreEqual(fromString, expectedFromDate);
					Assert.AreEqual(toString, expectedToDate);
					Assert.AreEqual(isRange.ToString(), expectedResult);

				}
				catch (Exception e)
				{
					if (expectedResult != "exception")
						Assert.Fail(e.Message);
				}

				try
				{
					int fromInt, toInt;

					DateRangeHelper.Parse(dateRange, out fromInt, out toInt, out isRange);
					if (expectedResult == "exception")
						Assert.Fail("expected an exception");

					if (expectedFromDate == "")
						Assert.AreEqual(fromInt, 0);
					else
						Assert.AreEqual(fromInt, Convert.ToInt32(expectedFromDate, CultureInfo.InvariantCulture));

					if (expectedToDate == "")
						Assert.AreEqual(toInt, 0);
					else
						Assert.AreEqual(toInt, Convert.ToInt32(expectedToDate, CultureInfo.InvariantCulture));

				}
				catch (Exception e)
				{
					if (expectedResult != "exception")
						Assert.Fail(e.Message);
				}

			}
		}
	}
}

#endif