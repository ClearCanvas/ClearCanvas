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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
	public class Operations
	{
		public abstract class RegistrationOperation
		{
		}

		public class CheckIn : RegistrationOperation
		{
			public void Execute(Procedure rp, Staff checkInStaff, DateTime? checkInTime, IWorkflow workflow)
			{
				rp.CheckIn(checkInStaff, checkInTime);
			}
		}

		public class ReconcilePatient : RegistrationOperation
		{
			public void Execute(List<Patient> patientsToReconcile, IWorkflow workflow)
			{
				// reconcile all patients
				for (var i = 1; i < patientsToReconcile.Count; i++)
				{
					Reconcile(patientsToReconcile[0], patientsToReconcile[i], workflow);
				}
			}

			/// <summary>
			/// Reconciles the specified patient to this patient
			/// </summary>
			/// <param name="thisPatient"></param>
			/// <param name="otherPatient"></param>
			/// <param name="workflow"></param>
			private static void Reconcile(Patient thisPatient, Patient otherPatient, IWorkflow workflow)
			{
				if (PatientIdentifierConflictsFound(thisPatient, otherPatient))
					throw new PatientReconciliationException("assigning authority conflict - cannot reconcile");

				// copy the collection to iterate
				var otherProfiles = new List<PatientProfile>(otherPatient.Profiles);
				foreach (var profile in otherProfiles)
				{
					thisPatient.AddProfile(profile);
				}

				// copy the collection to iterate
				var otherNotes = new List<PatientNote>(otherPatient.Notes);
				foreach (var note in otherNotes)
				{
					thisPatient.AddNote(note);
				}

				// copy the collection to iterate
				var otherAttachments = new List<PatientAttachment>(otherPatient.Attachments);
				foreach (var attachment in otherAttachments)
				{
					otherPatient.Attachments.Remove(attachment);
					thisPatient.Attachments.Add(attachment);
				}

				// copy the collection to iterate
				var otherAllergies = new List<Allergy>(otherPatient.Allergies);
				foreach (var allergy in otherAllergies)
				{
					otherPatient.Allergies.Remove(allergy);
					thisPatient.Allergies.Add(allergy);
				}

				var visitCriteria = new VisitSearchCriteria();
				visitCriteria.Patient.EqualTo(otherPatient);
				var otherVisits = workflow.GetBroker<IVisitBroker>().Find(visitCriteria);
				foreach (var visit in otherVisits)
				{
					visit.Patient = thisPatient;
				}

				var orderCriteria = new OrderSearchCriteria();
				orderCriteria.Patient.EqualTo(otherPatient);
				var otherOrders = workflow.GetBroker<IOrderBroker>().Find(orderCriteria);
				foreach (var order in otherOrders)
				{
					order.Patient = thisPatient;
				}
			}

			/// <summary>
			/// Check if any of the profiles in each patient have an Mrn with the same assigning authority.
			/// </summary>
			/// <param name="thisPatient"></param>
			/// <param name="otherPatient"></param>
			/// <returns>Returns true if any profiles for the other patient and any profiles for this patient have an Mrn with the same assigning authority.</returns>
			private static bool PatientIdentifierConflictsFound(Patient thisPatient, Patient otherPatient)
			{
				foreach (var x in thisPatient.Profiles)
					foreach (var y in otherPatient.Profiles)
						if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
							return true;

				return false;
			}
		}
	}
}
