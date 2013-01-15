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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	/// <summary>
	/// Provides services for entering orders into the system, and modifying existing orders.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	[ServiceKnownType(typeof(RegistrationWorklistItemSummary))]
	[ServiceKnownType(typeof(ModalityWorklistItemSummary))]
	public interface IOrderEntryService : IWorkflowService
	{
		/// <summary>
		/// List visits for the specified patient.  Orders can be placed on any visits, including discharged visits.
		/// </summary>
		/// <param name="request"><see cref="ListVisitsForPatientRequest"/></param>
		/// <returns><see cref="ListVisitsForPatientResponse"/></returns>
		[OperationContract]
		ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request);

		/// <summary>
		/// List the active orders for the specified patient.  Active orders are either Scheduled or In-Progress.
		/// </summary>
		/// <param name="request"><see cref="ListOrdersForPatientRequest"/></param>
		/// <returns><see cref="ListOrdersForPatientResponse"/></returns>
		[OperationContract]
		ListOrdersForPatientResponse ListActiveOrdersForPatient(ListOrdersForPatientRequest request);

		/// <summary>
		/// Loads all order entry form data.
		/// </summary>
		/// <param name="request"><see cref="GetOrderEntryFormDataRequest"/></param>
		/// <returns><see cref="GetOrderEntryFormDataResponse"/></returns>
		[OperationContract]
		GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

		/// <summary>
		/// Get order cancel form data.
		/// </summary>
		/// <param name="request"><see cref="GetCancelOrderFormDataRequest"/></param>
		/// <returns><see cref="GetCancelOrderFormDataResponse"/></returns>
		[OperationContract]
		GetCancelOrderFormDataResponse GetCancelOrderFormData(GetCancelOrderFormDataRequest request);

		/// <summary>
		/// Loads order requisition so that the order editing form can be populated. This method will
		/// fail with a RequestValidationException if the order requisition cannot be edited.
		/// </summary>
		/// <param name="request"><see cref="GetOrderRequisitionForEditRequest"/></param>
		/// <returns><see cref="GetOrderRequisitionForEditResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		GetOrderRequisitionForEditResponse GetOrderRequisitionForEdit(GetOrderRequisitionForEditRequest request);

		/// <summary>
		/// Gets the details of a diagnostic service plan.
		/// </summary>
		/// <param name="request"><see cref="LoadDiagnosticServicePlanRequest"/></param>
		/// <returns><see cref="LoadDiagnosticServicePlanRequest"/></returns>
		[OperationContract]
		LoadDiagnosticServicePlanResponse LoadDiagnosticServicePlan(LoadDiagnosticServicePlanRequest request);

		/// <summary>
		/// Gets detailed information about all of the contact points associated with a specified external practitioner.
		/// </summary>
		/// <param name="request"><see cref="GetExternalPractitionerContactPointsRequest"/></param>
		/// <returns><see cref="GetExternalPractitionerContactPointsResponse"/></returns>
		[OperationContract]
		GetExternalPractitionerContactPointsResponse GetExternalPractitionerContactPoints(GetExternalPractitionerContactPointsRequest request);

		/// <summary>
		/// Places a new order based on the specified information.
		/// </summary>
		/// <param name="request"><see cref="PlaceOrderRequest"/></param>
		/// <returns><see cref="PlaceOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

		/// <summary>
		/// Modifies an existing order based on the specified information.
		/// </summary>
		/// <param name="request"><see cref="ModifyOrderRequest"/></param>
		/// <returns><see cref="ModifyOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ModifyOrderResponse ModifyOrder(ModifyOrderRequest request);

		/// <summary>
		/// Cancels an existing order and places a new order as a single transaction.
		/// </summary>
		/// <param name="request"><see cref="ReplaceOrderRequest"/></param>
		/// <returns><see cref="ReplaceOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request);

		/// <summary>
		/// Merge an existing order into another order in a single transaction.
		/// </summary>
		/// <param name="request"><see cref="ReplaceOrderRequest"/></param>
		/// <returns><see cref="ReplaceOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		MergeOrderResponse MergeOrder(MergeOrderRequest request);

		/// <summary>
		/// Un-merge all orders that were merged into the specified order.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UnmergeOrderResponse UnmergeOrder(UnmergeOrderRequest request);


		/// <summary>
		/// Cancel orders with a cancellation reason for a patient
		/// </summary>
		/// <param name="request"><see cref="CancelOrderRequest"/></param>
		/// <returns><see cref="CancelOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		CancelOrderResponse CancelOrder(CancelOrderRequest request);

		/// <summary>
		/// Queries for warnings that user should heed before proceeding to cancel or replace the specified order.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		QueryCancelOrderWarningsResponse QueryCancelOrderWarnings(QueryCancelOrderWarningsRequest request);

		/// <summary>
		/// This method is for testing/demo purposes and is not intended to be called in production.
		/// It shifts the order and associated visit in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// This method does not really belong on this interface but there was no other
		/// convenient place to put it.
		/// </remarks>
		/// <param name="request"><see cref="TimeShiftOrderRequest"/></param>
		/// <returns><see cref="TimeShiftOrderResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		TimeShiftOrderResponse TimeShiftOrder(TimeShiftOrderRequest request);

		/// <summary>
		/// Print downtime forms.
		/// </summary>
		/// <param name="request"><see cref="PrintDowntimeFormsRequest"/></param>
		/// <returns><see cref="PrintDowntimeFormsResponse"/></returns>
		[OperationContract]
		PrintDowntimeFormsResponse PrintDowntimeForms(PrintDowntimeFormsRequest request);
	}
}
