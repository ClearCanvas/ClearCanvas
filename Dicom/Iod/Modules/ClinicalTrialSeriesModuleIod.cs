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
	/// ClinicalTrialSeries Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.3.2 (Table C.7-5b)</remarks>
	public class ClinicalTrialSeriesModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialSeriesModuleIod"/> class.
		/// </summary>	
		public ClinicalTrialSeriesModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClinicalTrialSeriesModuleIod"/> class.
		/// </summary>
		public ClinicalTrialSeriesModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.ClinicalTrialCoordinatingCenterName = null;
			this.ClinicalTrialSeriesId = null;
			this.ClinicalTrialSeriesDescription = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (string.IsNullOrEmpty(this.ClinicalTrialCoordinatingCenterName)
			    && string.IsNullOrEmpty(this.ClinicalTrialSeriesId)
			    && string.IsNullOrEmpty(this.ClinicalTrialSeriesDescription))
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialCoordinatingCenterName in the underlying collection. Type 2.
		/// </summary>
		public string ClinicalTrialCoordinatingCenterName
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialCoordinatingCenterName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialCoordinatingCenterName].SetNullValue();
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialCoordinatingCenterName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialSeriesId in the underlying collection. Type 3.
		/// </summary>
		public string ClinicalTrialSeriesId
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesId] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ClinicalTrialSeriesDescription in the underlying collection. Type 3.
		/// </summary>
		public string ClinicalTrialSeriesDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ClinicalTrialSeriesDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ClinicalTrialCoordinatingCenterName;
				yield return DicomTags.ClinicalTrialSeriesDescription;
				yield return DicomTags.ClinicalTrialSeriesId;
			}
		}
	}
}