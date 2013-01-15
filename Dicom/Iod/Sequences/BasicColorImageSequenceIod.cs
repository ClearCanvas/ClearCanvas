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
    /// <summary>
    /// Basic Color Image Sequence
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table C.13-5 (pg 871)</remarks>
    public class BasicColorImageSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicColorImageSequenceIod"/> class.
        /// </summary>
        public BasicColorImageSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicColorImageSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public BasicColorImageSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the samples per pixel.  Number of samples (planes) in this image.
        /// <para>Possible values for Basic Color Sequence Iod is 3.</para>
        /// </summary>
        /// <value>The samples per pixel.</value>
        /// <remarks>See Part 3, C.7.6.3.1.1 for more info.</remarks>
        public ushort SamplesPerPixel
        {
            get { return base.DicomAttributeProvider[DicomTags.SamplesPerPixel].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.SamplesPerPixel].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the photometric interpretation.
        /// <para>Possible values for Basic Grayscale SequenceIod are RGB.</para>
        /// </summary>
        /// <value>The photometric interpretation.</value>
        public PhotometricInterpretation PhotometricInterpretation
        {
            get { return PhotometricInterpretation.FromCodeString(base.DicomAttributeProvider[DicomTags.PhotometricInterpretation].GetString(0, String.Empty)); }
            set
            {
				if (value == null)
					base.DicomAttributeProvider[DicomTags.PhotometricInterpretation] = null;
				else
					base.DicomAttributeProvider[DicomTags.PhotometricInterpretation].SetStringValue(value.Code);
            }
        }

        /// <summary>
        /// Gets or sets the planar configuration.
        /// <para>Possible value for Basic Grayscale SequenceIod is 1 (frame interleave).</para>
        /// </summary>
        /// <value>The planar configuration.</value>
        public ushort PlanarConfiguration
        {
            get { return base.DicomAttributeProvider[DicomTags.PlanarConfiguration].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.PlanarConfiguration].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ushort Rows
        {
            get { return base.DicomAttributeProvider[DicomTags.Rows].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.Rows].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ushort Columns
        {
            get { return base.DicomAttributeProvider[DicomTags.Columns].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.Columns].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the pixel aspect ratio.
        /// </summary>
        /// <value>The pixel aspect ratio.</value>
		public PixelAspectRatio PixelAspectRatio
        {
			get { return PixelAspectRatio.FromString(base.DicomAttributeProvider[DicomTags.PixelAspectRatio].ToString()); }
			set
			{
				if (value == null || value.IsNull)
					base.DicomAttributeProvider[DicomTags.PixelAspectRatio].SetNullValue();
				else
					base.DicomAttributeProvider[DicomTags.PixelAspectRatio].SetStringValue(value.ToString());
			}
		}

        /// <summary>
        /// Gets or sets the bits allocated.
        /// <para>Possible values for Basic Color Sequence Iod is 8.</para>
        /// </summary>
        /// <value>The bits allocated.</value>
        public ushort BitsAllocated
        {
            get { return base.DicomAttributeProvider[DicomTags.BitsAllocated].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.BitsAllocated].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the bits stored.
        /// <para>Possible values for Basic Color Sequence Iod is 8.</para>
        /// </summary>
        /// <value>The bits stored.</value>
        public ushort BitsStored
        {
            get { return base.DicomAttributeProvider[DicomTags.BitsStored].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.BitsStored].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the high bit.
        /// <para>Possible values for Basic Color Sequence Iod is 7.</para>
        /// </summary>
        /// <value>The high bit.</value>
        public ushort HighBit
        {
            get { return base.DicomAttributeProvider[DicomTags.HighBit].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.HighBit].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the pixel representation.Data representation of the pixel samples. 
        /// Each sample shall have the same pixel representation. 
        /// <para>Possible values for Basic Color Sequence Iod is 0 (000H).</para>
        /// </summary>
        /// <value>The pixel representation.</value>
		public ushort PixelRepresentation
		{
			get { return base.DicomAttributeProvider[DicomTags.PixelRepresentation].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.PixelRepresentation].SetUInt16(0, value); }
		}

        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>The pixel data.</value>
        public byte[] PixelData
        {
            get
            {
            	DicomAttribute attribute = base.DicomAttributeProvider[DicomTags.PixelData];
				if (!attribute.IsNull && !attribute.IsEmpty)
                    return (byte[])attribute.Values;
                else
                    return null;
            }
            set { base.DicomAttributeProvider[DicomTags.PixelData].Values = value; }
        }

        #endregion

    }


}
