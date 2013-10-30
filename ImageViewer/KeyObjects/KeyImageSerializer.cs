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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// A class for serializing a key image series from a number of images with associated presentation states.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	// TODO CR (Oct 11): Refactor to use SopInstanceFactory
	public class KeyImageSerializer
	{
		private readonly FramePresentationList _framePresentationStates;
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private string _author;

		private string _stationName;
		private Institution _institution;
		private string _manufacturer;
		private string _manufacturersModelName;
		private string _deviceSerialNumber;
		private string _softwareVersions;
		private string _specificCharacterSet = @"ISO_IR 192";
		private KeyObjectSelectionDocumentTitle _docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

		/// <summary>
		/// Constructs a new instance of <see cref="KeyImageSerializer"/>.
		/// </summary>
		/// <remarks>
		/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
		/// </remarks>
		public KeyImageSerializer()
		{
			_framePresentationStates = new FramePresentationList();
			_datetime = Platform.Time;
			_author = GetUserName();

			_stationName = string.Empty;
			_institution = Institution.Empty;
			_manufacturer = "ClearCanvas";
			_manufacturersModelName = ProductInformation.Component;
			_deviceSerialNumber = string.Empty;
			_softwareVersions = ProductInformation.GetVersion(true, true);
		}

		/// <summary>
		/// Gets or sets the series date time to use for the key object selection document.
		/// </summary>
		public DateTime DateTime
		{
			get { return _datetime; }
			set { _datetime = value; }
		}

		/// <summary>
		/// Gets or sets the description of the key object selection.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		/// <summary>
		/// Gets or sets the KO document author.
		/// </summary>
		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		/// <summary>
		/// Gets or sets the key object selection document title.
		/// </summary>
		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

		/// <summary>
		/// Unused. The Source Application Entity Title of the serialized SOP instances is automatically filled from the referenced frames.
		/// </summary>
		[Obsolete("The Source Application Entity Title of the serialized SOP instances is automatically filled from the referenced frames.")]
		public string SourceAETitle
		{
			get { return string.Empty; }
			set { }
		}

		/// <summary>
		/// Gets or sets the instance creator's workstation name.
		/// </summary>
		public string StationName
		{
			get { return _stationName; }
			set { _stationName = value; }
		}

		/// <summary>
		/// Gets or sets the instance creator's institution.
		/// </summary>
		internal Institution Institution
		{
			get { return _institution; }
			set { _institution = value; }
		}

		/// <summary>
		/// Gets or sets the workstation's manufacturer.
		/// </summary>
		public string Manufacturer
		{
			get { return _manufacturer; }
			protected set { _manufacturer = value; }
		}

		/// <summary>
		/// Gets or sets the workstation's model name.
		/// </summary>
		public string ManufacturersModelName
		{
			get { return _manufacturersModelName; }
			protected set { _manufacturersModelName = value; }
		}

		/// <summary>
		/// Gets or sets the workstation's serial number.
		/// </summary>
		public string DeviceSerialNumber
		{
			get { return _deviceSerialNumber; }
			protected set { _deviceSerialNumber = value; }
		}

		/// <summary>
		/// Gets or sets the workstation's software version numbers (backslash-delimited for multiple values).
		/// </summary>
		public string SoftwareVersions
		{
			get { return _softwareVersions; }
			protected set { _softwareVersions = value; }
		}

		/// <summary>
		/// Gets or sets the DICOM specific character set to be used when encoding SOP instances.
		/// </summary>
		/// <remarks>
		/// By default, text attribute values will be encoded using UTF-8 Unicode (ISO-IR 192).
		/// If set to NULL or empty, values will be encoded using the default character repertoire (ISO-IR 6).
		/// </remarks>
		public string SpecificCharacterSet
		{
			get { return _specificCharacterSet; }
			set { _specificCharacterSet = value; }
		}

		/// <summary>
		/// Adds a frame and associated presentation state to the serialization queue.
		/// </summary>
		public void AddImage(Frame frame, DicomSoftcopyPresentationState presentationState)
		{
			AddImage((KeyImageReference) frame, presentationState);
		}

		/// <summary>
		/// Adds a frame and associated presentation state to the serialization queue.
		/// </summary>
		public void AddImage(KeyImageReference keyImage, PresentationStateReference presentationState)
		{
			_framePresentationStates.Add(new KeyValuePair<KeyImageReference, PresentationStateReference>(keyImage, presentationState));
		}

		/// <summary>
		/// Serializes the current contents into a number of key object selection document SOP instances.
		/// </summary>
		public List<DicomFile> Serialize()
		{
			return Serialize(null);
		}

		/// <summary>
		/// Serializes the current contents into a number of key object selection document SOP instances.
		/// </summary>
		/// <param name="callback">A callback method to initialize the series-level attributes of the key object document.
		/// Should return a data set from which patient and study level attributes can be copied, otherwise a KO document will not be created for this study.
		/// </param>
		public List<DicomFile> Serialize(InitializeKeyObjectDocumentSeriesCallback callback)
		{
			callback = callback ?? DefaultInitializeKeyObjectDocumentSeriesCallback;

			if (_framePresentationStates.Count == 0)
				throw new InvalidOperationException("Key object selection cannot be empty.");

			List<DicomFile> keyObjectDocuments = new List<DicomFile>();
			List<IHierarchicalSopInstanceReferenceMacro> identicalDocuments = new List<IHierarchicalSopInstanceReferenceMacro>();
			Dictionary<string, KeyObjectSelectionDocumentIod> koDocumentsByStudy = new Dictionary<string, KeyObjectSelectionDocumentIod>();
			foreach (var frame in (IEnumerable<KeyImageReference>) _framePresentationStates)
			{
				string studyInstanceUid = frame.StudyInstanceUid;
				if (!koDocumentsByStudy.ContainsKey(studyInstanceUid))
				{
					KeyObjectDocumentSeries seriesInfo = new KeyObjectDocumentSeries(studyInstanceUid);
					var prototypeDataSet = callback.Invoke(seriesInfo);
					if (prototypeDataSet == null) continue;

					DicomFile keyObjectDocument = new DicomFile();
					keyObjectDocument.SourceApplicationEntityTitle = frame.SourceApplicationEntityTitle;

					KeyObjectSelectionDocumentIod iod = CreatePrototypeDocument(prototypeDataSet, keyObjectDocument.DataSet, SpecificCharacterSet);

					iod.GeneralEquipment.Manufacturer = this.Manufacturer ?? string.Empty; // this one is type 2 - all other GenEq attributes are type 3
					iod.GeneralEquipment.ManufacturersModelName = string.IsNullOrEmpty(this.ManufacturersModelName) ? null : this.ManufacturersModelName;
					iod.GeneralEquipment.DeviceSerialNumber = string.IsNullOrEmpty(this.DeviceSerialNumber) ? null : this.DeviceSerialNumber;
					iod.GeneralEquipment.SoftwareVersions = string.IsNullOrEmpty(this.SoftwareVersions) ? null : this.SoftwareVersions;
					iod.GeneralEquipment.InstitutionName = string.IsNullOrEmpty(this.Institution.Name) ? null : this.Institution.Name;
					iod.GeneralEquipment.InstitutionAddress = string.IsNullOrEmpty(this.Institution.Address) ? null : this.Institution.Address;
					iod.GeneralEquipment.InstitutionalDepartmentName = string.IsNullOrEmpty(this.Institution.DepartmentName) ? null : this.Institution.DepartmentName;
					iod.GeneralEquipment.StationName = string.IsNullOrEmpty(this.StationName) ? null : this.StationName;

					string seriesDescription = _seriesDescription;
					if (!string.IsNullOrEmpty(_author))
						seriesDescription = string.Format("{0} ({1})", seriesDescription, _author);

					iod.KeyObjectDocumentSeries.InitializeAttributes();
					iod.KeyObjectDocumentSeries.Modality = Modality.KO;
					iod.KeyObjectDocumentSeries.SeriesDateTime = seriesInfo.SeriesDateTime;
					iod.KeyObjectDocumentSeries.SeriesDescription = seriesDescription;
					iod.KeyObjectDocumentSeries.SeriesInstanceUid = CreateUid(seriesInfo.SeriesInstanceUid);
					iod.KeyObjectDocumentSeries.SeriesNumber = seriesInfo.SeriesNumber ?? 1;
					iod.KeyObjectDocumentSeries.ReferencedPerformedProcedureStepSequence = null;

					iod.SopCommon.SopClass = SopClass.KeyObjectSelectionDocumentStorage;
					iod.SopCommon.SopInstanceUid = DicomUid.GenerateUid().UID;

					identicalDocuments.Add(iod.KeyObjectDocument.CreateIdenticalDocumentsSequence(
						studyInstanceUid,
						iod.KeyObjectDocumentSeries.SeriesInstanceUid,
						iod.SopCommon.SopClassUid,
						iod.SopCommon.SopInstanceUid));

					koDocumentsByStudy.Add(studyInstanceUid, iod);
					keyObjectDocuments.Add(keyObjectDocument);
				}
			}

			foreach (KeyObjectSelectionDocumentIod iod in koDocumentsByStudy.Values)
			{
				iod.KeyObjectDocument.InitializeAttributes();
				iod.KeyObjectDocument.InstanceNumber = 1;
				iod.KeyObjectDocument.ContentDateTime = _datetime;
				iod.KeyObjectDocument.ReferencedRequestSequence = null;

				iod.KeyObjectDocument.IdenticalDocumentsSequence = identicalDocuments.ToArray();

				iod.SrDocumentContent.InitializeContainerAttributes();
				iod.SrDocumentContent.ConceptNameCodeSequence = _docTitle;

				List<IContentSequence> contentList = new List<IContentSequence>();
				EvidenceDictionary currentRequestedProcedureEvidenceList = new EvidenceDictionary();

				List<KeyImageReference> frameMap = new List<KeyImageReference>();
				foreach (var frameAndPresentationState in _framePresentationStates)
				{
					var frame = frameAndPresentationState.Key;

					// build frame map by unique sop - used to make the evidence sequence less verbose
					if (!frameMap.Contains(frame))
						frameMap.Add(frame);

					// content sequence must still list all content as it was given, including any repeats
					IContentSequence content = iod.SrDocumentContent.CreateContentSequence();
					{
						content.RelationshipType = RelationshipType.Contains;

						IImageReferenceMacro imageReferenceMacro = content.InitializeImageReferenceAttributes();
						imageReferenceMacro.ReferencedSopSequence.InitializeAttributes();
						imageReferenceMacro.ReferencedSopSequence.ReferencedSopClassUid = frame.SopClassUid;
						imageReferenceMacro.ReferencedSopSequence.ReferencedSopInstanceUid = frame.SopInstanceUid;
						if (frame.FrameNumber.HasValue)
							imageReferenceMacro.ReferencedSopSequence.ReferencedFrameNumber = frame.FrameNumber.Value.ToString(CultureInfo.InvariantCulture);
						else
							imageReferenceMacro.ReferencedSopSequence.ReferencedFrameNumber = null;

						// save the presentation state
						if (frameAndPresentationState.Value != null)
						{
							var presentationState = frameAndPresentationState.Value;
							imageReferenceMacro.ReferencedSopSequence.CreateReferencedSopSequence();
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.InitializeAttributes();
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopClassUid = presentationState.SopClassUid;
							imageReferenceMacro.ReferencedSopSequence.ReferencedSopSequence.ReferencedSopInstanceUid = presentationState.SopInstanceUid;
						}
					}
					contentList.Add(content);
				}

				// add the author
				if (!string.IsNullOrEmpty(_author))
				{
					IContentSequence koAuthor = iod.SrDocumentContent.CreateContentSequence();
					koAuthor.InitializeAttributes();
					koAuthor.ConceptNameCodeSequence = KeyObjectSelectionCodeSequences.PersonObserverName;
					koAuthor.PersonName = _author;
					koAuthor.RelationshipType = RelationshipType.HasObsContext;
					contentList.Add(koAuthor);
				}

				// add the description
				if (!string.IsNullOrEmpty(_description))
				{
					IContentSequence koDescription = iod.SrDocumentContent.CreateContentSequence();
					koDescription.InitializeAttributes();
					koDescription.ConceptNameCodeSequence = KeyObjectSelectionCodeSequences.KeyObjectDescription;
					koDescription.TextValue = _description;
					koDescription.RelationshipType = RelationshipType.Contains;
					contentList.Add(koDescription);
				}

				// add each unique sop to the evidence list using the map built earlier
				foreach (var sop in frameMap.Distinct())
					currentRequestedProcedureEvidenceList.Add(sop);

				// add each referenced presentation state to the evidence list as well
				foreach (var state in (IEnumerable<PresentationStateReference>) _framePresentationStates)
				{
					if (state == null)
						continue;
					currentRequestedProcedureEvidenceList.Add(state);
				}

				// set the content and the evidence sequences
				iod.SrDocumentContent.ContentSequence = contentList.ToArray();
				iod.KeyObjectDocument.CurrentRequestedProcedureEvidenceSequence = currentRequestedProcedureEvidenceList.ToArray();
			}

			// set meta for the files
			foreach (DicomFile keyObjectDocument in keyObjectDocuments)
			{
				keyObjectDocument.MediaStorageSopClassUid = keyObjectDocument.DataSet[DicomTags.SopClassUid].ToString();
				keyObjectDocument.MediaStorageSopInstanceUid = keyObjectDocument.DataSet[DicomTags.SopInstanceUid].ToString();
			}

			return keyObjectDocuments;
		}

		private static KeyObjectSelectionDocumentIod CreatePrototypeDocument(IDicomAttributeProvider source, DicomAttributeCollection target, string specificCharacterSet)
		{
			KeyObjectSelectionDocumentIod iod = new KeyObjectSelectionDocumentIod(target);
			specificCharacterSet = specificCharacterSet ?? string.Empty;
			target.SpecificCharacterSet = specificCharacterSet;
			target[DicomTags.SpecificCharacterSet].SetStringValue(specificCharacterSet);

			PatientModuleIod sourcePatient = new PatientModuleIod(source);
			if (true) // patient module is always required
			{
				iod.Patient.BreedRegistrationSequence = sourcePatient.BreedRegistrationSequence;
				iod.Patient.DeIdentificationMethod = sourcePatient.DeIdentificationMethod;
				iod.Patient.DeIdentificationMethodCodeSequence = sourcePatient.DeIdentificationMethodCodeSequence;
				iod.Patient.EthnicGroup = sourcePatient.EthnicGroup;
				iod.Patient.IssuerOfPatientId = sourcePatient.IssuerOfPatientId;
				iod.Patient.OtherPatientIds = sourcePatient.OtherPatientIds;
				iod.Patient.OtherPatientIdsSequence = sourcePatient.OtherPatientIdsSequence;
				iod.Patient.OtherPatientNames = sourcePatient.OtherPatientNames;
				iod.Patient.PatientBreedCodeSequence = sourcePatient.PatientBreedCodeSequence;
				iod.Patient.PatientBreedDescription = sourcePatient.PatientBreedDescription;
				iod.Patient.PatientComments = sourcePatient.PatientComments;
				iod.Patient.PatientId = sourcePatient.PatientId;
				iod.Patient.PatientIdentityRemoved = sourcePatient.PatientIdentityRemoved;
				iod.Patient.PatientsBirthDateTime = sourcePatient.PatientsBirthDateTime;
				iod.Patient.PatientsName = sourcePatient.PatientsName;
				iod.Patient.PatientSpeciesCodeSequence = sourcePatient.PatientSpeciesCodeSequence;
				iod.Patient.PatientSpeciesDescription = sourcePatient.PatientSpeciesDescription;
				iod.Patient.PatientsSex = sourcePatient.PatientsSex;
				iod.Patient.ReferencedPatientSequence = sourcePatient.ReferencedPatientSequence;
				iod.Patient.ResponsibleOrganization = sourcePatient.ResponsibleOrganization;
				iod.Patient.ResponsiblePerson = sourcePatient.ResponsiblePerson;
				iod.Patient.ResponsiblePersonRole = sourcePatient.ResponsiblePersonRole;
			}

			SpecimenIdentificationModuleIod sourceSpecimen = new SpecimenIdentificationModuleIod(source);
			if (sourceSpecimen.HasValues()) // specimen module is required only if subject is a specimen
			{
				iod.SpecimenIdentification.SpecimenAccessionNumber = sourceSpecimen.SpecimenAccessionNumber;
				iod.SpecimenIdentification.SpecimenSequence = sourceSpecimen.SpecimenSequence;
			}

			ClinicalTrialSubjectModuleIod sourceTrialSubject = new ClinicalTrialSubjectModuleIod(source);
			if (sourceTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				iod.ClinicalTrialSubject.ClinicalTrialProtocolId = sourceTrialSubject.ClinicalTrialProtocolId;
				iod.ClinicalTrialSubject.ClinicalTrialProtocolName = sourceTrialSubject.ClinicalTrialProtocolName;
				iod.ClinicalTrialSubject.ClinicalTrialSiteId = sourceTrialSubject.ClinicalTrialSiteId;
				iod.ClinicalTrialSubject.ClinicalTrialSiteName = sourceTrialSubject.ClinicalTrialSiteName;
				iod.ClinicalTrialSubject.ClinicalTrialSponsorName = sourceTrialSubject.ClinicalTrialSponsorName;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectId = sourceTrialSubject.ClinicalTrialSubjectId;
				iod.ClinicalTrialSubject.ClinicalTrialSubjectReadingId = sourceTrialSubject.ClinicalTrialSubjectReadingId;
			}

			GeneralStudyModuleIod sourceGeneralStudy = new GeneralStudyModuleIod(source);
			if (true) // general study module is always required
			{
				iod.GeneralStudy.AccessionNumber = sourceGeneralStudy.AccessionNumber;
				iod.GeneralStudy.NameOfPhysiciansReadingStudy = sourceGeneralStudy.NameOfPhysiciansReadingStudy;
				iod.GeneralStudy.PhysiciansOfRecord = sourceGeneralStudy.PhysiciansOfRecord;
				iod.GeneralStudy.PhysiciansOfRecordIdentificationSequence = sourceGeneralStudy.PhysiciansOfRecordIdentificationSequence;
				iod.GeneralStudy.PhysiciansReadingStudyIdentificationSequence = sourceGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
				iod.GeneralStudy.ProcedureCodeSequence = sourceGeneralStudy.ProcedureCodeSequence;
				iod.GeneralStudy.ReferencedStudySequence = sourceGeneralStudy.ReferencedStudySequence;
				iod.GeneralStudy.ReferringPhysicianIdentificationSequence = sourceGeneralStudy.ReferringPhysicianIdentificationSequence;
				iod.GeneralStudy.ReferringPhysiciansName = sourceGeneralStudy.ReferringPhysiciansName;
				iod.GeneralStudy.StudyDateTime = sourceGeneralStudy.StudyDateTime;
				iod.GeneralStudy.StudyDescription = sourceGeneralStudy.StudyDescription;
				iod.GeneralStudy.StudyId = sourceGeneralStudy.StudyId;
				iod.GeneralStudy.StudyInstanceUid = sourceGeneralStudy.StudyInstanceUid;
			}

			PatientStudyModuleIod sourcePatientStudy = new PatientStudyModuleIod(source);
			if (sourcePatientStudy.HasValues()) // patient study module is user optional
			{
				iod.PatientStudy.AdditionalPatientHistory = sourcePatientStudy.AdditionalPatientHistory;
				iod.PatientStudy.AdmissionId = sourcePatientStudy.AdmissionId;
				iod.PatientStudy.AdmittingDiagnosesCodeSequence = sourcePatientStudy.AdmittingDiagnosesCodeSequence;
				iod.PatientStudy.AdmittingDiagnosesDescription = sourcePatientStudy.AdmittingDiagnosesDescription;
				iod.PatientStudy.IssuerOfAdmissionId = sourcePatientStudy.IssuerOfAdmissionId;
				iod.PatientStudy.IssuerOfServiceEpisodeId = sourcePatientStudy.IssuerOfServiceEpisodeId;
				iod.PatientStudy.Occupation = sourcePatientStudy.Occupation;
				iod.PatientStudy.PatientsAge = sourcePatientStudy.PatientsAge;
				iod.PatientStudy.PatientsSexNeutered = sourcePatientStudy.PatientsSexNeutered;
				iod.PatientStudy.PatientsSize = sourcePatientStudy.PatientsSize;
				iod.PatientStudy.PatientsWeight = sourcePatientStudy.PatientsWeight;
				iod.PatientStudy.ServiceEpisodeDescription = sourcePatientStudy.ServiceEpisodeDescription;
				iod.PatientStudy.ServiceEpisodeId = sourcePatientStudy.ServiceEpisodeId;
			}

			ClinicalTrialStudyModuleIod sourceTrialStudy = new ClinicalTrialStudyModuleIod(source);
			if (sourceTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				iod.ClinicalTrialStudy.ClinicalTrialTimePointDescription = sourceTrialStudy.ClinicalTrialTimePointDescription;
				iod.ClinicalTrialStudy.ClinicalTrialTimePointId = sourceTrialStudy.ClinicalTrialTimePointId;
			}

			return iod;
		}

		private static string CreateUid(string uidHint)
		{
			if (string.IsNullOrEmpty(uidHint))
				return DicomUid.GenerateUid().UID;
			return uidHint;
		}

		private static string GetUserName()
		{
			IPrincipal p = Thread.CurrentPrincipal;
			if (p == null || p.Identity == null || string.IsNullOrEmpty(p.Identity.Name))
				return Environment.UserName;
			return p.Identity.Name;
		}

		#region FramePresentationList Class

		private class FramePresentationList : IList<KeyValuePair<KeyImageReference, PresentationStateReference>>, IEnumerable<KeyImageReference>, IEnumerable<PresentationStateReference>
		{
			private readonly List<KeyValuePair<KeyImageReference, PresentationStateReference>> _list = new List<KeyValuePair<KeyImageReference, PresentationStateReference>>();

			public int IndexOf(KeyValuePair<KeyImageReference, PresentationStateReference> item)
			{
				return _list.IndexOf(item);
			}

			public void Insert(int index, KeyValuePair<KeyImageReference, PresentationStateReference> item)
			{
				Platform.CheckForNullReference(item, "item");
				Platform.CheckForNullReference(item.Key, "item");
				_list.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				_list.RemoveAt(index);
			}

			public KeyValuePair<KeyImageReference, PresentationStateReference> this[int index]
			{
				get { return _list[index]; }
				set
				{
					Platform.CheckForNullReference(value, "value");
					Platform.CheckForNullReference(value.Key, "value");
					_list[index] = value;
				}
			}

			public void Add(KeyValuePair<KeyImageReference, PresentationStateReference> item)
			{
				Platform.CheckForNullReference(item, "item");
				Platform.CheckForNullReference(item.Key, "item");
				_list.Add(item);
			}

			public void Clear()
			{
				_list.Clear();
			}

			public bool Contains(KeyValuePair<KeyImageReference, PresentationStateReference> item)
			{
				return _list.Contains(item);
			}

			public void CopyTo(KeyValuePair<KeyImageReference, PresentationStateReference>[] array, int arrayIndex)
			{
				_list.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _list.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(KeyValuePair<KeyImageReference, PresentationStateReference> item)
			{
				return _list.Remove(item);
			}

			public IEnumerator<KeyValuePair<KeyImageReference, PresentationStateReference>> GetEnumerator()
			{
				return _list.GetEnumerator();
			}

			IEnumerator<KeyImageReference> IEnumerable<KeyImageReference>.GetEnumerator()
			{
				foreach (KeyValuePair<KeyImageReference, PresentationStateReference> pair in _list)
					yield return pair.Key;
			}

			IEnumerator<PresentationStateReference> IEnumerable<PresentationStateReference>.GetEnumerator()
			{
				foreach (KeyValuePair<KeyImageReference, PresentationStateReference> pair in _list)
					yield return pair.Value;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		#endregion

		#region EvidenceDictionary Class

// ReSharper disable RedundantArgumentName

		private class EvidenceDictionary
		{
			private readonly HierarchicalSopInstanceReferenceDictionary _dictionary = new HierarchicalSopInstanceReferenceDictionary();

			public void Add(SopInstanceReference sop)
			{
				_dictionary.TryAddReference(sop.StudyInstanceUid, sop.SeriesInstanceUid, sop.SopClassUid, sop.SopInstanceUid,
				                            retrieveAeTitle : sop.SourceApplicationEntityTitle);
			}

			public IHierarchicalSopInstanceReferenceMacro[] ToArray()
			{
				return _dictionary.ToArray();
			}
		}

// ReSharper restore RedundantArgumentName

		#endregion

		#region Series-Level Attribute Initializer

		/// <summary>
		/// Represents the callback method that initializes the <see cref="KeyObjectDocumentSeriesModuleIod">series-level attributes</see> of a key object selection document.
		/// </summary>
		/// <param name="keyObjectDocumentSeries">A key object document series module.</param>
		public delegate IDicomAttributeProvider InitializeKeyObjectDocumentSeriesCallback(KeyObjectDocumentSeries keyObjectDocumentSeries);

		private static IDicomAttributeProvider DefaultInitializeKeyObjectDocumentSeriesCallback(KeyObjectDocumentSeries keyObjectDocumentSeries)
		{
			var dataset = new DicomAttributeCollection();
			dataset[DicomTags.StudyInstanceUid].SetStringValue(keyObjectDocumentSeries.StudyInstanceUid);
			return dataset;
		}

		/// <summary>
		/// Supplies the series-level attribute values of a key object selection document.
		/// </summary>
		public sealed class KeyObjectDocumentSeries
		{
			/// <summary>
			/// Gets the study instance UID of the study for which the key object selection document is being created.
			/// </summary>
			public readonly string StudyInstanceUid;

			/// <summary>
			/// Gets or sets the series instance UID for the key object selection document.
			/// </summary>
			/// <remarks>
			/// If this property is set to empty or null, a new UID will be generated automatically.
			/// </remarks>
			public string SeriesInstanceUid = null;

			/// <summary>
			/// Gets or sets the series number for the key object selection document.
			/// </summary>
			/// <remarks>
			/// If this property is set to null, a series number will be computed automatically.
			/// </remarks>
			public int? SeriesNumber = null;

			/// <summary>
			/// Gets or sets the series date/time for the key object selection document.
			/// </summary>
			/// <remarks>
			/// If this property is set to null, the series date/time attributes will not be included.
			/// </remarks>
			public DateTime? SeriesDateTime = null;

			internal KeyObjectDocumentSeries(string studyInstanceUid)
			{
				StudyInstanceUid = studyInstanceUid;
			}
		}

		#endregion
	}
}