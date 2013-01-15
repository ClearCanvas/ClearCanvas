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
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Scheduled Procedure Step Sequence
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table C.13-5 (pg 871)</remarks>
    public class BasicGrayscaleImageSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicGrayscaleImageSequenceIod"/> class.
        /// </summary>
        public BasicGrayscaleImageSequenceIod()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicGrayscaleImageSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public BasicGrayscaleImageSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the samples per pixel.  Number of samples (planes) in this image.
        /// <para>Possible values for Basic Grayscale Sequence Iod is 1.</para>
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
        /// <para>Possible values for Basic Grayscale SequenceIod are MONOCHOME1 or MONOCHROME2.</para>
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
        /// <para>Possible values for Bits Allocated are 8 (if Bits Stored = 8) or 16 (if Bits Stored = 12).</para>
        /// </summary>
        /// <value>The bits allocated.</value>
        public ushort BitsAllocated
        {
            get { return base.DicomAttributeProvider[DicomTags.BitsAllocated].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.BitsAllocated].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the bits stored.
        /// <para>Possible values for Bits Stored are 8 or 12.</para>
        /// </summary>
        /// <value>The bits stored.</value>
        public ushort BitsStored
        {
            get { return base.DicomAttributeProvider[DicomTags.BitsStored].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.BitsStored].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the high bit.
        /// <para>Possible values for High Bit are 7 (if Bits Stored = 8) or 11 (if Bits Stored = 12).</para>
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
        /// <para>Possible values for Basic Grayscale Sequence Iod is 0 (000H).</para>
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

        /// <summary>
        /// Adds the dicom file values.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="FileNotFoundException"/>
        public void AddDicomFileValues(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            DicomFile dicomFile = new DicomFile(filePath);
            dicomFile.Load();
            AddDicomFileValues(dicomFile.DataSet);
        }


        /// <summary>
        /// Adds the attribute values for the specified <see cref="dicomFile"/>.  Tags it sets are:
        /// ImageType, SopClassUid, SopInstanceUid, StudyInstanceUid, SamplesPerPixel, PhotometricInterpretation,NumberOfFrames,
        /// Rows, Columns, BitsAllocated,BitsStored, HighBit,  PixelRepresentation, SmallestImagePixelValue, LargestImagePixelValue,
        /// WindowCenter, WindowWidth, PixelData.
        /// </summary>
        public void AddDicomFileValues(DicomFile dicomFile)
        {
            if (dicomFile == null)
                throw new ArgumentNullException("dicomFile");
            AddDicomFileValues(dicomFile.DataSet);
        }

        public void AddDicomFileValues(IDicomAttributeProvider dicomAttributes)
        {
			uint[] dicomTags = new uint[]
                {
                    DicomTags.ImageType,
                    DicomTags.SopClassUid,
                    DicomTags.SopInstanceUid,
                    DicomTags.StudyInstanceUid,
                    DicomTags.SamplesPerPixel,
                    DicomTags.PhotometricInterpretation,
                    DicomTags.NumberOfFrames,
                    DicomTags.Rows,
                    DicomTags.Columns,
                    DicomTags.BitsAllocated,
                    DicomTags.BitsStored,
                    DicomTags.HighBit,
                    DicomTags.PixelRepresentation,
                    DicomTags.SmallestImagePixelValue,
                    DicomTags.LargestImagePixelValue,
                    DicomTags.WindowCenter,
                    DicomTags.WindowWidth,
                    DicomTags.PixelData
                };

            foreach (uint dicomTag in dicomTags)
            {
                try
                {

                    DicomAttribute dicomAttribute;
                    if (dicomAttributes.TryGetAttribute(dicomTag, out dicomAttribute))
                        DicomAttributeProvider[dicomTag].Values = dicomAttribute.Values;
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Exception adding dicom tag value for uint: " + dicomTag);
                    throw;
                }
            }
        }
    }
}
