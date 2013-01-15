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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Workflow.Tests
{
    [TestFixture]
    public class ActivityTests
    {
        class ConcreteActivity : Activity
        {
        }

        class ConcreteActivityPerformer : ActivityPerformer
        {
        }

        class ConcretePerformedStep : PerformedStep
        {
        }

        #region Method Tests

        public ActivityTests()
		{
			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test_ctor()
		{
            ConcreteActivity activity = new ConcreteActivity();
            Assert.AreEqual(ActivityStatus.SC, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.CreationTime));
            Assert.IsNotNull(activity.Scheduling);
            Assert.IsNotNull(activity.PerformedSteps);
            Assert.IsTrue(activity.PerformedSteps.IsEmpty);
        }

        [Test]
        public void Test_Schedule()
        {
            DateTime now = DateTime.Now;

            // Properly scheduled activity
            ConcreteActivity activity = new ConcreteActivity();
            activity.Schedule(now, now + TimeSpan.FromDays(3));
            // Assertion: scheduling proprety is set properly
            Assert.IsNotNull(activity.Scheduling);
            Assert.AreEqual(now, activity.Scheduling.StartTime);
            Assert.AreEqual(now + TimeSpan.FromDays(3), activity.Scheduling.EndTime);
        }

        [Test]
        public void Test_Schedule_WithNulls()
        {
            DateTime now = DateTime.Now;
            DateTime later = now + TimeSpan.FromHours(3);

            // Null scheduled activity
            ConcreteActivity activity2 = new ConcreteActivity();
            activity2.Schedule(null, null);
            Assert.IsNull(activity2.Scheduling.StartTime);
            Assert.IsNull(activity2.Scheduling.EndTime);
            Assert.AreEqual(ActivityStatus.SC, activity2.State);

            // Null scheduled activity
            ConcreteActivity activity3 = new ConcreteActivity();
            activity3.Schedule(null);
            Assert.IsNull(activity3.Scheduling.StartTime);
            Assert.IsNull(activity3.Scheduling.EndTime);
            Assert.AreEqual(ActivityStatus.SC, activity3.State);

            ConcreteActivity activity4 = new ConcreteActivity();
            activity4.Schedule(later);
            Assert.AreEqual(later, activity4.Scheduling.StartTime);
            Assert.IsNull(activity4.Scheduling.EndTime);
            Assert.AreEqual(ActivityStatus.SC, activity4.State);

            ConcreteActivity activity5 = new ConcreteActivity();
            activity5.Schedule(null, later);
            Assert.IsNull(activity5.Scheduling.StartTime);
            Assert.AreEqual(later, activity5.Scheduling.EndTime);
            Assert.AreEqual(ActivityStatus.SC, activity5.State);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Schedule_FromNonScheduledState()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Schedule(DateTime.Now, DateTime.Now + TimeSpan.FromDays(3));
        }

        [Test]
        public void Test_Assign()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();

            activity.Assign(activityPerformer);

            Assert.IsNotNull(activity.Scheduling);
            Assert.AreEqual(activityPerformer, activity.Scheduling.Performer);
            Assert.AreEqual(ActivityStatus.SC, activity.State);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Assign_FromNonScheduledState()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Assign(new ConcreteActivityPerformer());
        }

        [Test]
        public void Test_Start()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();

            activity.Start(activityPerformer, now); // Perform event

            // Make assertions
            Assert.AreEqual(activityPerformer, activity.Performer);
            Assert.AreEqual(now, activity.StartTime);
            Assert.AreEqual(ActivityStatus.IP, activity.State);
            Assert.AreEqual(now, activity.LastStateChangeTime);
        }

        [Test]
        public void Test_Start_NullTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            
            activity.Start(activityPerformer); // Perform event
            
            // Make assertions
            Assert.AreEqual(activityPerformer, activity.Performer);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.StartTime));
            Assert.AreEqual(ActivityStatus.IP, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Start_NullPerformer()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(null); // Perform event
        }

        [Test]
        public void Test_Suspend()
        {
            ConcreteActivity activity = new ConcreteActivity();

            activity.Suspend(); // Perform event
            
            Assert.AreEqual(ActivityStatus.SU, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Resume()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Suspend();
            
            activity.Resume(); // Perform event
            
            Assert.AreEqual(ActivityStatus.IP, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Resume_NonSuspendedState()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Resume(); // Perform event
        }

        [Test]
        public void Test_Discontinue()
        {
            DateTime? now = DateTime.Now; 
            ConcreteActivity activity = new ConcreteActivity();
            activity.PerformedSteps.Add(new ConcretePerformedStep());
            activity.PerformedSteps.Add(new ConcretePerformedStep());
            activity.PerformedSteps.Add(new ConcretePerformedStep());

            activity.Discontinue(now); // Perform event
            
            // Make assertions
            Assert.AreEqual(now, activity.EndTime);
            foreach (PerformedStep step in activity.PerformedSteps)
            {
                Assert.IsTrue(step.IsTerminated);
            }
            Assert.AreEqual(ActivityStatus.DC, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Discontinue_NullTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivity activity = new ConcreteActivity();
            activity.PerformedSteps.Add(new ConcretePerformedStep());
            activity.PerformedSteps.Add(new ConcretePerformedStep());
            activity.PerformedSteps.Add(new ConcretePerformedStep());

            activity.Discontinue(); // Perform event

            // Make assertions
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.EndTime));
            foreach (PerformedStep step in activity.PerformedSteps)
            {
                Assert.IsTrue(step.IsTerminated);
            }
            Assert.AreEqual(ActivityStatus.DC, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Complete()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcreteActivity activity = new ConcreteActivity();
            
            activity.Complete(activityPerformer, end);// Perform event

            // Make assertions
            Assert.AreEqual(activityPerformer, activity.Performer);
            Assert.AreEqual(activity.StartTime, activity.EndTime);
            Assert.AreEqual(ActivityStatus.CM, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Complete_NullArgs()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            activity.Start(activityPerformer);

            activity.Complete(); // Perform event

            // Make assertions
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.EndTime));
            Assert.AreEqual(ActivityStatus.CM, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Complete_NullPerformer()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            activity.Start(activityPerformer);

            activity.Complete(end); // Perform event

            // Make assertions
            Assert.AreEqual(activity.EndTime, end);
            Assert.AreEqual(ActivityStatus.CM, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        public void Test_Complete_NullEndTime()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcreteActivity activity = new ConcreteActivity();

            activity.Complete(activityPerformer); // Perform event

            // Make assertions
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.EndTime));
            Assert.AreEqual(activityPerformer, activity.Performer);
            Assert.AreEqual(activity.StartTime, activity.EndTime);
            Assert.AreEqual(ActivityStatus.CM, activity.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, activity.LastStateChangeTime));
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_NoPerformer()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Complete(); // Perform event
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_PerformerExistsAlready()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            activity.Start(activityPerformer);

            activity.Complete(new ConcreteActivityPerformer(), DateTime.Now); // Perform event
        }

        [Test]
        public void Test_AddPerformedStep()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcretePerformedStep performedStep = new ConcretePerformedStep();

            Assert.IsTrue(activity.PerformedSteps.IsEmpty);
            Assert.IsTrue(performedStep.Activities.IsEmpty);

            activity.AddPerformedStep(performedStep);// Perform event

            // Make assertions
            Assert.IsTrue(activity.PerformedSteps.Contains(performedStep));
            Assert.AreEqual(1, activity.PerformedSteps.Count);
            Assert.IsTrue(performedStep.Activities.Contains(activity));
            Assert.AreEqual(1, performedStep.Activities.Count);
        }

        [Test]
        public void Test_RemovePerformedStep()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcretePerformedStep performedStep = new ConcretePerformedStep();
            activity.AddPerformedStep(performedStep);

            Assert.IsTrue(activity.PerformedSteps.Contains(performedStep));
            Assert.AreEqual(1, activity.PerformedSteps.Count);
            Assert.IsTrue(performedStep.Activities.Contains(activity));
            Assert.AreEqual(1, performedStep.Activities.Count);

            activity.RemovePerformedStep(performedStep);// Perform event

            // Make assertions
            Assert.IsTrue(activity.PerformedSteps.IsEmpty);
            Assert.IsTrue(performedStep.Activities.IsEmpty);
        }

        [Test]
        public void Test_RemovePerformedStep_WithoutAdding()
        {
            ConcreteActivity activity = new ConcreteActivity();
            ConcretePerformedStep performedStep = new ConcretePerformedStep();
            ConcretePerformedStep performedStep2 = new ConcretePerformedStep();

            activity.AddPerformedStep(performedStep2);

            Assert.AreEqual(1, activity.PerformedSteps.Count);
            Assert.IsTrue(activity.PerformedSteps.Contains(performedStep2));

            // remove a step that does not exist in the set
            activity.RemovePerformedStep(performedStep);// Perform event

            Assert.AreEqual(1, activity.PerformedSteps.Count);
            Assert.IsTrue(activity.PerformedSteps.Contains(performedStep2));
        }

        [Test]
        public void Test_Timeshift()
        {
			DateTime? now = DateTime.Now;
			DateTime? end = DateTime.Now + TimeSpan.FromDays(30);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
			ConcreteActivity activity = new ConcreteActivity();
			activity.Schedule(now, end);
            activity.Start(activityPerformer, now);
            activity.Complete(end);
			
			activity.TimeShift(10); // Perform event
			
            // Make assertions
            Assert.AreEqual(activity.StartTime, now.Value.AddMinutes(10));
			Assert.AreEqual(activity.EndTime, end.Value.AddMinutes(10));
			Assert.AreEqual(activity.Scheduling.StartTime, now.Value.AddMinutes(10));
			Assert.AreEqual(activity.Scheduling.EndTime, end.Value.AddMinutes(10));
        }

        [Test]
        public void Test_TimeShift_TimeAndSchedulingNull()
        {
            ConcreteActivity activity = new ConcreteActivity();
            DateTime? start = activity.StartTime,
                      end = activity.EndTime,
                      schedStart = activity.Scheduling.StartTime,
                      schedEnd = activity.Scheduling.EndTime;

            activity.TimeShift(10);// Perform event

            // Make assertions
            Assert.AreEqual(activity.StartTime, start);
            Assert.AreEqual(activity.EndTime, end);
            Assert.AreEqual(activity.Scheduling.StartTime, schedStart);
            Assert.AreEqual(activity.Scheduling.EndTime, schedEnd);
        }

        #endregion

        #region Legal State Transition Logic Tests

        [Test]
        public void Test_SC_to_SU()
        {
            ConcreteActivity activity = new ConcreteActivity();
            Assert.AreEqual(ActivityStatus.SC, activity.State);
            activity.Suspend();
            Assert.AreEqual(ActivityStatus.SU, activity.State);
        }

        [Test]
        public void Test_SC_to_CM()
        {
            ConcreteActivity activity = new ConcreteActivity();
            Assert.AreEqual(ActivityStatus.SC, activity.State);
            activity.Complete(new ConcreteActivityPerformer());
            Assert.AreEqual(ActivityStatus.CM, activity.State);
        }

        [Test]
        public void Test_SC_to_DC()
        {
            ConcreteActivity activity = new ConcreteActivity();
            Assert.AreEqual(ActivityStatus.SC, activity.State);
            activity.Discontinue();
            Assert.AreEqual(ActivityStatus.DC, activity.State);
        }

        [Test]
        public void Test_SU_to_CM()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Suspend();
            Assert.AreEqual(ActivityStatus.SU, activity.State);
            activity.Complete(new ConcreteActivityPerformer());
            Assert.AreEqual(ActivityStatus.CM, activity.State);
        }

        [Test]
        public void Test_SU_to_DC()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Suspend();
            Assert.AreEqual(ActivityStatus.SU, activity.State);
            activity.Discontinue();
            Assert.AreEqual(ActivityStatus.DC, activity.State);
        }

        #endregion

        #region Illegal State Transition Logic Tests

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_IP_to_IP()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Start(new ConcreteActivityPerformer());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_SU_to_SU()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Suspend();
            activity.Suspend();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_CM_to_IP()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Complete();
            activity.Start(new ConcreteActivityPerformer());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_CM_to_SU()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Complete();
            activity.Suspend();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_CM_to_CM()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Complete();
            activity.Complete();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_CM_to_DC()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Complete();
            activity.Discontinue();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_DC_to_IP()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Discontinue();
            activity.Start(new ConcreteActivityPerformer());
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_DC_to_SU()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Discontinue();
            activity.Suspend();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_DC_to_CM()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Discontinue();
            activity.Complete();
        }

        [Test]
        [ExpectedException(typeof(IllegalStateTransitionException))]
        public void Test_DC_to_DC()
        {
            ConcreteActivity activity = new ConcreteActivity();
            activity.Start(new ConcreteActivityPerformer());
            activity.Discontinue();
            activity.Discontinue();
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