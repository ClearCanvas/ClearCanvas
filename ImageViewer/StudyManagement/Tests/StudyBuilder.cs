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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
    public class StudyBuilder : IPatientData, IStudyData
    {
        private List<SeriesBuilder> _series;
        private IDisplaySetFactory _displaySetFactory;
        
        public StudyBuilder()
        {
        }

        public bool IsRemote { get; set; }
        public IDisplaySetFactory DisplaySetFactory
        {
            get
            {
                if (_displaySetFactory == null)
                    _displaySetFactory = new BasicDisplaySetFactory();

                return _displaySetFactory;
            }
            set { _displaySetFactory = value; }
        }

        public string ReconciledPatientId { get; set; }

        #region Implementation of IPatientData

        [DicomField(DicomTags.PatientId, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string PatientId { get; set; }
        [DicomField(DicomTags.PatientsName, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string PatientsName { get; set; }
        [DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string PatientsBirthDate { get; set; }
        [DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string PatientsBirthTime { get; set; }
        [DicomField(DicomTags.PatientsSex, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string PatientsSex { get; set; }

        string IPatientData.PatientSpeciesDescription
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientSpeciesCodeSequenceCodingSchemeDesignator
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientSpeciesCodeSequenceCodeValue
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientSpeciesCodeSequenceCodeMeaning
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientBreedDescription
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientBreedCodeSequenceCodingSchemeDesignator
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientBreedCodeSequenceCodeValue
        {
            get { return String.Empty; }
        }

        string IPatientData.PatientBreedCodeSequenceCodeMeaning
        {
            get { return String.Empty; }
        }

        string IPatientData.ResponsiblePerson
        {
            get { return String.Empty; }
        }

        string IPatientData.ResponsiblePersonRole
        {
            get { return String.Empty; }
        }

        string IPatientData.ResponsibleOrganization
        {
            get { return String.Empty; }
        }

        #endregion

        #region Implementation of IStudyData

        [DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string StudyInstanceUid { get; set; }
        [DicomField(DicomTags.StudyDescription, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string StudyDescription { get; set; }
        [DicomField(DicomTags.StudyId, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string StudyId { get; set; }
        [DicomField(DicomTags.StudyDate, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string StudyDate { get; set; }
        [DicomField(DicomTags.StudyTime, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string StudyTime { get; set; }
        [DicomField(DicomTags.AccessionNumber, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string AccessionNumber { get; set; }
        [DicomField(DicomTags.ReferringPhysiciansName, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string ReferringPhysiciansName { get; set; }

        string[] IStudyData.SopClassesInStudy { get { return new string[] { "" }; } }
        string[] IStudyData.ModalitiesInStudy { get { return new string[]{""}; } }
        int? IStudyData.NumberOfStudyRelatedSeries { get { return Series.Count; } }
        int? IStudyData.NumberOfStudyRelatedInstances { get { return (from s in Series from i in s.Images select i).Count(); } }

        #endregion

        public List<SeriesBuilder> Series
        {
            get
            {
                if (_series == null)
                    _series = new List<SeriesBuilder>();
                return _series;
            }
            set { _series = value; }
        }

        public List<DicomFile> BuildFiles()
        {
            var files = new List<DicomFile>();
            if (StudyInstanceUid == null)
                StudyInstanceUid = DicomUid.GenerateUid().UID;

            foreach (var series in Series)
                files.AddRange(series.Build(this));

            return files;
        }

        public List<Sop> BuildSops()
        {
            return BuildFiles().ConvertAll(f => Sop.Create(new TestDataSource(f)));
        }

        public Study AddStudy(StudyTree studyTree)
        {
            var sops = BuildSops();
            foreach (var sop in sops)
                studyTree.AddSop(sop);

            return studyTree.GetStudy(sops[0].StudyInstanceUid);
        }

        public IImageSet BuildImageSet(StudyTree studyTree, out Study study)
        {
            if (IsRemote)
            {
                study = null;
                //Add only the image set, which essentially makes it a remote study.
                var identifier = new StudyRootStudyIdentifier(this, this, new StudyIdentifier())
                                     {
                                         PatientId = ReconciledPatientId ?? PatientId
                                     };

                return new ImageSet(new DicomImageSetDescriptor(identifier));
            }

            study = AddStudy(studyTree);
            var studyIdentifier = study.GetIdentifier();
            studyIdentifier = new StudyRootStudyIdentifier(studyIdentifier) { PatientId = ReconciledPatientId ?? study.ParentPatient.PatientId };
            var imageSet = new ImageSet(new DicomImageSetDescriptor(studyIdentifier));
            foreach (var series in study.Series)
            {
                foreach (var displaySet in DisplaySetFactory.CreateDisplaySets(series))
                    imageSet.DisplaySets.Add(displaySet);
            }

            return imageSet;
        }
    }

    public class SeriesBuilder : ISeriesData
    {
        private List<ImageBuilder> _images;

        [DicomField(DicomTags.Modality, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string Modality { get; set; }

        [DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string SeriesInstanceUid { get; set; }

        [DicomField(DicomTags.SeriesDescription, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string SeriesDescription { get; set; }

        [DicomField(DicomTags.SeriesNumber, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public int SeriesNumber { get; set; }

        [DicomField(DicomTags.SeriesDate, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string SeriesDate { get; set; }

        [DicomField(DicomTags.SeriesTime, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string SeriesTime { get; set; }

        [DicomField(DicomTags.Laterality, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public int? Laterality { get; set; }

        string ISeriesData.StudyInstanceUid { get { return ""; } }
        int? ISeriesData.NumberOfSeriesRelatedInstances { get { return Images.Count; } }

        public List<ImageBuilder> Images
        {
            get
            {
                if (_images == null)
                    _images = new List<ImageBuilder>();
                return _images;
            }
            set { _images = value; }
        }

        public List<DicomFile> Build(StudyBuilder study)
        {
            if (SeriesInstanceUid == null)
                SeriesInstanceUid = DicomUid.GenerateUid().UID;

            var files = new List<DicomFile>();
            foreach (var image in Images)
                files.Add(image.Build(study, this));

            return files;
        }
    }

    public class ImageBuilder : AbstractTest //a little weird to inherit from this class, but oh well.
    {
        [DicomField(DicomTags.SopClassUid, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public string SopClassUid { get; set; }

        [DicomField(DicomTags.InstanceNumber, CreateEmptyElement = false, SetNullValueIfEmpty = true)]
        public int? InstanceNumber { get; set; }

        [DicomField(DicomTags.NumberOfFrames, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public int? NumberOfFrames { get; set; }

        [DicomField(DicomTags.ImageLaterality, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public string ImageLaterality { get; set; }

        [DicomField(DicomTags.ViewPosition, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public string ViewPosition { get; set; }

        [DicomField(DicomTags.PresentationIntentType, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public string PresentationIntentType { get; set; }

        [DicomField(DicomTags.ImagePositionPatient, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public string ImagePositionPatient { get; set; }

        [DicomField(DicomTags.ImageOrientationPatient, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public string ImageOrientationPatient { get; set; }

        [DicomField(DicomTags.EchoNumbers, CreateEmptyElement = false, SetNullValueIfEmpty = false)]
        public int? EchoNumber { get; set; }

        public DicomFile Build(StudyBuilder study, SeriesBuilder series)
        {
            var file = new DicomFile();
            var dataset = file.DataSet;

            if (study != null)
                dataset.SaveDicomFields(study);
            if (series != null)
                dataset.SaveDicomFields(series);

            dataset.SaveDicomFields(this);

            dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);

            dataset[DicomTags.PixelSpacing].SetStringValue(string.Format(@"{0}\{1}", 0.5, 0.5));
            dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
            dataset[DicomTags.SamplesPerPixel].SetInt32(0, 1);
            dataset[DicomTags.BitsStored].SetInt32(0, 16);
            dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
            dataset[DicomTags.HighBit].SetInt32(0, 15);
            dataset[DicomTags.PixelRepresentation].SetInt32(0, 1);
            dataset[DicomTags.Rows].SetInt32(0, 10);
            dataset[DicomTags.Columns].SetInt32(0, 10);

            CreatePixelData(dataset);

            SetupMetaInfo(file);
            file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            return file;
        }
    }
}

#endif