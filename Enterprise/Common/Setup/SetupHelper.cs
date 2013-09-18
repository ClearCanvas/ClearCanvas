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
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Common.Setup
{
	public static class SetupHelper
	{
		/// <summary>
		/// Import authority tokens defined in local plugins.
		/// </summary>
		public static void ImportAuthorityTokens(string[] addToGroups)
		{
			var tokens = AuthorityGroupSetup.GetAuthorityTokens();
			var summaries = tokens.Select(t => new AuthorityTokenSummary(t.Token, t.DefiningAssembly, t.Description, t.FormerIdentities)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityTokens(new ImportAuthorityTokensRequest(summaries, new List<string>(addToGroups))));

			LogImportedTokens(tokens);
		}

		/// <summary>
		/// Import authority groups defined in local plugins.
		/// </summary>
		public static void ImportAuthorityGroups()
		{

			var groups = AuthorityGroupSetup.GetDefaultAuthorityGroups();
			var groupDetails = groups.Select(g =>
				new AuthorityGroupDetail(
					null,
					g.Name,
					g.Description,
					g.BuiltIn,
					g.DataGroup,
					g.Tokens.Select(t => new AuthorityTokenSummary(t)).ToList()
				)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityGroups(new ImportAuthorityGroupsRequest(groupDetails)));

			LogImportedDefaultGroups(groups);
		}

		private static void LogImportedDefaultGroups(IEnumerable<AuthorityGroupDefinition> groups)
		{
			foreach (var g in groups.Distinct())
			{
				Platform.Log(LogLevel.Info, "Imported default authority group definition: {0}", g.Name);
			}
		}

		private static void LogImportedTokens(IEnumerable<AuthorityTokenDefinition> tokens)
		{
			foreach (var token in tokens)
			{
				Platform.Log(LogLevel.Info, "Imported authority token '{0}' from {1}", token.Token, token.DefiningAssembly);
			}
		}

	}
}
