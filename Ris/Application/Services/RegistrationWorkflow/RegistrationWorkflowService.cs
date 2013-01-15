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
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	[ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
	{
		#region IRegistrationWorkflowService Members

		[ReadOperation]
		[AuditRecorder(typeof(WorkflowServiceRecorder.SearchWorklists))]
		public TextQueryResponse<RegistrationWorklistItemSummary> SearchWorklists(WorklistItemTextQueryRequest request)
		{
			var assembler = new RegistrationWorkflowAssembler();
			var broker = this.PersistenceContext.GetBroker<IRegistrationWorklistItemBroker>();

			return SearchHelper<WorklistItem, RegistrationWorklistItemSummary>(
				request,
				broker,
				WorklistItemProjection.RegistrationWorklistSearch,
				item => assembler.CreateWorklistItemSummary(item, PersistenceContext));
		}

		[ReadOperation]
		[AuditRecorder(typeof(WorkflowServiceRecorder.SearchPatientProfiles))]
		public TextQueryResponse<PatientProfileSummary> PatientProfileTextQuery(TextQueryRequest request)
		{
			var helper = new PatientProfileTextQueryHelper(this.PersistenceContext);
			return helper.Query(request);
		}

		[ReadOperation]
		[ResponseCaching("GetQueryWorklistCacheDirective")]
		public QueryWorklistResponse<RegistrationWorklistItemSummary> QueryWorklist(QueryWorklistRequest request)
		{
			var assembler = new RegistrationWorkflowAssembler();

			return QueryWorklistHelper<WorklistItem, RegistrationWorklistItemSummary>(
				request,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
		}

		[ReadOperation]
		public ListProceduresForCheckInResponse ListProceduresForCheckIn(ListProceduresForCheckInRequest request)
		{
			var proceduresNotCheckedIn = GetProceduresEligibleForCheckIn(request.OrderRef);

			var assembler = new ProcedureAssembler();
			return new ListProceduresForCheckInResponse(
				CollectionUtils.Map<Procedure, ProcedureSummary, List<ProcedureSummary>>(
					proceduresNotCheckedIn,
					procedure => assembler.CreateProcedureSummary(procedure, this.PersistenceContext)));
		}

		[UpdateOperation]
		[OperationEnablement("CanCheckInProcedure")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Procedure.CheckIn)]
		public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
		{
			var broker = PersistenceContext.GetBroker<IProcedureBroker>();
			var op = new Operations.CheckIn();
			foreach (var procedureRef in request.Procedures)
			{
				var procedure = broker.Load(procedureRef, EntityLoadFlags.CheckVersion);
				op.Execute(procedure, this.CurrentUserStaff, request.CheckInTime, new PersistentWorkflow(this.PersistenceContext));

				LogicalHL7Event.ProcedureModified.EnqueueEvents(procedure);
			}

			return new CheckInProcedureResponse();
		}

		#endregion

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as RegistrationWorklistItemSummary;
			return summary == null ? null : new RegistrationWorklistItemKey(summary.OrderRef, summary.PatientProfileRef);
		}

		public bool CanCheckInProcedure(RegistrationWorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Procedure.CheckIn))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there are no procedures to check-in
			if (itemKey.OrderRef == null)
				return false;

			return GetProceduresEligibleForCheckIn(itemKey.OrderRef).Count > 0;
		}

		private List<Procedure> GetProceduresEligibleForCheckIn(EntityRef orderRef)
		{
			var order = this.PersistenceContext.Load<Order>(orderRef, EntityLoadFlags.Proxy);

			return CollectionUtils.Select(
				order.Procedures,
				p => p.IsPreCheckIn && !p.IsTerminated);
		}
	}
}
