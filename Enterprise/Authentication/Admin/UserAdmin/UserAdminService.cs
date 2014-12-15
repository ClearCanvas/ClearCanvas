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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using System.Threading;
using Iesi.Collections.Generic;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IUserAdminService))]
	public class UserAdminService : CoreServiceLayer, IUserAdminService
	{
		#region IUserAdminService Members

		[ReadOperation]
		public ListUsersResponse ListUsers(ListUsersRequest request)
		{
			// establish which account types this user is entitled to see
			var visibleAccountTypes = GetAccountTypesAuthorizedToManage(request.IncludeGroupAccounts, request.IncludeSystemAccounts).ToList();
			if (!visibleAccountTypes.Any())
				throw new SecurityException(SR.MessageUserNotAuthorized);

			var criteria = new UserSearchCriteria();
			criteria.AccountType.In(visibleAccountTypes);
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

			var broker = PersistenceContext.GetBroker<IUserBroker>();
			var assembler = new UserAssembler();
			var userSummaries = CollectionUtils.Map(
				broker.Find(criteria, request.Page),
				(User user) => assembler.GetUserSummary(user));
			var total = broker.Count(criteria);

			return new ListUsersResponse(userSummaries, (int)total);
		}

		[ReadOperation]
		public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
		{
			var user = FindUserByName(request.UserName);

			EnsureCurrentUserAuthorizedToManage(user.AccountType);

			var assembler = new UserAssembler();
			return new LoadUserForEditResponse(assembler.GetUserDetail(user));
		}

		[UpdateOperation]
		public AddUserResponse AddUser(AddUserRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserDetail, "UserDetail");

			var userDetail = request.UserDetail;
			var accountType = (userDetail.AccountType != null)
				? EnumUtils.GetEnumValue<UserAccountType>(userDetail.AccountType)
				: UserAccountType.U;	// default account type is U if not specified

			// is the current user authorized to create user accounts of this type?
			EnsureCurrentUserAuthorizedToManage(accountType);

			if(!UserName.IsLegalUserName(userDetail.UserName))
				throw new RequestValidationException("Illegal account name.");

			// create new user
			var userInfo = new UserInfo(
				accountType,
				userDetail.UserName,
				userDetail.DisplayName,
				userDetail.EmailAddress,
				userDetail.ValidFrom,
				userDetail.ValidUntil);

			var password = GetNewAccountPassword(accountType, request.Password);
			var user = User.CreateNewUser(userInfo, password, new HashedSet<AuthorityGroup>());

			// copy other info such as authority groups from request
			var assembler = new UserAssembler();
			assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

			// save
			PersistenceContext.Lock(user, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
		}

		[UpdateOperation]
		public UpdateUserResponse UpdateUser(UpdateUserRequest request)
		{
			var user = FindUserByName(request.UserDetail.UserName);
			EnsureCurrentUserAuthorizedToManage(user.AccountType);

			// update user account info
			var assembler = new UserAssembler();
			assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

			// for user accounts, reset password if requested
			if (request.UserDetail.ResetPassword)
			{
				if(user.AccountType != UserAccountType.U)
					throw new RequestValidationException(SR.MessageAccountTypeDoesNotSupportPasswordReset);

				var settings = new AuthenticationSettings();
				user.ResetPassword(settings.DefaultTemporaryPassword);
			}

			// for system accounts, update the password if specified
			if(!string.IsNullOrEmpty(request.Password) && user.AccountType == UserAccountType.S)
			{
				PasswordPolicy.CheckPasswordCandidate(user.AccountType, request.Password, new AuthenticationSettings());
				user.ChangePassword(request.Password, null);
			}

			PersistenceContext.SynchState();

			return new UpdateUserResponse(assembler.GetUserSummary(user));
		}

		[UpdateOperation]
		public DeleteUserResponse DeleteUser(DeleteUserRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForEmptyString(request.UserName, "UserName");

			// prevent current user from deleting own account
			if (request.UserName == Thread.CurrentPrincipal.Identity.Name)
				throw new RequestValidationException(SR.MessageCannotDeleteOwnUserAccount);

			var user = FindUserByName(request.UserName);
			EnsureCurrentUserAuthorizedToManage(user.AccountType);

			// remove user from groups we don't get errors from db references
			user.AuthorityGroups.Clear();

			// delete user
			var broker = PersistenceContext.GetBroker<IUserBroker>();
			broker.Delete(user);

			return new DeleteUserResponse();
		}

		[UpdateOperation]
		public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var user = FindUserByName(request.UserName);
			EnsureCurrentUserAuthorizedToManage(user.AccountType);

			if (user.AccountType != UserAccountType.U)
				throw new RequestValidationException(SR.MessageAccountTypeDoesNotSupportPasswordReset);


			var settings = new AuthenticationSettings();
			user.ResetPassword(settings.DefaultTemporaryPassword);

			var assembler = new UserAssembler();
			return new ResetUserPasswordResponse(assembler.GetUserSummary(user));
		}

		[ReadOperation]
		public ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var user = FindUserByName(request.UserName);
			EnsureCurrentUserAuthorizedToManage(user.AccountType);

			var assembler = new UserAssembler();
			var sessions = user.ActiveSessions.Where(s => !s.IsImpersonated);
			return new ListUserSessionsResponse(user.UserName, sessions.Select(assembler.GetUserSessionSummary).ToList());
		}


		[UpdateOperation]
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
			where.IsImpersonated.EqualTo(false);	// impersonated sessions cannot be terminated in this manner

			var sessions = PersistenceContext.GetBroker<IUserSessionBroker>().Find(where);

			// terminate sessions
			foreach (var session in sessions)
			{
				// but only if the current user is actually authorized to do so
				EnsureCurrentUserAuthorizedToManage(session.User.AccountType);
				session.Terminate();
			}

			return new TerminateUserSessionResponse(sessions.Select(s => s.SessionId).ToList());
		}

		#endregion

		private static string CurrentUserSessionId
		{
			get
			{
				if (!(Thread.CurrentPrincipal is DefaultPrincipal))
					throw new InvalidOperationException("Unable to obtain current user session ID on this thread");

				return (Thread.CurrentPrincipal as DefaultPrincipal).SessionToken.Id;
			}
		}

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
				throw new RequestValidationException(string.Format("{0} is not a valid account name.", name));
			}
		}

		private static Password GetNewAccountPassword(UserAccountType accountType, string password)
		{
			var settings = new AuthenticationSettings();
			switch (accountType)
			{
				case UserAccountType.U:
					// for user accounts, always use the temp password, set to expire immediately
					return Password.CreateTemporaryPassword(settings.DefaultTemporaryPassword);

				case UserAccountType.G:
					// for group accounts, generate a random password (since it will never be used)
					return Password.CreatePassword(Guid.NewGuid().ToString("N"), null);

				case UserAccountType.S:
					// for system accounts, use password provided in request, and set to never expire
					PasswordPolicy.CheckPasswordCandidate(UserAccountType.S, password, settings);
					return Password.CreatePassword(password, null);

				default:
					throw new ArgumentOutOfRangeException("accountType");
			}
		}

		private static void EnsureCurrentUserAuthorizedToManage(UserAccountType accountType)
		{
			if (!IsCurrentUserAuthorizedToManage(accountType))
				throw new SecurityException(SR.MessageUserNotAuthorized);
		}

		private static bool IsCurrentUserAuthorizedToManage(UserAccountType accountType)
		{
			return GetAccountTypesAuthorizedToManage(true, true).Contains(accountType);
		}

		private static IEnumerable<UserAccountType> GetAccountTypesAuthorizedToManage(bool includeGroupAccounts, bool includeSystemAccounts)
		{
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Security.User))
				yield return (UserAccountType.U);
			if (includeGroupAccounts && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Security.NonUserAccounts.Group))
				yield return (UserAccountType.G);
			if (includeSystemAccounts && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Security.NonUserAccounts.Service))
				yield return (UserAccountType.S);
		}
	}
}
