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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public static class ServerTreeExtensions
    {
        public static ApplicationEntity ToDataContract(this IServerTreeDicomServer server)
        {
            Platform.CheckForNullReference(server, "server");
            var ae = new ApplicationEntity
                         {
                             Name = server.Name, 
                             AETitle = server.AETitle,
                             Location = server.Location,
                             ScpParameters = new ScpParameters(server.HostName, server.Port)
                         };

            if (server.IsStreaming)
                ae.StreamingParameters = new StreamingParameters(server.HeaderServicePort, server.WadoServicePort);
            
            return ae;
        }

        public static List<IDicomServiceNode> ToDicomServiceNodes(this IServerTreeNode serverTreeNode)
        {
            Platform.CheckForNullReference(serverTreeNode, "serverTreeNode");
            if (serverTreeNode.IsLocalServer)
                return new List<IDicomServiceNode>{ ServerDirectory.GetLocalServer() };

            if (serverTreeNode.IsServer)
                return new List<IDicomServiceNode>{((IServerTreeDicomServer)serverTreeNode).ToDicomServiceNode()};

            var group = (IServerTreeGroup) serverTreeNode;
            var childServers = new List<IDicomServiceNode>();
            childServers.AddRange(group.ChildGroups.SelectMany(g => g.ToDicomServiceNodes()));
            childServers.AddRange(group.Servers.Cast<IServerTreeDicomServer>().Select(g => g.ToDicomServiceNode()));
            return childServers;
        }

        public static IDicomServiceNode ToDicomServiceNode(this IServerTreeDicomServer server)
        {
            Platform.CheckForNullReference(server, "server");
            var contract = server.ToDataContract();
            return contract.ToServiceNode();
        }

        public static StoredServerGroup ToStoredServerGroup(this IServerTreeGroup serverTreeGroup)
        {
            var storedServerGroup = new StoredServerGroup(serverTreeGroup.Name);
            foreach (var childGroup in serverTreeGroup.ChildGroups)
                storedServerGroup.ChildGroups.Add(childGroup.ToStoredServerGroup());

            foreach (var childServer in serverTreeGroup.Servers)
                storedServerGroup.DirectoryServerReferences.Add(new DirectoryServerReference { Name = childServer.Name });

            return storedServerGroup;
        }

        internal static ServerTreeGroup ToServerTreeGroup(this StoredServerGroup storedServerGroup, List<ApplicationEntity> servers)
        {
            var serverTreeGroup = new ServerTreeGroup(storedServerGroup.Name);
            foreach (var childGroup in storedServerGroup.ChildGroups)
                serverTreeGroup.ChildGroups.Add(childGroup.ToServerTreeGroup(servers));

            foreach (var directoryServerReference in storedServerGroup.DirectoryServerReferences)
            {
                var reference = directoryServerReference;
                var foundServerIndex = servers.FindIndex(d => d.Name == reference.Name);
                if (foundServerIndex >= 0)
                {
                    var server = (IApplicationEntity)servers[foundServerIndex];
                    servers.RemoveAt(foundServerIndex);
                    serverTreeGroup.Servers.Add(new ServerTreeDicomServer(server));
                }
            }

            return serverTreeGroup;
        }
    }
}
