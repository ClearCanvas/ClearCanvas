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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml.Study
{
    /// <summary>
    /// Represents an <see cref="IStudy"/> whose main source of data is a <see cref="StudyXml"/> document.
    /// </summary>
    public class Study : IStudy
    {
        private readonly StudyXml _xml;
        private IList<ISeries> _series;

        public Study(StudyXml xml, IDicomFileLoader headerProvider)
        {
            _xml = xml;
            HeaderProvider = headerProvider;
        }

        internal IDicomFileLoader HeaderProvider { get; private set; }

		public ISopInstance FirstSopInstance
        {
            get { return Series.First().SopInstances.First(); }
        }

        #region Implementation of IStudy

        public IList<ISeries> Series
        {
            get { return _series ?? (_series = _xml.Select(x => (ISeries)new Series(x, this)).ToList()); }
        }

        #endregion

        #region IStudyData Members

        public string StudyInstanceUid
        {
            get { return _xml.StudyInstanceUid; }
        }

        public string[] SopClassesInStudy
        {
            get
            {
                return (from series in Series
                        from sop in series.SopInstances
                        select sop.SopClassUid).Distinct().ToArray();
            }
        }

        public string[] ModalitiesInStudy
        {
            get
            {
                var list = Series.Select(s => s.Modality).Distinct().ToList();
                list.Sort();
                return list.ToArray();
            }
        }

        public string StudyDescription
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.StudyDescription).ToString(); }
        }

        public string StudyId
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.StudyId).ToString(); }
        }

        public DateTime? StudyDate
        {
            get { return DateParser.Parse(FirstSopInstance.GetAttribute(DicomTags.StudyDate).ToString()); }
        }

        string IStudyData.StudyDate
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.StudyDate).ToString(); }
        }

        public TimeSpan? StudyTime
        {
            get
            {
                var time = TimeParser.Parse(FirstSopInstance.GetAttribute(DicomTags.StudyTime).ToString());
                if (time.HasValue)
                    return time.Value.TimeOfDay;

                return null;
            }
        }

        string IStudyData.StudyTime
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.StudyTime).ToString(); }
        }

        public string AccessionNumber
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.AccessionNumber).ToString(); }
        }

        public string ReferringPhysiciansName
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.ReferringPhysiciansName).ToString(); }
        }

        public int? NumberOfStudyRelatedSeries
        {
            get { return Series.Count; }
        }

        public int? NumberOfStudyRelatedInstances
        {
            get { return Series.Sum(s => s.SopInstances.Count); }
        }

        #endregion

        #region IPatientData Members

        public string PatientId
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientId).ToString(); }
        }

        public string PatientsName
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientsName).ToString(); }
        }

        public DateTime? PatientsBirthDate { get { return DateParser.Parse(FirstSopInstance.GetAttribute(DicomTags.PatientsBirthDate).ToString()); } }

        string IPatientData.PatientsBirthDate
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientsBirthDate).ToString(); }
        }

        public TimeSpan? PatientsBirthTime
        {
            get
            {
                var time = TimeParser.Parse(FirstSopInstance.GetAttribute(DicomTags.PatientsBirthTime).ToString());
                if (time.HasValue)
                    return time.Value.TimeOfDay;

                return null;
            }
        }

        string IPatientData.PatientsBirthTime
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientsBirthTime).ToString(); }
        }

        public string PatientsSex
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientsSex).ToString(); }
        }

        public string PatientSpeciesDescription
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientSpeciesDescription).ToString(); }
        }

        public string PatientSpeciesCodeSequenceCodingSchemeDesignator
        {
            get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodingSchemeDesignator); }
        }

        public string PatientSpeciesCodeSequenceCodeValue
        {
            get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeValue); }
        }

        public string PatientSpeciesCodeSequenceCodeMeaning
        {
            get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeMeaning); }
        }

        public string PatientBreedDescription
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.PatientBreedDescription).ToString(); }
        }

        public string PatientBreedCodeSequenceCodingSchemeDesignator
        {
            get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodingSchemeDesignator); }

        }

        public string PatientBreedCodeSequenceCodeValue
        {
            get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodeValue); }
        }

        public string PatientBreedCodeSequenceCodeMeaning
        {
            get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodeMeaning); }
        }

        public string ResponsiblePerson
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.ResponsiblePerson).ToString(); }
        }

        public string ResponsiblePersonRole
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.ResponsiblePersonRole).ToString(); }
        }

        public string ResponsibleOrganization
        {
            get { return FirstSopInstance.GetAttribute(DicomTags.ResponsibleOrganization).ToString(); }
        }

        #endregion

        private string GetSequenceValue(uint sequenceTag, uint itemTag)
        {
            var sequence = FirstSopInstance.GetAttribute(sequenceTag) as DicomAttributeSQ;
            if (sequence == null)
                return String.Empty;

            var item = sequence[0];
            if (item == null)
                return String.Empty;

            return item[itemTag].ToString();
        }
    }
}