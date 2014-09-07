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

namespace ClearCanvas.Dicom.Iod.Macros
{
    public interface  IHangingProtocolSelectorAttributeContextMacro:IIodMacro
    {
       uint? SelectorSequencePointer { get; set; }
       uint? FunctionalGroupPointer { get; set; }
       string SelectorSequencePointerPrivateCreator { get; set; }
       string FunctionalGroupPrivateCreator { get; set; }
       string SelectorAttributePrivateCreator { get; set; }
    }
    public class HangingProtocolSelectorAttributeContextMacro : SequenceIodBase, IHangingProtocolSelectorAttributeContextMacro
    {
        		/// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolSelectorAttributeContextMacro"/> class.
		/// </summary>
		public HangingProtocolSelectorAttributeContextMacro() : base() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolSelectorAttributeContextMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public HangingProtocolSelectorAttributeContextMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Initializes the underlying collection to implement the module or sequence using default values.
        /// </summary>
        public void InitializeAttributes()
        {
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetPatientOrientation in the underlying collection. Type 3.
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
            get { return DicomAttributeProvider[DicomTags.SelectorSequencePointer].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.SelectorSequencePointer].Values = value; }
        }

        /// <summary>
        /// Gets or sets the value of FunctionalGroupPointer in the underlying collection. Type 1C.
        /// </summary>
        public uint? FunctionalGroupPointer
        {
            get { return DicomAttributeProvider[DicomTags.FunctionalGroupPointer].GetUInt32(0, 0); }
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
    }
}
