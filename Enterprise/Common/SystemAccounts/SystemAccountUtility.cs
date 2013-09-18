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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Common.SystemAccounts
{
	/// <summary>
	/// Utility application for managing system accounts.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SystemAccountUtility : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SystemAccountCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "SystemAccount Utility", Dns.GetHostName(), cmdLine.Password))
				{
					CreateOrUpdateAccount(cmdLine);
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		#endregion

		private static void CreateOrUpdateAccount(SystemAccountCommandLine cmdLine)
		{
			var accountName = cmdLine.AccountName.ToLowerInvariant();

			var localAccounts = LocalRegistryManager.GetAccounts();
			if (!localAccounts.Contains(accountName))
				throw new InvalidOperationException(string.Format("Account '{0}' is not a locally-known account.", accountName));

			// update the enterprise server
			UserDetail accountDetail;
			if (GetExistingAccount(accountName, out accountDetail))
			{
				var updateRequest = new UpdateUserRequest(accountDetail);
				// update auth group if specified
				if (!string.IsNullOrEmpty(cmdLine.AuthorityGroup))
				{
					var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup);
					accountDetail.AuthorityGroups = new List<AuthorityGroupSummary> { authGroup };
				}

				// update password if specified
				var changePassword = !string.IsNullOrEmpty(cmdLine.AccountPassword);
				if (changePassword)
				{
					updateRequest.Password = cmdLine.AccountPassword;
				}

				Platform.GetService<IUserAdminService>(
					service => service.UpdateUser(updateRequest));

				// update the local machine
				if (changePassword)
				{
					LocalRegistryManager.SetAccountPassword(accountName, cmdLine.AccountPassword);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(cmdLine.AccountPassword))
					throw new CommandLineException("Password for the new account must be provided.");

				var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup ?? BuiltInAuthorityGroups.SystemAccounts.Name);
				var userDetail = new UserDetail
				{
					AccountType = new EnumValueInfo("S", null),
					UserName = accountName,
					DisplayName = accountName,
					Enabled = true,
					AuthorityGroups = new List<AuthorityGroupSummary> { authGroup }
				};

				Platform.GetService<IUserAdminService>(
					service => service.AddUser(new AddUserRequest(userDetail) { Password = cmdLine.AccountPassword }));
				
				// save password locally
				LocalRegistryManager.SetAccountPassword(accountName, cmdLine.AccountPassword);
			}
		}

		private static bool GetExistingAccount(string account, out UserDetail detail)
		{
			try
			{
				LoadUserForEditResponse response = null;
				Platform.GetService<IUserAdminService>(
					service => response = service.LoadUserForEdit(new LoadUserForEditRequest(account)));

				detail = response.UserDetail;
				return true;
			}
			catch(FaultException<RequestValidationException>)
			{
				detail = null;
				return false;
			}
		}

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
