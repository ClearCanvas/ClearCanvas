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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public partial class ServerTree
    {
        private const string _rootPath = "./";

        #region Private fields

        private event EventHandler _serverTreeUpdated;

        #endregion

        public IServerTreeLocalServer LocalServer { get; private set; }
        public IServerTreeGroup RootServerGroup { get; private set; }

        #region Public Properties / Events

        public IServerTreeNode CurrentNode { get; set; }

        public event EventHandler ServerTreeUpdated
        {
            add { _serverTreeUpdated += value; }
            remove { _serverTreeUpdated -= value; }
        }

        #endregion

        #region Public Methods

        public void FireServerTreeUpdatedEvent()
        {
            EventsHelper.Fire(_serverTreeUpdated, this, EventArgs.Empty);
        }

        #region Editing Rules

        public bool CanMove(IServerTreeNode destinationNode, IServerTreeNode addMoveNode)
        {
            if (destinationNode == addMoveNode || !destinationNode.IsServerGroup)
                return false;

            if (addMoveNode == RootServerGroup || addMoveNode == LocalServer)
                return false;

            if (addMoveNode.IsServer)
            {
                var server = (ServerTreeDicomServer)addMoveNode;
                string conflictingPath;
                if (IsConflictingServerInGroup((ServerTreeGroup)destinationNode, false, server.AETitle, server.HostName, server.Port, out conflictingPath))
                    return false;
            }
            else if (addMoveNode is ServerTreeGroup)
            {
                string conflictingPath;
                if (IsConflictingServerTreeGroupInGroup((ServerTreeGroup)destinationNode, addMoveNode.Name, false, out conflictingPath))
                    return false;

                if (addMoveNode.ParentPath == destinationNode.Path)
                    return false;

                // if the node that's being added is actually a direct parent of
                // the destination node, then it's not possible for it to be added as a child
                // direct parent:
                //      destination - ./a/b/c/destination
                //      direct parents - ./a/b/c; ./a/b; ./a
                //      non-d parents - ./a/b/d; ./a/b/c/e
                // thus, if the proposed node's path is NOT wholly contained in the destination node's
                // path, then it's okay to add the proposed node
                return (destinationNode.Path.IndexOf(addMoveNode.Path) == -1);
            }

            return true;
        }

        public bool CanAddServerToCurrentGroup(string serverName, string AETitle, string serverHost, int port, out string conflictingServerPath)
        {
            if (!CurrentNode.IsServerGroup)
            {
                conflictingServerPath = "";
                return false;
            }

            return !IsConflictingServerNameInTree(serverName, false, out conflictingServerPath) &&
                !IsConflictingServerInGroup((IServerTreeGroup)CurrentNode, false, AETitle, serverHost, port, out conflictingServerPath);
        }

        public bool CanEditCurrentServer(string serverName, string AETitle, string serverHost, int port, out string conflictingServerPath)
        {
            if (!CurrentNode.IsServer)
            {
                conflictingServerPath = "";
                return false;
            }

            return !IsConflictingServerNameInTree(serverName, true, out conflictingServerPath) &&
                !IsConflictingServerInGroup(FindParentGroup(CurrentNode), true, AETitle, serverHost, port, out conflictingServerPath);
        }

        public bool CanAddGroupToCurrentGroup(string newGroupName, out string conflictingGroupPath)
        {
            if (!CurrentNode.IsServerGroup)
            {
                conflictingGroupPath = "";
                return false;
            }

            return !IsConflictingServerTreeGroupInGroup((ServerTreeGroup)CurrentNode, newGroupName, false, out conflictingGroupPath);
        }

        public bool CanEditCurrentGroup(string newGroupName, out string conflictingGroupPath)
        {
            if (!CurrentNode.IsServerGroup)
            {
                conflictingGroupPath = "";
                return false;
            }

            return !IsConflictingServerTreeGroupInGroup(FindParentGroup(CurrentNode), newGroupName, true, out conflictingGroupPath);
        }

        #endregion

        public IServerTreeNode FindServer(string path)
        {
            return FindServer(RootServerGroup, path);
        }

        public IServerTreeNode FindServer(IServerTreeGroup group, string path)
        {
            foreach (IServerTreeNode server in group.Servers)
            {
                if (server.Path == path)
                    return server;
            }

            foreach (IServerTreeGroup childGroup in group.ChildGroups)
            {
                IServerTreeNode server = FindServer(childGroup, path);
                if (server != null)
                    return server;
            }

            return null;
        }

        public List<IServerTreeNode> FindChildServers()
        {
            return FindChildServers(RootServerGroup);
        }

        public List<IServerTreeNode> FindChildServers(IServerTreeGroup serverGroup)
        {
            var listOfChildrenServers = new List<IServerTreeNode>();
            FindChildServers(serverGroup, listOfChildrenServers);
            return listOfChildrenServers;
        }

        public IServerTreeGroup FindServerTreeGroup(string path)
        {
            return FindServerTreeGroup(RootServerGroup, path);
        }

        public IServerTreeGroup FindServerTreeGroup(IServerTreeGroup startNode, string path)
        {
            if (!startNode.IsServerGroup)
                return null;

            if (startNode.Path == path)
                return startNode;

            foreach (IServerTreeGroup childrenServerTreeGroup in startNode.ChildGroups)
            {
                IServerTreeGroup foundNode = FindServerTreeGroup(childrenServerTreeGroup, path);
                if (null != foundNode)
                    return foundNode;
            }

            return null;
        }

        #endregion

        #region Private methods

        private IServerTreeGroup FindParentGroup(IServerTreeNode node)
        {
            return FindServerTreeGroup(RootServerGroup, node.ParentPath);
        }

        private bool IsConflictingServerNameInTree(string name, bool excludeCurrentNode, out string conflictingServerPath)
        {
            var allServers = RootServerGroup.GetAllServers();
            if (excludeCurrentNode)
                allServers = allServers.Where(s => s != CurrentNode).ToList();

            foreach (var server in allServers)
            {
                if (String.Compare(server.Name, name, true) == 0)
                {
                    conflictingServerPath = server.Path;
                    return true;
                }
            }

            conflictingServerPath = "";
            return false;
        }

        private bool IsConflictingServerInGroup(IServerTreeGroup serverGroup, bool excludeCurrentNode, string toFindServerAE, string toFindServerHost, int toFindServerPort, out string conflictingServerPath)
        {
            foreach (IServerTreeDicomServer server in serverGroup.Servers)
            {
                if (excludeCurrentNode && server == CurrentNode)
                    continue;

                if (server.AETitle == toFindServerAE &&
                        String.Compare(server.HostName, toFindServerHost, true) == 0 &&
                        server.Port == toFindServerPort)
                {
                    conflictingServerPath = server.Path;
                    return true;
                }
            }

            conflictingServerPath = "";
            return false;
        }

        private bool IsConflictingServerTreeGroupInGroup(IServerTreeGroup searchSite, string toFindServerTreeGroupName, bool excludeCurrentNode, out string conflictingGroupPath)
        {
            foreach (IServerTreeGroup serverGroup in searchSite.ChildGroups)
            {
                if (excludeCurrentNode && serverGroup == CurrentNode)
                    continue;

                if (String.Compare(serverGroup.Name, toFindServerTreeGroupName, true) == 0)
                {
                    conflictingGroupPath = serverGroup.Path;
                    return true;
                }
            }

            conflictingGroupPath = "";
            return false;
        }

        private void FindChildServers(IServerTreeGroup serverGroup, List<IServerTreeNode> list)
        {
            foreach (IServerTreeGroup group in serverGroup.ChildGroups)
                FindChildServers(group, list);

            foreach (IServerTreeNode server in serverGroup.Servers)
                list.Add(server);
        }

        #endregion
    }
}
