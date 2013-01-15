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

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    /// <summary>
    /// Provides services for administration of persistent worklist definitions
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IWorklistAdminService
    {
        /// <summary>
        /// Returns a list of worklist categories.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListWorklistCategoriesResponse ListWorklistCategories(ListWorklistCategoriesRequest request);

        /// <summary>
        /// Returns a list of worklists matching the specified criteria.
        /// </summary>
        /// <param name="request"><see cref="ListWorklistsRequest"/></param>
        /// <returns><see cref="ListWorklistsResponse"/></returns>
        [OperationContract]
        ListWorklistClassesResponse ListWorklistClasses(ListWorklistClassesRequest request);
        
        /// <summary>
        /// Returns a list of worklists matching the specified criteria.
        /// </summary>
        /// <param name="request"><see cref="ListWorklistsRequest"/></param>
        /// <returns><see cref="ListWorklistsResponse"/></returns>
        [OperationContract]
        ListWorklistsResponse ListWorklists(ListWorklistsRequest request);

        /// <summary>
        /// Returns a list of ProcedureTypeGroups of a specified class.
        /// </summary>
        /// <param name="request"><see cref="ListProcedureTypeGroupsRequest"/></param>
        /// <returns><see cref="ListProcedureTypeGroupsResponse"/></returns>
        [OperationContract]
        ListProcedureTypeGroupsResponse ListProcedureTypeGroups(ListProcedureTypeGroupsRequest request);

        /// <summary>
        /// Returns data suitable for populating a form for the purpose of editing a worklist definition
        /// </summary>
        /// <param name="request"><see cref="GetWorklistEditFormDataRequest"/></param>
        /// <returns><see cref="GetWorklistEditFormDataResponse"/></returns>
        [OperationContract]
        GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request);

        /// <summary>
        /// Loads a worklist definition for editing
        /// </summary>
        /// <param name="request"><see cref="LoadWorklistForEditRequest"/></param>
        /// <returns><see cref="LoadWorklistForEditResponse"/></returns>
        [OperationContract]
        LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request);

        /// <summary>
        /// Adds a new worklist
        /// </summary>
        /// <param name="request"><see cref="AddWorklistRequest"/></param>
        /// <returns><see cref="AddWorklistResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddWorklistResponse AddWorklist(AddWorklistRequest request);

        /// <summary>
        /// Updates an existing worklist
        /// </summary>
        /// <param name="request"><see cref="UpdateWorklistRequest"/></param>
        /// <returns><see cref="UpdateWorklistResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request);

        /// <summary>
        /// Deletes an existing worklist.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        DeleteWorklistResponse DeleteWorklist(DeleteWorklistRequest request);
    }
}
