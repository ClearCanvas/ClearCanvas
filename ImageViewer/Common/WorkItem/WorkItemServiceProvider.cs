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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [ExtensionOf(typeof(DuplexServiceProviderExtensionPoint))]
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class WorkItemServiceProvider : IServiceProvider, IDuplexServiceProvider
    {
        #region Implementation of IDuplexServiceProvider

        public object GetService(Type type, object callback)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemActivityMonitorService))
                return null;

            Platform.CheckExpectedType(callback, typeof(IWorkItemActivityCallback));
            
            var client = new WorkItemActivityMonitorServiceClient(new InstanceContext(callback));
            if (client.State != CommunicationState.Opened)
                client.Open();

            return client;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            Platform.CheckForNullReference(serviceType, "serviceType");
            if (serviceType == typeof(IWorkItemService))
            {
                var client = new WorkItemServiceClient();
                if (client.State != CommunicationState.Opened)
                    client.Open();

                return client;
            }

            //Someone could be requesting a single-use instance of the activity monitor, I suppose.
            return GetService(serviceType, WorkItemActivityCallback.Nil);
        }

        #endregion
    }
}
