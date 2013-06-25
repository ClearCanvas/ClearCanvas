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

#if UNIT_TESTS

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Vtk.Rendering;

namespace ClearCanvas.ImageViewer.Vtk.Tests
{
	[MenuAction("testimage", "global-menus/VTK Debug/Test Image", "TestImage")]
	[MenuAction("fps", "global-menus/VTK Debug/Show'/Hide FPS", "ShowHideFps")]
	[MenuAction("report", "global-menus/VTK Debug/System Info", "SystemReport")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class VtkRendererTestTool : ImageViewerTool
	{
		public void TestImage()
		{
			if (ImageViewer.SelectedImageBox == null) return;

			try
			{
				var sphere = new EllipsoidPrimitive {FrontTopLeft = new Vector3D(85, 85, 85), BackBottomRight = new Vector3D(115, 115, 115), Color = Color.Yellow};

				var image = new VtkTestPresentationImage();
				image.OverlayGraphics3D.Add(sphere);

				var dset = new DisplaySet {Description = "blah", Name = "blah", Uid = Guid.NewGuid().ToString()};
				dset.PresentationImages.Add(image);

				ImageViewer.SelectedImageBox.DisplaySet = dset;
				ImageViewer.SelectedImageBox.Draw();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}

		public void ShowHideFps()
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

		public void SystemReport()
		{
			try
			{
				string log;
				string msg;

				var lines = Win32VtkRenderingSurface.ReportCapabilities().Split('\r', '\n');
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

#endif