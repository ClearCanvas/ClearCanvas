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

using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Acquisition Context Module
    /// </summary>
    /// <remarks>
    /// This Module shall not contain descriptions of conditions that replace those that are already described
    /// in specific Modules or Attributes that are also contained within the IOD that contains this Module.
    /// </remarks>
    /// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.14 (Table C.7.6.14-1)</remarks>
    public class AcquisitionContextModuleIod : IodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
        /// </summary>	
        public AcquisitionContextModuleIod() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
        /// </summary>
        public AcquisitionContextModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

        /// <summary>
        /// Free-text description of the image-acquisition context.
        /// </summary>
        public string AcquisitionContextDescription
        {
            get { return DicomAttributeProvider[DicomTags.AcquisitionContextDescription].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.AcquisitionContextDescription].SetNullValue();
                    return;
                }
                DicomAttributeProvider[DicomTags.AcquisitionContextDescription].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of AcquisitionContextSequence in the underlying collection. Type 2.
        /// </summary>
        /// <remarks>
        /// A sequence of items that describes the conditions present during the acquisition of the 
        /// data of the SOP Instance.
        /// 
        /// Zero or more items shall be included in this sequence.
        /// </remarks>
        public AcquisitionContextSequence[] AcquisitionContextSequence
        {
            get
            {
                DicomAttribute dicomAttribute = DicomAttributeProvider[DicomTags.AcquisitionContextSequence];
                if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
                {
                    return null;
                }

                var result = new AcquisitionContextSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new AcquisitionContextSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    DicomAttributeProvider[DicomTags.AcquisitionContextSequence].SetNullValue();
                    return;
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.AcquisitionContextSequence].Values = result;
            }
        }
     

        /// <summary>
        /// Initializes the attributes of the module to their default values.
        /// </summary>
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
                yield return DicomTags.AcquisitionContextSequence;
                yield return DicomTags.AcquisitionContextDescription;               
            }
        }
    }
}
