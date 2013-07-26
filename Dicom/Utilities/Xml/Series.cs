#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml
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
