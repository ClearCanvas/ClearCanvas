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

namespace ClearCanvas.Dicom.Iod.Macros
{
    public class ScreenSpecificationsMacro : SequenceIodBase
    {
        #region Constructors

		/// <summary>
        /// Initializes a new instance of the <see cref="ScreenSpecificationsMacro"/> class.
		/// </summary>
		public ScreenSpecificationsMacro()
		{}

		/// <summary>
        /// Initializes a new instance of the <see cref="ScreenSpecificationsMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ScreenSpecificationsMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

		#endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the NumberOfVerticalPixels.Type 1.
        /// </summary>
        /// <value>The NumberOfVerticalPixels.</value>
        public ushort NumberOfVerticalPixels
        {
            get { return base.DicomAttributeProvider[DicomTags.NumberOfVerticalPixels].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.NumberOfVerticalPixels].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the NumberOfVerticalPixels.Type 1.
        /// </summary>
        /// <value>The NumberOfVerticalPixels.</value>
        public ushort NumberOfHorizontalPixels
        {
            get { return base.DicomAttributeProvider[DicomTags.NumberOfHorizontalPixels].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.NumberOfHorizontalPixels].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the DisplayEnvironmentSpatialPosition.Type 1.
        /// </summary>
        /// <value>The DisplayEnvironmentSpatialPosition.</value>
        public double DisplayEnvironmentSpatialPosition
        {
            get { return base.DicomAttributeProvider[DicomTags.DisplayEnvironmentSpatialPosition].GetFloat64(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.DisplayEnvironmentSpatialPosition].SetFloat64(0, value); }
        }

        /// <summary>
        /// Gets or sets the ScreenMinimumGrayscaleBitDepth.Type 1C.
        /// </summary>
        /// <value>The ScreenMinimumGrayscaleBitDepth.</value>
        public ushort ScreenMinimumGrayscaleBitDepth
        {
            get { return base.DicomAttributeProvider[DicomTags.ScreenMinimumGrayscaleBitDepth].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ScreenMinimumGrayscaleBitDepth].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the ScreenMinimumColorBitDepth.Type 1C.
        /// </summary>
        /// <value>The ScreenMinimumColorBitDepth.</value>
        public ushort ScreenMinimumColorBitDepth
        {
            get { return base.DicomAttributeProvider[DicomTags.ScreenMinimumColorBitDepth].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ScreenMinimumColorBitDepth].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the ApplicationMaximumRepaintTime.Type 3.
        /// </summary>
        /// <value>The ApplicationMaximumRepaintTime.</value>
        public ushort ApplicationMaximumRepaintTime
        {
            get { return base.DicomAttributeProvider[DicomTags.ApplicationMaximumRepaintTime].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ApplicationMaximumRepaintTime].SetUInt16(0, value); }
        }

        #endregion
    }
}
