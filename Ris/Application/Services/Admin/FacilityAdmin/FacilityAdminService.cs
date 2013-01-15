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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IFacilityAdminService))]
	public class FacilityAdminService : ApplicationServiceBase, IFacilityAdminService
	{
		#region IFacilityAdminService Members

		[ReadOperation]
		public ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request)
		{
			var criteria = new FacilitySearchCriteria();
			criteria.Code.SortAsc(0);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			var assembler = new FacilityAssembler();
			return new ListAllFacilitiesResponse(
				CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
					PersistenceContext.GetBroker<IFacilityBroker>().Find(criteria, request.Page),
					assembler.CreateFacilitySummary));
		}

		[ReadOperation]
		public GetFacilityEditFormDataResponse GetFacilityEditFormData(GetFacilityEditFormDataRequest request)
		{
			return new GetFacilityEditFormDataResponse(EnumUtils.GetEnumValueList<InformationAuthorityEnum>(PersistenceContext));
		}

		[ReadOperation]
		public LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request)
		{
			// note that the version of the FacilityRef is intentionally ignored here (default behaviour of ReadOperation)
			var f = PersistenceContext.Load<Facility>(request.FacilityRef);
			var assembler = new FacilityAssembler();

			return new LoadFacilityForEditResponse(assembler.CreateFacilityDetail(f));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
		public AddFacilityResponse AddFacility(AddFacilityRequest request)
		{
			var facility = new Facility();
			var assembler = new FacilityAssembler();
			assembler.UpdateFacility(request.FacilityDetail, facility, this.PersistenceContext);

			PersistenceContext.Lock(facility, DirtyState.New);

			CheckMultipleInformationAuthoritiesUsed();

			// ensure the new facility is assigned an OID before using it in the return value
			PersistenceContext.SynchState();

			return new AddFacilityResponse(assembler.CreateFacilitySummary(facility));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
		public UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request)
		{
			var facility = PersistenceContext.Load<Facility>(request.FacilityDetail.FacilityRef, EntityLoadFlags.CheckVersion);

			var assembler = new FacilityAssembler();
			assembler.UpdateFacility(request.FacilityDetail, facility, this.PersistenceContext);

			CheckMultipleInformationAuthoritiesUsed();

			return new UpdateFacilityResponse(assembler.CreateFacilitySummary(facility));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Facility)]
		public DeleteFacilityResponse DeleteFacility(DeleteFacilityRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IFacilityBroker>();
				var item = broker.Load(request.FacilityRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteFacilityResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Facility))));
			}
		}

		#endregion

		private void CheckMultipleInformationAuthoritiesUsed()
		{
			var workflowConfig = new WorkflowConfigurationReader();

			// if we're not generating the MRNs, then this doesn't apply
			if (!workflowConfig.AutoGenerateMrn)
				return;

			var iaInUse = PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false)
				.Select(f => f.InformationAuthority)
				.Distinct()
				.Count();

			if(iaInUse > 1)
				throw new RequestValidationException(SR.MessageMultipleInformationAuthoritiesNotSupported);
		}

	}
}
