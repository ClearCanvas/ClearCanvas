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
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ProcedureTests
	{
		#region ConcreteProcedureStep

		class ConcreteProcedureStep : ProcedureStep
		{
			private readonly List<Procedure> _linkedProcedures = new List<Procedure>();
			private readonly List<ProcedureStep> _relatedSteps = new List<ProcedureStep>();

			public ConcreteProcedureStep(Procedure procedure)
				: base(procedure)
			{
				_relatedSteps.Add(this);
			}

			public void AddRelatedStep(ProcedureStep step)
			{
				if (!Equals(step.Procedure, this.Procedure))
					throw new Exception();

				_relatedSteps.Add(step);
			}

			protected override void LinkProcedure(Procedure procedure)
			{
				_linkedProcedures.Add(procedure);
			}

			public override string Name
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}

			public override bool CreateInDowntimeMode
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}

			public override bool IsPreStep
			{
				get { return false; }
			}

			public override TimeSpan SchedulingOffset
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}

			public override List<Procedure> GetLinkedProcedures()
			{
				return _linkedProcedures;
			}

			protected override ProcedureStep CreateScheduledCopy()
			{
				throw new Exception("The method or operation is not implemented.");
			}

			protected override bool IsRelatedStep(ProcedureStep step)
			{
				return _relatedSteps.Contains(step);
			}
		}

		#endregion

		#region ConcreteReportingProcedureStep
		class ConcreteReportingProcedureStep : ReportingProcedureStep
		{

			public ConcreteReportingProcedureStep(Procedure procedure)
				: base(procedure, new ReportPart())
			{

			}

			public override string Name
			{
				get { return "Concrete Reporting Procedure Step"; }
			}

			protected override ProcedureStep CreateScheduledCopy()
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}
		#endregion

		public ProcedureTests()
		{
			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new TestExtensionFactory());
		}

		[Test]
		public void Test_Constructor()
		{
			var procedureType = new ProcedureType();
			var procedure = new Procedure(procedureType, "111", "1.111");

			Assert.AreEqual(procedureType, procedure.Type);
			Assert.AreEqual(0, procedure.ProcedureSteps.Count);
			Assert.IsNotNull(procedure.ProcedureCheckIn);
			Assert.AreEqual(0, procedure.Reports.Count);
			Assert.AreEqual(0, procedure.Protocols.Count);
		}

		#region Property Tests

		[Test]
		public void Test_ModailtyProcedureSteps()
		{
			var procedure = new Procedure();
			var ps1 = new ModalityProcedureStep(procedure, "description", new Modality());
			var ps2 = new DocumentationProcedureStep(procedure);

			Assert.AreEqual(2, procedure.ProcedureSteps.Count);
			Assert.IsTrue(procedure.ModalityProcedureSteps.Contains(ps1));
			Assert.AreEqual(1, procedure.ModalityProcedureSteps.Count);
		}

		[Test]
		public void Test_ReportingProcedureSteps()
		{
			var procedure = new Procedure();
			var ps1 = new ConcreteReportingProcedureStep(procedure);
			var ps2 = new DocumentationProcedureStep(procedure);

			Assert.AreEqual(2, procedure.ProcedureSteps.Count);
			Assert.IsTrue(procedure.ReportingProcedureSteps.Contains(ps1));
			Assert.AreEqual(1, procedure.ReportingProcedureSteps.Count);
		}

		[Test]
		public void Test_IsTerminatedTrue()
		{
			var procedure = new Procedure();
			procedure.Cancel();

			Assert.AreEqual(ProcedureStatus.CA, procedure.Status);
			Assert.IsTrue(procedure.IsTerminated);

			procedure = new Procedure();
			var procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
			procedureStep.Start(new Staff());
			procedure.Complete(Platform.Time);

			Assert.AreEqual(ProcedureStatus.CM, procedure.Status);
			Assert.IsTrue(procedure.IsTerminated);

			procedure = new Procedure();
			procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
			procedureStep.Start(new Staff());
			procedure.Discontinue();

			Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
			Assert.IsTrue(procedure.IsTerminated);
		}

		[Test]
		public void Test_IsTerminatedFalse()
		{
			var procedure = new Procedure();

			Assert.IsFalse(procedure.IsTerminated);

			var procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
			procedureStep.Start(new Staff());

			Assert.IsFalse(procedure.IsTerminated);
		}

		[Test]
		public void Test_DocumentationProcedureStep()
		{
			var procedure = new Procedure();
			var ps1 = new DocumentationProcedureStep(procedure);
			var ps2 = new ConcreteReportingProcedureStep(procedure);

			Assert.AreEqual(2, procedure.ProcedureSteps.Count);
			Assert.AreEqual(ps1, procedure.DocumentationProcedureStep);
		}

		[Test]
		public void Test_DocumentationProcedureStep_Null()
		{
			var procedure = new Procedure();
			var ps1 = new ConcreteProcedureStep(procedure);
			var ps2 = new ConcreteReportingProcedureStep(procedure);

			Assert.AreEqual(2, procedure.ProcedureSteps.Count);
			Assert.IsNull(procedure.DocumentationProcedureStep);
		}

		[Test]
		public void Test_ActiveReport()
		{
			var procedure = new Procedure();
			Assert.IsNull(procedure.ActiveReport);

			// add a report, which is the active report
			var r1 = new Report(procedure);
			Assert.AreEqual(r1, procedure.ActiveReport);

			// cancel r1, so there is no acive report
			r1.Parts[0].Cancel();
			Assert.IsNull(procedure.ActiveReport);

			// add r2 which is then the active report
			var r2 = new Report(procedure);
			Assert.AreEqual(r2, procedure.ActiveReport);
		}

		[Test]
		public void Test_ActiveProtocol()
		{
			var procedure = new Procedure();
			Assert.IsNull(procedure.ActiveProtocol);

			var p1 = new Protocol(procedure);
			Assert.AreEqual(p1, procedure.ActiveProtocol);

			p1.Cancel();
			Assert.IsNull(procedure.ActiveProtocol);

			var p2 = new Protocol(procedure);
			Assert.AreEqual(p2, procedure.ActiveProtocol);
		}

		[Test]
		public void Test_PerformedTime()
		{
			var procedure = new Procedure();

			var ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
			ps1.Start(new Staff());
			ps1.Complete(Platform.Time);

			var ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());
			ps2.Start(new Staff());
			ps2.Complete(Platform.Time + TimeSpan.FromDays(1));

			var ps3 = new ModalityProcedureStep(procedure, "ps3", new Modality());
			ps3.Start(new Staff());
			ps3.Complete(Platform.Time + TimeSpan.FromDays(2));

			var ps4 = new ModalityProcedureStep(procedure, "ps4", new Modality());
			ps4.Start(new Staff());
			ps4.Complete(Platform.Time + TimeSpan.FromDays(3));

			Assert.AreEqual(ps4.EndTime, procedure.PerformedTime);
		}

		[Test]
		public void Test_PerformedTime_NoneCompleted()
		{
			var procedure = new Procedure();
			var ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
			var ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());

			Assert.IsNull(procedure.PerformedTime);
		}

		[Test]
		public void Test_IsPerformed()
		{
			var procedure = new Procedure();
			var ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
			var ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());
			var ps3 = new ModalityProcedureStep(procedure, "ps3", new Modality());

			Assert.IsFalse(procedure.IsPerformed);

			ps1.Start(new Staff());
			Assert.IsFalse(procedure.IsPerformed);

			ps1.Complete(Platform.Time);
			Assert.IsFalse(procedure.IsPerformed);

			ps2.Start(new Staff());
			ps3.Start(new Staff());
			Assert.IsFalse(procedure.IsPerformed);

			ps2.Discontinue();
			Assert.IsFalse(procedure.IsPerformed);

			ps3.Complete();
			Assert.IsTrue(procedure.IsPerformed);
		}

		[Test]
		public void Test_IsPerformed_NonePerformed()
		{
			var procedure = new Procedure();
			var ps1 = new ModalityProcedureStep(procedure, "ps1", new Modality());
			ps1.Start(new Staff());

			var ps2 = new ModalityProcedureStep(procedure, "ps2", new Modality());

			Assert.IsFalse(procedure.IsPerformed);
		}

		#endregion

		#region Public Operations Tests

		[Test]
		public void Test_CheckIn()
		{
			DateTime? now = DateTime.Now;
			var procedure = new Procedure { ProcedureCheckIn = new ProcedureCheckIn() };
			var regStep = new RegistrationProcedureStep(procedure);
			var staff = TestStaffFactory.CreateStaff();

			Assert.IsTrue(procedure.IsPreCheckIn);
			Assert.IsFalse(procedure.IsCheckedIn);
			Assert.IsFalse(procedure.IsCheckedOut);
			Assert.AreEqual(ActivityStatus.SC, regStep.State);
			Assert.IsNull(regStep.StartTime);

			procedure.CheckIn(staff, now);

			Assert.IsFalse(procedure.IsPreCheckIn);
			Assert.IsTrue(procedure.IsCheckedIn);
			Assert.IsFalse(procedure.IsCheckedOut);
			Assert.AreEqual(now, procedure.ProcedureCheckIn.CheckInTime);

			Assert.AreEqual(ActivityStatus.IP, regStep.State);
			Assert.AreEqual(staff, regStep.PerformingStaff);
			Assert.AreEqual(now, regStep.StartTime);
		}

		[Test]
		public void Test_CheckOut()
		{
			DateTime? now = DateTime.Now;
			var procedure = new Procedure { ProcedureCheckIn = new ProcedureCheckIn() };
			var regStep = new RegistrationProcedureStep(procedure);
			var staff = TestStaffFactory.CreateStaff();
			procedure.CheckIn(staff, now);

			Assert.IsFalse(procedure.IsPreCheckIn);
			Assert.IsTrue(procedure.IsCheckedIn);
			Assert.IsFalse(procedure.IsCheckedOut);
			Assert.AreEqual(ActivityStatus.IP, regStep.State);
			Assert.IsNull(regStep.EndTime);

			procedure.CheckOut(now);

			Assert.IsFalse(procedure.IsPreCheckIn);
			Assert.IsFalse(procedure.IsCheckedIn);
			Assert.IsTrue(procedure.IsCheckedOut);
			Assert.AreEqual(now, procedure.ProcedureCheckIn.CheckOutTime);

			Assert.AreEqual(ActivityStatus.CM, regStep.State);
			Assert.AreEqual(staff, regStep.PerformingStaff);
			Assert.AreEqual(now, regStep.EndTime);
		}

		[Test]
		public void Test_RevertCheckIn()
		{
			DateTime? now = DateTime.Now;
			var procedure = new Procedure { ProcedureCheckIn = new ProcedureCheckIn() };
			var regStep = new RegistrationProcedureStep(procedure);
			var staff = TestStaffFactory.CreateStaff();
			procedure.CheckIn(staff, now);

			Assert.IsFalse(procedure.IsPreCheckIn);
			Assert.IsTrue(procedure.IsCheckedIn);
			Assert.IsFalse(procedure.IsCheckedOut);
			Assert.AreEqual(ActivityStatus.IP, regStep.State);
			Assert.IsNull(regStep.EndTime);

			procedure.RevertCheckIn();

			Assert.IsTrue(procedure.IsPreCheckIn);
			Assert.IsFalse(procedure.IsCheckedIn);
			Assert.IsFalse(procedure.IsCheckedOut);
			Assert.IsNull(procedure.ProcedureCheckIn.CheckInTime);
			Assert.IsNull(procedure.ProcedureCheckIn.CheckOutTime);

			// the registration step is not reverted
			Assert.AreEqual(ActivityStatus.IP, regStep.State);
			Assert.AreEqual(staff, regStep.PerformingStaff);
		}

		[Test]
		public void Test_GetProcedureSteps()
		{
			var procedure = new Procedure();

			Assert.IsEmpty(procedure.GetProcedureSteps(delegate { return true; }));

			var ps1 = new DocumentationProcedureStep(procedure);
			var ps2 = new ModalityProcedureStep(procedure, "1", new Modality());
			var ps3 = new ModalityProcedureStep(procedure, "2", new Modality());
			var ps4 = new ProtocolAssignmentStep();
			procedure.AddProcedureStep(ps4);

			Assert.AreEqual(4, procedure.GetProcedureSteps(delegate { return true; }).Count);
			Assert.IsEmpty(procedure.GetProcedureSteps(delegate { return false; }));
			Assert.AreEqual(1, procedure.GetProcedureSteps(ps => ps is DocumentationProcedureStep).Count);
			Assert.AreEqual(2, procedure.GetProcedureSteps(ps => ps is ModalityProcedureStep).Count);
			Assert.AreEqual(0, procedure.GetProcedureSteps(ps => ps is ReportingProcedureStep).Count);
			Assert.AreEqual(1, procedure.GetProcedureSteps(ps => ps is ProtocolProcedureStep).Count);
		}

		[Test]
		public void Test_GetProcedureStep()
		{
			var procedure = new Procedure();
			Assert.IsNull(procedure.GetProcedureStep(delegate { return true; }));

			var ps1 = new DocumentationProcedureStep(procedure);
			var ps2 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
			var ps3 = new ModalityProcedureStep(procedure, "new Modality", new Modality());
			var ps4 = new ConcreteReportingProcedureStep(procedure);

			Assert.IsNull(procedure.GetProcedureStep(delegate { return false; }));

			Assert.AreEqual(ps1, procedure.GetProcedureStep(ps => ps is DocumentationProcedureStep));
			Assert.AreEqual(ps2, procedure.GetProcedureStep(ps => ps is ModalityProcedureStep));
			Assert.AreEqual(ps4, procedure.GetProcedureStep(ps => ps is ReportingProcedureStep));
			Assert.IsNull(procedure.GetProcedureStep(ps => ps is ProtocolProcedureStep));
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CreateProcedureSteps()
		{
			var procedure = new Procedure();
			var ps = new DocumentationProcedureStep(procedure);

			procedure.CreateProcedureSteps();
		}

		[Test]
		public void Test_AddProcedureStep()
		{
			var procedure = new Procedure();

			var ps = new DocumentationProcedureStep();
			Assert.IsFalse(procedure.ProcedureSteps.Contains(ps));
			Assert.AreNotEqual(procedure, ps.Procedure);

			procedure.AddProcedureStep(ps);

			Assert.IsTrue(procedure.ProcedureSteps.Contains(ps));
			Assert.AreEqual(procedure, ps.Procedure);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_AddProcedureStep_StepProcedureNotNull()
		{
			var p1 = new Procedure();
			var ps = new DocumentationProcedureStep();
			Assert.IsNull(ps.Procedure);

			p1.AddProcedureStep(ps);
			Assert.AreEqual(p1, ps.Procedure);

			var p2 = new Procedure();
			p2.AddProcedureStep(ps);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_AddProcedureStep_StepNotScheduling()
		{
			var procedure = new Procedure();
			var procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
			procedureStep.Start(new Staff());

			Assert.AreEqual(ActivityStatus.IP, procedureStep.State);

			procedure.AddProcedureStep(procedureStep);
		}

		[Test]
		public void Test_Schedule()
		{
			var procedure = new Procedure();
			var ps = new RegistrationProcedureStep(procedure);
			var now = DateTime.Now;
			var later = now + TimeSpan.FromDays(3);

			Assert.IsNull(ps.Scheduling.StartTime);
			Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
			Assert.AreEqual(ActivityStatus.SC, ps.State);

			procedure.Schedule(now);

			Assert.AreEqual(now.Truncate(DateTimePrecision.Minute), ps.Scheduling.StartTime);
			Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
			Assert.AreEqual(ActivityStatus.SC, ps.State);

			procedure.Schedule(later);

			Assert.AreEqual(later.Truncate(DateTimePrecision.Minute), ps.Scheduling.StartTime);
			Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
			Assert.AreEqual(ActivityStatus.SC, ps.State);
		}

		[Test]
		public void Test_Schedule_PreStep_And_SchedulingOffset()
		{
			var procedure = new Procedure(new ProcedureType(), "111", "1.111");

			var protocolStep = new ProtocolAssignmentStep(new Protocol(procedure));
			procedure.AddProcedureStep(protocolStep);
			var registrationStep = new RegistrationProcedureStep(procedure);
			var modalityStep = new ModalityProcedureStep(procedure, "description", new Modality());
			var documentationStep = new DocumentationProcedureStep(procedure);
			var reportingStep = new ConcreteReportingProcedureStep(procedure);

			Assert.IsNull(protocolStep.Scheduling.StartTime);
			Assert.IsNull(registrationStep.Scheduling.StartTime);
			Assert.IsNull(modalityStep.Scheduling.StartTime);
			Assert.IsNull(documentationStep.Scheduling.StartTime);
			Assert.IsNull(reportingStep.Scheduling.StartTime);

			var now = DateTime.Now;
			procedure.Schedule(now);

			Assert.AreEqual(now.Truncate(DateTimePrecision.Minute), procedure.ScheduledStartTime);
			Assert.AreEqual(protocolStep.CreationTime, protocolStep.Scheduling.StartTime);
			Assert.AreEqual(procedure.ScheduledStartTime, registrationStep.Scheduling.StartTime);
			Assert.AreEqual(procedure.ScheduledStartTime, modalityStep.Scheduling.StartTime);
			Assert.IsNull(documentationStep.Scheduling.StartTime);
			Assert.IsNull(reportingStep.Scheduling.StartTime);
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Schedule_Cancelled()
		{
			var procedure = new Procedure();
			procedure.Cancel();
			Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

			procedure.Schedule(Platform.Time);
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Schedule_Discontinued()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Discontinue();
			Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

			procedure.Schedule(Platform.Time);
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Schedule_InProgress()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			Assert.AreEqual(ProcedureStatus.IP, procedure.Status);

			procedure.Schedule(Platform.Time);
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Schedule_Complete()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Complete(DateTime.Now);
			Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

			procedure.Schedule(Platform.Time);
		}

		[Test]
		public void Discontinue()
		{
			var procedure = new Procedure();
			var procedureStep = new ModalityProcedureStep(procedure, "New Modality", new Modality());
			procedureStep.Start(new Staff());

			Assert.AreEqual(ProcedureStatus.IP, procedure.Status);
			Assert.AreEqual(ActivityStatus.IP, procedureStep.State);

			procedure.Discontinue();

			Assert.AreEqual(ProcedureStatus.DC, procedure.Status);
			Assert.AreEqual(ActivityStatus.DC, procedureStep.State);
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Discontinue_Cancelled()
		{
			var procedure = new Procedure();
			procedure.Cancel();

			Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

			procedure.Discontinue();
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Discontinue_Scheduling()
		{
			var procedure = new Procedure();
			Assert.AreEqual(ProcedureStatus.SC, procedure.Status);

			procedure.Discontinue();
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Discontinue_Complete()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Complete(DateTime.Now);
			Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

			procedure.Discontinue();
		}

		[Test]
		[ExpectedException((typeof(WorkflowException)))]
		public void Test_Discontinue_DiscontinuedState()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Discontinue();
			Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

			procedure.Discontinue();
		}

		[Test]
		public void Test_Cancel()
		{
			var procedure = new Procedure();
			var ps = new ConcreteProcedureStep(procedure);
			Assert.AreEqual(ProcedureStatus.SC, procedure.Status);
			Assert.AreEqual(ActivityStatus.SC, ps.State);
			Assert.IsFalse(ps.IsPreStep);  // Only nPreStep will update procedure status

			procedure.Cancel();

			Assert.AreEqual(ActivityStatus.DC, ps.State);
			Assert.AreEqual(ProcedureStatus.CA, procedure.Status);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Cancel_CancelledState()
		{
			var procedure = new Procedure();
			procedure.Cancel();
			Assert.AreEqual(ProcedureStatus.CA, procedure.Status);

			procedure.Cancel();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Cancel_InProgress()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			Assert.AreEqual(ProcedureStatus.IP, procedure.Status);

			procedure.Cancel();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Cancel_Discontinued()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Discontinue();
			Assert.AreEqual(ProcedureStatus.DC, procedure.Status);

			procedure.Cancel();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_Cancel_Completed()
		{
			var procedure = new Procedure();
			var ps = new ModalityProcedureStep(procedure, "New modality.", new Modality());
			ps.Start(new Staff());
			procedure.Complete(DateTime.Now);
			Assert.AreEqual(ProcedureStatus.CM, procedure.Status);

			procedure.Cancel();
		}

		[Test]
		public void Test_GetWorkflowHistory_OneLevel()
		{
			var procedure = new Procedure();
			var ps1 = new DocumentationProcedureStep(procedure);
			var ps2 = new DocumentationProcedureStep(procedure);

			Assert.AreEqual(2, procedure.GetWorkflowHistory().Count);
		}

		/// NOTE: STRUCTURE OF TEST WORKFLOW HISTORY "TREES"
		// Procedure <-> ProcedureStep relationship:
		// -> In keeping with the model, 1 Procedure has multiple ProcedureSteps
		// -> Every two sibling nodes/children on each level are each constructed
		//    with ONE procedure
		// ProcedureStep <-> ProcedureStep relationship:
		// -> ProcedureSteps of the same parent node (except for the first level) are
		//    "IsRelatedStep" to each other
		// -> ProcedureSteps of adjacent levels are related via Linking, and the "IsLinked"
		//    property.
		// -> Only the first (or leftmost) child node of the adjacent level is linked,
		//    ProcedureSteps within the same level are related as previously mentioned.
		// The following ASCII-drawn diagram shows a rudimentary illustration of how a basic
		// binary tree-like test workflow history is constructed.
		// 
		// NOTE: Diagram nomenclature
		// -> px, where x is a number, letter, or word: Procedure
		// -> psx, where x is a number, letter, or word: ProcedureStep
		// -> px: all ProcedureSteps at this level are constructed with this Procedure
		// -> |
		//    |---->, directory style arrow indicates ProcedureStep linking
		// -> "->psx", indicates another ProcedureStep on the same level as the one above it

		///  pRoot:
		///  ->ps1
		///  |   p1:
		///  |---->ps11
		///      |   p11:
		///      |---->ps111
		///          ->ps112
		///      ->ps12
		///      |   p12:
		///      |---->ps121
		///          ->ps122
		///  ->ps2
		///  |   p2:
		///  |---->ps21
		///      |   p21:
		///      |---->ps211
		///          ->ps212
		///      ->ps22
		///      |   p22:
		///      |---->ps221
		///          ->ps222


		[Test]
		public void Test_GetWorkflowHistory_TwoLevel()
		{
			// Testing an ideal 6 item Workflow history "tree"

			var pRoot = new Procedure(); // associative Procedure to first level
			var ps1 = new ConcreteProcedureStep(pRoot); // First level

			var p1 = new Procedure(); // associative Procedure to left branch of second level
			var ps11 = new ConcreteProcedureStep(p1); // Second level, left branch
			var ps12 = new ConcreteProcedureStep(p1); // Second level, left branch
			ps11.AddRelatedStep(ps12); // Relate children
			ps1.LinkTo(ps11); // Link adjacent levels

			var ps2 = new ConcreteProcedureStep(pRoot); // First level

			var p2 = new Procedure(); // associative Procedure to right branch of second level
			var ps21 = new ConcreteProcedureStep(p2); // Second level, right branch
			var ps22 = new ConcreteProcedureStep(p2); // Second level, right branch
			ps21.AddRelatedStep(ps22); // Relate children
			ps2.LinkTo(ps21); // Link adjacent levels

			Assert.AreEqual(6, pRoot.GetWorkflowHistory().Count); // Assert all items in "tree" are present in Workflow history
		}

		[Test]
		public void Test_GetWorkflowHistory_ThreeLevel()
		{
			var pRoot = new Procedure(); // associative Procedure to first level
			var ps1 = new ConcreteProcedureStep(pRoot);

			var p1 = new Procedure(); // associative Procedure to left branch of second level
			var ps11 = new ConcreteProcedureStep(p1);

			var p11 = new Procedure(); // associative Procedure to leftmost branch of third level
			var ps111 = new ConcreteProcedureStep(p11);
			var ps112 = new ConcreteProcedureStep(p11);
			ps111.AddRelatedStep(ps112); // relate steps of p11
			ps11.LinkTo(ps111); // link adjacent levels

			var ps12 = new ConcreteProcedureStep(p1);

			var p12 = new Procedure(); // associative Procedure to second leftmost branch of third level
			var ps121 = new ConcreteProcedureStep(p12);
			var ps122 = new ConcreteProcedureStep(p12);
			ps121.AddRelatedStep(ps122); // relate steps of p12
			ps12.LinkTo(ps121); // link adjacent levels
			ps11.AddRelatedStep(ps12); // relate steps of p1

			ps1.LinkTo(ps11); // link adjacent levels

			var ps2 = new ConcreteProcedureStep(pRoot);

			var p2 = new Procedure(); // associative Procedure to right branch of second level
			var ps21 = new ConcreteProcedureStep(p2);

			var p21 = new Procedure(); // associative Procedure to second rightmost branch of third level
			var ps211 = new ConcreteProcedureStep(p21);
			var ps212 = new ConcreteProcedureStep(p21);
			ps211.AddRelatedStep(ps212); // relate steps of p21
			ps21.LinkTo(ps211); // link adjacent levels

			var ps22 = new ConcreteProcedureStep(p2);

			var p22 = new Procedure(); // associative Procedure to rightmost branch of third level
			var ps221 = new ConcreteProcedureStep(p22);
			var ps222 = new ConcreteProcedureStep(p22);
			ps221.AddRelatedStep(ps222); // relate steps of p22
			ps22.LinkTo(ps221); // link adjacent levels
			ps21.AddRelatedStep(ps22); // relate steps of p2

			ps2.LinkTo(ps21); // link adjacent levels

			Assert.AreEqual(14, pRoot.GetWorkflowHistory().Count); // Assert all items in "tree" are present in Workflow history
		}

		[Test]
		public void Test_GetWorkflowHistory_UnrelatedSteps()
		{
			// This "tree" doesn't have the right children of each pair in the third level
			// Due to the abscence of relating these steps

			var pRoot = new Procedure(); // associative Procedure to first level
			var ps1 = new ConcreteProcedureStep(pRoot);

			var p1 = new Procedure(); // associative Procedure to left branch of second level
			var ps11 = new ConcreteProcedureStep(p1);

			var p11 = new Procedure(); // associative Procedure to leftmost branch of third level
			var ps111 = new ConcreteProcedureStep(p11);
			var ps112 = new ConcreteProcedureStep(p11); // is not related to ps111
			ps11.LinkTo(ps111); // link adjacent levels

			var ps12 = new ConcreteProcedureStep(p1);

			var p12 = new Procedure(); // associative Procedure to second leftmost branch of third level
			var ps121 = new ConcreteProcedureStep(p12);
			var ps122 = new ConcreteProcedureStep(p12); // is not related to ps121
			ps12.LinkTo(ps121); // link adjacent levels
			ps11.AddRelatedStep(ps12); // relate steps of p1

			ps1.LinkTo(ps11); // link adjacent levels

			var ps2 = new ConcreteProcedureStep(pRoot);

			var p2 = new Procedure(); // associative Procedure to right branch of second level
			var ps21 = new ConcreteProcedureStep(p2);

			var p21 = new Procedure(); // associative Procedure to second rightmost branch of third level
			var ps211 = new ConcreteProcedureStep(p21);
			var ps212 = new ConcreteProcedureStep(p21); // is not related to ps211
			ps21.LinkTo(ps211); // link adjacent levels

			var ps22 = new ConcreteProcedureStep(p2);

			var p22 = new Procedure(); // associative Procedure to rightmost branch of third level
			var ps221 = new ConcreteProcedureStep(p22);
			var ps222 = new ConcreteProcedureStep(p22); // is not related to ps221
			ps22.LinkTo(ps221); // link adjacent levels
			ps21.AddRelatedStep(ps22); // relate steps of p2

			ps2.LinkTo(ps21); // link adjacent levels

			Assert.AreEqual(10, pRoot.GetWorkflowHistory().Count);
			// Assert all items in "tree" are present in Workflow history... except these ones:
			Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps112));
			Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps122));
			Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps212));
			Assert.IsFalse(pRoot.GetWorkflowHistory().Contains(ps222));
		}

		[Test]
		public void Test_GetWorkflowHistory_Empty()
		{
			var procedure = new Procedure();

			Assert.IsEmpty(procedure.GetWorkflowHistory());
		}

		#endregion
	}
}

#endif
