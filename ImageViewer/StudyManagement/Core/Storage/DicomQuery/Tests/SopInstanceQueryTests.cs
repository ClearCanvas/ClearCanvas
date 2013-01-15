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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class SopInstanceQueryTests : TestBase
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            var extensionFactory = new UnitTestExtensionFactory
                                       {
                                            { typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(DicomServerTestServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(StudyStoreQueryServiceProvider) },
                                            { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryServiceProvider) }
                                       };

            Platform.SetExtensionFactory(extensionFactory);
        }

        [Test]
        public void SelectAllSops()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            var filters = new SopInstancePropertyFilters(criteria);
            var results = filters.FilterResults(sops);
            Assert.AreEqual(5, results.Count());
        }

        [Test]
        public void SelectByInstanceNumber()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.InstanceNumber].SetInt32(0, 102);

            var filters = new SopInstancePropertyFilters(criteria);
            var results = filters.FilterResults(sops);
            Assert.AreEqual(1, results.Count());

            criteria[DicomTags.InstanceNumber].SetInt32(0, 106);
            filters = new SopInstancePropertyFilters(criteria);

            results = filters.FilterResults(sops);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void AssertUniqueKeys()
        {
            Study study = CreateTestStudy1();
            var sops = study.GetSeries().First().GetSopInstances().Cast<SopInstance>().ToList();

            var criteria = new DicomAttributeCollection();
            var filters = new SopInstancePropertyFilters(criteria);
            var results = filters.FilterResults(sops);
            var converted = filters.ConvertResultsToDataSets(results);
            foreach (var result in converted)
            {
                //It's 6 because of InstanceAvailability, RetrieveAE, SpecificCharacterSet.
                Assert.AreEqual(6, result.Count);
                Assert.IsNotEmpty(result[DicomTags.StudyInstanceUid]);
                Assert.IsNotEmpty(result[DicomTags.SeriesInstanceUid]);
                Assert.IsNotEmpty(result[DicomTags.SopInstanceUid]);
            }
        }
    }
}

#endif