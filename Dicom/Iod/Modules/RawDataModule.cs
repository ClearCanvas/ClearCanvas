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
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{

    /// <summary>
	/// RawData Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.19.1 (Table C.19-1)</remarks>
	public class RawDataModule : IodBase
	{
        public const string ClearCanvasStudyAttachmentSeriesDescription = "ClearCanvas Attachment Series";
        public static DicomTag ClearCanvasRawDataGroupTag = new DicomTag(0x7FD10010, "ClearCanvas Raw Data Group", "ClearCanvasRawDataGroup",
                                                      DicomVr.LOvr, false, 1, 1, false);
        public static DicomTag ClearCanvasRawDataFilenameTag = new DicomTag(0x7FD11010, "ClearCanvas Raw Data Filename", "ClearCanvasRawDataFilename",
                                                              DicomVr.STvr, false, 1, 1, false);
        public static DicomTag ClearCanvasRawDataMimeTypeTag = new DicomTag(0x7FD11012, "ClearCanvas Raw Data Mime Type", "ClearCanvasRawDataMimeType",
                                                              DicomVr.LOvr, false, 1, 1, false);
        public static DicomTag ClearCanvasRawDataPaddingAddedTag = new DicomTag(0x7FD11014, "ClearCanvas Raw Data Padding Added", "ClearCanvasRawDataPaddingAdded",
                                                              DicomVr.LOvr, false, 1, 1, false);
        public static DicomTag ClearCanvasRawDataTag = new DicomTag(0x7FD11016, "ClearCanvas Raw Data", "ClearCanvasRawData",
                                                              DicomVr.OBvr, false, 1, 1, false);

		/// <summary>
        /// Initializes a new instance of the <see cref="RawDataModule"/> class.
		/// </summary>	
		public RawDataModule() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="RawDataModule"/> class.
		/// </summary>
        public RawDataModule(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }



        #region Public Properties
        /// <summary>
        /// Gets or sets the value of InstanceNumber in the underlying collection. Type 3.
        /// </summary>
        /// <value>The instance number.</value>
        public int? InstanceNumber
        {
            get
            {
                int result;
                if (DicomAttributeProvider[DicomTags.InstanceNumber].TryGetInt32(0, out result))
                    return result;
                return null;
            }
            set
            {
                if (!value.HasValue)
                {
                    DicomAttributeProvider[DicomTags.InstanceNumber] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value.Value);
            }
        }


        /// <summary>
        /// Gets or sets the content date.
        /// </summary>
        /// <value>The content date.</value>
        public DateTime? ContentDate
        {
            get
            {
                return DateTimeParser.ParseDateAndTime(DicomAttributeProvider, 0, DicomTags.ContentDate,
                                                       DicomTags.ContentTime);
            }
            set
            {
                DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider, 0, DicomTags.ContentDate,
                                                          DicomTags.ContentTime);
            }
        }

        /// <summary>
        /// Gets or sets the acquisition date.  Checks both the AcquisitionDatetime tag and the AcquisitionDate/AcquisitionTime tags.
        /// </summary>
        /// <value>The acquisition date.</value>
        public DateTime? AcquisitionDate
        {
            get { return DateTimeParser.ParseDateAndTime(DicomAttributeProvider, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate, DicomTags.AcquisitionTime); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider, DicomTags.AcquisitionDatetime, DicomTags.AcquisitionDate, DicomTags.AcquisitionTime); }
        }

        /// <summary>
        /// Laterality of (possibly paired) body part examined.
        /// </summary>
        /// <value>
        /// Enumerated values:
        /// R = right
        /// L = Left
        /// U = unpaired
        /// B = both left and right
        /// </value>
        public string ImageLaterality
        {
            get { return DicomAttributeProvider[DicomTags.ImageLaterality].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.ImageLaterality].SetNullValue();
                    return;
                }
                DicomAttributeProvider[DicomTags.ImageLaterality].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the Creator Version UID.  Unique identification of the equipment and version fo the software that has created the Raw Data information.
        /// The UID allows one to avoid attempting to interpret raw data with an unknown format.
        /// </summary>
        /// <value>The UID.</value>
        public string CreatorVersionUid
        {
            get { return DicomAttributeProvider[DicomTags.CreatorVersionUid].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.CreatorVersionUid].SetString(0, value); }
        }


        /// <summary>
        ///Other instances significantly related to this instance.  One or more items are permitted in this sequence.
        /// </summary>
        /// <value>The referenced instance sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedInstanceSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(DicomAttributeProvider[DicomTags.ReferencedInstanceSequence] as DicomAttributeSQ);
            }
        }

        public byte[] ClearCanvasRawData
        {
            get
            {
                DicomAttribute attribute;
                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataTag, out attribute))
                    return null;

                return attribute.Values as byte[];
            }
            set
            {
                DicomAttribute attribute;

                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataGroupTag, out attribute))
                    DicomAttributeProvider[ClearCanvasRawDataGroupTag].SetString(0, ClearCanvasRawDataGroupTag.Name);

                if (value == null)
                {
                    DicomAttributeProvider[ClearCanvasRawDataTag].SetEmptyValue();
                    ClearCanvasRawDataPaddingAdded = false;
                }
                else
                {
                    DicomAttributeProvider[ClearCanvasRawDataTag].Values = value;
                    ClearCanvasRawDataPaddingAdded = (value.Length & 0x00000001) == 0x00000001;
                }
            }
        }
        
        public bool ClearCanvasRawDataPaddingAdded
        {
            get
            {
                DicomAttribute attribute;
                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataPaddingAddedTag, out attribute))
                    return false;

                return attribute.ToString().Equals("TRUE");
            }
            set
            {
                DicomAttribute attribute;

                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataGroupTag, out attribute))
                    DicomAttributeProvider[ClearCanvasRawDataGroupTag].SetString(0, ClearCanvasRawDataGroupTag.Name);

                DicomAttributeProvider[ClearCanvasRawDataPaddingAddedTag].SetStringValue(value ? "TRUE" : "FALSE");
            }
        }

        public string ClearCanvasRawDataMimeType
        {
            get
            {
                DicomAttribute attribute;
                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataMimeTypeTag, out attribute))
                    return null;

                return attribute.ToString();
            }
            set
            {
                DicomAttribute attribute;

                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataGroupTag, out attribute))
                    DicomAttributeProvider[ClearCanvasRawDataGroupTag].SetString(0, ClearCanvasRawDataGroupTag.Name);

                if (value == null)
                    DicomAttributeProvider[ClearCanvasRawDataMimeTypeTag].SetEmptyValue();
                else
                    DicomAttributeProvider[ClearCanvasRawDataMimeTypeTag].Values = value;
            }
        }

        public string ClearCanvasRawDataFilename
        {
            get
            {
                DicomAttribute attribute;
                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataFilenameTag, out attribute))
                    return null;

                return attribute.ToString();
            }
            set
            {
                DicomAttribute attribute;

                if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasRawDataGroupTag, out attribute))
                    DicomAttributeProvider[ClearCanvasRawDataGroupTag].SetString(0, ClearCanvasRawDataGroupTag.Name);

                if (value == null)
                    DicomAttributeProvider[ClearCanvasRawDataFilenameTag].SetEmptyValue();
                else
                    DicomAttributeProvider[ClearCanvasRawDataFilenameTag].SetStringValue(value);
            }
        }
        #endregion


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
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.ContentDate;
				yield return DicomTags.ContentTime;
				yield return DicomTags.AcquisitionDatetime;
				yield return DicomTags.ImageLaterality;
				yield return DicomTags.CreatorVersionUid;
				yield return DicomTags.ReferencedImageSequence;
			}
		}
	}
}
