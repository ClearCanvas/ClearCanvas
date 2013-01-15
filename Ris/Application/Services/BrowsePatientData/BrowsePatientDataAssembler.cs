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
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Application.Services.BrowsePatientData
{
	public class BrowsePatientDataAssembler
	{
		public OrderListItem CreateOrderListItem(Order order, IPersistenceContext context)
		{
			var data = new OrderListItem();

			UpdateListItem(data, order, context);
			UpdateListItem(data, order.Visit, context);

			return data;
		}

		public OrderListItem CreateOrderListItem(Procedure rp, IPersistenceContext context)
		{
			var data = new OrderListItem();

			UpdateListItem(data, rp.Order, context);
			UpdateListItem(data, rp.Order.Visit, context);
			UpdateListItem(data, rp, context);

			return data;
		}

		public ReportListItem CreateReportListItem(Report report, Procedure rp, IPersistenceContext context)
		{
			var data = new ReportListItem();

			UpdateListItem(data, rp.Order, context);
			UpdateListItem(data, rp.Order.Visit, context);
			UpdateListItem(data, rp, context);
			UpdateListItem(data, report, context);

			return data;
		}

		public VisitListItem CreateVisitListItem(Visit visit, IPersistenceContext context)
		{
			var data = new VisitListItem();

			UpdateListItem(data, visit, context);

			return data;
		}

		#region Private Helpers

		private static void UpdateListItem(VisitListItem data, Visit visit, IPersistenceContext context)
		{
			var facilityAssembler = new FacilityAssembler();

			data.VisitRef = visit.GetRef();
			data.VisitNumber = new CompositeIdentifierDetail(visit.VisitNumber.Id, EnumUtils.GetEnumValueInfo(visit.VisitNumber.AssigningAuthority));
			data.PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass);
			data.PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType);
			data.AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType);
			data.VisitStatus = EnumUtils.GetEnumValueInfo(visit.Status, context);
			data.AdmitTime = visit.AdmitTime;
			data.DischargeTime = visit.DischargeTime;
			data.VisitFacility = facilityAssembler.CreateFacilitySummary(visit.Facility);
			data.PreadmitNumber = visit.PreadmitNumber;
		}

		private static void UpdateListItem(OrderListItem data, Order order, IPersistenceContext context)
		{
			var practitionerAssembler = new ExternalPractitionerAssembler();
			var dsAssembler = new DiagnosticServiceAssembler();
			var facilityAssembler = new FacilityAssembler();

			data.OrderRef = order.GetRef();
			data.PlacerNumber = order.PlacerNumber;
			data.AccessionNumber = order.AccessionNumber;
			data.DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService);
			data.EnteredTime = order.EnteredTime;
			data.SchedulingRequestTime = order.SchedulingRequestTime;
			data.OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
			data.OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility);
			data.ReasonForStudy = order.ReasonForStudy;
			data.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
			data.CancelReason = order.CancelInfo != null && order.CancelInfo.Reason != null ? EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason) : null;
			data.OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context);
			data.OrderScheduledStartTime = order.ScheduledStartTime;
		}

		private static void UpdateListItem(OrderListItem data, Procedure rp, IPersistenceContext context)
		{
			var rptAssembler = new ProcedureTypeAssembler();
			data.ProcedureRef = rp.GetRef();
			data.ProcedureType = rptAssembler.CreateSummary(rp.Type);
			data.ProcedureScheduledStartTime = rp.ScheduledStartTime;
			data.ProcedureSchedulingCode = EnumUtils.GetEnumValueInfo(rp.SchedulingCode);
			data.ProcedureCheckInTime = rp.ProcedureCheckIn.CheckInTime;
			data.ProcedureCheckOutTime = rp.ProcedureCheckIn.CheckOutTime;
			data.ProcedureStatus = EnumUtils.GetEnumValueInfo(rp.Status, context);
			data.ProcedurePerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility);
			data.ProcedurePortable = rp.Portable;
			data.ProcedureLaterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context);
		}

		private static void UpdateListItem(ReportListItem data, Report report, IPersistenceContext context)
		{
			data.ReportRef = report.GetRef();
			data.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);
		}

		#endregion
	}
}
