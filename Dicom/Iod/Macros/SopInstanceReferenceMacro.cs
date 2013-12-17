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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// SOP Instance Reference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section 10.8 (Table 10-11)</remarks>
	public interface ISopInstanceReferenceMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of ReferencedSopClassUid in the underlying collection. Type 1.
		/// </summary>
		string ReferencedSopClassUid { get; set; }

		/// <summary>
		/// Gets or sets the value of ReferencedSopInstanceUid in the underlying collection. Type 1.
		/// </summary>
		string ReferencedSopInstanceUid { get; set; }
	}

	/// <summary>
	/// SOP Instance Reference Macro Base Implementation
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section 10.8 (Table 10-11)</remarks>
	public class SopInstanceReferenceMacro : SequenceIodBase, ISopInstanceReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SopInstanceReferenceMacro"/> class.
		/// </summary>
		public SopInstanceReferenceMacro() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SopInstanceReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public SopInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.ReferencedSopClassUid;
				yield return DicomTags.ReferencedSopInstanceUid;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module using default values.
		/// </summary>
		public virtual void InitializeAttributes()
		{
			ReferencedSopClassUid = "1";
			ReferencedSopInstanceUid = "1";
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopClassUid in the underlying collection. Type 1.
		/// </summary>
		public string ReferencedSopClassUid
		{
			get { return DicomAttributeProvider[DicomTags.ReferencedSopClassUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ReferencedSopClassUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ReferencedSopClassUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string ReferencedSopInstanceUid
		{
			get { return DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ReferencedSopInstanceUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].SetString(0, value);
			}
		}
	}
}