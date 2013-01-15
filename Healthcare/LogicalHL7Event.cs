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

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Events that may generate HL7 messages. The string constant must contain either "Order" or "Procedure", this is
	/// checked in the Event constructor to provide some safety that a good event type is passed in
	/// </summary>
	public class LogicalHL7Event
	{
		public class PatientEvent : LogicalHL7Event
		{
			public PatientEvent(string eventType)
				: base(eventType)
			{
			}

			public void EnqueueEvents(PatientProfile patientProfile)
			{
				var queueItem = new WorkQueueItem(WorkQueueItemType);
				queueItem.ExtendedProperties.Add("EventType", this.EventType);
				queueItem.ExtendedProperties.Add("PatientOID", patientProfile.Patient.OID.ToString());
				queueItem.ExtendedProperties.Add("PatientProfileOID", patientProfile.OID.ToString());
				queueItem.ExtendedProperties.Add("Mrn", patientProfile.Mrn.ToString());

				EnqueueWorkItem(queueItem);
			}
		}

		public class OrderEvent : LogicalHL7Event
		{
			public OrderEvent(string eventType)
				: base(eventType)
			{
			}

			public void EnqueueEvents(Order order)
			{
				var queueItem = new WorkQueueItem(WorkQueueItemType);
				queueItem.ExtendedProperties.Add("EventType", this.EventType);
				queueItem.ExtendedProperties.Add("OrderOID", order.OID.ToString());
				queueItem.ExtendedProperties.Add("AccessionNumber", order.AccessionNumber);
				
				EnqueueWorkItem(queueItem);
			}
		}

		public class ProcedureEvent : LogicalHL7Event
		{
			public ProcedureEvent(string eventType)
				: base(eventType)
			{
			}

			public void EnqueueEvents(Procedure procedure)
			{
				var order = procedure.Order;

				var queueItem = new WorkQueueItem(WorkQueueItemType);
				queueItem.ExtendedProperties.Add("EventType", this.EventType);
				queueItem.ExtendedProperties.Add("OrderOID", order.OID.ToString());
				queueItem.ExtendedProperties.Add("AccessionNumber", order.AccessionNumber);
				queueItem.ExtendedProperties.Add("ProcedureOID", procedure.OID.ToString());
				queueItem.ExtendedProperties.Add("ProcedureNumber", procedure.Number);
				
				EnqueueWorkItem(queueItem);
			}
		}

		public class ReportEvent : LogicalHL7Event
		{
			public ReportEvent(string eventType)
				: base(eventType)
			{
			}

			public void EnqueueEvents(Report report)
			{
				var orders = report.Procedures.Select(rp => rp.Order).Distinct();

				var workItems =  orders.Select(order =>
						{
							var queueItem = new WorkQueueItem(WorkQueueItemType);
							queueItem.ExtendedProperties.Add("EventType", this.EventType);
							queueItem.ExtendedProperties.Add("OrderOID", order.OID.ToString());
							queueItem.ExtendedProperties.Add("AccessionNumber", order.AccessionNumber);
							queueItem.ExtendedProperties.Add("ReportOID", report.OID.ToString());
							return queueItem;
						});

				EnqueueWorkItems(workItems);
			}
		}

		public static readonly PatientEvent PatientCreated = new PatientEvent("PatientCreated");
		public static readonly PatientEvent PatientProfileModified = new PatientEvent("PatientProfileModified");

		public static readonly OrderEvent OrderCreated = new OrderEvent("OrderCreated");
		public static readonly OrderEvent OrderModified = new OrderEvent("OrderModified");
		public static readonly OrderEvent OrderCancelled = new OrderEvent("OrderCancelled");

		public static readonly ProcedureEvent ProcedureCreated = new ProcedureEvent("ProcedureCreated");
		public static readonly ProcedureEvent ProcedureModified = new ProcedureEvent("ProcedureModified");
		public static readonly ProcedureEvent ProcedureCancelled = new ProcedureEvent("ProcedureCancelled");

		public static readonly ReportEvent ReportPublished = new ReportEvent("ReportPublished");

		public const string WorkQueueItemType = "Logical HL7 Event";


		public LogicalHL7Event(string eventType)
		{
			EventType = eventType;
		}

		public string EventType { get; private set; }

		protected void EnqueueWorkItem(WorkQueueItem workItem)
		{
			if (!new LogicalHL7EventSettings().EnableEvents)
				return;
			PersistenceScope.CurrentContext.Lock(workItem, DirtyState.New);
		}

		protected void EnqueueWorkItems(IEnumerable<WorkQueueItem> workItems)
		{
			foreach (var workItem in workItems)
			{
				EnqueueWorkItem(workItem);
			}
		}
	}
}