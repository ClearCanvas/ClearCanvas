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
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin.AuthorityGroupAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IAuthorityGroupReadService))]
	public class AuthorityGroupReadService : CoreServiceLayer, IAuthorityGroupReadService
	{
		#region IAuthorityGroupAdminService Members

		[ReadOperation]
		public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
		{
			var criteria = new AuthorityGroupSearchCriteria();
			criteria.Name.SortAsc(0);

			if (request.DataGroup.HasValue)
				criteria.DataGroup.EqualTo(request.DataGroup.Value);

			var broker = PersistenceContext.GetBroker<IAuthorityGroupBroker>();
			var assembler = new AuthorityGroupAssembler();
			if (request.Details.HasValue && request.Details.Value)
			{
				var authorityGroups = CollectionUtils.Map(
				 broker.Find(criteria, request.Page),
				 (AuthorityGroup authorityGroup) => assembler.CreateAuthorityGroupDetail(authorityGroup));
				var total = broker.Count(criteria);
				return new ListAuthorityGroupsResponse(authorityGroups, (int)total);
			}
			else
			{
				var authorityGroups = CollectionUtils.Map(
					broker.Find(criteria, request.Page),
					(AuthorityGroup authorityGroup) => assembler.CreateAuthorityGroupSummary(authorityGroup));
				var total = broker.Count(criteria);
				return new ListAuthorityGroupsResponse(authorityGroups, (int)total);
			}
		}

		#endregion

	}
}
