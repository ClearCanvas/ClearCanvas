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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class StudyTableItem : IStudyRootStudyIdentifier, IStudyEntryData
    {
        private static readonly string[] _emptyStringArray = new string[0];
        private StudyEntry _entry;

        public StudyTableItem(StudyEntry entry)
        {
            Entry = entry;
        }

        public StudyEntry Entry
        {
            get { return _entry; }
            set
            {
                _entry = value;
                _entry.Study.ResolveServer(true);
                if (_entry.Data == null)
                    _entry.Data = new StudyEntryData();
            }
        }

        private IStudyRootStudyIdentifier Identifier
        {
            get { return Entry.Study; }
        }

        private IStudyEntryData EntryData
        {
            get { return Entry.Data; }
        }

        #region IStudyEntryData Members

        public string[] InstitutionNamesInStudy
        {
            get { return EntryData.InstitutionNamesInStudy ?? _emptyStringArray; }
            set { EntryData.InstitutionNamesInStudy = value; }
        }

        public string[] StationNamesInStudy
        {
            get { return EntryData.StationNamesInStudy ?? _emptyStringArray; }
            set { EntryData.StationNamesInStudy = value; }
        }

        public string[] SourceAETitlesInStudy
        {
            get { return EntryData.SourceAETitlesInStudy ?? _emptyStringArray; }
            set { EntryData.SourceAETitlesInStudy = value; }
        }

        public DateTime? DeleteTime
        {
            get { return EntryData.DeleteTime; }
            set { EntryData.DeleteTime = value; }
        }

        public DateTime? StoreTime
        {
            get { return EntryData.StoreTime; }
            set { EntryData.StoreTime = value; }
        }

        #endregion

        #region IStudyRootStudyIdentifier Members

        public string ResponsibleOrganization
        {
            get { return Identifier.ResponsibleOrganization ?? string.Empty; }
        }

        public string SpecificCharacterSet
        {
            get { return Identifier.SpecificCharacterSet ?? string.Empty; }
        }

        string IIdentifier.RetrieveAeTitle
        {
            get { return Identifier.RetrieveAeTitle ?? string.Empty; }
        }

        IApplicationEntity IIdentifier.RetrieveAE
        {
            get { return Identifier.RetrieveAE; }
        }

        public string InstanceAvailability
        {
            get { return Identifier.InstanceAvailability ?? string.Empty; }
        }

        public string ResponsiblePersonRole
        {
            get { return Identifier.ResponsiblePersonRole ?? string.Empty; }
        }

        public string ResponsiblePerson
        {
            get { return Identifier.ResponsiblePerson ?? string.Empty; }
        }

        public string PatientBreedCodeSequenceCodeMeaning
        {
            get { return Identifier.PatientBreedCodeSequenceCodeMeaning ?? string.Empty; }
        }

        public string PatientBreedCodeSequenceCodeValue
        {
            get { return Identifier.PatientBreedCodeSequenceCodeValue ?? string.Empty; }
        }

        public string PatientBreedCodeSequenceCodingSchemeDesignator
        {
            get { return Identifier.PatientBreedCodeSequenceCodingSchemeDesignator ?? string.Empty; }
        }

        public string PatientBreedDescription
        {
            get { return Identifier.PatientBreedDescription ?? string.Empty; }
        }

        public string PatientSpeciesCodeSequenceCodeMeaning
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodeMeaning ?? string.Empty; }
        }

        public string PatientSpeciesCodeSequenceCodeValue
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodeValue ?? string.Empty; }
        }

        public string PatientSpeciesCodeSequenceCodingSchemeDesignator
        {
            get { return Identifier.PatientSpeciesCodeSequenceCodingSchemeDesignator ?? string.Empty; }
        }

        public string PatientSpeciesDescription
        {
            get { return Identifier.PatientSpeciesDescription ?? string.Empty; }
        }

        public string PatientsSex
        {
            get { return Identifier.PatientsSex ?? string.Empty; }
        }

        public string PatientsBirthTime
        {
            get { return Identifier.PatientsBirthTime ?? string.Empty; }
        }

        public string PatientsBirthDate
        {
            get { return Identifier.PatientsBirthDate ?? string.Empty; }
        }

        public PersonName PatientsName { get { return new PersonName(Identifier.PatientsName ?? ""); } }

        string IPatientData.PatientsName
        {
            get { return Identifier.PatientsName ?? string.Empty; }
        }

        public string PatientId
        {
            get { return Identifier.PatientId ?? string.Empty; }
        }

        public int? NumberOfStudyRelatedInstances
        {
            get { return Identifier.NumberOfStudyRelatedInstances; }
        }

        public int? NumberOfStudyRelatedSeries
        {
            get { return Identifier.NumberOfStudyRelatedSeries; }
        }

        public PersonName ReferringPhysiciansName { get { return new PersonName(Identifier.ReferringPhysiciansName ?? ""); }}
        
        string IStudyData.ReferringPhysiciansName
        {
            get { return Identifier.ReferringPhysiciansName ?? string.Empty; }
        }

        public string AccessionNumber
        {
            get { return Identifier.AccessionNumber ?? string.Empty; }
        }

        public string StudyTime
        {
            get { return Identifier.StudyTime ?? string.Empty; }
        }

        public string StudyDate
        {
            get { return Identifier.StudyDate ?? string.Empty; }
        }

        public string StudyId
        {
            get { return Identifier.StudyId ?? string.Empty; }
        }

        public string StudyDescription
        {
            get { return Identifier.StudyDescription ?? string.Empty; }
        }

        public string[] ModalitiesInStudy
        {
            get { return Identifier.ModalitiesInStudy ?? _emptyStringArray; }
        }

        public string[] SopClassesInStudy
        {
            get { return Identifier.SopClassesInStudy ?? _emptyStringArray; }
        }

        public string StudyInstanceUid
        {
            get { return Identifier.StudyInstanceUid ?? string.Empty; }
        }

        public IDicomServiceNode Server
        {
            get { return (IDicomServiceNode)((IIdentifier)this).RetrieveAE; }
        }

        #endregion
    }
}