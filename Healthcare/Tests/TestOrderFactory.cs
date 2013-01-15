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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Tests
{
	public static class TestOrderFactory
    {


		public static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps)
        {
            return CreateOrder(numProcedures, numMpsPerProcedure, createProcedureSteps, true);
        }
		public static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
		{
			Patient patient = TestPatientFactory.CreatePatient();
			Visit visit = TestVisitFactory.CreateVisit(patient);
			var facility = TestFacilityFactory.CreateFacility();
			return CreateOrder(patient, visit, facility, "10000001", numProcedures, numMpsPerProcedure, createProcedureSteps, schedule);
		}

		public static Order CreateOrder(Patient patient, Visit visit, string accession, int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
		{
			var facility = TestFacilityFactory.CreateFacility();
			return CreateOrder(patient, visit, facility, accession, numProcedures, numMpsPerProcedure, createProcedureSteps,
			                   schedule);
		}

		public static Order CreateOrder(Patient patient, Visit visit, Facility facility, string accession, int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
        {
			var procedureNumberBroker = new TestProcedureNumberBroker();
			var dicomUidBroker = new TestDicomUidBroker();
			DateTime? scheduleTime = DateTime.Now;

            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService(numProcedures);
            string reasonForStudy = "Test";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();

            Order order =  Order.NewOrder(new OrderCreationArgs(
				Platform.Time,
				TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)),
				null,
                accession,
                patient,
                visit,
                ds,
                reasonForStudy,
                OrderPriority.R,
                facility,
                facility,
                scheduleTime,
                orderingPrac,
                new List<ResultRecipient>()),
				procedureNumberBroker,
				dicomUidBroker);

            if(createProcedureSteps)
            {
                foreach (Procedure proc in order.Procedures)
                {
                    AddProcedureSteps(proc, numMpsPerProcedure);
                }
            }

            DateTime dt = DateTime.Now;
            if(schedule)
            {
                foreach (Procedure proc in order.Procedures)
                {
                	proc.Schedule(dt);
                }
            }

            return order;
        }

        private static void AddProcedureSteps(Procedure procedure, int numMps)
        {
            Modality m = new Modality("01", "CT", procedure.PerformingFacility, null, null);

            for (int s = 0; s < numMps; s++)
            {
                ModalityProcedureStep step = new ModalityProcedureStep();
                step.Description = "MPS 10" + s;
                step.Modality = m;
                procedure.AddProcedureStep(step);
            }
        }
    }
}
