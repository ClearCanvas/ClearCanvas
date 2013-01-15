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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
	public class Utility
	{
		static public bool PatientIdentifiersFromIdenticalInformationAuthority(Patient thisPatient, Patient otherPatient)
		{
			foreach (PatientProfile x in thisPatient.Profiles)
				foreach (PatientProfile y in otherPatient.Profiles)
					if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
						return true;

			return false;
		}

		static public void ReconcileBetweenInformationAuthorities(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			if (Utility.PatientIdentifiersFromIdenticalInformationAuthority(thisPatient, otherPatient))
				throw new PatientReconciliationException("assigning authorities not identical conflict - cannot reconcile");

			// copy patient profiles over
			foreach (PatientProfile profile in otherPatient.Profiles)
				thisPatient.AddProfile(profile);

			ReconnectRelatedPatientInformation(thisPatient, otherPatient, context);
		}

		static public void ReconcileWithinInformationAuthority(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			ReconnectRelatedPatientInformation(thisPatient, otherPatient, context);
		}

		/// <summary>
		/// The specified visit and its associated orders get moved from otherPatient to thisPatient
		/// </summary>
		/// <param name="thisPatient"></param>
		/// <param name="otherPatient"></param>
		/// <param name="visit"></param>
		/// <param name="context"></param>
		static public void MoveVisit(Patient thisPatient, Patient otherPatient, Visit visit, IPersistenceContext context)
		{
			var orderCriteria = new OrderSearchCriteria();
			orderCriteria.Visit.EqualTo(visit);
			var visitOrders = context.GetBroker<IOrderBroker>().Find(orderCriteria);
			foreach (var order in visitOrders)
			{
				order.Patient = thisPatient;
			}

			visit.Patient = thisPatient;
		}

		/// <summary>
		/// All pertinent data other than the Profiles gets copied from otherPatient to thisPatient
		/// </summary>
		/// <param name="thisPatient"></param>
		/// <param name="otherPatient"></param>
		/// <param name="context"></param>
		static private void ReconnectRelatedPatientInformation(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			foreach (PatientNote note in otherPatient.Notes)
			{
				thisPatient.AddNote(note);
			}

			OrderSearchCriteria orderCriteria = new OrderSearchCriteria();
			orderCriteria.Patient.EqualTo(otherPatient);
			IList<Order> otherOrders = context.GetBroker<IOrderBroker>().Find(orderCriteria);
			foreach (Order order in otherOrders)
			{
				order.Patient = thisPatient;
			}

			VisitSearchCriteria visitCriteria = new VisitSearchCriteria();
			visitCriteria.Patient.EqualTo(otherPatient);
			IList<Visit> otherVisits = context.GetBroker<IVisitBroker>().Find(visitCriteria);
			foreach (Visit visit in otherVisits)
			{
				visit.Patient = thisPatient;
			}

			// TODO: delete the otherPatient
		}

	}
}
