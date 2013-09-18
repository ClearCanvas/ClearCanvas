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

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines some built-in authority groups.
	/// </summary>
	public static class BuiltInAuthorityGroups
	{
		/// <summary>
		/// 'Administrators' group.
		/// </summary>
		/// <remarks>
		/// The name of this group can be modified at install time by the setup application.
		/// </remarks>
		public static readonly AuthorityGroupDefinition Administrators
			= new AuthorityGroupDefinition(
				"Administrators",
				"Administrators",
				false,
				new string[0],	// all authority tokens will be assigned automatically
				true);

		public static readonly AuthorityGroupDefinition SystemAccounts
			= new AuthorityGroupDefinition(
				"System Accounts",
				"Built-in default authority group for System Accounts.",
				false,
				new[]
				{
					AuthorityTokens.Admin.Security.User,
					AuthorityTokens.Login.Impersonate
				},
				true);
	}
}
