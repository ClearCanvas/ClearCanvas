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
    public class VerificationStepTests
    {
        [Test]
        public void Test_Name()
        {
            VerificationStep procedureStep = new VerificationStep();

            Assert.AreEqual("Verification", procedureStep.Name);
        }

        [Test]
        public void Test_Complete()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            InterpretationStep previousStep = new InterpretationStep(procedure);
            previousStep.ReportPart = reportPart;
            VerificationStep procedureStep = new VerificationStep(previousStep);
            procedureStep.Start(performer);

            procedureStep.Complete();

            Assert.AreEqual(performer, procedureStep.ReportPart.Verifier);
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            reportPart.Supervisor = new Staff();
            VerificationStep procedureStep = new VerificationStep();
            procedureStep.Procedure = procedure;
            procedureStep.ReportPart = reportPart;
            Assert.IsNotNull(procedureStep.ReportPart);

            VerificationStep newStep = (VerificationStep)procedureStep.Reassign(performer);

            Assert.AreEqual(performer, newStep.ReportPart.Supervisor);
            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOf(typeof(VerificationStep), newStep);
        }
    }
}

#endif