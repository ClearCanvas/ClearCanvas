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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal class RealWorkItemActivityMonitor : WorkItemActivityMonitor
    {
        [CallbackBehavior(UseSynchronizationContext = false)]
        private class Callback : WorkItemActivityCallback
        {
            private readonly RealWorkItemActivityMonitor _realActivityMonitor;

            internal Callback(RealWorkItemActivityMonitor real)
            {
                _realActivityMonitor = real;
            }

            public override void WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
            {
                _realActivityMonitor.OnWorkItemsChanged(eventType, workItems);
            }

            public override void StudiesCleared()
            {
                _realActivityMonitor.OnStudiesCleared();
            }
        }

        internal static TimeSpan ConnectionRetryInterval = TimeSpan.FromSeconds(5);

        private readonly object _syncLock = new object();

        private Thread _connectionThread;
        private bool _disposed;

        private bool _subscribedToService;
        
        private event EventHandler _isConnectedChanged;
        private event EventHandler<WorkItemsChangedEventArgs> _workItemsChanged;
        private event EventHandler _studiesCleared;

        private volatile IWorkItemActivityMonitorService _client;

        internal RealWorkItemActivityMonitor()
        {
            _connectionThread = new Thread(MonitorConnection) {IsBackground = true};
            lock (_syncLock)
            {
                _connectionThread.Start();
                Monitor.Wait(_syncLock); //Wait for the thread to start up.
            }
        }

        public override bool IsConnected
        {
            get { return _client != null; }
        }

        public override event EventHandler IsConnectedChanged
        {
            add
            {
                lock (_syncLock)
                {
                    _isConnectedChanged += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _isConnectedChanged -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }

        public override event EventHandler StudiesCleared
        {
            add
            {
                lock (_syncLock)
                {
                    _studiesCleared += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studiesCleared -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }


        public override event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged
        {
            add
            {
                lock (_syncLock)
                {
                    _workItemsChanged += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _workItemsChanged -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }

        private void MonitorConnection(object ignore)
        {
            lock (_syncLock)
            {
                //Try to connect first.
                Connect();
                ManageSubscription();

                //startup pulse.
                Monitor.Pulse(_syncLock);
            }

            while (true)
            {
                lock (_syncLock)
                {
                    //Check disposed before and after the wait because it could have changed.
                    if (_disposed) break;

                    Monitor.Wait(_syncLock, ConnectionRetryInterval);
                    if (_disposed) break;
                }

                Connect();
                ManageSubscription();
            }

            DropConnection();
            ManageSubscription();
        }

        private void ManageSubscription()
        {
            if (_client == null)
            {
                //No need to sync because this variable's only used on one thread.
                _subscribedToService = false;
                return;
            }

            bool subscribe;
            lock (_syncLock)
            {
                var hasListeners = _workItemsChanged != null;
                bool needsSubscriptionChange = _subscribedToService != hasListeners;
                if (!needsSubscriptionChange)
                    return;

                subscribe = hasListeners;
            }

            try
            {
                if (subscribe)
                {
                    _client.Subscribe(new WorkItemSubscribeRequest());
                    _subscribedToService = true;
                }
                else
                {
                    _client.Unsubscribe(new WorkItemUnsubscribeRequest());
                    _subscribedToService = false;
                }
            }
            catch(Exception e)
            {
                _subscribedToService = false;
                Platform.Log(LogLevel.Warn, e, "Failed to subscribe/unsubscribe from ActivityMonitorService.");
            }
        }

        private void Connect()
        {
            if (_client != null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to connect to ActivityMonitorService.");

            try
            {
                var callback = new Callback(this);
                var client = Platform.GetDuplexService<IWorkItemActivityMonitorService, IWorkItemActivityCallback>(callback);
                var communication = (ICommunicationObject)client;
                if (communication.State == CommunicationState.Created)
                    communication.Open();

                communication.Closed += OnConnectionClosed;
                communication.Faulted += OnConnectionFaulted;

                Platform.Log(LogLevel.Debug, "Successfully connected to ActivityMonitorService.");

                _client = client;
                FireIsConnectedChanged();
            }
            catch (EndpointNotFoundException)
            {
                Platform.Log(LogLevel.Debug, "ActivityMonitorService is not running.");
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error trying to connect to ActivityMonitorService.");
            }
        }

        void OnConnectionFaulted(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscription();
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscription();
        }

        private void DropConnection()
        {
            if (_client == null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to disconnect from ActivityMonitorService.");

            try
            {
                var communication = (ICommunicationObject)_client;
                communication.Closed -= OnConnectionClosed;
                communication.Faulted -= OnConnectionFaulted;

                if (communication.State == CommunicationState.Opened)
                    communication.Close();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error disconnecting from ActivityMonitorService.");
            }

            _client = null;
            FireIsConnectedChanged();
        }

        private void FireIsConnectedChanged()
        {
            Delegate[] delegates;
            lock (_syncLock)
            {
                if (_disposed)
                    return;

                delegates = _isConnectedChanged != null ? _isConnectedChanged.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Length > 0)
            {
                //ThreadPool.QueueUserWorkItem(ignore => CallDelegates(delegates, EventArgs.Empty));
                CallDelegates(delegates, EventArgs.Empty);
            }
        }

		private void OnWorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
        {
            IList<Delegate> delegates;
            lock (_syncLock)
            {
                if (_disposed)
                    return;

                delegates = _workItemsChanged != null ? _workItemsChanged.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Count <= 0)
                return;

			var args = new WorkItemsChangedEventArgs(eventType, workItems);
            CallDelegates(delegates, args);
        }

        private void OnStudiesCleared()
        {
            IList<Delegate> delegates;
            lock (_syncLock)
            {
                delegates = _studiesCleared != null ? _studiesCleared.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Count > 0)
                CallDelegates(delegates, EventArgs.Empty);
        }

        private void CallDelegates(IEnumerable<Delegate> delegates, EventArgs e)
        {
            foreach (var @delegate in delegates)
            {
                try
                {
                    @delegate.DynamicInvoke(this, e);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error encountered while firing event.");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            lock (_syncLock)
            {
                if (_disposed)
                    return;

                _disposed = true; //Setting disposed causes the thread to stop.
                Monitor.Pulse(_syncLock);

                //Don't bother joining - no point.
                _connectionThread = null;
            }
        }
    }
}