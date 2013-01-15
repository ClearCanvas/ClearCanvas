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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom DOC 3 Table C.4-17
    /// </summary>
    public class BillingAndMaterialManagementCodesModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BillingAndMaterialManagementCodesModuleIod"/> class.
        /// </summary>
        public BillingAndMaterialManagementCodesModuleIod()
            :base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BillingAndMaterialManagementCodesModuleIod"/> class.
        /// </summary>
		public BillingAndMaterialManagementCodesModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains billing codes for the Procedure Type performed within the Procedure Step. The sequence may have zero or more Items.
        /// </summary>
        /// <value>The billing procedure step sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> BillingProcedureStepSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.BillingProcedureStepSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Information about the film consumption for this Performed Procedure Step. The sequence may have zero or more Items.
        /// </summary>
        /// <value>The film consumption sequence list.</value>
        public SequenceIodList<FilmConsumptionSequenceIod> FilmConsumptionSequenceList
        {
            get
            {
                return new SequenceIodList<FilmConsumptionSequenceIod>(base.DicomAttributeProvider[DicomTags.FilmConsumptionSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Chemicals, supplies and devices for billing used in the Performed Procedure Step. The sequence may have one or more Items.
        /// </summary>
        /// <value>The billing supplies and devices sequence list.</value>
        public SequenceIodList<BillingSuppliesAndDevicesSequenceIod> BillingSuppliesAndDevicesSequenceList
        {
            get
            {
                return new SequenceIodList<BillingSuppliesAndDevicesSequenceIod>(base.DicomAttributeProvider[DicomTags.BillingSuppliesAndDevicesSequence] as DicomAttributeSQ);
            }
        }
        
        

        #endregion

    }
}
