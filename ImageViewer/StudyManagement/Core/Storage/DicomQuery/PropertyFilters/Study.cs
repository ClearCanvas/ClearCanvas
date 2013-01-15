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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using System.Data.Linq.SqlClient;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    internal class StudyInstanceUid : UidPropertyFilter<Study>
    {
        public StudyInstanceUid(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyInstanceUid), criteria)
        {
            IsReturnValueRequired = true;
        }

        protected override IQueryable<Study> AddUidToQuery(IQueryable<Study> query, string uid)
        {
            return from study in query where study.StudyInstanceUid == uid select study;
        }

        protected override IQueryable<Study> AddUidsToQuery(IQueryable<Study> query, string[] uids)
        {
            return from study in query where uids.Contains(study.StudyInstanceUid) select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyInstanceUid);
        }
    }

    #region Strings

    internal class StudyId : StringDicomPropertyFilter<Study>
    {
        public StudyId(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyId), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.StudyId == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.StudyId, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyId);
        }
    }

    internal class StudyDescription : StringDicomPropertyFilter<Study>
    {
        public StudyDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.StudyDescription == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.StudyDescription, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyDescription);
        }
    }

    internal class AccessionNumber : StringDicomPropertyFilter<Study>
    {
        public AccessionNumber(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.AccessionNumber), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.AccessionNumber == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.AccessionNumber, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.AccessionNumber);
        }
    }

    internal class ProcedureCodeSequence
    {
        internal class CodeValue : StringDicomPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.ProcedureCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where study.ProcedureCodeSequenceCodeValue == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where SqlMethods.Like(study.ProcedureCodeSequenceCodeValue, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.ProcedureCodeSequenceCodeValue);
            }
        }

        internal class CodingSchemeDesignator : StringDicomPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.ProcedureCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where study.ProcedureCodeSequenceCodingSchemeDesignator == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where SqlMethods.Like(study.ProcedureCodeSequenceCodingSchemeDesignator, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.ProcedureCodeSequenceCodingSchemeDesignator);
            }
        }
    }

    internal class ModalitiesInStudy : StringDicomPropertyFilter<Study>
    {
        public ModalitiesInStudy(DicomAttributeCollection criteria)
            : base(DicomTags.ModalitiesInStudy, criteria)
        {
            AddToQueryEnabled = true;
            FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Study item)
        {
            return item.ModalitiesInStudy;
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.ModalitiesInStudy == criterion
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"{0}\%", criterion))
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"%\{0}", criterion))
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"%\{0}\%", criterion))
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.ModalitiesInStudy, criterion)
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"{0}\%", criterion))
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"%\{0}", criterion))
                        || SqlMethods.Like(study.ModalitiesInStudy, String.Format(@"%{0}\%", criterion))
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ModalitiesInStudy);
        }
    }

    internal class SopClassesInStudy : StringDicomPropertyFilter<Study>
    {
        public SopClassesInStudy(DicomAttributeCollection criteria)
            : base(DicomTags.SopClassesInStudy, criteria)
        {
            AddToQueryEnabled = true;
            FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Study item)
        {
            return item.SopClassesInStudy;
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.SopClassesInStudy == criterion
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"{0}\%", criterion))
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}", criterion))
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}\%", criterion))
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.SopClassesInStudy, criterion)
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"{0}\%", criterion))
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%\{0}", criterion))
                        || SqlMethods.Like(study.SopClassesInStudy, String.Format(@"%{0}\%", criterion))
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SopClassesInStudy);
        }
    }

    internal class ReferringPhysiciansName : StringDicomPropertyFilter<Study>
    {
        public ReferringPhysiciansName(DicomAttributeCollection criteria)
            : base(DicomTags.ReferringPhysiciansName, criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.ReferringPhysiciansName == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.ReferringPhysiciansName, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ReferringPhysiciansName);
        }
    }

    #endregion

    #region Dates

    internal class StudyDate : DateDicomPropertyFilter<Study>
    {
        public StudyDate(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyDate), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this is exact, ignoring studies with no date.
            return from study in query where study.StudyDate == date select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this excludes studies with no dates.
            return from study in query where study.StudyDate != null && study.StudyDate >= date select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this includes studies with no date.
            return from study in query where study.StudyDate == null || study.StudyDate <= date select study;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> query, DateTime startDate, DateTime endDate)
        {
            //DICOM says this excludes studies with no dates.
            return from study in query
                   where study.StudyDate >= startDate && study.StudyDate <= endDate
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyDateRaw);
        }
    }

    #endregion

    #region Times

    internal class StudyTime : TimePropertyFilter<Study>
    {
        public StudyTime(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.StudyTime), criteria)
        {
        }

        //NOTE (Marmot): None if this will get called currently because the base class doesn't work.
        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this is exact, ignoring studies with no time.
            return from study in query where study.StudyTimeTicks == timeTicks select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this excludes studies with no time.
            return from study in query where study.StudyTimeTicks != null && study.StudyTimeTicks >= timeTicks select study;

        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this includes studies with no time.
            return from study in query where study.StudyTimeTicks == null || study.StudyTimeTicks <= timeTicks select study;
        }

        protected override IQueryable<Study> AddBetweenTimesToQuery(IQueryable<Study> query, long startTimeTicks, long endTimeTicks)
        {
            //DICOM says this excludes studies with no time.
            return from study in query
                   where study.StudyTimeTicks >= startTimeTicks && study.StudyTimeTicks <= endTimeTicks
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.StudyTimeRaw);
        }
    }

    #endregion

    #region Non-Queryable

    internal class NumberOfStudyRelatedInstances : DicomPropertyFilter<Study>
    {
        public NumberOfStudyRelatedInstances(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedInstances), criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            if (item.NumberOfStudyRelatedInstances.HasValue)
                resultAttribute.SetInt32(0, item.NumberOfStudyRelatedInstances.Value);
            else
                resultAttribute.SetNullValue();
        }
    }

    internal class NumberOfStudyRelatedSeries : DicomPropertyFilter<Study>
    {
        public NumberOfStudyRelatedSeries(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.NumberOfStudyRelatedSeries), criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            if (item.NumberOfStudyRelatedSeries.HasValue)
                resultAttribute.SetInt32(0, item.NumberOfStudyRelatedSeries.Value);
            else
                resultAttribute.SetNullValue();
        }
    }

    #endregion
}