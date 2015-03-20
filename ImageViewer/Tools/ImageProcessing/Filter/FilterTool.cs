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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	// We decorate FilterTool with the DropDownButtonAction attribute
	// and set the path such that it shows up in the main toolbar. We specify that the
	// contents of the menu are to retrieved using the DropDownMenuModel property.
	[DropDownAction("apply", "global-toolbars/ToolbarStandard/ToolbarFilter", "DropDownMenuModel")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "Filters")]
	[IconSet("apply", "Icons.FilterToolSmall.png", "Icons.FilterToolMedium.png", "Icons.FilterToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class FilterTool : ImageViewerTool
	{
		public FilterTool() {}

		// We have to provide the dropdown button with the data to populate the dropdown menu.
		public ActionModelNode DropDownMenuModel
		{
			get
			{
				// The filter tools are ImageViewerToolExtensions, so we have to get the
				// actions from the ImageViewerComponent. Note that while 
				// ImageViewerComponent.ExportedActions gets *all* the actions associated with
				// the ImageViewerComponent, the fact that we specify the site (i.e.
				// imageviewer-filterdropdownmenu) when we call CreateModel will cause 
				// the model to only contain those actions which have that site specified
				// in its path.

				return ActionModelRoot.CreateModel(
					this.GetType().FullName,
					"imageviewer-filterdropdownmenu",
					this.ImageViewer.ExportedActions);
			}
		}
	}
}