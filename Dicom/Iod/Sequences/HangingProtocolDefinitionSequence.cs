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
    /// HangingProtocolDefinitionSequenceItem
    /// </summary>
    public class HangingProtocolDefinitionSequenceItem:SequenceIodBase
    {
        public HangingProtocolDefinitionSequenceItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolDefinitionSequence"/> class.
        /// </summary>	
        public HangingProtocolDefinitionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}


        /// <summary>
        /// Gets or sets the value of Modality in the underlying collection. Type 1C.
        /// </summary>
        public string Modality
        {
            get { return DicomAttributeProvider[DicomTags.Modality].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.CodingSchemeRegistry] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.Modality].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of AnatomicRegionSequence in the underlying collection. Type 1C.
        /// </summary>
        public CodeSequenceMacro AnatomicRegionSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.AnatomicRegionSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.AnatomicRegionSequence];
                if (value == null)
                {
                    dicomAttribute.SetNullValue();
                    return;
                }
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// Gets or sets the value of Laterality in the underlying collection. Type 2C.
        /// </summary>
        public string Laterality
        {
            get { return DicomAttributeProvider[DicomTags.Laterality].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.Laterality] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.Laterality].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ProcedureCodeSequence in the underlying collection. Type 2.
        /// </summary>
        public CodeSequenceMacro ProcedureCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ProcedureCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ProcedureCodeSequence];
                if (value == null)
                {
                    dicomAttribute.SetNullValue();
                    return;
                }
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// Gets or sets the value of ReasonForRequestedProcedureCodeSequence in the underlying collection. Type 2.
        /// </summary>
        public CodeSequenceMacro ReasonForRequestedProcedureCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ReasonForRequestedProcedureCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ReasonForRequestedProcedureCodeSequence];
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
