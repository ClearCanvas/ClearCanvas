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

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Content Template Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.8 (Table C.18.8-1)</remarks>
	public class ContentTemplateSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentTemplateSequence"/> class.
		/// </summary>
		public ContentTemplateSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentTemplateSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ContentTemplateSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		public void InitializeAttributes()
		{
			this.MappingResource = "DCMR";
			this.TemplateIdentifier = "1";
		}

		/// <summary>
		/// Gets or sets the value of MappingResource in the underlying collection. Type 1.
		/// </summary>
		public string MappingResource
		{
			get { return base.DicomAttributeProvider[DicomTags.MappingResource].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "MappingResource is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.MappingResource].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of TemplateIdentifier in the underlying collection. Type 1.
		/// </summary>
		public string TemplateIdentifier
		{
			get { return base.DicomAttributeProvider[DicomTags.TemplateIdentifier].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "Template Identifier is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.TemplateIdentifier].SetString(0, value);
			}
		}
	}
}