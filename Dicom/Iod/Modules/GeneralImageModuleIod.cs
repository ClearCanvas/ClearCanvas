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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// General Image Module as per Part 3 Table C.7-9 page 293
    /// </summary>
    public class GeneralImageModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralImageModuleIod"/> class.
        /// </summary>
        public GeneralImageModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralImageModuleIod"/> class.
        /// </summary>
		public GeneralImageModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the instance number.
        /// </summary>
        /// <value>The instance number.</value>
        public int InstanceNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.InstanceNumber].GetInt32(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the patient orientation.  TODO: make it easier to specify values
        /// </summary>
        /// <value>The patient orientation.</value>
        public string PatientOrientation
        {
            get { return base.DicomAttributeProvider[DicomTags.PatientOrientation].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PatientOrientation].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the content date.
        /// </summary>
        /// <value>The content date.</value>
        public DateTime? ContentDate
        {
            get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeProvider, 0, DicomTags.ContentDate, DicomTags.ContentTime); }
            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider, 0, DicomTags.ContentDate, DicomTags.ContentTime); }
        }

        /// <summary>
        /// Gets or sets the type of the image.  TODO: make it easier to specify values
        /// </summary>
        /// <value>The type of the image.</value>
        public string ImageType
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageType].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ImageType].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the acquisition number.
        /// </summary>
        /// <value>The acquisition number.</value>
        public int AcquisitionNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.AcquisitionNumber].GetInt32(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.AcquisitionNumber].SetInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the acquisition date.  Checks both the AcquisitionDatetime tag and the AcquisitionDate/AcquisitionTime tags.
        /// </summary>
        /// <value>The acquisition date.</value>
        public DateTime? AcquisitionDate
        {
            get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeProvider, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate, DicomTags.AcquisitionTime);  }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate, DicomTags.AcquisitionTime); }
        }

        /// <summary>
        /// A sequence that references other images significantly related to this image (e.g. post-localizer CT image or
        /// Mammographic biopsy or partial view images). One or more Items may be included in this sequence.
        /// </summary>
        /// <value>The referenced image box sequence list.</value>
        public SequenceIodList<ImageSopInstanceReferenceMacro> ReferencedImageBoxSequenceList
        {
            get
            {
                return new SequenceIodList<ImageSopInstanceReferenceMacro>(base.DicomAttributeProvider[DicomTags.ReferencedImageBoxSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Describes the purpose for which the reference is made. Only a single Item shall be permitted in this sequence.
        /// </summary>
        /// <value>The purpose of reference code sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> PurposeOfReferenceCodeSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.PurposeOfReferenceCodeSequence] as DicomAttributeSQ);
            }
        }
        
        
        /// <summary>
        /// Gets or sets the derivation description.
        /// </summary>
        /// <value>The derivation description.</value>
        public string DerivationDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.DerivationDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.DerivationDescription].SetString(0, value); }
        }

        /// <summary>
        /// A coded description of how this image was derived. See C.7.6.1.1.3 for further explanation. One or more Items may be included in this Sequence. 
        /// More than one Item indicates that successive derivation steps have been applied.
        /// </summary>
        /// <value>The derivation code sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> DerivationCodeSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.DerivationCodeSequence] as DicomAttributeSQ);
            }
        }

        //TODO: SourceImageSequence


        /// <summary>
        /// A sequence which provides reference to a set of non-image SOP Class/Instance pairs significantly related to this Image,
        /// including waveforms that may or may not be temporally synchronized with this image . One or more Items may be included in
        /// this sequence.
        /// </summary>
        /// <value>The referenced instance sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedInstanceSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedInstanceSequence] as DicomAttributeSQ);
            }
        }
        
        /// <summary>
        /// Gets or sets the images in acquisition.
        /// </summary>
        /// <value>The images in acquisition.</value>
        public int ImagesInAcquisition
        {
            get { return base.DicomAttributeProvider[DicomTags.ImagesInAcquisition].GetInt32(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImagesInAcquisition].SetInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the image comments.
        /// </summary>
        /// <value>The image comments.</value>
        public string ImageComments
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageComments].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ImageComments].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the quality control image.
        /// </summary>
        /// <value>The quality control image.</value>
        public DicomBoolean QualityControlImage
        {
            get { return IodBase.ParseEnum<DicomBoolean>(base.DicomAttributeProvider[DicomTags.QualityControlImage].GetString(0, String.Empty), DicomBoolean.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.QualityControlImage], value, false); }
        }

        /// <summary>
        /// Gets or sets the burned in annotation.
        /// </summary>
        /// <value>The burned in annotation.</value>
        public DicomBoolean BurnedInAnnotation
        {
            get { return IodBase.ParseEnum<DicomBoolean>(base.DicomAttributeProvider[DicomTags.BurnedInAnnotation].GetString(0, String.Empty), DicomBoolean.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.BurnedInAnnotation], value, false); }
        }

        /// <summary>
        /// Gets or sets the lossy image compression.
        /// <para>00 = Image has NOT been subjected to lossy compression.</para>
        /// 	<para>01 = Image has been subjected to lossy compression.</para>
        /// </summary>
        /// <value>The lossy image compression.</value>
        public string LossyImageCompression
        {
            get { return base.DicomAttributeProvider[DicomTags.LossyImageCompression].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.LossyImageCompression].SetString(0, value); }
        }

        public float LossyImageCompressionRatio
        {
            get { return base.DicomAttributeProvider[DicomTags.LossyImageCompressionRatio].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.LossyImageCompressionRatio].SetFloat32(0, value); }
        }

        public string LossyImageCompressionMethod
        {
            get { return base.DicomAttributeProvider[DicomTags.LossyImageCompressionMethod].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.LossyImageCompressionMethod].SetString(0, value); }
        }

        //TODO: Icon Image Sequence

        public PresentationLutShape PresentationLutShape
        {
            get { return IodBase.ParseEnum<PresentationLutShape>(base.DicomAttributeProvider[DicomTags.PresentationLutShape].GetString(0, String.Empty), PresentationLutShape.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PresentationLutShape], value, false); }
        }

        public string IrradiationEventUid
        {
            get { return base.DicomAttributeProvider[DicomTags.IrradiationEventUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.IrradiationEventUid].SetString(0, value); }
        }
        
        
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the commonly used tags in the base dicom attribute collection.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(base.DicomAttributeProvider);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the commonly used tags in the specified dicom attribute collection.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
            if (dicomAttributeProvider == null)
				throw new ArgumentNullException("dicomAttributeProvider");

            //dicomAttributeProvider[DicomTags.NumberOfCopies].SetNullValue();
            //dicomAttributeProvider[DicomTags.PrintPriority].SetNullValue();
            //dicomAttributeProvider[DicomTags.MediumType].SetNullValue();
            //dicomAttributeProvider[DicomTags.FilmDestination].SetNullValue();
            //dicomAttributeProvider[DicomTags.FilmSessionLabel].SetNullValue();
            //dicomAttributeProvider[DicomTags.MemoryAllocation].SetNullValue();
            //dicomAttributeProvider[DicomTags.OwnerId].SetNullValue();
        }





        #endregion
    }

    #region PresentationLutShape Enum
    /// <summary>
    /// When present, specifies an identity transformation for the Presentation LUT such that the 
    /// output of all grayscale transformations, if any, are defined to be in P-Values.
    /// <para>
    /// When this attribute is used with a color photometric interpretation then the
    /// luminance component is in P-Values.</para>
    /// </summary>
    public enum PresentationLutShape
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// output is in P-Values - shall be used if Photometric Interpretation 
        /// (0028,0004) is MONOCHROME2 or any color photometric interpretation.
        /// </summary>
        Identity,
        /// <summary>
        /// output after inversion is in PValues - shall be used if Photometric 
        /// Interpretation (0028,0004) is MONOCHROME1.
        /// </summary>
        Inverse
    }
    #endregion
 
}

