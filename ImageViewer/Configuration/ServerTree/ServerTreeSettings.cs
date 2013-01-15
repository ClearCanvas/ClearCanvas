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
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    [Serializable]
    public class StoredServerGroup
    {
        public StoredServerGroup()
        {
            ChildGroups = new List<StoredServerGroup>();
            DirectoryServerReferences = new List<DirectoryServerReference>();
        }

        public StoredServerGroup(string name)
        {
            Name = name;
            ChildGroups = new List<StoredServerGroup>();
            DirectoryServerReferences = new List<DirectoryServerReference>();
        }

        public string Name { get; set; }

        public List<StoredServerGroup> ChildGroups { get; set; }
        public List<DirectoryServerReference> DirectoryServerReferences { get; set; }
    }

    [Serializable]
    public class DirectoryServerReference
    {
        public DirectoryServerReference()
        {
        }

        public DirectoryServerReference(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
    
    //For now, this is a local shared setting, just like it used to be in a shared xml file.
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    internal sealed partial class ServerTreeSettings
    {
        internal void UpdateSharedServers(StoredServerGroup storedServerGroup)
        {
            this.SetSharedPropertyValue("SharedServers", storedServerGroup);
        }

        internal StoredServerGroup GetSharedServers()
        {
            if (SharedServers == null)
                return null;

            return SharedServers;
        }
    }
}
