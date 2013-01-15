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

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    /// <summary>
    /// Provides operations to administer staff groups.
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IStaffGroupAdminService
    {
        /// <summary>
        /// Lists staff groups based on a text query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<StaffGroupSummary> TextQuery(StaffGroupTextQueryRequest request);

        /// <summary>
        /// Summary list of all staff groups.
        /// </summary>
        [OperationContract]
        ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request);

        /// <summary>
        /// Loads details of specified staff group for editing.
        /// </summary>
        [OperationContract]
        LoadStaffGroupForEditResponse LoadStaffGroupForEdit(LoadStaffGroupForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a staff group.
        /// </summary>
        [OperationContract]
        LoadStaffGroupEditorFormDataResponse LoadStaffGroupEditorFormData(LoadStaffGroupEditorFormDataRequest request);

        /// <summary>
        /// Adds a new staff group.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddStaffGroupResponse AddStaffGroup(AddStaffGroupRequest request);

        /// <summary>
        /// Updates a staff group.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateStaffGroupResponse UpdateStaffGroup(UpdateStaffGroupRequest request);

		/// <summary>
		/// Deletes a staff group.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteStaffGroupResponse DeleteStaffGroup(DeleteStaffGroupRequest request);
	}
}
