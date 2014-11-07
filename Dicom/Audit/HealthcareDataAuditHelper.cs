using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// ClearCanvas Data Updated Audit Message Helper
	/// </summary>
	/// <remarks>
	/// This message describes the event of DICOM SOP Instances being viewed, utilized, updated, or deleted.
	/// This event is summarized at the level of studies. This message may only include information about a
	/// single patient.
	/// </remarks>
	public class HealthcareDataAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public HealthcareDataAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			EventIdentificationContentsEventActionCode action)
			: base("HealthcareDataAccessed")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents
			{
				EventID = EventID.CCHealthcareDataEvent,
				EventActionCode = action,
				EventActionCodeSpecified = true,
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
		/// <param name="patient"></param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of the object being audited.
		/// </summary>
		/// <param name="o"></param>
		public void AddHealthcareParticipantObject(HealthcareDataParticipantObject o)
		{
			InternalAddParticipantObject(o.ParticipantObjectId, o);
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
