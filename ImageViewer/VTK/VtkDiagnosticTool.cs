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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Vtk.Rendering;

namespace ClearCanvas.ImageViewer.Vtk
{
	[MenuAction("fps", "global-menus/MenuTools/MenuDiagnostics/MenuVtkShowFps", "ToggleFps")]
	[GroupHint("fps", "Tools.Diagnostics.VTK.Rendering")]
	[VisibleStateObserver("fps", "Enabled", null)]
	//
	[MenuAction("report", "global-menus/MenuTools/MenuDiagnostics/MenuVtkSystemReport", "ShowSystemReport")]
	[GroupHint("report", "Tools.Diagnostics.VTK.System")]
	[VisibleStateObserver("report", "Enabled", null)]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal sealed class VtkDiagnosticTool : ImageViewerTool
	{
		public override void Initialize()
		{
			base.Initialize();

			Enabled = Settings.Default.ShowDiagnosticTools;
		}

		public void ToggleFps()
		{
			try
			{
				VtkPresentationImageRenderer.ShowFps = !VtkPresentationImageRenderer.ShowFps;

				ImageViewer.PhysicalWorkspace.Draw();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}

		public void ShowSystemReport()
		{
			try
			{
				string log;
				string msg;

				var lines = Win32VtkRenderingSurface.ReportCapabilities().Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
				if (lines.Length > 16)
				{
					msg = string.Join(Environment.NewLine, lines, 0, 12) + Environment.NewLine + "(see logs for complete report)";
					log = string.Join(Environment.NewLine, lines);
				}
				else
				{
					log = msg = string.Join(Environment.NewLine, lines);
				}

				Context.DesktopWindow.ShowMessageBox(msg, MessageBoxActions.Ok);
				Platform.Log(LogLevel.Info, "VTK System Info" + Environment.NewLine + Environment.NewLine + log);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}
	}
}