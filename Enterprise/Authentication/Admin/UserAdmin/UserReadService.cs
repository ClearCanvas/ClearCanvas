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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IUserReadService))]
	public class UserReadService : CoreServiceLayer, IUserReadService
	{
		#region IUserReadService Members

		[ReadOperation]
		public ListUsersResponse ListUsers(ListUsersRequest request)
		{
			var criteria = new UserSearchCriteria();
			criteria.UserName.SortAsc(0);
			criteria.AccountType.EqualTo(UserAccountType.U); // only regular user accounts are visible through this service

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
			var broker = PersistenceContext.GetBroker<IUserBroker>();
			var userSummaries = CollectionUtils.Map(
				broker.Find(criteria, request.Page),
				(User user) => assembler.GetUserSummaryMinimal(user));
			var total = broker.Count(criteria);

			return new ListUsersResponse(userSummaries, (int)total);
		}

		#endregion
	}
}
