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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StudyRootQueryStoreAdapter : IStudyStoreQuery, IDisposable
    {
        private IStudyRootQuery _studyRootQuery;

        public StudyRootQueryStoreAdapter(IStudyRootQuery studyRootQuery)
        {
            Platform.CheckForNullReference(studyRootQuery, "studyRootQuery");
            _studyRootQuery = studyRootQuery;
        }

        #region IStudyStoreQuery Members

        GetStudyCountResult IStudyStoreQuery.GetStudyCount(GetStudyCountRequest request)
        {
            throw new NotSupportedException("IStudyRootQuery does not support study count queries.");
        }

        public GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new StudyEntry();

            if (request.Criteria.Study == null)
                request.Criteria.Study = new StudyRootStudyIdentifier();

            return new GetStudyEntriesResult
                       {
                           StudyEntries = _studyRootQuery.StudyQuery(request.Criteria.Study)
                            .Select(identifier => new StudyEntry { Study = identifier }).ToList()
                       };
        }

        public GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new SeriesEntry();

            if (request.Criteria.Series == null)
                request.Criteria.Series = new SeriesIdentifier();

            return new GetSeriesEntriesResult
            {
                SeriesEntries = _studyRootQuery.SeriesQuery(request.Criteria.Series)
                 .Select(identifier => new SeriesEntry { Series = identifier }).ToList()
            };
        }

        public GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.Criteria == null)
                request.Criteria = new ImageEntry();

            if (request.Criteria.Image == null)
                request.Criteria.Image = new ImageIdentifier();

            return new GetImageEntriesResult
            {
                ImageEntries = _studyRootQuery.ImageQuery(request.Criteria.Image)
                                    .Select(identifier => new ImageEntry { Image = identifier }).ToList()
            };
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_studyRootQuery == null)
                return;

            var disposable = _studyRootQuery as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _studyRootQuery = null;
        }

        #endregion
    }
}
