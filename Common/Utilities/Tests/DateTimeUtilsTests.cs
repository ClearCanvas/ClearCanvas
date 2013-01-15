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

namespace ClearCanvas.Common.Utilities.Tests
{
    [TestFixture]
    public class DateTimeUtilsTests
    {
        public DateTimeUtilsTests()
        {
        }

        [Test]
        public void TestFormatISO()
        {
            DateTime date1 = new DateTime(2008, 4, 10, 6, 30, 0);
            Assert.AreEqual("2008-04-10T06:30:00", DateTimeUtils.FormatISO(date1));
        }

        [Test]
        public void TestParseISO()
        {
            string s = "2008-04-10T06:30:00";
            DateTime date1 = new DateTime(2008, 4, 10, 6, 30, 0);
            Assert.AreEqual(date1, DateTimeUtils.ParseISO(s));
        }
    }
}

#endif