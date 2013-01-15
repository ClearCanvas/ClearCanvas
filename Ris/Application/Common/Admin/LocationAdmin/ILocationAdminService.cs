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
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    /// <summary>
    /// Provides operations to administer locations
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface ILocationAdminService
    {
        /// <summary>
        /// Summary list of all locations
        /// </summary>
        /// <param name="request"><see cref="ListAllLocationsRequest"/></param>
        /// <returns><see cref="ListAllLocationsResponse"/></returns>
        [OperationContract]
        ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request);

        /// <summary>
        /// Add a new location.  A location with the same content as an existing location cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddLocationRequest"/></param>
        /// <returns><see cref="AddLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddLocationResponse AddLocation(AddLocationRequest request);

        /// <summary>
        /// Update a new location.  A location with the same content as an existing location cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateLocationRequest"/></param>
        /// <returns><see cref="UpdateLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateLocationResponse UpdateLocation(UpdateLocationRequest request);

		/// <summary>
		/// Delete a location.
		/// </summary>
		/// <param name="request"><see cref="DeleteLocationRequest"/></param>
		/// <returns><see cref="DeleteLocationResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteLocationResponse DeleteLocation(DeleteLocationRequest request);

		/// <summary>
        /// Loads all form data needed to edit a location
        /// </summary>
        /// <param name="request"><see cref="GetLocationEditFormDataRequest"/></param>
        /// <returns><see cref="GetLocationEditFormDataResponse"/></returns>
        [OperationContract]
        GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request);

        /// <summary>
        /// Load details for a specified location
        /// </summary>
        /// <param name="request"><see cref="LoadLocationForEditRequest"/></param>
        /// <returns><see cref="LoadLocationForEditResponse"/></returns>
        [OperationContract]
        LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request);
    }
}
