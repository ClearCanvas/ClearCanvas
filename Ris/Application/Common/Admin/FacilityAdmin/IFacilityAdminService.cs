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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    /// <summary>
    /// Provides operations to administer facilities
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IFacilityAdminService
    {
        /// <summary>
        /// Summary list of all facilities
        /// </summary>
        /// <param name="request"><see cref="ListAllFacilitiesRequest"/></param>
        /// <returns><see cref="ListAllFacilitiesResponse"/></returns>
        [OperationContract]
        ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request);

        /// <summary>
        /// Add a new facility.  A facility with the same code as an existing facility cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddFacilityRequest"/></param>
        /// <returns><see cref="AddFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddFacilityResponse AddFacility(AddFacilityRequest request);

        /// <summary>
        /// Update a new facility.  A facility with the same code as an existing facility cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateFacilityRequest"/></param>
        /// <returns><see cref="UpdateFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request);

		/// <summary>
		/// Delete a facility.
		/// </summary>
		/// <param name="request"><see cref="DeleteFacilityRequest"/></param>
		/// <returns><see cref="DeleteFacilityResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteFacilityResponse DeleteFacility(DeleteFacilityRequest request);
		
		/// <summary>
        /// Load details for a specified facility
        /// </summary>
        /// <param name="request"><see cref="LoadFacilityForEditRequest"/></param>
        /// <returns><see cref="LoadFacilityForEditResponse"/></returns>
        [OperationContract]
        LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a location
        /// </summary>
        /// <param name="request"><see cref="GetFacilityEditFormDataRequest"/></param>
        /// <returns><see cref="GetFacilityEditFormDataResponse"/></returns>
        [OperationContract]
        GetFacilityEditFormDataResponse GetFacilityEditFormData(GetFacilityEditFormDataRequest request);
    }
}
