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

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// HangingProtocolDefinitionModuleIod Module
    /// </summary>
    /// <remarks>
    /// <para>As defined in the DICOM Standard 2011, Part 3, Section C.23.2 (Table C.23.2-1)</para>
    /// </remarks>
    public class HangingProtocolEnvironmentModuleIod : IodBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolEnvironmentModuleIod"/> class.
        /// </summary>	
        public HangingProtocolEnvironmentModuleIod() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolEnvironmentModuleIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
        public HangingProtocolEnvironmentModuleIod(IDicomAttributeProvider dicomAttributeProvider)
            : base(dicomAttributeProvider) { }

        /// <summary>
        /// Gets or sets the value of NumberOfScreens in the underlying collection. Type 2.
        /// </summary>
        public string NumberOfScreens
        {
            get { return DicomAttributeProvider[DicomTags.NumberOfScreens].ToString(); }
            set { DicomAttributeProvider[DicomTags.NumberOfScreens].SetStringValue(value); }
        }

        /// <summary>
        /// Gets or sets the value of NominalScreenDefinitionSequence in the underlying collection. Type 2.
        /// </summary>
        public ScreenSpecificationsMacro NominalScreenDefinitionSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.NominalScreenDefinitionSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                {
                    return null;
                }
                return new ScreenSpecificationsMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.NominalScreenDefinitionSequence];
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
