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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using Timer=System.Threading.Timer;

namespace ClearCanvas.ImageViewer
{
	// This tool is basically a cheap hack to make sure that the garbage collector
	// runs a few times after a workspace is closed.  Performing a single GC 
	// when listening for a workspace removed event doesn't work since DotNetMagic
	// is still holding on to certain references at that point.  We have to wait
	// until the workspace is completely closed and all UI resources released
	// before we do the GC.  The easiest way to do that without hooking into 
	// the UI code itself is to get a timer to perform a GC a few times after
	// the workspace has been closed.


	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class GarbageCollectionTool : Tool<IDesktopToolContext>
	{
		public GarbageCollectionTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.DesktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.DesktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;
			base.Dispose(disposing);
		}

		void OnWorkspaceClosed(object sender, ItemEventArgs<Workspace> e)
		{
			// When a workspace has been closed, we want the GC to run
			// a few times to release what is often a significant amount of managed memory.

			WaitCallback del = delegate
			                   	{
									for (int i = 0; i < 5; ++i)
									{
										Thread.Sleep(500);
										GC.Collect();
									}
			                   	};

			ThreadPool.QueueUserWorkItem(del);
		}
	}
}
