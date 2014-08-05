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
using System.IO;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Enterprise.Core.Printing;
using ClearCanvas.Healthcare.Printing;
using ClearCanvas.Healthcare.Workflow.OrderEntry;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;
using System;
using System.Linq;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IOrderEntryService))]
	public class OrderEntryService : WorkflowServiceBase, IOrderEntryService
	{
		public class WorklistItemKey
		{
			private readonly EntityRef _orderRef;

			public WorklistItemKey(EntityRef orderRef)
			{
				_orderRef = orderRef;
			}

			public EntityRef OrderRef
			{
				get { return _orderRef; }
			}
		}

		#region IOrderEntryService Members

		[ReadOperation]
		public ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.PatientRef, "PatientRef");

			var patient = this.PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientRef, EntityLoadFlags.Proxy);

			var criteria = new VisitSearchCriteria();
			criteria.Patient.EqualTo(patient);
			criteria.AdmitTime.SortDesc(0);

			var assembler = new VisitAssembler();
			return new ListVisitsForPatientResponse(
				CollectionUtils.Map<Visit, VisitSummary, List<VisitSummary>>(
					this.PersistenceContext.GetBroker<IVisitBroker>().Find(criteria),
					v => assembler.CreateVisitSummary(v, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListOrdersForPatientResponse ListActiveOrdersForPatient(ListOrdersForPatientRequest request)
		{
			var criteria = new OrderSearchCriteria();

			var profile = this.PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);
			criteria.Patient.EqualTo(profile.Patient);
			criteria.Status.In(new[] { OrderStatus.SC, OrderStatus.IP });

			var assembler = new OrderAssembler();
			return new ListOrdersForPatientResponse(
				CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
					this.PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
					order => assembler.CreateOrderSummary(order, this.PersistenceContext)));
		}

		[ReadOperation]
		public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			// Sorted list of facility summaries for active facilities
			var facilityAssembler = new FacilityAssembler();
			var facilitySearchCriteria = new FacilitySearchCriteria();
			facilitySearchCriteria.Deactivated.EqualTo(false);
			facilitySearchCriteria.Name.SortAsc(0);
			var facilities = CollectionUtils.Map(
				this.PersistenceContext.GetBroker<IFacilityBroker>().Find(facilitySearchCriteria),
				(Facility f) => facilityAssembler.CreateFacilitySummary(f));

			// Sorted list of department summaries for active departments
			var departmentAssembler = new DepartmentAssembler();
			var departmentSearchCriteria = new DepartmentSearchCriteria();
			departmentSearchCriteria.Deactivated.EqualTo(false);
			departmentSearchCriteria.Name.SortAsc(0);
			var departments = CollectionUtils.Map(
				this.PersistenceContext.GetBroker<IDepartmentBroker>().Find(departmentSearchCriteria),
				(Department d) => departmentAssembler.CreateSummary(d, this.PersistenceContext));

			// Sorted list of department summaries for active departments
			var modalityAssembler = new ModalityAssembler();
			var modalitySearchCriteria = new ModalitySearchCriteria();
			modalitySearchCriteria.Deactivated.EqualTo(false);
			modalitySearchCriteria.Name.SortAsc(0);
			var modalities = CollectionUtils.Map(
				this.PersistenceContext.GetBroker<IModalityBroker>().Find(modalitySearchCriteria),
				(Modality d) => modalityAssembler.CreateModalitySummary(d));

			return new GetOrderEntryFormDataResponse(
				facilities,
				departments,
				modalities,
				EnumUtils.GetEnumValueList<OrderPriorityEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<LateralityEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<SchedulingCodeEnum>(this.PersistenceContext)
				);
		}

		[ReadOperation]
		public LoadDiagnosticServicePlanResponse LoadDiagnosticServicePlan(LoadDiagnosticServicePlanRequest request)
		{
			var dsBroker = this.PersistenceContext.GetBroker<IDiagnosticServiceBroker>();

			var diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);

			var assembler = new DiagnosticServiceAssembler();
			return new LoadDiagnosticServicePlanResponse(assembler.CreatePlanDetail(diagnosticService, request.IncludeDeactivatedProcedures, PersistenceContext));
		}

		[ReadOperation]
		public GetExternalPractitionerContactPointsResponse GetExternalPractitionerContactPoints(GetExternalPractitionerContactPointsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.PractitionerRef, "PractitionerRef");

			var practitioner = this.PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
			var assembler = new ExternalPractitionerAssembler();

			// sort contact points such that default is first (descending sort)
			var sortedContactPoints = CollectionUtils.Sort(practitioner.ContactPoints,
														   (x, y) => -x.IsDefaultContactPoint.CompareTo(y.IsDefaultContactPoint));

			var responseContactPoints = sortedContactPoints;
			if (!request.IncludeDeactivated)
			{
				responseContactPoints = CollectionUtils.Select(sortedContactPoints, cp => !cp.Deactivated);
			}

			return new GetExternalPractitionerContactPointsResponse(
				CollectionUtils.Map(
					responseContactPoints,
					(ExternalPractitionerContactPoint cp) => assembler.CreateExternalPractitionerContactPointDetail(cp, this.PersistenceContext)));
		}

		[ReadOperation]
		public GetCancelOrderFormDataResponse GetCancelOrderFormData(GetCancelOrderFormDataRequest request)
		{
			return new GetCancelOrderFormDataResponse(EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(this.PersistenceContext));
		}

		[ReadOperation]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.LoadOrderForEdit))]
		public GetOrderRequisitionForEditResponse GetOrderRequisitionForEdit(GetOrderRequisitionForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			if (request.ProcedureRef == null)
				Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
			if (request.OrderRef == null)
				Platform.CheckMemberIsSet(request.ProcedureRef, "ProcedureRef");


			var order = request.OrderRef != null ?
				this.PersistenceContext.Load<Order>(request.OrderRef)
				: this.PersistenceContext.Load<Procedure>(request.ProcedureRef).Order;

			var assembler = new OrderEntryAssembler();
			var requisition = assembler.CreateOrderRequisition(order, this.PersistenceContext);
			return new GetOrderRequisitionForEditResponse(requisition);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Create)]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.PlaceOrder))]
		public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");

			var order = PlaceOrderHelper(request.Requisition);

			ValidateVisitsExist(order);

			// ensure the new order is assigned an OID before using it in the return value
			this.PersistenceContext.SynchState();

			LogicalHL7Event.OrderCreated.EnqueueEvents(order);

			var orderAssembler = new OrderAssembler();
			return new PlaceOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Modify)]
		[OperationEnablement("CanModifyOrder")]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.ModifyOrder))]
		public ModifyOrderResponse ModifyOrder(ModifyOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");
			Platform.CheckMemberIsSet(request.Requisition.OrderRef, "OrderRef");

			var order = this.PersistenceContext.Load<Order>(request.Requisition.OrderRef);

			var assembler = new OrderEntryAssembler();
			assembler.UpdateOrderFromRequisition(order, request.Requisition, this.CurrentUserStaff, this.PersistenceContext);

			UpdateProceduresHelper(order, request.Requisition.Procedures, request);
			ValidateVisitsExist(order);

			this.PersistenceContext.SynchState();

			var orderAssembler = new OrderAssembler();
			return new ModifyOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Replace)]
		[OperationEnablement("CanReplaceOrder")]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.ReplaceOrder))]
		public ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");

			var orderToReplace = this.PersistenceContext.Load<Order>(request.OrderRef);
			ValidateOrderReplacable(orderToReplace);

			// reason is optional
			var reason = (request.CancelReason != null) ?
				EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, this.PersistenceContext) : null;

			// duplicate any attachments in the requisition,
			// so that the replacement order gets a copy while the replaced order
			// retains the association to the originals
			DuplicateAttachmentsForOrderReplace(orderToReplace, request.Requisition);

			// place new order
			var newOrder = PlaceOrderHelper(request.Requisition);
			ValidateVisitsExist(newOrder);

			// cancel existing order
			CancelOrderHelper(orderToReplace, new OrderCancelInfo(reason, this.CurrentUserStaff, null, newOrder));

			this.PersistenceContext.SynchState();

			LogicalHL7Event.OrderCreated.EnqueueEvents(newOrder);
			LogicalHL7Event.OrderCancelled.EnqueueEvents(orderToReplace);

			var orderAssembler = new OrderAssembler();
			return new ReplaceOrderResponse(orderAssembler.CreateOrderSummary(newOrder, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Merge)]
		[OperationEnablement("CanMergeOrder")]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.MergeOrder))]
		public MergeOrderResponse MergeOrder(MergeOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.SourceOrderRefs, "SourceOrderRefs");
			Platform.CheckTrue(request.SourceOrderRefs.Count > 0, "SourceOrderRefs.Count > 0");
			Platform.CheckMemberIsSet(request.DestinationOrderRef, "DestinationOrderRef");

			var response = new MergeOrderResponse();
			DryRunHelper(request.DryRun,
				delegate
				{
					var destinationOrder = this.PersistenceContext.Load<Order>(request.DestinationOrderRef);
					var sourceOrders = CollectionUtils.Map(request.SourceOrderRefs, (EntityRef r) => PersistenceContext.Load<Order>(r));
					var mergeInfo = new OrderMergeInfo(this.CurrentUserStaff, Platform.Time, destinationOrder);

					MergeOrderHelper(destinationOrder, sourceOrders, mergeInfo, request.ValidationOnly);

					if (request.DryRun)
					{
						var orderAssembler = new OrderAssembler();
						var orderDetail = orderAssembler.CreateOrderDetail(destinationOrder,
							OrderAssembler.CreateOrderDetailOptions.GetVerboseOptions(), PersistenceContext);
						response.DryRunMergedOrder = orderDetail;
					}
				});
			return response;
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Unmerge)]
		[OperationEnablement("CanUnmergeOrder")]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.UnmergeOrder))]
		public UnmergeOrderResponse UnmergeOrder(UnmergeOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			// reason is not required for dry run, but otherwise it is
			if (!request.DryRun && request.UnmergeReason == null)
				throw new ArgumentNullException("UnmergeReason");

			DryRunHelper(request.DryRun,
				delegate
				{
					var destinationOrder = this.PersistenceContext.Load<Order>(request.OrderRef);
					var sourceOrders = destinationOrder.MergeSourceOrders;
					if (sourceOrders.Count == 0)
						throw new RequestValidationException(SR.InvalidRequest_NoOrdersToUnmerge);

					// load the reason; if reason is null (eg dry run), just get the first available reason
					var reason = request.UnmergeReason == null ?
						CollectionUtils.FirstElement(PersistenceContext.GetBroker<IEnumBroker>().Load<OrderCancelReasonEnum>(false))
						: EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.UnmergeReason, PersistenceContext);

					var cancelInfo = new OrderCancelInfo(reason, this.CurrentUserStaff, "Un-merged");
					var accBroker = PersistenceContext.GetBroker<IAccessionNumberBroker>();

					// do unmerge
					UnmergeHelper(sourceOrders, cancelInfo, accBroker);
				});

			return new UnmergeOrderResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanCancelOrder")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Cancel)]
		[AuditRecorder(typeof(OrderEntryServiceRecorder.CancelOrder))]
		public CancelOrderResponse CancelOrder(CancelOrderRequest request)
		{
			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
			var reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, this.PersistenceContext);

			CancelOrderHelper(order, new OrderCancelInfo(reason, this.CurrentUserStaff));

			LogicalHL7Event.OrderCancelled.EnqueueEvents(order);

			return new CancelOrderResponse();
		}

		[ReadOperation]
		public QueryCancelOrderWarningsResponse QueryCancelOrderWarnings(QueryCancelOrderWarningsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			var order = this.PersistenceContext.Load<Order>(request.OrderRef);
			var warnings = new List<string>();
			var errors = new List<string>();

			if(order.IsTerminated)
			{
				errors.Add(SR.CancelOrderWarning_OrderAlreadyTerminated);
				return new QueryCancelOrderWarningsResponse(warnings, errors);
			}

			var hasActiveReportingSteps = CollectionUtils.Contains(
				order.Procedures,
				p => CollectionUtils.Contains(p.ReportingProcedureSteps, ps => !ps.IsTerminated));

			if (hasActiveReportingSteps)
			{
				warnings.Add(SR.CancelOrderWarning_OrderPerformedMayHaveInProgressReports);
			}

			return new QueryCancelOrderWarningsResponse(warnings, errors);
		}

		[UpdateOperation]
		public TimeShiftOrderResponse TimeShiftOrder(TimeShiftOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			// load the order, explicitly ignoring the version (since this is only used for testing/demo data creation, we don't care)
			var order = this.PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);

			// shift the order, which will also shift all procedures, etc.
			order.TimeShift(request.NumberOfMinutes);

			// shift the visit
			order.Visit.TimeShift(request.NumberOfMinutes);

			this.PersistenceContext.SynchState();

			LogicalHL7Event.OrderModified.EnqueueEvents(order);

			var orderAssembler = new OrderAssembler();
			return new TimeShiftOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		public PrintDowntimeFormsResponse PrintDowntimeForms(PrintDowntimeFormsRequest request)
		{
			Platform.CheckArgumentRange(request.NumberOfForms, 1, 50, "NumberOfForms");

			var broker = this.PersistenceContext.GetBroker<IAccessionNumberBroker>();

			var q = from i in Enumerable.Range(0, request.NumberOfForms)
			        select new DowntimeFormPageModel(broker.GetNext());

			using (var printResult = PrintJob.Run(q.ToArray()))
			{
				var contents = File.ReadAllBytes(printResult.OutputFilePath);
				return new PrintDowntimeFormsResponse(contents);
			}
		}

		#endregion

		#region Operation Enablement

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as WorklistItemSummaryBase;
			return summary == null ? null : new WorklistItemKey(summary.OrderRef);
		}

		public bool CanReplaceOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Replace))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there is no order to cancel
			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);

			// the order can be replaced iff it can be cancelled/discontinued
			var operation = new CancelOrDiscontinueOrderOperation();
			return operation.CanExecute(order);
		}

		public bool CanMergeOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Merge))
				return false;

			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.Load<Order>(itemKey.OrderRef);
			return order.Status == OrderStatus.SC;
		}

		public bool CanUnmergeOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Unmerge))
				return false;

			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.Load<Order>(itemKey.OrderRef);
			return order.Status == OrderStatus.SC && order.MergeSourceOrders.Count > 0;
		}

		public bool CanModifyOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Modify))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there is no order to modify
			if (itemKey.OrderRef == null)
				return false;

			return true;
		}

		public bool CanCancelOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Cancel))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there is no order to cancel
			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);

			// cancel or discontinue
			var operation = new CancelOrDiscontinueOrderOperation();
			return operation.CanExecute(order);
		}

		#endregion

		private void ValidateOrderReplacable(Order order)
		{
			if (order.IsTerminated)
				throw new RequestValidationException(string.Format(SR.InvalidRequest_OrderInStatusCannotBeReplaced,
					EnumUtils.GetEnumValueInfo(order.Status, this.PersistenceContext)));

			if (CollectionUtils.Contains(order.Procedures, p => p.DowntimeRecoveryMode))
				throw new RequestValidationException(SR.InvalidRequest_DowntimeOrdersCannotBeReplaced);
		}

		private void ValidateVisitsExist(Order order)
		{
			try
			{
				var visitSearchCriteria = new VisitSearchCriteria();
				visitSearchCriteria.Patient.EqualTo(order.Patient);
				visitSearchCriteria.VisitNumber.AssigningAuthority.EqualTo(order.OrderingFacility.InformationAuthority);
				this.PersistenceContext.GetBroker<IVisitBroker>().FindOne(visitSearchCriteria);
			}
			catch (EntityNotFoundException)
			{
				throw new RequestValidationException(
					string.Format(SR.InvalidRequest_CannotEnterOrderWithoutVisits,
					order.OrderingFacility.InformationAuthority.Value));
			}
		}

		private Order PlaceOrderHelper(OrderRequisition requisition)
		{
			// get appropriate A# for this order
			var accNum = GetAccessionNumberForOrder(requisition);

			var patient = this.PersistenceContext.Load<Patient>(requisition.Patient.PatientRef, EntityLoadFlags.Proxy);
			var orderingFacility = this.PersistenceContext.Load<Facility>(requisition.OrderingFacility.FacilityRef, EntityLoadFlags.Proxy);
			var visit = FindOrCreateVisit(requisition, patient, orderingFacility, accNum);
			var orderingPhysician = this.PersistenceContext.Load<ExternalPractitioner>(requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
			var diagnosticService = this.PersistenceContext.Load<DiagnosticService>(requisition.DiagnosticService.DiagnosticServiceRef);
			var priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);


			var resultRecipients = CollectionUtils.Map(
				requisition.ResultRecipients ?? new List<ResultRecipientDetail>(),
				(ResultRecipientDetail s) => new ResultRecipient(
												this.PersistenceContext.Load<ExternalPractitionerContactPoint>(s.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy),
												EnumUtils.GetEnumValue<ResultCommunicationMode>(s.PreferredCommunicationMode)));

			// generate set of procedures
			// create a temp map from procedure back to its requisition, this will be needed later
			var orderAssembler = new OrderEntryAssembler();
			var mapProcToReq = new Dictionary<Procedure, ProcedureRequisition>();
			var procedureNumberBroker = PersistenceContext.GetBroker<IProcedureNumberBroker>();
			var dicomUidBroker = PersistenceContext.GetBroker<IDicomUidBroker>();
			var procedures = CollectionUtils.Map(
				requisition.Procedures ?? new List<ProcedureRequisition>(),
				delegate(ProcedureRequisition req)
				{
					var rpt = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
					var rp = new Procedure(rpt, procedureNumberBroker.GetNext(), dicomUidBroker.GetNewUid());
					mapProcToReq.Add(rp, req);

					// important to set this flag prior to creating the procedure steps, because it may affect
					// which procedure steps are created
					rp.DowntimeRecoveryMode = requisition.IsDowntimeOrder;
					return rp;
				});


			// generate a new order with the default set of procedures
			var order = Order.NewOrder(
				new OrderCreationArgs(
					Platform.Time,
					this.CurrentUserStaff,
					null,
					accNum,
					patient,
					visit,
					diagnosticService,
					requisition.ReasonForStudy,
					priority,
					orderingFacility,
					requisition.SchedulingRequestTime,
					orderingPhysician,
					resultRecipients,
					procedures),
				procedureNumberBroker,
				dicomUidBroker);

			// note: need to lock the new order now, prior to creating the procedure steps
			// otherwise may get exceptions saying the Procedure is a transient object
			this.PersistenceContext.Lock(order, DirtyState.New);

			// create procedure steps and update from requisition
			foreach (var procedure in order.Procedures)
			{
				procedure.CreateProcedureSteps();
				if(mapProcToReq.ContainsKey(procedure))
				{
					orderAssembler.UpdateProcedureFromRequisition(procedure, mapProcToReq[procedure], this.CurrentUserStaff, this.PersistenceContext);
				}
			}

			// add order notes
			if (requisition.Notes != null)
			{
				var noteAssembler = new OrderNoteAssembler();
				noteAssembler.SynchronizeOrderNotes(order, requisition.Notes, this.CurrentUserStaff, this.PersistenceContext);
			}

			// add attachments
			if(requisition.Attachments != null)
			{
				var attachmentAssembler = new OrderAttachmentAssembler();
				attachmentAssembler.Synchronize(order.Attachments, requisition.Attachments, this.CurrentUserStaff, this.PersistenceContext);
			}

			if (requisition.ExtendedProperties != null)
			{
				ExtendedPropertyUtils.Update(order.ExtendedProperties, requisition.ExtendedProperties);
			}

			return order;
		}

		private static void CancelOrderHelper(Order order, OrderCancelInfo info)
		{
			var operation = new CancelOrDiscontinueOrderOperation();
			operation.Execute(order, info);
		}

		private void MergeOrderHelper(Order destinationOrder, IEnumerable<Order> sourceOrders, OrderMergeInfo mergeInfo, bool validateOnly)
		{
			var sourceOrderAccessionNumbers = new List<string>();
			foreach (var sourceOrder in sourceOrders)
			{
				sourceOrderAccessionNumbers.Add(sourceOrder.AccessionNumber);

				string failureReason;
				if (!sourceOrder.CanMerge(mergeInfo, out failureReason))
					throw new RequestValidationException(failureReason);

				if (validateOnly)
					continue;

				// Merge the source order into the destination order.
				var result = sourceOrder.Merge(mergeInfo);

				// sync state so that ghost procedures get OIDs, prior to queuing ghost HL7 events
				PersistenceContext.SynchState();

				// create all necessary HL7 events
				foreach (var ghostProcedure in result.GhostProcedures)
				{
					LogicalHL7Event.ProcedureCancelled.EnqueueEvents(ghostProcedure);
					LogicalHL7Event.ProcedureCreated.EnqueueEvents(ghostProcedure.GhostOf);
				}
				LogicalHL7Event.OrderModified.EnqueueEvents(destinationOrder);
			}
		}

		private void UnmergeHelper(IEnumerable<Order> sourceOrders, OrderCancelInfo cancelInfo, IAccessionNumberBroker accBroker)
		{
			foreach (var order in sourceOrders)
			{
				string failureReason;
				if (!order.CanUnmerge(cancelInfo, out failureReason))
					throw new RequestValidationException(failureReason);

				var result = order.Unmerge(cancelInfo, accBroker.GetNext());
				var replacementOrder = result.ReplacementOrder;
				PersistenceContext.Lock(replacementOrder, DirtyState.New);

				// sync state so that ghost procedures get OIDs, prior to queuing ghost HL7 events
				PersistenceContext.SynchState();

				// notify HL7 of cancelled procedures (now existing as ghosts on dest order)
				foreach (var procedure in result.GhostProcedures)
				{
					LogicalHL7Event.ProcedureCancelled.EnqueueEvents(procedure);
				}

				// if the replacement order is not terminated
				if (!replacementOrder.IsTerminated)
				{
					// notify HL7 of replacement
					LogicalHL7Event.OrderCreated.EnqueueEvents(replacementOrder);

					// recur on items that were merged into this order
					UnmergeHelper(replacementOrder.MergeSourceOrders, cancelInfo, accBroker);
				}
			}
		}

		private static void DryRunHelper(bool dryRun, Action<object> action)
		{
			if (dryRun)
			{
				// create a new persistence scope, so that we do not use the scope inherited by the service
				using (var scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					action(null);

					// try to synch state to see if DB will accept changes
					scope.Context.SynchState();

					//note: do not call scope.Complete() under any circumstances - we want this transaction to rollback
				}
			}
			else
			{
				// just do the action in the usual scope
				action(null);
			}
		}

		private string GetAccessionNumberForOrder(OrderRequisition requisition)
		{
			// if this is a downtime requisition, validate the downtime A#, otherwise obtain a new A#
			var accessionBroker = this.PersistenceContext.GetBroker<IAccessionNumberBroker>();
			if (requisition.IsDowntimeOrder)
			{
				// validate that the downtime A# is less than then current sequence position
				var currentMaxAccession = accessionBroker.PeekNext();
				if (requisition.DowntimeAccessionNumber.CompareTo(currentMaxAccession) > -1)
					throw new RequestValidationException(SR.InvalidRequest_InvalidDowntimeAccessionNumber);

				return requisition.DowntimeAccessionNumber;
			}

			// get new A#
			return this.PersistenceContext.GetBroker<IAccessionNumberBroker>().GetNext();
		}

		private void UpdateProceduresHelper(Order order, IEnumerable<ProcedureRequisition> procedureReqs, ModifyOrderRequest request)
		{
			// do not update the procedures if the order is completed
			if (order.IsTerminated)
				return;

			var assembler = new OrderEntryAssembler();

			// if any procedure is in downtime recovery mode, assume the entire order is a "downtime order"
			var isDowntime = CollectionUtils.Contains(order.Procedures, p => p.DowntimeRecoveryMode);

			// separate the list into additions and updates
			var existingReqs = new List<ProcedureRequisition>();
			var addedReqs = new List<ProcedureRequisition>();

			foreach (var req in procedureReqs)
			{
				if (CollectionUtils.Contains(order.Procedures, x => req.ProcedureNumber == x.Number))
				{
					existingReqs.Add(req);
				}
				else
				{
					addedReqs.Add(req);
				}
			}

			// process the additions first, so that we don't accidentally cancel an order (if all its procedures are cancelled momentarily)
			var procedureNumberBroker = PersistenceContext.GetBroker<IProcedureNumberBroker>();
			var dicomUidBroker = PersistenceContext.GetBroker<IDicomUidBroker>();
			foreach (var req in addedReqs)
			{
				var requestedType = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);

				// create a new procedure for this requisition
				var procedure = new Procedure(requestedType, procedureNumberBroker.GetNext(), dicomUidBroker.GetNewUid()) { DowntimeRecoveryMode = isDowntime };
				order.AddProcedure(procedure);

				// note: need to lock the new procedure now, prior to creating the procedure steps
				// otherwise may get exceptions saying the Procedure is a transient object
				this.PersistenceContext.Lock(procedure, DirtyState.New);

				// create the procedure steps
				procedure.CreateProcedureSteps();

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(procedure, req, this.CurrentUserStaff, this.PersistenceContext);

				LogicalHL7Event.ProcedureCreated.EnqueueEvents(procedure);
			}

			// process updates
			foreach (var req in existingReqs)
			{
				var requestedType = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
				var procedure = CollectionUtils.SelectFirst(order.Procedures, x => req.ProcedureNumber == x.Number);

				// validate that the type has not changed
				if (!procedure.Type.Equals(requestedType))
					throw new RequestValidationException(SR.InvalidRequest_CannotModifyProcedureType);

				// If the procedure is already terminated, just move on to the next one since procedures cannot be "un-terminated".
				if (procedure.IsTerminated)
					continue;

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(procedure, req, this.CurrentUserStaff, this.PersistenceContext);

				(req.Cancelled ? LogicalHL7Event.ProcedureCancelled : LogicalHL7Event.ProcedureModified).EnqueueEvents(procedure);
			}
		}

		/// <summary>
		/// Creates duplicates of any attached documents in the order that also appear in the
		/// requisition, and then replaces the references in the requisition to refer to the
		/// duplicates.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="requisition"></param>
		private void DuplicateAttachmentsForOrderReplace(Order order, OrderRequisition requisition)
		{
			foreach (var attachment in order.Attachments)
			{
				var summary = CollectionUtils.SelectFirst(requisition.Attachments,
								  s => s.Document.DocumentRef.Equals(attachment.Document.GetRef(), true));

				if (summary != null)
				{
					var dup = attachment.Document.Duplicate(true);
					PersistenceContext.Lock(dup, DirtyState.New);
					summary.Document.DocumentRef = dup.GetRef();
				}
			}
		}

		/// <summary>
		/// Finds the visit specified in the requisition, or if no visit is specified, auto-generates a visit.
		/// </summary>
		/// <param name="requisition"></param>
		/// <param name="patient"></param>
		/// <param name="orderingFacility"></param>
		/// <returns></returns>
		private Visit FindOrCreateVisit(OrderRequisition requisition, Patient patient, Facility orderingFacility, string accessionNumber)
		{
			if (requisition.Visit != null && requisition.Visit.VisitRef != null)
			{
				return this.PersistenceContext.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);
			}

			// if Visit Workflow is disabled, then we must auto-generate a "dummy" visit in order to keep the system happy
			// the user will never see this dummy visit
			if (!new WorkflowConfigurationReader().EnableVisitWorkflow)
			{
				var patientClasses = PersistenceContext.GetBroker<IEnumBroker>().Load<PatientClassEnum>(false);

				// create a visit using the minimum possible amount of information
				var visit = new Visit
								{
									Patient = patient,
									VisitNumber = new VisitNumber(accessionNumber, orderingFacility.InformationAuthority),
									Status = VisitStatus.AA,
									AdmitTime = Platform.Time,
									Facility = orderingFacility,
									PatientClass = CollectionUtils.FirstElement(patientClasses)
								};

				this.PersistenceContext.Lock(visit, DirtyState.New);
				return visit;
			}

			throw new RequestValidationException(SR.InvalidRequest_VisitRequired);
		}
	}
}
