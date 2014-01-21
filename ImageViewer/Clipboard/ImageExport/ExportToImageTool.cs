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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
	[EnabledStateObserver("export", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("export", AuthorityTokens.Clipboard.Export.Jpeg)]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
		public ExportToImageTool()
		{
		}

		public void Export()
		{
			try
			{
				List<IClipboardItem> selectedClipboardItems = new List<IClipboardItem>(this.Context.SelectedClipboardItems);
				ImageExportComponent.Launch(this.Context.DesktopWindow, selectedClipboardItems);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageExportFailed, Context.DesktopWindow);
			}
		}
	}
}