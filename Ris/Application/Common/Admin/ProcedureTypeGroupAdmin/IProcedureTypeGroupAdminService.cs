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

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin
{
    /// <summary>
    /// 
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IProcedureTypeGroupAdminService
    {
        /// <summary>
		/// Loads details of specified item for editing.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProcedureTypeGroupEditFormDataResponse GetProcedureTypeGroupEditFormData(
            GetProcedureTypeGroupEditFormDataRequest request);

        /// <summary>
		/// Loads details of specified item for editing.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		GetProcedureTypeGroupSummaryFormDataResponse GetProcedureTypeGroupSummaryFormData(
			GetProcedureTypeGroupSummaryFormDataRequest request);

        /// <summary>
		/// Summary list of all items.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProcedureTypeGroupsResponse ListProcedureTypeGroups(
            ListProcedureTypeGroupsRequest request);

        /// <summary>
		/// Loads all form data needed to edit an item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProcedureTypeGroupForEditResponse LoadProcedureTypeGroupForEdit(
            LoadProcedureTypeGroupForEditRequest request);

        /// <summary>
		/// Adds a new item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddProcedureTypeGroupResponse AddProcedureTypeGroup(
            AddProcedureTypeGroupRequest request);

        /// <summary>
		/// Updates an item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateProcedureTypeGroupResponse UpdateProcedureTypeGroup(
            UpdateProcedureTypeGroupRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProcedureTypeGroupResponse DeleteProcedureTypeGroup(
			DeleteProcedureTypeGroupRequest request);
	}
}