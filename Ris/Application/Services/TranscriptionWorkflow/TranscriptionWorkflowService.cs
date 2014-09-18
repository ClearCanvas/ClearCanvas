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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Healthcare.Workflow.Transcription;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.TranscriptionWorkflow
{
	[ServiceImplementsContract(typeof(ITranscriptionWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class TranscriptionWorkflowService : WorkflowServiceBase, ITranscriptionWorkflowService
	{
		#region IWorklistService Members

		[ReadOperation]
		[AuditRecorder(typeof(WorkflowServiceRecorder.SearchWorklists))]
		public TextQueryResponse<ReportingWorklistItemSummary> SearchWorklists(WorklistItemTextQueryRequest request)
		{
			var assembler = new ReportingWorkflowAssembler();
			var broker = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>();

			return SearchHelper<ReportingWorklistItem, ReportingWorklistItemSummary>(
				request,
				broker,
				WorklistItemProjection.ReportingWorklistSearch,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
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

		[ReadOperation]
		public GetRejectReasonChoicesResponse GetRejectReasonChoices(GetRejectReasonChoicesRequest request)
		{
			return new GetRejectReasonChoicesResponse(
				EnumUtils.GetEnumValueList<TranscriptionRejectReasonEnum>(this.PersistenceContext));
		}

		#endregion

		#region ITranscriptionWorkflowService Members

		[ReadOperation]
		public LoadTranscriptionForEditResponse LoadTranscriptionForEdit(LoadTranscriptionForEditRequest request)
		{
			var step = this.PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
			var reportAssembler = new ReportAssembler();
			var orderAssembler = new OrderAssembler();

			var orderDetailOptions = new OrderAssembler.CreateOrderDetailOptions {IncludeExtendedProperties = true};
			return new LoadTranscriptionForEditResponse(
				reportAssembler.CreateReportDetail(step.ReportPart.Report, false, this.PersistenceContext),
				step.ReportPart.Index,
				orderAssembler.CreateOrderDetail(step.Procedure.Order, orderDetailOptions, this.PersistenceContext));
		}

		[UpdateOperation]
		[OperationEnablement("CanStartTranscription")]
		public StartTranscriptionResponse StartTranscription(StartTranscriptionRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);

			var op = new TranscriptionOperations.StartTranscription();
			op.Execute(transcriptionStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new StartTranscriptionResponse(transcriptionStep.GetRef());
		}

		[UpdateOperation]
		[OperationEnablement("CanDiscardTranscription")]
		public DiscardTranscriptionResponse DiscardTranscription(DiscardTranscriptionRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);

			var op = new TranscriptionOperations.DiscardTranscription();
			op.Execute(transcriptionStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new DiscardTranscriptionResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveTranscription")]
		public SaveTranscriptionResponse SaveTranscription(SaveTranscriptionRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);
			var supervisor = request.SupervisorRef == null
				? null
				: this.PersistenceContext.Load<Staff>(request.SupervisorRef, EntityLoadFlags.Proxy);

			SaveReportHelper(transcriptionStep, request.ReportPartExtendedProperties, supervisor);

			this.PersistenceContext.SynchState();

			return new SaveTranscriptionResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSubmitTranscriptionForReview")]
		public SubmitTranscriptionForReviewResponse SubmitTranscriptionForReview(SubmitTranscriptionForReviewRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);
			var supervisor = ResolveSupervisor(transcriptionStep, request.SupervisorRef);

			if (supervisor == null)
				throw new RequestValidationException(SR.InvalidRequest_SupervisorIsRequired);

			SaveReportHelper(transcriptionStep, request.ReportPartExtendedProperties, supervisor);

			var op = new TranscriptionOperations.SubmitTranscriptionForReview();
			op.Execute(transcriptionStep, this.CurrentUserStaff, supervisor);

			this.PersistenceContext.SynchState();

			return new SubmitTranscriptionForReviewResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanCompleteTranscription")]
		public CompleteTranscriptionResponse CompleteTranscription(CompleteTranscriptionRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);

			SaveReportHelper(transcriptionStep, request.ReportPartExtendedProperties);

			var op = new TranscriptionOperations.CompleteTranscription();
			op.Execute(transcriptionStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new CompleteTranscriptionResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanRejectTranscription")]
		public RejectTranscriptionResponse RejectTranscription(RejectTranscriptionRequest request)
		{
			var transcriptionStep = this.PersistenceContext.Load<TranscriptionStep>(request.TranscriptionStepRef);

			SaveReportHelper(transcriptionStep, request.ReportPartExtendedProperties);

			var rejectReason = EnumUtils.GetEnumValue<TranscriptionRejectReasonEnum>(request.RejectReason, this.PersistenceContext);

			var op = new TranscriptionOperations.RejectTranscription();
			op.Execute(transcriptionStep, this.CurrentUserStaff, rejectReason);

			AddAdditionalCommentsNote(request.AdditionalComments, transcriptionStep.Procedure.Order);

			this.PersistenceContext.SynchState();

			return new RejectTranscriptionResponse();
		}

		#endregion

		#region Private members

		#region OperationEnablement methods

		public bool CanStartTranscription(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.StartTranscription(), itemKey);
		}

		public bool CanDiscardTranscription(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.DiscardTranscription(), itemKey);
		}

		public bool CanSaveTranscription(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.SaveTranscription(), itemKey);
		}

		public bool CanSubmitTranscriptionForReview(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.SubmitTranscriptionForReview(), itemKey);
		}

		public bool CanCompleteTranscription(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.CompleteTranscription(), itemKey);
		}

		public bool CanRejectTranscription(ReportingWorklistItemKey itemKey)
		{
			return CanExecuteOperation(new TranscriptionOperations.RejectTranscription(), itemKey);
		}

		private bool CanExecuteOperation(TranscriptionOperations.TranscriptionOperation op, ReportingWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Transcription.Create))
				return false;

			// if there is no proc step ref, operation is not available
			if (itemKey.ProcedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

			// for now, all of these operations assume they are operating on a 
			// this may need to change in future
			if (!step.Is<TranscriptionStep>())
				return false;

			return op.CanExecute(step.As<TranscriptionStep>(), this.CurrentUserStaff);
		}


		#endregion

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as ReportingWorklistItemSummary;
			return summary == null ? null : new ReportingWorklistItemKey(summary.ProcedureStepRef, summary.ProcedureRef);
		}

		private static void SaveReportHelper(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties)
		{
			if (reportPartExtendedProperties == null)
				return;

			var op = new TranscriptionOperations.SaveTranscription();
			op.Execute(step, reportPartExtendedProperties);
		}

		private static void SaveReportHelper(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor)
		{
			var op = new TranscriptionOperations.SaveTranscription();
			op.Execute(step, reportPartExtendedProperties, supervisor);
		}

		/// <summary>
		/// Get the supervisor, using the new supervisor if supplied, otherwise using an existing supervisor if found.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="newSupervisorRef"></param>
		/// <returns></returns>
		private Staff ResolveSupervisor(TranscriptionStep step, EntityRef newSupervisorRef)
		{
			var supervisor = newSupervisorRef == null ? null : this.PersistenceContext.Load<Staff>(newSupervisorRef, EntityLoadFlags.Proxy);

			if (supervisor == null && step.ReportPart != null)
				supervisor = step.ReportPart.TranscriptionSupervisor;

			return supervisor;
		}

		private void AddAdditionalCommentsNote(OrderNoteDetail detail, Order order)
		{
			if (detail == null) return;

			var noteAssembler = new OrderNoteAssembler();
			noteAssembler.CreateOrderNote(detail, order, this.CurrentUserStaff, true, this.PersistenceContext);
		}

		#endregion
	}
}
