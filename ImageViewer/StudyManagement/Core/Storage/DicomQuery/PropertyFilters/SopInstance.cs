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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    internal class SopInstanceUniqueKey
    {
        internal class StudyInstanceUid : UidPropertyFilter<SopInstance>
        {
            public StudyInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.StudyInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.StudyInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.StudyInstanceUid);
            }
        }

        internal class SeriesInstanceUid : UidPropertyFilter<SopInstance>
        {
            public SeriesInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SeriesInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SeriesInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SeriesInstanceUid);
            }
        }

        internal class SopInstanceUid : UidPropertyFilter<SopInstance>
        {
            public SopInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SopInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SopInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SopInstanceUid);
            }
        }
    }

    internal class InstanceNumber : DicomPropertyFilter<SopInstance>
    {
        public InstanceNumber(DicomAttributeCollection criteria)
            : base(DicomTags.InstanceNumber, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
        {
            var criterion = Criterion.GetInt32(0, 0);
            return results.Where(s => s.InstanceNumber == criterion);
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.InstanceNumber);
        }
    }

    internal class SopClassUid : UidPropertyFilter<SopInstance>
    {
        public SopClassUid(DicomAttributeCollection criteria)
            : base(DicomTags.SopClassUid, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
        {
            var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
            return results.Where(s => criterion.Contains(s.SopClassUid));
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SopClassUid);
        }
    }
}
