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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Protocolling;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Services;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Extended.Services.ProtocollingWorkflow
{
	[ServiceImplementsContract(typeof(IProtocollingWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class ProtocollingWorkflowService : WorkflowServiceBase, IProtocollingWorkflowService
	{
		/// <summary>
		/// Provides a context for determining if protocol operations are enabled.
		/// </summary>
		public class ProtocolOperationEnablementContext
		{
			private readonly EntityRef _procedureStepRef;
			private readonly EntityRef _orderRef;

			/// <summary>
			/// Constructor.  One of the entity refs should be non-null.
			/// </summary>
			/// <param name="orderRef"></param>
			/// <param name="procedureStepRef"></param>
			public ProtocolOperationEnablementContext(EntityRef orderRef, EntityRef procedureStepRef)
			{
				this._orderRef = orderRef;
				this._procedureStepRef = procedureStepRef;
			}

			public EntityRef OrderRef
			{
				get { return _orderRef; }
			}

			public EntityRef ProcedureStepRef
			{
				get { return _procedureStepRef; }
			}
		}

		#region IProtocollingWorkflowService Members

		[ReadOperation]
		public GetProtocolFormDataResponse GetProtocolFormData(GetProtocolFormDataRequest request)
		{
			return new GetProtocolFormDataResponse
				{
					ProtocolUrgencyChoices = EnumUtils.GetEnumValueList<ProtocolUrgencyEnum>(this.PersistenceContext)
				};
		}

		[ReadOperation]
		public GetLinkableProtocolsResponse GetLinkableProtocols(GetLinkableProtocolsRequest request)
		{
			var step = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef, EntityLoadFlags.Proxy);

			var broker = this.PersistenceContext.GetBroker<IProtocolWorklistItemBroker>();
			var candidateSteps = broker.GetLinkedProtocolCandidates(step, this.CurrentUserStaff);

			// if any candidate steps were found, need to convert them to worklist items
			IList<ReportingWorklistItem> worklistItems;
			if (candidateSteps.Count > 0)
			{
				// because CLR does not support List co-variance, need to map to a list of the more general type (this seems silly!)
				var protocolSteps = CollectionUtils.Map<ProtocolAssignmentStep, ProtocolProcedureStep>(
					candidateSteps, s => s);

				worklistItems = broker.GetWorklistItems(protocolSteps, WorklistItemField.ProcedureStepCreationTime);
			}
			else
			{
				worklistItems = new List<ReportingWorklistItem>();
			}

			var assembler = new ReportingWorkflowAssembler();
			return new GetLinkableProtocolsResponse(
				CollectionUtils.Map<ReportingWorklistItem, ReportingWorklistItemSummary>(
					worklistItems, 
					item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request)
		{
			var assembler = new ProtocolAssembler();
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			var groups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
				this.PersistenceContext.GetBroker<IProtocolGroupBroker>().FindAll(procedure.Type),
				assembler.CreateProtocolGroupSummary);

			var initialProtocolGroup = CollectionUtils.FirstElement(groups);

			return new ListProtocolGroupsForProcedureResponse(groups, initialProtocolGroup);
		}

		[ReadOperation]
		public GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request)
		{
			var protocolGroup = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroup.ProtocolGroupRef);

			var assembler = new ProtocolAssembler();

			return new GetProtocolGroupDetailResponse(assembler.CreateProtocolGroupDetail(protocolGroup, false, this.PersistenceContext));
		}

		[ReadOperation]
		public GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request)
		{
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);
			var assembler = new ProtocolAssembler();

			return procedure.ActiveProtocol != null
				? new GetProcedureProtocolResponse(assembler.CreateProtocolDetail(procedure.ActiveProtocol, this.PersistenceContext))
				: new GetProcedureProtocolResponse(null);
		}

		[ReadOperation]
		public GetProcedurePlanForProtocollingWorklistItemResponse GetProcedurePlanForProtocollingWorklistItem(GetProcedurePlanForProtocollingWorklistItemRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var assembler = new ProcedurePlanAssembler();
			var procedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

			return new GetProcedurePlanForProtocollingWorklistItemResponse(procedurePlanSummary);
		}

		[ReadOperation]
		public GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request)
		{
			var choices = EnumUtils.GetEnumValueList<ProtocolRejectReasonEnum>(this.PersistenceContext);
			return new GetSuspendRejectReasonChoicesResponse(choices);
		}

		[UpdateOperation]
		[OperationEnablement("CanStartProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public StartProtocolResponse StartProtocol(StartProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			var protocolClaimed = false;
			var canPerformerAcceptProtocols = Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Accept);
			Staff assignedStaff = null;

			var linkedSteps = new List<ProtocolAssignmentStep>();
			if (request.LinkedProtocolAssignmentStepRefs != null && request.LinkedProtocolAssignmentStepRefs.Count > 0)
			{
				linkedSteps = CollectionUtils.Map<EntityRef, ProtocolAssignmentStep>(
					request.LinkedProtocolAssignmentStepRefs,
					stepRef => this.PersistenceContext.Load<ProtocolAssignmentStep>(stepRef));
			}

			if (request.ShouldClaim)
			{
				try
				{
					var op = new ProtocollingOperations.StartProtocolOperation();
					op.Execute(assignmentStep, linkedSteps, this.CurrentUserStaff, canPerformerAcceptProtocols, out protocolClaimed, out assignedStaff);
				}
				catch (Exception e)
				{
					throw new RequestValidationException(e.Message);
				}
			}

			var noteDetails = GetNoteDetails(assignmentStep.Procedure.Order, request.NoteCategory);

			var orderAssembler = new OrderAssembler();
			var orderDetailOptions = new OrderAssembler.CreateOrderDetailOptions {IncludeExtendedProperties = true};
			var orderDetail = orderAssembler.CreateOrderDetail(assignmentStep.Procedure.Order, orderDetailOptions, this.PersistenceContext);

			this.PersistenceContext.SynchState();

			return new StartProtocolResponse(
				assignmentStep.GetRef(),
				assignedStaff == null ? null : assignedStaff.GetRef(),
				protocolClaimed,
				noteDetails,
				orderDetail);
		}

		[UpdateOperation]
		[OperationEnablement("CanDiscardProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Create)]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Cancel)]
		public DiscardProtocolResponse DiscardProtocol(DiscardProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);
			var staff = request.ReassignToStaff == null ? null : this.PersistenceContext.Load<Staff>(request.ReassignToStaff);

			// demand authority token if trying to cancel a protocol that is perfomed by someone else
			if ((assignmentStep.State == ActivityStatus.SC && !Equals(assignmentStep.AssignedStaff, this.CurrentUserStaff)) ||
				(assignmentStep.State == ActivityStatus.IP && !Equals(assignmentStep.PerformingStaff, this.CurrentUserStaff)))
			{
				var permission = new PrincipalPermission(null, Extended.Common.AuthorityTokens.Workflow.Protocol.Cancel);
				permission.Demand();
			}

			var op = new ProtocollingOperations.DiscardProtocolOperation();
			op.Execute(assignmentStep, staff);

			this.PersistenceContext.SynchState();

			return new DiscardProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanAcceptProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Accept)]
		public AcceptProtocolResponse AcceptProtocol(AcceptProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var op = new ProtocollingOperations.AcceptProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new AcceptProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanRejectProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public RejectProtocolResponse RejectProtocol(RejectProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var reason = EnumUtils.GetEnumValue<ProtocolRejectReasonEnum>(request.RejectReason, this.PersistenceContext);

			var op = new ProtocollingOperations.RejectProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff, reason);

			AddAdditionalCommentsNote(request.AdditionalCommentsNote, assignmentStep.Procedure.Order);

			this.PersistenceContext.SynchState();

			return new RejectProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Create)]
		public SaveProtocolResponse SaveProtocol(SaveProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, false);

			return new SaveProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanResubmitProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.Resubmit)]
		public ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var op = new ProtocollingOperations.ResubmitProtocolOperation();
			op.Execute(order, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ResubmitProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSubmitProtocolForApproval")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public SubmitProtocolForApprovalResponse SubmitProtocolForApproval(SubmitProtocolForApprovalRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var op = new ProtocollingOperations.SubmitForApprovalOperation();
			op.Execute(assignmentStep);

			this.PersistenceContext.SynchState();

			return new SubmitProtocolForApprovalResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanReviseSubmittedProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public ReviseSubmittedProtocolResponse ReviseSubmittedProtocol(ReviseSubmittedProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			var op = new ProtocollingOperations.ReviseSubmittedProtocolOperation();
			var step = op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ReviseSubmittedProtocolResponse(GetWorklistItemSummary(step));
		}

		[UpdateOperation]
		[OperationEnablement("CanReassignProcedureStep")]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Workflow.Protocol.Reassign)]
		public ReassignProcedureStepResponse ReassignProcedureStep(ReassignProcedureStepRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureStepRef, "ProcedureStepRef");
			Platform.CheckMemberIsSet(request.ReassignedRadiologistRef, "ReassignedRadiologistRef");

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef, EntityLoadFlags.Proxy);
			var newStaff = this.PersistenceContext.Load<Staff>(request.ReassignedRadiologistRef, EntityLoadFlags.Proxy);

			var newStep = procedureStep.Reassign(newStaff);

			// Bug: #6342 - ensure that the reassigned step does not get lost because it is missing a scheduled start time.
			if (!newStep.Scheduling.StartTime.HasValue)
				newStep.Schedule(Platform.Time);

			this.PersistenceContext.SynchState();

			return new ReassignProcedureStepResponse(newStep.GetRef());
		}

		#endregion

		#region OperationEnablement methods

		public bool CanStartProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.StartProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanDiscardProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			// if there is no proc step ref, operation is not available
			if (enablementContext.ProcedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			var isAssignedToMe =
				(step.State == ActivityStatus.SC && Equals(step.AssignedStaff, this.CurrentUserStaff)) ||
				(step.State == ActivityStatus.IP && Equals(step.PerformingStaff, this.CurrentUserStaff));

			if (isAssignedToMe)
			{
				// Protocol is assigned to current user, allow cancel only if user has Create or Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Create) &&
					!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Cancel))
					return false;
			}
			else
			{
				// Protocol not assigned to current user, allow cancel only if user has Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Cancel))
					return false;
			}

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.DiscardProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanAcceptProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Accept))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.AcceptProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanRejectProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.RejectProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanSaveProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Create))
				return false;

			if (enablementContext.ProcedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			if (!step.Is<ProtocolAssignmentStep>())
				return false;

			if (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff))
				return false;

			if (step.PerformingStaff != null && !Equals(step.PerformingStaff, CurrentUserStaff))
				return false;

			// items submitted for review should not be editable.
			var assignmentStep = step.As<ProtocolAssignmentStep>();
			if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
			{
				if (Equals(assignmentStep.Protocol.Author, this.CurrentUserStaff))
					return false;
			}

			if (step.IsTerminated)
				return false;

			return true;
		}

		public bool CanResubmitProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.Resubmit))
				return false;
			return CanExecuteOperation(new ProtocollingOperations.ResubmitProtocolOperation(), enablementContext.OrderRef);
		}

		public bool CanSubmitProtocolForApproval(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.SubmitForApprovalOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanReviseSubmittedProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.ReviseSubmittedProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanReassignProcedureStep(ReportingWorklistItemKey itemKey)
		{
			if (itemKey.ProcedureStepRef == null)
				return false;

			var procedureStep = this.PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

			if (procedureStep.Is<ProtocolProcedureStep>())
				return Thread.CurrentPrincipal.IsInRole(Common.AuthorityTokens.Workflow.Protocol.Reassign);

			return false;
		}

		private bool CanExecuteOperation<T>(ProtocollingOperations.ProtocollingOperation op, EntityRef procedureStepRef)
			where T : ProtocolProcedureStep
		{
			// if there is no proc step ref, operation is not available
			if (procedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(procedureStepRef);

			if (!step.Is<T>())
				return false;

			return op.CanExecute(step.As<T>(), this.CurrentUserStaff);
		}

		private bool CanExecuteOperation(ProtocollingOperations.ProtocollingOperation op, EntityRef orderRef)
		{
			if (orderRef == null)
				return false;

			var order = this.PersistenceContext.Load<Order>(orderRef);

			return op.CanExecute(order, this.CurrentUserStaff);
		}

		#endregion

		private void SaveProtocolHelper(ProtocolAssignmentStep step, ProtocolDetail protocolDetail, List<OrderNoteDetail> notes, EntityRef supervisorRef, bool supervisorValidationRequired)
		{
			var protocol = step.Protocol;

			if (protocolDetail != null && supervisorRef != null)
				throw new RequestValidationException("UpdateProtocolRequest should not specify both a ProtocolDetail and a SupervisorRef");

			if (supervisorValidationRequired
				&& Thread.CurrentPrincipal.IsInRole(Extended.Common.AuthorityTokens.Workflow.Protocol.OmitSupervisor) == false
				&& protocol.Supervisor == null
				&& (protocolDetail == null || protocolDetail.Supervisor == null)
				&& supervisorRef == null)
			{
				throw new SupervisorValidationException();
			}

			if (protocolDetail != null)
			{
				var assembler = new ProtocolAssembler();
				assembler.UpdateProtocol(protocol, protocolDetail, this.PersistenceContext);
			}

			if (supervisorRef != null)
			{
				var supervisor = this.PersistenceContext.Load<Staff>(supervisorRef);
				protocol.Supervisor = supervisor;
			}

			if (notes != null)
				UpdateOrderNotes(step.Procedure.Order, notes);
		}

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as WorklistItemSummaryBase;
			return summary == null ? null : new ProtocolOperationEnablementContext(summary.OrderRef, summary.ProcedureStepRef);
		}

		private void AddAdditionalCommentsNote(OrderNoteDetail detail, Order order)
		{
			if (detail != null)
			{
				var noteAssembler = new OrderNoteAssembler();
				noteAssembler.CreateOrderNote(detail, order, this.CurrentUserStaff, true, this.PersistenceContext);
			}
		}

		private List<OrderNoteDetail> GetNoteDetails(Order order, string category)
		{
			var noteAssembler = new OrderNoteAssembler();

			return CollectionUtils.Map<OrderNote, OrderNoteDetail>(
				OrderNote.GetNotesForOrder(order, new[]{category}, false),
				note => noteAssembler.CreateOrderNoteDetail(note, this.PersistenceContext));
		}

		private void UpdateOrderNotes(Order order, IList<OrderNoteDetail> notes)
		{
			var noteAssembler = new OrderNoteAssembler();

			noteAssembler.SynchronizeOrderNotes(order, notes, this.CurrentUserStaff, this.PersistenceContext);
		}

		private ReportingWorklistItemSummary GetWorklistItemSummary(ProtocolProcedureStep reportingProcedureStep)
		{
			var worklistItem = new ReportingWorklistItem();
			worklistItem.InitializeFromProcedureStep(reportingProcedureStep, WorklistItemField.ProcedureStepCreationTime);
			return new ReportingWorkflowAssembler().CreateWorklistItemSummary(worklistItem, this.PersistenceContext);
		}

	}
}
