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

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
	/// <summary>
	/// Encapsulated PDF IOD
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section A.45.1 (Table A.45.1-1)</para>
	/// </remarks>
	public class EncapsulatedPdfIod
	{
		private readonly IDicomAttributeProvider _dicomAttributeProvider;
		private readonly PatientModuleIod _patientModule;
		private readonly ClinicalTrialSubjectModuleIod _clinicalTrialSubjectModule;
		private readonly GeneralStudyModuleIod _generalStudyModule;
		private readonly PatientStudyModuleIod _patientStudyModule;
		private readonly ClinicalTrialStudyModuleIod _clinicalTrialStudyModule;
		private readonly EncapsulatedDocumentSeriesModuleIod _encapsulatedDocumentSeriesModule;
		private readonly ClinicalTrialSeriesModuleIod _clinicalTrialSeriesModule;
		private readonly GeneralEquipmentModuleIod _generalEquipmentModule;
		private readonly ScEquipmentModuleIod _scEquipmentModule;
		private readonly EncapsulatedDocumentModuleIod _encapsulatedDocumentModule;
		private readonly SopCommonModuleIod _sopCommonModule;

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedPdfIod"/> class.
		/// </summary>	
		public EncapsulatedPdfIod()
			: this(new DicomAttributeCollection()) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedPdfIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public EncapsulatedPdfIod(IDicomAttributeProvider dicomAttributeProvider)
		{
			_dicomAttributeProvider = dicomAttributeProvider;

			_patientModule = new PatientModuleIod(_dicomAttributeProvider);
			_clinicalTrialSubjectModule = new ClinicalTrialSubjectModuleIod(_dicomAttributeProvider);
			_generalStudyModule = new GeneralStudyModuleIod(_dicomAttributeProvider);
			_patientStudyModule = new PatientStudyModuleIod(_dicomAttributeProvider);
			_clinicalTrialStudyModule = new ClinicalTrialStudyModuleIod(_dicomAttributeProvider);
			_encapsulatedDocumentSeriesModule = new EncapsulatedDocumentSeriesModuleIod(_dicomAttributeProvider);
			_clinicalTrialSeriesModule = new ClinicalTrialSeriesModuleIod(_dicomAttributeProvider);
			_generalEquipmentModule = new GeneralEquipmentModuleIod(_dicomAttributeProvider);
			_scEquipmentModule = new ScEquipmentModuleIod(_dicomAttributeProvider);
			_encapsulatedDocumentModule = new EncapsulatedDocumentModuleIod(_dicomAttributeProvider);
			_sopCommonModule = new SopCommonModuleIod(_dicomAttributeProvider);
		}

		public PatientModuleIod Patient
		{
			get { return _patientModule; }
		}

		public ClinicalTrialSubjectModuleIod ClinicalTrialSubject
		{
			get { return _clinicalTrialSubjectModule; }
		}

		public GeneralStudyModuleIod GeneralStudy
		{
			get { return _generalStudyModule; }
		}

		public PatientStudyModuleIod PatientStudy
		{
			get { return _patientStudyModule; }
		}

		public ClinicalTrialStudyModuleIod ClinicalTrialStudy
		{
			get { return _clinicalTrialStudyModule; }
		}

		public EncapsulatedDocumentSeriesModuleIod EncapsulatedDocumentSeries
		{
			get { return _encapsulatedDocumentSeriesModule; }
		}

		public ClinicalTrialSeriesModuleIod ClinicalTrialSeries
		{
			get { return _clinicalTrialSeriesModule; }
		}

		public GeneralEquipmentModuleIod GeneralEquipment
		{
			get { return _generalEquipmentModule; }
		}

		public ScEquipmentModuleIod ScEquipment
		{
			get { return _scEquipmentModule; }
		}

		public EncapsulatedDocumentModuleIod EncapsulatedDocument
		{
			get { return _encapsulatedDocumentModule; }
		}

		public SopCommonModuleIod SopCommon
		{
			get { return _sopCommonModule; }
		}

		public bool HasClinicalTrialSubjectModule { get; set; }

		public bool HasPatientStudyModule { get; set; }

		public bool HasClinicalTrialStudyModule { get; set; }

		public bool HasClinicalTrialSeriesModule { get; set; }
	}
}