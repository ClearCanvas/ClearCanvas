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
using ClearCanvas.Common;
using NUnit.Framework;

namespace ClearCanvas.Workflow.Tests
{
    [TestFixture]
    public class PerformedStepTests
    {
        class ConcretePerformedStep : PerformedStep
        {
            public ConcretePerformedStep(ConcreteActivityPerformer performer, DateTime? startTime)
                : base(performer, startTime, new PerformedStepStatusTransitionLogic())
            {
                
            }
        }
        class ConcreteActivityPerformer : ActivityPerformer
        {
            
        }

        #region Method Tests

        [Test]
        public void Test_Ctor()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.IsNotNull(performedStep.Activities);
            Assert.AreEqual(now, performedStep.StartTime);
            Assert.AreEqual(performedStep.Performer, activityPerformer);
        }

        [Test]
        public void Test_Discontinue()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Discontinue(end); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.DC, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.AreEqual(end, performedStep.EndTime);
        }

        [Test]
        public void Test_Discontinue_NullEndTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Discontinue(); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.DC, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.IsTrue(RoughlyEqual(performedStep.EndTime, Platform.Time));
        }

        [Test]
        public void Test_Complete()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Complete(end); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.CM, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.AreEqual(end, performedStep.EndTime);
        }

        [Test]
        public void Test_Complete_NullEndTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Complete(); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.CM, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.IsTrue(RoughlyEqual(performedStep.EndTime, Platform.Time));
        }

        #endregion

        private static bool RoughlyEqual(DateTime? x, DateTime? y)
        {
            if (!x.HasValue && !y.HasValue)
                return true;

            if (!x.HasValue || !y.HasValue)
                return false;

            DateTime xx = x.Value;
            DateTime yy = y.Value;

            // for these purposes, if the times are within 1 second, that is good enough
            return Math.Abs((xx - yy).TotalSeconds) < 1;
        }
    }
}

#endif