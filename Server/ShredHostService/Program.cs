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

using System.ServiceProcess;
using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHostService
{
    static class Program
    {
		private class CommandLine : Common.Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{}

			[CommandLineParameter("service", "s", "Instructs the application that it is to run as a service.", Required = false)]
			public bool RunAsService { get; set; }

			[CommandLineParameter("migrate", "m", "Migrates settings from a previous version of the application, given the previous config filename.", Required = false)]
			public string PreviousExeConfigurationFilename { get; set; }
		}

    	/// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
			Platform.Log(LogLevel.Info, ProductInformation.GetNameAndVersion(true,true,true,true));

			var commandLine = new CommandLine(args);

			if (commandLine.RunAsService)
			{
				var ServicesToRun = new ServiceBase[] { new ShredHostService() };
				ServiceBase.Run(ServicesToRun);
			}
			else if (!String.IsNullOrEmpty(commandLine.PreviousExeConfigurationFilename))
			{
				var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.LocalStorage);
				groups.Add(new SettingsGroupDescriptor(typeof (ShredSettingsMigrator).Assembly.GetType("ClearCanvas.Server.ShredHost.ShredHostServiceSettings")));
				foreach (var group in groups)
					SettingsMigrator.MigrateSharedSettings(group, commandLine.PreviousExeConfigurationFilename);

				ShredSettingsMigrator.MigrateAll(commandLine.PreviousExeConfigurationFilename);
			}
			else
			{
				ShredHostService.InternalStart();
				Console.WriteLine("Press <Enter> to terminate the ShredHost.");
				Console.WriteLine();
				Console.ReadLine();
				ShredHostService.InternalStop();
			}
        }
    }
}