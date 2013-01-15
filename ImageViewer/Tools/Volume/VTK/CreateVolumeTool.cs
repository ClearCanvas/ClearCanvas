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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	[ButtonAction("show", "global-toolbars/ToolbarsVolume/CreateVolumeTool", "Show")]
	[Tooltip("show", "Create Volume")]
	[IconSet("show", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolMedium.png", "Icons.CreateVolumeToolLarge.png")]
	[GroupHint("show", "Tools.VolumeImage.Create")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CreateVolumeTool : ImageViewerTool
	{
		private static VolumeComponent _volumeComponent;
		private static IShelf _volumeShelf;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public CreateVolumeTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		public void Show()
		{
			if (_volumeComponent == null)
			{
				// create and initialize the layout component
				_volumeComponent = new VolumeComponent(this.Context.DesktopWindow);

				// launch the layout component in a shelf
				_volumeShelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_volumeComponent,
					SR.TitleVolumeController,
					ShelfDisplayHint.DockLeft);

				_volumeShelf.Closed += VolumeShelf_Closed;
			}
		}

		private static void VolumeShelf_Closed(object sender, ClosedEventArgs e) {
			// note that the component is thrown away when the shelf is closed by the user
			_volumeShelf.Closed -= VolumeShelf_Closed;
			_volumeShelf = null;
			_volumeComponent = null;
		}
	}
}
