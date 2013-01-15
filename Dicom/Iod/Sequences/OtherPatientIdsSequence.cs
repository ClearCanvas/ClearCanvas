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
	/// OtherPatientIds Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public class OtherPatientIdsSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OtherPatientIdsSequence"/> class.
		/// </summary>
		public OtherPatientIdsSequence() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OtherPatientIdsSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public OtherPatientIdsSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes() {
			this.PatientId = " ";
			this.IssuerOfPatientId = " ";
			this.TypeOfPatientId = TypeOfPatientId.Text;
		}

		/// <summary>
		/// Gets or sets the value of PatientId in the underlying collection. Type 1.
		/// </summary>
		public string PatientId
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "PatientId is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.PatientId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of IssuerOfPatientId in the underlying collection. Type 1.
		/// </summary>
		public string IssuerOfPatientId
		{
			get { return base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "IssuerOfPatientId is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of TypeOfPatientId in the underlying collection. Type 1.
		/// </summary>
		public TypeOfPatientId TypeOfPatientId
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.TypeOfPatientId].GetString(0, string.Empty), TypeOfPatientId.Unknown); }
			set
			{
				if (value == TypeOfPatientId.Unknown)
					throw new ArgumentOutOfRangeException("value", "TypeOfPatientId is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.TypeOfPatientId], value);
			}
		}
	}
}
