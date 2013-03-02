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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuCine", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarCine", "Activate", KeyStroke = XKeys.C)]
	[Tooltip("activate", "TooltipCine")]
	[IconSet("activate", "Icons.CineToolSmall.png", "Icons.CineToolMedium.png", "Icons.CineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Cine")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class CineTool : ImageViewerTool
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic]
		private static Dictionary<IDesktopWindow, IShelf> _shelves;
		[ThreadStatic]
		private static Dictionary<IImageViewer, CineTool> _tools;

		private SynchronizationContext _synchronizationContext;

		public CineTool() {}

		private static Dictionary<IDesktopWindow, IShelf> Shelves
		{
			get
			{
				if (_shelves == null)
					_shelves = new Dictionary<IDesktopWindow, IShelf>();
				return _shelves;
			}	
		}

		private static Dictionary<IImageViewer, CineTool> Tools
		{
			get
			{
				if (_tools == null)
					_tools = new Dictionary<IImageViewer, CineTool>();
				return _tools;
			}
		}

		public void Activate()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;

			// check if a layout component is already displayed
			if (Shelves.ContainsKey(desktopWindow))
			{
				Shelves[desktopWindow].Activate();
			}
			else
			{
				LaunchShelf(desktopWindow, new CineApplicationComponent(desktopWindow), ShelfDisplayHint.DockFloat);
			}
		}

		protected IImageBox SelectedImageBox
		{
			get
			{
				if (this.ImageViewer == null)
					return null;
				return this.ImageViewer.SelectedImageBox;
			}
		}

		private static void LaunchShelf(IDesktopWindow desktopWindow, IApplicationComponent component, ShelfDisplayHint shelfDisplayHint)
		{
			IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleCine, "Cine", shelfDisplayHint);
			Shelves[desktopWindow] = shelf;
			Shelves[desktopWindow].Closed += OnShelfClosed;
		}

		private static void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			// We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
			// ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
			// exist at the DesktopWindow level and there can only be one of each type of shelf
			// open at the same time per DesktopWindow (otherwise things look funny).  Because of 
			// this, we need to allow this event handling method to be called after this tool has
			// already been disposed (e.g. viewer workspace closed), which is why we store the 
			// _desktopWindow variable.

			IShelf shelf = (IShelf) sender;
			shelf.Closed -= OnShelfClosed;
			Shelves.Remove(shelf.DesktopWindow);
		}
	}
}