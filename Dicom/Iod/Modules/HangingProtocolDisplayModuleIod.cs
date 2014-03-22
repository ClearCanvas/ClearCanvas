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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// HangingProtocolDisplayModuleIod Module
    /// </summary>
    /// <remarks>
    /// <para>As defined in the DICOM Standard 2011, Part 3, Section C.23.3 (Table C.23.3-1)</para>
    /// </remarks>
    public class HangingProtocolDisplayModuleIod:IodBase
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolDisplayModuleIod"/> class.
        /// </summary>	
        public HangingProtocolDisplayModuleIod() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolDisplayModuleIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
        public HangingProtocolDisplayModuleIod(IDicomAttributeProvider dicomAttributeProvider)
            : base(dicomAttributeProvider) { }


        /// <summary>
        /// Gets or sets the value of DisplaySetsSequence in the underlying collection. Type 1.
        /// </summary>
        public DisplaySetsSequence[] DisplaySetsSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.DisplaySetsSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new DisplaySetsSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new DisplaySetsSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentNullException("value", "DisplaySetsSequence is Type 1 Required.");
                    return;
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.DisplaySetsSequence].Values = result;
            }
        }


        /// <summary>
        /// Gets or sets the value of PartialDataDisplayHandling in the underlying collection. Type 2.
        /// </summary>
        public string PartialDataDisplayHandling
        {
            get { return DicomAttributeProvider[DicomTags.PartialDataDisplayHandling].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.PartialDataDisplayHandling].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SynchronizedScrollingSequence in the underlying collection. Type 3.
        /// </summary>
        public SynchronizedScrollingSequence[] SynchronizedScrollingSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.SynchronizedScrollingSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new SynchronizedScrollingSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new SynchronizedScrollingSequence(items[n]);

                return result;
            }
            set
            {
                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.SynchronizedScrollingSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of NavigationIndicatorSequence in the underlying collection. Type 3.
        /// </summary>
        public NavigationIndicatorSequence[] NavigationIndicatorSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.NavigationIndicatorSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new NavigationIndicatorSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new NavigationIndicatorSequence(items[n]);

                return result;
            }
            set
            {
                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.NavigationIndicatorSequence].Values = result;
            }
        }
    }
}
