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
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "imageviewer-contextmenu/MenuShowHideTextOverlay", "ShowHide", InitiallyAvailable = false)]
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideTextOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideTextOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("showHide", "Icons.TextOverlayToolSmall.png", "Icons.TextOverlayToolMedium.png", "Icons.TextOverlayToolLarge.png")]
	//
    [ButtonAction("toggle", "overlays-dropdown/ToolbarTextOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipTextOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("toggle", "Icons.TextOverlayToolSmall.png", "Icons.TextOverlayToolMedium.png", "Icons.TextOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class TextOverlayTool : OverlayToolBase
	{
		public TextOverlayTool()
		{
		}

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IAnnotationLayoutProvider)
			{
				foreach (AnnotationBox box in ((IAnnotationLayoutProvider)image).AnnotationLayout.AnnotationBoxes)
					box.Visible = visible;
			}
		}
	}
}
