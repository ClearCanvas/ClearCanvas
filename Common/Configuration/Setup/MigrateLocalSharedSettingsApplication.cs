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
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class MigrateLocalSharedSettingsApplication : IApplicationRoot
    {
		private class CommandLine : Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{
			}

			[CommandLineParameter(0, "The name of the local file where previous application scoped and default user settings should be taken from.", Required = true)]
			public string PreviousExeConfigurationFilename { get; set; }
		}
		
		#region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
			var commandLine = new CommandLine(args);

        	var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.LocalStorage);
			foreach (var group in groups)
				SettingsMigrator.MigrateSharedSettings(group, commandLine.PreviousExeConfigurationFilename);
        }

        #endregion
    }
}