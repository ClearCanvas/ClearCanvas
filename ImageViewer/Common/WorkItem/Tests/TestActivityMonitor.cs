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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    internal class TestActivityMonitor : IWorkItemActivityMonitor, IWorkItemActivityCallback
    {
        private bool _isConnected;

        #region IWorkItemActivityMonitor Members

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (Equals(value, _isConnected))
                    return;

                _isConnected = value;
                EventsHelper.Fire(IsConnectedChanged, this, EventArgs.Empty);
            }
        }

        public event EventHandler IsConnectedChanged;

        public string[] WorkItemTypeFilters { get; set; }
        public long[] WorkItemIdFilters { get; set; }

        public event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged;
        public event EventHandler StudiesCleared;
        
        public void Refresh()
    	{
    		throw new NotImplementedException();
    	}

    	#endregion

		void IWorkItemActivityCallback.WorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
        {
            EventsHelper.Fire(WorkItemsChanged, this, new WorkItemsChangedEventArgs(eventType, workItems));
        }

        void IWorkItemActivityCallback.StudiesCleared()
        {
            EventsHelper.Fire(StudiesCleared, this, EventArgs.Empty);
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}

#endif