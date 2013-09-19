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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Authentication.Imex
{

	/// <summary>
	/// Imports authority groups from plugins that define extensions to <see cref="DefineAuthorityGroupsExtensionPoint"/>.
	/// </summary>
	/// <remarks>
	/// This class implements <see cref="IApplicationRoot"/> so that it may be run stand-alone from a console.  However,
	/// it may also be used as a utility class to be invoked by other means.
	/// </remarks>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class AuthorityGroupImporter : IApplicationRoot
	{
		/// <summary>
		/// Import authority groups from extensions of <see cref="DefineAuthorityGroupsExtensionPoint"/>.
		/// </summary>
		/// <remarks>
		/// Creates any authority groups that do not already exist.
		/// This method performs an additive import.  It will never remove an existing authority group or
		/// remove authority tokens from an existing group.
		/// </remarks>
		/// <param name="context"></param>
		public IList<AuthorityGroup> ImportFromPlugins(IUpdateContext context)
		{
			var groupDefs = AuthorityGroupSetup.GetDefaultAuthorityGroups();
			return Import(groupDefs, context);
		}

		/// <summary>
		/// Import authority groups.
		/// </summary>
		/// <remarks>
		/// Creates any authority groups that do not already exist.
		/// This method performs an additive import.  It will never remove an existing authority group or
		/// remove authority tokens from an existing group.
		/// </remarks>
		/// <param name="groupDefs"></param>
		/// <param name="context"></param>
		public IList<AuthorityGroup> Import(IEnumerable<AuthorityGroupDefinition> groupDefs, IUpdateContext context)
		{
			// first load all the existing tokens into memory
			// there should not be that many tokens ( < 500), so this should not be a problem
			var tokenBroker = context.GetBroker<IAuthorityTokenBroker>();
			var existingTokens = tokenBroker.FindAll();

			// load existing groups
			var groupBroker = context.GetBroker<IAuthorityGroupBroker>();
			var existingGroups = groupBroker.FindAll();

			foreach (var groupDef in groupDefs)
			{
				var group = CollectionUtils.SelectFirst(existingGroups, g => g.Name == groupDef.Name);

				// if group does not exist, create it
				if (group == null)
				{
					group = new AuthorityGroup
								{
									Name = groupDef.Name,
									Description = groupDef.Description,
									DataGroup = groupDef.DataGroup,
									BuiltIn = groupDef.BuiltIn
								};
					context.Lock(group, DirtyState.New);
					existingGroups.Add(group);
				}

				// process all token nodes contained in group
				foreach (var tokenName in groupDef.Tokens)
				{
					var token = CollectionUtils.SelectFirst(existingTokens, t => t.Name == tokenName);

					// ignore non-existent tokens
					if (token == null)
						continue;

					// add the token to the group
					group.AuthorityTokens.Add(token);
				}
			}

			return existingGroups;
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			using (var scope = new PersistenceScope(PersistenceContextType.Update))
			{
				((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
				ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);

				scope.Complete();
			}
		}

		#endregion

	}
}
