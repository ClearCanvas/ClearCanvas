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
	/// ReferencedSopInstanceMac Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.2.1 (Table C.17-3a)</remarks>
	public class ReferencedSopInstanceMacSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedSopInstanceMacSequence"/> class.
		/// </summary>
		public ReferencedSopInstanceMacSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedSopInstanceMacSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ReferencedSopInstanceMacSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of MacCalculationTransferSyntaxUid in the underlying collection. Type 1.
		/// </summary>
		public string MacCalculationTransferSyntaxUid
		{
			get { return base.DicomAttributeProvider[DicomTags.MacCalculationTransferSyntaxUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "MacCalculationTransferSyntaxUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.MacCalculationTransferSyntaxUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of MacAlgorithm in the underlying collection. Type 1.
		/// </summary>
		public MacAlgorithm MacAlgorithm
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.MacAlgorithm].GetString(0, string.Empty), MacAlgorithm.Unknown); }
			set
			{
				if (value == MacAlgorithm.Unknown)
					throw new ArgumentOutOfRangeException("value", "MacAlgorithm is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MacAlgorithm], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DataElementsSigned in the underlying collection. Type 1.
		/// </summary>
		public uint[] DataElementsSigned
		{
			get { return (uint[]) base.DicomAttributeProvider[DicomTags.DataElementsSigned].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "DataElementsSigned is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.DataElementsSigned].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of Mac in the underlying collection. Type 1.
		/// </summary>
		public byte[] Mac
		{
			get { return (byte[]) base.DicomAttributeProvider[DicomTags.Mac].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "Mac is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.Mac].Values = value;
			}
		}
	}
}