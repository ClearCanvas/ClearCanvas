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

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	/// <summary>
	/// Provides operations to administer user accounts and authority groups
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	public interface IUserAdminService
	{
		/// <summary>
		/// Summary list of all user accounts
		/// </summary>
		/// <param name="request"><see cref="ListUsersRequest"/></param>
		/// <returns><see cref="ListUsersResponse"/></returns>
		[OperationContract]
		ListUsersResponse ListUsers(ListUsersRequest request);

		/// <summary>
		/// Add a new user account
		/// </summary>
		/// <param name="request"><see cref="AddUserRequest"/></param>
		/// <returns><see cref="AddUserResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddUserResponse AddUser(AddUserRequest request);

		/// <summary>
		/// Updates a user account.  The UserID cannot be updated
		/// </summary>
		/// <param name="request"><see cref="UpdateUserRequest"/></param>
		/// <returns><see cref="UpdateUserResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateUserResponse UpdateUser(UpdateUserRequest request);

		/// <summary>
		/// Deletes a user account.
		/// </summary>
		/// <param name="request"><see cref="DeleteUserRequest"/></param>
		/// <returns><see cref="DeleteUserResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteUserResponse DeleteUser(DeleteUserRequest request);

		/// <summary>
		/// Resets a user's password to the temporary password.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request);

		/// <summary>
		/// Load details for a specified user account.
		/// </summary>
		/// <param name="request"><see cref="LoadUserForEditRequest"/></param>
		/// <returns><see cref="LoadUserForEditResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request);

		/// <summary>
		/// List active sessions for a specified user account.
		/// </summary>
		/// <param name="request"><see cref="ListUserSessionsRequest"/></param>
		/// <returns><see cref="ListUserSessionsResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request);

		/// <summary>
		/// Terminate the specified session(s).
		/// </summary>
		/// <param name="request"><see cref="ListUserSessionsRequest"/></param>
		/// <returns><see cref="TerminateUserSessionResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		TerminateUserSessionResponse TerminateUserSession(TerminateUserSessionRequest request);
	}
}
