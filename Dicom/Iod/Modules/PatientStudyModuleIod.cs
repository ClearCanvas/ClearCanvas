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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PatientStudy Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.2 (Table C.7.4-a)</remarks>
	public class PatientStudyModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientStudyModuleIod"/> class.
		/// </summary>	
		public PatientStudyModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientStudyModuleIod"/> class.
		/// </summary>
		public PatientStudyModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.AdditionalPatientHistory = null;
			this.AdmissionId = null;
			this.AdmittingDiagnosesCodeSequence = null;
			this.AdmittingDiagnosesDescription = null;
			this.IssuerOfAdmissionId = null;
			this.IssuerOfServiceEpisodeId = null;
			this.Occupation = null;
			this.PatientsAge = null;
			this.PatientsSexNeutered = null;
			this.PatientsSize = null;
			this.PatientsWeight = null;
			this.ServiceEpisodeDescription = null;
			this.ServiceEpisodeId = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (string.IsNullOrEmpty(this.AdditionalPatientHistory)
			    && string.IsNullOrEmpty(this.AdmissionId)
			    && this.AdmittingDiagnosesCodeSequence == null
			    && string.IsNullOrEmpty(this.AdmittingDiagnosesDescription)
			    && string.IsNullOrEmpty(this.IssuerOfAdmissionId)
			    && string.IsNullOrEmpty(this.IssuerOfServiceEpisodeId)
			    && string.IsNullOrEmpty(this.Occupation)
			    && string.IsNullOrEmpty(this.PatientsAge)
			    && (!this.PatientsSexNeutered.HasValue || this.PatientsSexNeutered == Iod.PatientsSexNeutered.Unknown)
			    && !this.PatientsSize.HasValue
			    && !this.PatientsWeight.HasValue
			    && string.IsNullOrEmpty(this.ServiceEpisodeDescription)
			    && string.IsNullOrEmpty(this.ServiceEpisodeId))
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of AdmittingDiagnosesDescription in the underlying collection. Type 3.
		/// </summary>
		public string AdmittingDiagnosesDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesDescription].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesDescription].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AdmittingDiagnosesCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro[] AdmittingDiagnosesCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				CodeSequenceMacro[] result = new CodeSequenceMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CodeSequenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesCodeSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.AdmittingDiagnosesCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsAge in the underlying collection. Type 3.
		/// </summary>
		public string PatientsAge
		{
			get { return base.DicomAttributeProvider[DicomTags.PatientsAge].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PatientsAge] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientsAge].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsSize in the underlying collection. Type 3.
		/// </summary>
		public double? PatientsSize
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[DicomTags.PatientsSize].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.PatientsSize] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientsSize].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsWeight in the underlying collection. Type 3.
		/// </summary>
		public double? PatientsWeight
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[DicomTags.PatientsWeight].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.PatientsWeight] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PatientsWeight].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Occupation in the underlying collection. Type 3.
		/// </summary>
		public string Occupation
		{
			get { return base.DicomAttributeProvider[DicomTags.Occupation].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.Occupation] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Occupation].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AdditionalPatientHistory in the underlying collection. Type 3.
		/// </summary>
		public string AdditionalPatientHistory
		{
			get { return base.DicomAttributeProvider[DicomTags.AdditionalPatientHistory].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.AdditionalPatientHistory] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.AdditionalPatientHistory].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AdmissionId in the underlying collection. Type 3.
		/// </summary>
		public string AdmissionId
		{
			get { return base.DicomAttributeProvider[DicomTags.AdmissionId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.AdmissionId] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.AdmissionId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of IssuerOfAdmissionId (Retired) in the underlying collection. Type 3.
		/// </summary>
		public string IssuerOfAdmissionId
		{
			get { return base.DicomAttributeProvider[DicomTags.IssuerOfAdmissionIdRetired].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.IssuerOfAdmissionIdRetired] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.IssuerOfAdmissionIdRetired].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ServiceEpisodeId in the underlying collection. Type 3.
		/// </summary>
		public string ServiceEpisodeId
		{
			get { return base.DicomAttributeProvider[DicomTags.ServiceEpisodeId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ServiceEpisodeId] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ServiceEpisodeId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of IssuerOfServiceEpisodeId in the underlying collection. Type 3.
		/// </summary>
		public string IssuerOfServiceEpisodeId
		{
			get { return base.DicomAttributeProvider[DicomTags.IssuerOfServiceEpisodeIdRetired].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
                    base.DicomAttributeProvider[DicomTags.IssuerOfServiceEpisodeIdRetired] = null;
					return;
				}
                base.DicomAttributeProvider[DicomTags.IssuerOfServiceEpisodeIdRetired].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ServiceEpisodeDescription in the underlying collection. Type 3.
		/// </summary>
		public string ServiceEpisodeDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.ServiceEpisodeDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.ServiceEpisodeDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ServiceEpisodeDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientsSexNeutered in the underlying collection. Type 2C.
		/// </summary>
		public PatientsSexNeutered? PatientsSexNeutered
		{
			get
			{
				if (base.DicomAttributeProvider[DicomTags.PatientsSexNeutered].IsEmpty)
					return null;
				return ParseEnum(base.DicomAttributeProvider[DicomTags.PatientsSexNeutered].GetString(0, string.Empty), Iod.PatientsSexNeutered.Unknown);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.PatientsSexNeutered] = null;
					return;
				}
				if (value == Iod.PatientsSexNeutered.Unknown)
				{
					base.DicomAttributeProvider[DicomTags.PatientsSexNeutered].SetNullValue();
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PatientsSexNeutered], value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.AdditionalPatientHistory;
				yield return DicomTags.AdmissionId;
				yield return DicomTags.AdmittingDiagnosesCodeSequence;
				yield return DicomTags.AdmittingDiagnosesDescription;
				yield return DicomTags.IssuerOfAdmissionIdRetired;
				yield return DicomTags.IssuerOfServiceEpisodeIdRetired;
				yield return DicomTags.Occupation;
				yield return DicomTags.PatientsAge;
				yield return DicomTags.PatientsSexNeutered;
				yield return DicomTags.PatientsSize;
				yield return DicomTags.PatientsWeight;
				yield return DicomTags.ServiceEpisodeDescription;
				yield return DicomTags.ServiceEpisodeId;
			}
		}
	}
}