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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Connects to the enterprise server and imports settings groups, authority tokens, and authority groups.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SetupApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SetupCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "setup", Dns.GetHostName(), cmdLine.Password))
				{
					// first import the tokens, since the default groups will likely depend on these tokens
					if (cmdLine.ImportAuthorityTokens)
					{
						SetupHelper.ImportAuthorityTokens(new[] { BuiltInAuthorityGroups.Administrators.Name });
					}

					// import authority groups
					if (cmdLine.ImportDefaultAuthorityGroups)
					{
						SetupHelper.ImportEmbeddedAuthorityGroups();
					}

					if(!string.IsNullOrEmpty(cmdLine.AuthorityGroupData))
					{
						SetupHelper.ImportAuthorityGroups(cmdLine.AuthorityGroupData);
					}

					// import settings groups
					if (cmdLine.ImportSettingsGroups)
					{
						ImportSettingsGroups();
					}

					if (cmdLine.MigrateSharedSettings)
					{
						MigrateSharedSettings(cmdLine.PreviousExeConfigFilename);
					}
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		#endregion

		private static void MigrateSharedSettings(string previousExeConfigFilename)
		{
			foreach (var group in SettingsGroupDescriptor.ListInstalledSettingsGroups())
			{
				try
				{
					SettingsMigrator.MigrateSharedSettings(group, previousExeConfigFilename);
				}
				catch (UnknownServiceException e)
				{
					//Failure to migrate a settings is not good enough reason to cause the whole app to fail.
					//Some of the viewer settings classes SHOULD actually fail to migrate in the context of the ImageServer.
					Platform.Log(LogLevel.Debug, e, "Failed to migrate settings '{0}'", group.AssemblyQualifiedTypeName);
				}
				catch (Exception e)
				{
					//Failure to migrate a settings is not good enough reason to cause the whole app to fail.
					//Some of the viewer settings classes SHOULD actually fail to migrate in the context of the ImageServer.
					Platform.Log(LogLevel.Warn, e, "Failed to migrate settings '{0}'", group.AssemblyQualifiedTypeName);
				}
			}
		}

		/// <summary>
		/// Import settings groups defined in local plugins.
		/// </summary>
		private static void ImportSettingsGroups()
		{
			var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.SupportEnterpriseStorage);

			foreach (var g in groups)
			{
				Platform.Log(LogLevel.Info, "Import settings group {0}, Version={1}, Type={2}", g.Name, g.Version.ToString(), g.AssemblyQualifiedTypeName);
			}

			Platform.GetService(
				delegate(Configuration.IConfigurationService service)
				{
					foreach (var group in groups)
					{
						var props = SettingsPropertyDescriptor.ListSettingsProperties(group);
						service.ImportSettingsGroup(new Configuration.ImportSettingsGroupRequest(group, props));
					}
				});
		}

	}
}
