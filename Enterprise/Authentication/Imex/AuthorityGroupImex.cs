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

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("AuthorityGroup")]
	public class AuthorityGroupImex : XmlEntityImex<AuthorityGroup, AuthorityGroupDefinition>
	{
		#region Overrides

		protected override IList<AuthorityGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new AuthorityGroupSearchCriteria();
			where.Name.SortAsc(0);
			return context.GetBroker<IAuthorityGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override AuthorityGroupDefinition Export(AuthorityGroup group, IReadContext context)
		{
			return new AuthorityGroupDefinition(
				group.Name,
				group.Description,
				group.DataGroup,
				group.AuthorityTokens.Select(t => t.Name).ToArray(),
				group.BuiltIn);
		}

		protected override void Import(AuthorityGroupDefinition data, IUpdateContext context)
		{
			var group = LoadOrCreateGroup(data.Name, context);
			group.Description = data.Description;
			group.DataGroup = data.DataGroup;
			group.BuiltIn = data.BuiltIn;

			if (data.Tokens != null)
			{
				foreach (var token in data.Tokens)
				{
					var where = new AuthorityTokenSearchCriteria();
					where.Name.EqualTo(token);

					var authToken = CollectionUtils.FirstElement(context.GetBroker<IAuthorityTokenBroker>().Find(where));
					if (authToken != null)
						group.AuthorityTokens.Add(authToken);
				}
			}
		}

		#endregion


		private static AuthorityGroup LoadOrCreateGroup(string name, IPersistenceContext context)
		{
			AuthorityGroup group;

			try
			{
				var criteria = new AuthorityGroupSearchCriteria();
				criteria.Name.EqualTo(name);

				var broker = context.GetBroker<IAuthorityGroupBroker>();
				group = broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				group = new AuthorityGroup {Name = name};
				context.Lock(group, DirtyState.New);
			}
			return group;
		}
	}
}

