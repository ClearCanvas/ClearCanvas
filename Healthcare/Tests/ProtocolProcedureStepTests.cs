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

using NUnit.Framework;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class ProtocolProcedureStepTests
    {
        class ConcreteProtocolProcedureStep : ProtocolProcedureStep
        {
            public ConcreteProtocolProcedureStep(Protocol protocol)
                :base(protocol)
            {
                
            }

            public override string Name
            {
                get { return "whatever"; }
            }

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new ConcreteProtocolProcedureStep(this.Protocol);
            }
        }
        class GenericReportingProcedureStep : ReportingProcedureStep
        {
            public GenericReportingProcedureStep(Procedure procedure)
                : base(procedure, new ReportPart())
            {
                
            }

            public override string Name
            {
                get { return "Generic Reporting"; }
            } 

            protected override ProcedureStep CreateScheduledCopy()
            {
                return new GenericReportingProcedureStep(this.Procedure);
            }
        }

        #region Property Tests

        [Test]
        public void Test_IsPreStep()
        {
            Protocol protocol = new Protocol();
            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);

            Assert.IsTrue(procedureStep.IsPreStep);
        }

        #endregion

        #region Constructor Test

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);
            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);

            Assert.AreEqual(protocol, procedureStep.Protocol);
        }

        #endregion

        #region Method Tests

        [Test]
        public void Test_GetLinkedProcedures_NoLinkedProcedures()
        {
            Procedure procedure = new Procedure();
            Protocol protocol = new Protocol(procedure);

            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);
            procedureStep.Procedure = procedure; //TODO why isn't this passed to constructor?

            Assert.IsNotNull(procedureStep.Protocol);
            Assert.AreEqual(1, procedureStep.Protocol.Procedures.Count);
            Assert.IsNotNull(procedureStep.GetLinkedProcedures());
            Assert.IsEmpty(procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetLinkedProcedures_OneLinkedProcedure()
        {
            Procedure procedure = new Procedure();
            Procedure procedure2 = new Procedure();
            Protocol protocol = new Protocol(procedure);
            protocol.Procedures.Add(procedure2);

            ConcreteProtocolProcedureStep procedureStep = new ConcreteProtocolProcedureStep(protocol);
            procedureStep.Procedure = procedure; //TODO why isn't this passed to constructor?

            Assert.IsNotNull(procedureStep.Protocol);
            Assert.AreEqual(2, procedureStep.Protocol.Procedures.Count);
            Assert.AreEqual(1, procedureStep.GetLinkedProcedures().Count);
            Assert.Contains(procedure2, procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_GetRelatedProcedureSteps()
        {
            Procedure procedure = new Procedure();

            // Testing that procedure steps with tied protocols will be related steps
            Protocol protocol = new Protocol(procedure);
            ConcreteProtocolProcedureStep ps1 = new ConcreteProtocolProcedureStep(protocol);
            procedure.AddProcedureStep(ps1);
            ConcreteProtocolProcedureStep ps2 = new ConcreteProtocolProcedureStep(protocol);
            procedure.AddProcedureStep(ps2);

            // expect that each ps is tied by common protocol
            Assert.AreEqual(protocol, ps1.Protocol);
            Assert.AreEqual(protocol, ps2.Protocol);
            Assert.Contains(ps2, ps1.GetRelatedProcedureSteps());
            Assert.Contains(ps1, ps2.GetRelatedProcedureSteps());
            
            // Testing that the relative has to be a protocol step
            GenericReportingProcedureStep ps3 = new GenericReportingProcedureStep(procedure);
            Assert.IsTrue(procedure.ProcedureSteps.Contains(ps3));

            // expect that the related psteps are not related to the different step
            Assert.IsFalse(ps3.GetRelatedProcedureSteps().Contains(ps1));
            Assert.IsFalse(ps3.GetRelatedProcedureSteps().Contains(ps2));
            Assert.IsFalse(ps1.GetRelatedProcedureSteps().Contains(ps3));
            Assert.IsFalse(ps2.GetRelatedProcedureSteps().Contains(ps3));
        }
 
        #endregion
    }
}

#endif