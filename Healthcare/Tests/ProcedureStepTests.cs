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
using ClearCanvas.Workflow;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProcedureStepTests
    {
        #region ConcreteProcedureStep

		/// <summary>
		/// Concrete derivation of the abstract ProcedureStep, for use in tests.
		/// </summary>
        class ConcreteProcedureStep : ProcedureStep
        {
        	private readonly bool _supportsLinking;
			private readonly List<Procedure> _linkedProcedures = new List<Procedure>();
			private List<ProcedureStep> _relatedSteps = new List<ProcedureStep>();

			public ConcreteProcedureStep(Procedure procedure)
				:this(procedure, false)
			{
			}

            public ConcreteProcedureStep(Procedure procedure, bool supportsLinking) 
                : base(procedure)
            {
            	_supportsLinking = supportsLinking;
            }

			/// <summary>
			/// Method used to set the "related" steps, for testing
			/// </summary>
			/// <param name="relatedSteps"></param>
			public void SetRelatedSteps(ConcreteProcedureStep[] relatedSteps)
			{
				_relatedSteps = new List<ProcedureStep>(relatedSteps);
			}

            public override string Name
            {
				get { return "Concrete"; }
            }

			public override bool CreateInDowntimeMode
			{
				get { return true; }
			}

            public override bool IsPreStep
            {
                get { return false; }
            }

			public override TimeSpan SchedulingOffset
			{
				get { return TimeSpan.MaxValue; }
			}

            protected override void LinkProcedure(Procedure procedure)
            {
				if(_supportsLinking)
				{
					_linkedProcedures.Add(procedure);
				}
				else
				{
					base.LinkProcedure(procedure);
				}
            }

            public override List<Procedure> GetLinkedProcedures()
            {
				// note that even if _supporstLinking == false, we
				// still return an empty list here - we don't throw
				return _linkedProcedures;
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new ConcreteProcedureStep(Procedure);
            }

            protected override bool IsRelatedStep(ProcedureStep step)
            {
				// this is by definition related to this!
                return Equals(this, step) || _relatedSteps.Contains(step);
            }
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            ProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Assert.AreEqual(procedure, procedureStep.Procedure);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(procedureStep));
        }
        
        #endregion

        #region Assign/Reassign functionality

        [Test]
        public void Test_Assign()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
            
            procedureStep.Assign(performer);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNotNull(procedureStep.Scheduling.Performer);
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), procedureStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Scheduling.Performer).Staff);
        }

        [Test]
        public void Test_Assign_NullStaff()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            // checking that assigning null has no effect if already null
            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
            procedureStep.Assign((Staff)null);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);

            // check that assigning null has the effect of un-assiging if already assigned
            Staff performer = new Staff();
            procedureStep.Assign(performer);

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNotNull(procedureStep.Scheduling.Performer);

            procedureStep.Assign((Staff)null);
            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);
        }

        [Test]
        public void Test_Reassign_Scheduled()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();

            Assert.IsNotNull(procedureStep.Scheduling);
            Assert.IsNull(procedureStep.Scheduling.Performer);

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer); // Perform event

            // Make assertions
            Assert.IsNotNull(newStep.Scheduling);
            Assert.IsNotNull(newStep.Scheduling.Performer);
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), newStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)newStep.Scheduling.Performer).Staff);
        }

        [Test]
        public void Test_Reassign_InProgressOrSuspended()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            procedureStep.Schedule(now, end);
            procedureStep.Suspend();
            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer); // Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
            Assert.AreEqual(ActivityStatus.SC, newStep.State);
            Assert.AreEqual(now, newStep.Scheduling.StartTime);
            Assert.AreEqual(end, newStep.Scheduling.EndTime);
            Assert.IsNotNull(newStep.Scheduling);
            Assert.IsNotNull(newStep.Scheduling.Performer);
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), newStep.Scheduling.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)newStep.Scheduling.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Reassign_Terminated()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            procedureStep.Discontinue();

            ConcreteProcedureStep newStep = (ConcreteProcedureStep)procedureStep.Reassign(performer);
		}

		[Test]
		public void Test_AssignedStaff()
		{
			Procedure procedure = new Procedure();
			ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
			Staff performer = new Staff();

			procedureStep.Assign(performer);

			Assert.IsNotNull(procedureStep.Scheduling);
			Assert.IsNotNull(procedureStep.Scheduling.Performer);
			Assert.AreEqual(performer, procedureStep.AssignedStaff);
		}

		[Test]
		public void Test_AssignedStaff_SchedulingNull()
		{
			Procedure procedure = new Procedure();
			ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

			Assert.IsFalse(procedureStep.Scheduling != null && procedureStep.Scheduling.Performer != null);
			Assert.IsNull(procedureStep.AssignedStaff);
		}

		#endregion

		#region Starting and Completing functionality

		[Test]
        public void Test_Start()
        {
            DateTime? later = DateTime.Now.AddHours(2);
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer, later);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(later, procedureStep.StartTime);
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        public void Test_Start_NullStartTime()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer, null);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.StartTime));
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        public void Test_Start_NoStartTime()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(performer);// Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.StartTime));
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Start_NullPerformer()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            procedureStep.Start((Staff)null); // Perform event
        }

        [Test]
        public void Test_Complete()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
            Staff performer = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Complete(performer); // Perform event

            // Make assertions
            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.IsTrue(RoughlyEqual(Platform.Time, procedureStep.EndTime));
            Assert.IsInstanceOf(typeof(ProcedureStepPerformer), procedureStep.Performer);
            Assert.AreEqual(performer, ((ProcedureStepPerformer)procedureStep.Performer).Staff);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Complete_NullPerformer()
        {
            Procedure procedure = new Procedure();
            ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

            procedureStep.Complete((Staff)null); // Perform event
		}
		[Test]
		public void Test_PerformingStaff()
		{
			Procedure procedure = new Procedure();
			ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);
			Staff performer = new Staff();

			procedureStep.Start(performer);

			Assert.IsNotNull(procedureStep.Performer);
			Assert.AreEqual(performer, procedureStep.PerformingStaff);
		}

		[Test]
		public void Test_PerformingStaff_PerformerNull()
		{
			Procedure procedure = new Procedure();
			ConcreteProcedureStep procedureStep = new ConcreteProcedureStep(procedure);

			Assert.IsNull(procedureStep.Performer);
			Assert.IsNull(procedureStep.PerformingStaff);
		}

		#endregion

		#region Procedure Linking Functionality

		[Test]
        public void Test_LinkTo()
        {
            Procedure p1 = new Procedure();
            Procedure p2 = new Procedure();

			// create proc steps that support linking
            ConcreteProcedureStep ps1 = new ConcreteProcedureStep(p1, true);
            ConcreteProcedureStep ps2 = new ConcreteProcedureStep(p2, true);
            Assert.AreEqual(ActivityStatus.SC, ps1.State);

			// pre-conditions:
			// both link step properties are null (not set), and 
			// linked procedure lists are emtpy
			Assert.IsNull(ps1.LinkStep);
        	Assert.IsFalse(ps1.IsLinked);
			Assert.IsEmpty(ps1.GetLinkedProcedures());
			Assert.IsNull(ps2.LinkStep);
			Assert.IsFalse(ps2.IsLinked);
			Assert.IsEmpty(ps2.GetLinkedProcedures());
           
			// link step 1 to step 2
            ps1.LinkTo(ps2);

			// which should discontinue step 1
			Assert.AreEqual(ActivityStatus.DC, ps1.State);

			// expect the ps1.LinkStep is set to point to ps2
            Assert.AreEqual(ps2, ps1.LinkStep);
			Assert.IsTrue(ps1.IsLinked);	// ps1 is linked

			// expect ps1 does not have any linked procedures,
			// and AllProcedures returns only ps1
			Assert.IsEmpty(ps1.GetLinkedProcedures());
			Assert.AreEqual(1, ps1.AllProcedures.Count);
			Assert.Contains(p1, ps1.AllProcedures);

			// expect ps2 is still not linked
			Assert.IsNull(ps2.LinkStep);
			Assert.IsFalse(ps2.IsLinked);	// ps2 is not linked
			// expect ps2 has exactly 1 linked procedure, which is p1
			Assert.AreEqual(1, ps2.GetLinkedProcedures().Count);
			Assert.Contains(p1, ps2.GetLinkedProcedures());

			// expect AllProcedures contains both p1 and p2
			Assert.AreEqual(2, ps2.AllProcedures.Count);
			Assert.Contains(p1, ps2.AllProcedures);
			Assert.Contains(p2, ps2.AllProcedures);
		}

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_LinkTo_StateNotScheduled()
        {
            Procedure p1 = new Procedure();
            Procedure p2 = new Procedure();

			// create proc steps that support linking
			ConcreteProcedureStep ps1 = new ConcreteProcedureStep(p1, true);
			ConcreteProcedureStep ps2 = new ConcreteProcedureStep(p2, true);

			ps1.Discontinue();
			Assert.AreEqual(ActivityStatus.DC, ps1.State);

			// should throw, because only a scheduled step can be linked to another
            ps1.LinkTo(ps2);
        }

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_LinkTo_LinkingNotSupported()
		{
			Procedure p1 = new Procedure();
			Procedure p2 = new Procedure();

			// create proc steps that don't support linking
			ConcreteProcedureStep ps1 = new ConcreteProcedureStep(p1, false);
			ConcreteProcedureStep ps2 = new ConcreteProcedureStep(p2, false);

			// should throw, because linking is not supported
			ps1.LinkTo(ps2);
		}

		#endregion

		#region Related-Steps functionality

		[Test]
		public void Test_GetRelatedProcedureSteps()
		{
			Procedure p1 = new Procedure();

			// attach 2 procedure steps to p1
			ConcreteProcedureStep ps11 = new ConcreteProcedureStep(p1, false);
			ConcreteProcedureStep ps12 = new ConcreteProcedureStep(p1, false);

			// expect that each ps is only related to itself
			Assert.AreEqual(1, ps11.GetRelatedProcedureSteps().Count);
			Assert.Contains(ps11, ps11.GetRelatedProcedureSteps());
			Assert.AreEqual(1, ps12.GetRelatedProcedureSteps().Count);
			Assert.Contains(ps12, ps12.GetRelatedProcedureSteps());

			// now set ps12 related to ps11
			ps11.SetRelatedSteps(
				new ConcreteProcedureStep[]{ps12});

			// expect ps11 now has 2 related steps, itself and ps12
			Assert.AreEqual(2, ps11.GetRelatedProcedureSteps().Count);
			Assert.Contains(ps11, ps11.GetRelatedProcedureSteps());
			Assert.Contains(ps12, ps11.GetRelatedProcedureSteps());

			// create another procedure and proc step
			Procedure p2 = new Procedure();
			ConcreteProcedureStep ps21 = new ConcreteProcedureStep(p2, false);

			// now set ps21 related to ps11
			ps11.SetRelatedSteps(
				new ConcreteProcedureStep[] { ps21 });

			// expect that ps11 is only related to itself,
			// (eg ps21 has no effect, because it is associated with a different procedure,
			// and only steps associated to the same procedure can be related.
			Assert.AreEqual(1, ps11.GetRelatedProcedureSteps().Count);
			Assert.Contains(ps11, ps11.GetRelatedProcedureSteps());
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