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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// ClinicalTrialStudy Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.3 (Table C.7-4b)</remarks>
	public class ClinicalTrialStudyModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialStudyModuleIod"/> class.
		/// </summary>	
		public ClinicalTrialStudyModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialStudyModuleIod"/> class.
		/// </summary>
		public ClinicalTrialStudyModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.ClinicalTrialTimePointId = null;
			this.ClinicalTrialTimePointDescription = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (string.IsNullOrEmpty(this.ClinicalTrialTimePointId) && string.IsNullOrEmpty(this.ClinicalTrialTimePointDescription))
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialTimePointId in the underlying collection. Type 2.
		/// </summary>
		public string ClinicalTrialTimePointId
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialTimePointDescription in the underlying collection. Type 3.
		/// </summary>
		public string ClinicalTrialTimePointDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialTimePointDescription].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ClinicalTrialTimePointDescription;
				yield return DicomTags.ClinicalTrialTimePointId;
			}
		}
	}
}