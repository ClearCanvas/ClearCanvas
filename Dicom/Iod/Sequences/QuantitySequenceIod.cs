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

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Quantity Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.17, pg 260</remarks>
    public class QuantitySequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QuantitySequenceIod"/> class.
        /// </summary>
        public QuantitySequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantitySequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public QuantitySequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Numerical quantity value.
        /// </summary>
        /// <value>The quantity.</value>
        public float Quantity
        {
            get { return base.DicomAttributeProvider[DicomTags.Quantity].GetFloat32(0, 0.0F); }
            set { base.DicomAttributeProvider[DicomTags.Quantity].SetFloat32(0, value); }
        }

        /// <summary>
        /// Unit of measurement. The sequence may have zero or one Items.
        /// Baseline Context ID is 82.
        /// </summary>
        /// <value>The measuring units sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> MeasuringUnitsSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.MeasuringUnitsSequence] as DicomAttributeSQ);
            }
        }
        
        #endregion
    }

    
}
