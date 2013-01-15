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
	/// ReferencedDigitalSignature Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
	public class ReferencedDigitalSignatureSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedDigitalSignatureSequence"/> class.
		/// </summary>
		public ReferencedDigitalSignatureSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedDigitalSignatureSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ReferencedDigitalSignatureSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DigitalSignatureUid in the underlying collection. Type 1.
		/// </summary>
		public string DigitalSignatureUid
		{
			get { return base.DicomAttributeProvider[DicomTags.DigitalSignatureUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "DigitalSignatureUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.DigitalSignatureUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Signature in the underlying collection. Type 1.
		/// </summary>
		public byte[] Signature
		{
			get { return (byte[]) base.DicomAttributeProvider[DicomTags.Signature].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "Signature is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.Signature].Values = value;
			}
		}
	}
}