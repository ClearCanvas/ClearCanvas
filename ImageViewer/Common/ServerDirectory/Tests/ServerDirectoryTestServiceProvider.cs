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
using System.Linq;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory.Tests
{
    public class ServerDirectoryTestServiceProvider : IServiceProvider
    {
        public static void Reset()
        {
            TestServerDirectory.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IServerDirectory) == serviceType)
                return new TestServerDirectory();

            return null;
        }

        #endregion
    }

    public class TestServerDirectory : IServerDirectory
    {
        public static List<ServerDirectoryEntry> Entries;

        static TestServerDirectory()
        {
            Reset();
        }

        public static void Reset()
        {
            Entries = new List<ServerDirectoryEntry>
                           {
                               new ServerDirectoryEntry
                                   {
                                       Server = new ApplicationEntity{Name = "Name1", AETitle = "AE1", ScpParameters = new ScpParameters("localhost", 104)},
                                       IsPriorsServer = true,
                                       Data = new Dictionary<string, object>{{"test1", "value1"}}
                                   },
                               new ServerDirectoryEntry
                                   {
                                       Server = new ApplicationEntity{Name = "Name2", AETitle = "AE2", ScpParameters = null},
                                       IsPriorsServer = false,
                                       Data = new Dictionary<string, object>{{"test2", "value2"}}
                                   },
                           };
        }

        #region IServerDirectory Members

        public GetServersResult GetServers(GetServersRequest request)
        {
            IEnumerable<ServerDirectoryEntry> entries = Entries;
            if (!String.IsNullOrEmpty(request.Name))
                entries = entries.Where(e => e.Server.Name == request.Name);
            else if (!String.IsNullOrEmpty(request.AETitle))
                entries = entries.Where(e => e.Server.AETitle == request.AETitle);

            return new GetServersResult { ServerEntries = entries.ToList() };
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            Entries.Add(request.ServerEntry);
            return new AddServerResult { ServerEntry = request.ServerEntry };
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            var index = Entries.FindIndex(e => e.Server.Name == request.ServerEntry.Server.Name);
            Entries[index] = request.ServerEntry;
            return new UpdateServerResult { ServerEntry = request.ServerEntry };
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            var index = Entries.FindIndex(e => e.Server.Name == request.ServerEntry.Server.Name);
            Entries.RemoveAt(index);
            return new DeleteServerResult();
        }

        #endregion
    }
}

#endif