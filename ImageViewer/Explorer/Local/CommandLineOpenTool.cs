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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class CommandLineOpenTool : Tool<IDesktopToolContext>
	{
		private static bool _alreadyProcessed = false;

		public CommandLineOpenTool()
		{
		}

		public override void Initialize()
		{
			if (_alreadyProcessed)
				return;

			_alreadyProcessed = true;

			base.Initialize();

			try
			{
				List<string> args = new List<string>(Environment.GetCommandLineArgs());
				if (args.Count > 0)
					args.RemoveAt(0); //remove process name.

				if (args.Count > 0)
				{
					if (!PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
					{
						this.Context.DesktopWindow.ShowMessageBox(SR.MessagePermissionToOpenFilesDenied, MessageBoxActions.Ok);
						return;
					}

					CommandLine commandLine = new CommandLine(args.ToArray());
					List<string> files = BuildFileList(commandLine.Positional);
					if (files.Count > 0)
						new OpenFilesHelper(files) {WindowBehaviour = ViewerLaunchSettings.WindowBehaviour}.OpenFiles();
					else
						this.Context.DesktopWindow.ShowMessageBox(SR.MessageFileNotFound, MessageBoxActions.Ok);
				}
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageUnableToOpenImages, Context.DesktopWindow);
			}
		}

		private static List<string> BuildFileList(IEnumerable<string> files)
		{
			List<string> fileList = new List<string>();

			foreach (string path in files)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList;
		}
	}
}
