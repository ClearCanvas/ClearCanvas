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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// HangingProtocolDefinition Module
    /// </summary>
    /// <remarks>
    /// <para>As defined in the DICOM Standard 2011, Part 3, Section C.23.1 (Table C.23.1-1)</para>
    /// </remarks>
    public class HangingProtocolDefinitionModuleIod : IodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolDefinitionModuleIod"/> class.
        /// </summary>	
        public HangingProtocolDefinitionModuleIod() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolDefinitionModuleIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
        public HangingProtocolDefinitionModuleIod(IDicomAttributeProvider dicomAttributeProvider)
            : base(dicomAttributeProvider) { }

        /// <summary>
        /// Gets or sets the value of HangingProtocolName in the underlying collection. Type 1.
        /// </summary>
        public string HangingProtocolName
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolName].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "HangingProtocolName is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolName].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolDescription in the underlying collection. Type 1.
        /// </summary>
        public string HangingProtocolDescription
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolDescription].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "HangingProtocolDescription is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolDescription].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolLevel in the underlying collection. Type 1.
        /// </summary>
        public string HangingProtocolLevel
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolLevel].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "HangingProtocolLevel is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolLevel].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolCreator in the underlying collection. Type 1.
        /// </summary>
        public string HangingProtocolCreator
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolCreator].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "HangingProtocolCreator is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolCreator].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolCreationDateTime in the underlying collection. Type 1.
        /// </summary>
        public string HangingProtocolCreationDateTime
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolCreationDatetime].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "HangingProtocolCreationDatetime is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolCreationDatetime].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolDefinitionSequence in the underlying collection.Type 1.
        /// </summary>
        public HangingProtocolDefinitionSequenceItem[] HangingProtocolDefinitionSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.HangingProtocolDefinitionSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new HangingProtocolDefinitionSequenceItem[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new HangingProtocolDefinitionSequenceItem(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentNullException("value", "HangingProtocolDefinitionSequence is Type 1 Required.");
                    return;
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.HangingProtocolDefinitionSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of NumberOfPriorsReferenced in the underlying collection.Type 1.
        /// </summary>
        public uint NumberOfPriorsReferenced
        {
            get { return (uint)DicomAttributeProvider[DicomTags.NumberOfPriorsReferenced].Values; }
            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    throw new ArgumentNullException("value", "NumberOfPriorsReferenced is Type 1 Required.");
                DicomAttributeProvider[DicomTags.HangingProtocolCreationDatetime].SetUInt32(0,value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageSetsSequence in the underlying collection.Type 1.
        /// </summary>
        public ImageSetsSequence[] ImageSetsSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.ImageSetsSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new ImageSetsSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new ImageSetsSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentNullException("value", "ImageSetsSequence is Type 1 Required.");
                    return;
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.ImageSetsSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolUserIdentificationCodeSequence in the underlying collection.Type 2.
        /// </summary>
        public CodeSequenceMacro HangingProtocolUserIdentificationCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.HangingProtocolUserIdentificationCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.HangingProtocolUserIdentificationCodeSequence];
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// Gets or sets the value of HangingProtocolUserGroupName in the underlying collection.Type 3.
        /// </summary>
        public string HangingProtocolUserGroupName
        {
            get { return DicomAttributeProvider[DicomTags.HangingProtocolUserGroupName].ToString(); }
            set
            { DicomAttributeProvider[DicomTags.HangingProtocolUserGroupName].SetStringValue(value); }
        }

        /// <summary>
        /// Gets or sets the value of SourceHangingProtocolSequence in the underlying collection.Type 3.
        /// </summary>
        public SopInstanceReferenceMacro SourceHangingProtocolSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.SourceHangingProtocolSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;
                return new SopInstanceReferenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.SourceHangingProtocolSequence];
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }
    }
}
