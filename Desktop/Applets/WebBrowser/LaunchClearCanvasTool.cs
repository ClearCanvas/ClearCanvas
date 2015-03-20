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
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	[ButtonAction("activate1", "webbrowser-toolbar/ClearCanvas", "LaunchClearCanvas")]
	[Tooltip("activate1", "Show ClearCanvas Home")]
	[IconSet("activate1", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]
	//
	[ButtonAction("activate2", "webbrowser-toolbar/ClearCanvas Open Source Projects", "LaunchGitHub")]
	[Tooltip("activate2", "Show ClearCanvas Open Source Projects Home")]
	[IconSet("activate2", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]
	//
	[ExtensionOf(typeof (WebBrowserToolExtensionPoint))]
	public class LaunchClearCanvasTool : Tool<IWebBrowserToolContext>
	{
		public void LaunchClearCanvas()
		{
			this.Context.Url = "http://www.clearcanvas.ca";
			this.Context.Go();
		}

		public void LaunchGitHub()
		{
			this.Context.Url = "https://clearcanvas.github.io";
			this.Context.Go();
		}
	}
}