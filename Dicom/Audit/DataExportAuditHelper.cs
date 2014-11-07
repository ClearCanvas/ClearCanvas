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
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Data Export Audit Log Helper
	/// </summary>
	/// <remarks>
	/// This message describes the event of exporting data from a system, implying that the data is leaving
	/// control of the system�s security domain. Examples of exporting include printing to paper, recording on film,
	/// creation of a .pdf or HTML file, conversion to another format for storage in an EHR, writing to removable
	/// media, or sending via e-mail. Multiple patients may be described in one event message.
	/// </remarks>
	public class DataExportAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSource">The source of the audit.</param>
		/// <param name="outcome">The outcome (success or failure)</param>
		/// <param name="exportDestination">Any machine readable identifications on the media, such as media serial number, volume label, 
		/// DICOMDIR SOP Instance UID.</param>
		public DataExportAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome, string exportDestination)
			: base("DataExport")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents
				{
					EventID = EventID.Export,
					EventActionCode = EventIdentificationContentsEventActionCode.E,
					EventActionCodeSpecified = true,
					EventDateTime = Platform.Time.ToUniversalTime(),
					EventOutcomeIndicator = outcome
				};

			InternalAddAuditSource(auditSource);
			AddUserParticipant(new AuditProcessActiveParticipant());

			// Add the Destination
			_participantList.Add(
				new ActiveParticipantContents(RoleIDCode.DestinationMedia, ProcessName, exportDestination, null, null, null,false));
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSource">The source of the audit.</param>
		/// <param name="outcome">The outcome (success or failure)</param>
		/// <param name="participant">The active participant in the audit</param>
		public DataExportAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome, AuditActiveParticipant participant)
			: base("DataExport")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents
				{
					EventID = EventID.Export,
					EventActionCode = EventIdentificationContentsEventActionCode.E,
					EventActionCodeSpecified = true,
					EventDateTime = Platform.Time.ToUniversalTime(),
					EventOutcomeIndicator = outcome
				};

			InternalAddAuditSource(auditSource);
			AddUserParticipant(new AuditProcessActiveParticipant());

			// Add the Destination
			_participantList.Add(
				new ActiveParticipantContents(participant));
		}

		/// <summary>
		/// Add an exporter.
		/// </summary>
		public void AddExporter(AuditActiveParticipant participant)
		{
            participant.RoleIdCode = RoleIDCode.SourceMedia;
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="patient">The patient in the export.</param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study">The study in the export.</param>
		public void AddStudyParticipantObject(AuditStudyParticipantObject study)
		{
			InternalAddParticipantObject(study.StudyInstanceUid, study);
		}

		/// <summary>
		/// Add details of images within a study.  SOP Class information is automatically updated.
		/// </summary>
		/// <param name="instance">Descriptive object being audited</param>
		public void AddStorageInstance(StorageInstance instance)
		{
			InternalAddStorageInstance(instance);
		}

		/// <summary>
		/// Add details of a general purpose participant object.
		/// </summary>
		/// <param name="o"></param>
		public void AddGeneralParticipantObject(AuditParticipantObject o)
		{
			InternalAddParticipantObject(o.ParticipantObjectId, o);
		}

		/// <summary>
		/// Add the ID of person or process deidentifing the study.  If both the person
		/// and process are known, both shall be included.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;

			InternalAddActiveParticipant(participant);
		}
	}
}
