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
using ClearCanvas.Dicom.ServiceModel;
using NUnit.Framework;

#if UNIT_TESTS


namespace ClearCanvas.ImageViewer.Configuration.ServerTree.Tests
{
    internal static class TestHelper
    {
        public static ServerTree CreateTestTree1()
        {
            List<ApplicationEntity> dummy;
            return CreateTestTree1(out dummy);
        }

        public static ServerTree CreateTestTree1(out List<ApplicationEntity> directoryServers)
        {
            var rootGroup = CreateStoredServerGroup1();
            directoryServers = CreateDirectoryServers1();
            return new ServerTree(rootGroup, directoryServers);
        }

        public static StoredServerGroup CreateStoredServerGroup1()
        {
            return new StoredServerGroup(@"My Servers")
                       {
                           ChildGroups =
                               {
                                   new StoredServerGroup("Empty"),
                                   new StoredServerGroup("Test")
                                       {
                                           DirectoryServerReferences =
                                               {
                                                   new DirectoryServerReference("server1"),
                                                   new DirectoryServerReference("server2")
                                               }
                                       }

                               },
                           DirectoryServerReferences =
                               {
                                   new DirectoryServerReference("server3"),
                                   new DirectoryServerReference("server4")
                               }
                       };
        }

        public static List<ApplicationEntity> CreateDirectoryServers1()
        {
            return new List<ApplicationEntity>
                       {
                           new ApplicationEntity
                               {
                                   Name = "server1",
                                   AETitle = "server1",
                                   ScpParameters = new ScpParameters("server1", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server2",
                                   AETitle = "server2",
                                   ScpParameters = new ScpParameters("server2", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server3",
                                   AETitle = "server3",
                                   ScpParameters = new ScpParameters("server3", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server4",
                                   AETitle = "server4",
                                   ScpParameters = new ScpParameters("server4", 104)
                               }
                       };
        }

        public static void AssertExampleTree(ServerTree tree)
        {
            Assert.IsNotNull(tree.LocalServer);
            Assert.IsNotNull(tree.RootServerGroup);
            Assert.AreEqual(1, tree.RootServerGroup.Servers.Count);
            Assert.AreEqual(1, tree.RootServerGroup.ChildGroups.Count);

            Assert.AreEqual(SR.ExampleServer, tree.RootServerGroup.Servers[0].Name);
            Assert.AreEqual(SR.ExampleGroup, tree.RootServerGroup.ChildGroups[0].Name);
        }
    }
}

#endif