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
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    #region InstanceAvailability

    internal class InstanceAvailability<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        public InstanceAvailability(DicomAttributeCollection criteria)
            : base(DicomTags.InstanceAvailability, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, "ONLINE");
        }
    }

    internal class StudyInstanceAvailability : InstanceAvailability<Study>
    {
        public StudyInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesInstanceAvailability : InstanceAvailability<Series>
    {
        public SeriesInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceAvailability : InstanceAvailability<SopInstance>
    {
        public SopInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region RetrieveAETitle

    internal class RetrieveAETitle<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        public RetrieveAETitle(DicomAttributeCollection criteria)
            : base(DicomTags.RetrieveAeTitle, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, ServerDirectory.GetLocalServer().AETitle);
        }
    }

    internal class StudyRetrieveAETitle: RetrieveAETitle<Study>
    {
        public StudyRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesRetrieveAETitle : RetrieveAETitle<Series>
    {
        public SeriesRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceRetrieveAETitle : RetrieveAETitle<SopInstance>
    {
        public SopInstanceRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region Specific Character Set

    internal abstract class SpecificCharacterSet<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected SpecificCharacterSet(DicomAttributeCollection criteria)
            : base(DicomTags.SpecificCharacterSet, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }
    }

    internal class StudySpecificCharacterSet : SpecificCharacterSet<Study>
    {
        public StudySpecificCharacterSet(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SpecificCharacterSet);
        }
    }

    internal class SeriesSpecificCharacterSet : SpecificCharacterSet<Series>
    {
        public SeriesSpecificCharacterSet(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SpecificCharacterSet);
        }
    }

    internal class SopInstanceSpecificCharacterSet : SpecificCharacterSet<SopInstance>
    {
        public SopInstanceSpecificCharacterSet(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SpecificCharacterSet);
        }
    }

    #endregion
}