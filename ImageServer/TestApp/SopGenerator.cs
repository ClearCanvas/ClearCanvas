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
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageServer.TestApp
{
    public class SopGeneratorHospital
    {
        private readonly object _syncLock = new object();

        public string Name { get; set; }
        public int Accession { get; set; }
        public int ScheduledProcedureStepId { get; set; }
        public int RequestedProcedureStepId { get; set; }

        public string GetNextAccession()
        {
            lock (_syncLock)
            {
                Accession++;
                return string.Format("A{0}", Accession);
            }
        }

        public string GetNextScheduledProcedureStepId()
        {
            lock (_syncLock)
            {
                ScheduledProcedureStepId++;
                return string.Format("{0}{1}", ScheduledProcedureStepId, Name);
            }
        }

        public string GetNextRequestedProcedureStepId()
        {
            lock (_syncLock)
            {
                RequestedProcedureStepId++;
                return string.Format("{0}{1}", RequestedProcedureStepId, Name);
            }
        }

    }

    public abstract class SopGenerator
    {

        public DateTime StudyDate { get; set; }
        public string StudyInstanceUid { get; set; }
        public string Modality { get; set; }
        public string SopClassUid { get; set; }
        public SopGeneratorHospital ModalityHospital { get; set; }
        public int MaxSeries { get; set; }
        private static readonly Random Rand = new Random();
        private static readonly object SyncLock = new object();
        private static readonly List<string> _lastNames = new List<string>();
        private static readonly List<string> _givenNames = new List<string>();
        protected readonly List<string> _seriesDescription = new List<string>();
        protected readonly List<string> _studyDescription = new List<string>();
        private int _seriesNumber = 0;
        private int _studyNumber = 1;
        protected DicomFile _theFile;

        protected SopGenerator()
        {
            StudyInstanceUid = DicomUid.GenerateUid().UID;
            StudyDate = DateTime.Now;
            EnsureTagsSet();
        }

        private static void EnsureTagsSet()
        {
            lock (SyncLock)
            {
                if (_lastNames.Count == 0)
                {
                    using (StreamReader re = File.OpenText("LastNames.txt"))
                    {
                        string input;
                        while ((input = re.ReadLine()) != null)
                        {
                            _lastNames.Add(input.Trim());
                        }
                        re.Close();
                    }
                }
                if (_givenNames.Count == 0)
                {

                    using (StreamReader re = File.OpenText("GivenNames.txt"))
                    {
                        string input;
                        while ((input = re.ReadLine()) != null)
                        {
                            _givenNames.Add(input.Trim());
                        }
                        re.Close();
                    }
                }
            }
        }

        private static string GetPatient()
        {
            lock (SyncLock)
            {
                   return String.Format("{0}^{1}", _lastNames[Rand.Next(_lastNames.Count)],
                                  _givenNames[Rand.Next(_givenNames.Count)]);
            }
        }

        private DateTime GetBirthdate(string name)
        {
            return StudyDate.AddDays(-1 * (Math.Abs(name.GetHashCode()) % 30000));
        }

        private void SetStudyTags()
        {
            StudyInstanceUid = DicomUid.GenerateUid().UID;

            _theFile.DataSet[DicomTags.StudyInstanceUid].SetStringValue(StudyInstanceUid);
            _theFile.DataSet[DicomTags.StudyDate].SetStringValue(DateParser.ToDicomString(StudyDate));
            _theFile.DataSet[DicomTags.StudyTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
            _theFile.DataSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue(DateParser.ToDicomString(StudyDate));
            _theFile.DataSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
            _theFile.DataSet[DicomTags.AccessionNumber].SetStringValue(ModalityHospital.GetNextAccession());

            _studyNumber++;
            _theFile.DataSet[DicomTags.StudyId].SetStringValue(string.Format("S{0:0000}",_studyNumber));
            _theFile.DataSet[DicomTags.ReferringPhysiciansName].SetStringValue(GetPatient());

            string name = GetPatient();
            _theFile.DataSet[DicomTags.PatientsName].SetStringValue(name);
            _theFile.DataSet[DicomTags.PatientId].SetStringValue(string.Format("{0} {1}", Math.Abs(name.GetHashCode()), ModalityHospital.Name));
            _theFile.DataSet[DicomTags.IssuerOfPatientId].SetStringValue(ModalityHospital.Name);

            DateTime birthdate = GetBirthdate(name);
            TimeSpan age = StudyDate.Subtract(birthdate);

            _theFile.DataSet[DicomTags.PatientsBirthDate].SetStringValue(DateParser.ToDicomString(birthdate));
            _theFile.DataSet[DicomTags.PatientsAge].SetStringValue(String.Format("{0:000}Y", age.Days / 365));
            _theFile.DataSet[DicomTags.PatientsSex].SetStringValue("M");

            _theFile.DataSet[DicomTags.StudyDescription].SetStringValue(_studyDescription[Rand.Next(_studyDescription.Count)]);

        }

        private void SetSeriesTags()
        {
            _theFile.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);

            _theFile.DataSet[DicomTags.SeriesDate].SetStringValue(DateParser.ToDicomString(StudyDate));
            _theFile.DataSet[DicomTags.SeriesTime].SetStringValue(TimeParser.ToDicomString(DateTime.Now));
            _theFile.DataSet[DicomTags.Modality].SetStringValue(Modality);

            DicomSequenceItem item = new DicomSequenceItem();
            _theFile.DataSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);
            item[DicomTags.RequestedProcedureId].SetStringValue(ModalityHospital.GetNextRequestedProcedureStepId());
            item[DicomTags.ScheduledProcedureStepId].SetStringValue(ModalityHospital.GetNextScheduledProcedureStepId());

            _theFile.DataSet[DicomTags.SeriesDescription].SetStringValue(_seriesDescription[Rand.Next(_seriesDescription.Count)]);
            _seriesNumber++;
            _theFile.DataSet[DicomTags.SeriesNumber].AppendInt32(_seriesNumber);
     
        }

        private void SetSopTags( )
        {
            EnsureTagsSet();

            _theFile.MetaInfo[DicomTags.SourceApplicationEntityTitle].SetStringValue(String.Format("{0}_{1}",ModalityHospital.Name, Modality));
            _theFile.DataSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            _theFile.DataSet[DicomTags.SopClassUid].SetStringValue(SopClassUid);

            _theFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            _theFile.DataSet[DicomTags.SopClassUid].SetStringValue(SopClassUid);
        }

        public DicomFile NewStudy(DateTime studyDate)
        {
            _theFile = new DicomFile();
            StudyDate = studyDate;
            SetStudyTags();
            SetSeriesTags();
            SetSopTags();
            return _theFile;
        }

        public DicomFile NewSeries()
        {
            SetSeriesTags();
            SetSopTags();
            return _theFile;
        }

        public DicomFile NewSop()
        {
            SetSopTags();
            return _theFile;
        }
    }
    

    public class MrSopGenerator : SopGenerator
    {
        public MrSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "MR";
            SopClassUid = SopClass.MrImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 4;
            _seriesDescription.Add("aortic valve PC (500 VENC) 5/0 NBH_P");
            _seriesDescription.Add("aortic valve PC (500 VENC)_P");
            _seriesDescription.Add("AX  T1W_FS PRE");
            _seriesDescription.Add("AX  T1W_TSE");
            _seriesDescription.Add("AX  T1W_TSE LOWER");
            _seriesDescription.Add("AX  T2");
            _seriesDescription.Add("AX  T2   SURGICAL SITE");
            _seriesDescription.Add("COR STIR  BILAT");
            _seriesDescription.Add("COR STIR  BILATERAL");
            _seriesDescription.Add("COR STIR - BILATERAL");
            _seriesDescription.Add("COR STIR  BILATERAL FEMORA");
            _seriesDescription.Add("COR T1 F.S. POST GAD RPT");
            _seriesDescription.Add("COR T1 F.S. POST GAD RT");
            _seriesDescription.Add("COR T1 F.S. POST GAD RT RPT");
            _seriesDescription.Add("SAG FSPGR T1");
            _seriesDescription.Add("SAG FSPGR T1 FS POST GAD");
            _seriesDescription.Add("SAG FSPGR T1 w Gad delayed");
            _seriesDescription.Add("STIR COR lt wrist");
            _seriesDescription.Add("STIR COR lt wrist #2");
            _seriesDescription.Add("STIR COR lt wrist /hand");
            _seriesDescription.Add("STIR COR lt wrsit");

            _studyDescription.Add("ABDOMEN (KIDNEY)");
            _studyDescription.Add("ABDOMEN (LIVER)");
            _studyDescription.Add("ABDOMEN (MRCP)");
            _studyDescription.Add("ABDOMEN (PANCREAS)");
            _studyDescription.Add("ABDOMEN (RENAL)");
            _studyDescription.Add("BRAIN - MRA)");
            _studyDescription.Add("BRAIN  MRV)");
            _studyDescription.Add("Chest MEDIASTINAL MASS RT)");
            _studyDescription.Add("Chest MEDIASTINUM");
            _studyDescription.Add("CHEST MEDISTINAL MASS");
            _studyDescription.Add("CHEST- NON OTMH PROTOCOL^MASS");
            _studyDescription.Add("Chest PAN COAST");
            _studyDescription.Add("C-spine ATECO");
            _studyDescription.Add("C-spine Diffusion");
            _studyDescription.Add("cspine fusion");
            _studyDescription.Add("C-SPINE GAD");
            _studyDescription.Add("C-SPINE NE WITH 3 PUL SEQ-VC");
            _studyDescription.Add("C-spine post-op & ATECO Carotid");
            _studyDescription.Add("C-spine Trauma");
            _studyDescription.Add("FETAL/brain");
            _studyDescription.Add("FETAL NICU HEAD");
            _studyDescription.Add("FETAL NICU  BRAIN");
            _studyDescription.Add("MR SHOULDER");
            _studyDescription.Add("MR Shoulder Right - UE");
            _studyDescription.Add("MR Spine Lumbar");
            _studyDescription.Add("MR Spine Lumbar wwo 4 Seq-L");
            _studyDescription.Add("MR Thorax WWO Cont 4 Seq-CH");
            _studyDescription.Add("MR Upper Extremity Left - UE");
            _studyDescription.Add("MRA Head WWO Contrast");
            _studyDescription.Add("MRI  COMPLEX LSP");
            _studyDescription.Add("MRI  HEAD + POST PROCESSING");
        }
    }

    public class CtSopGenerator : SopGenerator
    {
        public CtSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "CT";
            MaxSeries = 2; 
            SopClassUid = SopClass.CtImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            _seriesDescription.Add("CTA,0.5,CE");
            _seriesDescription.Add("Head,0.5");
            _seriesDescription.Add(",,Oblique,5.0,Oblique,Oblique.1");
            _seriesDescription.Add(",,Sagittal,0.8,Sagittal,VENOUS/PHASE/Sagittal.1");
            _seriesDescription.Add(",,Scano,2.0");
            _seriesDescription.Add(",,Sft Tissue,0.5");
            _seriesDescription.Add(",,Spine,1.0");
            _seriesDescription.Add(",,Spine,2.0");
            _seriesDescription.Add(",,Spine,3.0");
            _seriesDescription.Add(",,Vol,1.0,Vol.");
            _seriesDescription.Add(",ABDOMEN/PELVIS,,2,CE");
            _seriesDescription.Add(",ABDOMEN/PELVIS,Body,5.0,CE");
            _seriesDescription.Add(",ABDOMEN/PELVIS/Coronal,Body,3.0,CE");
            _seriesDescription.Add(",ABDOMEN/PELVIS/Sagittal,Body,3.0,CE");
            _seriesDescription.Add(",ARTERIAL K,Body,0.5,CE");
            _seriesDescription.Add(",ARTERIAL K/Coronal,Body,3.0,CE");
            _seriesDescription.Add(",ARTERIAL,,5,CE");
            _seriesDescription.Add(",ARTERIAL,Body,3.0,CE");
            _seriesDescription.Add(",ARTERIAL,Body,5.0,");
            _seriesDescription.Add(",ARTERIAL,Body,5.0,CE");
            _seriesDescription.Add(",ARTERIAL,CTA,0.5,CE");
            _seriesDescription.Add(",ARTERIAL,CTA,1.0,CE");
            _seriesDescription.Add(",ARTERIAL,Lung,1.0,CE");
            _seriesDescription.Add(",ARTERIAL/Axial.Ref,Body,2.0,CE");
            _seriesDescription.Add(",ARTERIAL/Coronal.2,Body,0.8,CE");
            _seriesDescription.Add(",ARTERIAL/PHASE/Coronal,CTA,3.0,CE");
            _seriesDescription.Add(",ARTERIAL/PHASE/Sagittal,CTA,3.0,CE");
            _seriesDescription.Add(",ARTERIAL/Sagittal.Ref,Bone,3.0,CE");
            _seriesDescription.Add(",ARTERIAL/Sag-MIP,CTA,3.0,CE");
            _seriesDescription.Add(",Axial.1,Head,5.0,");
            _seriesDescription.Add(",Axial.13,Bone,2.0,");
            _studyDescription.Add("CT 3 PHASE LIVER");
            _studyDescription.Add("CT ABD, PELVIS W");
            _studyDescription.Add("CT ABD/LIVER W+WO");
            _studyDescription.Add("CT ANGIO HEAD");
            _studyDescription.Add("CT BRAIN LAB W");
            _studyDescription.Add("CT CERVICAL SPINE WO");
            _studyDescription.Add("CT CHEST");
            _studyDescription.Add("CT CHEST BIOPSY WO");
            _studyDescription.Add("CT CHEST HIRES");
            _studyDescription.Add("CT CHEST W");
            _studyDescription.Add("CT CHEST WO");
            _studyDescription.Add("CT CHEST, ABD W");
            _studyDescription.Add("CT CHEST,ABD,PELV W");
            _studyDescription.Add("CT Chest,Abdomen,Pelv");
            _studyDescription.Add("CT EXTREMITY LOWER WO");
            _studyDescription.Add("CT GUIDANCE BIOPSY         -A");
            _studyDescription.Add("CT HEAD");
            _studyDescription.Add("CT HEAD W");
            _studyDescription.Add("CT HEAD W+WO");
            _studyDescription.Add("CT HEAD WO");
            _studyDescription.Add("CT LUMBAR SPINE WO");
            _studyDescription.Add("CT NECK + CHEST W");
            _studyDescription.Add("CT NECK W");
            _studyDescription.Add("CT NECK, CHEST, ABDOMEN W");
            _studyDescription.Add("CT PULMONARY EMBOLUS");
            _studyDescription.Add("CT SINUSES");
            _studyDescription.Add("CT SPINE WO");
        }
    }

    public class MgSopGenerator : SopGenerator
    {
        public MgSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "MG";
            SopClassUid = SopClass.DigitalMammographyXRayImageStorageForPresentationUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 1; 
            _seriesDescription.Add("BILATERAL MAMMOGRAM PMH");
            _seriesDescription.Add("BILATERAL SPOT COMPRESSION");
            _seriesDescription.Add("BIOPSY SPECIMEN RT. BREAST, Specimen");
            _seriesDescription.Add("LT MAMMOGRAM - SPOT");
            _seriesDescription.Add("LT SP");
            _seriesDescription.Add("LT SPE");
            _seriesDescription.Add("LT SPECIMEN-FNA");
            _seriesDescription.Add("LT SPOT COMPRESSION VIEWS");
            _seriesDescription.Add("MAMMOGRAM-LEFT SPOT");
            _seriesDescription.Add("MAMOGRAM - BILATERAL");
            _seriesDescription.Add("MAMOGRAM-LEFT");
            _seriesDescription.Add("MAMOGRAPHY");
            _seriesDescription.Add("REVISIT-ADDED VWS BILAT.SCREEN");
            _seriesDescription.Add("RT BIOSPY");
            _seriesDescription.Add("RT MOL SPOT");
            _seriesDescription.Add("RT NEEDLE LOC");
            _seriesDescription.Add("RT NEEDLE LOC UNDER US");
            _seriesDescription.Add("RT ROLLED VIEWS");
            _seriesDescription.Add("RT SPECIMAN");
            _seriesDescription.Add("RT SPECIMEN-CENTRAL");
            _seriesDescription.Add("RT SPOT");
            _seriesDescription.Add("RT SPOT & MAG");
            _seriesDescription.Add("RT SPOT COMPORESSION");

            _studyDescription.Add("BILATERAL MAMMOGRM");
            _studyDescription.Add("BILATERAL NEEDLE LOC");
            _studyDescription.Add("BILATERAL PATHOLOGY SPECIMEN");
            _studyDescription.Add("BILATERAL SCREENING MAMMOGRAM DIGITAL");
            _studyDescription.Add("BILATERAL SPECIMEN");
            _studyDescription.Add("LEFT BREAST SPECIMEN");
            _studyDescription.Add("LEFT CORE BX");
            _studyDescription.Add("LEFT CORE BIOPSY");
            _studyDescription.Add("LEFT DUCTOGRAM");
            _studyDescription.Add("RIGHT LATERAL");
            _studyDescription.Add("RIGHT LOC");
            _studyDescription.Add("RIGHT MAG VIEWS");
            _studyDescription.Add("RIGHT MAGNIFICATION");
        }
    }


    public class RfSopGenerator : SopGenerator
    {

        public RfSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "RF";
            SopClassUid = SopClass.XRayRadiofluoroscopicImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 2;
            _seriesDescription.Add("Upper GI Series with Air Contrast - AB");
            _seriesDescription.Add("Upper GI Series with SBFT - AB");
            _seriesDescription.Add("Upper GI Single Contrast");
            _seriesDescription.Add("Upper_GI 1");

            _studyDescription.Add("COLON HE");
            _studyDescription.Add("COMPLETE MYELOGRAM");
            _studyDescription.Add("CTENTE");
            _studyDescription.Add("DISCOGRAM L4/5, L5/S1");
            _studyDescription.Add("Durchleuchtung Fisteldarstell.");
            _studyDescription.Add("ESOPHAGOGRAM,UGIS");
            _studyDescription.Add("FLUOROSCOPY BY MD PER 15 MIN");
            _studyDescription.Add("GI-UGI dc");
            _studyDescription.Add("J-TUBE");
            _studyDescription.Add("LT HIP GADO INJ.");
            _studyDescription.Add("LT SHOULDER MR ARTHROGRAM");
        }
    }

    public class XaSopGenerator : SopGenerator
    {
        public XaSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "XA";
            SopClassUid = SopClass.XRayAngiographicImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 3;
            _seriesDescription.Add("RT CENTRAL V POST BALLOON");
            _seriesDescription.Add("RT CENTRAL V PRE");
            _seriesDescription.Add("RT CENTRAL VEIN");
            _seriesDescription.Add("RT CENTRAL VEIN POST ANGIOPLAST RUN2");
            _seriesDescription.Add("RT CENTRAL VEIN POST ANGIOPLASTY 1");
            _seriesDescription.Add("LT CENTRAL V POST BALLOON");
            _seriesDescription.Add("LT CENTRAL V PRE");
            _seriesDescription.Add("LT CENTRAL VEIN");
            _seriesDescription.Add("LT CENTRAL VEIN POST ANGIOPLAST RUN2");
            _seriesDescription.Add("LT CENTRAL VEIN POST ANGIOPLASTY 1");

            _studyDescription.Add("ANGIOGRAM, CAROTID UNILATERAL");
            _studyDescription.Add("ANGIOPLAST");
            _studyDescription.Add("Angiogram Extremity");
            _studyDescription.Add("C-ARMBONE, SPINE COUNT");
            _studyDescription.Add("CARMBONE LUMBAR SPINE");
            _studyDescription.Add("FLUORO/SKULL");
            _studyDescription.Add("FLUORO/T SPINE");
            _studyDescription.Add("FLUORO-ABDOMEN");
            _studyDescription.Add("GJ-TUBE CHECK CHANGE");
            _studyDescription.Add("LT NEPH, TUBE REINSERTION");
            _studyDescription.Add("LT NEPHROSTOMY");
            _studyDescription.Add("Peripheral Line Removal");
            _studyDescription.Add("Peripheral Line Removal");
            _studyDescription.Add("Peripheral Run Off");
            _studyDescription.Add("SPINE FLUORO");
            _studyDescription.Add("SPINAL FLUORO");
            _studyDescription.Add("SPINE STIMULATION");
        }
    }

    public class CrSopGenerator : SopGenerator
    {

        public CrSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "CR";
            SopClassUid = SopClass.ComputedRadiographyImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 1;
            _seriesDescription.Add("Patella Sunrise Pa");
            _seriesDescription.Add("Pelvis");
            _seriesDescription.Add("PORT CHEST AP");
            _seriesDescription.Add("Port CXR  AP/LAT");
            _seriesDescription.Add("Porta Cath Injection -C");
            _seriesDescription.Add("Ribs 8-12 ap oblique");
            _seriesDescription.Add("Ribs 8-12 obl");
            _seriesDescription.Add("Routine Shoulder TRANS SCAPULA");
            _seriesDescription.Add("Routine Shoulder TRANS SCAPULA (0)");
            _seriesDescription.Add("Routine Shoulder trans scapular");
            _seriesDescription.Add("Routine Shoulder trans scapular (0)");
            _seriesDescription.Add("SHOULDER LEFT 3 VIEWS-UE");
            _seriesDescription.Add("Shoulder Left 3+ views -UE");
            _seriesDescription.Add("Standing foot lateral");
            _seriesDescription.Add("Standing Knee oblique");
            _seriesDescription.Add("Thoracic spine ap");
            _seriesDescription.Add("Thoracic spine lat");
            _seriesDescription.Add("Thoracic spine lat xtable");

            _studyDescription.Add("BILATERAL 2V SHOULDERS");
            _studyDescription.Add("BILATERAL ANKLES");
            _studyDescription.Add("BILATERAL ANKLES/FEET/RT ELBOW");
            _studyDescription.Add("BILATERAL CALCANEUS");
            _studyDescription.Add("BILATERAL CLAVICAL");
            _studyDescription.Add("BILATERAL FEET");
            _studyDescription.Add("BILATERAL FEMUR");
            _studyDescription.Add("BILATERAL FEMURS");
            _studyDescription.Add("Bilateral Fingers 3+ Views");
            _studyDescription.Add("BILATERAL FOREARMS");
            _studyDescription.Add("BILATERAL HANDS");
            _studyDescription.Add("BILATERAL HANDS AND WRISTS");
            _studyDescription.Add("BILATERAL HANDS PA");
            _studyDescription.Add("BILATERAL HANDS/WRISTS");
            _studyDescription.Add("BILATERAL HIP");
            _studyDescription.Add("BILATERAL HIPS");
             _studyDescription.Add("BILATERAL HIPS/KNEES");
            _studyDescription.Add("BILATERAL HIPS/PELVIS");
            _studyDescription.Add("BILATERAL HUMERUS");
            _studyDescription.Add("BILATERAL KNEE");
            _studyDescription.Add("BILATERAL KNEES");
            _studyDescription.Add("BILATERAL KNEES 3/4 VIEWS");
            _studyDescription.Add("BILATERAL KNEES 5+V");
            _studyDescription.Add("BILATERAL KNEES/RIGHT HIP");
            _studyDescription.Add("BILATERAL LEG LENGTH STUDY");
            _studyDescription.Add("BILATERAL MAMMOGRAM");
            _studyDescription.Add("BILATERAL ORBITS 01");
            _studyDescription.Add("BILATERAL ORBITS/FB/MRI");
            _studyDescription.Add("BILATERAL RIBS");
            _studyDescription.Add("BILATERAL SHOULDER");
            _studyDescription.Add("BILATERAL SHOULDERS");
            _studyDescription.Add("BILATERAL STANDING KNEES");
            _studyDescription.Add("BILATERAL TIB - FIB");
            _studyDescription.Add("BILATERAL TIB/FIB");
            _studyDescription.Add("BILATERAL TM- JOINTS");
            _studyDescription.Add("BILATERAL WRISTS");
            _studyDescription.Add("BILATERL HANDS/WRISTS");
            _studyDescription.Add("BILATRAL HANDS");
            _studyDescription.Add("BILIARY TUBE CHANGE -A");
            _studyDescription.Add("BILIARY TUBE CHANGE w/CONTRAST -A");
        }

    }

    public class DxSopGenerator : SopGenerator
    {
        public DxSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "DX";
            SopClassUid = SopClass.DigitalXRayImageStorageForPresentationUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 1;
            _seriesDescription.Add("Patella Sunrise Pa");
            _seriesDescription.Add("Pelvis");
            _seriesDescription.Add("PORT CHEST AP");
            _seriesDescription.Add("Port CXR  AP/LAT");
            _seriesDescription.Add("Porta Cath Injection -C");
            _seriesDescription.Add("Ribs 8-12 ap oblique");
            _seriesDescription.Add("Ribs 8-12 obl");
            _seriesDescription.Add("Routine Shoulder TRANS SCAPULA");
            _seriesDescription.Add("Routine Shoulder TRANS SCAPULA (0)");
            _seriesDescription.Add("Routine Shoulder trans scapular");
            _seriesDescription.Add("Routine Shoulder trans scapular (0)");
            _seriesDescription.Add("SHOULDER LEFT 3 VIEWS-UE");
            _seriesDescription.Add("Shoulder Left 3+ views -UE");
            _seriesDescription.Add("Standing foot lateral");
            _seriesDescription.Add("Standing Knee oblique");
            _seriesDescription.Add("Thoracic spine ap");
            _seriesDescription.Add("Thoracic spine lat");
            _seriesDescription.Add("Thoracic spine lat xtable");

            _studyDescription.Add("C- SPINE AND SHOULDER");
            _studyDescription.Add("C-Chest AP+Lat");
            _studyDescription.Add("C-Chest PA+Lat");
            _studyDescription.Add("Cervical Spine");
            _studyDescription.Add("CHEST - 1 VIEW");
            _studyDescription.Add("CHEST - (ROUTINE)");
            _studyDescription.Add("CHEST  (PRE-ADMIT)");
            _studyDescription.Add("Chest 2 views");
            _studyDescription.Add("Chest 2 Views - ROUTINE");
            _studyDescription.Add("Chest PA");
            _studyDescription.Add("Chest PA - CH");
            _studyDescription.Add("Chest PA & Lat");
            _studyDescription.Add("Chest routine plus AP inspiration");
            _studyDescription.Add("Chest routine plus AP inspiration Port");
            _studyDescription.Add("Chest routine plus PA");
            _studyDescription.Add("CHEST XRAY (2 V) -X091");
            _studyDescription.Add("CHEST ABD");
            _studyDescription.Add("CHEST ABD 3V'S");
            _studyDescription.Add("CHEST X-RAY");
            _studyDescription.Add("CHEST, ABDOMEN");
            _studyDescription.Add("CHEST, ANKLE");
            _studyDescription.Add("CHEST, HIPS");
            _studyDescription.Add("HIPS, FOOT, KNEES");
            _studyDescription.Add("HIPS PELVIS");
            _studyDescription.Add("HUMEROUS");
            _studyDescription.Add("HNAD RIGHT");
            _studyDescription.Add("HIPS+L-SPINE");
            _studyDescription.Add("L-SPINE + SI JTS");
            _studyDescription.Add("L-SPINE W/ OBLIQUES");
            _studyDescription.Add("L-SPINE T-SPINE");
            _studyDescription.Add("PELVIS+HIP");
            _studyDescription.Add("PELVIS+HIPS BILAT");
            _studyDescription.Add("PH CHEST 2 VIEWS");
            _studyDescription.Add("PELVIS/SI J");
            _studyDescription.Add("SACRUM AND COCCYX");
            _studyDescription.Add("SC JNTS");
            _studyDescription.Add("RT-WRIST/SCAPHOID");
            _studyDescription.Add("RT-SHOULDER");
            _studyDescription.Add("Teaching / Facial Bones");
            _studyDescription.Add("Thoracic + Lumbar Spine - SP");
            _studyDescription.Add("THORACIC / CERVICAL SPINE");

        }
    }

        public class UsSopGenerator : SopGenerator
    {

        public UsSopGenerator(SopGeneratorHospital hospital)
        {
            Modality = "US";
            SopClassUid = SopClass.UltrasoundImageStorageUid;
            _theFile = new DicomFile();
            ModalityHospital = hospital;
            MaxSeries = 2;
            _seriesDescription.Add("Head and Neck");
            _seriesDescription.Add("Head and Neck - Bilateral");
            _seriesDescription.Add("Head and Neck - Bilateral Port");
            _seriesDescription.Add("Head and Neck - Left");
            _seriesDescription.Add("Ultrasound Face/Neck");
            _seriesDescription.Add("Ultrasound Femur");
            _seriesDescription.Add("Ultrasound Foot");
            _seriesDescription.Add("Ultrasound Forearm");
            _seriesDescription.Add("ULTRASOUND GUIDED BIOPSY/ASPIR");
            _seriesDescription.Add("Ultrasound Hand");
            _seriesDescription.Add("Ultrasound Head and Neck");
            _seriesDescription.Add("Ultrasound Head and Neck Port");
            _seriesDescription.Add("Ultrasound Hip");
            _seriesDescription.Add("Ultrasound Hysterosonogram");
            _seriesDescription.Add("Ultrasound Knee");

            _studyDescription.Add("BIOPHYSICAL PRO");
            _studyDescription.Add("BIOPSY");
            _studyDescription.Add("Biopsy Abdomen");
            _studyDescription.Add("Biopsy Breast Left US Guidance");
            _studyDescription.Add("BIOPSY LIVER");
            _studyDescription.Add("Biopsy Prostate");
            _studyDescription.Add("BIOPSY THYROID - Q-SITE 02");
            _studyDescription.Add("BIOPSY THYROID");
            _studyDescription.Add("BIOPSY US GUIDED");
            _studyDescription.Add("Biopsy-breast-BR");
            _studyDescription.Add("Calf - Left");
            _studyDescription.Add("Calf - Right");
            _studyDescription.Add("Cardiac");
            _studyDescription.Add("CAROTID DOPPLER (Bilateral)");
            _studyDescription.Add("CEOU OB-BPP (SINGLETON)");
            _studyDescription.Add("Carotid/Vertebral Artery Doppler - Left");
            _studyDescription.Add("Chest - Right Port");
            _studyDescription.Add("Chest - Left Port");
            _studyDescription.Add("CHEST-US");
            _studyDescription.Add("CORE BIOPSY BREAST RT W/US-B");
            _studyDescription.Add("DOPP ASSESS OF TRANSPLANT");
            _studyDescription.Add("Dopp Ven Lower Ext Rt. -LE");
            _studyDescription.Add("DOPPLER ARTERIAL LOWER EXT-D");
            _studyDescription.Add("DOPPLER VEIN");
            _studyDescription.Add("DOPPLER VEINS LEFT");
            _studyDescription.Add("DOPPLER VEINS RIGHT");
            _studyDescription.Add("ECHO");
            _studyDescription.Add("ECHO ADULT COMPLETE");
            _studyDescription.Add("EXTREMITIES");
            _studyDescription.Add("Face/Neck/Thyroid Ultrasound");
            _studyDescription.Add("Fetal Assessment");
            _studyDescription.Add("Femur - Right");
            _studyDescription.Add("Forearm - Right");
            _studyDescription.Add("Head and Neck - Bilateral");
            _studyDescription.Add("Hip - Right");
            _studyDescription.Add("KIDNEYS-US");
            _studyDescription.Add("LIVER HCC ETOH INJ");
            _studyDescription.Add("LT BREAST U/S CORE BIOPSY");
            _studyDescription.Add("LYMPH NODE BX CERVICAL-US");
            _studyDescription.Add("Mammogram - Bilateral");
            _studyDescription.Add("Neck US");            
            _studyDescription.Add("Obstetrics/General");
            _studyDescription.Add("OBS LIMITED-US");
            _studyDescription.Add("OBS US");
            _studyDescription.Add("OBS 18WKS/LEVEL 2-OB");
            _studyDescription.Add("OBSTETRIC");
            _studyDescription.Add("P1 BOTH BREASTS");
            _studyDescription.Add("P2 SCROTUM ULTRASOUND");
            _studyDescription.Add("Pelvic Complete. -P");
            _studyDescription.Add("PERIFERAL VEIN DOPPLER LOWER");
            _studyDescription.Add("PORT CHEST U/S");
            _studyDescription.Add("PROSTATE-MALE PELVIS");
            _studyDescription.Add("RENAL/BLADDER/SCROTUM");
            _studyDescription.Add("SCROT");
        }
    }    
}
