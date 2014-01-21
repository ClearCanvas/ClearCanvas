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

using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
	internal class UserAssembler
	{
		internal UserSummary GetUserSummary(User user)
		{
			return new UserSummary(
				EnumUtils.GetEnumValueInfo(user.AccountType, PersistenceScope.CurrentContext),
				user.UserName,
				user.DisplayName,
				user.EmailAddress,
				user.CreationTime,
				user.ValidFrom,
				user.ValidUntil,
				user.LastLoginTime,
				user.Password.ExpiryTime,
				user.Enabled,
				user.ActiveSessions.Count());
		}

		internal UserSummary GetUserSummaryMinimal(User user)
		{
			return new UserSummary(
				EnumUtils.GetEnumValueInfo(user.AccountType, PersistenceScope.CurrentContext),
				user.UserName,
				user.DisplayName,
				user.EmailAddress);
		}

		internal UserDetail GetUserDetail(User user)
		{
			var assembler = new AuthorityGroupAssembler();

			var groups = CollectionUtils.Map(
				user.AuthorityGroups,
				(AuthorityGroup group) => assembler.CreateAuthorityGroupSummary(group));

			return new UserDetail(
				EnumUtils.GetEnumValueInfo(user.AccountType, PersistenceScope.CurrentContext),
				user.UserName,
				user.DisplayName,
				user.EmailAddress,
				user.CreationTime,
				user.ValidFrom,
				user.ValidUntil,
				user.LastLoginTime,
				user.Enabled,
				user.Password.ExpiryTime,
				groups);
		}

		internal void UpdateUser(User user, UserDetail detail, IPersistenceContext context)
		{
			// do not update user.AccountType
			// do not update user.UserName
			// do not update user.Password
			user.DisplayName = detail.DisplayName;
			user.ValidFrom = detail.ValidFrom;
			user.ValidUntil = detail.ValidUntil;
			user.Enabled = detail.Enabled;
			user.Password.ExpiryTime = detail.PasswordExpiryTime;
			user.EmailAddress = detail.EmailAddress;

			// process authority groups
			var authGroups = CollectionUtils.Map(
				detail.AuthorityGroups,
				(AuthorityGroupSummary group) => context.Load<AuthorityGroup>(group.AuthorityGroupRef, EntityLoadFlags.Proxy));

			user.AuthorityGroups.Clear();
			user.AuthorityGroups.AddAll(authGroups);
		}

		internal UserSessionSummary GetUserSessionSummary(UserSession session)
		{
			return new UserSessionSummary
					{
						Application = session.Application,
						CreationTime = session.CreationTime,
						ExpiryTime = session.ExpiryTime,
						HostName = session.HostName,
						SessionId = session.SessionId
					};
		}
	}
}
