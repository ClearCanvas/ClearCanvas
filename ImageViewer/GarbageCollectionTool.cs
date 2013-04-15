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
using System.Reflection;
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

    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    internal class GarbageCollectionTool : Tool<IImageViewerToolContext>
    {
        private static int _count;
        private static volatile bool _isRunning;
        private bool _initialized;
        private bool _disposed;

        ~GarbageCollectionTool()
        {
            InternalDispose();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (!_initialized)
            {
                Interlocked.Increment(ref _count);
                _initialized = true;
            }
        }

        private void InternalDispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            //When there are viewers actively open, and even when there are active "large objects" outside the viewer(s) (e.g. clipboard),
            //the MemoryManager is looking after garbage collection. However, when all viewers have closed, it makes sense to just force
            //garbage collection to bring us back to "zero".
            if (Interlocked.Decrement(ref _count) <= 0)
                ForceGC();
        }

        internal static void ForceGC()
        {
            if (_isRunning) return;
            _isRunning = true;
            ThreadPool.QueueUserWorkItem((ignore) =>
            {
                //Sleep for a couple seconds to let things settle.
                Thread.Sleep(2000);

                for (int i = 0; i < 3; ++i)
                {
                    //Sometimes the first GC doesn't get everything, so as long as there's no new viewers open yet, we collect a couple extra times.
                    if (Thread.VolatileRead(ref _count) > 0)
                        break;

                    GC.Collect();
                    Thread.Sleep(1000);
                }
                _isRunning = false;
            }, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            InternalDispose();
        }
    }
}
