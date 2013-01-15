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
	public class DataImportAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSource">The source of the audit message.</param>
		/// <param name="outcome">The outcome (success or failure)</param>
		/// <param name="importDescriptor">Any machine readable identifications on the media, such as media serial number, volume label, 
		/// DICOMDIR SOP Instance UID.</param>
		public DataImportAuditHelper(DicomAuditSource auditSource,
            EventIdentificationContentsEventOutcomeIndicator outcome, string importDescriptor)
			: base("DataImport")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.Import;
            AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.C;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			// Add the Destination
			_participantList.Add(
				new ActiveParticipantContents(RoleIDCode.SourceMedia, ProcessName, importDescriptor, null, null, null, false));
		}

		/// <summary>
		/// Add an importer.
		/// </summary>
		/// <param name="userId">The identity of the local user or process importer the data. If both
		/// are known, then two active participants shall be included (both the
		/// person and the process).</param>
		/// <param name="participant">The active participant</param>
		public void AddImporter(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			participant.RoleIdCode = RoleIDCode.Destination;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="study"></param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study"></param>
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
	}
}
