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
	/// Enum for use with <see cref="UserAuthenticationAuditHelper"/>.
	/// </summary>
	public enum UserAuthenticationEventType
	{
		Login,
		Logout
	}

	/// <summary>
	/// User Authentication Audit Message
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of a user has attempting to log on or log off, whether successful or not.
	/// No Participant Objects are needed for this message.
	/// </para>
	/// </remarks>
	public class UserAuthenticationAuditHelper : DicomAuditHelper
	{
		public UserAuthenticationAuditHelper(DicomAuditSource auditSource,
			EventIdentificationContentsEventOutcomeIndicator outcome, UserAuthenticationEventType type)
			: base("UserAuthentication")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.UserAuthentication;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			if (type == UserAuthenticationEventType.Login)
				AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.Login };
			else
				AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.Logout };
		}

		/// <summary>
		/// The identity of the person authenticated if successful. Asserted identity if not successful.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditPersonActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// The identity of the node that is authenticating the user. This is to
		/// identify another node that is performing enterprise wide
		/// authentication, e.g. Kerberos authentication.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddNode(AuditProcessActiveParticipant participant)
		{
			participant.UserIsRequestor = false;
			InternalAddActiveParticipant(participant);
		}
	}
}
