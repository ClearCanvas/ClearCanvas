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
        uint SelectorATValue { get; set; }
        uint SelectorCSValue { get; set; }
        uint SelectorISValue { get; set; }
        uint SelectorLOValue { get; set; }
        uint SelectorLTValue { get; set; }
        uint SelectorPNValue { get; set; }
        uint SelectorSHValue { get; set; }
        uint SelectorSTValue { get; set; }
        uint SelectorUTValue { get; set; }
        uint SelectorDSValue { get; set; }
        uint SelectorFDValue { get; set; }
        uint SelectorFLValue { get; set; }
        uint SelectorULValue { get; set; }
        uint SelectorUSValue { get; set; }
        uint SelectorSLValue { get; set; }
        uint SelectorSSValue { get; set; }

        CodeSequenceMacro SelectorCodeSequenceValue { get; set; }
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
    }
}
