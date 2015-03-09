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
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	public class SystemAccountUtility : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SystemAccountCommandLine();
			try
			{
				cmdLine.Parse(args);

				// use lower-case, to avoid casing issues
				var accountName = cmdLine.AccountName.ToLowerInvariant();

				var localAccounts = LocalRegistryManager.GetAccounts();
				if (!localAccounts.Contains(accountName))
					throw new InvalidOperationException(string.Format("Account '{0}' is not a locally-known system account.", accountName));

				StdOut(string.Format("Connecting to Enterprise Server as '{0}'...", cmdLine.UserName), cmdLine);
				using (new AuthenticationScope(cmdLine.UserName, "SystemAccount Utility", Dns.GetHostName(), cmdLine.Password))
				{
					CreateOrUpdateAccount(accountName, cmdLine);
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		#endregion

		private static void CreateOrUpdateAccount(string accountName, SystemAccountCommandLine cmdLine)
		{
			StdOut(string.Format("Checking Enterprise Server for existing account '{0}'...", accountName), cmdLine);

			// update the enterprise server
			UserDetail accountDetail;
			if (GetExistingAccount(accountName, out accountDetail))
			{
				StdOut(string.Format("Account '{0}' exists.", accountName), cmdLine);

				var updateRequest = new UpdateUserRequest(accountDetail);

				bool authGroupChange = false, passwordChange = false;
				// update auth group if specified
				if (!string.IsNullOrEmpty(cmdLine.AuthorityGroup))
				{
					var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup);
					accountDetail.AuthorityGroups = new List<AuthorityGroupSummary> {authGroup};
					authGroupChange = true;
					StdOut(string.Format("Authority group will be set to '{0}'.", authGroup.Name), cmdLine);
				}

				// update password if specified
				var changePassword = !string.IsNullOrEmpty(cmdLine.AccountPassword);
				if (changePassword)
				{
					updateRequest.Password = cmdLine.AccountPassword;
					passwordChange = true;
					StdOut(string.Format("Account password will be set to '{0}'.", updateRequest.Password), cmdLine);
				}
				else
				{
					// if password was not specified, generate password, and check if reset password flag was set
					// this distinction exists because in upgrade+distributed scenarios, it is possible that the account exists
					// but that the password may be explicitly specified by another (earlier) component install
					// so the installer needs a way to use generated password locally without effecting any changes to enterprise server
					updateRequest.Password = cmdLine.AccountPassword = GeneratePassword(accountName);
					if (cmdLine.ResetPassword)
					{
						// save generated password to both server and local
						passwordChange = changePassword = true;
					}
					else
					{
						// only save generated password locally
						StdOut(string.Format("Synchronizing password locally..."), cmdLine);
						LocalRegistryManager.SetAccountPassword(accountName, cmdLine.AccountPassword);
					}
				}

				if (passwordChange || authGroupChange)
				{
					StdOut(string.Format("Saving changes to Enterprise Server..."), cmdLine);

					Platform.GetService<IUserAdminService>(
						service => service.UpdateUser(updateRequest));

					// update the local machine
					if (changePassword)
					{
						StdOut(string.Format("Saving password locally..."), cmdLine);
						LocalRegistryManager.SetAccountPassword(accountName, cmdLine.AccountPassword);
					}
					StdOut(string.Format("All changes saved."), cmdLine);
				}
				else
				{
					StdOut(string.Format("No changes to save."), cmdLine);
				}
			}
			else
			{
				StdOut(string.Format("Account '{0}' does not exist. It will be created.", accountName), cmdLine);

				if (string.IsNullOrEmpty(cmdLine.AccountPassword))
				{
					// generate a password
					cmdLine.AccountPassword = GeneratePassword(accountName);
				}

				var authGroup = GetAuthorityGroup(cmdLine.AuthorityGroup ?? BuiltInAuthorityGroups.SystemAccounts.Name);
				var userDetail = new UserDetail
				                 {
					                 AccountType = new EnumValueInfo("S", null),
					                 UserName = accountName,
					                 DisplayName = accountName,
					                 Enabled = true,
					                 AuthorityGroups = new List<AuthorityGroupSummary> {authGroup}
				                 };

				StdOut(string.Format("Creating account '{0}', authority group '{1}'.",
				                     accountName, authGroup.Name), cmdLine);

				StdOut(string.Format("Saving changes to Enterprise Server..."), cmdLine);
				Platform.GetService<IUserAdminService>(
					service => service.AddUser(new AddUserRequest(userDetail) {Password = cmdLine.AccountPassword}));

				// save password locally
				StdOut(string.Format("Saving password locally..."), cmdLine);
				LocalRegistryManager.SetAccountPassword(accountName, cmdLine.AccountPassword);

				StdOut(string.Format("All changes saved."), cmdLine);
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
			catch (FaultException<RequestValidationException>)
			{
				detail = null;
				return false;
			}
		}

		private static AuthorityGroupSummary GetAuthorityGroup(string name)
		{
			List<AuthorityGroupSummary> authGroups = null;
			Platform.GetService<IAuthorityGroupAdminService>(
				service => authGroups = service.ListAuthorityGroups(new ListAuthorityGroupsRequest()).AuthorityGroups);
			var authGroup = authGroups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (authGroup == null)
				throw new InvalidOperationException(string.Format("Authority group '{0}' does not exist.", name));
			return authGroup;
		}

		private static void StdOut(string message, SystemAccountCommandLine cmdLine)
		{
			if (cmdLine.Verbose)
			{
				Console.WriteLine(message);
			}
		}

		private static string GeneratePassword(string systemAccountName)
		{
			string password = null;
			Platform.GetService<ISystemInfoService>(svc => { password = svc.GetDerivedSystemSecretKey(new GetDerivedSystemSecretKeyRequest {Input = "SystemAccount|" + systemAccountName.ToLowerInvariant()}).Key; });
			return password;
		}
	}
}