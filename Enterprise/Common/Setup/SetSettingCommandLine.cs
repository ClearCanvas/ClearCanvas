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
    class SetSettingCommandLine : CommandLine
	{
		public SetSettingCommandLine()
		{
			Password = "clearcanvas";
			UserName = "sa";
		}

		/// <summary>
		/// Specifies the settings group to updates.
		/// </summary>
		[CommandLineParameter("settingGroup", "g", "Specifies the settings group to update.")]
		public string SettingGroup { get; set; }

		/// <summary>
		/// Specifies the setting name with the group to update.
		/// </summary>
		[CommandLineParameter("settingName", "s", "Specifies the setting name within the group to update.")]
		public string SettingName { get; set; }

		/// <summary>
		/// Specifies whether to import settings groups.
		/// </summary>
		[CommandLineParameter("version", "n", "(Optional) The version string of the setting to update.")]
		public string Version { get; set; }

		/// <summary>
		/// Specifies whether to import settings groups.
		/// </summary>
		[CommandLineParameter("value", "v", "The value to update the setting to.")]
		public string Value { get; set; }

		/// <summary>
		/// Specifies the name of a file or folder containing configuration data to be imported.
		/// </summary>
		[CommandLineParameter("overwrite", "o", "Specifies whether to overwrite existing setting if it has already been modified.")]
		public bool Overwrite { get; set; }

		/// <summary>
		/// Specifies the name of a file or folder containing configuration data to be imported.
		/// </summary>
		[CommandLineParameter("settingData", "Specifies the name of a file or folder containing settings to be imported.")]
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
