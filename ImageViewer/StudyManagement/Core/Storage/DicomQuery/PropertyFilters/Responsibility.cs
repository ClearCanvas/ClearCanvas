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

using System.Data.Linq.SqlClient;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    internal class ResponsiblePerson : StringDicomPropertyFilter<Study>
    {
        public ResponsiblePerson(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePerson), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.ResponsiblePerson == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.ResponsiblePerson, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePerson);
        }
    }

    internal class ResponsiblePersonRole : StringDicomPropertyFilter<Study>
    {
        public ResponsiblePersonRole(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsiblePersonRole), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.ResponsiblePersonRole == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.ResponsiblePersonRole, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsiblePersonRole);
        }
    }

    internal class ResponsibleOrganization : StringDicomPropertyFilter<Study>
    {
        public ResponsibleOrganization(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.ResponsibleOrganization), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.ResponsibleOrganization == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.ResponsibleOrganization, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.ResponsibleOrganization);
        }
    }
}