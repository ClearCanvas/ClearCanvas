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
	class GetSettingCommandLine : CommandLine
	{
		public GetSettingCommandLine()
		{
			Password = "clearcanvas";
			UserName = "sa";
		}

		/// <summary>
		/// Specifies the settings group to updates.
		/// </summary>
		[CommandLineParameter("settingGroup", "g", "Specifies the name of the settings group.")]
		public string SettingGroup { get; set; }

		/// <summary>
		/// Specifies the setting name with the group to update.
		/// </summary>
		[CommandLineParameter("settingName", "s", "Specifies the name of the setting within the group.")]
		public string SettingName { get; set; }

		/// <summary>
		/// Specifies the version of the settings groups.
		/// </summary>
		[CommandLineParameter("version", "n", "Specifies the version of the setting.")]
		public string Version { get; set; }

		/// <summary>
		/// Specifies the name of a file to export the setting.
		/// </summary>
		[CommandLineParameter("settingData", "Specifies the name of a file to export the setting.")]
		public string SettingData { get; set; }

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
	}
}
