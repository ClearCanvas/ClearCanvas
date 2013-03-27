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
using ClearCanvas.Workflow;
using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProtocolAssignmentStepTests
    {
        #region Property Tests

        [Test]
        public void Test_CanAcceptRejctSuspendSave()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Start(new Staff());

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.IsTrue(procedureStep.CanAccept);
            Assert.IsTrue(procedureStep.CanReject);
            Assert.IsTrue(procedureStep.CanSuspend);
            Assert.IsTrue(procedureStep.CanSave);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
            Assert.IsFalse(procedureStep.CanAccept);
            Assert.IsFalse(procedureStep.CanReject);
            Assert.IsFalse(procedureStep.CanSuspend);
            Assert.IsFalse(procedureStep.CanSave);

            procedureStep.Complete();

            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.IsFalse(procedureStep.CanAccept);
            Assert.IsFalse(procedureStep.CanReject);
            Assert.IsFalse(procedureStep.CanSuspend);
            Assert.IsFalse(procedureStep.CanSave);
        }

        [Test]
        public void Test_CanApprove()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(protocol);

            Assert.AreEqual(ProtocolStatus.PN, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            protocol.SubmitForApproval();

            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsTrue(procedureStep.CanApprove);

            procedureStep.Start(new Staff());

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsTrue(procedureStep.CanApprove);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            procedureStep.Complete();

            Assert.AreEqual(ActivityStatus.CM, procedureStep.State);
            Assert.AreEqual(ProtocolStatus.AA, procedureStep.Protocol.Status);
            Assert.IsFalse(procedureStep.CanApprove);

            // TODO : test all other Protocol state conditions
        }

        [Test]
        public void Test_CanEdit()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());
            Staff staff = new Staff();
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);
            Assert.IsFalse(procedureStep.CanEdit(staff));

            procedureStep.Start(staff);

            Assert.AreEqual(ActivityStatus.IP, procedureStep.State);
            Assert.AreEqual(staff, procedureStep.PerformingStaff);
            Assert.IsTrue(procedureStep.CanEdit(staff));
            Assert.IsFalse(procedureStep.CanEdit(new Staff()));

            // TODO : test all other Activity state conditions
        }

        [Test]
        public void Test_Name()
        {
            ProtocolAssignmentStep procedureStep = new ProtocolAssignmentStep(new Protocol());

            Assert.AreEqual("Protocol Assignment", procedureStep.Name);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_LinkTo()
        {
            Procedure p1 = new Procedure();
            Procedure p2 = new Procedure();
            Protocol protocol = new Protocol(p1);
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep(protocol),
                                   ps2 = new ProtocolAssignmentStep(new Protocol(p2));
            ps1.Procedure = p1;
            ps2.Procedure = p2;
            ps1.LinkTo(ps2);

            Assert.IsTrue(ps2.Protocol.Procedures.Contains(p1));
            Assert.Contains(p1, ps2.GetLinkedProcedures());
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_LinkTo_NullProtocol()
        {
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep();
            ProtocolAssignmentStep ps2 = new ProtocolAssignmentStep();
            ps1.LinkTo(ps2);
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            Staff staff1 = new Staff(),
                  staff2 = new Staff();
            ProtocolAssignmentStep ps1 = new ProtocolAssignmentStep(protocol);
            ps1.Procedure = procedure;
            ps1.Start(staff1);

            ProcedureStep ps2 = ps1.Reassign(staff2);

            Assert.IsInstanceOf(typeof (ProtocolAssignmentStep), ps2);
            Assert.AreNotEqual(ps1, ps2);
            Assert.AreEqual(ps1.Protocol, ((ProtocolAssignmentStep)ps2).Protocol);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps2));
        }

        #endregion
    }
}

#endif