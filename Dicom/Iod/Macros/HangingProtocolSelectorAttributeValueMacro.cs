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

namespace ClearCanvas.Dicom.Iod.Macros
{
    public interface IHangingProtocolSelectorAttributeValueMacro : IIodMacro
    {
        uint[] SelectorATValue { get; set; }
        uint[] SelectorCSValue { get; set; }
        uint[] SelectorISValue { get; set; }
        uint[] SelectorLOValue { get; set; }
        uint[] SelectorLTValue { get; set; }
        uint[] SelectorPNValue { get; set; }
        uint[] SelectorSHValue { get; set; }
        uint[] SelectorSTValue { get; set; }
        uint[] SelectorUTValue { get; set; }
        uint[] SelectorDSValue { get; set; }
        uint[] SelectorFDValue { get; set; }
        uint[] SelectorFLValue { get; set; }
        uint[] SelectorULValue { get; set; }
        uint[] SelectorUSValue { get; set; }
        uint[] SelectorSLValue { get; set; }
        uint[] SelectorSSValue { get; set; }

        CodeSequenceMacro[] SelectorCodeSequenceValue { get; set; }
    }

    public class HangingProtocolSelectorAttributeValueMacro : SequenceIodBase, IHangingProtocolSelectorAttributeValueMacro
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolSelectorAttributeValueMacro"/> class.
		/// </summary>
		public HangingProtocolSelectorAttributeValueMacro() : base() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolSelectorAttributeValueMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public HangingProtocolSelectorAttributeValueMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Initializes the underlying collection to implement the module or sequence using default values.
        /// </summary>
        public void InitializeAttributes()
        {

        }

        //AT expressed by uint all Type 1C
        public uint[] SelectorATValue
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAtValue].Values as uint[];}
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
    }
}
