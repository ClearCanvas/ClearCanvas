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
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    // TODO (CR Mar 2012): One day, maybe merge this into the ServerTreeComponent, but it's probably not worth it ... it works.
    public partial class ServerTree
    {
        public ServerTree()
            : this(ServerTreeSettings.Default.GetSharedServers(), GetServersFromDirectory())
        {
        }

        public ServerTree(string legacyXmlFilename)
            : this(SerializationHelper.LoadFromXml(legacyXmlFilename))
        {
        }

        internal ServerTree(StoredServerGroup rootGroup, List<ApplicationEntity> directoryServers)
        {
            LocalServer = new ServerTreeLocalServer();

            //Create a copy because we will modify it.
            directoryServers = directoryServers == null
                ? new List<ApplicationEntity>()
                : new List<ApplicationEntity>(directoryServers);

            if (rootGroup == null)
            {
                InitializeRootGroup();
            }
            else
            {
                //ToServerTreeGroup eliminates the items from the passed in list of
                //servers as the references are found in the tree.
                RootServerGroup = rootGroup.ToServerTreeGroup(directoryServers);
            }

            //rootGroup.ToServerTreeGroup above deletes the entries from the list of servers,
            //so if there are any left, that means there was no match in the tree. So,
            //we will just add those servers to the root.
            foreach (ApplicationEntity server in directoryServers)
                RootServerGroup.Servers.Add(new ServerTreeDicomServer(server));

            InitializePaths();
            CurrentNode = LocalServer;
        }

        internal ServerTree(ServerTreeRoot legacyRoot)
        {
            LocalServer = new ServerTreeLocalServer();
            
            if (legacyRoot == null)
            {
                InitializeRootGroup();
            }
            else
            {
                RootServerGroup = legacyRoot.ServerGroupNode.ToServerTreeGroup();
            }

            InitializePaths();
            CurrentNode = LocalServer;
        }

        private void InitializeRootGroup()
        {
            RootServerGroup = new ServerTreeGroup(@"My Servers");
        }

        private void InitializePaths()
        {
            ((ServerTreeLocalServer)LocalServer).ChangeParentPath(_rootPath);
            ((ServerTreeGroup)RootServerGroup).ChangeParentPath(_rootPath);
        }

        ////TODO (Marmot): Factor this out to ServerTreeComponent, since saving is done outside everywhere else.
        //private void AddExamples()
        //{
        //    RootServerGroup.ChildGroups.Add(new ServerTreeGroup(SR.ExampleGroup));
        //    var exampleServer = new ServerTreeDicomServer(SR.ExampleServer, "", "localhost", "SAMPLE", 104, false, 50221, 1000);
        //    RootServerGroup.Servers.Add(exampleServer);
        //}

        internal static List<ApplicationEntity> GetServersFromDirectory()
        {
            try
            {
                List<ApplicationEntity> servers = null;
                Platform.GetService<IServerDirectory>(
                    directory => servers = directory.GetServers(new GetServersRequest()).ServerEntries.Select(e => e.Server).ToList());
                return servers.ToList();
            }
            catch (Exception e)
            {
                //TODO (Marmot): Should this throw?
                Platform.Log(LogLevel.Warn, e, "Failed to load servers from directory.");
            }

            return new List<ApplicationEntity>();
        }

        public void DeleteCurrentNode()
        {
            if (CurrentNode.IsServer)
                DeleteServer();
            else if (CurrentNode.IsServerGroup)
                DeleteGroup();
        }

        public void DeleteServer()
        {
            if (!CurrentNode.IsServer)
                return;

            var parentGroup = FindParentGroup(CurrentNode);
            if (parentGroup == null)
                return;

            for (int i = 0; i < parentGroup.Servers.Count; i++)
            {
                if (parentGroup.Servers[i].Name == CurrentNode.Name)
                {
                    parentGroup.Servers.RemoveAt(i);
                    CurrentNode = parentGroup;
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void DeleteGroup()
        {
            if (!CurrentNode.IsServerGroup)
				return;
			
			var parentGroup = FindParentGroup(CurrentNode);
            if (null == parentGroup)
                return;	


            for (int i = 0; i < parentGroup.ChildGroups.Count; ++i)
            {
                if (parentGroup.ChildGroups[i].Name == CurrentNode.Name)
                {
                    parentGroup.ChildGroups.RemoveAt(i);
                    CurrentNode = parentGroup;
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void ReplaceDicomServerInCurrentGroup(IServerTreeDicomServer newServer)
        {
            if (!CurrentNode.IsServer)
                return;

            var serverGroup = FindParentGroup(CurrentNode);
            if (serverGroup == null)
                return;

            for (int i = 0; i < serverGroup.Servers.Count; i++)
            {
                if (serverGroup.Servers[i].Name != CurrentNode.Name)
                    continue;
                
                serverGroup.Servers[i] = newServer;
                ((ServerTreeDicomServer)newServer).ChangeParentPath(serverGroup.Path);
                CurrentNode = newServer;
                FireServerTreeUpdatedEvent();
                break;
            }
        }

        public void Save()
        {
            SaveServersToDirectory();
            SaveToSettings();
        }

        internal void SaveServersToDirectory()
        {
            Platform.GetService<IServerDirectory>(SaveServersToDirectory);
        }

        internal void SaveServersToDirectory(IServerDirectory directory)
        {
            //NOTE: The server tree currently doesn't handle any of the "extended" server data, so we just abstract that part away
            //and only deal with it when we are saving the servers back to the directory.

            //Get all the entries from the directory.
            var serverDirectoryEntries = directory.GetServers(new GetServersRequest()).ServerEntries.ToList();

            //Convert the tree items to data contracts (ApplicationEntity).
            var treeServers = RootServerGroup.GetAllServers().OfType<IServerTreeDicomServer>().Select(a => a.ToDataContract()).ToList();
            
            //Figure out which entries have been deleted.
            IEnumerable<ServerDirectoryEntry> deletedEntries = from d in serverDirectoryEntries where !treeServers.Any(t => t.Name == d.Server.Name) select d;
            //Figure out which are new.
            IEnumerable<ApplicationEntity> addedServers = (from t in treeServers where !serverDirectoryEntries.Any(d => t.Name == d.Server.Name) select t);
            //Figure out which have changed.
            IEnumerable<ApplicationEntity> changedServers = (from t in treeServers where serverDirectoryEntries.Any(d => t.Name == d.Server.Name && !t.Equals(d.Server)) select t);

            //Most updates are done one server at a time, anyway, so we'll just do this.
            //Could implement bulk update methods on the service, too.
            foreach (var d in deletedEntries)
            {
                try
                {
                    directory.DeleteServer(new DeleteServerRequest { ServerEntry = d });
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Server being deleted ('{0}') does not exist in directory.", d.Server.Name);
                }
            }

            foreach (var c in changedServers)
            {
                //Find the corresponding entry and update IT because we don't want to lose the "extended" data.
                var changedEntry = serverDirectoryEntries.First(e => e.Server.Name == c.Name);
                changedEntry.Server = c;
                directory.UpdateServer(new UpdateServerRequest{ServerEntry = changedEntry});
            }
            foreach (var a in addedServers)
            {
                directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(a) });
            }
        }

        private void SaveToSettings()
        {
            var settings = ServerTreeSettings.Default;
            var newValue = RootServerGroup.ToStoredServerGroup();
            settings.UpdateSharedServers(newValue);
        }
    }
}