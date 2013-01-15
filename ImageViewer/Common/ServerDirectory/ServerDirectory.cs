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
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public abstract class ServerDirectory : IServerDirectory
    {
        static ServerDirectory()
        {
            try
            {
                var service = Platform.GetService<IServerDirectory>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch(EndpointNotFoundException)
            {
                //This doesn't mean it's not supported, it means it's not running.
                IsSupported = true;
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Server Directory is not supported.");
            }
            catch (UnknownServiceException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Server Directory is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Server Directory is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        private static int GetSortKey(IDicomServiceNode serviceNode)
        {
            if (serviceNode.IsLocal)
                return 0; //local first.

            // TODO (CR Jun 2012): !!!!! This should be IsSupported<IStudyLoader>(), but it's in the wrong assembly !!!!!! Need to move IStudyLoader to Common.
            //Ones that can load, followed by those that can't.
            return serviceNode.StreamingParameters != null ? 1 : 2;
        }

        private static List<IDicomServiceNode> SortServers(IEnumerable<IDicomServiceNode> servers)
        {
            //Sort servers
            return servers.OrderBy(GetSortKey).ToList();
        }

        public static IDicomServiceNode GetLocalServer()
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetLocalServer();
            }
        }

        public static IDicomServiceNode GetRemoteServerByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            using (var bridge = new ServerDirectoryBridge())
            {
                return bridge.GetServerByName(name);
            }
        }

        public static IList<IDicomServiceNode> GetRemoteServersByAETitle(string aeTitle)
        {
            if (String.IsNullOrEmpty(aeTitle))
                return new List<IDicomServiceNode>();

            using (var bridge = new ServerDirectoryBridge())
            {
                return SortServers(bridge.GetServersByAETitle(aeTitle));
            }
        }

        public static IList<IDicomServiceNode> GetRemoteServers()
        {
            using (var bridge = new ServerDirectoryBridge())
            {
                return SortServers(bridge.GetServers());
            }
        }

        public static IList<IDicomServiceNode> GetPriorsServers(bool includeLocal)
        {
            List<ServerDirectoryEntry> entries = null;
            Platform.GetService<IServerDirectory>(s => entries = s.GetServers(new GetServersRequest()).ServerEntries);
            var priorsServers = entries.Where(e => e.IsPriorsServer).Select(e => e.ToServiceNode()).ToList();

            if (includeLocal) //local server always first.
                priorsServers.Add(GetLocalServer());

            return SortServers(priorsServers);
        }

        public abstract GetServersResult GetServers(GetServersRequest request);
        public abstract AddServerResult AddServer(AddServerRequest request);
        public abstract UpdateServerResult UpdateServer(UpdateServerRequest request);
        public abstract DeleteServerResult DeleteServer(DeleteServerRequest request);
        public abstract DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request);
    }
}
