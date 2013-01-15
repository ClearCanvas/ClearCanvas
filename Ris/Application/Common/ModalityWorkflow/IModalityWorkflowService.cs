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

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [RisApplicationService]
    [ServiceContract]
	[ServiceKnownType(typeof(ModalityWorklistItemSummary))]
	public interface IModalityWorkflowService : IWorklistService<ModalityWorklistItemSummary>, IWorkflowService
    {
        /// <summary>
        /// Returns a summary of the procedure plan for a specified order.
        /// </summary>
        /// <param name="request"><see cref="GetProcedurePlanRequest"/></param>
        /// <returns><see cref="GetProcedurePlanResponse"/></returns>
        [OperationContract]
		GetProcedurePlanResponse GetProcedurePlan(GetProcedurePlanRequest request);

        /// <summary>
        /// Returns a list of all modality performed procedure steps for a particular order.
        /// </summary>
        /// <param name="request"><see cref="ListPerformedProcedureStepsRequest"/></param>
        /// <returns><see cref="ListPerformedProcedureStepsResponse"/></returns>
        [OperationContract]
        ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request);

        /// <summary>
        /// Starts a specified set of modality procedure steps with a single modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="StartModalityProcedureStepsRequest"/></param>
        /// <returns><see cref="StartModalityProcedureStepsResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request);

        /// <summary>
        /// Discontinues a set of specified modality procedure steps.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityProcedureStepsResponse"/></param>
        /// <returns><see cref="DiscontinueModalityProcedureStepsRequest"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request);

        /// <summary>
        /// Completes a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="CompleteModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="CompleteModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		CompleteModalityPerformedProcedureStepResponse CompleteModalityPerformedProcedureStep(CompleteModalityPerformedProcedureStepRequest request);

        /// <summary>
        /// Discontinues a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="DiscontinueModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request);

		/// <summary>
		/// Load performing documentation data for an order.
		/// </summary>
		/// <param name="request"><see cref="LoadOrderDocumentationDataRequest"/></param>
		/// <returns><see cref="LoadOrderDocumentationDataResponse"/></returns>
		[OperationContract]
		LoadOrderDocumentationDataResponse LoadOrderDocumentationData(LoadOrderDocumentationDataRequest request);

		/// <summary>
		/// Save performing documentation data for an order.
		/// </summary>
		/// <param name="request"><see cref="SaveOrderDocumentationDataRequest"/></param>
		/// <returns><see cref="SaveOrderDocumentationDataResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		SaveOrderDocumentationDataResponse SaveOrderDocumentationData(SaveOrderDocumentationDataRequest request);

		/// <summary>
		/// Verify if an order has documentation to complete.
		/// </summary>
		/// <param name="request"><see cref="CanCompleteOrderDocumentationRequest"/></param>
		/// <returns><see cref="CanCompleteOrderDocumentationResponse"/></returns>
		[OperationContract]
		CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request);

		/// <summary>
		/// Complete documentation for an order.
		/// </summary>
		/// <param name="request"><see cref="CompleteOrderDocumentationRequest"/></param>
		/// <returns><see cref="CompleteOrderDocumentationResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request);
	}
}
