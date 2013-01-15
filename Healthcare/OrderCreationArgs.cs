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

namespace ClearCanvas.Healthcare
{
	public class OrderCreationArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public OrderCreationArgs()
		{
		}

		/// <summary>
		/// Constructor that allows procedures to be explicitly specified.
		/// </summary>
		/// <param name="enteredTime"></param>
		/// <param name="enteredBy"></param>
		/// <param name="enteredComment"></param>
		/// <param name="accessionNumber"></param>
		/// <param name="patient"></param>
		/// <param name="visit"></param>
		/// <param name="diagnosticService"></param>
		/// <param name="reasonForStudy"></param>
		/// <param name="priority"></param>
		/// <param name="orderingFacility"></param>
		/// <param name="schedulingRequestTime"></param>
		/// <param name="orderingPractitioner"></param>
		/// <param name="resultRecipients"></param>
		/// <param name="procedures"></param>
		public OrderCreationArgs(DateTime enteredTime, Staff enteredBy, string enteredComment, string accessionNumber,
			Patient patient, Visit visit, DiagnosticService diagnosticService, string reasonForStudy,
			OrderPriority priority, Facility orderingFacility, DateTime? schedulingRequestTime,
			ExternalPractitioner orderingPractitioner, IList<ResultRecipient> resultRecipients, IList<Procedure> procedures)
		{
			EnteredTime = enteredTime;
			EnteredBy = enteredBy;
			EnteredComment = enteredComment;
			AccessionNumber = accessionNumber;
			Patient = patient;
			Visit = visit;
			DiagnosticService = diagnosticService;
			ReasonForStudy = reasonForStudy;
			Priority = priority;
			OrderingFacility = orderingFacility;
			SchedulingRequestTime = schedulingRequestTime;
			OrderingPractitioner = orderingPractitioner;
			ResultRecipients = resultRecipients;
			Procedures = procedures;
		}

		/// <summary>
		/// Constructor that does not specify an explicit list of procedures.
		/// </summary>
		/// <param name="enteredTime"></param>
		/// <param name="enteredBy"></param>
		/// <param name="enteredComment"></param>
		/// <param name="accessionNumber"></param>
		/// <param name="patient"></param>
		/// <param name="visit"></param>
		/// <param name="diagnosticService"></param>
		/// <param name="reasonForStudy"></param>
		/// <param name="priority"></param>
		/// <param name="orderingFacility"></param>
		/// <param name="performingFacility"></param>
		/// <param name="schedulingRequestTime"></param>
		/// <param name="orderingPractitioner"></param>
		/// <param name="resultRecipients"></param>
		public OrderCreationArgs(DateTime enteredTime, Staff enteredBy, string enteredComment, string accessionNumber,
			Patient patient, Visit visit, DiagnosticService diagnosticService, string reasonForStudy, OrderPriority priority,
			Facility orderingFacility, Facility performingFacility, DateTime? schedulingRequestTime, ExternalPractitioner orderingPractitioner,
			IList<ResultRecipient> resultRecipients)
		{
			EnteredTime = enteredTime;
			EnteredBy = enteredBy;
			EnteredComment = enteredComment;
			AccessionNumber = accessionNumber;
			Patient = patient;
			Visit = visit;
			DiagnosticService = diagnosticService;
			ReasonForStudy = reasonForStudy;
			Priority = priority;
			OrderingFacility = orderingFacility;
			PerformingFacility = performingFacility;
			SchedulingRequestTime = schedulingRequestTime;
			OrderingPractitioner = orderingPractitioner;
			ResultRecipients = resultRecipients;
		}

		/// <summary>
		/// Time at which the order was entered. Required.
		/// </summary>
		public DateTime EnteredTime;

		/// <summary>
		/// Staff who entered the order.  Optional - may be null if entered via external system (HL7).
		/// </summary>
		public Staff EnteredBy;

		/// <summary>
		/// Free text field useful for capturing extra information sent by external system (HL7), such as who placed the order.
		/// </summary>
		public string EnteredComment;

		/// <summary>
		/// Accession number to assign to the order. Required.
		/// </summary>
		public string AccessionNumber;

		/// <summary>
		/// Patient for whom the order is being placed. Required.
		/// </summary>
		public Patient Patient;

		/// <summary>
		/// Visit that the order is associated with. Required.
		/// </summary>
		public Visit Visit;

		/// <summary>
		/// Diagnostic service being ordered. Required.
		/// </summary>
		public DiagnosticService DiagnosticService;

		/// <summary>
		/// Reason that the order is being placed. Required.
		/// </summary>
		public string ReasonForStudy;

		/// <summary>
		/// Order priority. Required.
		/// </summary>
		public OrderPriority Priority;

		/// <summary>
		/// Facility that is placing the order. Required.
		/// </summary>
		public Facility OrderingFacility;

		/// <summary>
		/// Facility at which procedures are to be performed - if null, the <see cref="OrderingFacility"/> will be used.
		/// </summary>
		public Facility PerformingFacility;

		/// <summary>
		/// Scheduling request time. Optional.
		/// </summary>
		public DateTime? SchedulingRequestTime;

		/// <summary>
		/// Ordering practitioner. Required.
		/// </summary>
		public ExternalPractitioner OrderingPractitioner;

		/// <summary>
		/// Set of procedures being order - if null or empty, the procedures will be inferred from the <see cref="DiagnosticService"/>.
		/// </summary>
		public IList<Procedure> Procedures;

		/// <summary>
		/// List of recipients to receive order results. Optional.
		/// </summary>
		public IList<ResultRecipient> ResultRecipients;

		/// <summary>
		/// Placer order number.  Optional.
		/// </summary>
		public string PlacerOrderNumber;
	}
}
