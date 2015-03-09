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

namespace ClearCanvas.Enterprise.Common.SystemAccounts
{
	internal class SystemAccountCommandLine : CommandLine
	{
		public SystemAccountCommandLine()
		{
			Password = "clearcanvas";
			UserName = "sa";
			Verbose = true;
		}

		/// <summary>
		/// Specifies user name to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("suid", "Specifies user name to connect to enterprise server. Default is 'sa'.")]
		public string UserName { get; set; }

		/// <summary>
		/// Specifies password to connect to enterprise server.
		/// </summary>
		[CommandLineParameter("spwd", "Specifies password to connect to enterprise server. Default is 'clearcanvas'.")]
		public string Password { get; set; }

		/// <summary>
		/// Specifies the name of the system account to create or update. Required.
		/// </summary>
		[CommandLineParameter("account", "a", "Specifies the name of the system account to create or update.", Required = true)]
		public string AccountName { get; set; }

		/// <summary>
		/// Specifies the password to be set on the account.
		/// </summary>
		[CommandLineParameter("password", "p", "Specifies the password to be set on the account.")]
		public string AccountPassword { get; set; }

		/// <summary>
		/// Specifies the name of the authority group to which the account should be assigned.
		/// If not specified, new accounts will be assigned to the 'Service Accounts' group.
		/// </summary>
		[CommandLineParameter("auth", "Specifies the name of the authority group to which the account should be assigned. If not specified, new accounts will be assigned to the 'Service Accounts' group.")]
		public string AuthorityGroup { get; set; }

		/// <summary>
		/// Specifies whether the program should reset the account password when updating an existing account.
		/// If the account exists, this switch is implied when not explicitly specifying the password.
		/// </summary>
		[CommandLineParameter("resetpassword", "Specifies that the account password should be reset if updating an existing account.")]
		public bool ResetPassword { get; set; }

		/// <summary>
		/// Specifies whether the program should emit informational messages to stdout.
		/// </summary>
		[CommandLineParameter("verbose", "v", "Specifies whether the program should emit informational messages to stdout. Default is true.")]
		public bool Verbose { get; set; }
	}
}