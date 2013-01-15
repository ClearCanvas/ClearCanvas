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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    //TODO (Marmot):Test with Deleted and Reindex column set.

    [TestFixture]
    public class StudyStoreQueryTests
    {
        private const string _testDatabaseFilename = "test_store.sdf";

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

            if (!File.Exists(SqlCeDatabaseHelper<DicomStoreDataContext>.GetDatabaseFilePath(_testDatabaseFilename)))
                SqlCeDatabaseHelper<DicomStoreDataContext>.CreateDatabase(_testDatabaseFilename);
        }

        private static DataAccessContext CreateContext()
        {
            return new DataAccessContext(null, _testDatabaseFilename);
        }

        [Test]
        public void TestGetStudyCount()
        {
            using (var context = CreateContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount();
                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, count);
            }
        }

        [Test]
        public void TestGetStudyCount_WithCriteria()
        {
            using (var context = CreateContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount(
                    new StudyEntry
                        {
                            Study = new StudyRootStudyIdentifier{PatientId = "SCS*"}
                        });
                Assert.AreEqual(3, count);
            }
        }

        [Test]
        public void SelectPatientIdEquals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "12345678"
                };

                var results = query.GetStudyEntries(new StudyEntry{Study = criteria});
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].Study.PatientId);
            }
        }
    }
}

#endif