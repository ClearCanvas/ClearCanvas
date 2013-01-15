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
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Begin Transferring DICOM Instances
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of a system begining to transfer a set of DICOM instances from one
	/// node to another node within control of the system�s security domain. This message may only include
	/// information about a single patient.
	/// </para>
	/// <para>
	/// Note: A separate Instances Transferred message is defined for transfer completion, allowing comparison of
	/// what was intended to be sent and what was actually sent.
	/// </para>
	/// </remarks>
	public class BeginTransferringDicomInstancesAuditHelper : DicomAuditHelper
	{
        public BeginTransferringDicomInstancesAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			AssociationParameters parms,
			AuditPatientParticipantObject patient)
			: base("BeginTransferringDicomInstances")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.BeginTransferringDICOMInstances;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			InternalAddActiveDicomParticipant(parms);

			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

        public BeginTransferringDicomInstancesAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			string sourceAE, string sourceHost, string destinationAE, string destinationHost,
			AuditPatientParticipantObject patient)
			: base("BeginTransferringDicomInstances")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.BeginTransferringDICOMInstances;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			InternalAddActiveDicomParticipant(sourceAE, sourceHost, destinationAE, destinationHost);

			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// (Optional) The identity of any other participants that might be involved andknown, especially third parties that are the requestor
		/// </summary>
		/// <param name="participant">The participant</param>
		public void AddOtherParticipants(AuditActiveParticipant participant)
		{
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study"></param>
		public void AddStudyParticipantObject(AuditStudyParticipantObject study)
		{
			InternalAddParticipantObject(study.StudyInstanceUid,study);
		}

		/// <summary>
		/// Add details of images within a study.  SOP Class information is automatically updated.
		/// </summary>
		/// <param name="instance">Descriptive object being audited</param>
		public void AddStorageInstance(StorageInstance instance)
		{
			base.InternalAddStorageInstance(instance);
		}
	}
}
