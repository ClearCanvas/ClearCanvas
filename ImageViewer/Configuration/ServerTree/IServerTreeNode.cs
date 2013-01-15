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

using System.Collections.Generic;
using System;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public interface IServerTreeLocalServer : IServerTreeNode
    {
        string AETitle { get; }
        string HostName { get; }
        int? Port { get; }
        string FileStoreLocation { get; }

        void Refresh();
    }
    
    public interface IServerTreeGroup : IServerTreeNode
    {
        new string Name { get; set; }
        IList<IServerTreeGroup> ChildGroups { get; }
        IList<IServerTreeNode> Servers { get; }

        void AddChild(IServerTreeNode child);

        List<IServerTreeNode> GetAllServers();
        List<IServerTreeNode> GetCheckedServers(bool recursive);

        bool IsEntireGroupChecked();
        bool IsEntireGroupUnchecked();
    }

    public interface IServerTreeDicomServer : IServerTreeNode
    {
        string AETitle { get; set; }
        string Location { get; set; }
        string HostName { get; set; }
        int Port { get; set; }
        bool IsStreaming { get; set; }
        int HeaderServicePort { get; set; }
        int WadoServicePort { get; set; }
    }

    public interface IServerTreeNode
    {
        bool IsChecked { get; set; }

        [Obsolete("Use IsLocalServer instead.")]
        bool IsLocalDataStore { get; }

        bool IsLocalServer { get; }
        bool IsServer { get; }
        bool IsServerGroup { get; }
        
        string ParentPath { get; }
        string Path { get; }
        string Name { get; }
        string DisplayName { get; }
    }
}