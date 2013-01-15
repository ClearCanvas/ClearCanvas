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
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
#if BUG_5725_FIXED	//FIXME: Fix and rerun this unit test when bug #5725 is addressed (i.e. delete this line!!)
    [TestFixture]
    public class FormatTests
    {
        /// <summary>
        /// We use the boundary or "threshold" as the test here to determine
        /// assume that if case is that we have friendly terms just inside the boundary
        /// and regular dates just outside that the threshold does indeed hold.
        /// 
        /// We override the default setting here by manually setting it to something known
        /// that can be tested against.
        /// </summary>
        [Test]
        public void Test_Date_Descriptive()
        {
            int threshold = 5;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;
            DateTime? now = DateTime.Now;
            DateTime? today = DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);
            
            // if input was before yesterday
            // we test for a day before the threshold
            Assert.AreEqual(threshold + " days ago", Format.Date(now.Value.AddDays(-threshold), true));           // within threshold
            Assert.AreEqual(Format.DateTime(now.Value.AddDays(-(threshold + 1))), Format.Date(now.Value.AddDays(-(threshold + 1)), true));  // outside threshold
            
            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));
            
            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));
            
            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));
            
            // if input is beyond tomorrow
            // we test for a day before the threshold
            Assert.AreEqual(threshold + " days from now", Format.Date(now.Value.AddDays(threshold), true));       // within threshold
            Assert.AreEqual(Format.DateTime(now.Value.AddDays(threshold + 2)), Format.Date(now.Value.AddDays(threshold + 2), true));    // outside threshold
        }

        /// <summary>
        /// premise of this test is that if the following assertions succeed, 
        /// then that means that the execution path never directs to the "friendly terms"
        /// 
        /// We override the default setting here by manually setting it to something known
        /// that can be tested against.
        /// </summary>
        [Test]
        public void Test_Date_Descriptive_MininumThresholds()
        {
            int threshold = 0;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;
            DateTime? now = DateTime.Now;
            DateTime? today = DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);

            // if input was yesterday
            Assert.AreEqual(yesterday.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(yesterday, true));

            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));

            // if input is tomorrow
            Assert.AreEqual(tomorrow.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(tomorrow, true));

            threshold = 1;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;

            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));

            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));

            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));
        }

        [Test]
        public void Test_Date_NonDescriptive_API()
        {
            DateTime? today = System.DateTime.Today;

            Assert.AreEqual(today.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(today, false));
        }

        [Test]
        public void Test_Date_NonDescriptive_Admin()
        {
            DateTime? today = System.DateTime.Today;

            FormatSettings.Default.DescriptiveFormattingEnabled = false;

            Assert.AreEqual(today.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(today, true));
        }
    }
#endif
}

#endif