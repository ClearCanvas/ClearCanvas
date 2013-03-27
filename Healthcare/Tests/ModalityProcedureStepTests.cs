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
    public class ModalityProcedureStepTests
    {
        #region Property Tests

        [Test]
        public void Test_Name()
        {
            Procedure procedure = new Procedure();
            string description = "description.";
            Modality modality = new Modality();

            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, description, modality);

            Assert.AreEqual("Modality", procedureStep.Name);
        }

        [Test]
        public void Test_IsPreStep()
        {
            Procedure procedure = new Procedure();
            string description = "description.";
            Modality modality = new Modality();

            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, description, modality);

            Assert.IsFalse(procedureStep.IsPreStep);
        }

        #endregion

        [Test]
        public void Test_Constructor()
        {
            Procedure procedure = new Procedure();
            string description = "description.";
            Modality modality = new Modality();

            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, description, modality);

            Assert.AreEqual(procedure, procedureStep.Procedure);
            Assert.AreEqual(description, procedureStep.Description);
            Assert.AreEqual(modality, procedureStep.Modality);
        }

        [Test]
        public void Test_GetLinkedProcedures()
        {
            Procedure procedure = new Procedure();
            string description = "description.";
            Modality modality = new Modality();

            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, description, modality);

            Assert.IsNotNull(procedureStep.GetLinkedProcedures());
            Assert.IsEmpty(procedureStep.GetLinkedProcedures());
        }

        [Test]
        public void Test_Reassign()
        {
            Procedure procedure = new Procedure();
            string description = "description.";
            Modality modality = new Modality();
            ModalityProcedureStep procedureStep = new ModalityProcedureStep(procedure, description, modality);
            procedureStep.Suspend();

            Staff performer = new Staff();
            ProcedureStep newStep = procedureStep.Reassign(performer); // Perform event

            // just need to test that it returns a new instance of this class
            // everything else has been covered in base class tests
            Assert.IsNotNull(newStep);
            Assert.AreNotEqual(this, newStep);
            Assert.IsInstanceOf(typeof(ModalityProcedureStep), newStep);
            Assert.AreEqual(procedure, newStep.Procedure);
            Assert.AreEqual(description, ((ModalityProcedureStep)newStep).Description);
            Assert.AreEqual(modality, ((ModalityProcedureStep)newStep).Modality);
        }

        [Test]
        public void Test_GetRelatedProcedureSteps()
        {
            Procedure p1 = new Procedure();
            string description = "description.";
            Modality modality = new Modality();

            // attach 2 procedure steps to p1
            ModalityProcedureStep ps11 = new ModalityProcedureStep(p1, description, modality);
            ModalityProcedureStep ps12 = new ModalityProcedureStep(p1, description, modality);

            // expect that each ps is only related to itself
            Assert.IsEmpty(ps11.GetRelatedProcedureSteps());
            Assert.IsEmpty(ps12.GetRelatedProcedureSteps());
        }
    }
}

#endif