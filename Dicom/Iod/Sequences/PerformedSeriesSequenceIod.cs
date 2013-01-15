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

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Performed Series Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.15, pg 256</remarks>
    public class PerformedSeriesSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedSeriesSequenceIod"/> class.
        /// </summary>
        public PerformedSeriesSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedSeriesSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public PerformedSeriesSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Name of the physician(s) administering this Series.
        /// </summary>
        /// <value>The name of the performing physicians.</value>
        public PersonName PerformingPhysiciansName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.PerformingPhysiciansName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.PerformingPhysiciansName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Identification of the physician(s) administering the Series. One or more items 
        /// shall be included in this sequence. If more than one Item, the number and
        /// order shall correspond to the value of Performing Physician�s Name (0008,1050), if present.
        /// </summary>
        /// <value>The performing physician identification sequence list.</value>
        public SequenceIodList<PersonIdentificationMacro> PerformingPhysicianIdentificationSequenceList
        {
            get
            {
                return new SequenceIodList<PersonIdentificationMacro>(base.DicomAttributeProvider[DicomTags.PerformingPhysicianIdentificationSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets or sets the name of the operators.
        /// </summary>
        /// <value>The name of the operators.</value>
        public PersonName OperatorsName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.OperatorsName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.OperatorsName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Identification of the operator(s) supporting the Series. One or more items shall be 
        /// included in this sequence. If more than one Item, the number and
        /// order shall correspond to the value of Operators� Name (0008,1070), if present.
        /// </summary>
        /// <value>The operator identification sequence list.</value>
        public SequenceIodList<PersonIdentificationMacro> OperatorIdentificationSequenceList
        {
            get
            {
                return new SequenceIodList<PersonIdentificationMacro>(base.DicomAttributeProvider[DicomTags.OperatorIdentificationSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// User-defined description of the conditions under which the Series was performed. 
        /// <para>Note: This attribute conveys series-specific protocol identification and may or may not be identical 
        /// to the one presented in the Performed Protocol Code Sequence (0040,0260).</para>
        /// </summary>
        /// <value>The name of the protocol.</value>
        public string ProtocolName
        {
            get { return base.DicomAttributeProvider[DicomTags.ProtocolName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ProtocolName].SetString(0, value); }
        }

        public string SeriesInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value); }
        }

        public string SeriesDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.SeriesDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SeriesDescription].SetString(0, value); }
        }

        /// <summary>
        /// Title of the DICOM Application Entity where the Images and other Composite SOP 
        /// Instances in this Series may be retrieved on the network.
        /// <para>Note: The duration for which this location remains valid is unspecified.</para>
        /// </summary>
        /// <value>The retrieve ae title.</value>
        public string RetrieveAeTitle
        {
            get { return base.DicomAttributeProvider[DicomTags.RetrieveAeTitle].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RetrieveAeTitle].SetString(0, value); }
        }

        /// <summary>
        /// A Sequence that provides reference to one or more sets of Image SOP Class/SOP 
        /// Instance pairs created during the acquisition of the procedure step.
        /// The sequence may have zero or more Items.
        /// </summary>
        /// <value>The referenced image sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedImageSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedImageSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Uniquely identifies instances, other than images, of any SOP Class that conforms to the DICOM 
        /// Composite IOD Information Model, such as Waveforms, Presentation States or Structured 
        /// Reports, created during the acquisition of the procedure step. The sequence may have zero or
        /// more Items.
        /// </summary>
        /// <value>The referenced non image composite sop instance sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedNonImageCompositeSopInstanceSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedNonImageCompositeSopInstanceSequence] as DicomAttributeSQ);
            }
        }
        
       #endregion
    }

}
