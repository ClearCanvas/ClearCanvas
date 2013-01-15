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
using System.Linq;
using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using System.Threading;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IUserAdminService))]
	public class UserAdminService : CoreServiceLayer, IUserAdminService
	{
		#region IUserAdminService Members

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ListUsersResponse ListUsers(ListUsersRequest request)
		{
			var criteria = new UserSearchCriteria();
			criteria.UserName.SortAsc(0);

			// create the criteria, depending on whether matches should be "exact" or "like"
			if (request.ExactMatchOnly)
			{
				if (!string.IsNullOrEmpty(request.UserName))
					criteria.UserName.EqualTo(request.UserName);
				if (!string.IsNullOrEmpty(request.DisplayName))
					criteria.DisplayName.EqualTo(request.DisplayName);
			}
			else
			{
				if (!string.IsNullOrEmpty(request.UserName))
					criteria.UserName.StartsWith(request.UserName);
				if (!string.IsNullOrEmpty(request.DisplayName))
					criteria.DisplayName.Like(string.Format("%{0}%", request.DisplayName));
			}

			var assembler = new UserAssembler();
			var userSummaries = CollectionUtils.Map(
				PersistenceContext.GetBroker<IUserBroker>().Find(criteria, request.Page),
				(User user) => assembler.GetUserSummary(user));

			return new ListUsersResponse(userSummaries);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
		{
			var user = FindUserByName(request.UserName);

			var assembler = new UserAssembler();
			return new LoadUserForEditResponse(assembler.GetUserDetail(user));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public AddUserResponse AddUser(AddUserRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserDetail, "UserDetail");

			var userDetail = request.UserDetail;
			var settings = new AuthenticationSettings();

			// create new user
			var userInfo =
				new UserInfo(userDetail.UserName, userDetail.DisplayName, userDetail.EmailAddress, userDetail.ValidFrom, userDetail.ValidUntil);

			var user = User.CreateNewUser(userInfo, settings.DefaultTemporaryPassword);

			// copy other info such as authority groups from request
			var assembler = new UserAssembler();
			assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

			// save
			PersistenceContext.Lock(user, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public UpdateUserResponse UpdateUser(UpdateUserRequest request)
		{
			var user = FindUserByName(request.UserDetail.UserName);

			// update user account info
			var assembler = new UserAssembler();
			assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

			// reset password if requested
			if (request.UserDetail.ResetPassword)
			{
				var settings = new AuthenticationSettings();
				user.ResetPassword(settings.DefaultTemporaryPassword);

			}

			PersistenceContext.SynchState();

			return new UpdateUserResponse(assembler.GetUserSummary(user));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public DeleteUserResponse DeleteUser(DeleteUserRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForEmptyString(request.UserName, "UserName");

			// prevent current user from deleting own account
			if (request.UserName == Thread.CurrentPrincipal.Identity.Name)
				throw new RequestValidationException(SR.MessageCannotDeleteOwnUserAccount);

			var broker = PersistenceContext.GetBroker<IUserBroker>();
			var user = FindUserByName(request.UserName);

			// remove user from groups we don't get errors from db references
			user.AuthorityGroups.Clear();

			// delete user
			broker.Delete(user);

			return new DeleteUserResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var user = FindUserByName(request.UserName);

			var settings = new AuthenticationSettings();
			user.ResetPassword(settings.DefaultTemporaryPassword);

			var assembler = new UserAssembler();
			return new ResetUserPasswordResponse(assembler.GetUserSummary(user));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var user = FindUserByName(request.UserName);
			var assembler = new UserAssembler();
			return new ListUserSessionsResponse(user.UserName, user.ActiveSessions.Select(assembler.GetUserSessionSummary).ToList());
		}


		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public TerminateUserSessionResponse TerminateUserSession(TerminateUserSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForNullReference(request.SessionIds, "SessionIds");

			// exclude the current session - user must not delete own active session!
			var sessionIds = request.SessionIds.Where(id => id != CurrentUserSessionId).ToList();
			if(sessionIds.Count == 0)
				throw new RequestValidationException(SR.MessageCannotDeleteOwnUserCurrentSession);

			// load all sessions by id 
			var where = new UserSessionSearchCriteria();
			where.SessionId.In(sessionIds);

			var sessions = PersistenceContext.GetBroker<IUserSessionBroker>().Find(where);

			// terminate all sessions
			foreach (var session in sessions)
			{
				session.Terminate();
			}

			return new TerminateUserSessionResponse(sessions.Select(s => s.SessionId).ToList());
		}

		#endregion

		private User FindUserByName(string name)
		{
			try
			{
				var where = new UserSearchCriteria();
				where.UserName.EqualTo(name);

				return PersistenceContext.GetBroker<IUserBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				throw new RequestValidationException(string.Format("{0} is not a valid user name.", name));
			}
		}

		private static string CurrentUserSessionId
		{
			get
			{
				if (!(Thread.CurrentPrincipal is DefaultPrincipal))
					throw new InvalidOperationException("Unable to obtain current user session ID on this thread");

				return (Thread.CurrentPrincipal as DefaultPrincipal).SessionToken.Id;
			}
		}
	}
}
