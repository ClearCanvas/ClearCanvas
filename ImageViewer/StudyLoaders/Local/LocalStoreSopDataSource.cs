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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core;

namespace ClearCanvas.ImageViewer.StudyLoaders.Local
{
    internal class LocalStoreSopDataSource : LocalSopDataSource
    {
        private readonly ISopInstance _sop;

        public LocalStoreSopDataSource(ISopInstance sop)
            : base(sop.FilePath)
        {
            _sop = sop;
        }

        public override string TransferSyntaxUid
        {
            get { return _sop.TransferSyntaxUid; }
        }

        public override string StudyInstanceUid
        {
            get { return _sop.GetParentSeries().GetParentStudy().StudyInstanceUid; }
        }
		
        public override string SeriesInstanceUid
        {
            get { return _sop.GetParentSeries().SeriesInstanceUid; }
        }

        public override string SopInstanceUid
        {
            get { return _sop.SopInstanceUid; }
        }
		
        public override string SopClassUid
        {
            get { return _sop.SopClassUid; }
        }

        public override DicomAttribute this[DicomTag tag]
        {
            get
            {
                //the _sop indexer is not thread-safe.
                lock (SyncLock)
                {
                    if (_sop.IsStoredTag(tag))
                        return _sop[tag];

                    return base[tag];
                }
            }
        }

        public override DicomAttribute this[uint tag]
        {
            get
            {
                //the _sop indexer is not thread-safe.
                lock (SyncLock)
                {
                    if (_sop.IsStoredTag(tag))
                        return _sop[tag];

                    return base[tag];
                }
            }
        }


        public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (_sop.IsStoredTag(tag))
                {
                    attribute = _sop[tag];
                    if (!attribute.IsEmpty)
                        return true;
                }

                return base.TryGetAttribute(tag, out attribute);
            }
        }

        public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
        {
            lock (SyncLock)
            {
                if (_sop.IsStoredTag(tag))
                {
                    attribute = _sop[tag];
                    if (!attribute.IsEmpty)
                        return true;
                }

                return base.TryGetAttribute(tag, out attribute);
            }
        }
    }
}