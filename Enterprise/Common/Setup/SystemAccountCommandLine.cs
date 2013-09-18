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

namespace ClearCanvas.Enterprise.Common.Setup
{
	class SystemAccountCommandLine : CommandLine
	{
		public SystemAccountCommandLine()
		{
			Password = "clearcanvas";
			UserName = "sa";
		}

		/// <summary>
		/// Specifies what to do - either create, modify, or remove. Required.
		/// </summary>
		[CommandLineParameter("do", "Specifies what to do - either create, modify, or remove. Required.", Required = true)]
		public SystemAccountApplication.Action Action { get; set; }

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
		/// Specifies the name of the system account to create or modify. Required.
		/// </summary>
		[CommandLineParameter("account", "a", "Specifies the name of the system account to create, modify or remove. Required.", Required = true)]
		public string AccountName { get; set; }

		/// <summary>
		/// Specifies the password to be set on the account.
		/// </summary>
		[CommandLineParameter("password", "p", "Specifies the password to be set on the account. Required for 'create' and 'modify'.")]
		public string AccountPassword { get; set; }

		/// <summary>
		/// Specifies the name of the authority group to which the account should be assigned.
		/// If not specified, new accounts will be assigned to the 'Service Accounts' group.
		/// </summary>
		[CommandLineParameter("auth", "Specifies the name of the authority group to which the account should be assigned. If not specified, new accounts will be assigned to the 'Service Accounts' group.")]
		public string AuthorityGroup { get; set; }
	}
}
