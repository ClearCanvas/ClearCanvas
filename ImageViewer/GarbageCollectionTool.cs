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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
#if DEBUG
    [KeyboardAction("forcegc", "imageviewer-keyboard/ForceFullGarbageCollection", "ForceGC", KeyStroke = XKeys.G)]
    [KeyboardAction("reloadSettings", "imageviewer-keyboard/ReloadMemorySettings", "ReloadSettings", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("collectLargeObjects", "imageviewer-keyboard/CollectAllLargeObjects", "CollectAll", KeyStroke = XKeys.Control | XKeys.C)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    internal class ManualGarbageCollectionTool : Tool<IImageViewerToolContext>
    {
        public void ForceGC()
        {
            GarbageCollectionTool.ForceGC();
        }

        public void CollectAll()
        {
            Platform.Log(LogLevel.Info, "Forcing collection of all large objects.");
            
            bool keepGoing = true;
            EventHandler<MemoryCollectedEventArgs> del = delegate(object sender, MemoryCollectedEventArgs args)
            {
                if (args.IsLast)
                    keepGoing = args.BytesCollectedCount > 0;
            };

            MemoryManager.MemoryCollected += del;

            try
            {
                MemoryManager.Execute(delegate
                {
                    if (keepGoing)
                        throw new OutOfMemoryException();
                }, TimeSpan.FromSeconds(30));
            }
            catch (Exception)
            {
            }
            finally
            {
                MemoryManager.MemoryCollected -= del;
            }

            ForceGC();
        }

        public void ReloadSettings()
        {
            try
            {
                Platform.Log(LogLevel.Info, "Forcing reload of MemoryManagementSettings.");

                var groups = SettingsGroupDescriptor.ListInstalledSettingsGroups(SettingsGroupFilter.LocalStorage);
                var settingsGroups = groups.First(g => g.Name == "ClearCanvas.ImageViewer.Common.MemoryManagementSettings");
                var instance = ApplicationSettingsHelper.GetSettingsClassInstance(Type.GetType(settingsGroups.AssemblyQualifiedTypeName));
                instance.Reload();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Context.DesktopWindow);
            }
        }
    }
#endif

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

		private void OnWorkspaceClosed(object sender, ItemEventArgs<Workspace> e)
		{
            ForceGC();
		}

        internal static void ForceGC()
        {
            Platform.Log(LogLevel.Debug, "Forcing full garbage collection.");

            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                Thread.Sleep(2000);
                                                GC.Collect();
                                             });
        }
	}
}
