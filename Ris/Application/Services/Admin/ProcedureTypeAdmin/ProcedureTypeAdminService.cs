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
using System.Linq;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IProcedureTypeAdminService))]
	public class ProcedureTypeAdminService : ApplicationServiceBase, IProcedureTypeAdminService
	{
		#region IProcedureTypeAdminService Members

		[ReadOperation]
		public TextQueryResponse<ProcedureTypeSummary> TextQuery(TextQueryRequest request)
		{
			var broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
			var assembler = new ProcedureTypeAssembler();

			var helper = new TextQueryHelper<ProcedureType, ProcedureTypeSearchCriteria, ProcedureTypeSummary>(
					delegate
					{
						var rawQuery = request.TextQuery;

						IList<string> terms = TextQueryHelper.ParseTerms(rawQuery);
						var criteria = new List<ProcedureTypeSearchCriteria>();

						// allow matching on name (assume entire query is a name which may contain spaces)
						var nameCriteria = new ProcedureTypeSearchCriteria();
						nameCriteria.Name.StartsWith(rawQuery);
						criteria.Add(nameCriteria);

						// allow matching of any term against ID
						criteria.AddRange(CollectionUtils.Map(terms,
									 delegate(string term)
									 {
										 var c = new ProcedureTypeSearchCriteria();
										 c.Id.StartsWith(term);
										 return c;
									 }));

						return criteria.ToArray();
					},
					assembler.CreateSummary,
					(criteria, threshold) => broker.Count(criteria) <= threshold,
					broker.Find);

			return helper.Query(request);
		}

		[ReadOperation]
		public ListProcedureTypesResponse ListProcedureTypes(ListProcedureTypesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			if (!string.IsNullOrEmpty(request.Id))
				where.Id.StartsWith(request.Id);
			if (!string.IsNullOrEmpty(request.Name))
				where.Name.Like(string.Format("%{0}%", request.Name));
			if (!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

			var broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
			var items = broker.Find(where, request.Page);

			var assembler = new ProcedureTypeAssembler();
			return new ListProcedureTypesResponse(CollectionUtils.Map(items, (ProcedureType item) => assembler.CreateSummary(item)));
		}

		[ReadOperation]
		public LoadProcedureTypeForEditResponse LoadProcedureTypeForEdit(LoadProcedureTypeForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureTypeRef, "request.ProcedureTypeRef");

			var item = PersistenceContext.Load<ProcedureType>(request.ProcedureTypeRef);

			var assembler = new ProcedureTypeAssembler();
			return new LoadProcedureTypeForEditResponse(assembler.CreateDetail(item, PersistenceContext));
		}

		[ReadOperation]
		public LoadProcedureTypeEditorFormDataResponse LoadProcedureTypeEditorFormData(LoadProcedureTypeEditorFormDataRequest request)
		{
			var where = new ModalitySearchCriteria();
			where.Name.SortAsc(0);
			where.Deactivated.EqualTo(false);

			var modalities = PersistenceContext.GetBroker<IModalityBroker>().Find(where);

			var assembler = new ModalityAssembler();
			return new LoadProcedureTypeEditorFormDataResponse(modalities.Select(assembler.CreateModalitySummary).ToList());
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public AddProcedureTypeResponse AddProcedureType(AddProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");

			var item = new ProcedureType();

			var assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.Lock(item, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public UpdateProcedureTypeResponse UpdateProcedureType(UpdateProcedureTypeRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ProcedureType, "request.ProcedureType");
			Platform.CheckMemberIsSet(request.ProcedureType.ProcedureTypeRef, "request.ProcedureType.ProcedureTypeRef");

			var item = PersistenceContext.Load<ProcedureType>(request.ProcedureType.ProcedureTypeRef);

			var assembler = new ProcedureTypeAssembler();
			assembler.UpdateProcedureType(item, request.ProcedureType, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateProcedureTypeResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ProcedureType)]
		public DeleteProcedureTypeResponse DeleteProcedureType(DeleteProcedureTypeRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
				var item = broker.Load(request.ProcedureTypeRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteProcedureTypeResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ProcedureType))));
			}
		}

		#endregion
	}
}
