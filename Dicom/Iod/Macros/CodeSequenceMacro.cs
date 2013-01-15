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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// Code Sequence Attributes Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 8.8 (Table 8.8-1)</remarks>
	public class CodeSequenceMacro : SequenceIodBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeSequenceMacro"/> class.
		/// </summary>
		public CodeSequenceMacro()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeSequenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public CodeSequenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the code value.
		/// </summary>
		/// <value>The code value.</value>
		public string CodeValue
		{
			get { return DicomAttributeProvider[DicomTags.CodeValue].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.CodeValue].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the coding scheme designator.
		/// </summary>
		/// <value>The coding scheme designator.</value>
		public string CodingSchemeDesignator
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeDesignator].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.CodingSchemeDesignator].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the coding scheme version.
		/// </summary>
		/// <value>The coding scheme version.</value>
		public string CodingSchemeVersion
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeVersion].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.CodingSchemeVersion].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the code meaning.
		/// </summary>
		/// <value>The code meaning.</value>
		public string CodeMeaning
		{
			get { return DicomAttributeProvider[DicomTags.CodeMeaning].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.CodeMeaning].SetString(0, value); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the context identifier.
		/// </summary>
		/// <value>The context identifier.</value>
		public string ContextIdentifier
		{
			get { return DicomAttributeProvider[DicomTags.ContextIdentifier].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.ContextIdentifier].SetString(0, value); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the mapping resource.
		/// </summary>
		/// <value>The mapping resource.</value>
		public string MappingResource
		{
			get { return DicomAttributeProvider[DicomTags.MappingResource].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.MappingResource].SetString(0, value); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the context group version.
		/// </summary>
		/// <value>The context group version.</value>
		public DateTime? ContextGroupVersion
		{
			get { return DateTimeParser.ParseDateAndTime(DicomAttributeProvider, DicomTags.ContextGroupVersion, 0, 0); }

			set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider, DicomTags.ContextGroupVersion, 0, 0); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the context group extension flag.  Y or N
		/// </summary>
		/// <value>The context group extension flag.</value>
		public string ContextGroupExtensionFlag
		{
			get { return DicomAttributeProvider[DicomTags.ContextGroupExtensionFlag].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.ContextGroupExtensionFlag].SetString(0, value); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the context group local version.
		/// </summary>
		/// <value>The context group local version.</value>
		public DateTime? ContextGroupLocalVersion
		{
			get { return DateTimeParser.ParseDateAndTime(DicomAttributeProvider, DicomTags.ContextGroupLocalVersion, 0, 0); }

			set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider, DicomTags.ContextGroupLocalVersion, 0, 0); }
		}

		/// <summary>
		/// Enhanced Encoding Mode: Gets or sets the context group extension creator uid.
		/// </summary>
		/// <value>The context group extension creator uid.</value>
		public string ContextGroupExtensionCreatorUid
		{
			get { return DicomAttributeProvider[DicomTags.ContextGroupExtensionCreatorUid].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.ContextGroupExtensionCreatorUid].SetString(0, value); }
		}

		#endregion
	}
}