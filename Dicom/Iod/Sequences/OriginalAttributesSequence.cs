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
using System.Collections.Generic;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Original Attributes Sequence Item
    /// </summary>
    /// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.12.1 (Table C.12-1)</remarks>
    public class OriginalAttributesSequence : SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OriginalAttributesSequence"/> class.
        /// </summary>	
        public OriginalAttributesSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginalAttributesSequence"/> class.
        /// </summary>
        public OriginalAttributesSequence(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
      
        /// <summary>
        /// Gets or sets the value of SourceOfPreviousValues in the underlying collection. Type 1.
        /// </summary>
        /// <remarks>
        /// The source that provided the SOP Instance prior to the removal or replacement of the values.
        /// For example, this might be the Institution from which imported SOP Instances were received.
        /// </remarks>
        public string SourceOfPreviousValues
        {
            get { return DicomAttributeProvider[DicomTags.SourceOfPreviousValues].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.SourceOfPreviousValues] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.SourceOfPreviousValues].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the Attribute modification date/time.  Type 1.
        /// </summary>
        /// <remarks>Date and time the attributes were removed and/or replaced.</remarks>
        /// <value>The attribute modification date.</value>
        public DateTime? AttributeModificationDatetime
        {
            get { return DateTimeParser.Parse(DicomAttributeProvider[DicomTags.AttributeModificationDatetime].ToString()); }
            set
            {
                if (!value.HasValue)
                {
                    DicomAttributeProvider[DicomTags.AttributeModificationDatetime] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.AttributeModificationDatetime].SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
            }
        }

        /// <summary>
        /// Gets or sets the value of ModifyingSystem in the underlying collection. Type 1.
        /// </summary>
        /// <remarks>
        /// Identification of the system which removed and/or replaced the attributes.
        /// </remarks>
        public string ModifyingSystem
        {
            get { return DicomAttributeProvider[DicomTags.ModifyingSystem].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.ModifyingSystem] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.ModifyingSystem].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReasonForTheAttributeModification in the underlying collection. Type 1.
        /// </summary>
        /// <remarks>
        /// Reason for the attribute modification.  Defined terms are: 
        /// COERCE = Replace values of attributes such as Patient Name, ID, Accession Number, for example, during import of media from an external institution or reconciliation against a master patient index.
        /// CORRECT = Replace incorrect values, such as Patient Name or ID, for example, when incorrect worklist item was chosen or operator input error.
        /// </remarks>
        public string ReasonForTheAttributeModification
        {
            get { return DicomAttributeProvider[DicomTags.ReasonForTheAttributeModification].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.ReasonForTheAttributeModification] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.ReasonForTheAttributeModification].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ModifiedAttributesSequence in the underlying collection. Type 1.
        /// </summary>
        /// <remarks>
        /// Sequence that contains all the attributes with their previous values, that were modified or removed from the main data set.  
        /// Only a single item shall be included in this sequence.
        /// </remarks>
        public DicomSequenceItem ModifiedAttributesSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.ModifiedAttributesSequence, out dicomAttribute))
                {
                    return null;
                }

                var items = (DicomSequenceItem[])dicomAttribute.Values;
                return items[0];
            }
            set
            {
                if (value == null)
                {
                    DicomAttributeProvider[DicomTags.ModifiedAttributesSequence].SetNullValue();
                    return;
                }

                DicomAttributeProvider[DicomTags.ModifiedAttributesSequence].SetEmptyValue();
                DicomAttributeProvider[DicomTags.ModifiedAttributesSequence].Values = value;
            }
        }

        public void InitializeAttributes()
        {
  
        }

        /// <summary>
        /// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
        /// </summary>
        public static IEnumerable<uint> DefinedTags
        {
            get
            {
                yield return DicomTags.SourceOfPreviousValues;
                yield return DicomTags.AttributeModificationDatetime;
                yield return DicomTags.ModifyingSystem;
                yield return DicomTags.ReasonForTheAttributeModification;
                yield return DicomTags.ModifiedAttributesSequence;
            }
        }
    }
}