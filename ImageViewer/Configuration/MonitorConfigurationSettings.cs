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

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class ViewerLaunchSettings
	{
		public static WindowBehaviour WindowBehaviour
		{
			get
			{
				return (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;
			}
		}

		public static bool AllowEmptyViewer
		{
			get { return MonitorConfigurationSettings.Default.AllowEmptyViewer; }
		}
	}

	[SettingsGroupDescription("Settings related to monitor configuration and multiple windows.")]
    [SettingsProvider(typeof(LocalFileOrDefaultValueSettingsProvider))]
	internal sealed class MonitorConfigurationSettings : ApplicationSettingsBase
	{
		private MonitorConfigurationSettings()
		{
		}

		private static MonitorConfigurationSettings defaultInstance = ((MonitorConfigurationSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MonitorConfigurationSettings())));

		public static MonitorConfigurationSettings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("1")]
		[System.Configuration.SettingsDescription("Specifies whether viewers should be launched in the main window, or in a separate dedicated window.")]
		public int WindowBehaviour
		{
			get
			{
				return ((int)(this["WindowBehaviour"]));
			}
			set
			{
				this["WindowBehaviour"] = value;
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		[System.Configuration.SettingsDescription("Specifies whether or not to allow a viewer to open when there are no images to display.")]
		public bool AllowEmptyViewer
		{
			get
			{
				return ((bool)(this["AllowEmptyViewer"]));
			}
			set
			{
				this["AllowEmptyViewer"] = value;
			}
		}
	}
}