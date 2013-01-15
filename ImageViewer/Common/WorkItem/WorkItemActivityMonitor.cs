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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public interface IWorkItemActivityMonitor : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler IsConnectedChanged;

        event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged;
        event EventHandler StudiesCleared;
        void Refresh();

    }

    public abstract partial class WorkItemActivityMonitor
    {
        static WorkItemActivityMonitor()
        {
            try
            {
                var service = Platform.GetService<IWorkItemActivityMonitorService>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch (EndpointNotFoundException)
            {
                //This doesn't mean it's not supported, it means it's not running.
                IsSupported = true;
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Work Item Activity Monitor is not supported.");
            }
            catch (UnknownServiceException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Work Item Activity Monitor is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Work Item Activity Monitor is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }
    }

    public abstract partial class WorkItemActivityMonitor : IWorkItemActivityMonitor
    {
        private static readonly object _instanceLock = new object();
        private static RealWorkItemActivityMonitor _instance;

        internal static volatile int _proxyCount = 0;

        internal WorkItemActivityMonitor()
        {
        }

        ~WorkItemActivityMonitor()
        {
            Dispose(false);
        }

        #region IWorkItemActivityMonitor Members

        public abstract bool IsConnected { get; }
        public abstract event EventHandler IsConnectedChanged;

        public abstract event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged;
        public abstract event EventHandler StudiesCleared;

        public void Refresh()
        {
            Platform.GetService<IWorkItemActivityMonitorService>(s => s.Refresh(new WorkItemRefreshRequest()));
        }

        #endregion

        public static bool IsRunning 
        {
            get
            {
                if (!IsSupported)
                    return false;

                using (var monitor = Create(false))
                    return monitor.IsConnected;
            }
        }

        public static IWorkItemActivityMonitor Create()
        {
            return Create(true);
        }

        public static IWorkItemActivityMonitor Create(bool useSynchronizationContext)
        {
            var syncContext = useSynchronizationContext ? SynchronizationContext.Current : null;
            if (useSynchronizationContext && syncContext == null)
                throw new ArgumentException("Current thread has no synchronization context.", "useSynchronizationContext");

            return Create(syncContext);
        }

        public static IWorkItemActivityMonitor Create(SynchronizationContext synchronizationContext)
        {
            if (!IsSupported)
                throw new NotSupportedException("The work item activity monitor is not supported.");

            lock (_instanceLock)
            {
                if (_instance == null)
                    _instance = new RealWorkItemActivityMonitor();

                ++_proxyCount;
                Platform.Log(LogLevel.Debug, "WorkItemActivityMonitor proxy created (count = {0})", _proxyCount);
                return new WorkItemActivityMonitorProxy(_instance, synchronizationContext);
            }
        }

        internal static void OnProxyDisposed()
        {
            lock (_instanceLock)
            {
                if (_proxyCount == 0)
                    return; //Should never happen, except possibly when there's unit tests running.

                --_proxyCount;
                Platform.Log(LogLevel.Debug, "WorkItemActivityMonitor proxy disposed (count = {0}).", _proxyCount);
                if (_proxyCount > 0)
                    return;

                var monitor = _instance;
                _instance = null;
                //No need to do this synchronously.
                ThreadPool.QueueUserWorkItem(ignore => monitor.Dispose());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error disposing WorkItemActivityMonitor.");
            }
        }

        #endregion
    }
}