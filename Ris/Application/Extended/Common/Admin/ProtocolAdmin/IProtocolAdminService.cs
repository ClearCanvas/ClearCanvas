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
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin
{
    /// <summary>
    /// Provides operations to administer protocol codes and protocol groups
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IProtocolAdminService
    {
		/// <summary>
		/// Lists all protocol codes.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
    	ListProtocolCodesResponse ListProtocolCodes(ListProtocolCodesRequest request);

		/// <summary>
		/// Loads protocol code for editing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		LoadProtocolCodeForEditResponse LoadProtocolCodeForEdit(LoadProtocolCodeForEditRequest request);

		/// <summary>
        /// Adds a new protocol code with specified name and description (optional)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request);

        /// <summary>
        /// Updates name and/or description of specified protocol code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request);

        /// <summary>
        /// Marks a protocol code as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request);

        /// <summary>
        /// Summary list of all protocol groups
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request);

        /// <summary>
        /// Loads details for specified protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request);

        /// <summary>
        /// Provides a list of available protocol codes and reading groups that can be assigned while adding/updating a protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(GetProtocolGroupEditFormDataRequest request);

        /// <summary>
        /// Adds a new protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request);

        /// <summary>
        /// Updates an existing protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request);

        /// <summary>
        /// Deletes an existing protocol
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProtocolGroupResponse DeleteProtocolGroup(DeleteProtocolGroupRequest request);
    }
}
