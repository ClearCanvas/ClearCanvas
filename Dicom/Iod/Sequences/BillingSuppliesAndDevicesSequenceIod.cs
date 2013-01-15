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
    /// Film Consumption Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.17, pg 260</remarks>
    public class BillingSuppliesAndDevicesSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BillingSuppliesAndDevicesSequenceIod"/> class.
        /// </summary>
        public BillingSuppliesAndDevicesSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingSuppliesAndDevicesSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public BillingSuppliesAndDevicesSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Code values of chemicals, supplies or devices required for billing. The sequence may have zero or one Items.
        /// </summary>
        /// <value>The billing item sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> BillingItemSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.BillingItemSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Sequence containing the quantity of used chemicals or devices. The sequence may have zero or one Items.
        /// </summary>
        /// <value>The quantity sequence list.</value>
        public SequenceIodList<QuantitySequenceIod> QuantitySequenceList
        {
            get
            {
                return new SequenceIodList<QuantitySequenceIod>(base.DicomAttributeProvider[DicomTags.QuantitySequence] as DicomAttributeSQ);
            }
        }
        
                
        #endregion
    }

    
}
