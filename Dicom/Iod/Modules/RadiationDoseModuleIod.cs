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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom DOC 3 Table C.4-16
    /// </summary>
    public class RadiationDoseModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RadiationDoseModuleIod"/> class.
        /// </summary>
        public RadiationDoseModuleIod()
            :base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RadiationDoseModuleIod"/> class.
        /// </summary>
		public RadiationDoseModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Anatomic structure, space or region that has been exposed to ionizing radiation. 
        /// The sequence may have zero or one Items.
        /// </summary>
        /// <value>The anatomic structure space or region sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> AnatomicStructureSpaceOrRegionSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.AnatomicStructureSpaceOrRegionSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Total duration of X-Ray exposure during fluoroscopy in seconds (pedal time) during this Performed Procedure Step.
        /// </summary>
        /// <value>The total time of fluoroscopy.</value>
        public ushort TotalTimeOfFluoroscopy
        {
            get { return base.DicomAttributeProvider[DicomTags.TotalTimeOfFluoroscopy].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.TotalTimeOfFluoroscopy].SetUInt16(0, value); }
        }

        /// <summary>
        /// Total number of exposures made during this Performed Procedure Step. 
        /// The number includes non-digital and digital exposures.
        /// </summary>
        /// <value>The total number of exposures.</value>
        public ushort TotalNumberOfExposures
        {
            get { return base.DicomAttributeProvider[DicomTags.TotalNumberOfExposures].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.TotalNumberOfExposures].SetUInt16(0, value); }
        }

        /// <summary>
        /// Distance in mm from the source to detector center. 
        /// <para>Note: This value is traditionally referred to as Source Image Receptor Distance (SID).</para>
        /// </summary>
        /// <value>The distance source to detector.</value>
        public float DistanceSourceToDetector
        {
            get { return base.DicomAttributeProvider[DicomTags.DistanceSourceToDetector].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.DistanceSourceToDetector].SetFloat32(0, value); }
        }

        /// <summary>
        /// Distance in mm from the source to the surface of the patient closest to the source during this Performed Procedure Step.
        /// Note: This may be an estimated value based on assumptions about the patient�s body size and habitus.
        /// </summary>
        /// <value>The distance source to entrance.</value>
        public float DistanceSourceToEntrance
        {
            get { return base.DicomAttributeProvider[DicomTags.DistanceSourceToEntrance].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.DistanceSourceToEntrance].SetFloat32(0, value); }
        }

        /// <summary>
        /// Average entrance dose value measured in dGy at the surface of the patient during this Performed Procedure Step.
        /// Note: This may be an estimated value based on assumptions about the patient�s body size and habitus.
        /// </summary>
        /// <value>The entrance dose.</value>
        public ushort EntranceDose
        {
            get { return base.DicomAttributeProvider[DicomTags.EntranceDose].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.EntranceDose].SetUInt16(0, value); }
        }

        /// <summary>
        /// Average entrance dose value measured in mGy at the surface of the patient during this Performed Procedure Step.
        /// Note: This may be an estimated value based on assumptions about the patient�s body size and habitus.
        /// </summary>
        /// <value>The entrance dose in mgy.</value>
        public float EntranceDoseInMgy
        {
            get { return base.DicomAttributeProvider[DicomTags.EntranceDoseInMgy].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.EntranceDoseInMgy].SetFloat32(0, value); }
        }

        /// <summary>
        /// Typical dimension of the exposed area at the detector plane. If Rectangular: ExposeArea1 is row dimension followed by column (ExposeArea2); if Round: ExposeArea1 is diameter. Measured in mm.
        /// </summary>
        /// <value>The exposed area1.</value>
        public float ExposedArea1
        {
            get { return base.DicomAttributeProvider[DicomTags.ExposedArea].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.ExposedArea].SetFloat32(0, value); }
        }

        /// <summary>
        /// Typical dimension of the exposed area at the detector plane. If Rectangular: ExposeArea2 is column dimension (ExposeArea1 is column); if Round: ExposeArea2 is Null...
        /// </summary>
        /// <value>The exposed area2.</value>
        public float ExposedArea2
        {
            get { return base.DicomAttributeProvider[DicomTags.ExposedArea].GetFloat32(1, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.ExposedArea].SetFloat32(1, value); }
        }

        /// <summary>
        /// Total area-dose-product to which the patient was exposed, accumulated over the complete Performed
        /// Procedure Step and measured in dGy*cm*cm, including fluoroscopy.
        /// <para>Notes: 1. The sum of the area dose product of all images of a Series or a Study may not result in
        /// the total area dose product to which the patient was exposed.</para>
        /// <para>2. This may be an estimated value based on assumptions about the patient�s body size and habitus.</para>
        /// </summary>
        /// <value>The image and fluoroscopy area dose product.</value>
        public float ImageAndFluoroscopyAreaDoseProduct
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageAndFluoroscopyAreaDoseProduct].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.ImageAndFluoroscopyAreaDoseProduct].SetFloat32(0, value); }
        }

        /// <summary>
        /// User-defined comments on any special conditions related to radiation dose encountered during this Performed Procedure Step.
        /// </summary>
        /// <value>The comments on radiation dose.</value>
        public string CommentsOnRadiationDose
        {
            get { return base.DicomAttributeProvider[DicomTags.CommentsOnRadiationDose].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.CommentsOnRadiationDose].SetString(0, value); }
        }

        /// <summary>
        /// Exposure Dose Sequence will contain Total Number of Exposures (0040,0301) items plus an item for
        /// each fluoroscopy episode not already counted as an exposure.
        /// </summary>
        /// <value>The exposure dose sequence list.</value>
        public SequenceIodList<ExposureDoseSequenceIod> ExposureDoseSequenceList
        {
            get
            {
                return new SequenceIodList<ExposureDoseSequenceIod>(base.DicomAttributeProvider[DicomTags.ExposureDoseSequence] as DicomAttributeSQ);
            }
        }
        
        
        #endregion

    }
}
