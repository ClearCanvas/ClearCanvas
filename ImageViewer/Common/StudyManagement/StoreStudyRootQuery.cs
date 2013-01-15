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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StoreStudyRootQuery : IStudyRootQuery, IDisposable
    {
        private IStudyStoreQuery _real;

        public StoreStudyRootQuery()
        {
        }

        public StoreStudyRootQuery(IStudyStoreQuery real)
        {
            _real = real;
        }

        private IStudyStoreQuery Real
        {
            get { return _real ?? (_real = Platform.GetService<IStudyStoreQuery>()); }
        }

        #region IStudyRootQuery Members

        public System.Collections.Generic.IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            var criteria = new StudyEntry {Study = queryCriteria};
            var result = Real.GetStudyEntries(new GetStudyEntriesRequest {Criteria = criteria});
            return result.StudyEntries.Select(e => e.Study).ToList();
        }

        public System.Collections.Generic.IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            var criteria = new SeriesEntry {Series = queryCriteria};
            var result = Real.GetSeriesEntries(new GetSeriesEntriesRequest { Criteria = criteria });
            return result.SeriesEntries.Select(e => e.Series).ToList();
        }

        public System.Collections.Generic.IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            var criteria = new ImageEntry { Image = queryCriteria };
            var result = Real.GetImageEntries(new GetImageEntriesRequest { Criteria = criteria });
            return result.ImageEntries.Select(e => e.Image).ToList();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_real == null)return;

            var disposable = _real as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _real = null;
        }

        #endregion
    }
}
