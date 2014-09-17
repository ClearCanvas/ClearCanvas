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
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Printing;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Printing;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
	[ServiceImplementsContract(typeof(IReportingWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
	{
		#region IReportingWorkflowService Members

		[ReadOperation]
		public GetDocumentationStatusResponse GetDocumentationStatus(GetDocumentationStatusRequest request)
		{
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			var message = "";
			var isIncomplete = false;

			if (!procedure.Order.AreAllActiveProceduresPerformed)
			{
				isIncomplete = true;
				message = SR.MessageNotAllProceduresPerformed;
			}
			else if (!procedure.IsDocumented)
			{
				isIncomplete = true;
				message = SR.MessageDocumentationIncomplete;
			}

			return new GetDocumentationStatusResponse(isIncomplete, message);
		}

		[ReadOperation]
		[AuditRecorder(typeof(WorkflowServiceRecorder.SearchWorklists))]
		public TextQueryResponse<ReportingWorklistItemSummary> SearchWorklists(WorklistItemTextQueryRequest request)
		{
			var procedureStepClass = request.ProcedureStepClassName == null ? null
				: ProcedureStep.GetSubClass(request.ProcedureStepClassName, PersistenceContext);

			// decide which broker/projection to use for searching
			var isReporting = typeof (ReportingProcedureStep).IsAssignableFrom(procedureStepClass);
			var broker = isReporting ?
				(IWorklistItemBroker)PersistenceContext.GetBroker<IReportingWorklistItemBroker>()
				: PersistenceContext.GetBroker<IProtocolWorklistItemBroker>();

			var projection = isReporting ? 
				WorklistItemProjection.ReportingWorklistSearch :
				WorklistItemProjection.ProtocolWorklistSearch;
			

			var assembler = new ReportingWorkflowAssembler();
			return SearchHelper<ReportingWorklistItem, ReportingWorklistItemSummary>(
				request,
				broker,
				projection,
				item => assembler.CreateWorklistItemSummary(item, PersistenceContext));
		}

		[ReadOperation]
		[ResponseCaching("GetQueryWorklistCacheDirective")]
		public QueryWorklistResponse<ReportingWorklistItemSummary> QueryWorklist(QueryWorklistRequest request)
		{
			var assembler = new ReportingWorkflowAssembler();

			return QueryWorklistHelper<ReportingWorklistItem, ReportingWorklistItemSummary>(
				request,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[OperationEnablement("CanStartInterpretation")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
		public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
		{
			var interpretation = this.PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);
			var staffAssignedBeforeStart = interpretation.AssignedStaff;

			var linkedInterpretations = new List<InterpretationStep>();
			if (request.LinkedInterpretationStepRefs != null && request.LinkedInterpretationStepRefs.Count > 0)
			{
				linkedInterpretations = CollectionUtils.Map<EntityRef, InterpretationStep>(
					request.LinkedInterpretationStepRefs,
					stepRef => this.PersistenceContext.Load<InterpretationStep>(stepRef));
			}

			var op = new Operations.StartInterpretation();
			op.Execute(interpretation, this.CurrentUserStaff, linkedInterpretations, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new StartInterpretationResponse(interpretation.GetRef(),
				staffAssignedBeforeStart == null ? null : interpretation.AssignedStaff.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanStartTranscriptionReview")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
		public StartTranscriptionReviewResponse StartTranscriptionReview(StartTranscriptionReviewRequest request)
		{
			var transcriptionReviewStep = this.PersistenceContext.Load<TranscriptionReviewStep>(request.TranscriptionReviewStepRef, EntityLoadFlags.CheckVersion);

			var op = new Operations.StartTranscriptionReview();
			op.Execute(transcriptionReviewStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();
			return new StartTranscriptionReviewResponse(transcriptionReviewStep.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteInterpretationForTranscription")]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.CompleteInterpretation))]
		public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
		{
			var interpretation = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

			SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);
			UpdatePriority(interpretation, request.Priority);

			ValidateReportTextExists(interpretation);

			var op = new Operations.CompleteInterpretationForTranscription();
			var nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new CompleteInterpretationForTranscriptionResponse
					{
						InterpretationStepRef = interpretation.GetRef(),
						TranscriptionStepRef = nextStep.GetRef()
					};
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteInterpretationForVerification")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.SubmitForReview)]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.CompleteInterpretation))]
		public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
		{
			var interpretation = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

			SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);
			UpdatePriority(interpretation, request.Priority);

			ValidateReportTextExists(interpretation);

			var op = new Operations.CompleteInterpretationForVerification();
			var nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new CompleteInterpretationForVerificationResponse
					{
						InterpretationStepRef = interpretation.GetRef(),
						VerificationStepRef = nextStep.GetRef()
					};
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteInterpretationAndVerify")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.CompleteVerification))]
		public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
		{
			var interpretation = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

			SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);
			UpdatePriority(interpretation, request.Priority);

			ValidateReportTextExists(interpretation);

			var op = new Operations.CompleteInterpretationAndVerify();
			var nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new CompleteInterpretationAndVerifyResponse
					{
						InterpretationStepRef = interpretation.GetRef(),
						PublicationStepRef = nextStep.GetRef()
					};
		}

		[UpdateOperation]
		[OperationEnablement("CanCancelReportingStep")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Cancel)]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.Discard))]
		public CancelReportingStepResponse CancelReportingStep(CancelReportingStepRequest request)
		{
			var step = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var reassignStaff = request.ReassignedToStaff != null
				? this.PersistenceContext.Load<Staff>(request.ReassignedToStaff, EntityLoadFlags.CheckVersion)
				: null;

			// demand authority token if trying to cancel a step that is assigned to someone else
			if (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff))
			{
				var permission = new PrincipalPermission(null, AuthorityTokens.Workflow.Report.Cancel);
				permission.Demand();
			}

			var op = new Operations.CancelReportingStep();
			var scheduledInterpretations = op.Execute(step, this.CurrentUserStaff, reassignStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();

			return new CancelReportingStepResponse(step.GetRef(),
				CollectionUtils.Map<InterpretationStep, EntityRef>(scheduledInterpretations, s => s.GetRef()));
		}

		[UpdateOperation]
		[OperationEnablement("CanReviseResidentReport")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.SubmitForReview)]
		public ReviseResidentReportResponse ReviseResidentReport(ReviseResidentReportRequest request)
		{
			var step = this.PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

			var op = new Operations.ReviseResidentReport();
			var interpretation = op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new ReviseResidentReportResponse(GetWorklistItemSummary(interpretation));
		}

		[UpdateOperation]
		[OperationEnablement("CanReturnToInterpreter")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
		public ReturnToInterpreterResponse ReturnToInterpreter(ReturnToInterpreterRequest request)
		{
			var step = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(step, request.SupervisorRef);

			SaveReportHelper(request.ReportPartExtendedProperties, step, supervisor, true);
			UpdatePriority(step, request.Priority);

			ValidateReportTextExists(step);

			var op = new Operations.ReturnToInterpreter();
			var newStep = op.Execute(step, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new ReturnToInterpreterResponse(newStep.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanStartVerification")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
		public StartVerificationResponse StartVerification(StartVerificationRequest request)
		{
			var verification = this.PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

			var op = new Operations.StartVerification();
			op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();
			return new StartVerificationResponse(verification.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteVerification")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.CompleteVerification))]
		public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
		{
			var verification = this.PersistenceContext.Load<VerificationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(verification, request.SupervisorRef);

			SaveReportHelper(request.ReportPartExtendedProperties, verification, supervisor, true);
			UpdatePriority(verification, request.Priority);

			ValidateReportTextExists(verification);

			var op = new Operations.CompleteVerification();
			var publication = op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();

			return new CompleteVerificationResponse
					{
						VerificationStepRef = verification.GetRef(),
						PublicationStepRef = publication.GetRef()
					};
		}

		[UpdateOperation]
		[OperationEnablement("CanCreateAddendum")]
		public CreateAddendumResponse CreateAddendum(CreateAddendumRequest request)
		{
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			var op = new Operations.CreateAddendum();
			var interpretation = op.Execute(procedure, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();

			return new CreateAddendumResponse
					{
						ReportingWorklistItem = GetWorklistItemSummary(interpretation)
					};
		}

		[UpdateOperation]
		[OperationEnablement("CanReviseUnpublishedReport")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.Revise))]
		public ReviseUnpublishedReportResponse ReviseUnpublishedReport(ReviseUnpublishedReportRequest request)
		{
			var publication = this.PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

			var op = new Operations.ReviseUnpublishedReport();
			var verification = op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			this.PersistenceContext.SynchState();

			return new ReviseUnpublishedReportResponse(GetWorklistItemSummary(verification));
		}

		[UpdateOperation]
		[OperationEnablement("CanPublishReport")]
		public PublishReportResponse PublishReport(PublishReportRequest request)
		{
			var publication = this.PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

			var op = new Operations.PublishReport();
			op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

			LogicalHL7Event.ReportPublished.EnqueueEvents(publication.Report);

			this.PersistenceContext.SynchState();
			return new PublishReportResponse(publication.GetRef());
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
		public LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request)
		{
			var step = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);

			var reportAssembler = new ReportAssembler();
			var reportDetail = reportAssembler.CreateReportDetail(step.ReportPart.Report, false, this.PersistenceContext);

			var orderAssembler = new OrderAssembler();
			var orderDetailOptions = new OrderAssembler.CreateOrderDetailOptions {IncludeExtendedProperties = true};
			var orderDetail = orderAssembler.CreateOrderDetail(step.Procedure.Order, orderDetailOptions, this.PersistenceContext);

			return new LoadReportForEditResponse(
				reportDetail,
				step.ReportPart.Index,
				orderDetail,
				EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext));
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveReport")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
		public SaveReportResponse SaveReport(SaveReportRequest request)
		{
			var step = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var supervisor = ResolveSupervisor(step, request.SupervisorRef);

			// saving a draft does not require supervisor validation
			SaveReportHelper(request.ReportPartExtendedProperties, step, supervisor, false);
			UpdatePriority(step, request.Priority);

			this.PersistenceContext.SynchState();

			return new SaveReportResponse(step.GetRef());
		}

		[ReadOperation]
		public GetPriorsResponse GetPriors(GetPriorsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			if (request.OrderRef == null && request.ReportRef == null)
				throw new ArgumentException("Either OrderRef or ReportRef must be non-null");

			var priorReports = new HashedSet<Prior>();
			var broker = this.PersistenceContext.GetBroker<IPriorReportBroker>();

			// if an order was supplied, find relevant priors for the order
			if (request.OrderRef != null)
			{
				var order = this.PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.Proxy);
				priorReports.AddAll(broker.GetPriors(order, request.RelevantOnly));
			}
			// if a report was supplied, find relevent priors for the report
			else if (request.ReportRef != null)
			{
				var report = this.PersistenceContext.Load<Report>(request.ReportRef, EntityLoadFlags.Proxy);
				priorReports.AddAll(broker.GetPriors(report, request.RelevantOnly));
			}

			// assemble results
			var procedureTypeAssembler = new ProcedureTypeAssembler();
			var diagnosticServiceAssembler = new DiagnosticServiceAssembler();

			// Note: we use the ProcedureCheckin.CheckOutTime as the PerformedDate
			// because it is the closest to the end of modality procedure step completion time.
			// However, if we change the definition of CheckOutTime in the future, this won't be accurate
			var priorSummaries = CollectionUtils.Map(priorReports,
				(Prior prior) => new PriorProcedureSummary(
							prior.Order.GetRef(),
							prior.Procedure.GetRef(),
				         	prior.Report.GetRef(),
							prior.Order.AccessionNumber,
							diagnosticServiceAssembler.CreateSummary(prior.Order.DiagnosticService),
							procedureTypeAssembler.CreateSummary(prior.ProcedureType),
							prior.Procedure.Portable,
							EnumUtils.GetEnumValueInfo(prior.Procedure.Laterality, PersistenceContext),
							EnumUtils.GetEnumValueInfo(prior.Report.Status, PersistenceContext),
							prior.Procedure.ProcedureCheckIn.CheckOutTime));

			return new GetPriorsResponse(priorSummaries);
		}

		[ReadOperation]
		public GetLinkableInterpretationsResponse GetLinkableInterpretations(GetLinkableInterpretationsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.InterpretationStepRef, "InterpretationStepRef");

			var step = this.PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.Proxy);

			var broker = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>();
			var candidateSteps = broker.GetLinkedInterpretationCandidates(step, this.CurrentUserStaff);

			// if any candidate steps were found, need to convert them to worklist items
			IList<ReportingWorklistItem> worklistItems;
			if (candidateSteps.Count > 0)
			{
				// because CLR does not support List co-variance, need to map to a list of the more general type (this seems silly!)
				var reportingSteps = CollectionUtils.Map<InterpretationStep, ReportingProcedureStep>(candidateSteps, s => s);
				worklistItems = broker.GetWorklistItems(reportingSteps, WorklistItemField.ProcedureStepScheduledStartTime);
			}
			else
			{
				worklistItems = new List<ReportingWorklistItem>();
			}

			var assembler = new ReportingWorkflowAssembler();
			return new GetLinkableInterpretationsResponse(
				CollectionUtils.Map<ReportingWorklistItem, ReportingWorklistItemSummary>(
					worklistItems,
					item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext)));
		}

		//[UpdateOperation]
		//[OperationEnablement("CanSendReportToQueue")]
		//[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.SendToFaxQueue)]
		//public SendReportToQueueResponse SendReportToQueue(SendReportToQueueRequest request)
		//{
		//    var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

		//    foreach (var detail in request.Recipients)
		//    {
		//        var item = MailFaxWorkQueueItem.Create(
		//            procedure.Order.AccessionNumber,
		//            procedure.ActiveReport.GetRef(),
		//            detail.PractitionerRef,
		//            detail.ContactPointRef);

		//        this.PersistenceContext.Lock(item, DirtyState.New);
		//    }

		//    return new SendReportToQueueResponse();
		//}

		[UpdateOperation]
		[OperationEnablement("CanReassignProcedureStep")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Reassign)]
		public ReassignProcedureStepResponse ReassignProcedureStep(ReassignProcedureStepRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureStepRef, "ProcedureStepRef");
			Platform.CheckMemberIsSet(request.ReassignedRadiologistRef, "ReassignedRadiologistRef");

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef, EntityLoadFlags.Proxy);

			// bug #6418 - this operation doesn't apply to transcription steps, because it doesn't make any sense to assign
			// a transcription step to a radiologist


			var newStaff = this.PersistenceContext.Load<Staff>(request.ReassignedRadiologistRef, EntityLoadFlags.Proxy);

			var newStep = procedureStep.Reassign(newStaff);

			// Bug: #6342 - ensure that the reassigned step does not get lost because it is missing a scheduled start time.
			if(!newStep.Scheduling.StartTime.HasValue)
				newStep.Schedule(Platform.Time);

			this.PersistenceContext.SynchState();

			return new ReassignProcedureStepResponse(newStep.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteDowntimeProcedure")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
		public CompleteDowntimeProcedureResponse CompleteDowntimeProcedure(CompleteDowntimeProcedureRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureRef, "ProcedureRef");

			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			if (request.ReportProvided)
			{
				Platform.CheckMemberIsSet(request.InterpreterRef, "InterpreterRef");

				var interpreter = this.PersistenceContext.Load<Staff>(request.InterpreterRef);
				var transcriptionist = request.TranscriptionistRef == null ? null : this.PersistenceContext.Load<Staff>(request.TranscriptionistRef);

				// find the relevant interpretation step for this procedure
				var interpStep = procedure.
					GetProcedureStep(ps => ps.Is<InterpretationStep>() && ps.State == ActivityStatus.SC).
					As<InterpretationStep>();

				// ideally this should not happen, but what do we do if it does?
				if (interpStep == null)
					throw new RequestValidationException(SR.InvalidRequest_ReportCannotBeSubmittedForProcedure);

				// start interpretation, using specified interpreter
				// the report will end up in their drafts folder
				var startOp = new Operations.StartInterpretation();
				startOp.Execute(interpStep, interpreter, new List<InterpretationStep>(), new PersistentWorkflow(this.PersistenceContext));

				// save the report data
				SaveReportHelper(request.ReportPartExtendedProperties, interpStep, null, false);

				ValidateReportTextExists(interpStep);

				// set the transcriptionist if known
				interpStep.ReportPart.Transcriber = transcriptionist;
			}

			// flip the downtime mode switch
			procedure.DowntimeRecoveryMode = false;

			return new CompleteDowntimeProcedureResponse();
		}

		[ReadOperation]
		[AuditRecorder(typeof(ReportingWorkflowServiceRecorder.PrintReport))]
		public PrintReportResponse PrintReport(PrintReportRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ReportRef, "ReportRef");

			var report = PersistenceContext.Load<Report>(request.ReportRef);

			var printModel = request.RecipientContactPointRef != null ?
				 new ReportPageModel(report, PersistenceContext.Load<ExternalPractitionerContactPoint>(request.RecipientContactPointRef))
				: new ReportPageModel(report);

			using(var printResult = PrintJob.Run(printModel))
			{
				var contents = File.ReadAllBytes(printResult.OutputFilePath);
				return new PrintReportResponse(contents);
			}
		}

		#endregion

		#region OperationEnablement Helpers

		public bool CanStartInterpretation(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
				return false;
			return CanExecuteOperation(new Operations.StartInterpretation(), itemKey);
		}

		public bool CanStartTranscriptionReview(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
				return false;
			return CanExecuteOperation(new Operations.StartTranscriptionReview(), itemKey);
		}

		public bool CanCompleteInterpretationForTranscription(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
				return false;
			return CanExecuteOperation(new Operations.CompleteInterpretationForTranscription(), itemKey);
		}

		public bool CanCompleteInterpretationForVerification(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview))
				return false;

			return CanExecuteOperation(new Operations.CompleteInterpretationForVerification(), itemKey);
		}

		public bool CanCompleteInterpretationAndVerify(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
				return false;

			return CanExecuteOperation(new Operations.CompleteInterpretationAndVerify(), itemKey);
		}

		public bool CanCancelReportingStep(ReportingWorklistItemKey itemKey)
		{
			// if there is no proc step ref, operation is not available
			if (itemKey.ProcedureStepRef == null)
				return false;

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

			var isAssignedToMe = procedureStep.AssignedStaff != null && Equals(procedureStep.AssignedStaff, this.CurrentUserStaff);
			if (isAssignedToMe)
			{
				// Report is assigned to current user, allow cancel only if user has Create or Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create) &&
					!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Cancel))
					return false;
			}
			else
			{
				// Report not assigned to current user, allow cancel only if user has Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Cancel))
					return false;
			}

			return CanExecuteOperation(new Operations.CancelReportingStep(), itemKey);
		}

		public bool CanReviseResidentReport(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview))
				return false;

			return CanExecuteOperation(new Operations.ReviseResidentReport(), itemKey);
		}

		public bool CanReturnToInterpreter(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
				return false;

			return CanExecuteOperation(new Operations.ReturnToInterpreter(), itemKey);
		}

		public bool CanStartVerification(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
				return false;

			// If the submit for review token is present, do not enable verification, defer to 
			// revise report.  This ensures items submitted with or without a supervisor have a
			// consistent set of operations.
			return CanExecuteOperation(new Operations.StartVerification(), itemKey, true);
		}

		public bool CanCompleteVerification(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
				return false;

			// If the submit for review token is present, do not enable verification, defer to 
			// revise report.  This ensures items submitted with or without a supervisor have a
			// consistent set of operations.
			return CanExecuteOperation(new Operations.CompleteVerification(), itemKey, true);
		}

		public bool CanCreateAddendum(ReportingWorklistItemKey itemKey)
		{
			// special case: procedure step not known, but procedure is
			if (itemKey.ProcedureRef != null)
			{
				var procedure = this.PersistenceContext.Load<Procedure>(itemKey.ProcedureRef);
				return (new Operations.CreateAddendum()).CanExecute(procedure, CurrentUserStaff);
			}
			return false;
		}

		public bool CanReviseUnpublishedReport(ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
				return false;

			return CanExecuteOperation(new Operations.ReviseUnpublishedReport(), itemKey);
		}

		public bool CanPublishReport(ReportingWorklistItemKey itemKey)
		{
#if DEBUG
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.TestPublishReport))
				return false;

			return CanExecuteOperation(new Operations.PublishReport(), itemKey);
