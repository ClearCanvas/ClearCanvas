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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin
{
	internal class AuthorityGroupAssembler
	{
		internal AuthorityGroupSummary CreateAuthorityGroupSummary(AuthorityGroup authorityGroup)
		{
			return new AuthorityGroupSummary(
				authorityGroup.GetRef(),
				authorityGroup.Name,
				authorityGroup.Description,
				authorityGroup.BuiltIn,
				authorityGroup.DataGroup);
		}

		internal AuthorityGroupDetail CreateAuthorityGroupDetail(AuthorityGroup authorityGroup)
		{

			var assembler = new AuthorityTokenAssembler();
			var tokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary>(
				authorityGroup.AuthorityTokens,
				assembler.GetAuthorityTokenSummary);

			return new AuthorityGroupDetail(
				authorityGroup.GetRef(),
				authorityGroup.Name,
				authorityGroup.Description,
				authorityGroup.BuiltIn,
				authorityGroup.DataGroup,
				tokens);
		}

		internal void UpdateAuthorityGroup(AuthorityGroup authorityGroup, AuthorityGroupDetail detail, IPersistenceContext persistenceContext)
		{
			// do not update BuiltIn flag
			authorityGroup.Name = detail.Name;
			authorityGroup.Description = detail.Description;
			authorityGroup.DataGroup = detail.DataGroup;
			authorityGroup.AuthorityTokens.Clear();

			if (detail.AuthorityTokens.Count > 0)
			{
				// process authority tokens
				var tokenNames = CollectionUtils.Map<AuthorityTokenSummary, string>(
					detail.AuthorityTokens,
					token => token.Name);

				var where = new AuthorityTokenSearchCriteria();
				where.Name.In(tokenNames);
				var authTokens = persistenceContext.GetBroker<IAuthorityTokenBroker>().Find(where);

				authorityGroup.AuthorityTokens.AddAll(authTokens);
			}
		}
	}
}
