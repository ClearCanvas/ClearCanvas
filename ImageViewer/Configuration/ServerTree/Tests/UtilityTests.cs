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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.ServerDirectory.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            DicomServerTestServiceProvider.Reset();
            StudyStoreTestServiceProvider.Reset();
            ServerDirectoryTestServiceProvider.Reset();

            var factory = new UnitTestExtensionFactory
                              {
                                  { typeof (ServiceProviderExtensionPoint), typeof (DicomServerTestServiceProvider) },
                                  { typeof (ServiceProviderExtensionPoint), typeof (StudyStoreTestServiceProvider) },
                                  { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryTestServiceProvider) },
                                  {typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider)}
                              };
            Platform.SetExtensionFactory(factory);
        }

        [Test]
        public void TestToDataContract()
        {
            TestSettingsStore.Instance.Reset();

            var treeServer = new ServerTreeDicomServer("name", "location", "host", "aetitle", 105, true, 51121, 51122);
            var ae = treeServer.ToDataContract();
            Assert.AreEqual("name", ae.Name);
            Assert.AreEqual("location", ae.Location);
            Assert.AreEqual("aetitle", ae.AETitle);
            Assert.AreEqual("host", ae.ScpParameters.HostName);
            Assert.AreEqual(105, ae.ScpParameters.Port);
            Assert.IsNotNull(ae.StreamingParameters);
            Assert.AreEqual(51121, ae.StreamingParameters.HeaderServicePort);
            Assert.AreEqual(51122, ae.StreamingParameters.WadoServicePort);

            treeServer.IsStreaming = false;
            ae = treeServer.ToDataContract();
            Assert.AreEqual("name", ae.Name);
            Assert.AreEqual("location", ae.Location);
            Assert.AreEqual("aetitle", ae.AETitle);
            Assert.AreEqual("host", ae.ScpParameters.HostName);
            Assert.AreEqual(105, ae.ScpParameters.Port);
            Assert.IsNull(ae.StreamingParameters);
        }

        [Test]
        public void TestToDicomServiceNodes_Local()
        {
            TestSettingsStore.Instance.Reset();

            var tree = new ServerTree(null, null);
            tree.CurrentNode = tree.LocalServer;
            var serviceNodes = tree.CurrentNode.ToDicomServiceNodes();
            Assert.AreEqual(1, serviceNodes.Count);
            var ae = serviceNodes.First();

            Assert.AreEqual(Common.SR.LocalServerName, ae.Name);
            Assert.AreEqual(string.Empty, ae.Location ?? "");
            Assert.AreEqual("AETITLE", ae.AETitle);
            Assert.AreEqual("localhost", ae.ScpParameters.HostName);
            Assert.AreEqual(104, ae.ScpParameters.Port);
            Assert.IsNull(ae.StreamingParameters);
        }

        [Test]
        public void TestToDicomServiceNodes_Remote()
        {
            TestSettingsStore.Instance.Reset();

            var tree = TestHelper.CreateTestTree1();

            tree.CurrentNode = tree.RootServerGroup;
            var serviceNodes = tree.CurrentNode.ToDicomServiceNodes();
            Assert.AreEqual(4, serviceNodes.Count);
        }
    }
}

#endif