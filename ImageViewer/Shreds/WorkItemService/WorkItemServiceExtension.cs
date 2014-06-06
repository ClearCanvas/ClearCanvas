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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    [ShredIsolation(Level = ShredIsolationLevel.None)]
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class WorkItemServiceExtension : WcfShred
    {
        private const string _workItemServiceEndpointName = "WorkItemService";
        private const string _workItemActivityMonitorServiceEndpointName = "WorkItemActivityMonitor";
        private bool _workItemServiceWCFInitialized;
        private bool _workItemActivityMonitorServiceWCFInitialized;

        private readonly WorkItemProcessorExtension _processor;

        public WorkItemServiceExtension()
        {
            _workItemServiceWCFInitialized = false;
            _workItemActivityMonitorServiceWCFInitialized = false;
            _processor = new WorkItemProcessorExtension();
        }

        public override void Start()
        {
            try
            {
                WorkItemService.Instance.Start();
                string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.WorkItemService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.WorkItemService));
                return;
            }

            try
            {
                StartNetPipeHost<WorkItemServiceType, IWorkItemService>(_workItemServiceEndpointName, SR.WorkItemService);
                _workItemServiceWCFInitialized = true;
                string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.WorkItemService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.WorkItemService));
            }

            try
            {
                StartNetPipeHost<WorkItemActivityMonitorServiceType, IWorkItemActivityMonitorService>(_workItemActivityMonitorServiceEndpointName, SR.WorkItemActivityMonitorService);
                _workItemActivityMonitorServiceWCFInitialized = true;
                string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.WorkItemActivityMonitorService);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.WorkItemActivityMonitorService));
            }

            _processor.Start();
        }

        public override void Stop()
        {
            // Stop the processor first, so status updates go out.
            _processor.Stop();

            if (_workItemActivityMonitorServiceWCFInitialized)
            {
                try
                {
                    StopHost(_workItemActivityMonitorServiceEndpointName);
                    Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.WorkItemActivityMonitorService));
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }

            if (_workItemServiceWCFInitialized)
            {
                try
                {
                    StopHost(_workItemServiceEndpointName);
                    Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.WorkItemService));
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }

            try
            {
                WorkItemService.Instance.Stop();
                Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.WorkItemService));
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        public override string GetDisplayName()
        {
            return SR.WorkItemService;
        }

        public override string GetDescription()
        {
            return SR.WorkItemServiceDescription;
        }
    }
}
