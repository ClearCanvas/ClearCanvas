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
using ClearCanvas.Dicom.ServiceModel.Query;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    public class StudyStoreTestServiceProvider : IServiceProvider
    {
        public static void Reset()
        {
            TestStudyStoreQuery.Reset();
            TestStorageConfiguration.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IStudyStoreQuery) == serviceType)
                return new TestStudyStoreQuery();
            if (typeof(IStorageConfiguration) == serviceType)
                return new TestStorageConfiguration();
            return null;
        }

        #endregion
    }

    public class TestStorageConfiguration : IStorageConfiguration
    {
        public static StorageConfiguration Configuration;

        static TestStorageConfiguration()
        {
            Reset();
        }

        public static void Reset()
        {
            Configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                MinimumFreeSpaceBytes = 5 * 1024L * 1024L * 1024L
            };
        }

        #region IStorageConfiguration Members

        public GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request)
        {
            return new GetStorageConfigurationResult
            {
                Configuration = Configuration
            };
        }

        public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
        {
            Configuration = request.Configuration;
            return new UpdateStorageConfigurationResult();
        }

        #endregion
    }

    public class TestStudyStoreQuery : IStudyStoreQuery
    {
        public static List<StudyEntry> StudyEntries;

        static TestStudyStoreQuery()
        {
            Reset();
        }

        public static void Reset()
        {
            StudyEntries = new List<StudyEntry>
                               {
                                   new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           PatientId = "123",
                                                           StudyInstanceUid = "1.2.3",
                                                           PatientsName = "Test^Patient",
                                                           AccessionNumber = "a1",
                                                           ModalitiesInStudy = new string[] {"CR", "KO", "PR"}
                                                       },
                                           Data = new StudyEntryData {DeleteTime = null, StoreTime = null}
                                       },

                                   new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           PatientId = "223",
                                                           StudyInstanceUid = "2.2.3",
                                                           PatientsName = "Test^Patient2",
                                                           AccessionNumber = "a2",
                                                           ModalitiesInStudy = new string[] {"CT", "DOC"}
                                                       },
                                           Data = new StudyEntryData {DeleteTime = null, StoreTime = null}
                                       },
                               };
        }


        #region IStudyStoreQuery Members

        public GetStudyCountResult GetStudyCount(GetStudyCountRequest request)
        {
            return new GetStudyCountResult {StudyCount = StudyEntries.Count};
        }

        public GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            return new GetStudyEntriesResult {StudyEntries = StudyEntries};
        }

        public GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

#endif