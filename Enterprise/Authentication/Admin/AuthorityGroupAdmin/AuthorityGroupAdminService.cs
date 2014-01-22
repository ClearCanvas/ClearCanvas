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

using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Authentication.Imex;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication.Admin.AuthorityGroupAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IAuthorityGroupAdminService))]
	public class AuthorityGroupAdminService : CoreServiceLayer, IAuthorityGroupAdminService
	{
		#region IAuthorityGroupAdminService Members

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
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

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
		{
			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef);
			var assembler = new AuthorityGroupAssembler();
			return new LoadAuthorityGroupForEditResponse(assembler.CreateAuthorityGroupDetail(authorityGroup));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
		{
			var assembler = new AuthorityTokenAssembler();
			var authorityTokens = CollectionUtils.Map(
				PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindAll(),
				(AuthorityToken authorityToken) => assembler.GetAuthorityTokenSummary(authorityToken));

			return new ListAuthorityTokensResponse(authorityTokens);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroupDetail, "AuthorityGroupDetail");

			if (request.AuthorityGroupDetail.BuiltIn)
				throw new RequestValidationException(SR.MessageCannotManageBuiltInAuthorityGroups);

			// create new group
			var authorityGroup = new AuthorityGroup();

			// set properties from request
			var assembler = new AuthorityGroupAssembler();
			assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

			// save
			PersistenceContext.Lock(authorityGroup, DirtyState.New);
			PersistenceContext.SynchState();

			return new AddAuthorityGroupResponse(assembler.CreateAuthorityGroupSummary(authorityGroup));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroupDetail, "AuthorityGroupDetail");

			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupDetail.AuthorityGroupRef);
			if (authorityGroup.BuiltIn || request.AuthorityGroupDetail.BuiltIn)
				throw new RequestValidationException(SR.MessageCannotManageBuiltInAuthorityGroups);

			if (authorityGroup.DataGroup && !request.AuthorityGroupDetail.DataGroup)
			{
				var user = GetUser(Thread.CurrentPrincipal.Identity.Name, PersistenceContext);
				if (!user.Password.Verify(request.Password))
				{
					// the error message is deliberately vague
					throw new UserAccessDeniedException();
				}
			}

			// set properties from request
			var assembler = new AuthorityGroupAssembler();
			assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

			PersistenceContext.SynchState();

			return new UpdateAuthorityGroupResponse(assembler.CreateAuthorityGroupSummary(authorityGroup));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroupRef, "AuthorityGroupRef");

			var broker = PersistenceContext.GetBroker<IAuthorityGroupBroker>();
			var authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef, EntityLoadFlags.Proxy);
			if (authorityGroup.BuiltIn)
				throw new RequestValidationException(SR.MessageCannotManageBuiltInAuthorityGroups);

			if (request.DeleteOnlyWhenEmpty)
			{
				var count = broker.GetUserCountForGroup(authorityGroup);
				if (count > 0)
					throw new AuthorityGroupIsNotEmptyException(authorityGroup.Name, count);
			}

			// before we can delete an authority group, first need to remove all tokens and users
			authorityGroup.AuthorityTokens.Clear();
			authorityGroup.RemoveAllUsers();

			// delete group
			broker.Delete(authorityGroup);

			PersistenceContext.SynchState();

			return new DeleteAuthorityGroupResponse();
		}


		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Tokens, "Tokens");

			if (request.Tokens.Count > 0)
			{
				var importer = new AuthorityTokenImporter();
				importer.Import(
					CollectionUtils.Map(request.Tokens, (AuthorityTokenSummary s) => new AuthorityTokenDefinition(s.Name, s.DefiningAssembly, s.Description, s.FormerIdentities.ToArray())),
						request.AddToGroups,
						(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityTokensResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroups, "AuthorityGroups");

			if (request.AuthorityGroups.Count > 0)
			{
				var importer = new AuthorityGroupImporter();
				importer.Import(
					CollectionUtils.Map(request.AuthorityGroups, (AuthorityGroupDetail g) => GetAuthorityGroupDefinition(g)),
					(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityGroupsResponse();
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Gets the user specified by the user name, or null if no such user exists.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="persistenceContext"></param>
		/// <returns></returns>
		private static User GetUser(string userName, IPersistenceBrokerFactory persistenceContext)
		{
			var criteria = new UserSearchCriteria();
			criteria.UserName.EqualTo(userName);

			// use query caching here to make this fast (assuming the user table is not often updated)
			var users = persistenceContext.GetBroker<IUserBroker>().Find(
				criteria, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			// bug #3701: to ensure the username match is case-sensitive, we need to compare the stored name to the supplied name
			// returns null if no match
			return CollectionUtils.SelectFirst(users, u => u.UserName == userName);
		}

		private static AuthorityGroupDefinition GetAuthorityGroupDefinition(AuthorityGroupDetail detail)
		{
			return new AuthorityGroupDefinition(
				detail.Name,
				detail.Description,
				detail.DataGroup,
				CollectionUtils.Map(detail.AuthorityTokens, (AuthorityTokenSummary s) => s.Name).ToArray(),
				detail.BuiltIn);
		}

		#endregion

	}
}
