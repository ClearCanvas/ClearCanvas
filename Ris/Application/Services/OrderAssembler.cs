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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services
{
	public class OrderAssembler
	{
		public class CreateOrderDetailOptions
		{
			public static CreateOrderDetailOptions GetVerboseOptions()
			{
				return new CreateOrderDetailOptions
					{
						IncludeVisit = true,
						IncludeProcedures = true,
						IncludeNotes = true,
						IncludeVirtualNotes = false, // even verbose does not include virtual notes by default
						NoteCategoriesFilter = null,
						IncludeAttachments = true,
						IncludeResultRecipients = true,
						IncludeExtendedProperties = true,
					};
			}

			public bool IncludeVisit { get; set; }
			public bool IncludeProcedures { get; set; }
			public bool IncludeNotes { get; set; }
			public bool IncludeVirtualNotes { get; set; }
			public IList<string> NoteCategoriesFilter { get; set; }
			public bool IncludeAttachments { get; set; }
			public bool IncludeResultRecipients { get; set; }
			public bool IncludeExtendedProperties { get; set; }
		}

		/// <summary>
		/// Creates order detail document including only specified parts.
		/// </summary>
		public OrderDetail CreateOrderDetail(Order order, CreateOrderDetailOptions options, IPersistenceContext context)
		{
			var orderDetail = new OrderDetail();

			var practitionerAssembler = new ExternalPractitionerAssembler();
			var facilityAssembler = new FacilityAssembler();
			var dsAssembler = new DiagnosticServiceAssembler();
			var procedureAssembler = new ProcedureAssembler();
			var staffAssembler = new StaffAssembler();

			orderDetail.OrderRef = order.GetRef();
			orderDetail.PatientRef = order.Patient.GetRef();

			if (options.IncludeVisit)
			{
				var visitAssembler = new VisitAssembler();
				orderDetail.Visit = visitAssembler.CreateVisitDetail(order.Visit, context);
			}

			orderDetail.PlacerNumber = order.PlacerNumber;
			orderDetail.AccessionNumber = order.AccessionNumber;
			orderDetail.DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService);

			orderDetail.EnteredTime = order.EnteredTime;
			orderDetail.EnteredBy = order.EnteredBy == null ? null :
				staffAssembler.CreateStaffSummary(order.EnteredBy, context);
			orderDetail.EnteredComment = order.EnteredComment;

			orderDetail.SchedulingRequestTime = order.SchedulingRequestTime;
			orderDetail.OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
			orderDetail.OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility);
			orderDetail.ReasonForStudy = order.ReasonForStudy;
			orderDetail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);

			if (order.CancelInfo != null)
			{
				orderDetail.CancelReason = order.CancelInfo.Reason == null ? null : EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason);
				orderDetail.CancelledBy = order.CancelInfo.CancelledBy == null ? null :
					staffAssembler.CreateStaffSummary(order.CancelInfo.CancelledBy, context);
				orderDetail.CancelComment = order.CancelInfo.Comment;
			}

			if (options.IncludeProcedures)
			{
				orderDetail.Procedures = CollectionUtils.Map(order.Procedures,
					(Procedure p) => procedureAssembler.CreateProcedureDetail(p, context));
			}

			if (options.IncludeNotes)
			{
				var orderNoteAssembler = new OrderNoteAssembler();
				var orderNotes = new List<OrderNote>(OrderNote.GetNotesForOrder(order, options.NoteCategoriesFilter, options.IncludeVirtualNotes));

				// sort notes by post-time (guaranteed non-null because only "posted" notes are in this collection)
				orderNotes.Sort((x, y) => x.PostTime.Value.CompareTo(y.PostTime.Value));

				// Put most recent notes first
				orderNotes.Reverse();

				orderDetail.Notes = CollectionUtils.Map<OrderNote, OrderNoteSummary>(
					orderNotes,
					note => orderNoteAssembler.CreateOrderNoteSummary(note, context));
			}

			if (options.IncludeAttachments)
			{
				var orderAttachmentAssembler = new OrderAttachmentAssembler();
				var attachments = new List<OrderAttachment>(order.Attachments);

				orderDetail.Attachments = CollectionUtils.Map<OrderAttachment, AttachmentSummary>(
					attachments,
					a => orderAttachmentAssembler.CreateOrderAttachmentSummary(a, context));
			}

			if (options.IncludeResultRecipients)
			{
				var resultRecipientAssembler = new ResultRecipientAssembler();
				var resultRecipients = new List<ResultRecipient>(order.ResultRecipients);

				orderDetail.ResultRecipients = CollectionUtils.Map<ResultRecipient, ResultRecipientDetail>(
					resultRecipients,
					r => resultRecipientAssembler.CreateResultRecipientDetail(r, context));
			}

			if (options.IncludeExtendedProperties)
			{
				orderDetail.ExtendedProperties = ExtendedPropertyUtils.Copy(order.ExtendedProperties);
			}

			return orderDetail;
		}

		public OrderSummary CreateOrderSummary(Order order, IPersistenceContext context)
		{
			var practitionerAssembler = new ExternalPractitionerAssembler();
			return new OrderSummary
				{
					OrderRef = order.GetRef(),
					AccessionNumber = order.AccessionNumber,
					DiagnosticServiceName = order.DiagnosticService.Name,
					EnteredTime = order.EnteredTime,
					SchedulingRequestTime = order.SchedulingRequestTime,
					OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context),
					OrderingFacility = order.OrderingFacility.Name,
					ReasonForStudy = order.ReasonForStudy,
					OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context),
					OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context)
				};
		}
	}
}
