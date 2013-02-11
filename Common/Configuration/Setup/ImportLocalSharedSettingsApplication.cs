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
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Common.Configuration.Setup
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	internal class ImportLocalSharedSettingsApplication : IApplicationRoot
	{
		private class CommandLine : Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{
			}

			[CommandLineParameter(0, "The name of the local file where settings should be imported from.", Required = true)]
			public string ConfigurationFilename { get; set; }
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			CommandLine commandLine = new CommandLine(args);

			foreach (SettingsGroupDescriptor group in SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.LocalStorage))
			{
				Type type = Type.GetType(group.AssemblyQualifiedTypeName, true);
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(type);
				ApplicationSettingsExtensions.ImportSharedSettings(settings, commandLine.ConfigurationFilename);
			}
		}

		#endregion
	}
}
