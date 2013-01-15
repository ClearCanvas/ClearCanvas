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

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.DicomServer.Tests
{
    public class DicomServerTestServiceProvider : IServiceProvider
    {
        public static void Reset()
        {
            TestDicomServer.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IDicomServerConfiguration) == serviceType)
                return new DicomServerConfigurationProxy();

            if (typeof(IDicomServer) == serviceType)
                return new TestDicomServer();

            return null;
        }

        #endregion
    }

    public class TestDicomServer : IDicomServer
    {
        public static ServiceStateEnum ServiceState;

        static TestDicomServer()
        {
            Reset();
        }

        public static void Reset()
        {
            ServiceState = ServiceStateEnum.Stopped;
        }

        #region IDicomServer Members

        public GetListenerStateResult GetListenerState(GetListenerStateRequest request)
        {
            return new GetListenerStateResult {State = ServiceState};
        }

        public RestartListenerResult RestartListener(RestartListenerRequest request)
        {
            ServiceState = ServiceStateEnum.Started;
            return new RestartListenerResult();
        }

        #endregion
    }
}

#endif