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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Acquisition Context Sequence Item
    /// </summary>
    /// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.14 (Table C.7.6.14-1)</remarks>
    public class AcquisitionContextSequence : SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcquisitionContextSequence"/> class.
        /// </summary>	
        public AcquisitionContextSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcquisitionContextSequence"/> class.
        /// </summary>
        public AcquisitionContextSequence(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }

        /// <summary>
        /// The type of the value encoded in this item.  Type 3.
        /// </summary>
        /// <remarks>
        /// Defined Terms:
        /// TEXT, NUMERIC, CODE, DATE, TIME, PNAME
        /// </remarks>
        public string ValueType
        {
            get { return DicomAttributeProvider[DicomTags.ValueType].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.ValueType].SetNullValue();
                    return;
                }
                DicomAttributeProvider[DicomTags.ValueType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ConceptNameCodeSequence in the underlying collection. Type 2.
        /// </summary>
        public CodeSequenceMacro ConceptNameCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
                if (value == null)
                {
                    dicomAttribute.SetNullValue();
                    return;
                }
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// This is the Value component of a Name/Value pair when the Concept implied by Concept Name Code Sequence
        /// is a set of one or more numeric values.
        /// </summary>
        /// <value>The images in acquisition.</value>
        public string[] NumericValue
        {
            get { return DicomAttributeProvider[DicomTags.NumericValue].Values as string[]; }
            set { DicomAttributeProvider[DicomTags.NumericValue].Values = value; }
        }

        /// <summary>
        /// Gets or sets the value of MeasurementUnitsCodeSequence in the underlying collection. Type 1C.
        /// </summary>
        public CodeSequenceMacro MeasurementUnitsCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.MeasurementUnitsCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.MeasurementUnitsCodeSequence];
                if (value == null)
                {
                    dicomAttribute.SetNullValue();
                    return;
                }
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// This is the Value component of the Name/Value pair when the Concept implied by Concept Name Code Sequence is a date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime? DateTime
        {
            get
            {
                string date = DicomAttributeProvider[DicomTags.Date].GetString(0, string.Empty);
                string time = DicomAttributeProvider[DicomTags.Time].GetString(0, string.Empty);
                return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
            }
            set
            {
                if (!value.HasValue)
                {
                    DicomAttributeProvider[DicomTags.Date].SetNullValue();
                    DicomAttributeProvider[DicomTags.Time].SetNullValue();
                    return;
                }
                DicomAttribute date = DicomAttributeProvider[DicomTags.Date];
                DicomAttribute time = DicomAttributeProvider[DicomTags.Time];
                DateTimeParser.SetDateTimeAttributeValues(value, date, time);
            }
        }

        /// <summary>
        /// Gets or sets the value of PersonName in the underlying collection. Type 1C.
        /// </summary>
        public string PersonName
        {
            get { return DicomAttributeProvider[DicomTags.PersonName].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.PersonName] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.PersonName].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of TextValue in the underlying collection. Type 1C.
        /// </summary>
        public string TextValue
        {
            get { return DicomAttributeProvider[DicomTags.TextValue].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.TextValue] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.TextValue].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ConceptCodeSequence in the underlying collection. Type 1C.
        /// </summary>
        /// <remarks>
        /// This is the Value component of Name/Value pair when the Concept implied by the Concept Name
        /// Code Sequence is a Coded Value.
        /// 
        /// Onlya single item shall be included in this sequence.
        /// </remarks>
        public CodeSequenceMacro ConceptCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptCodeSequence];
                if (value == null)
                {
                    dicomAttribute.SetNullValue();
                    return;
                }
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }
    }
}
