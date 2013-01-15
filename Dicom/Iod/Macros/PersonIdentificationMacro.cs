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

namespace ClearCanvas.Dicom.Iod.Macros
{
    /// <summary>
    /// Person Identification Macro
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table 10-1 (pg 75)</remarks>
    public class PersonIdentificationMacro : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonIdentificationMacro"/> class.
        /// </summary>
        public PersonIdentificationMacro()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonIdentificationMacro"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public PersonIdentificationMacro(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// A coded entry which identifies a person.
        /// <para>The Code Meaning attribute, though it will be encoded with a VR of LO, 
        /// may be encoded according to the rules of the PN VR (e.g. caret �^� delimiters 
        /// shall separate name components), except that a single component (i.e. the
        /// whole name unseparated by caret delimiters) is not permitted. Name component 
        /// groups for use with multi-byte character sets are permitted, as long as they fit 
        /// within the 64 characters (the length of the LO VR).</para>
        /// <para>One or more Items may be permitted in this Sequence.</para>
        /// </summary>
        /// <value>The person identification code sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> PersonIdentificationCodeSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.PersonIdentificationCodeSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets or sets the persons address.
        /// </summary>
        /// <value>The persons address.</value>
        public string PersonsAddress
        {
            get { return base.DicomAttributeProvider[DicomTags.PersonsAddress].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PersonsAddress].SetString(0, value); }
        }

        /// <summary>
        /// Person's telephone number(s).  TODO: be able to specify list...
        /// </summary>
        /// <value>The persons telephone numbers.</value>
        public string PersonsTelephoneNumbers
        {
            get { return base.DicomAttributeProvider[DicomTags.PersonsTelephoneNumbers].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PersonsTelephoneNumbers].SetString(0, value); }
        }

        /// <summary>
        /// Institution or organization to which the identified individual is
        /// responsible or accountable. Shall not be present if Institution Code Sequence (0008,0082) is present.
        /// </summary>
        /// <value>The name of the institution.</value>
        public string InstitutionName
        {
            get { return base.DicomAttributeProvider[DicomTags.InstitutionName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.InstitutionName].SetString(0, value); }
        }

        /// <summary>
        /// Mailing address of the institution or organization to which 
        /// the identified individual is responsible or accountable.
        /// </summary>
        /// <value>The institution address.</value>
        public string InstitutionAddress
        {
            get { return base.DicomAttributeProvider[DicomTags.InstitutionAddress].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.InstitutionAddress].SetString(0, value); }
        }

        /// <summary>
        /// Institution or organization to which the identified individual is responsible or 
        /// accountable. Shall not be present if Institution Name (0008,0080) is present.
        /// <para>Only a single Item shall be permitted in this Sequence.</para>
        /// </summary>
        /// <value>The institution code sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> InstitutionCodeSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.InstitutionCodeSequence] as DicomAttributeSQ);
            }
        }

        
        #endregion

    }

}
