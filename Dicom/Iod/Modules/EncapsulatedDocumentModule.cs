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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// EncapsulatedDocument Module
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.24.2 (Table C.24-2)</para>
	/// </remarks>
	public class EncapsulatedDocumentModuleIod
		: IodBase
	{
		public static DicomTag ClearCanvasEncapsulatedDocumentGroupTag = new DicomTag(0x00430010, "ClearCanvas Encapsulated Document Group", "ClearCanvasEncapsulatedDocumentGroup",
											  DicomVr.LOvr, false, 1, 1, false);
		public static DicomTag ClearCanvasHl7MessageControlIdTag = new DicomTag(0x00431012, "ClearCanvas HL7 Message Control Id", "ClearCanvasHl7MessageControlId",
													  DicomVr.LOvr, false, 1, 1, false);

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedDocumentModuleIod"/> class.
		/// </summary>	
		public EncapsulatedDocumentModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedDocumentModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public EncapsulatedDocumentModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of InstanceNumber in the underlying collection. Type 1.
		/// </summary>
		public int InstanceNumber
		{
			get { return DicomAttributeProvider[DicomTags.InstanceNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.InstanceNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ContentDate and ContentTime in the underlying collection.  Type 2.
		/// </summary>
		public DateTime? ContentDateTime
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.ContentDate].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.ContentTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ContentDate].SetNullValue();
					DicomAttributeProvider[DicomTags.ContentTime].SetNullValue();
					return;
				}
				var date = DicomAttributeProvider[DicomTags.ContentDate];
				var time = DicomAttributeProvider[DicomTags.ContentTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of AcquisitionDatetime in the underlying collection. Type 2.
		/// </summary>
		public DateTime? AcquisitionDateTime
		{
			get { return DateTimeParser.Parse(DicomAttributeProvider[DicomTags.AcquisitionDatetime].ToString()); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.AcquisitionDatetime].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.AcquisitionDatetime].SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of BurnedInAnnotation in the underlying collection. Type 1.
		/// </summary>
		public bool BurnedInAnnotation
		{
			get { return !string.Equals(@"NO", DicomAttributeProvider[DicomTags.BurnedInAnnotation].GetString(0, string.Empty), StringComparison.InvariantCultureIgnoreCase); }
			set { DicomAttributeProvider[DicomTags.BurnedInAnnotation].SetString(0, value ? @"YES" : @"NO"); }
		}

		/// <summary>
		/// Gets or sets the value of SourceInstanceSequence in the underlying collection. Type 1C.
		/// </summary>
		public ISopInstanceReferenceMacro[] SourceInstanceSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SourceInstanceSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new ISopInstanceReferenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (var n = 0; n < items.Length; n++)
					result[n] = new SopInstanceReferenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.SourceInstanceSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (var n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.SourceInstanceSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a SourceInstanceSequence item. Does not modify the SourceInstanceSequence in the underlying collection.
		/// </summary>
		public ISopInstanceReferenceMacro CreateSourceInstanceSequence()
		{
			var iodBase = new SopInstanceReferenceMacro(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of DocumentTitle in the underlying collection. Type 2.
		/// </summary>
		public string DocumentTitle
		{
			get { return DicomAttributeProvider[DicomTags.DocumentTitle].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DocumentTitle].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.DocumentTitle].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ConceptNameCodeSequence in the underlying collection. Type 2.
		/// </summary>
		public CodeSequenceMacro ConceptNameCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
				if (value == null)
				{
					dicomAttribute.SetNullValue();
					return;
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ConceptNameCodeSequence in the underlying collection. Type 2.
		/// </summary>
		public CodeSequenceMacro CreateConceptNameCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CodeSequenceMacro(dicomSequenceItem);
				return sequenceType;
			}
			return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of VerificationFlag in the underlying collection. Type 3.
		/// </summary>
		public VerificationFlag VerificationFlag
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.VerificationFlag].GetString(0, string.Empty), VerificationFlag.None); }
			set
			{
				if (value == VerificationFlag.None)
				{
					DicomAttributeProvider[DicomTags.VerificationFlag] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.VerificationFlag], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Hl7InstanceIdentifier in the underlying collection. Type 1C.
		/// </summary>
		public string Hl7InstanceIdentifier
		{
			get { return DicomAttributeProvider[DicomTags.Hl7InstanceIdentifier].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.Hl7InstanceIdentifier] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Hl7InstanceIdentifier].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of MimeTypeOfEncapsulatedDocument in the underlying collection. Type 1.
		/// </summary>
		public string MimeTypeOfEncapsulatedDocument
		{
			get { return DicomAttributeProvider[DicomTags.MimeTypeOfEncapsulatedDocument].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "MimeTypeOfEncapsulatedDocument is Type 1 Required.");
				DicomAttributeProvider[DicomTags.MimeTypeOfEncapsulatedDocument].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ListOfMimeTypes in the underlying collection. Type 1C.
		/// </summary>
		public string[] ListOfMimeTypes
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ListOfMimeTypes];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new string[dicomAttribute.Count];
				for (var n = 0; n < result.Length; n++)
					result[n] = dicomAttribute.GetString(n, string.Empty);
				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ListOfMimeTypes] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.ListOfMimeTypes];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetString(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of EncapsulatedDocument in the underlying collection. Type 1.
		/// </summary>
		/// <remarks></remarks>
		public byte[] EncapsulatedDocument
		{
			get
			{
				var attribute = DicomAttributeProvider[DicomTags.EncapsulatedDocument];
				if (attribute.IsNull || attribute.IsEmpty)
					return null;

                if (MimeTypeOfEncapsulatedDocument.Equals(@"application/pdf", StringComparison.InvariantCultureIgnoreCase))
			        return PdfStreamHelper.StripDicomPaddingBytes((byte[]) attribute.Values);
			    
                return (byte[]) attribute.Values;
			}
			set
			{
				if (value == null)
					throw new ArgumentOutOfRangeException("value", "EncapsulatedDocument is Type 1 Required.");

                DicomAttributeProvider[DicomTags.EncapsulatedDocument].Values = PadToEvenLength(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of the private tag ClearCanvas HL7 Message Control Id.
		/// </summary>
		/// <remarks>
		/// This private attribute stores an HL7 Message Control Id associated with an HL7 message that created the encapsulated document.  
		/// </remarks>
		public string ClearCanvasHl7MessageControlId
		{
			get
			{
				DicomAttribute attribute;
				if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasHl7MessageControlIdTag, out attribute))
					return null;

				return attribute.ToString();
			}
			set
			{
				DicomAttribute attribute;

				if (!DicomAttributeProvider.TryGetAttribute(ClearCanvasEncapsulatedDocumentGroupTag, out attribute))
					DicomAttributeProvider[ClearCanvasEncapsulatedDocumentGroupTag].SetString(0, ClearCanvasEncapsulatedDocumentGroupTag.Name);

				if (value == null)
					DicomAttributeProvider[ClearCanvasHl7MessageControlIdTag].SetEmptyValue();
				else
					DicomAttributeProvider[ClearCanvasHl7MessageControlIdTag].Values = value;
			}
		}

		/// <summary>
		/// Initializes the attributes in this module to their default values.
		/// </summary>
		public void InitializeAttributes()
		{
			InstanceNumber = 0;
			ContentDateTime = null;
			AcquisitionDateTime = null;
			BurnedInAnnotation = false;
			SourceInstanceSequence = null;
			DocumentTitle = string.Empty;
			ConceptNameCodeSequence = null;
			Hl7InstanceIdentifier = null;
			MimeTypeOfEncapsulatedDocument = @"application/octet-stream";
			ListOfMimeTypes = null;
			EncapsulatedDocument = new byte[0];
		}

		/// <summary>
		/// Gets an enumeration of <see cref="ClearCanvas.Dicom.DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.InstanceNumber;
				yield return DicomTags.ContentDate;
				yield return DicomTags.ContentTime;
				yield return DicomTags.AcquisitionDatetime;
				yield return DicomTags.BurnedInAnnotation;
				yield return DicomTags.SourceInstanceSequence;
				yield return DicomTags.DocumentTitle;
				yield return DicomTags.ConceptNameCodeSequence;
				yield return DicomTags.VerificationFlag;
				yield return DicomTags.Hl7InstanceIdentifier;
				yield return DicomTags.MimeTypeOfEncapsulatedDocument;
				yield return DicomTags.ListOfMimeTypes;
				yield return DicomTags.EncapsulatedDocument;
			}
		}

        #region Private Methods

        private static byte[] PadToEvenLength(byte[] data)
        {
            if (data.Length % 2 == 1)
            {
                // pad data to even length
                var buffer = new byte[data.Length + 1];
                Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
                buffer[buffer.Length - 1] = 0;
                return buffer;
            }
            return data;
        }

        #endregion
    }
}