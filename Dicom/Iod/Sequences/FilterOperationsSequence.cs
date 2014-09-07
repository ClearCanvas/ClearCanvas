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
        public uint? SelectorSequencePointer
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSequencePointer].Values as uint?; }
            set { DicomAttributeProvider[DicomTags.SelectorSequencePointer].Values = value; }
        }

        /// <summary>
        /// Gets or sets the value of FunctionalGroupPointer in the underlying collection. Type 1C.
        /// </summary>
        public uint? FunctionalGroupPointer
        {
            get { return DicomAttributeProvider[DicomTags.FunctionalGroupPointer].Values as uint?; }
            set { DicomAttributeProvider[DicomTags.FunctionalGroupPointer].Values = value; }
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
        public uint[] SelectorATValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAtValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorAtValue].Values = value; }
        }

        public uint[] SelectorCSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorCsValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorCsValue].Values = value; }
        }

        public uint[] SelectorISValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorIsValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorIsValue].Values = value; }
        }

        public uint[] SelectorLOValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorLoValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorLoValue].Values = value; }
        }
        public uint[] SelectorLTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorLtValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorLtValue].Values = value; }
        }

        public uint[] SelectorPNValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorPnValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorPnValue].Values = value; }
        }
        public uint[] SelectorSHValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorShValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorShValue].Values = value; }
        }

        public uint[] SelectorSTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorStValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorStValue].Values = value; }
        }

        public uint[] SelectorUTValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUtValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorUtValue].Values = value; }
        }

        public uint[] SelectorDSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorDsValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorDsValue].Values = value; }
        }

        public uint[] SelectorFDValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorFdValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorFdValue].Values = value; }
        }

        public uint[] SelectorFLValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorFlValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorFlValue].Values = value; }
        }

        public uint[] SelectorULValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUlValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorUlValue].Values = value; }
        }

        public uint[] SelectorUSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorUsValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorUsValue].Values = value; }
        }

        public uint[] SelectorSLValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSlValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorSlValue].Values = value; }
        }

        public uint[] SelectorSSValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorSsValue].Values as uint[]; }
            set { DicomAttributeProvider[DicomTags.SelectorSsValue].Values = value; }
        }

        /// <summary>
        /// Gets or sets the value of SelectorCodeSequenceValue in the underlying collection. Type 1C.
        /// </summary>
        public CodeSequenceMacro[] SelectorCodeSequenceValue
        {
            get
            {
                DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue];
                if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
                {
                    return null;
                }

                CodeSequenceMacro[] result = new CodeSequenceMacro[dicomAttribute.Count];
                DicomSequenceItem[] items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new CodeSequenceMacro(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue] = null;
                    return;
                }

                DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                base.DicomAttributeProvider[DicomTags.SelectorCodeSequenceValue].Values = result;
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
