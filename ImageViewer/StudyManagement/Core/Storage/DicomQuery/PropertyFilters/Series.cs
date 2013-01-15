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
    internal class SeriesUniqueKey
    {
        internal class StudyInstanceUid : UidPropertyFilter<Series>
        {
            public StudyInstanceUid(DicomAttributeCollection criteria) 
                : base(DicomTags.StudyInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.StudyInstanceUid));
            }

            protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.StudyInstanceUid);
            }
        }

        internal class SeriesInstanceUid : UidPropertyFilter<Series>
        {
            public SeriesInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SeriesInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SeriesInstanceUid));
            }

            protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SeriesInstanceUid);
            }
        }
    }

    internal class Modality : StringDicomPropertyFilter<Series>
    {
        public Modality(DicomAttributeCollection criteria)
            : base(DicomTags.Modality, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Series item)
        {
            return item. Modality;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.Modality);
        }
    }

    internal class SeriesDescription : StringDicomPropertyFilter<Series>
    {
        public SeriesDescription(DicomAttributeCollection criteria)
            : base(DicomTags.SeriesDescription, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Series item)
        {
            return item.SeriesDescription;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SeriesDescription);
        }
    }

    internal class SeriesNumber : DicomPropertyFilter<Series>
    {
        public SeriesNumber(DicomAttributeCollection criteria)
            : base(DicomTags.SeriesNumber, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
        {
            var criterion = Criterion.GetInt32(0, 0);
            return results.Where(s => s.SeriesNumber == criterion);
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.SeriesNumber);
        }
    }

    internal class NumberOfSeriesRelatedInstances : DicomPropertyFilter<Series>
    {
        public NumberOfSeriesRelatedInstances(DicomAttributeCollection criteria)
            : base(DicomTags.NumberOfSeriesRelatedInstances, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = true;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.NumberOfSeriesRelatedInstances);
        }
    }
}
