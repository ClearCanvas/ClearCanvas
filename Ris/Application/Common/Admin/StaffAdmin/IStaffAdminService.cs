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

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    /// <summary>
    /// Provides operations to administer staffs
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    [ServiceKnownType(typeof(StaffTextQueryRequest))]
    public interface IStaffAdminService
    {
        /// <summary>
        /// Returns a list of staff based on a textual query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<StaffSummary> TextQuery(StaffTextQueryRequest request);

        /// <summary>
        /// Summary list of all staffs
        /// </summary>
        /// <param name="request"><see cref="ListStaff"/></param>
        /// <returns><see cref="ListStaff"/></returns>
        [OperationContract]
        ListStaffResponse ListStaff(ListStaffRequest request);

        /// <summary>
        /// Add a new staff.  A staff with the same name as an existing staff cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddStaffRequest"/></param>
        /// <returns><see cref="AddStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddStaffResponse AddStaff(AddStaffRequest request);

        /// <summary>
        /// Update a new staff.  A staff with the same name as an existing staff cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateStaffRequest"/></param>
        /// <returns><see cref="UpdateStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateStaffResponse UpdateStaff(UpdateStaffRequest request);

		/// <summary>
		/// Delete a staff.
		/// </summary>
		/// <param name="request"><see cref="DeleteStaffRequest"/></param>
		/// <returns><see cref="DeleteStaffResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteStaffResponse DeleteStaff(DeleteStaffRequest request);

        /// <summary>
        /// Load details for a specified staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffForEditRequest"/></param>
        /// <returns><see cref="LoadStaffForEditResponse"/></returns>
        [OperationContract]
        LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffEditorFormDataRequest"/></param>
        /// <returns><see cref="LoadStaffEditorFormDataResponse"/></returns>
        [OperationContract]
        LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request);
    }
}
