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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Help
{
	[MenuAction("getSupport", "global-menus/MenuHelp/MenuGetSupport", "ShowProductSupport")]
	[GroupHint("getSupport", "Application.Help.Support")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint), Enabled = false)]
	public sealed class SupportTool : Tool<IDesktopToolContext>
	{
		public void ShowProductSupport()
		{
			Execute(@"http://www.clearcanvas.ca/support/", SR.URLNotFound);
		}

		private void Execute(string filename, string errorMessage)
		{
			bool showMessageBox = String.IsNullOrEmpty(filename);
			if (!showMessageBox)
			{
				try
				{
					ProcessStartInfo info = new ProcessStartInfo();
					info.WorkingDirectory = Platform.InstallDirectory;
					info.FileName = filename;

					Process.Start(info);
				}
				catch (Exception e)
				{
					showMessageBox = true;
					Platform.Log(LogLevel.Warn, e, "Failed to launch '{0}'.", filename);
				}
			}

			if (showMessageBox)
				Context.DesktopWindow.ShowMessageBox(errorMessage, MessageBoxActions.Ok);
		}
	}
}