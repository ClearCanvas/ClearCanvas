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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [MenuAction("showHide", "imageviewer-contextmenu/MenuShowHideShutterOverlay", "ShowHide", InitiallyAvailable = false)]
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideShutterOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideShutterOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Shutter.ShowHide")]
	[IconSet("showHide", "Icons.ShutterOverlayToolSmall.png", "Icons.ShutterOverlayToolMedium.png", "Icons.ShutterOverlayToolLarge.png")]
	//
    [ButtonAction("toggle", "overlays-dropdown/ToolbarShutterOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipShutterOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.Shutter.ShowHide")]
	[IconSet("toggle", "Icons.ShutterOverlayToolSmall.png", "Icons.ShutterOverlayToolMedium.png", "Icons.ShutterOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShutterOverlayTool : OverlayToolBase
	{
		public ShutterOverlayTool()
		{
		}

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				DicomGraphicsPlane dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(image as IDicomPresentationImage, false);
				if (dicomGraphics != null)
					dicomGraphics.Shutters.Enabled = Checked;
			}
		}
	}
}