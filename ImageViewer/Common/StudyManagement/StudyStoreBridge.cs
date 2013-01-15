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
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StudyStoreBridge : IDisposable
    {
        private IStudyStoreQuery _real;

        public StudyStoreBridge()
        {
        }

        public StudyStoreBridge(IStudyStoreQuery real)
        {
            _real = real;
        }

        private IStudyStoreQuery Real
        {
            get { return _real ?? (_real = Platform.GetService<IStudyStoreQuery>()); }
        }

        public int GetStudyCount()
        {
            return Real.GetStudyCount(new GetStudyCountRequest()).StudyCount;
        }

        public int GetStudyCount(StudyRootStudyIdentifier criteria)
        {
            return Real.GetStudyCount(new GetStudyCountRequest
                                           {
                                               Criteria = new StudyEntry{Study = criteria}
                                           }).StudyCount;
        }

        public IList<StudyEntry> GetStudyEntries()
        {
            return GetStudyEntries(null as StudyEntry);
        }

        public IList<StudyEntry> GetStudyEntries(StudyRootStudyIdentifier criteria)
        {
            return GetStudyEntries(new StudyEntry {Study = criteria});
        }

        public IList<StudyEntry> GetStudyEntries(StudyEntry criteria)
        {
            return Real.GetStudyEntries(new GetStudyEntriesRequest { Criteria = criteria }).StudyEntries;
        }

        public IList<StudyEntry> QueryByAccessionNumber(string accessionNumber)
        {
            return Real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { AccessionNumber = accessionNumber }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByPatientId(string patientId)
        {
            return Real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { PatientId = patientId }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByStudyInstanceUid(string studyInstanceUid)
        {
            return Real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier { StudyInstanceUid = studyInstanceUid }
                                       }
                    }).StudyEntries;
        }

        public IList<StudyEntry> QueryByStudyInstanceUid(IEnumerable<string> studyInstanceUids)
        {
            return Real.GetStudyEntries(
                new GetStudyEntriesRequest
                    {
                        Criteria = new StudyEntry
                                       {
                                           Study = new StudyRootStudyIdentifier
                                                       {
                                                           StudyInstanceUid = DicomStringHelper.GetDicomStringArray(studyInstanceUids)
                                                       }
                                       }
                    }).StudyEntries;
        }

        public IList<SeriesEntry> GetSeriesEntries(string studyInstanceUid)
        {
            return Real.GetSeriesEntries(
                new GetSeriesEntriesRequest
                {
                    Criteria = new SeriesEntry
                    {
                        Series = new SeriesIdentifier { StudyInstanceUid = studyInstanceUid }
                    }
                }).SeriesEntries;
        }

        public IList<ImageEntry> GetImageEntries(string studyInstanceUid, string seriesInstanceUid)
        {
            return Real.GetImageEntries(
                new GetImageEntriesRequest
                {
                    Criteria = new ImageEntry
                    {
                        Image = new ImageIdentifier{StudyInstanceUid = studyInstanceUid, SeriesInstanceUid = seriesInstanceUid}
                    }
                }).ImageEntries;
        }

        public void Dispose()
        {
            if (_real == null)
                return;

            var disposable = _real as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _real = null;
        }
    }
}
