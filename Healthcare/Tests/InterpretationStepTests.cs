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
    public class InterpretationStepTests
    {
        #region Property Test

        [Test]
        public void Test_Name()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            Assert.AreEqual("Interpretation", procedureStep.Name);
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor_Procedure()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            Assert.IsNull(procedureStep.ReportPart);
        }

        #endregion

        #region Method Tests

        // LinkProcedure in Report does not allow a null Report to be linked to another
        // one, understandably so.... 
        // TODO: This test is incomplete or not required, I'm not sure yet...
        //[Test]
        //public void Test_LinkTo()
        //{
        //    Procedure p1 = new Procedure();
        //    Report r1 = new Report(p1);
        //    Procedure p2 = new Procedure();
        //    Report r2 = new Report(p2);
            
        //    InterpretationStep ps1 = new InterpretationStep(p1),
        //                       ps2 = new InterpretationStep(p2);
        //    ps1.ReportPart = new ReportPart(r1, 0);
        //    ps2.ReportPart = new ReportPart(r2, 0);

        //    ps1.LinkTo(ps2);

        //    Assert.IsTrue(ps2.Report.Procedures.Contains(p1));
        //}

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_LinkTo_NullReport()
        {
            InterpretationStep ps1 = new InterpretationStep(new Procedure());
            InterpretationStep ps2 = new InterpretationStep(new Procedure());
            ps1.LinkTo(ps2);
        }

        // assumption is suspend represents every other state from which InterpretationStep could
        // go to from that is not "Complete" and thereby will not trigger the exception and will
        // set the ActivityStatus properly
        [Test]
        public void Test_Suspend()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);
            Assert.AreEqual(ActivityStatus.SC, procedureStep.State);

            procedureStep.Suspend();

            Assert.AreEqual(ActivityStatus.SU, procedureStep.State);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_NullReport()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            procedureStep.Complete();
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            InterpretationStep procedureStep = new InterpretationStep(procedure);

            InterpretationStep newStep = (InterpretationStep)procedureStep.Reassign(new Staff());

            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOf(typeof (InterpretationStep), newStep);
        }
        #endregion
    }
}

#endif