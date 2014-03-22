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

namespace ClearCanvas.Dicom.Iod.Sequences
{
    public class ImageBoxesSequence:SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBoxesSequence"/> class.
        /// </summary>	
        public ImageBoxesSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBoxesSequence"/> class.
        /// </summary>
        public ImageBoxesSequence(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxNumber in the underlying collection. Type 1.
        /// </summary>
        public string ImageBoxNumber
        {
            get { return DicomAttributeProvider[DicomTags.ImageBoxNumber].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "ImageBoxNumber is Type 1 Required.");
                DicomAttributeProvider[DicomTags.ImageBoxNumber].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisplayEnvironmentSpatialPosition in the underlying collection. Type 1.
        /// </summary>
        public double DisplayEnvironmentSpatialPosition
        {
            get { return DicomAttributeProvider[DicomTags.DisplayEnvironmentSpatialPosition].GetFloat64(0, 0); }
            set
            {
                DicomAttributeProvider[DicomTags.DisplayEnvironmentSpatialPosition].SetFloat64(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxLayoutType in the underlying collection. Type 1.
        /// </summary>
        public ImageBoxLayoutType ImageBoxLayoutType
        {
			get { return ParseEnum<ImageBoxLayoutType>(base.DicomAttributeProvider[DicomTags.FilmOrientation].GetString(0, String.Empty), ImageBoxLayoutType.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ImageBoxLayoutType], value, false); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxTileHorizontalDimension in the underlying collection. Type 1C.
        /// </summary>
        public short ImageBoxTileHorizontalDimension
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageBoxTileHorizontalDimension].GetInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageBoxTileHorizontalDimension].SetInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxScrollDirection in the underlying collection. Type 1C.
        /// </summary>
        public ImageBoxScrollDirection ImageBoxScrollDirection
        {
            get { return ParseEnum<ImageBoxScrollDirection>(base.DicomAttributeProvider[DicomTags.ImageBoxScrollDirection].GetString(0, String.Empty), ImageBoxScrollDirection.None); }
            set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ImageBoxScrollDirection], value, false); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxSmallScrollType in the underlying collection. Type 2C.
        /// </summary>
        public ImageBoxSmallScrollType ImageBoxSmallScrollType
        {
            get { return ParseEnum<ImageBoxSmallScrollType>(base.DicomAttributeProvider[DicomTags.ImageBoxSmallScrollType].GetString(0, String.Empty), ImageBoxSmallScrollType.None); }
            set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ImageBoxSmallScrollType], value, false); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxSmallScrollAmount in the underlying collection. Type 1C.
        /// </summary>
        public short ImageBoxSmallScrollAmount
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageBoxSmallScrollAmount].GetInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageBoxSmallScrollAmount].SetInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxLargeScrollType in the underlying collection. Type 2C.
        /// </summary>
        public string ImageBoxLargeScrollType
        {
            get { return DicomAttributeProvider[DicomTags.ImageBoxLargeScrollType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ImageBoxLargeScrollType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxLargeScrollAmount in the underlying collection. Type 1C.
        /// </summary>
        public short ImageBoxLargeScrollAmount
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageBoxLargeScrollAmount].GetInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageBoxLargeScrollAmount].SetInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxOverlapPriority in the underlying collection. Type 3.
        /// </summary>
        public ushort ImageBoxOverlapPriority
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageBoxOverlapPriority].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageBoxOverlapPriority].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of PreferredPlaybackSequencing in the underlying collection. Type 1C.
        /// </summary>
        public ushort PreferredPlaybackSequencing
        {
            get { return base.DicomAttributeProvider[DicomTags.PreferredPlaybackSequencing].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.PreferredPlaybackSequencing].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of RecommendedDisplayFrameRate in the underlying collection. Type 1C.
        /// </summary>
        public string RecommendedDisplayFrameRate
        {
            get { return DicomAttributeProvider[DicomTags.RecommendedDisplayFrameRate].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.RecommendedDisplayFrameRate].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of CineRelativetoRealTime in the underlying collection. Type 1C.
        /// </summary>
        public double CineRelativetoRealTime
        {
            get { return DicomAttributeProvider[DicomTags.CineRelativeToRealTime].GetFloat64(0, 0); }
            set
            {
                DicomAttributeProvider[DicomTags.CineRelativeToRealTime].SetFloat64(0, value);
            }
        }
   }


    public enum ImageBoxLayoutType
    {
        TILED,
        STACK,
        CINE,
        PROCESSED,
        None,
    }

    public enum ImageBoxScrollDirection
    {
        VERTICAL,
        HORIZONTAL,
        None,
    }
    
    public enum ImageBoxSmallScrollType
    {
        PAGE,
        ROW_COLUMN,
        IMAGE,
        None,
    }
}