#else
			return false;
#endif
		}

		public bool CanSaveReport(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new Operations.SaveReport(), itemKey);
		}

		public bool CanReassignProcedureStep(ReportingWorklistItemKey itemKey)
		{
			if (itemKey.ProcedureStepRef == null)
				return false;

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

			// bug #6418 - this operation doesn't apply to transcription steps, because it doesn't make any sense to assign
			// a transcription step to a radiologist
			if (procedureStep.Is<TranscriptionStep>())
				return false;

			if (procedureStep.Is<ReportingProcedureStep>())
				return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Reassign)
					   && !(procedureStep.Is<PublicationStep>());

			return false;
		}

		public bool CanSendReportToQueue(ReportingWorklistItemKey itemKey)
		{
			// does the item have a procedure ref, or is it just a patient?
			if (itemKey.ProcedureRef == null)
				return false;

			// does the procedure have an active report
			var procedure = this.PersistenceContext.Load<Procedure>(itemKey.ProcedureRef);
			if (procedure.ActiveReport == null)
				return false;

			return true;
		}

		public bool CanCompleteDowntimeProcedure(ReportingWorklistItemKey itemKey)
		{
			// does the item have a procedure ref, or is it just a patient?
			if (itemKey.ProcedureRef == null)
				return false;

			var procedure = this.PersistenceContext.Load<Procedure>(itemKey.ProcedureRef);

			// is the procedure a downtime proc, and is it performed and documented??
			return procedure.DowntimeRecoveryMode && procedure.IsPerformed && procedure.IsDocumented;
		}

		private bool CanExecuteOperation(Operations.ReportingOperation op, ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(op, itemKey, false);
		}

		private bool CanExecuteOperation(Operations.ReportingOperation op, ReportingWorklistItemKey itemKey, bool disableIfSubmitForReview)
		{
			// if there is no proc step ref, operation is not available
			if (itemKey.ProcedureStepRef == null)
				return false;

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

			// for now, all of these operations assume they are operating on a ReportingProcedureStep
			// this may need to change in future
			if (!procedureStep.Is<ReportingProcedureStep>())
				return false;

			// Special Case:
			// If the user has the SubmitForReview token and the step is unassigned, disable the operation
			if (disableIfSubmitForReview
				&& procedureStep.AssignedStaff == null
				&& Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview))
			{
				return false;
			}

			return op.CanExecute(procedureStep.As<ReportingProcedureStep>(), this.CurrentUserStaff);
		}

		#endregion

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as WorklistItemSummaryBase; // bug #4866: changed this to base class, so that it can be used by other folder systems
			return summary == null ? null : new ReportingWorklistItemKey(summary.ProcedureStepRef, summary.ProcedureRef);
		}

		/// <summary>
		/// Get the supervisor, using the new supervisor if supplied, otherwise using an existing supervisor if found.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="newSupervisorRef"></param>
		/// <returns></returns>
		private Staff ResolveSupervisor(ReportingProcedureStep step, EntityRef newSupervisorRef)
		{
			var supervisor = newSupervisorRef == null ? null : this.PersistenceContext.Load<Staff>(newSupervisorRef, EntityLoadFlags.Proxy);

			if (supervisor == null && step.ReportPart != null)
				supervisor = step.ReportPart.Supervisor;

			return supervisor;
		}

		/// <summary>
		/// Saves the report, and validates that a supervisor is present if the current user does not have 'unsupervised reporting' permissions.
		/// </summary>
		/// <param name="reportPartExtendedProperties"></param>
		/// <param name="step"></param>
		/// <param name="supervisor"></param>
		/// <param name="supervisorValidationRequired"></param>
		private void SaveReportHelper(Dictionary<string, string> reportPartExtendedProperties, ReportingProcedureStep step, Staff supervisor, bool supervisorValidationRequired)
		{
			if (supervisorValidationRequired
				&& Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview)
				&& Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.OmitSupervisor) == false
				&& supervisor == null)
			{
				throw new SupervisorValidationException();
			}

			var saveReportOp = new Operations.SaveReport();
			saveReportOp.Execute(step, reportPartExtendedProperties, supervisor);
		}

		private void UpdatePriority(ReportingProcedureStep step, EnumValueInfo priority)
		{
			if (priority == null)
				return;

			// update the priority of the associated order
			step.Procedure.Order.Priority = EnumUtils.GetEnumValue<OrderPriority>(priority);
		}

		private void ValidateReportTextExists(ReportingProcedureStep step)
		{
			string content;
			if (step.ReportPart == null || step.ReportPart.ExtendedProperties == null
				|| !step.ReportPart.ExtendedProperties.TryGetValue(ReportPartDetail.ReportContentKey, out content)
				|| string.IsNullOrEmpty(content))
			{
				throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);
			}
		}

		private ReportingWorklistItemSummary GetWorklistItemSummary(ReportingProcedureStep reportingProcedureStep)
		{
			var worklistItem = new ReportingWorklistItem();
			worklistItem.InitializeFromProcedureStep(reportingProcedureStep, WorklistItemField.ProcedureStepCreationTime);
			return new ReportingWorkflowAssembler().CreateWorklistItemSummary(worklistItem, this.PersistenceContext);
		}
	}
}
