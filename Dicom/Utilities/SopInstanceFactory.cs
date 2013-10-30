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

using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Utilities
{
	/// <summary>
	/// Base implementation for a factory of composite SOP instances.
	/// </summary>
	public abstract class SopInstanceFactory
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopInstanceFactory"/>.
		/// </summary>
		protected SopInstanceFactory()
		{
			Manufacturer = @"ClearCanvas Inc.";
			ManufacturersModelName = ProductInformation.GetName(true, false);
			DeviceSerialNumber = string.Empty;
			SoftwareVersions = ProductInformation.GetVersion(true, true, true);
			SpecificCharacterSet = @"ISO_IR 192";
		}

		/// <summary>
		/// Gets or sets the user defined name identifying the machine that produced the composite instances.
		/// </summary>
		public string StationName { get; set; }

		/// <summary>
		/// Gets or sets the institution where the equipment that produced the composite instances is located.
		/// </summary>
		public Institution Institution { get; set; }

		/// <summary>
		/// Gets the manufacturer of the equipment that produced the composite instances.
		/// </summary>
		public string Manufacturer { get; protected set; }

		/// <summary>
		/// Gets the manufacturer's model name of the equipment that produced the composite instances.
		/// </summary>
		public string ManufacturersModelName { get; protected set; }

		/// <summary>
		/// Gets the manufacturer's serial number of the equipment that produced the composite instances.
		/// </summary>
		public string DeviceSerialNumber { get; protected set; }

		/// <summary>
		/// Gets the manufacturer's designation of software version of the equipment that produced the composite instances.
		/// </summary>
		public string SoftwareVersions { get; protected set; }

		/// <summary>
		/// Gets or sets the DICOM specific character set to be used when encoding SOP instances.
		/// </summary>
		/// <remarks>
		/// By default, text attribute values will be encoded using UTF-8 Unicode (ISO-IR 192).
		/// If set to NULL or empty, values will be encoded using the default character repertoire (ISO-IR 6).
		/// </remarks>
		public string SpecificCharacterSet { get; set; }

		/// <summary>
		/// Fills basic attributes in the General Equipment module.
		/// </summary>
		/// <param name="target">The destination attribute collection whose attributes are to be updated.</param>
		protected void FillGeneralEquipmentModule(IDicomAttributeProvider target)
		{
			var targetGeneralEquipment = new GeneralEquipmentModuleIod(target);
			targetGeneralEquipment.InitializeAttributes();
			targetGeneralEquipment.Manufacturer = Manufacturer ?? string.Empty; // Manufacturer here is Type 2
			targetGeneralEquipment.InstitutionName = Institution.Name;
			targetGeneralEquipment.InstitutionAddress = Institution.Address;
			targetGeneralEquipment.StationName = StationName;
			targetGeneralEquipment.InstitutionalDepartmentName = Institution.DepartmentName;
			targetGeneralEquipment.ManufacturersModelName = ManufacturersModelName;
			targetGeneralEquipment.DeviceSerialNumber = DeviceSerialNumber;
			targetGeneralEquipment.SoftwareVersions = SoftwareVersions;
		}

		/// <summary>
		/// Creates a prototype of a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a new composite SOP instance with certain attributes initialized based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <returns>A <see cref="DicomFile"/> whose data set has been initialized with the attributes of common modules.</returns>
		protected DicomFile CreatePrototypeFile(IDicomAttributeProvider source)
		{
			var prototypeFile = new DicomFile();
			InitializePrototypeDataSet(source, prototypeFile.DataSet);
			return prototypeFile;
		}

		/// <summary>
		/// Creates a prototype of a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a new composite SOP instance with certain attributes initialized based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <returns>A <see cref="DicomAttributeCollection"/> whose data set has been initialized with the attributes of common modules.</returns>
		protected DicomAttributeCollection CreatePrototypeAttributeCollection(IDicomAttributeProvider source)
		{
			var dataSet = new DicomAttributeCollection();
			InitializePrototypeDataSet(source, dataSet);
			return dataSet;
		}

		/// <summary>
		/// Initializes a dataset with attributes for a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method initializes a data set with certain attributes based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <param name="target">The destination data set to be initialized with the attributes of common modules.</param>
		protected void InitializePrototypeDataSet(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			// specific character set must be copied first before any other attributes are set!
			var specificCharacterSet = SpecificCharacterSet ?? string.Empty;
			var dicomAttributeCollection = target as DicomAttributeCollection;
			if (dicomAttributeCollection != null) dicomAttributeCollection.SpecificCharacterSet = specificCharacterSet;
			target[DicomTags.SpecificCharacterSet].SetStringValue(specificCharacterSet);

			// patient IE
			CopyPatientModule(source, target);
			CopyClinicalTrialSubjectModule(source, target);

			// study IE
			CopyGeneralStudyModule(source, target);
			CopyPatientStudyModule(source, target);
			CopyClinicalTrialStudyModule(source, target);

			// equipment IE
			FillGeneralEquipmentModule(target);
		}

		protected static bool CopyPatientModule(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			var sourcePatient = new PatientModuleIod(source);
			if (!sourcePatient.HasValues())
				return false;

			var targetPatient = new PatientModuleIod(target);
			targetPatient.BreedRegistrationSequence = sourcePatient.BreedRegistrationSequence;
			targetPatient.DeIdentificationMethod = sourcePatient.DeIdentificationMethod;
			targetPatient.DeIdentificationMethodCodeSequence = sourcePatient.DeIdentificationMethodCodeSequence;
			targetPatient.EthnicGroup = sourcePatient.EthnicGroup;
			targetPatient.IssuerOfPatientId = sourcePatient.IssuerOfPatientId;
			targetPatient.OtherPatientIds = sourcePatient.OtherPatientIds;
			targetPatient.OtherPatientIdsSequence = sourcePatient.OtherPatientIdsSequence;
			targetPatient.OtherPatientNames = sourcePatient.OtherPatientNames;
			targetPatient.PatientBreedCodeSequence = sourcePatient.PatientBreedCodeSequence;
			targetPatient.PatientBreedDescription = sourcePatient.PatientBreedDescription;
			targetPatient.PatientComments = sourcePatient.PatientComments;
			targetPatient.PatientId = sourcePatient.PatientId;
			targetPatient.PatientIdentityRemoved = sourcePatient.PatientIdentityRemoved;
			targetPatient.PatientsBirthDateTime = sourcePatient.PatientsBirthDateTime;
			targetPatient.PatientsName = sourcePatient.PatientsName;
			targetPatient.PatientSpeciesCodeSequence = sourcePatient.PatientSpeciesCodeSequence;
			targetPatient.PatientSpeciesDescription = sourcePatient.PatientSpeciesDescription;
			targetPatient.PatientsSex = sourcePatient.PatientsSex;
			targetPatient.ReferencedPatientSequence = sourcePatient.ReferencedPatientSequence;
			targetPatient.ResponsibleOrganization = sourcePatient.ResponsibleOrganization;
			targetPatient.ResponsiblePerson = sourcePatient.ResponsiblePerson;
			targetPatient.ResponsiblePersonRole = sourcePatient.ResponsiblePersonRole;
			return true;
		}

		protected static bool CopyClinicalTrialSubjectModule(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			var sourceTrialSubject = new ClinicalTrialSubjectModuleIod(source);
			if (!sourceTrialSubject.HasValues())
				return false;

			var targetTrialSubject = new ClinicalTrialSubjectModuleIod(target);
			targetTrialSubject.ClinicalTrialProtocolId = sourceTrialSubject.ClinicalTrialProtocolId;
			targetTrialSubject.ClinicalTrialProtocolName = sourceTrialSubject.ClinicalTrialProtocolName;
			targetTrialSubject.ClinicalTrialSiteId = sourceTrialSubject.ClinicalTrialSiteId;
			targetTrialSubject.ClinicalTrialSiteName = sourceTrialSubject.ClinicalTrialSiteName;
			targetTrialSubject.ClinicalTrialSponsorName = sourceTrialSubject.ClinicalTrialSponsorName;
			targetTrialSubject.ClinicalTrialSubjectId = sourceTrialSubject.ClinicalTrialSubjectId;
			targetTrialSubject.ClinicalTrialSubjectReadingId = sourceTrialSubject.ClinicalTrialSubjectReadingId;
			return true;
		}

		protected static bool CopyGeneralStudyModule(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			var sourceGeneralStudy = new GeneralStudyModuleIod(source);
			if (!sourceGeneralStudy.HasValues())
				return false;

			var targetGeneralStudy = new GeneralStudyModuleIod(target);
			targetGeneralStudy.AccessionNumber = sourceGeneralStudy.AccessionNumber;
			targetGeneralStudy.NameOfPhysiciansReadingStudy = sourceGeneralStudy.NameOfPhysiciansReadingStudy;
			targetGeneralStudy.PhysiciansOfRecord = sourceGeneralStudy.PhysiciansOfRecord;
			targetGeneralStudy.PhysiciansOfRecordIdentificationSequence = sourceGeneralStudy.PhysiciansOfRecordIdentificationSequence;
			targetGeneralStudy.PhysiciansReadingStudyIdentificationSequence = sourceGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
			targetGeneralStudy.ProcedureCodeSequence = sourceGeneralStudy.ProcedureCodeSequence;
			targetGeneralStudy.ReferencedStudySequence = sourceGeneralStudy.ReferencedStudySequence;
			targetGeneralStudy.ReferringPhysicianIdentificationSequence = sourceGeneralStudy.ReferringPhysicianIdentificationSequence;
			targetGeneralStudy.ReferringPhysiciansName = sourceGeneralStudy.ReferringPhysiciansName;
			targetGeneralStudy.StudyDateTime = sourceGeneralStudy.StudyDateTime;
			targetGeneralStudy.StudyDescription = sourceGeneralStudy.StudyDescription;
			targetGeneralStudy.StudyId = sourceGeneralStudy.StudyId;
			targetGeneralStudy.StudyInstanceUid = sourceGeneralStudy.StudyInstanceUid;
			return true;
		}

		protected static bool CopyPatientStudyModule(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			var sourcePatientStudy = new PatientStudyModuleIod(source);
			if (!sourcePatientStudy.HasValues())
				return false;

			var targetPatientStudy = new PatientStudyModuleIod(target);
			targetPatientStudy.AdditionalPatientHistory = sourcePatientStudy.AdditionalPatientHistory;
			targetPatientStudy.AdmissionId = sourcePatientStudy.AdmissionId;
			targetPatientStudy.AdmittingDiagnosesCodeSequence = sourcePatientStudy.AdmittingDiagnosesCodeSequence;
			targetPatientStudy.AdmittingDiagnosesDescription = sourcePatientStudy.AdmittingDiagnosesDescription;
			targetPatientStudy.IssuerOfAdmissionId = sourcePatientStudy.IssuerOfAdmissionId;
			targetPatientStudy.IssuerOfServiceEpisodeId = sourcePatientStudy.IssuerOfServiceEpisodeId;
			targetPatientStudy.Occupation = sourcePatientStudy.Occupation;
			targetPatientStudy.PatientsAge = sourcePatientStudy.PatientsAge;
			targetPatientStudy.PatientsSexNeutered = sourcePatientStudy.PatientsSexNeutered;
			targetPatientStudy.PatientsSize = sourcePatientStudy.PatientsSize;
			targetPatientStudy.PatientsWeight = sourcePatientStudy.PatientsWeight;
			targetPatientStudy.ServiceEpisodeDescription = sourcePatientStudy.ServiceEpisodeDescription;
			targetPatientStudy.ServiceEpisodeId = sourcePatientStudy.ServiceEpisodeId;
			return true;
		}

		protected static bool CopyClinicalTrialStudyModule(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			var sourceTrialStudy = new ClinicalTrialStudyModuleIod(source);
			if (!sourceTrialStudy.HasValues())
				return false;

			var targetTrialStudy = new ClinicalTrialStudyModuleIod(target);
			targetTrialStudy.ClinicalTrialTimePointDescription = sourceTrialStudy.ClinicalTrialTimePointDescription;
			targetTrialStudy.ClinicalTrialTimePointId = sourceTrialStudy.ClinicalTrialTimePointId;
			return true;
		}
	}

	#region PrototypeSopInstanceFactory

	/// <summary>
	/// An implementation of <see cref="SopInstanceFactory"/> that creates only empty prototype SOP instances.
	/// </summary>
	public sealed class PrototypeSopInstanceFactory : SopInstanceFactory
	{
		/// <summary>
		/// Creates a prototype of a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a new composite SOP instance with certain attributes initialized based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <returns>A <see cref="DicomFile"/> whose data set has been initialized with the attributes of common modules.</returns>
		public DicomFile CreateFile(IDicomAttributeProvider source)
		{
			return CreatePrototypeFile(source);
		}

		/// <summary>
		/// Creates a prototype of a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a new composite SOP instance with certain attributes initialized based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <returns>A <see cref="DicomAttributeCollection"/> whose data set has been initialized with the attributes of common modules.</returns>
		public DicomAttributeCollection CreateAttributeCollection(IDicomAttributeProvider source)
		{
			return CreatePrototypeAttributeCollection(source);
		}

		/// <summary>
		/// Initializes a dataset with attributes for a new composite SOP instance in the same study as the specified source.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method initializes a data set with certain attributes based on the specified
		/// source SOP instance and details of the software application and institution creating the instance.
		/// </para>
		/// <para>
		/// The following modules are copied verbatim from the source SOP instance:
		/// </para>
		/// <list type="bullet">
		/// <item>Patient Module (C.7.1.1)</item>
		/// <item>Clinical Trial Subject Module (C.7.1.3)</item>
		/// <item>General Study Module (C.7.2.1)</item>
		/// <item>Patient Study Module (C.7.2.2)</item>
		/// <item>Clinical Trial Study Module (C.7.2.3)</item>
		/// </list>
		/// <para>
		/// Additionally, certain attributes of the General Equipment Module (C.7.5.1) are automatically filled in with values
		/// to identify the creating software application and institution, and the Specific Character Set (0008,0005) is copied
		/// from the source SOP instance to ensure that copied attributes remain consistent.
		/// </para>
		/// </remarks>
		/// <param name="source">An existing SOP instance whose attributes are used as a template to creating the new composite SOP instance.</param>
		/// <param name="target">The destination data set to be initialized with the attributes of common modules.</param>
		public void InitializeDataSet(IDicomAttributeProvider source, IDicomAttributeProvider target)
		{
			InitializePrototypeDataSet(source, target);
		}
	}

	#endregion
}