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

using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.Admin.ProtocolAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IProtocolAdminService))]
	public class ProtocolAdminService : ApplicationServiceBase, IProtocolAdminService
	{
		#region IProtocolAdminService Members

		[ReadOperation]
		public ListProtocolCodesResponse ListProtocolCodes(ListProtocolCodesRequest request)
		{
			var where = new ProtocolCodeSearchCriteria();
			where.Name.SortAsc(0);
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

			var codes = this.PersistenceContext.GetBroker<IProtocolCodeBroker>().Find(where, request.Page);

			var assembler = new ProtocolGroupAssembler();
			return new ListProtocolCodesResponse(
				CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
					codes,
					code => assembler.GetProtocolCodeSummary(code)));
		}

		[ReadOperation]
		public LoadProtocolCodeForEditResponse LoadProtocolCodeForEdit(LoadProtocolCodeForEditRequest request)
		{
			var code = this.PersistenceContext.Load<ProtocolCode>(request.ProtocolCodeRef);

			var assembler = new ProtocolGroupAssembler();
			return new LoadProtocolCodeForEditResponse(assembler.GetProtocolCodeDetail(code));
		}

		[UpdateOperation]
		public AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request)
		{
			var assembler = new ProtocolGroupAssembler();
			var protocolCode = new ProtocolCode();
			assembler.UpdateProtocolCode(protocolCode, request.ProtocolCode);
			this.PersistenceContext.Lock(protocolCode, DirtyState.New);

			this.PersistenceContext.SynchState();

			return new AddProtocolCodeResponse(assembler.GetProtocolCodeSummary(protocolCode));
		}

		[UpdateOperation]
		public UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request)
		{
			var code = this.PersistenceContext.Load<ProtocolCode>(request.ProtocolCode.ProtocolCodeRef);
			var assembler = new ProtocolGroupAssembler();
			assembler.UpdateProtocolCode(code, request.ProtocolCode);

			this.PersistenceContext.SynchState();

			return new UpdateProtocolCodeResponse(assembler.GetProtocolCodeSummary(code));
		}

		[UpdateOperation]
		public DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request)
		{
			try
			{
				var broker = this.PersistenceContext.GetBroker<IProtocolCodeBroker>();
				var item = broker.Load(request.ProtocolCodeRef, EntityLoadFlags.Proxy);
				broker.Delete(item);

				this.PersistenceContext.SynchState();

				return new DeleteProtocolCodeResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete,
					TerminologyTranslator.Translate(typeof(ProtocolCode))));
			}
		}

		[ReadOperation]
		public ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request)
		{
			var where = new ProtocolGroupSearchCriteria();
			where.Name.SortAsc(0);

			var protocolGroups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
				this.PersistenceContext.GetBroker<IProtocolGroupBroker>().Find(where, request.Page),
				pg => new ProtocolGroupSummary(pg.GetRef(), pg.Name, pg.Description));

			return new ListProtocolGroupsResponse(protocolGroups);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
		public LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request)
		{
			var group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

			var assembler = new ProtocolGroupAssembler();
			return new LoadProtocolGroupForEditResponse(group.GetRef(), assembler.GetProtocolGroupDetail(group, this.PersistenceContext));
		}

		[ReadOperation]
		public GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(
			GetProtocolGroupEditFormDataRequest request)
		{
			var protocolAssembler = new ProtocolGroupAssembler();
			var codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
				this.PersistenceContext.GetBroker<IProtocolCodeBroker>().FindAll(false),
				code => protocolAssembler.GetProtocolCodeSummary(code));

			var procedureTypeGroupAssembler = new ProcedureTypeGroupAssembler();

			var readingGroups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
				this.PersistenceContext.GetBroker<IReadingGroupBroker>().FindAll(),
				readingGroup => procedureTypeGroupAssembler.GetProcedureTypeGroupSummary(readingGroup, this.PersistenceContext));

			return new GetProtocolGroupEditFormDataResponse(codes, readingGroups);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
		public AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request)
		{
			var assembler = new ProtocolGroupAssembler();

			var group = new ProtocolGroup();
			assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

			this.PersistenceContext.Lock(group, DirtyState.New);
			this.PersistenceContext.SynchState();

			return new AddProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
		public UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request)
		{
			var group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

			var assembler = new ProtocolGroupAssembler();
			assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

			this.PersistenceContext.SynchState();

			return new UpdateProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
		public DeleteProtocolGroupResponse DeleteProtocolGroup(DeleteProtocolGroupRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IProtocolGroupBroker>();
				var item = broker.Load(request.ProtocolGroupRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				this.PersistenceContext.SynchState();
				return new DeleteProtocolGroupResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ProtocolGroup))));
			}
		}

		#endregion
	}
}
