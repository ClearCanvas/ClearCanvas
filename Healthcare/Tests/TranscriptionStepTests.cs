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
using NUnit.Framework;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class TranscriptionStepTests
    {
        [Test]
        public void Test_Name()
        {
            TranscriptionStep procedureStep = new TranscriptionStep();

            Assert.AreEqual("Transcription", procedureStep.Name);
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
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);
            procedureStep.Start(performer);
            
            procedureStep.Complete();

            Assert.AreEqual(performer, procedureStep.ReportPart.Transcriber);
        }

        [Test]
        [ExpectedException(typeof(WorkflowException))]
        public void Test_Complete_NullReportPart()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 0);
            Staff performer = new Staff();
            InterpretationStep previousStep = new InterpretationStep(procedure);
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);
            procedureStep.Start(performer);
            
            procedureStep.Complete();
        }

        [Test]
        public void Test_Reassign()
        {
            InterpretationStep previousStep = new InterpretationStep(new Procedure());
            TranscriptionStep procedureStep = new TranscriptionStep(previousStep);

            TranscriptionStep newStep = (TranscriptionStep)procedureStep.Reassign(new Staff());

            Assert.AreEqual(procedureStep, newStep);
            Assert.IsInstanceOf(typeof(TranscriptionStep), newStep);
        }
    }
}

#endif