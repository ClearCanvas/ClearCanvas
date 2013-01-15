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

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [RisApplicationService]
    [ServiceContract]
    [ServiceKnownType(typeof(ReportingWorklistItemSummary))]
	[ServiceKnownType(typeof(ModalityWorklistItemSummary))] // bug  #4866: need to call this service from Performing FS
	public interface IReportingWorkflowService : IWorklistService<ReportingWorklistItemSummary>, IWorkflowService
    {
        /// <summary>
        /// Indicates if all documentation for the order containing the specified procedure is complete
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetDocumentationStatusResponse GetDocumentationStatus(GetDocumentationStatusRequest request);

        /// <summary>
        /// Start an interpretation step
        /// </summary>
        /// <param name="request"><see cref="StartInterpretationRequest"/></param>
        /// <returns><see cref="StartInterpretationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartInterpretationResponse StartInterpretation(StartInterpretationRequest request);

        /// <summary>
        /// Start a transcription review step
        /// </summary>
        /// <param name="request"><see cref="StartTranscriptionReviewRequest"/></param>
        /// <returns><see cref="StartTranscriptionReviewResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartTranscriptionReviewResponse StartTranscriptionReview(StartTranscriptionReviewRequest request);

        /// <summary>
        /// Complete an interpretation step and create a transcription step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForTranscriptionRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForTranscriptionResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(SupervisorValidationException))]
        CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        /// <summary>
        /// Complete an interpretation step and create a verification step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForVerificationRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(SupervisorValidationException))]
        CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        /// <summary>
        /// Complete an interpretation step and verify it.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationAndVerifyRequest"/></param>
        /// <returns><see cref="CompleteInterpretationAndVerifyResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(SupervisorValidationException))]
        CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        /// <summary>
        /// Cancel a reporting step and create a new interpretation step.
        /// </summary>
        /// <param name="request"><see cref="CancelReportingStepRequest"/></param>
        /// <returns><see cref="CancelReportingStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CancelReportingStepResponse CancelReportingStep(CancelReportingStepRequest request);

        /// <summary>
        /// Cancel a verification step and create a new interpretation step with the same report part.
        /// This is used by the resident to revise the report that is currently waiting to be verified by radiologist
        /// </summary>
        /// <param name="request"><see cref="ReviseResidentReportRequest"/></param>
        /// <returns><see cref="ReviseResidentReportResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ReviseResidentReportResponse ReviseResidentReport(ReviseResidentReportRequest request);

		/// <summary>
		/// Cancel a step and create a new interpretation step with the same report part.
		/// This is used by the radiologist to send back a reviewed report back to the interpreter.
		/// </summary>
		/// <param name="request"><see cref="ReturnToInterpreterRequest"/></param>
		/// <returns><see cref="ReturnToInterpreterResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ReturnToInterpreterResponse ReturnToInterpreter(ReturnToInterpreterRequest request);

        /// <summary>
        /// Start an verification step
        /// </summary>
        /// <param name="request"><see cref="StartVerificationRequest"/></param>
        /// <returns><see cref="StartVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartVerificationResponse StartVerification(StartVerificationRequest request);

        /// <summary>
        /// Complete a verification step
        /// </summary>
        /// <param name="request"><see cref="CompleteVerificationRequest"/></param>
        /// <returns><see cref="CompleteVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request);

        /// <summary>
        /// Start an addendum step
        /// </summary>
        /// <param name="request"><see cref="CreateAddendumRequest"/></param>
        /// <returns><see cref="CreateAddendumResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CreateAddendumResponse CreateAddendum(CreateAddendumRequest request);

        /// <summary>
        /// Cancel a publication step and create a new verification step with the same report part.
        /// This is used by the radiologist to revise the report that is still unpublished.
        /// </summary>
        /// <param name="request"><see cref="ReviseUnpublishedReportRequest"/></param>
        /// <returns><see cref="ReviseUnpublishedReportResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ReviseUnpublishedReportResponse ReviseUnpublishedReport(ReviseUnpublishedReportRequest request);

        /// <summary>
        /// This provide a mean to complete a publication step.  It is meant for testing only. 
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        PublishReportResponse PublishReport(PublishReportRequest request);

        /// <summary>
        /// Load the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="LoadReportForEditRequest"/></param>
        /// <returns><see cref="LoadReportForEditResponse"/></returns>
        [OperationContract]
        LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request);

        /// <summary>
        /// Obtains the set of scheduled interpretations that can optionally be linked to the specified interpretation
        /// so that the report is shared.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetLinkableInterpretationsResponse GetLinkableInterpretations(GetLinkableInterpretationsRequest request);

        /// <summary>
        /// Save the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="SaveReportRequest"/></param>
        /// <returns><see cref="SaveReportResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        SaveReportResponse SaveReport(SaveReportRequest request);

        /// <summary>
        /// Get all the reports of a given patient
        /// </summary>
        /// <param name="request"><see cref="GetPriorsRequest"/></param>
        /// <returns><see cref="GetPriorsResponse"/></returns>
        [OperationContract]
        GetPriorsResponse GetPriors(GetPriorsRequest request);

		//[OperationContract]
		//SendReportToQueueResponse SendReportToQueue(SendReportToQueueRequest request);

        /// <summary>
        /// Reassigning a step to another radiologist.
        /// </summary>
        /// <param name="request"><see cref="ReassignProcedureStepRequest"/></param>
        /// <returns><see cref="ReassignProcedureStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ReassignProcedureStepResponse ReassignProcedureStep(ReassignProcedureStepRequest request);

        /// <summary>
        /// This is basically a hack to allow entry of downtime reports for downtime recovery.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteDowntimeProcedureResponse CompleteDowntimeProcedure(CompleteDowntimeProcedureRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		PrintReportResponse PrintReport(PrintReportRequest request);
    }
}
