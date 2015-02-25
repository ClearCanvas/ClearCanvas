#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	public class ApplicationSettingsMigrationRoot : IApplicationRoot
	{
		private static readonly string[] _applicationSettingsClasses =
		{
			@"ClearCanvas.ImageViewer.Configuration.DicomPublishingSettings, ClearCanvas.ImageViewer.Configuration",
			@"ClearCanvas.ImageViewer.Configuration.ServerTree.ServerTreeSettings, ClearCanvas.ImageViewer.Configuration",
			@"ClearCanvas.ImageViewer.Common.DicomServer.DicomServerSettings, ClearCanvas.ImageViewer.Common",
			@"ClearCanvas.ImageViewer.Common.StudyManagement.StorageSettings, ClearCanvas.ImageViewer.Common",
			@"ClearCanvas.ImageViewer.Common.WorkItem.DicomSendSettings, ClearCanvas.ImageViewer.Common",
			@"ClearCanvas.ImageViewer.Common.StudyManagement.StudyDeletionSettings, ClearCanvas.ImageViewer.Common"
		};

		private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
		{
			public CommandLine()
			{
				DicomServersFileName = "DicomAEServers.xml";
			}

			[CommandLineParameter(0, @"source application configuration file", Required = true)]
			public string Source { get; set; }

			[CommandLineParameter(1, @"target application configuration file", Required = true)]
			public string Target { get; set; }

			[CommandLineParameter("serversxml", "sx", "Path to old DicomAEServers xml file.", Required = false)]
			public string DicomServersFileName { get; set; }
		}

		protected virtual IEnumerable<string> ApplicationSettingsClasses
		{
			get { return _applicationSettingsClasses; }
		}

		public void RunApplication(string[] args)
		{
			var cmd = new CommandLine();
			try
			{
				cmd.Parse(args);
			}
			catch (Exception)
			{
				cmd.PrintUsage(Console.Out);
				Environment.Exit(-1);
			}

			//Hack to redirect local shared settings to a different exe's config file.
			ExtendedLocalFileSettingsProvider.ExeConfigFileName = cmd.Target;
			foreach (var settingsClass in _applicationSettingsClasses)
			{
				var settingsType = Type.GetType(settingsClass);
				SettingsMigrator.MigrateSharedSettings(settingsType, cmd.Source);
			}

			try
			{
				if (!String.IsNullOrEmpty(cmd.DicomServersFileName) && File.Exists(cmd.DicomServersFileName))
				{
					var existingServerTree = new ServerTree.ServerTree();
					if (existingServerTree.RootServerGroup.GetAllServers().Count == 0)
					{
						//Settings NOT from an old xml file were just migrated, so
						//if there's still no servers defined, import from old xml file.
						var serverTree = new ServerTree.ServerTree(cmd.DicomServersFileName);
						serverTree.Save();
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to import legacy server tree '{0}'.", cmd.DicomServersFileName);
			}
		}
	}
}