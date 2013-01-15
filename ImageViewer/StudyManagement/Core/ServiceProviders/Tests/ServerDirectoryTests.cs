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

using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders.Tests
{
    [DataContract]
    [ServerDataContractAttribute("309FCB6E-80EB-4385-9732-CCA7E4B88544")]
    internal struct TestValue
    {
        [DataMember]
        public string Value;
    }
    [DataContract]
    [ServerDataContractAttribute("309FCB6E-80EB-4385-9732-CCA7E4B88543")]
    internal struct TestValue2
    {
        [DataMember]
        public string Value;
    }

    [TestFixture]
    public class ServerDirectoryTests
    {
        internal class DataTypeProvider : ServerDirectoryEntry.IDataTypeProvider
        {
            #region IDataTypeProvider Members

            public System.Collections.Generic.IEnumerable<System.Type> GetTypes()
            {
                yield return typeof (TestValue);
                yield return typeof(TestValue2);
            }

            #endregion
        }

        [TestFixtureSetUp]
        public void Initialize()
        {
            var extensionFactory = new UnitTestExtensionFactory
                                       {
                                            { typeof(ServiceProviderExtensionPoint), typeof(DicomServerTestServiceProvider) },
                                            { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryServiceProvider) },
                                            { typeof (ServerDirectoryEntry.DataTypeProviderExtensionPoint), typeof (DataTypeProvider) }
                                       };

            Platform.SetExtensionFactory(extensionFactory);
        }

        public void DeleteAllServers()
        {
            var directory = Platform.GetService<IServerDirectory>();
            var entries = directory.GetServers(new GetServersRequest()).ServerEntries;
            foreach (var entry in entries)
                directory.DeleteServer(new DeleteServerRequest { ServerEntry = entry });
        }

        [Test]
        public void TestAddServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("streaming", true);
            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server){IsPriorsServer = true} });
            var entries = directory.GetServers(new GetServersRequest()).ServerEntries;
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(true, entries[0].IsPriorsServer);

            server = CreateServer("normal", false);
            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server) });

            entries = directory.GetServers(new GetServersRequest()).ServerEntries;
            Assert.AreEqual(2, entries.Count);

            entries = directory.GetServers(new GetServersRequest{Name = "normal"}).ServerEntries;
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(server, entries[0].Server);
        }

        [Test]
        public void TestUpdateServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server) });

            server.AETitle = "ae2";
            server.ScpParameters = new ScpParameters("host2", 100);
            server.Description = "blah";
            server.Location = "blah";

            directory.UpdateServer(new UpdateServerRequest { ServerEntry = new ServerDirectoryEntry(server) });
            var servers = directory.GetServers(new GetServersRequest()).ServerEntries;
            Assert.AreEqual(1, servers.Count);

            Assert.AreEqual(server, servers[0].Server);
        }

        [Test]
        public void TestDeleteServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server) });
            directory.DeleteServer(new DeleteServerRequest { ServerEntry = new ServerDirectoryEntry(server) });

            var servers = directory.GetServers(new GetServersRequest()).ServerEntries;
            Assert.AreEqual(0, servers.Count);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerExistsFault>))]
        public void TestAddServerAlreadyExists()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server) });

            directory.AddServer(new AddServerRequest { ServerEntry = new ServerDirectoryEntry(server) });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestUpdateServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.UpdateServer(new UpdateServerRequest { ServerEntry = new ServerDirectoryEntry(server) });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestDeleteServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.DeleteServer(new DeleteServerRequest { ServerEntry = new ServerDirectoryEntry(server) });
        }

        [Test]
        public void TestExtensionDataSerialization()
        {
            var entry = new ServerDirectoryEntry();
            entry.Data["test1"] = new TestValue {Value = "value1"};
            entry.Data["test2"] = new TestValue2 { Value = "value2" };

            var serialized = Serializer.SerializeServerExtensionData(entry.Data);
            var deserialized = Serializer.DeserializeServerExtensionData(serialized);

            Assert.AreEqual(2, deserialized.Count);
            Assert.AreEqual(deserialized["test1"], new TestValue{Value = "value1"});
            Assert.AreEqual(deserialized["test2"], new TestValue2 { Value = "value2" });

            var serializer = new DataContractSerializer(typeof(ServerDirectoryEntry), ServerDirectoryEntry.GetKnownTypes()); 
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, entry);
                stream.Position = 0;
                var deserializedEntry = serializer.ReadObject(stream) as ServerDirectoryEntry;
                deserialized = deserializedEntry.Data;

                Assert.AreEqual(2, deserialized.Count);
                Assert.AreEqual(deserialized["test1"], new TestValue { Value = "value1" });
                Assert.AreEqual(deserialized["test2"], new TestValue2 { Value = "value2" });
            }
        }

        private ApplicationEntity CreateServer(string name, bool streaming)
        {
            var ae = new ApplicationEntity
            {
                Name = name,
                AETitle = "ae",
                ScpParameters = new ScpParameters{HostName = "host", Port = 104},
                Description = "Some server",
                Location = "Room 101"
            };

            if (streaming)
                ae.StreamingParameters = new StreamingParameters(50221, 1000);
            return ae;
        }
    }
}

#endif