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
using System.Linq;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	public partial class DicomAnonymizer
	{
		// This list of tags is derived from DICOM 2011 PS 3.15 Section E.1.1 (Table E.1-1) 'Basic Profile'
		// X/Z tags are treated as Z (assume that the original dataset meets the IOD requirements)
		// D tags are treated as Z (we don't have a way to figure out a suitable dummy value so just leave it blank but present)
		// Many Date/Time tags are treated as Z/D if they have a logical relationship to the study date - and if study date/time is being blanked, these tags will also be blanked
		// The following SQ tags do not receive any special processing (we automatically process all nested data sets, so all nested tags will get the appropriate action anyway)
		// - ContentSequence; // X (but required for key image support)
		// - GraphicAnnotationSequence; // D (but required for presentation state support)
		// - OverlayData; // X (but the overlay doesn't usually contain identifying information, and should really be supported through a clean pixels option)
		// - ReferencedImageSequence; // X/Z/U*
		// - ReferencedStudySequence; // X/Z
		// - SourceImageSequence; // X/Z/U*

		/// <summary>
		/// Tags that are removed if they are present (DICOM PS 3.15 Action 'X') 
		/// </summary>
		internal static readonly ICollection<uint> TagsToRemove = ListTagsToRemove();

		/// <summary>
		/// Tags that are blanked if they are present (DICOM PS 3.15 Action 'Z') 
		/// </summary>
		internal static readonly ICollection<uint> TagsToNull = ListTagsToNull();

		/// <summary>
		/// UI tags that are remapped if they are present (DICOM PS 3.15 Action 'U') 
		/// </summary>
		internal static readonly ICollection<uint> UidTagsToRemap = ListUidTagsToRemap();

		/// <summary>
		/// DA/TM/DT tags that, if present, are adjusted relative to the study date/time (effectively a DICOM PS 3.15 Action 'D') 
		/// </summary>
		internal static readonly ICollection<uint> DateTimeTagsToAdjust = ListDateTimeTagsToAdjust();

		private static ICollection<uint> ListTagsToRemove()
		{
			return new ReadOnlyTagList(new[]
			                           	{
			                           		DicomTags.AcquisitionCommentsRetired,
			                           		DicomTags.AcquisitionContextSequence,
			                           		DicomTags.AcquisitionProtocolDescription,
			                           		DicomTags.ActualHumanPerformersSequence,
			                           		DicomTags.AdditionalPatientHistory,
			                           		DicomTags.AdmissionId,
			                           		DicomTags.AdmittingDate,
			                           		DicomTags.AdmittingDiagnosesCodeSequence,
			                           		DicomTags.AdmittingDiagnosesDescription,
			                           		DicomTags.AdmittingTime,
			                           		DicomTags.AffectedSopInstanceUid,
			                           		DicomTags.Allergies,
			                           		DicomTags.ArbitraryRetired,
			                           		DicomTags.AuthorObserverSequence,
			                           		DicomTags.BranchOfService,
			                           		DicomTags.CassetteId,
			                           		DicomTags.CommentsOnThePerformedProcedureStep,
			                           		DicomTags.ConfidentialityConstraintOnPatientDataDescription,
			                           		DicomTags.ContentCreatorsIdentificationCodeSequence,
			                           		DicomTags.ContributionDescription,
			                           		DicomTags.CountryOfResidence,
			                           		DicomTags.CurrentPatientLocation,
			                           		DicomTags.CurveDataRetired,
			                           		DicomTags.CurveDateRetired,
			                           		DicomTags.CurveTimeRetired,
			                           		DicomTags.CustodialOrganizationSequence,
			                           		DicomTags.DataSetTrailingPadding,
			                           		DicomTags.DerivationDescription,
			                           		DicomTags.DetectorId,
			                           		DicomTags.DigitalSignatureUid,
			                           		DicomTags.DigitalSignaturesSequence,
			                           		DicomTags.DischargeDiagnosisDescriptionRetired,
			                           		DicomTags.DistributionAddressRetired,
			                           		DicomTags.DistributionNameRetired,
			                           		DicomTags.EthnicGroup,
			                           		DicomTags.FrameComments,
			                           		DicomTags.GantryId,
			                           		DicomTags.GeneratorId,
			                           		DicomTags.HumanPerformersName,
			                           		DicomTags.HumanPerformersOrganization,
			                           		DicomTags.IconImageSequence,
			                           		DicomTags.IdentifyingCommentsRetired,
			                           		DicomTags.ImageComments,
			                           		DicomTags.ImagePresentationCommentsRetired,
			                           		DicomTags.ImagingServiceRequestComments,
			                           		DicomTags.ImpressionsRetired,
			                           		DicomTags.InstitutionAddress,
			                           		DicomTags.InstitutionalDepartmentName,
			                           		DicomTags.InsurancePlanIdentificationRetired,
			                           		DicomTags.IntendedRecipientsOfResultsIdentificationSequence,
			                           		DicomTags.InterpretationApproverSequenceRetired,
			                           		DicomTags.InterpretationAuthorRetired,
			                           		DicomTags.InterpretationDiagnosisDescriptionRetired,
			                           		DicomTags.InterpretationIdIssuerRetired,
			                           		DicomTags.InterpretationRecorderRetired,
			                           		DicomTags.InterpretationTextRetired,
			                           		DicomTags.InterpretationTranscriberRetired,
			                           		DicomTags.IssuerOfAdmissionIdRetired,
			                           		DicomTags.IssuerOfPatientId,
			                           		DicomTags.IssuerOfServiceEpisodeIdRetired,
			                           		DicomTags.LastMenstrualDate,
			                           		DicomTags.Mac,
			                           		DicomTags.MedicalAlerts,
			                           		DicomTags.MedicalRecordLocator,
			                           		DicomTags.MilitaryRank,
			                           		DicomTags.ModifiedAttributesSequence,
			                           		DicomTags.ModifiedImageDescriptionRetired,
			                           		DicomTags.ModifyingDeviceIdRetired,
			                           		DicomTags.ModifyingDeviceManufacturerRetired,
			                           		DicomTags.NameOfPhysiciansReadingStudy,
			                           		DicomTags.NamesOfIntendedRecipientsOfResults,
			                           		DicomTags.Occupation,
			                           		DicomTags.OriginalAttributesSequence,
			                           		DicomTags.OrderCallbackPhoneNumber,
			                           		DicomTags.OrderEnteredBy,
			                           		DicomTags.OrderEnterersLocation,
			                           		DicomTags.OtherPatientIds,
			                           		DicomTags.OtherPatientIdsSequence,
			                           		DicomTags.OtherPatientNames,
			                           		DicomTags.OverlayCommentsRetired,
			                           		DicomTags.OverlayDateRetired,
			                           		DicomTags.OverlayTimeRetired,
			                           		DicomTags.ParticipantSequence,
			                           		DicomTags.PatientsAddress,
			                           		DicomTags.PatientComments,
			                           		DicomTags.PatientState,
			                           		DicomTags.PatientTransportArrangements,
			                           		DicomTags.PatientsAge,
			                           		DicomTags.PatientsBirthName,
			                           		DicomTags.PatientsBirthTime,
			                           		DicomTags.PatientsInstitutionResidence,
			                           		DicomTags.PatientsInsurancePlanCodeSequence,
			                           		DicomTags.PatientsMothersBirthName,
			                           		DicomTags.PatientsPrimaryLanguageCodeSequence,
			                           		DicomTags.PatientsPrimaryLanguageModifierCodeSequence,
			                           		DicomTags.PatientsReligiousPreference,
			                           		DicomTags.PatientsSize,
			                           		DicomTags.PatientsTelephoneNumbers,
			                           		DicomTags.PatientsWeight,
			                           		DicomTags.PerformedLocation,
			                           		DicomTags.PerformedProcedureStepDescription,
			                           		DicomTags.PerformedProcedureStepId,
			                           		DicomTags.PerformedProcedureStepStartDate,
			                           		DicomTags.PerformedProcedureStepStartTime,
			                           		DicomTags.PerformedStationAeTitle,
			                           		DicomTags.PerformedStationGeographicLocationCodeSequence,
			                           		DicomTags.PerformedStationName,
			                           		DicomTags.PerformedStationNameCodeSequence,
			                           		DicomTags.PerformingPhysicianIdentificationSequence,
			                           		DicomTags.PerformingPhysiciansName,
			                           		DicomTags.PersonsAddress,
			                           		DicomTags.PersonsTelephoneNumbers,
			                           		DicomTags.PhysicianApprovingInterpretationRetired,
			                           		DicomTags.PhysiciansReadingStudyIdentificationSequence,
			                           		DicomTags.PhysiciansOfRecord,
			                           		DicomTags.PhysiciansOfRecordIdentificationSequence,
			                           		DicomTags.PlateId,
			                           		DicomTags.PreMedication,
			                           		DicomTags.PregnancyStatus,
			                           		DicomTags.ReasonForTheImagingServiceRequestRetired,
			                           		DicomTags.ReasonForStudyRetired,
			                           		DicomTags.ReferencedDigitalSignatureSequence,
			                           		DicomTags.ReferencedPatientAliasSequence,
			                           		DicomTags.ReferencedPatientSequence,
			                           		DicomTags.ReferencedSopInstanceMacSequence,
			                           		DicomTags.ReferringPhysiciansAddress,
			                           		DicomTags.ReferringPhysicianIdentificationSequence,
			                           		DicomTags.ReferringPhysiciansTelephoneNumbers,
			                           		DicomTags.RegionOfResidence,
			                           		DicomTags.RequestAttributesSequence,
			                           		DicomTags.RequestedContrastAgent,
			                           		DicomTags.RequestedProcedureComments,
			                           		DicomTags.RequestedProcedureId,
			                           		DicomTags.RequestedProcedureLocation,
			                           		DicomTags.RequestingPhysician,
			                           		DicomTags.RequestingService,
			                           		DicomTags.ResponsibleOrganization,
			                           		DicomTags.ResponsiblePerson,
			                           		DicomTags.ResultsCommentsRetired,
			                           		DicomTags.ResultsDistributionListSequenceRetired,
			                           		DicomTags.ResultsIdIssuerRetired,
			                           		DicomTags.ScheduledHumanPerformersSequence,
			                           		DicomTags.ScheduledPatientInstitutionResidenceRetired,
			                           		DicomTags.ScheduledPerformingPhysicianIdentificationSequence,
			                           		DicomTags.ScheduledPerformingPhysiciansName,
			                           		DicomTags.ScheduledProcedureStepEndDate,
			                           		DicomTags.ScheduledProcedureStepEndTime,
			                           		DicomTags.ScheduledProcedureStepDescription,
			                           		DicomTags.ScheduledProcedureStepLocation,
			                           		DicomTags.ScheduledProcedureStepStartDate,
			                           		DicomTags.ScheduledProcedureStepStartTime,
			                           		DicomTags.ScheduledStationAeTitle,
			                           		DicomTags.ScheduledStationGeographicLocationCodeSequence,
			                           		DicomTags.ScheduledStationName,
			                           		DicomTags.ScheduledStationNameCodeSequence,
			                           		DicomTags.ScheduledStudyLocationRetired,
			                           		DicomTags.ScheduledStudyLocationAeTitleRetired,
			                           		DicomTags.SeriesDescription,
			                           		DicomTags.ServiceEpisodeDescription,
			                           		DicomTags.ServiceEpisodeId,
			                           		DicomTags.SmokingStatus,
			                           		DicomTags.SpecialNeeds,
			                           		DicomTags.StudyCommentsRetired,
			                           		DicomTags.StudyDescription,
			                           		DicomTags.StudyIdIssuerRetired,
			                           		DicomTags.TextCommentsRetired,
			                           		DicomTags.TextString,
			                           		DicomTags.TimezoneOffsetFromUtc,
			                           		DicomTags.TopicAuthorRetired,
			                           		DicomTags.TopicKeywordsRetired,
			                           		DicomTags.TopicSubjectRetired,
			                           		DicomTags.TopicTitleRetired,
			                           		DicomTags.VerifyingOrganization,
			                           		DicomTags.VisitComments
			                           	});
		}

		private static ICollection<uint> ListTagsToNull()
		{
			return new ReadOnlyTagList(new[]
			                           	{
			                           		DicomTags.AccessionNumber,
			                           		DicomTags.AcquisitionDeviceProcessingDescription, // X/D
			                           		DicomTags.ContentCreatorsName,
			                           		DicomTags.ContrastBolusAgent, // Z/D
			                           		DicomTags.DeviceSerialNumber, // X/Z/D
			                           		DicomTags.FillerOrderNumberImagingServiceRequest,
			                           		DicomTags.InstitutionCodeSequence, // X/Z/D
			                           		DicomTags.InstitutionName, // X/Z/D
			                           		DicomTags.OperatorIdentificationSequence, // X/D
			                           		DicomTags.OperatorsName, // X/Z/D
			                           		DicomTags.PatientId,
			                           		DicomTags.PatientsBirthDate,
			                           		DicomTags.PatientsName,
			                           		DicomTags.PatientsSex,
			                           		DicomTags.PatientsSexNeutered, // X/Z
			                           		DicomTags.PersonIdentificationCodeSequence, // D
			                           		DicomTags.PersonName, // D
			                           		DicomTags.PlacerOrderNumberImagingServiceRequest,
			                           		DicomTags.ProtocolName, // X/D
			                           		DicomTags.ReferencedPerformedProcedureStepSequence, // X/Z/D
			                           		DicomTags.ReferringPhysiciansName,
			                           		DicomTags.RequestedProcedureDescription, // X/Z
			                           		DicomTags.ReviewerName, // X/Z
			                           		DicomTags.StationName, // X/Z/D
			                           		DicomTags.StudyId,
			                           		DicomTags.VerifyingObserverIdentificationCodeSequence,
			                           		DicomTags.VerifyingObserverName, // D
			                           		DicomTags.VerifyingObserverSequence // D
			                           	});
		}

		private static ICollection<uint> ListUidTagsToRemap()
		{
			return new ReadOnlyTagList(new[]
			                           	{
			                           		DicomTags.ConcatenationUid,
			                           		DicomTags.ContextGroupExtensionCreatorUid,
			                           		DicomTags.CreatorVersionUid,
			                           		DicomTags.DeviceUid,
			                           		DicomTags.DimensionOrganizationUid,
			                           		DicomTags.DoseReferenceUid,
			                           		DicomTags.FailedSopInstanceUidList,
			                           		DicomTags.FiducialUid,
			                           		DicomTags.FrameOfReferenceUid,
			                           		DicomTags.InstanceCreatorUid,
			                           		DicomTags.IrradiationEventUid,
			                           		DicomTags.LargePaletteColorLookupTableUidRetired,
			                           		DicomTags.MediaStorageSopInstanceUid,
			                           		DicomTags.PaletteColorLookupTableUid,
			                           		DicomTags.ReferencedFrameOfReferenceUid,
			                           		DicomTags.ReferencedGeneralPurposeScheduledProcedureStepTransactionUid,
			                           		DicomTags.ReferencedSopInstanceUid,
			                           		DicomTags.ReferencedSopInstanceUidInFile,
			                           		DicomTags.RelatedFrameOfReferenceUid,
			                           		DicomTags.RequestedSopInstanceUid,
			                           		DicomTags.SeriesInstanceUid,
			                           		DicomTags.SopInstanceUid,
			                           		DicomTags.StorageMediaFileSetUid,
			                           		DicomTags.StudyInstanceUid,
			                           		DicomTags.SynchronizationFrameOfReferenceUid,
			                           		DicomTags.TemplateExtensionCreatorUidRetired,
			                           		DicomTags.TemplateExtensionOrganizationUidRetired,
			                           		DicomTags.TransactionUid,
			                           		DicomTags.Uid
			                           	});
		}

		private static ICollection<uint> ListDateTimeTagsToAdjust()
		{
			return new ReadOnlyTagList(new[]
			                           	{
			                           		DicomTags.AcquisitionDate, // X/Z
			                           		DicomTags.AcquisitionTime, // X/Z
			                           		DicomTags.AcquisitionDatetime, // X/D
			                           		DicomTags.ContentDate, // Z/D
			                           		DicomTags.ContentTime, // Z/D
			                           		DicomTags.SeriesDate, // X/D
			                           		DicomTags.SeriesTime, // X/D
			                           		DicomTags.StudyDate, // Z
			                           		DicomTags.StudyTime, // Z

			                           		// DICOM 2011 PS 3.15 Section E.1.1 #7 Note 4
			                           		// Include additional date/time tags not explicitly identified in the standard, but logically depend on the study date
			                           		DicomTags.ApprovalStatusDatetime,
			                           		DicomTags.AttributeModificationDatetime,
			                           		DicomTags.ContributionDateTime,
			                           		DicomTags.CreationDate,
			                           		DicomTags.CreationTime,
			                           		DicomTags.Date,
			                           		DicomTags.DateOfDocumentOrVerbalTransactionTrialRetired,
			                           		DicomTags.DateOfSecondaryCapture,
			                           		DicomTags.Datetime,
			                           		DicomTags.DecayCorrectionDatetime,
			                           		DicomTags.DigitalSignatureDatetime,
			                           		DicomTags.DischargeDateRetired,
			                           		DicomTags.DischargeTimeRetired,
			                           		DicomTags.EffectiveDatetime,
			                           		DicomTags.EndAcquisitionDatetime,
			                           		DicomTags.ExclusionStartDatetime,
			                           		DicomTags.ExpectedCompletionDateTime,
			                           		DicomTags.ExpiryDate,
			                           		DicomTags.FindingsGroupRecordingDateTrialRetired,
			                           		DicomTags.FindingsGroupRecordingTimeTrialRetired,
			                           		DicomTags.FirstTreatmentDate,
			                           		DicomTags.FrameAcquisitionDatetime,
			                           		DicomTags.FrameReferenceDatetime,
			                           		DicomTags.HangingProtocolCreationDatetime,
			                           		DicomTags.InformationIssueDatetime,
			                           		DicomTags.InstanceCreationDate,
			                           		DicomTags.InstanceCreationTime,
			                           		DicomTags.InterpretationApprovalDateRetired,
			                           		DicomTags.InterpretationApprovalTimeRetired,
			                           		DicomTags.InterpretationRecordedDateRetired,
			                           		DicomTags.InterpretationRecordedTimeRetired,
			                           		DicomTags.InterpretationTranscriptionDateRetired,
			                           		DicomTags.InterpretationTranscriptionTimeRetired,
			                           		DicomTags.IssueDateOfImagingServiceRequest,
			                           		DicomTags.IssueTimeOfImagingServiceRequest,
			                           		DicomTags.ModifiedImageDateRetired,
			                           		DicomTags.ModifiedImageTimeRetired,
			                           		DicomTags.MostRecentTreatmentDate,
			                           		DicomTags.ObservationDateTime,
			                           		DicomTags.ObservationDateTrialRetired,
			                           		DicomTags.ObservationTimeTrialRetired,
			                           		DicomTags.ParticipationDatetime,
			                           		DicomTags.PresentationCreationDate,
			                           		DicomTags.PresentationCreationTime,
			                           		DicomTags.ProcedureExpirationDate,
			                           		DicomTags.ProcedureLastModifiedDate,
			                           		DicomTags.ProcedureStepCancellationDatetime,
			                           		DicomTags.ProductExpirationDatetime,
			                           		DicomTags.RadiopharmaceuticalStartDatetime,
			                           		DicomTags.RadiopharmaceuticalStopDatetime,
			                           		DicomTags.ReferencedDatetime,
			                           		DicomTags.ReviewDate,
			                           		DicomTags.ReviewTime,
			                           		DicomTags.RtPlanDate,
			                           		DicomTags.RtPlanTime,
			                           		DicomTags.SafePositionExitDate,
			                           		DicomTags.SafePositionExitTime,
			                           		DicomTags.SafePositionReturnDate,
			                           		DicomTags.SafePositionReturnTime,
			                           		DicomTags.ScheduledAdmissionDateRetired,
			                           		DicomTags.ScheduledAdmissionTimeRetired,
			                           		DicomTags.ScheduledDischargeDateRetired,
			                           		DicomTags.ScheduledDischargeTimeRetired,
			                           		DicomTags.ScheduledProcedureStepModificationDateTime,
			                           		DicomTags.ScheduledStudyStartDateRetired,
			                           		DicomTags.ScheduledStudyStartTimeRetired,
			                           		DicomTags.SopAuthorizationDatetime,
			                           		DicomTags.SourceStrengthReferenceDate,
			                           		DicomTags.SourceStrengthReferenceTime,
			                           		DicomTags.StructureSetDate,
			                           		DicomTags.StructureSetTime,
			                           		DicomTags.StartAcquisitionDatetime,
			                           		DicomTags.StudyArrivalDateRetired,
			                           		DicomTags.StudyArrivalTimeRetired,
			                           		DicomTags.StudyCompletionDateRetired,
			                           		DicomTags.StudyCompletionTimeRetired,
			                           		DicomTags.StudyReadDateRetired,
			                           		DicomTags.StudyReadTimeRetired,
			                           		DicomTags.StudyVerifiedDateRetired,
			                           		DicomTags.StudyVerifiedTimeRetired,
			                           		DicomTags.SubstanceAdministrationDatetime,
			                           		DicomTags.Time,
			                           		DicomTags.TimeOfDocumentCreationOrVerbalTransactionTrialRetired,
			                           		DicomTags.TimeOfSecondaryCapture,
			                           		DicomTags.TreatmentControlPointDate,
			                           		DicomTags.TreatmentControlPointTime,
			                           		DicomTags.TreatmentDate,
			                           		DicomTags.TreatmentTime,
			                           		DicomTags.VerificationDateTime,
			                           		// Include additional date/time tags not explicitly identified in the standard, but could otherwise identify specific hardware
			                           		DicomTags.DateOfGainCalibration,
			                           		DicomTags.TimeOfGainCalibration,
			                           		DicomTags.DateOfLastCalibration,
			                           		DicomTags.TimeOfLastCalibration,
			                           		DicomTags.DateOfLastDetectorCalibration,
			                           		DicomTags.TimeOfLastDetectorCalibration,
			                           		DicomTags.CalibrationDate,
			                           		DicomTags.CalibrationTime
			                           	});
		}

		private class ReadOnlyTagList : ICollection<uint>
		{
			private readonly uint[] _innerList;

			public ReadOnlyTagList(IEnumerable<uint> tags)
			{
				// since the tag lists are constant, pre-sort them to optimize the lookups
				_innerList = tags.OrderBy(t => t).ToArray();
			}

			public int Count
			{
				get { return _innerList.Length; }
			}

			public bool Contains(uint item)
			{
				return Array.BinarySearch(_innerList, item) >= 0;
			}

			public void CopyTo(uint[] array, int arrayIndex)
			{
				Array.Copy(_innerList, 0, array, arrayIndex, _innerList.Length);
			}

			public IEnumerator<uint> GetEnumerator()
			{
				return _innerList.ToList().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _innerList.GetEnumerator();
			}

			#region Implementation of ICollection<uint>

			void ICollection<uint>.Add(uint item)
			{
				throw new NotSupportedException();
			}

			void ICollection<uint>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<uint>.Remove(uint item)
			{
				throw new NotSupportedException();
			}

			bool ICollection<uint>.IsReadOnly
			{
				get { return true; }
			}

			#endregion
		}

#if UNIT_TESTS

		/// <summary>
		/// For unit tests. All 4 lists contenated, suitable for checking for uniqueness of tags.
		/// </summary>
		internal static IList<uint> ListAllProcessedTags()
		{
			return TagsToRemove.Concat(TagsToNull).Concat(UidTagsToRemap).Concat(DateTimeTagsToAdjust).ToList();
		}

#endif
	}
}