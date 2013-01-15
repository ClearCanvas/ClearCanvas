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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Image Box Pixel Module as per Part 3 Table C.13-5 page 868
    /// </summary>
    public class ImageBoxPixelModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBoxPixelModuleIod"/> class.
        /// </summary>
        public ImageBoxPixelModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBoxPixelModuleIod"/> class.
        /// </summary>
		public ImageBoxPixelModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the image box position.  The position of the image on the film, based on 
        /// Image Display Format (2010,0010). See Part 3, C.13.5.1 for specification.
        /// </summary>
        /// <value>The image box position.</value>
        public ushort ImageBoxPosition
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageBoxPosition].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageBoxPosition].SetUInt16(0, value); }
        }
        /// <summary>
        /// Gets or sets the polarity. Specifies whether minimum pixel values (after VOI 3LUT transformation) are to printed black or white.
        /// If Polarity (2020,0020) is not specified by the SCU, the SCP shall print with NORMAL polarity.
        /// </summary>
        /// <value>The polarity.</value>
        public Polarity Polarity
        {
            get { return IodBase.ParseEnum<Polarity>(base.DicomAttributeProvider[DicomTags.Polarity].GetString(0, String.Empty), Polarity.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Polarity], value, false); }
        }

        /// <summary>
        /// Gets or sets the type of the magnification.
        /// </summary>
        /// <value>The type of the magnification.</value>
        public MagnificationType MagnificationType
        {
            get { return IodBase.ParseEnum<MagnificationType>(base.DicomAttributeProvider[DicomTags.MagnificationType].GetString(0, String.Empty), MagnificationType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MagnificationType], value, false); }
        }

        /// <summary>
        /// Gets or sets the type of the smoothing.
        /// </summary>
        /// <value>The type of the smoothing.</value>
        public SmoothingType SmoothingType
        {
            get { return IodBase.ParseEnum<SmoothingType>(base.DicomAttributeProvider[DicomTags.SmoothingType].GetString(0, String.Empty), SmoothingType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.SmoothingType], value, false); }
        }

        /// <summary>
        /// Gets or sets the configuration information.
        /// </summary>
        /// <value>The configuration information.</value>
        public string ConfigurationInformation
        {
            get { return base.DicomAttributeProvider[DicomTags.ConfigurationInformation].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ConfigurationInformation].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the size of the requested image.  Width (x-dimension) in mm of the image to be
        /// printed. This value overrides the size that corresponds with optimal filling of the Image Box.
        /// </summary>
        /// <value>The size of the requested image.</value>
        public float RequestedImageSize
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedImageSize].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.RequestedImageSize].SetFloat32(0, value); }
        }

        /// <summary>
        /// Gets or sets the requested decimate crop behavior.
        /// </summary>
        /// <value>The requested decimate crop behavior.</value>
        public DecimateCropBehavior RequestedDecimateCropBehavior
        {
            get { return IodBase.ParseEnum<DecimateCropBehavior>(base.DicomAttributeProvider[DicomTags.RequestedDecimateCropBehavior].GetString(0, String.Empty), DecimateCropBehavior.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.RequestedDecimateCropBehavior], value, false); }
        }


        /// <summary>
        /// Gets the basic grayscale image sequence list.
        /// </summary>
        /// <value>The basic grayscale image sequence list.</value>
        public SequenceIodList<BasicGrayscaleImageSequenceIod> BasicGrayscaleImageSequenceList
        {
            get
            {
                return new SequenceIodList<BasicGrayscaleImageSequenceIod>(base.DicomAttributeProvider[DicomTags.BasicGrayscaleImageSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets the basic color image sequence list.
        /// </summary>
        /// <value>The basic color image sequence list.</value>
        public SequenceIodList<BasicColorImageSequenceIod> BasicColorImageSequenceList
        {
            get
            {
                return new SequenceIodList<BasicColorImageSequenceIod>(base.DicomAttributeProvider[DicomTags.BasicColorImageSequence] as DicomAttributeSQ);
            }
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

    #region ImagePosition Enum
    /// <summary>
    /// Enumeration for Image Position. The position of the image on the film, based on Image Display Format (2010,0010).
    /// </summary>
    public enum ImagePosition
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// image box sequence shall be major row order (from left-toright and from top-to-bottom); top left image position shall be equal to 1.
        /// </summary>
        Standard,
        /// <summary>
        /// Image box sequence shall be major row order (from left-to-right and from top-to-bottom); top left image position shall be set to 1.
        /// </summary>
        Row,
        /// <summary>
        /// image box sequence shall be major column order (from top-tobottom and from left-to-right); top left image position shall be equal to 1.
        /// </summary>
        Col,
        /// <summary>
        /// image box sequence shall be major row order (from left-to-right and from top-to-bottom); top left image position shall be set to 1.
        /// </summary>
        Slide,
        /// <summary>
        /// image box sequence shall be major row order (from left-toright and from top-to-bottom); top left image position shall be set to 1.
        /// </summary>
        Superslide,
        /// <summary>
        /// image box sequence shall be defined in the Conformance Statement; top left image position shall be set to 1.
        /// </summary>
        CustomStandard
    }
    #endregion

    #region Polarity Enum
    /// <summary>
    /// Specifies whether minimum pixel values (after VOI 3LUT transformation) are to printed black or white.
    /// If Polarity (2020,0020) is not specified by the SCU, the SCP shall print with NORMAL polarity.
    /// </summary>
    public enum Polarity
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// pixels shall be printed as specified by the Photometric Interpretation (0028,0004)
        /// </summary>
        Normal,
        /// <summary>
        /// pixels shall be printed with the opposite polarity as specified by the Photometric Interpretation (0028,0004)
        /// </summary>
        Reverse
    }
    #endregion

    #region DecimateCropBehavior Enum
    /// <summary>
    /// 
    /// </summary>
    public enum DecimateCropBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// the SCP shall not crop or decimate
        /// </summary>
        Fail,
        /// <summary>
        /// a magnification factor less than 1 to be applied to the image.
        /// </summary>
        Decimate,
        /// <summary>
        /// some image rows and/or columns are to be deleted before printing. The specific algorithm 
        /// for cropping shall be described in the SCP Conformance Statement.
        /// </summary>
        Crop
    }
    #endregion
    
}

