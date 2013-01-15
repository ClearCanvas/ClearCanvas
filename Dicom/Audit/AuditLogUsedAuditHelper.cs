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

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// This message describes the event of a person or process accessing a log of audit trail information.
	/// </summary>
	/// <remarks>
	/// For example, an implementation that maintains a local cache of audit information that has not been
	/// transferred to a central collection point might generate this message if its local cache were accessed by
	/// a user.
	/// </remarks>
	public class AuditLogUsedAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="auditSource">The source of the audit</param>
		/// <param name="outcome">The outcome</param>
		/// <param name="uriOfAuditLog">Add the Identity of the Audit Log.  </param>
        public AuditLogUsedAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome,
			string uriOfAuditLog)
			: base("AuditLogUsed")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.AuditLogUsed;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.R;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			AuditSecurityAlertParticipantObject o =
				new AuditSecurityAlertParticipantObject(ParticipantObjectTypeCodeRoleEnum.SecurityResource,
														ParticipantObjectIdTypeCodeEnum.URI, uriOfAuditLog,
														"Security Audit Log");
			// Only one can be included.
			_participantObjectList.Clear();
			_participantObjectList.Add(uriOfAuditLog, o);
		}

		/// <summary>
		/// Add the ID of person or process that started or stopped the Application.  Can be called twice.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="userId">The person or process accessing the audit trail. If both are known,
		/// then two active participants shall be included (both the person and the process).</param>
		public void AddActiveParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}
	}
}
