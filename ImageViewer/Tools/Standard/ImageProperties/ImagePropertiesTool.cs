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

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "global-menus/MenuView/MenuImageProperties", "Show", KeyStroke = XKeys.Control | XKeys.P)]
	[MenuAction("show", "imageviewer-contextmenu/MenuImageProperties", "Show")]
	[Tooltip("show", "TooltipImageProperties")]
	[IconSet("show", "ImagePropertiesToolSmall.png", "ImagePropertiesToolMedium.png", "ImagePropertiesToolLarge.png")]
	[GroupHint("show", "Tools.Image.Information")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ImagePropertiesTool : ImageViewerTool
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
		[ThreadStatic]
		private static Dictionary<IDesktopWindow, IShelf> _shelves;
		
		public ImagePropertiesTool()
		{
		}

		private static Dictionary<IDesktopWindow, IShelf> Shelves
		{
			get
			{
				if (_shelves == null)
					_shelves = new Dictionary<IDesktopWindow, IShelf>();
				return _shelves;
			}
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (Shelves.ContainsKey(Context.DesktopWindow))
					return Shelves[Context.DesktopWindow];

				return null;
			}
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = Context.DesktopWindow;
					
					ImagePropertiesApplicationComponent component =
						new ImagePropertiesApplicationComponent(Context.DesktopWindow);

					IShelf shelf = ApplicationComponent.LaunchAsShelf(Context.DesktopWindow, component,
						SR.TitleImageProperties, "ImageProperties", ShelfDisplayHint.DockLeft);

					Shelves.Add(Context.DesktopWindow, shelf);
					shelf.Closed += delegate { Shelves.Remove(desktopWindow); };
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}
	}
}