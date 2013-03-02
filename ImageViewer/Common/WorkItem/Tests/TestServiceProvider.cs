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
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    // Use a UnitTestExtensionFactory instead of declaring this an extension, because it will break debug builds that include the UNIT_TESTS symbol. 
    internal class TestServiceProvider : IDuplexServiceProvider, IServiceProvider
    {
        internal static readonly TestWorkItemService ServiceInstance = new TestWorkItemService();

        #region Implementation of IDuplexServiceProvider

        public object GetService(Type type, object callback)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemActivityMonitorService))
                return null;

            Platform.CheckExpectedType(callback, typeof(IWorkItemActivityCallback));
            if (ServiceInstance.State != CommunicationState.Opened)
                throw new EndpointNotFoundException("Test service not running.");

            ServiceInstance.Callback = (IWorkItemActivityCallback)callback;
            return ServiceInstance;
        }

        #endregion

        #region Implementation of IServiceProvider

        public object GetService(Type type)
        {
            Platform.CheckForNullReference(type, "type");
            if (type != typeof(IWorkItemActivityMonitorService))
                return null;

            ServiceInstance.Callback = WorkItemActivityCallback.Nil;
            return ServiceInstance;
        }

        #endregion
    }
}

#endif