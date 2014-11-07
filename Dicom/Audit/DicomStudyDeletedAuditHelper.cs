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
	/// DICOM Study Deleted
	/// </summary>
	/// <remarks>
	/// This message describes the event of deletion of one or more studies and all associated SOP Instances in
	/// a single action. This message may only include information about a single patient.
	/// </remarks>
	public class DicomStudyDeletedAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DicomStudyDeletedAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome)
			: base("DicomStudyDeleted")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents
				{
					EventID = EventID.DICOMStudyDeleted,
					EventActionCode = EventIdentificationContentsEventActionCode.D,
					EventActionCodeSpecified = true,
					EventDateTime = Platform.Time.ToUniversalTime(),
					EventOutcomeIndicator = outcome
				};

			InternalAddAuditSource(auditSource);
			AddUserParticipant(new AuditProcessActiveParticipant());
		}

		/// <summary>
		/// Add the ID of person or process deleting the study.  If both the person
		/// and process are known, both shall be included.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;

			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="patient"></param>
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
		/// Add details of a general purpose participant object.
		/// </summary>
		/// <param name="o"></param>
		public void AddGeneralParticipantObject(AuditParticipantObject o)
		{
			InternalAddParticipantObject(o.ParticipantObjectId, o);
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
