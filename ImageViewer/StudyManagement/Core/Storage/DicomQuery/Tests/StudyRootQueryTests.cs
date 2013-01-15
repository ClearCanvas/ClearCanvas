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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    //TODO (CR Marmot): case insensitivity.

    [TestFixture]
    public class StudyRootQueryTests : ClearCanvas.Dicom.Tests.AbstractTest
    {
        private const string _testDatabaseFilename = "test_store.sdf";
        
        [TestFixtureSetUp]
        public void Setup()
        {
            var extensionFactory = new UnitTestExtensionFactory
                                       {
                                            { typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(DicomServerTestServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(StudyStoreQueryServiceProvider) },
                                            { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryServiceProvider) }
                                       };

            Platform.SetExtensionFactory(extensionFactory);

			SqlCeDatabaseHelper<DicomStoreDataContext>.CreateDatabase(_testDatabaseFilename);
		}

        private static DataAccessContext CreateContext()
        {
            return new DataAccessContext(null, _testDatabaseFilename);
        }

        [Test]
        public void SelectAllStudies()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier();

                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
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
                
                var results = query.StudyQuery(criteria);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].PatientId);
            }
        }

        [Test]
        public void SelectPatientIdEmpty_NoCriteria()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = ""
                };

                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectPatientId_WildcardSingle()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                                   {
                                       PatientId = "123?56?8"
                                   };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(2, results.Count);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "12345678") != null);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "123a56b8") != null);
            }
        }

        [Test]
        public void SelectPatientId_Wildcard()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                                   {
                                       PatientId = "123*"
                                   };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(2, results.Count);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "12345678") != null);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "123a56b8") != null);
            }
        }

        [Test]
        public void SelectPatientIdAndName_Wildcard()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "*5*",
                    PatientsName = "MIST*"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(4, results.Count);

                //Throw in an extra.
                criteria.AccessionNumber = "*6";
                results = query.StudyQuery(criteria);
                Assert.AreEqual(1, results.Count);
            }
        }

        [Test]
        public void SelectPatientIdAndName_Equals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "123a56b8",
                    PatientsName = "MISTER^CT"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "123a56b8") != null);
            }
        }

        [Test]
        public void SelectPatientIdAndStudyId_Wildcard()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "*EXAMPLE",
                    StudyId = "*EXAMPLE"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(4, results.Count);
            }
        }

        [Test]
        public void SelectEmptyStudyDescription_NoCriteria()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyDescription = ""
                };

                //Get all studies back.
                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectStudyDate_Equals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20111223"
                };

                var results = query.StudyQuery(criteria);

                Assert.AreEqual(1, results.Count);
            }
        }

        [Test]
        public void SelectStudyDate_Before()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "-20111223"
                };

                var results = query.StudyQuery(criteria);

                Assert.AreEqual(12, results.Count);

                criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "-20111222"
                };

                results = query.StudyQuery(criteria);

                Assert.AreEqual(11, results.Count);
            }
        }

        [Test]
        public void SelectStudyDate_After()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20050915-"
                };

                var results = query.StudyQuery(criteria);

                Assert.AreEqual(5, results.Count);

                criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20050916-"
                };

                results = query.StudyQuery(criteria);

                Assert.AreEqual(4, results.Count);
            }
        }

        [Test]
        public void SelectStudyDate_Between()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20050915-20111223"
                };

                var results = query.StudyQuery(criteria);

                Assert.AreEqual(4, results.Count);

                criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20050916-20111223"
                };

                results = query.StudyQuery(criteria);

                Assert.AreEqual(3, results.Count);

                criteria = new StudyRootStudyIdentifier
                {
                    StudyDate = "20050915-20111222"
                };

                results = query.StudyQuery(criteria);

                Assert.AreEqual(3, results.Count);
            }
        }

        [Test]
        public void SelectStudyTime_Equals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyTime = "083501"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectStudyTime_Before()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyTime = "-083501"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectStudyTime_After()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyTime = "083501-"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectStudyTime_Between()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyTime = "083501-100821"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count);
            }
        }

        [Test]
        public void SelectStudyInstanceUid_Single()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyInstanceUid = "1.3.51.0.7.633918642.633920010109.6339100821"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                Assert.AreEqual(1, results.Count);
            }
        }

        [Test]
        public void SelectStudyInstanceUid_Multiple()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    StudyInstanceUid = @"1.3.51.0.7.633918642.633920010109.6339100821\1.3.46.670589.6.1.0.98511171.2001010909203006\1.2.840.113619.2.67.2158294438.15745010109084247.20000"
                };

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.StudyQuery(criteria);

                Assert.AreEqual(3, results.Count);
            }
        }

        [Test]
        public void SelectStudyInstanceUid_Required()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                //criteria[DicomTags.StudyInstanceUid].SetString(0, "1.3.51.0.7.633918642.633920010109.6339100821");

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.Query(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count());

                foreach (var result in results)
                {
                    Assert.IsFalse(result[DicomTags.StudyInstanceUid].IsEmpty);

                    //It's 4 because of InstanceAvailability, RetrieveAE, SpecificCharacterSet.
                    Assert.AreEqual(4, result.Count);
                }
            }
        }
        [Test]
        public void SelectModalitiesInStudy_NoCriteria()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.ModalitiesInStudy].SetNullValue();

                var results = query.Query(criteria);
                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count());

                foreach (var result in results)
                {
                    Assert.IsFalse(result[DicomTags.ModalitiesInStudy].IsEmpty);
                    //The 1 requested attribute + Study UID, InstanceAvailability, RetrieveAE, SpecificCharacterSet.
                    Assert.AreEqual(5, result.Count);
                }
            }
        }

        [Test]
        public void SelectModalitiesInStudy_SingleValue()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.ModalitiesInStudy].SetStringValue("OT");

                var results = query.Query(criteria);
                Assert.AreEqual(7, results.Count());
            }
        }

        [Test]
        public void SelectModalitiesInStudy_SingleValueWildcard()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.ModalitiesInStudy].SetStringValue("C*");

                var results = query.Query(criteria);
                //CR and CT
                Assert.AreEqual(2, results.Count());

                criteria[DicomTags.ModalitiesInStudy].SetStringValue("C?");
                results = query.Query(criteria);
                Assert.AreEqual(2, results.Count());
            }
        }

        [Test]
        public void SelectModalitiesInStudy_MultipleValues()
        {
            using (var context = CreateContext())
            {
                //Technically, DICOM doesn't allow multiple values for the criteria, but we allow
                //the one special case of ModalitiesInStudy, and treat it as an *any*, rather than *all*.
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.ModalitiesInStudy].SetStringValue(@"CT\OT");

                var results = query.Query(criteria);
                //7 OT, 1CT
                Assert.AreEqual(8, results.Count());

                criteria[DicomTags.ModalitiesInStudy].SetStringValue(@"CT\OT\KO");
                results = query.Query(criteria);
                //7 OT (1 w/KO), 1CT
                Assert.AreEqual(8, results.Count());

                criteria[DicomTags.ModalitiesInStudy].SetStringValue(@"CT\KO");
                results = query.Query(criteria);
                //1 CT, 1 OT w/KO
                Assert.AreEqual(2, results.Count());
            }
        }

        [Test]
        public void SelectModalitiesInStudy_MultipleValue_WithWildcard()
        {
            using (var context = CreateContext())
            {
                //Technically, DICOM doesn't allow multiple values for the criteria, but we allow
                //the one special case of ModalitiesInStudy, and treat it as an *any*, rather than *all*.
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.ModalitiesInStudy].SetStringValue(@"C*\OT");

                var results = query.Query(criteria);
                //7 OT, 1CT, 1CR
                Assert.AreEqual(9, results.Count());
            }
        }

        [Test]
        public void SelectReturnOnlyRequestedAttributes()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new DicomAttributeCollection();
                criteria[DicomTags.QueryRetrieveLevel].SetString(0, "STUDY");
                criteria[DicomTags.PatientId].SetNullValue();
                criteria[DicomTags.PatientsName].SetNullValue();

                /// TODO (CR Apr 2012): For now, we don't support time queries at all,
                /// so the answer is that all studies match.
                var results = query.Query(criteria);

                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, results.Count());

                foreach (var result in results)
                {
                    Assert.IsFalse(result[DicomTags.PatientId].IsEmpty);

                    //The 2 requested attributes + Study UID, InstanceAvailability, RetrieveAE, SpecificCharacterSet.
                    Assert.AreEqual(6, result.Count);
                }
            }
        }
    }
}
#endif