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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ActivityMonitorTestApplication : IApplicationRoot
    {
        private IWorkItemActivityMonitor _workItemActivityMonitor;

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            Console.WriteLine("Starting WorkItemActivityMonitor test application ...");

            _workItemActivityMonitor = WorkItemActivityMonitor.Create(false);
            _workItemActivityMonitor.IsConnectedChanged += OnIsConnectedChanged;
            _workItemActivityMonitor.WorkItemsChanged += OnWorkItemsChanged;
            
            Console.WriteLine("Press <Enter> to terminate.");
            Console.WriteLine();
            Console.WriteLine();

            string message = String.Format("IsConnected={0}", _workItemActivityMonitor.IsConnected);
            Console.WriteLine(message);

            Console.ReadLine();

            _workItemActivityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
            _workItemActivityMonitor.WorkItemsChanged -= OnWorkItemsChanged;
            _workItemActivityMonitor.Dispose();
        }

        #endregion

        private void OnWorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
        {
        	foreach (var workItem in e.ChangedItems)
        	{
				if (workItem.Request != null)
				{
					Console.WriteLine("Received WorkItemsChanged event: {0}:{1}.", workItem.Request.ActivityTypeString,
									  workItem.Request.ActivityDescription);
					if (workItem.Progress != null)
						Console.WriteLine("  Progress: {0}, Details: {1}.", workItem.Progress.Status,
							workItem.Progress.StatusDetails);
				}
				else
					Console.WriteLine("Received WorkItemsChanged event.");
			}

        }

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            string message = String.Format("Received IsConnectedChanged event (value={0}).", _workItemActivityMonitor.IsConnected);
            Console.WriteLine(message);
        }
    }
}

#endif