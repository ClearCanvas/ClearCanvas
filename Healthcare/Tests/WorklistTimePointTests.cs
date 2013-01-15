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
using System.Collections.Generic;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class WorklistTimePointTests
    {

        [Test]
        public void CreateFixed()
        {
            WorklistTimePoint t1 = new WorklistTimePoint(DateTime.Now, WorklistTimePoint.Resolutions.Day);

            Assert.IsTrue(t1.IsFixed);

            // even though we initialized it with DateTime.Now, it should be rounded to the nearest day
            Assert.AreEqual(DateTime.Today, t1.FixedValue);
        }

        [Test]
        public void CreateRelative()
        {
            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromHours(0), WorklistTimePoint.Resolutions.RealTime);

            Assert.IsTrue(t1.IsRelative);
            Assert.AreEqual(TimeSpan.FromHours(0), t1.RelativeValue);
        }

        [Test]
        public void ResolveFixedWithDayResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(now.AddDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(now.AddDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(0), t2.ResolveUp(now));
        }

        [Test]
        public void ResolveRelativeWithDayResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(0), t2.ResolveUp(now));

            // what if "now" happens to be right on the boundary??
            DateTime midnight = DateTime.Now.Date;

            WorklistTimePoint t3 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(midnight.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(midnight.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t4 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(midnight.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(midnight.AddDays(0), t2.ResolveUp(now));
        }

        [Test]
        public void ResolveRelativeWithRealTimeResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.RealTime);
            Assert.AreEqual(now.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.AddDays(1), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.RealTime);
            Assert.AreEqual(now.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.AddDays(-1), t2.ResolveUp(now));
        }
    }
}

#endif

