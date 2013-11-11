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
	/// SC Image IOD
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2011, Part 3, Section A.8.1.3 (Table A.8-1)</para>
	/// </remarks>
	public class ScImageIod
	{
		private readonly IDicomAttributeProvider _dicomAttributeProvider;
		private readonly PatientModuleIod _patientModule;
		private readonly ClinicalTrialSubjectModuleIod _clinicalTrialSubjectModule;
		private readonly GeneralStudyModuleIod _generalStudyModule;
		private readonly PatientStudyModuleIod _patientStudyModule;
		private readonly ClinicalTrialStudyModuleIod _clinicalTrialStudyModule;
		private readonly GeneralSeriesModuleIod _generalSeriesModule;
		private readonly ClinicalTrialSeriesModuleIod _clinicalTrialSeriesModule;
		private readonly GeneralEquipmentModuleIod _generalEquipmentModule;
		private readonly ScEquipmentModuleIod _scEquipmentModule;
		private readonly GeneralImageModuleIod _generalImageModule;
		private readonly ImagePixelMacroIod _imagePixelModule;
		private readonly DeviceModuleIod _deviceModule;
		// Specimen not implemented
		private readonly ScImageModuleIod _scImageModule;
		private readonly OverlayPlaneModuleIod _overlayPlaneModule;
		private readonly ModalityLutModuleIod _modalityLutModule;
		private readonly VoiLutModuleIod _voiLutModule;
		private readonly IccProfileModuleIod _iccProfileModule;
		private readonly SopCommonModuleIod _sopCommonModule;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScImageIod"/> class.
		/// </summary>	
		public ScImageIod()
			: this(new DicomAttributeCollection()) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScImageIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public ScImageIod(IDicomAttributeProvider dicomAttributeProvider)
		{
			_dicomAttributeProvider = dicomAttributeProvider;

			_patientModule = new PatientModuleIod(_dicomAttributeProvider);
			_clinicalTrialSubjectModule = new ClinicalTrialSubjectModuleIod(_dicomAttributeProvider);
			_generalStudyModule = new GeneralStudyModuleIod(_dicomAttributeProvider);
			_patientStudyModule = new PatientStudyModuleIod(_dicomAttributeProvider);
			_clinicalTrialStudyModule = new ClinicalTrialStudyModuleIod(_dicomAttributeProvider);
			_generalSeriesModule = new GeneralSeriesModuleIod(_dicomAttributeProvider);
			_clinicalTrialSeriesModule = new ClinicalTrialSeriesModuleIod(_dicomAttributeProvider);
			_generalEquipmentModule = new GeneralEquipmentModuleIod(_dicomAttributeProvider);
			_scEquipmentModule = new ScEquipmentModuleIod(_dicomAttributeProvider);
			_generalImageModule = new GeneralImageModuleIod(_dicomAttributeProvider);
			_imagePixelModule = new ImagePixelMacroIod(_dicomAttributeProvider);
			_deviceModule = new DeviceModuleIod(_dicomAttributeProvider);
			_scImageModule = new ScImageModuleIod(_dicomAttributeProvider);
			_overlayPlaneModule = new OverlayPlaneModuleIod(_dicomAttributeProvider);
			_modalityLutModule = new ModalityLutModuleIod(_dicomAttributeProvider);
			_voiLutModule = new VoiLutModuleIod(_dicomAttributeProvider);
			_iccProfileModule = new IccProfileModuleIod(_dicomAttributeProvider);
			_sopCommonModule = new SopCommonModuleIod(_dicomAttributeProvider);
		}

		/// <summary>
		/// Gets the Patient module (mandatory).
		/// </summary>
		public PatientModuleIod Patient
		{
			get { return _patientModule; }
		}

		/// <summary>
		/// Gets the Clinical Trial Subject module (optional).
		/// </summary>
		public ClinicalTrialSubjectModuleIod ClinicalTrialSubject
		{
			get { return _clinicalTrialSubjectModule; }
		}

		/// <summary>
		/// Gets the General Study module (mandatory).
		/// </summary>
		public GeneralStudyModuleIod GeneralStudy
		{
			get { return _generalStudyModule; }
		}

		/// <summary>
		/// Gets the Patient Study module (optional).
		/// </summary>
		public PatientStudyModuleIod PatientStudy
		{
			get { return _patientStudyModule; }
		}

		/// <summary>
		/// Gets the Clinical Trial Study module (optional).
		/// </summary>
		public ClinicalTrialStudyModuleIod ClinicalTrialStudy
		{
			get { return _clinicalTrialStudyModule; }
		}

		/// <summary>
		/// Gets the General Series module (mandatory).
		/// </summary>
		public GeneralSeriesModuleIod GeneralSeriesModule
		{
			get { return _generalSeriesModule; }
		}

		/// <summary>
		/// Gets the Clinical Trial Series module (optional).
		/// </summary>
		public ClinicalTrialSeriesModuleIod ClinicalTrialSeries
		{
			get { return _clinicalTrialSeriesModule; }
		}

		/// <summary>
		/// Gets the General Equipment module (optional).
		/// </summary>
		public GeneralEquipmentModuleIod GeneralEquipment
		{
			get { return _generalEquipmentModule; }
		}

		/// <summary>
		/// Gets the SC Equipment module (mandatory).
		/// </summary>
		public ScEquipmentModuleIod ScEquipment
		{
			get { return _scEquipmentModule; }
		}

		/// <summary>
		/// Gets the General Image module (mandatory).
		/// </summary>
		public GeneralImageModuleIod GeneralImage
		{
			get { return _generalImageModule; }
		}

		/// <summary>
		/// Gets the Image Pixel module (mandatory).
		/// </summary>
		public ImagePixelMacroIod ImagePixel
		{
			get { return _imagePixelModule; }
		}

		/// <summary>
		/// Gets the Device module (optional).
		/// </summary>
		public DeviceModuleIod Device
		{
			get { return _deviceModule; }
		}

		// TODO CR (11 Nov 2013): Specimen module

		/// <summary>
		/// Gets the SC Image module (mandatory).
		/// </summary>
		public ScImageModuleIod ScImage
		{
			get { return _scImageModule; }
		}

		/// <summary>
		/// Gets the Overlay Plane module (optional).
		/// </summary>
		public OverlayPlaneModuleIod OverlayPlane
		{
			get { return _overlayPlaneModule; }
		}

		/// <summary>
		/// Gets the Modality LUT module (optional).
		/// </summary>
		public ModalityLutModuleIod ModalityLut
		{
			get { return _modalityLutModule; }
		}

		/// <summary>
		/// Gets the VOI LUT module (optional).
		/// </summary>
		public VoiLutModuleIod VoiLut
		{
			get { return _voiLutModule; }
		}

		/// <summary>
		/// Gets the ICC Profile module (optional).
		/// </summary>
		public IccProfileModuleIod IccProfile
		{
			get { return _iccProfileModule; }
		}

		/// <summary>
		/// Gets the SOP Common module (mandatory).
		/// </summary>
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