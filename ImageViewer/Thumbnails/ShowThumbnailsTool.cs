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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Thumbnails.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	[MenuAction("show", "global-menus/MenuView/MenuShowThumbnails", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowThumbnails", "Show")]
	[Tooltip("show", "TooltipShowThumbnails")]
	[IconSet("show", "Icons.ShowThumbnailsToolSmall.png", "Icons.ShowThumbnailsToolMedium.png", "Icons.ShowThumbnailsToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShowThumbnailsTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();

		public ShowThumbnailsTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_shelves.ContainsKey(this.Context.DesktopWindow))
					return _shelves[this.Context.DesktopWindow];

				return null;
			}	
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = this.Context.DesktopWindow;

					IShelf shelf = ThumbnailComponent.Launch(desktopWindow);
					shelf.Closed += delegate
					                	{
					                		_shelves.Remove(desktopWindow);
					                	};

					_shelves[this.Context.DesktopWindow] = shelf;
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			if (ThumbnailsSettings.Default.AutoOpenThumbnails)
			{
				this.Show();
			}
		}
	}
}
