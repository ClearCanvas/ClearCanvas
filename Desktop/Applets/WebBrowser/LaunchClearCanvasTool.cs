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


namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	[ButtonAction("activate1", "webbrowser-toolbar/ClearCanvas", "LaunchClearCanvas")]
	[Tooltip("activate1", "Launch ClearCanvas")]
	[IconSet("activate1", IconScheme.Colour, "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]

	[ButtonAction("activate2", "webbrowser-toolbar/Discussion Forum", "LaunchDiscussionForum")]
	[Tooltip("activate2", "Launch ClearCanvas Discussion Forum")]
	[IconSet("activate2", IconScheme.Colour, "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png", "Icons.ClearCanvasToolSmall.png")]

	[ExtensionOf(typeof(WebBrowserToolExtensionPoint))]
	public class LaunchClearCanvasTool : Tool<IWebBrowserToolContext>
	{
		public LaunchClearCanvasTool()
		{

		}

		private void LaunchClearCanvas()
		{
			this.Context.Url = "http://www.clearcanvas.ca";
			this.Context.Go();
		}

		private void LaunchDiscussionForum()
		{
			this.Context.Url = "http://www.clearcanvas.ca/dnn/Community/Forums/tabid/69/Default.aspx";
			this.Context.Go();
		}
	}
}
