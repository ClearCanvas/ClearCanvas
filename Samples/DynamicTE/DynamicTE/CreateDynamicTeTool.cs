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

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[MenuAction("show", "global-menus/MenuTools/MyTools/CreateDynamicTE", "Show")]
	[ButtonAction("show", "global-toolbars/MyTools/CreateDynamicTE", "Show")]
	[Tooltip("show", "CreateDynamicTE")]
	[IconSet("show", IconScheme.Colour, "Icons.CreateDynamicTeToolSmall.png", "Icons.CreateDynamicTeToolMedium.png", "Icons.CreateDynamicTeToolLarge.png")]
	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CreateDynamicTeTool : ImageViewerTool
	{
		private static DynamicTeComponent _cadComponent;

		public CreateDynamicTeTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
		}

		public void Show()
		{
			// check if a layout component is already displayed
			if (_cadComponent == null)
			{
				// create and initialize the layout component
				_cadComponent = new DynamicTeComponent(this.Context.DesktopWindow);

				// launch the layout component in a shelf
				// note that the component is thrown away when the shelf is closed by the user
				ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_cadComponent,
					"TE",
					ShelfDisplayHint.DockLeft,
					delegate(IApplicationComponent component) { _cadComponent = null; });
			}
		}

	}
}
