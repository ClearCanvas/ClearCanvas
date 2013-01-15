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
    public class ReportingProcedureStepTests
    {
        class ConcreteReportingProcedureStep : ReportingProcedureStep
        {
            public ConcreteReportingProcedureStep(Procedure procedure, ReportPart reportPart)
                :base(procedure, reportPart)
            {
                
            }

            public ConcreteReportingProcedureStep(ReportingProcedureStep procedureStep)
                : base(procedureStep)
            {

            }

            public override string Name
            {
                get { return "Concrete"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new ConcreteReportingProcedureStep(this.Procedure, this.ReportPart);
            }
        }

        class GenericProtocolProcedureStep : ProtocolProcedureStep
        {
            public GenericProtocolProcedureStep(Protocol protocol)
                : base(protocol)
            {

            }

            public override string Name
            {
                get { return "Generic Protocol"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new GenericProtocolProcedureStep(this.Protocol);
            }
        }

        #region Property Tests

        [Test]
        public void Test_IsPreStep()
        {
            Procedure procedure = new Procedure();
            ReportPart reportPart = new ReportPart();

            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, reportPart);

            Assert.IsFalse(procedureStep.IsPreStep);
        }

        [Test]
        public void Test_Report()
        {
            Procedure procedure = new Procedure();
            ReportPart reportPart = new ReportPart();

            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, reportPart);

            Assert.IsNotNull(procedureStep.ReportPart);
            Assert.AreEqual(reportPart.Report, procedureStep.Report);
        }

        [Test]
        public void Test_Report_NullReportPart()
        {
            Procedure procedure = new Procedure();

            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, null);

            Assert.IsNull(procedureStep.ReportPart);
            Assert.IsNull(procedureStep.Report);
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            ReportPart reportPart = new ReportPart();

            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, reportPart);

            Assert.AreEqual(reportPart, procedureStep.ReportPart);

            // Passing in last procedure step into a new one
            ConcreteReportingProcedureStep nextStep = new ConcreteReportingProcedureStep(procedureStep);

            Assert.AreEqual(procedure, nextStep.Procedure);
            Assert.AreEqual(reportPart, nextStep.ReportPart);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_GetLinkedProcedures()
        {
            Procedure procedure = new Procedure();
            Procedure procedure2 = new Procedure();
            Report report = new Report(procedure);
            report.Procedures.Add(procedure2);
            ReportPart reportPart = new ReportPart(report, 1);

            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, reportPart);

            Assert.IsNotNull(procedureStep.ReportPart);
            Assert.IsNotNull(procedureStep.ReportPart.Report);
            Assert.AreEqual(2, procedureStep.ReportPart.Report.Procedures.Count);
            Assert.AreEqual(1, procedureStep.GetLinkedProcedures().Count);
            Assert.Contains(procedure2, procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetLinkedProcedures_ReportNull()
        {
            Procedure procedure = new Procedure();
            ConcreteReportingProcedureStep procedureStep = new ConcreteReportingProcedureStep(procedure, null);

            Assert.IsNull(procedureStep.ReportPart);
            Assert.IsNotNull(procedureStep.GetLinkedProcedures());
            Assert.IsEmpty(procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetRelatedProcedureSteps()
        {
            Procedure procedure = new Procedure();
            Report report = new Report(procedure);
            ReportPart reportPart = new ReportPart(report, 1);

            // Testing that procedure steps with tied report will be related steps
            ConcreteReportingProcedureStep p1 = new ConcreteReportingProcedureStep(procedure, reportPart);
            ConcreteReportingProcedureStep p2 = new ConcreteReportingProcedureStep(procedure, reportPart);

            // expect that each ps is tied by common report
            Assert.AreEqual(report, p1.Report);
            Assert.AreEqual(report, p2.Report);
            Assert.Contains(p2, p1.GetRelatedProcedureSteps());
            Assert.Contains(p1, p2.GetRelatedProcedureSteps());

            // testing that proedure steps with null report will have no relatives
            p1 = new ConcreteReportingProcedureStep(procedure, null);
            p2 = new ConcreteReportingProcedureStep(procedure, null);

            // expect the each ps has no relatives
            Assert.IsEmpty(p1.GetRelatedProcedureSteps());
            Assert.IsEmpty(p2.GetRelatedProcedureSteps());

            // Testing that the relative has to be a report step
            GenericProtocolProcedureStep p3 = new GenericProtocolProcedureStep(new Protocol(procedure));
            procedure.AddProcedureStep(p3);

            // expect that the related psteps are not related to the different step
            Assert.IsFalse(p3.GetRelatedProcedureSteps().Contains(p1));
            Assert.IsFalse(p3.GetRelatedProcedureSteps().Contains(p2));
            Assert.IsFalse(p1.GetRelatedProcedureSteps().Contains(p3));
            Assert.IsFalse(p2.GetRelatedProcedureSteps().Contains(p3));
        }

        #endregion
    }
}

#endif
