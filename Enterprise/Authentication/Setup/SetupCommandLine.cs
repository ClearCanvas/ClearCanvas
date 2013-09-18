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

namespace ClearCanvas.Enterprise.Authentication.Setup
{
	class SetupCommandLine : CommandLine
	{
		/// <summary>
		/// Import default groups by default.
		/// </summary>
		private bool _importDefaultAuthorityGroups = true;
		private string _sysAdminUserName = "sa";
		private string _sysAdminDisplayName = "sysadmin";
		private string _sysAdminInitialPassword = "clearcanvas";

		/// <summary>
		/// Specifies whether to create default authority groups.
		/// </summary>
		[CommandLineParameter("groups", "g", "Specifies whether to import the default authority groups.")]
		public bool ImportDefaultAuthorityGroups
		{
			get { return _importDefaultAuthorityGroups; }
			set { _importDefaultAuthorityGroups = value; }
		}

		[CommandLineParameter("suid", "Specifies the system admin user-name.  Default is 'sa'.")]
		public string SysAdminUserName
		{
			get { return _sysAdminUserName; }
			set { _sysAdminUserName = value; }
		}

		[CommandLineParameter("sname", "Specifies the system admin user display name. Default is 'sysadmin'.")]
		public string SysAdminDisplayName
		{
			get { return _sysAdminDisplayName; }
			set { _sysAdminDisplayName = value; }
		}

		[CommandLineParameter("spwd", "Specifies the system admin user password.  Default is 'clearcanvas'.")]
		public string SysAdminInitialPassword
		{
			get { return _sysAdminInitialPassword; }
			set { _sysAdminInitialPassword = value; }
		}
	}
}
