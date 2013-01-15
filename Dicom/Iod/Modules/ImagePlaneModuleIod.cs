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

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Image Plane Module as per Part 3 Table C.7-10 page 301
    /// </summary>
    public class ImagePlaneModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePlaneModuleIod"/> class.
        /// </summary>
        public ImagePlaneModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePlaneModuleIod"/> class.
        /// </summary>
		public ImagePlaneModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the pixel spacing row (1st value in PixelSpacing tag).
        /// <para>In mm, that is the spacing between the centers of adjacent rows, or vertical spacing.
        /// </para>
        /// </summary>
        /// <value>The pixel spacing row.</value>
        /// <remarks>See Part 3, 10.7.1.3 for more info</remarks>
        public float PixelSpacingRow
        {
            get { return base.DicomAttributeProvider[DicomTags.PixelSpacing].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat32(0, value); }
        }

        /// <summary>
        /// Gets or sets the pixel spacing column (2nd value in PixelSpacing tag).
        /// <para>in mm, that is the spacing between the centers of adjacent columns, 
        /// or horizontal spacing.</para>
        /// </summary>
        /// <value>The pixel spacing column.</value>
        public float PixelSpacingColumn
        {
            get { return base.DicomAttributeProvider[DicomTags.PixelSpacing].GetFloat32(1, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat32(1, value); }
        }

        /// <summary>
        /// Gets the image orientation patient.
        /// <para>
        /// Image Orientation (0020,0037) specifies the direction cosines of the first row and the 
        /// first column with respect to the patient. These Attributes shall be provide as a pair. 
        /// Row value for the x, y, and z axes respectively followed by the Column value for the x, y, 
        /// and z axes respectively.</para>
        /// </summary>
        /// <value>The image orientation patient.</value>
        /// <remarks>See Part 3, C7.6.2.1.1 for more info</remarks>
        public DicomAttribute ImageOrientationPatient
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageOrientationPatient]; }
        }

        /// <summary>
        /// Gets the image position patient.
        /// <para>The Image Position (0020,0032) specifies the x, y, and z coordinates of the upper 
        /// left hand corner of the image; it is the center of the first voxel transmitted. 
        /// </para>
        /// </summary>
        /// <value>The image position patient.</value>
        /// <remarks>See Part 3, C7.6.2.1.1 for more info</remarks>
        public DicomAttribute ImagePositionPatient
        {
            get { return base.DicomAttributeProvider[DicomTags.ImagePositionPatient]; }
        }

        /// <summary>
        /// Gets or sets the slice thickness, in mm.
        /// </summary>
        /// <value>The slice thickness.</value>
        public float SliceThickness
        {
            get { return base.DicomAttributeProvider[DicomTags.SliceThickness].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.SliceThickness].SetFloat32(0, value); }
        }

        /// <summary>
        /// Gets or sets the slice location.  Relative position of exposure expressed in mm. 
        /// </summary>
        /// <value>The slice location.</value>
        /// <remarks>See part 3, C.7.6.2.1.2 for further explanation.</remarks>
        public float SliceLocation
        {
            get { return base.DicomAttributeProvider[DicomTags.SliceLocation].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.SliceLocation].SetFloat32(0, value); }
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

    
}

