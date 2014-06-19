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
using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Application root for updating specific setting in the enterprise settings store.
	/// </summary>
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	public class SetSettingApplication : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SetSettingCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "setup", Dns.GetHostName(), cmdLine.Password))
				{

					ISettingsStore store;
					try
					{
						store = SettingsStore.Create();
						//If there is a store, and it is not online, then settings can't be edited.
						if (!store.IsOnline)
						{
							Platform.Log(LogLevel.Error, "Settings Store is not online, cannot update configuration");
							return;
						}
					}
					catch (NotSupportedException)
					{
						// There is no central settings store; all settings will be treated as though they were local.
						Platform.Log(LogLevel.Error, "No Enterprise settings store, cannot update configuration");
						return;
					}

					if (!string.IsNullOrEmpty(cmdLine.SettingData))
					{
						SetupHelper.ImportSettingsDefinition(store, cmdLine.SettingData, cmdLine.Overwrite);
					} 
					else
					{
						var settings = new List<SettingDefinition>
									{
										new SettingDefinition
											{
												Group = cmdLine.SettingGroup,
												Property = cmdLine.SettingName,
												Version = cmdLine.Version,
												Value = cmdLine.Value
											}
									};

						SetupHelper.ImportSettingsDefinition(store, settings, cmdLine.Overwrite);
					}
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				Platform.Log(LogLevel.Error, e, "Command line error.");
			}
		}

		#endregion
	}
}
