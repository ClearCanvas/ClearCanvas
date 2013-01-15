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

namespace ClearCanvas.ImageViewer.BaseTools
{
	[SettingsGroupDescription("Settings for mouse tools in the ImageViewer.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	//TODO: make "universal" and re-enable for next release.
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal partial class MouseToolSettings
	{
		public MouseToolSettings()
		{
            // TODO (Phoenix5): Hack for RSNA that must be removed.
			//ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}