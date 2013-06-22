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
    public class RawDataIod
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
		private readonly AcquisitionContextModuleIod _acquisitionContextModule;
        private readonly RawDataModule _rawDataModule;
		private readonly SopCommonModuleIod _sopCommonModule;

		public RawDataIod() : this(new DicomAttributeCollection()) {}

        public RawDataIod(IDicomAttributeProvider dicomAttributeProvider)
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
            _acquisitionContextModule = new AcquisitionContextModuleIod(_dicomAttributeProvider);
            _rawDataModule = new RawDataModule(_dicomAttributeProvider);
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

		public GeneralSeriesModuleIod GeneralSeries
		{
			get { return _generalSeriesModule; }
		}

		public ClinicalTrialSeriesModuleIod ClinicalTrialSeries
		{
			get { return _clinicalTrialSeriesModule; }
		}

        // TODO Frame of Reference Module C.7.4.1
        // TODO Synchronization Module C.7.4.2

		public GeneralEquipmentModuleIod GeneralEquipment
		{
			get { return _generalEquipmentModule; }
		}

        public AcquisitionContextModuleIod AcquisitionContext
        {
            get { return _acquisitionContextModule; }
        }

        //  TODO Specimen Module (C.7.6.22)

		public RawDataModule RawData
		{
			get { return _rawDataModule; }
		}

		public SopCommonModuleIod SopCommon
		{
			get { return _sopCommonModule; }
		}
    }
}
