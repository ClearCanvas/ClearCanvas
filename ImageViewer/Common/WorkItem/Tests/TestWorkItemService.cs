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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    internal class TestWorkItemService : IWorkItemActivityMonitorService, IWorkItemActivityCallback, ICommunicationObject
    {
        private volatile bool _isSubscribed;
        private volatile CommunicationState _state;

        public TestWorkItemService()
        {
            State = CommunicationState.Opened;
        }

        public IWorkItemActivityCallback Callback { get; set; }

        #region IWorkItemActivityMonitorService Members

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            _isSubscribed = true;
            return new WorkItemSubscribeResponse();
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            _isSubscribed = false;
            return new WorkItemUnsubscribeResponse();
        }

        public void Refresh(WorkItemRefreshRequest request)
        {
        }

        public WorkItemPublishResponse Publish(WorkItemPublishRequest request)
        {            
            return new WorkItemPublishResponse();
        }

        #endregion

		void IWorkItemActivityCallback.WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
        {
            if (_isSubscribed)
                Callback.WorkItemsChanged(eventType, workItems);
        }

        void IWorkItemActivityCallback.StudiesCleared()
        {
            if (_isSubscribed)
                Callback.StudiesCleared();
        }

        #region ICommunicationObject Members

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            _isSubscribed = false;
            State = CommunicationState.Closed;
            EventsHelper.Fire(Closed, this, EventArgs.Empty);
        }

        public event EventHandler Closed;

        public event EventHandler Closing;

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Faulted;

        public void Open(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            State = CommunicationState.Opened;
            EventsHelper.Fire(Opened, this ,EventArgs.Empty);
        }

        public event EventHandler Opened;

        public event EventHandler Opening;

        public CommunicationState State
        {
            get { return _state; }
            private set { _state = value; }
        }

        #endregion

        public void Fault()
        {
            _isSubscribed = false;
            State = CommunicationState.Faulted;
            EventsHelper.Fire(Faulted, this, EventArgs.Empty);
        }
    }
}

#endif