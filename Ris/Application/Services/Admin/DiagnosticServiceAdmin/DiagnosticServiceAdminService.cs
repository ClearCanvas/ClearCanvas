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
using System.Text;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IDiagnosticServiceAdminService))]
	public class DiagnosticServiceAdminService : ApplicationServiceBase, IDiagnosticServiceAdminService
	{
		#region IDiagnosticServiceAdminService Members

		[ReadOperation]
		public ListDiagnosticServicesResponse ListDiagnosticServices(ListDiagnosticServicesRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
			where.Id.SortAsc(0);
			if (!string.IsNullOrEmpty(request.Id))
				where.Id.StartsWith(request.Id);
			if (!string.IsNullOrEmpty(request.Name))
				where.Name.Like(string.Format("%{0}%", request.Name));
			if(!request.IncludeDeactivated)
				where.Deactivated.EqualTo(false);

			IDiagnosticServiceBroker broker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();
			IList<DiagnosticService> items = broker.Find(where, request.Page);

			DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();
			return new ListDiagnosticServicesResponse(
				CollectionUtils.Map<DiagnosticService, DiagnosticServiceSummary>(items,
					delegate(DiagnosticService item)
					{
						return assembler.CreateSummary(item);
					})
				);
		}

		[ReadOperation]
		public TextQueryResponse<DiagnosticServiceSummary> TextQuery(TextQueryRequest request)
		{
			IDiagnosticServiceBroker broker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();
			DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();

			TextQueryHelper<DiagnosticService, DiagnosticServiceSearchCriteria, DiagnosticServiceSummary> helper
				= new TextQueryHelper<DiagnosticService, DiagnosticServiceSearchCriteria, DiagnosticServiceSummary>(
                    delegate
					{
                        string rawQuery = request.TextQuery;

						IList<string> terms = TextQueryHelper.ParseTerms(rawQuery);
						List<DiagnosticServiceSearchCriteria> criteria = new List<DiagnosticServiceSearchCriteria>();

						// allow matching on name (assume entire query is a name which may contain spaces)
						DiagnosticServiceSearchCriteria nameCriteria = new DiagnosticServiceSearchCriteria();
						nameCriteria.Name.StartsWith(rawQuery);
						criteria.Add(nameCriteria);

						// allow matching of any term against ID
						criteria.AddRange(CollectionUtils.Map<string, DiagnosticServiceSearchCriteria>(terms,
									 delegate(string term)
									 {
										 DiagnosticServiceSearchCriteria c = new DiagnosticServiceSearchCriteria();
										 c.Id.StartsWith(term);
										 return c;
									 }));

						return criteria.ToArray();
					},
					delegate(DiagnosticService ds)
					{
						return assembler.CreateSummary(ds);
					},
					delegate(DiagnosticServiceSearchCriteria[] criteria, int threshold)
					{
						return broker.Count(criteria) <= threshold;
					},
					delegate(DiagnosticServiceSearchCriteria[] criteria, SearchResultPage page)
					{
						return broker.Find(criteria, page);
					});
			return helper.Query(request);
		}

		[ReadOperation]
		public LoadDiagnosticServiceForEditResponse LoadDiagnosticServiceForEdit(LoadDiagnosticServiceForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DiagnosticServiceRef, "request.DiagnosticServiceRef");

			DiagnosticService item = PersistenceContext.Load<DiagnosticService>(request.DiagnosticServiceRef);

			DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();
			return new LoadDiagnosticServiceForEditResponse(assembler.CreateDetail(item));
		}

		[ReadOperation]
		public LoadDiagnosticServiceEditorFormDataResponse LoadDiagnosticServiceEditorFormData(LoadDiagnosticServiceEditorFormDataRequest request)
		{
			ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);
			where.Deactivated.EqualTo(false);

			IList<ProcedureType> procTypes = PersistenceContext.GetBroker<IProcedureTypeBroker>().Find(where);

			ProcedureTypeAssembler assembler = new ProcedureTypeAssembler();
			return new LoadDiagnosticServiceEditorFormDataResponse(
				CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(procTypes,
					delegate(ProcedureType pt) { return assembler.CreateSummary(pt); }));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.DiagnosticService)]
		public AddDiagnosticServiceResponse AddDiagnosticService(AddDiagnosticServiceRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DiagnosticService, "request.DiagnosticService");

			DiagnosticService item = new DiagnosticService();

			DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();
			assembler.UpdateDiagnosticService(item, request.DiagnosticService, PersistenceContext);

			PersistenceContext.Lock(item, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddDiagnosticServiceResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.DiagnosticService)]
		public UpdateDiagnosticServiceResponse UpdateDiagnosticService(UpdateDiagnosticServiceRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DiagnosticService, "request.DiagnosticService");
			Platform.CheckMemberIsSet(request.DiagnosticService.DiagnosticServiceRef, "request.DiagnosticService.DiagnosticServiceRef");

			DiagnosticService item = PersistenceContext.Load<DiagnosticService>(request.DiagnosticService.DiagnosticServiceRef);

			DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();
			assembler.UpdateDiagnosticService(item, request.DiagnosticService, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateDiagnosticServiceResponse(assembler.CreateSummary(item));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.DiagnosticService)]
		public DeleteDiagnosticServiceResponse DeleteDiagnosticService(DeleteDiagnosticServiceRequest request)
		{
			try
			{
				IDiagnosticServiceBroker broker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();
				DiagnosticService item = broker.Load(request.DiagnosticServiceRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteDiagnosticServiceResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(DiagnosticService))));
			}
		}

		#endregion
	}
}
