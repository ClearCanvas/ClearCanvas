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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Authentication.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Authorization;
using Iesi.Collections.Generic;

namespace ClearCanvas.Enterprise.Authentication.Setup
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SetupApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SetupCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (var scope = new PersistenceScope(PersistenceContextType.Update))
				{
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = GetType().FullName;

					// import authority tokens
					var tokenImporter = new AuthorityTokenImporter();
					var allTokens = tokenImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);


					// create the sys admin group, which has all tokens assigned by default
					var tokenStrings = CollectionUtils.Map<AuthorityToken, string, List<string>>(allTokens, t => t.Name).ToArray();
					var adminGroupDef = new AuthorityGroupDefinition(cmdLine.SysAdminGroup, cmdLine.SysAdminGroup, false,
																						  tokenStrings);
					var groupImporter = new AuthorityGroupImporter();

					var groups = groupImporter.Import(new[] { adminGroupDef }, (IUpdateContext)PersistenceScope.CurrentContext);

					// find the admin group entity that was just created
					var adminGroup = CollectionUtils.SelectFirst(groups, g => g.Name == cmdLine.SysAdminGroup);

					// create the "sa" user
					CreateSysAdminUser(adminGroup, cmdLine, PersistenceScope.CurrentContext, Console.Out);

					// optionally import other default authority groups defined in other plugins
					if (cmdLine.ImportDefaultAuthorityGroups)
					{
						groupImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.CurrentContext);
					}

					scope.Complete();
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
			}
		}

		private static void CreateSysAdminUser(AuthorityGroup adminGroup, SetupCommandLine cmdLine, IPersistenceContext context, TextWriter log)
		{
			try
			{
				// create the sa user, if doesn't already exist
				var userBroker = context.GetBroker<IUserBroker>();
				var where = new UserSearchCriteria();
				where.UserName.EqualTo(cmdLine.SysAdminUserName);
				userBroker.FindOne(where);

				log.WriteLine(string.Format("User '{0}' already exists.", cmdLine.SysAdminUserName));
			}
			catch (EntityNotFoundException)
			{
				var groups = new HashedSet<AuthorityGroup> { adminGroup };

				// create sa user using initial password, set to expire never
				var saUser = User.CreateNewUser(
					new UserInfo(UserAccountType.U, cmdLine.SysAdminUserName, cmdLine.SysAdminDisplayName, null, null, null),
					Password.CreatePassword(cmdLine.SysAdminInitialPassword, null),
					groups);
				context.Lock(saUser, DirtyState.New);
			}
		}

		#endregion
	}
}
