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
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[DropDownButtonAction("dropdown", "global-toolbars/ToolbarStandard/ToolbarShowHideOverlays", "ToggleAll", "DropDownActionModel", KeyStroke = XKeys.O)]
	[Tooltip("dropdown", "TooltipShowHideOverlays")]
	[GroupHint("dropdown", "Tools.Image.Overlays.Text.ShowHide")]
	[IconSet("dropdown", "Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ShowHideOverlaysTool : ImageViewerTool
	{
		private ActionModelNode _mainDropDownActionModel;

		public ShowHideOverlaysTool() {}

		public ActionModelNode DropDownActionModel
		{
			get
			{
				if (_mainDropDownActionModel == null)
				{
					_mainDropDownActionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "overlays-dropdown", this.ImageViewer.ExportedActions);
				}
				return _mainDropDownActionModel;
			}
		}

		public void ToggleAll()
		{
			// get the current check state
			bool currentCheckState = true;
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
			{
				if (!tool.Checked)
				{
					currentCheckState = false;
					break;
				}
			}

			// invert the check state
			currentCheckState = !currentCheckState;

			// apply new check state to all
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
			{
				tool.Checked = currentCheckState;
			}

			this.Context.Viewer.PhysicalWorkspace.Draw();
		}
	}
}