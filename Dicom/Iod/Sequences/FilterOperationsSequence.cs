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
    public class FilterOperationsSequence : SequenceIodBase, IHangingProtocolSelectorAttributeValueMacro, IHangingProtocolSelectorAttributeContextMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterOperationsSequence"/> class.
        /// </summary>
        public FilterOperationsSequence() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterOperationsSequence"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public FilterOperationsSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem)
        {
        }

        /// <summary>
        /// Gets or sets the value of FilterByCategory in the underlying collection. Type 1C.
        /// </summary>
        public string FilterByCategory
        {
            get { return DicomAttributeProvider[DicomTags.FilterByCategory].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.FilterByCategory].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorAttribute in the underlying collection. Type 1C.
        /// </summary>
        public uint SelectorAttribute
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAttribute].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorAttribute].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of SelectorAttributeVR in the underlying collection. Type 1C.
        /// </summary>
        public string SelectorAttributeVR
        {
            get { return DicomAttributeProvider[DicomTags.FilterByCategory].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.FilterByCategory].SetStringValue(value);
            }
        }

        #region attribute context marco & attribute value marco

        public void InitializeAttributes()
        {

        }

        /// <summary>
        /// Gets or sets the value of DisplaySetPatientOrientation in the underlying collection. Type 1C.
        /// </summary>
        public string DisplaySetPatientOrientation
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetPatientOrientation].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.DisplaySetPatientOrientation].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorSequencePointer in the underlying collection. Type 1C.
        /// </summary>
        public uint SelectorSequencePointer
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSequencePointer].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorSequencePointer].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of FunctionalGroupPointer in the underlying collection. Type 1C.
        /// </summary>
        public uint FunctionalGroupPointer
        {
            get { return DicomAttributeProvider[DicomTags.FunctionalGroupPointer].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.FunctionalGroupPointer].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of FunctionalGroupPrivateCreator in the underlying collection. Type 1C.
        /// </summary>
        public string FunctionalGroupPrivateCreator
        {
            get { return base.DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator] = null;
                    return;
                }
                base.DicomAttributeProvider[DicomTags.FunctionalGroupPrivateCreator].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorAttributePrivateCreator in the underlying collection. Type 1C.
        /// </summary>
        public string SelectorAttributePrivateCreator
        {
            get { return base.DicomAttributeProvider[DicomTags.SelectorAttributePrivateCreator].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.DicomAttributeProvider[DicomTags.SelectorAttributePrivateCreator] = null;
                    return;
                }
                base.DicomAttributeProvider[DicomTags.SelectorAttributePrivateCreator].SetString(0, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorSequencePointerPrivateCreator in the underlying collection. Type 1C.
        /// </summary>
        public string SelectorSequencePointerPrivateCreator
        {
            get { return base.DicomAttributeProvider[DicomTags.SelectorSequencePointerPrivateCreator].GetString(0, string.Empty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.DicomAttributeProvider[DicomTags.SelectorSequencePointerPrivateCreator] = null;
                    return;
                }
                base.DicomAttributeProvider[DicomTags.SelectorSequencePointerPrivateCreator].SetString(0, value);
            }
        }


        //AT expressed by uint all Type 1C
        public uint SelectorATValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAtValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorAtValue].SetUInt32(0, value); }
        }

        public uint SelectorCSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorCsValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorCsValue].SetUInt32(0, value); }
        }

        public uint SelectorISValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorIsValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorIsValue].SetUInt32(0, value); }
        }

        public uint SelectorLOValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorLoValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorLoValue].SetUInt32(0, value); }
        }
        public uint SelectorLTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorLtValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorLtValue].SetUInt32(0, value); }
        }

        public uint SelectorPNValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorPnValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorPnValue].SetUInt32(0, value); }
        }
        public uint SelectorSHValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorShValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorShValue].SetUInt32(0, value); }
        }

        public uint SelectorSTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorStValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorStValue].SetUInt32(0, value); }
        }

        public uint SelectorUTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUtValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorUtValue].SetUInt32(0, value); }
        }

        public uint SelectorDSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorDsValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorDsValue].SetUInt32(0, value); }
        }

        public uint SelectorFDValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorFdValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorFdValue].SetUInt32(0, value); }
        }

        public uint SelectorFLValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorFlValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorFlValue].SetUInt32(0, value); }
        }

        public uint SelectorULValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUlValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorUlValue].SetUInt32(0, value); }
        }

        public uint SelectorUSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUsValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorUsValue].SetUInt32(0, value); }
        }

        public uint SelectorSLValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSlValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorSlValue].SetUInt32(0, value); }
        }

        public uint SelectorSSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSsValue].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorSsValue].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of SelectorCodeSequenceValue in the underlying collection. Type 1C.
        /// </summary>
        public CodeSequenceMacro SelectorCodeSequenceValue
        {
            get
            {
                DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue];
                if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
                {
                    return null;
                }
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue];
                if (value == null)
                {
                    base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue] = null;
                    return;
                }
                dicomAttribute.Values = new DicomSequenceItem[] { value.DicomSequenceItem };
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the value of SelectorValueNumber in the underlying collection. Type 1C.
        /// </summary>
        public ushort SelectorValueNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.SelectorValueNumber].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.SelectorValueNumber].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of FilterByOperator in the underlying collection. Type 1C.
        /// </summary>
        public FilterByOperator FilterByOperator
        {
            get { return ParseEnum(DicomAttributeProvider[DicomTags.FilterByOperator].GetString(0, string.Empty), FilterByOperator.None); }
            set
            {
                if (value == FilterByOperator.None)
                {
                    DicomAttributeProvider[DicomTags.FilterByOperator] = null;
                    return;
                }
                SetAttributeFromEnum(DicomAttributeProvider[DicomTags.FilterByOperator], value);
            } 
        }
    }

    public enum FilterByOperator
    {
        None,
        RANGE_INCL,
        RANGE_EXCL,
        GREATER_OR_EQUAL,
        LESS_OR_EQUAL,
        GREATER_THAN,
        LESS_THAN,
        MEMBER_OF,
        NOT_MEMBER_OF
    }
}
