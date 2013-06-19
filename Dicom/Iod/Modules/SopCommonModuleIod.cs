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
	/// SopCommon Module
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.12.1 (Table C.12-1)</para>
	/// </remarks>
	public class SopCommonModuleIod
		: IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SopCommonModuleIod"/> class.
		/// </summary>	
		public SopCommonModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SopCommonModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public SopCommonModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of SopClassUid in the underlying collection. Type 1.
		/// </summary>
		public string SopClassUid
		{
			get { return DicomAttributeProvider[DicomTags.SopClassUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SopClassUid is Type 1 Required.");
				DicomAttributeProvider[DicomTags.SopClassUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SopClassUid in the underlying collection. Type 1.
		/// </summary>
		public SopClass SopClass
		{
			get { return SopClass.GetSopClass(SopClassUid); }
			set { SopClassUid = value != null ? value.Uid : string.Empty; }
		}

		/// <summary>
		/// Gets or sets the value of SopInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SopInstanceUid
		{
			get { return DicomAttributeProvider[DicomTags.SopInstanceUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SopInstanceUid is Type 1 Required.");
				DicomAttributeProvider[DicomTags.SopInstanceUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SpecificCharacterSet in the underlying collection. Type 1C.
		/// </summary>
		public string SpecificCharacterSet
		{
			get { return DicomAttributeProvider[DicomTags.SpecificCharacterSet].ToString(); }
			set
			{
				if (value == null) // an empty string has a special meaning for Specific Character Set
				{
					DicomAttributeProvider[DicomTags.SpecificCharacterSet] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SpecificCharacterSet].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InstanceCreationDate and InstanceCreationTime in the underlying collection. Type 3.
		/// </summary>
		public DateTime? InstanceCreationDateTime
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.InstanceCreationDate].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.InstanceCreationTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InstanceCreationDate] = null;
					DicomAttributeProvider[DicomTags.InstanceCreationTime] = null;
					return;
				}
				var date = DicomAttributeProvider[DicomTags.InstanceCreationDate];
				var time = DicomAttributeProvider[DicomTags.InstanceCreationTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of InstanceCreatorUid in the underlying collection. Type 3.
		/// </summary>
		public string InstanceCreatorUid
		{
			get { return DicomAttributeProvider[DicomTags.InstanceCreatorUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.InstanceCreatorUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InstanceCreatorUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RelatedGeneralSopClassUid in the underlying collection. Type 3.
		/// </summary>
		public string RelatedGeneralSopClassUid
		{
			get { return DicomAttributeProvider[DicomTags.RelatedGeneralSopClassUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.RelatedGeneralSopClassUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RelatedGeneralSopClassUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OriginalSpecializedSopClassUid in the underlying collection. Type 3.
		/// </summary>
		public string OriginalSpecializedSopClassUid
		{
			get { return DicomAttributeProvider[DicomTags.OriginalSpecializedSopClassUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.OriginalSpecializedSopClassUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.OriginalSpecializedSopClassUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeIdentificationSequence in the underlying collection. Type 3.
		/// </summary>
		public CodingSchemeIdentificationSequenceItem[] CodingSchemeIdentificationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CodingSchemeIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new CodingSchemeIdentificationSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (var n = 0; n < items.Length; n++)
					result[n] = new CodingSchemeIdentificationSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.CodingSchemeIdentificationSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (var n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.CodingSchemeIdentificationSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a CodingSchemeIdentificationSequence item. Does not modify the CodingSchemeIdentificationSequence in the underlying collection.
		/// </summary>
		public CodingSchemeIdentificationSequenceItem CreateCodingSchemeIdentificationSequence()
		{
			var iodBase = new CodingSchemeIdentificationSequenceItem(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of TimezoneOffsetFromUtc in the underlying collection. Type 3.
		/// </summary>
		public string TimezoneOffsetFromUtc
		{
			get { return DicomAttributeProvider[DicomTags.TimezoneOffsetFromUtc].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.TimezoneOffsetFromUtc] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.TimezoneOffsetFromUtc].SetStringValue(value);
			}
		}

		// TODO: Implement Contributing Equipment Sequence

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 3.
		/// </summary>
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
		/// Gets or sets the value of SopInstanceStatus in the underlying collection. Type 3.
		/// </summary>
		public string SopInstanceStatus
		{
			get { return DicomAttributeProvider[DicomTags.SopInstanceStatus].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SopInstanceStatus] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SopInstanceStatus].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SopAuthorizationDatetime in the underlying collection. Type 3.
		/// </summary>
		public DateTime? SopAuthorizationDatetime
		{
			get { return DateTimeParser.Parse(DicomAttributeProvider[DicomTags.SopAuthorizationDatetime].ToString()); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SopAuthorizationDatetime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SopAuthorizationDatetime].SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of SopAuthorizationComment in the underlying collection. Type 3.
		/// </summary>
		public string SopAuthorizationComment
		{
			get { return DicomAttributeProvider[DicomTags.SopAuthorizationComment].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SopAuthorizationComment] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SopAuthorizationComment].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AuthorizationEquipmentCertificationNumber in the underlying collection. Type 3.
		/// </summary>
		public string AuthorizationEquipmentCertificationNumber
		{
			get { return DicomAttributeProvider[DicomTags.AuthorizationEquipmentCertificationNumber].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.AuthorizationEquipmentCertificationNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.AuthorizationEquipmentCertificationNumber].SetStringValue(value);
			}
		}

		// TODO: Include Digital Signatures Macro
		// TODO: Implement Encrypted Attributes Sequence

        /// <summary>
        /// Gets or sets the value of OriginalAttributesSequence in the underlying collection. Type 3.
        /// </summary>
        /// <remarks>
        /// Sequence of items containing all attributes that were removed or replaced by other values in the main dataset.
        /// One or more items are permitted in this sequence.
        /// </remarks>
        public OriginalAttributesSequence[] OriginalAttributesSequence
        {
            get
            {
                DicomAttribute dicomAttribute;
                if (!DicomAttributeProvider.TryGetAttribute(DicomTags.OriginalAttributesSequence, out dicomAttribute))
                {
                    return null;
                }

                var result = new OriginalAttributesSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new OriginalAttributesSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    DicomAttributeProvider[DicomTags.OriginalAttributesSequence].SetNullValue();
                    return;
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.OriginalAttributesSequence].Values = result;
            }
        }


		// TODO: Implement HL7 Structured Document Reference Sequence

        /// <summary>
        /// Gets or sets the value of LongitudinalTemporalInformationModified in the underlying collection. Type 3.
        /// </summary>
        /// <remarks>
        /// Indicates whether or not the date and time attributes in the instance have been modified during
        /// de-dentification
        /// </remarks>
        /// <value>
        /// Enumerated values:
        /// UNMODIFIED
        /// MODIFIED
        /// REMOVED
        /// </value>
        public string LongitudinalTemporalInformationModified
        {
            get { return DicomAttributeProvider[DicomTags.LongitudinalTemporalInformationModified].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DicomAttributeProvider[DicomTags.AuthorizationEquipmentCertificationNumber] = null;
                    return;
                }
                DicomAttributeProvider[DicomTags.AuthorizationEquipmentCertificationNumber].SetStringValue(value);
            }
        }

		/// <summary>
		/// Initializes the attributes in this module to their default values.
		/// </summary>
		public void InitializeAttributes()
		{
			// nothing to initialize since the only required attributes will likely be overridden by client code anyway
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.SopClassUid;
				yield return DicomTags.SopInstanceUid;
				yield return DicomTags.SpecificCharacterSet;
				yield return DicomTags.InstanceCreationDate;
				yield return DicomTags.InstanceCreationTime;
				yield return DicomTags.InstanceCreatorUid;
				yield return DicomTags.RelatedGeneralSopClassUid;
				yield return DicomTags.OriginalSpecializedSopClassUid;
				yield return DicomTags.CodingSchemeIdentificationSequence;
				yield return DicomTags.TimezoneOffsetFromUtc;
				yield return DicomTags.ContributingEquipmentSequence;
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.SopInstanceStatus;
				yield return DicomTags.SopAuthorizationDatetime;
				yield return DicomTags.SopAuthorizationComment;
				yield return DicomTags.AuthorizationEquipmentCertificationNumber;
				yield return DicomTags.EncryptedAttributesSequence;
				yield return DicomTags.OriginalAttributesSequence;
				yield return DicomTags.Hl7StructuredDocumentReferenceSequence;
			    yield return DicomTags.LongitudinalTemporalInformationModified;
			}
		}
	}
}