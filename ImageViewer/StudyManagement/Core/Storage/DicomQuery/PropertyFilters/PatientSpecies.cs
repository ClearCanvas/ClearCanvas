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
    internal class PatientSpeciesDescription : StringDicomPropertyFilter<Study>
    {
        public PatientSpeciesDescription(DicomAttributeCollection criteria)
            : base(new DicomTagPath(DicomTags.PatientSpeciesDescription), criteria)
        {
        }

        protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where study.PatientSpeciesDescription == criterion
                   select study;
        }

        protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
        {
            //DICOM says for any keys (required or optional) that we support matching on will always consider
            //an empty value to be a match regardless of what the criteria is, but we're not doing that
            //because it doesn't make sense.
            return from study in query
                   where SqlMethods.Like(study.PatientSpeciesDescription, criterion)
                   select study;
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.PatientSpeciesDescription);
        }
    }

    internal class PatientSpeciesCodeSequence
    {
        internal class CodingSchemeDesignator : StringDicomPropertyFilter<Study>
        {
            public CodingSchemeDesignator(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodingSchemeDesignator), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodingSchemeDesignator == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodingSchemeDesignator, criterion)
                       select study;
            }


            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodingSchemeDesignator);
            }
        }

        internal class CodeValue : StringDicomPropertyFilter<Study>
        {
            public CodeValue(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeValue), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeValue == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeValue, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeValue);
            }
        }

        internal class CodeMeaning : StringDicomPropertyFilter<Study>
        {
            public CodeMeaning(DicomAttributeCollection criteria)
                : base(new DicomTagPath(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeMeaning), criteria)
            {
            }

            protected override IQueryable<Study> AddEqualsToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where study.PatientSpeciesCodeSequenceCodeMeaning == criterion
                       select study;
            }

            protected override IQueryable<Study> AddLikeToQuery(IQueryable<Study> query, string criterion)
            {
                //DICOM says for any keys (required or optional) that we support matching on will always consider
                //an empty value to be a match regardless of what the criteria is, but we're not doing that
                //because it doesn't make sense.
                return from study in query
                       where SqlMethods.Like(study.PatientSpeciesCodeSequenceCodeMeaning, criterion)
                       select study;
            }

            protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.PatientSpeciesCodeSequenceCodeMeaning);
            }
        }
    }
}