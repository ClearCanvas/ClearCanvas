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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [TestFixture]
    public class AutoRouteScheduledTimeTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
        
        }

        [Test]
        public void TestStandardWindow()
        {
            var request = new DicomAutoRouteRequest
                              {
                                  TimeWindowStart = 6, 
                                  TimeWindowEnd = 7
                              };

            var time = new DateTime(2012, 5, 3, 5, 0, 0);

            // Before
            var scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 6, 0, 0));

            scheduledTime = request.GetScheduledTime(time, 5);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 6, 0, 0));

            //During
            time = new DateTime(2012, 5, 3, 6, 30, 0);
            scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, time);
            scheduledTime = request.GetScheduledTime(time, 5);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 6, 30, 5));

            //After Window
            time = new DateTime(2012, 5, 3, 8, 0, 0);
            scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 4, 6, 0, 0));

            scheduledTime = request.GetScheduledTime(time, 5);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 4, 6, 0, 0));

        }

        [Test]
        public void TestStartAfterEnd()
        {
            var request = new DicomAutoRouteRequest
                              {
                                  TimeWindowStart = 23, 
                                  TimeWindowEnd = 1
                              };

            var time = new DateTime(2012, 5, 3, 22, 0, 0);

            // Before
            var scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 23, 0, 0));

            scheduledTime = request.GetScheduledTime(time, 10);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 23, 0, 0));

            //During
            time = new DateTime(2012, 5, 3, 23, 30, 0);
            scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, time);

            scheduledTime = request.GetScheduledTime(time, 10);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 3, 23, 30, 10));

            time = new DateTime(2012, 5, 4, 0, 30, 0);
            scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, time);

            scheduledTime = request.GetScheduledTime(time, 10);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 4, 0, 30, 10));

            //After Window
            time = new DateTime(2012, 5, 4, 2, 0, 0);
            scheduledTime = request.GetScheduledTime(time, 0);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 4, 23, 0, 0));

            scheduledTime = request.GetScheduledTime(time, 10);
            Assert.AreEqual(scheduledTime, new DateTime(2012, 5, 4, 23, 0, 0));

        } 
    }
}
#endif