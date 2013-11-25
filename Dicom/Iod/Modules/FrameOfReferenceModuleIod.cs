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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Frame of Reference Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.4.1 (Table C.7-6)</remarks>
	public class FrameOfReferenceModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameOfReferenceModuleIod"/> class.
		/// </summary>	
		public FrameOfReferenceModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameOfReferenceModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public FrameOfReferenceModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.FrameOfReferenceUid;
				yield return DicomTags.PositionReferenceIndicator;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			PositionReferenceIndicator = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(FrameOfReferenceUid) && IsNullOrEmpty(PositionReferenceIndicator));
		}

		/// <summary>
		/// Gets or sets the value of FrameOfReferenceUid in the underlying collection. Type 1.
		/// </summary>
		public string FrameOfReferenceUid
		{
			get { return DicomAttributeProvider[DicomTags.FrameOfReferenceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FrameOfReferenceUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FrameOfReferenceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PositionReferenceIndicator in the underlying collection. Type 2.
		/// </summary>
		public string PositionReferenceIndicator
		{
			get { return DicomAttributeProvider[DicomTags.PositionReferenceIndicator].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.PositionReferenceIndicator].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.PositionReferenceIndicator].SetString(0, value);
			}
		}
	}
}