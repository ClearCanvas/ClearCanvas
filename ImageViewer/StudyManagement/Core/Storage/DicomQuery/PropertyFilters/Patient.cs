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
using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    internal class PatientId : StringDicomPropertyFilter<Study>
    {
        public PatientId(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientId), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.PatientId == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.PatientId, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientId);
        }
    }

    internal class PatientsName : StringDicomPropertyFilter<Study>
    {
        public PatientsName(DicomAttributeCollection criteria)
            : base(DicomTags.PatientsName, criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.PatientsName == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.PatientsName, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsName);
        }
    }

    internal class PatientsSex : StringDicomPropertyFilter<Study>
    {
        public PatientsSex(DicomAttributeCollection criteria)
            : base(DicomTags.PatientsSex, criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.PatientsSex == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.PatientsSex, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsSex);
        }
    }

    internal class PatientsBirthDate : DateDicomPropertyFilter<Study>
    {
        public PatientsBirthDate(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientsBirthDate), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this is exact, ignoring studies with no date.
            return from study in query where study.PatientsBirthDate == date select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this excludes studies with no dates.
            return from study in query where study.PatientsBirthDate != null && study.PatientsBirthDate >= date select study;
        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, DateTime date)
        {
            //DICOM says this includes studies with no date.
            return from study in query where study.PatientsBirthDate == null || study.PatientsBirthDate <= date select study;
        }

        protected override IQueryable<Study> AddBetweenDatesToQuery(IQueryable<Study> query, DateTime startDate, DateTime endDate)
        {
            //DICOM says this excludes studies with no dates.
            return from study in query
                   where study.PatientsBirthDate >= startDate && study.PatientsBirthDate <= endDate
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsBirthDateRaw);
        }
    }

    internal class PatientsBirthTime : TimePropertyFilter<Study>
    {
        public PatientsBirthTime(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientsBirthTime), criteria)
        {
        }

        //NOTE (Marmot): None if this will get called currently because the base class doesn't work.
        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this is exact, ignoring studies with no time.
            return from study in query where study.PatientsBirthTimeTicks == timeTicks select study;
        }

        protected override IQueryable<Study> AddGreaterOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this excludes studies with no time.
            return from study in query where study.PatientsBirthTimeTicks != null && study.PatientsBirthTimeTicks >= timeTicks select study;

        }

        protected override IQueryable<Study> AddLessOrEqualToQuery(IQueryable<Study> query, long timeTicks)
        {
            //DICOM says this includes studies with no time.
            return from study in query where study.PatientsBirthTimeTicks == null || study.PatientsBirthTimeTicks <= timeTicks select study;
        }

        protected override IQueryable<Study> AddBetweenTimesToQuery(IQueryable<Study> query, long startTimeTicks, long endTimeTicks)
        {
            //DICOM says this excludes studies with no time.
            return from study in query
                   where study.PatientsBirthTimeTicks >= startTimeTicks && study.PatientsBirthTimeTicks <= endTimeTicks
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientsBirthTimeRaw);
        }
    }
}