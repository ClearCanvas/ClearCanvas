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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IModalityWorkflowService))]
	public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
	{
		#region IModalityWorkflowService members

		/// <summary>
		/// SearchWorklists for worklist items based on specified criteria.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ReadOperation]
		[AuditRecorder(typeof(WorkflowServiceRecorder.SearchWorklists))]
		public TextQueryResponse<ModalityWorklistItemSummary> SearchWorklists(WorklistItemTextQueryRequest request)
		{
			var assembler = new ModalityWorkflowAssembler();
			var broker = this.PersistenceContext.GetBroker<IModalityWorklistItemBroker>();
			return SearchHelper<WorklistItem, ModalityWorklistItemSummary>(
				request,
				broker,
				WorklistItemProjection.ModalityWorklistSearch,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
		}

		/// <summary>
		/// Query the specified worklist.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ReadOperation]
		[ResponseCaching("GetQueryWorklistCacheDirective")]
		public QueryWorklistResponse<ModalityWorklistItemSummary> QueryWorklist(QueryWorklistRequest request)
		{
			var assembler = new ModalityWorkflowAssembler();
			return QueryWorklistHelper<WorklistItem, ModalityWorklistItemSummary>(
				request,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
		}

		/// <summary>
		/// Returns a summary of the procedure plan for a specified order.
		/// </summary>
		/// <param name="request"><see cref="GetProcedurePlanRequest"/></param>
		/// <returns><see cref="GetProcedurePlanResponse"/></returns>
		[ReadOperation]
		public GetProcedurePlanResponse GetProcedurePlan(GetProcedurePlanRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);
			var assembler = new ProcedurePlanAssembler();
			return new GetProcedurePlanResponse(assembler.CreateProcedurePlanSummary(order, this.PersistenceContext));
		}

		/// <summary>
		/// Returns a list of all modality performed procedure steps for a particular order.
		/// </summary>
		/// <param name="request"><see cref="ListPerformedProcedureStepsRequest"/></param>
		/// <returns><see cref="ListPerformedProcedureStepsResponse"/></returns>
		[ReadOperation]
		public ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var assembler = new ModalityPerformedProcedureStepAssembler();

			var mppsSet = new HashedSet<PerformedStep>();
			foreach (var procedure in order.Procedures)
			{
				foreach (var mps in procedure.ModalityProcedureSteps)
				{
					mppsSet.AddAll(mps.PerformedSteps);
				}
			}

			return new ListPerformedProcedureStepsResponse(
				CollectionUtils.Map<ModalityPerformedProcedureStep, ModalityPerformedProcedureStepDetail>(
					mppsSet,
					mpps => assembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext)));
		}


		/// <summary>
		/// Starts a specified set of modality procedure steps with a single modality performed procedure step.
		/// </summary>
		/// <param name="request"><see cref="StartModalityProcedureStepsRequest"/></param>
		/// <returns><see cref="StartModalityProcedureStepsResponse"/></returns>
		[UpdateOperation]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.StartProcedures))]
		public StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ModalityProcedureSteps, "ModalityProcedureSteps");

			// load the set of mps
			var modalitySteps = CollectionUtils.Map<EntityRef, ModalityProcedureStep>(
				request.ModalityProcedureSteps,
				mpsRef => this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef));

			var hasProcedureNotCheckedIn = CollectionUtils.Contains(
				modalitySteps,
				mps => mps.Procedure.IsPreCheckIn);

			if (hasProcedureNotCheckedIn)
				throw new RequestValidationException(SR.ExceptionProcedureNotCheckedIn);

			var op = new StartModalityProcedureStepsOperation();
			var mpps = op.Execute(modalitySteps, request.StartTime, this.CurrentUserStaff, new PersistentWorkflow(PersistenceContext));

			this.PersistenceContext.SynchState();

			var procedurePlanAssembler = new ProcedurePlanAssembler();
			var mppsAssembler = new ModalityPerformedProcedureStepAssembler();
			return new StartModalityProcedureStepsResponse(
				procedurePlanAssembler.CreateProcedurePlanSummary(modalitySteps[0].Procedure.Order, this.PersistenceContext),
				mppsAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext));
		}

		/// <summary>
		/// Discontinues a set of specified modality procedure steps.
		/// </summary>
		/// <param name="request"><see cref="DiscontinueModalityProcedureStepsResponse"/></param>
		/// <returns><see cref="DiscontinueModalityProcedureStepsRequest"/></returns>
		[UpdateOperation]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.DiscontinueProcedures))]
		public DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ModalityProcedureSteps, "ModalityProcedureSteps");

			// load the set of mps
			var modalitySteps = CollectionUtils.Map<EntityRef, ModalityProcedureStep>(
				request.ModalityProcedureSteps,
				mpsRef => this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef));

			foreach (var step in modalitySteps)
			{
				var op = new DiscontinueModalityProcedureStepOperation();
				op.Execute(step, request.DiscontinuedTime, new PersistentWorkflow(this.PersistenceContext));

				// If discontinuing the procedure step caused the parent procedure to be discontinued,
				// create an HL7 event.
				if (step.Procedure.IsTerminated)
					LogicalHL7Event.ProcedureCancelled.EnqueueEvents(step.Procedure);
			}

			this.PersistenceContext.SynchState();

			var assembler = new ProcedurePlanAssembler();
			return new DiscontinueModalityProcedureStepsResponse(
				assembler.CreateProcedurePlanSummary(modalitySteps[0].Procedure.Order, this.PersistenceContext));
		}

		/// <summary>
		/// Completes a specified modality performed procedure step.
		/// </summary>
		/// <param name="request"><see cref="CompleteModalityPerformedProcedureStepRequest"/></param>
		/// <returns><see cref="CompleteModalityPerformedProcedureStepResponse"/></returns>
		[UpdateOperation]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.CompleteProcedures))]
		public CompleteModalityPerformedProcedureStepResponse CompleteModalityPerformedProcedureStep(CompleteModalityPerformedProcedureStepRequest request)
		{
			var mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef);

			// update extended properties (should this be in an assembler?)
			ExtendedPropertyUtils.Update(mpps.ExtendedProperties, request.Mpps.ExtendedProperties);

			var dicomSeriesAssembler = new DicomSeriesAssembler();
			dicomSeriesAssembler.SynchronizeDicomSeries(mpps, request.Mpps.DicomSeries, this.PersistenceContext);

			var op = new CompleteModalityPerformedProcedureStepOperation();
			op.Execute(mpps, request.CompletedTime, new PersistentWorkflow(PersistenceContext));

			this.PersistenceContext.SynchState();

			// Drill back to order so we can refresh procedure plan
			var onePs = CollectionUtils.FirstElement(mpps.Activities).As<ProcedureStep>();

			var planAssembler = new ProcedurePlanAssembler();
			var stepAssembler = new ModalityPerformedProcedureStepAssembler();
			return new CompleteModalityPerformedProcedureStepResponse(
				planAssembler.CreateProcedurePlanSummary(onePs.Procedure.Order, this.PersistenceContext),
				stepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext));
		}

		/// <summary>
		/// Discontinues a specified modality performed procedure step.
		/// </summary>
		/// <param name="request"><see cref="DiscontinueModalityPerformedProcedureStepRequest"/></param>
		/// <returns><see cref="DiscontinueModalityPerformedProcedureStepResponse"/></returns>
		[UpdateOperation]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.DiscontinueProcedures))]
		public DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request)
		{
			var mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef);

			// update extended properties (should this be in an assembler?)
			ExtendedPropertyUtils.Update(mpps.ExtendedProperties, request.Mpps.ExtendedProperties);

			var dicomSeriesAssembler = new DicomSeriesAssembler();
			dicomSeriesAssembler.SynchronizeDicomSeries(mpps, request.Mpps.DicomSeries, this.PersistenceContext);

			var op = new DiscontinueModalityPerformedProcedureStepOperation();
			op.Execute(mpps, request.DiscontinuedTime, new PersistentWorkflow(PersistenceContext));

			this.PersistenceContext.SynchState();

			// If discontinuing the MPPS caused any associated procedures to be discontinued,
			// create an HL7 event.
			foreach (var activity in mpps.Activities)
			{
				var procedure = activity.As<ProcedureStep>().Procedure;
				if(procedure.IsTerminated)
					LogicalHL7Event.ProcedureCancelled.EnqueueEvents(procedure);
			}
	
			// Drill back to order so we can refresh procedure plan
			var order = CollectionUtils.FirstElement(mpps.Activities).As<ProcedureStep>().Procedure.Order;

			var planAssembler = new ProcedurePlanAssembler();
			var stepAssembler = new ModalityPerformedProcedureStepAssembler();
			return new DiscontinueModalityPerformedProcedureStepResponse(
				planAssembler.CreateProcedurePlanSummary(order, this.PersistenceContext),
				stepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadOrderDocumentationDataResponse LoadOrderDocumentationData(LoadOrderDocumentationDataRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var noteAssembler = new OrderNoteAssembler();

			return new LoadOrderDocumentationDataResponse
			{
				OrderRef = order.GetRef(),
				OrderExtendedProperties = ExtendedPropertyUtils.Copy(order.ExtendedProperties),
				OrderNotes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(
					OrderNote.GetNotesForOrder(order),
					note => noteAssembler.CreateOrderNoteDetail(note, PersistenceContext)),
				AssignedInterpreter = GetUniqueAssignedInterpreter(order)
			};
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Documentation.Create)]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.UpdateDocumentation))]
		public SaveOrderDocumentationDataResponse SaveOrderDocumentationData(SaveOrderDocumentationDataRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			ExtendedPropertyUtils.Update(order.ExtendedProperties, request.OrderExtendedProperties);

			var dicomSeriesAssembler = new DicomSeriesAssembler();
			foreach (var detail in request.ModalityPerformedProcedureSteps)
			{
				var mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(detail.ModalityPerformendProcedureStepRef);
				ExtendedPropertyUtils.Update(mpps.ExtendedProperties, detail.ExtendedProperties);
				dicomSeriesAssembler.SynchronizeDicomSeries(mpps, detail.DicomSeries, this.PersistenceContext);
			}

			// add new order notes
			var noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, request.OrderNotes, CurrentUserStaff, this.PersistenceContext);

			// assign all procedures for this order to the specified interpreter (or unassign them, if null)
			var interpreter = request.AssignedInterpreter == null
				? null
				: this.PersistenceContext.Load<Staff>(request.AssignedInterpreter.StaffRef, EntityLoadFlags.Proxy);
			foreach (var procedure in order.Procedures)
			{
				if (procedure.IsPerformed)
				{
					var interpretationStep = GetPendingInterpretationStep(procedure);
					if (interpretationStep != null)
					{
						interpretationStep.Assign(interpreter);
					}
				}
			}

			this.PersistenceContext.SynchState();

			var planAssembler = new ProcedurePlanAssembler();
			return new SaveOrderDocumentationDataResponse(planAssembler.CreateProcedurePlanSummary(order, this.PersistenceContext));
		}

		[ReadOperation]
		public CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Documentation.Accept))
				return new CanCompleteOrderDocumentationResponse(false, false);

			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			// order documentation can be completed if all modality steps have been terminated
			var allModalityStepsTerminated = CollectionUtils.TrueForAll(
				order.Procedures,
				procedure => CollectionUtils.TrueForAll(procedure.ModalityProcedureSteps, mps => mps.IsTerminated));

			// order documentation is already completed if all procedures have a completed documentation step
			var alreadyCompleted = CollectionUtils.TrueForAll(
				order.Procedures,
				procedure => procedure.DocumentationProcedureStep != null && procedure.DocumentationProcedureStep.IsTerminated);

			return new CanCompleteOrderDocumentationResponse(
				allModalityStepsTerminated && alreadyCompleted == false,
				alreadyCompleted);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Documentation.Accept)]
		[AuditRecorder(typeof(ModalityWorkflowServiceRecorder.CompleteDocumentation))]
		public CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var interpretationSteps = new List<InterpretationStep>();
			foreach (var procedure in order.Procedures)
			{
				if (procedure.DocumentationProcedureStep != null && !procedure.DocumentationProcedureStep.IsTerminated)
				{
					procedure.DocumentationProcedureStep.Complete();
				}

				// schedule the interpretation step if the procedure was performed
				// Note: this logic is probably UHN-specific... ideally this aspect of the workflow should be configurable,
				// because it may be desirable to scheduled the interpretation prior to completing the documentation
				if (procedure.IsPerformed)
				{
					var interpretationStep = GetPendingInterpretationStep(procedure);
					if (interpretationStep != null)
					{
						// bug #3037: schedule the interpretation for the performed time, which may be earlier than the current time 
						// in downtime mode
						interpretationStep.Schedule(procedure.PerformedTime);
						interpretationSteps.Add(interpretationStep);
					}
				}
			}

			this.PersistenceContext.SynchState();

			var planAssembler = new ProcedurePlanAssembler();
			return new CompleteOrderDocumentationResponse
			{
				ProcedurePlan = planAssembler.CreateProcedurePlanSummary(order, this.PersistenceContext),
				InterpretationStepRefs = CollectionUtils.Map<InterpretationStep, EntityRef>(interpretationSteps, step => step.GetRef())
			};
		}

		#endregion

		#region WorkflowServiceBase overrides

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as ModalityWorklistItemSummary;
			return summary == null ? null : new ModalityWorklistItemKey(summary.ProcedureStepRef);
		}

		#endregion

		#region Private Members

		private InterpretationStep GetPendingInterpretationStep(Procedure procedure)
		{
			// bug #3859: don't want to create an interpretation step for a completed procedure
			// (migrated data may not have any interpretation steps even for a completed procedure)
			if (procedure.IsTerminated)
				return null;

			var interpretationSteps = CollectionUtils.Select(
				procedure.ProcedureSteps,
				ps => ps.Is<InterpretationStep>());

			// no interp step, so create one
			if (interpretationSteps.Count == 0)
			{
				var interpretationStep = new InterpretationStep(procedure);
				this.PersistenceContext.Lock(interpretationStep, DirtyState.New);
				return interpretationStep;
			}

			// may be multiple interp steps (eg maybe one was started and discontinued), so find the one that is scheduled
			var pendingStep = CollectionUtils.SelectFirst(
				interpretationSteps,
				ps => ps.State == ActivityStatus.SC);

			return pendingStep == null ? null : pendingStep.As<InterpretationStep>();
		}

		private StaffSummary GetUniqueAssignedInterpreter(Order order)
		{
			StaffSummary uniqueAssignedInterpreter = null;
			var staffAssembler = new StaffAssembler();

			// establish whether there is a unique assigned interpreter for all procedures
			var interpreters = new HashedSet<Staff>();
			foreach (var procedure in order.Procedures)
			{
				var pendingInterpretationStep = procedure.GetProcedureStep(
					ps => ps.Is<InterpretationStep>() && ps.State == ActivityStatus.SC);

				if (pendingInterpretationStep != null && pendingInterpretationStep.AssignedStaff != null)
					interpreters.Add(pendingInterpretationStep.AssignedStaff);
			}

			if (interpreters.Count == 1)
			{
				uniqueAssignedInterpreter = staffAssembler.CreateStaffSummary(
					CollectionUtils.FirstElement(interpreters),
					this.PersistenceContext);
			}

			return uniqueAssignedInterpreter;
		}

		#endregion
	}
}
