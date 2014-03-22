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

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// DisplaySetsSequence
    /// </summary>
    /// <remarks>As defined in the DICOM Standard 2011, Part 3</remarks>
    public class DisplaySetsSequence : SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaySetsSequence"/> class.
        /// </summary>	
        public DisplaySetsSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaySetsSequence"/> class.
        /// </summary>
        public DisplaySetsSequence(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetNumber in the underlying collection. Type 1.
        /// </summary>
        public string DisplaySetNumber
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetNumber].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "DisplaySetNumber is Type 1 Required.");
                DicomAttributeProvider[DicomTags.DisplaySetNumber].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetLabel in the underlying collection. Type 3.
        /// </summary>
        public string DisplaySetLabel
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetLabel].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.DisplaySetLabel].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetPresentationGroup in the underlying collection. Type 1.
        /// </summary>
        public string DisplaySetPresentationGroup
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetPresentationGroup].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "DisplaySetPresentationGroup is Type 1 Required.");
                DicomAttributeProvider[DicomTags.DisplaySetPresentationGroup].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageSetNumber in the underlying collection. Type 1.
        /// </summary>
        public string ImageSetNumber
        {
            get { return DicomAttributeProvider[DicomTags.ImageSetNumber].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "ImageSetNumber is Type 1 Required.");
                DicomAttributeProvider[DicomTags.ImageSetNumber].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageBoxesSequence in the underlying collection. Type 1.
        /// </summary>
        public ImageBoxesSequence[] ImageBoxesSequences
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.ImageBoxesSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new ImageBoxesSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new ImageBoxesSequence(items[n]);

                return result;
            }
            set
            {
                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.ImageBoxesSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of FilterOperationsSequence in the underlying collection. Type 1.
        /// </summary>
        public FilterOperationsSequence[] FilterOperationsSequences
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.FilterOperationsSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new FilterOperationsSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new FilterOperationsSequence(items[n]);

                return result;
            }
            set
            {
                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.FilterOperationsSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of FilterOperationsSequence in the underlying collection. Type 2.
        /// </summary>
        public SortingOperationsSequence[] SortingOperationsSequences
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.SortingOperationsSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new SortingOperationsSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new SortingOperationsSequence(items[n]);

                return result;
            }
            set
            {
                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.SortingOperationsSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of BlendingOperationType in the underlying collection. Type 3.
        /// </summary>
        public string BlendingOperationType
        {
            get { return DicomAttributeProvider[DicomTags.BlendingOperationType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.BlendingOperationType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReformattingOperationType in the underlying collection. Type 3.
        /// </summary>
        public string ReformattingOperationType
        {
            get { return DicomAttributeProvider[DicomTags.ReformattingOperationType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ReformattingOperationType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReformattingThickness in the underlying collection. Type 1C.
        /// </summary>
        public double ReformattingThickness
        {
            get { return DicomAttributeProvider[DicomTags.ReformattingThickness].GetFloat64(0,0); }
            set
            {
                DicomAttributeProvider[DicomTags.BlendingOperationType].SetFloat64(0,value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReformattingInterval in the underlying collection. Type 1C.
        /// </summary>
        public double ReformattingInterval
        {
            get { return DicomAttributeProvider[DicomTags.ReformattingInterval].GetFloat64(0,0); }
            set
            {
                DicomAttributeProvider[DicomTags.ReformattingInterval].SetFloat64(0,value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReformattingOperationInitialViewDirection in the underlying collection. Type 1C.
        /// </summary>
        public string ReformattingOperationInitialViewDirection
        {
            get { return DicomAttributeProvider[DicomTags.ReformattingOperationInitialViewDirection].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ReformattingOperationInitialViewDirection].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ThreeDRenderingType in the underlying collection. Type 1C.
        /// </summary>
        public string ThreeDRenderingType
        {
            get { return DicomAttributeProvider[DicomTags.ThreeDRenderingType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ThreeDRenderingType].SetStringValue(value);
            }
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
        /// Gets or sets the value of DisplaySetHorizontalJustificationItem in the underlying collection. Type 3.
        /// </summary>
        public DisplaySetHorizontalJustificationItem DisplaySetHorizontalJustification
        {
            get { return ParseEnum(DicomAttributeProvider[DicomTags.DisplaySetHorizontalJustification].GetString(0, string.Empty), DisplaySetHorizontalJustificationItem.None); }
            set
            {
                if (value == DisplaySetHorizontalJustificationItem.None)
                {
                    DicomAttributeProvider[DicomTags.DisplaySetHorizontalJustification] = null;
                    return;
                }
                SetAttributeFromEnum(DicomAttributeProvider[DicomTags.DisplaySetHorizontalJustification], value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetVerticalJustification in the underlying collection. Type 3.
        /// </summary>
        public DisplaySetVerticalJustificationItem DisplaySetVerticalJustification
        {
            get { return ParseEnum(DicomAttributeProvider[DicomTags.DisplaySetVerticalJustification].GetString(0, string.Empty), DisplaySetVerticalJustificationItem.None); }
            set
            {
                if (value == DisplaySetVerticalJustificationItem.None)
                {
                    DicomAttributeProvider[DicomTags.DisplaySetVerticalJustification] = null;
                    return;
                }
                SetAttributeFromEnum(DicomAttributeProvider[DicomTags.DisplaySetVerticalJustification], value);
            }
        }

        /// <summary>
        /// Gets or sets the value of VoiType in the underlying collection. Type 3.
        /// </summary>
        public string VoiType
        {
            get { return DicomAttributeProvider[DicomTags.VoiType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.VoiType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of VoiType in the underlying collection. Type 3.
        /// </summary>
        public string PseudoColorType
        {
            get { return DicomAttributeProvider[DicomTags.PseudoColorType].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.PseudoColorType].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of PseudoColorPaletteInstanceReferenceSequence in the underlying collection. Type 3.
        /// </summary>
        public SopInstanceReferenceMacro PseudoColorPaletteInstanceReferenceSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.PseudoColorPaletteInstanceReferenceSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;
                return new SopInstanceReferenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.PseudoColorPaletteInstanceReferenceSequence];
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// Gets or sets the value of ShowGrayscaleInverted in the underlying collection. Type 3.
        /// </summary>
        public string ShowGrayscaleInverted
        {
            get { return DicomAttributeProvider[DicomTags.ShowGrayscaleInverted].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ShowGrayscaleInverted].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ShowImageTrueSizeFlag in the underlying collection. Type 3.
        /// </summary>
        public string ShowImageTrueSizeFlag
        {
            get { return DicomAttributeProvider[DicomTags.ShowImageTrueSizeFlag].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ShowImageTrueSizeFlag].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ShowGraphicAnnotationFlag in the underlying collection. Type 3.
        /// </summary>
        public string ShowGraphicAnnotationFlag
        {
            get { return DicomAttributeProvider[DicomTags.ShowGraphicAnnotationFlag].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ShowGraphicAnnotationFlag].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ShowPatientDemographicsFlag in the underlying collection. Type 3.
        /// </summary>
        public string ShowPatientDemographicsFlag
        {
            get { return DicomAttributeProvider[DicomTags.ShowPatientDemographicsFlag].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ShowPatientDemographicsFlag].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ShowAcquisitionTechniquesFlag in the underlying collection. Type 3.
        /// </summary>
        public string ShowAcquisitionTechniquesFlag
        {
            get { return DicomAttributeProvider[DicomTags.ShowAcquisitionTechniquesFlag].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ShowAcquisitionTechniquesFlag].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisplaySetPresentationGroupDescription in the underlying collection. Type 3.
        /// </summary>
        public string DisplaySetPresentationGroupDescription
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetPresentationGroupDescription].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.DisplaySetPresentationGroupDescription].SetStringValue(value);
            }
        }

        public enum DisplaySetHorizontalJustificationItem
        {
            None,
            Left,
            Center,
            Right
        }

        public enum DisplaySetVerticalJustificationItem
        {
            None,
            Top,
            Center,
            Bottom
        }
    }
}
