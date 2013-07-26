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
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml.Study
{
    /// <summary>
    /// Represents an <see cref="ISeries"/> whose main source of data is a <see cref="StudyXml"/> document.
    /// </summary>
    internal class Series : ISeries
    {
        private readonly SeriesXml _xml;
        private IList<ISopInstance> _sopInstances;

        public Series(SeriesXml xml, Study parent)
        {
            _xml = xml;
            ParentStudy = parent;
        }

        internal Study ParentStudy { get; private set; }

        IStudy ISeries.ParentStudy { get { return ParentStudy; } }

        #region ISeries Members

        public IList<ISopInstance> SopInstances
        {
            get
            {
                return _sopInstances ?? (_sopInstances = _xml.Select(s => (ISopInstance) new SopInstance(s, this)).ToList());
            }
        }

        #endregion

        #region ISeriesData Members

        public string StudyInstanceUid
        {
            get { return ParentStudy.StudyInstanceUid; }
        }

        public string SeriesInstanceUid
        {
            get { return _xml.SeriesInstanceUid; }
        }

        public string Modality
        {
            get { return SopInstances.First().GetAttribute(DicomTags.Modality).ToString(); }
        }

        public string SeriesDescription
        {
            get { return SopInstances.First().GetAttribute(DicomTags.SeriesDescription).ToString(); }
        }

        public int SeriesNumber
        {
            get { return SopInstances.First().GetAttribute(DicomTags.SeriesNumber).GetInt32(0,0); }
        }

        public int? NumberOfSeriesRelatedInstances
        {
            get { return SopInstances.Count; }
        }

        #endregion
    }
}
