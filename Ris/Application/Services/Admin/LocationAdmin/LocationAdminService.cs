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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.LocationAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ILocationAdminService))]
    public class LocationAdminService : ApplicationServiceBase, ILocationAdminService
    {
        #region ILocationAdminService Members

        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request)
        {
            LocationSearchCriteria criteria = new LocationSearchCriteria();
			criteria.Id.SortAsc(0);
			if (request.Facility != null)
				criteria.Facility.EqualTo(PersistenceContext.Load<Facility>(request.Facility.FacilityRef));
			if (!string.IsNullOrEmpty(request.Name))
				criteria.Name.StartsWith(request.Name);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

            LocationAssembler assembler = new LocationAssembler();
            return new ListAllLocationsResponse(
                CollectionUtils.Map<Location, LocationSummary, List<LocationSummary>>(
                    PersistenceContext.GetBroker<ILocationBroker>().Find(criteria, request.Page),
                    delegate(Location l)
                    {
                        return assembler.CreateLocationSummary(l);
                    }));
        }

        [ReadOperation]
        public GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request)
        {
            FacilityAssembler assembler = new FacilityAssembler();
            return new GetLocationEditFormDataResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
                    delegate(Facility f)
                    {
                        return assembler.CreateFacilitySummary(f);
                    }));
        }

        [ReadOperation]
        public LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request)
        {
            // note that the version of the LocationRef is intentionally ignored here (default behaviour of ReadOperation)
            Location l = PersistenceContext.Load<Location>(request.LocationRef);
            LocationAssembler assembler = new LocationAssembler();

            return new LoadLocationForEditResponse(assembler.CreateLocationDetail(l));
        }

        /// <summary>
        /// Add the specified location
        /// </summary>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Location)]
        public AddLocationResponse AddLocation(AddLocationRequest request)
        {
            Location location = new Location();

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(request.LocationDetail, location, PersistenceContext);

            PersistenceContext.Lock(location, DirtyState.New);

            // ensure the new location is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddLocationResponse(assembler.CreateLocationSummary(location));
        }


        /// <summary>
        /// Update the specified location
        /// </summary>
        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Location)]
		public UpdateLocationResponse UpdateLocation(UpdateLocationRequest request)
        {
            Location location = PersistenceContext.Load<Location>(request.LocationDetail.LocationRef, EntityLoadFlags.CheckVersion);

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(request.LocationDetail, location, PersistenceContext);

            return new UpdateLocationResponse(assembler.CreateLocationSummary(location));
        }

		/// <summary>
		/// Delete the specified location
		/// </summary>
		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Location)]
		public DeleteLocationResponse DeleteLocation(DeleteLocationRequest request)
		{
			try
			{
				ILocationBroker broker = PersistenceContext.GetBroker<ILocationBroker>();
				Location item = broker.Load(request.LocationRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteLocationResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(Location))));
			}
		}

		#endregion
    }
}
