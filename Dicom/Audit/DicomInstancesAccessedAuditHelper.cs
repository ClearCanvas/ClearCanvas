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
	/// DICOM Instances Accessed Audit Message Helper
	/// </summary>
	/// <remarks>
	/// This message describes the event of DICOM SOP Instances being viewed, utilized, updated, or deleted.
	/// This event is summarized at the level of studies. This message may only include information about a
	/// single patient.
	/// </remarks>
	public class DicomInstancesAccessedAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
        public DicomInstancesAccessedAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			EventIdentificationContentsEventActionCode action)
			: base("DicomInstancesAccessed")
		{
		    AuditMessage.EventIdentification = new EventIdentificationContents
		                                           {
		                                               EventID = EventID.DICOMInstancesAccessed,
		                                               EventActionCode = action,
		                                               EventActionCodeSpecified = true,
		                                               EventDateTime = Platform.Time.ToUniversalTime(),
		                                               EventOutcomeIndicator = outcome
		                                           };
			InternalAddAuditSource(auditSource);
			AddUser(new AuditProcessActiveParticipant());
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public DicomInstancesAccessedAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
            EventIdentificationContentsEventActionCode action, EventTypeCode type)
            : base("DicomInstancesAccessed")
        {
            AuditMessage.EventIdentification = new EventIdentificationContents
                                                   {
                                                       EventID = EventID.DICOMInstancesAccessed,
                                                       EventActionCode = action,
                                                       EventActionCodeSpecified = true,
                                                       EventTypeCode = new[] {type},
                                                       EventDateTime = Platform.Time.ToUniversalTime(),
                                                       EventOutcomeIndicator = outcome
                                                   };
			InternalAddAuditSource(auditSource);
			AddUser(new AuditProcessActiveParticipant());
        }

		/// <summary>
		/// The identity of the person or process manipulating the data. If both
		/// the person and the process are known, both shall be included.
		/// </summary>
		/// <param name="participant">The participant</param>
		public void AddUser(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
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

		/// <summary>
		/// Add details of a general purpose participant object.
		/// </summary>
		/// <param name="o"></param>
		public void AddGeneralParticipantObject(AuditParticipantObject o)
		{
			InternalAddParticipantObject(o.ParticipantObjectId, o);
		}
	}
}
