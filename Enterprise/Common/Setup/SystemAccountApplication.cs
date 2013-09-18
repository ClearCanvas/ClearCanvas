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
using System.Linq;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Connects to the enterprise server to manage system accounts.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SystemAccountApplication : IApplicationRoot
	{
		public enum Action
		{
			Create,
			Modify,
			Remove
		}


		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SystemAccountCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "SystemAccount Utility", Dns.GetHostName(), cmdLine.Password))
				{
					switch (cmdLine.Action)
					{
						case Action.Create:
							CreateAccount(cmdLine);
							break;
						case Action.Modify:
							ModifyAccount(cmdLine);
							break;
						case Action.Remove:
							RemoveAccount(cmdLine);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static void CreateAccount(SystemAccountCommandLine cmdLine)
		{
			if(string.IsNullOrEmpty(cmdLine.AccountPassword))
				throw new CommandLineException("Password for the new account must be provided.");

			var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup ?? BuiltInAuthorityGroups.SystemAccounts.Name);
			var userDetail = new UserDetail
								{
									AccountType = new EnumValueInfo("S", null),
									UserName = cmdLine.AccountName,
									DisplayName = cmdLine.AccountName,
									Enabled = true,
									AuthorityGroups = new List<AuthorityGroupSummary> { authGroup }
								};

			Platform.GetService<IUserAdminService>(
				service => service.AddUser(new AddUserRequest(userDetail) { Password = cmdLine.AccountPassword }));
		}

		private static void ModifyAccount(SystemAccountCommandLine cmdLine)
		{
			Platform.GetService<IUserAdminService>(
				service =>
					{
						var account = service.LoadUserForEdit(new LoadUserForEditRequest(cmdLine.AccountName)).UserDetail;
						var updateRequest = new UpdateUserRequest(account);

						// update auth group if specified
						if(!string.IsNullOrEmpty(cmdLine.AuthorityGroup))
						{
							var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup);
							account.AuthorityGroups = new List<AuthorityGroupSummary> {authGroup};
						}

						// update password if specified
						if(!string.IsNullOrEmpty(cmdLine.AccountPassword))
						{
							updateRequest.Password = cmdLine.AccountPassword;
						}

						service.UpdateUser(updateRequest);
					});
		}

		private static void RemoveAccount(SystemAccountCommandLine cmdLine)
		{
			Platform.GetService<IUserAdminService>(
				service => service.DeleteUser(new DeleteUserRequest(cmdLine.AccountName)));
		}

		#endregion

		private static AuthorityGroupSummary GetAuthorityGroup(string name)
		{
			List<AuthorityGroupSummary> authGroups = null;
			Platform.GetService<IAuthorityGroupReadService>(
				service => authGroups = service.ListAuthorityGroups(new ListAuthorityGroupsRequest()).AuthorityGroups);
			var authGroup = authGroups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (authGroup == null)
				throw new InvalidOperationException(string.Format("Authority group '{0}' does not exist.", name));
			return authGroup;
		}
	}
}
