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

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	/// <summary>
	/// Provides operations to administer authority groups.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
    public interface IAuthorityGroupAdminService
	{		
		/// <summary>
		/// Add a new authority group
		/// </summary>
		/// <param name="request"><see cref="AddAuthorityGroupRequest"/></param>
		/// <returns><see cref="AddAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request);

		/// <summary>
		/// Updates an authority group
		/// </summary>
		/// <param name="request"><see cref="UpdateAuthorityGroupRequest"/></param>
		/// <returns><see cref="UpdateAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(UserAccessDeniedException))]
        UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request);

		/// <summary>
		/// Deletes an authority group
		/// </summary>
		/// <param name="request"><see cref="DeleteAuthorityGroupRequest"/></param>
		/// <returns><see cref="DeleteAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(AuthorityGroupIsNotEmptyException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request);

		/// <summary>
		/// Load details for a specified authority group
		/// </summary>
		/// <param name="request"><see cref="LoadAuthorityGroupForEditRequest"/></param>
		/// <returns><see cref="LoadAuthorityGroupForEditResponse"/></returns>
		[OperationContract]
		LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request);

		/// <summary>
		/// Provides a list of all available authority tokens
		/// </summary>
		/// <param name="request"><see cref="ListAuthorityTokensRequest"/></param>
		/// <returns><see cref="ListAuthorityTokensResponse"/></returns>
		[OperationContract]
		ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request);

		/// <summary>
		/// Imports authority tokens from a remote source.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request);

		/// <summary>
		/// Imports authority groups from a remote source.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request);

        /// <summary>
        /// Summary list of all authority groups
        /// </summary>
        /// <param name="request"><see cref="ListAuthorityGroupsRequest"/></param>
        /// <returns><see cref="ListAuthorityGroupsResponse"/></returns>
        [OperationContract]
        ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request);
	}
}