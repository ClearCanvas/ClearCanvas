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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuLocateOnDisk", "Activate")]
	[Tooltip("activate", "TooltipLocateOnDisk")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class LocateOnDiskTool : ImageViewerTool
    {
        public LocateOnDiskTool()
        {
        }

        public void Activate()
        {
            if (this.SelectedPresentationImage == null)
                return;

            IImageSopProvider image = this.SelectedPresentationImage as IImageSopProvider;
			if (image == null)
				return;

        	ILocalSopDataSource localSource = image.ImageSop.DataSource as ILocalSopDataSource;
			if (localSource == null)
			{
				base.Context.DesktopWindow.ShowMessageBox(SR.MessageUnableToLocateNonLocalImage, MessageBoxActions.Ok);
				return;
			}

			System.Diagnostics.Process.Start("explorer.exe", "/n,/select," + localSource.Filename);
        }
    }
}
