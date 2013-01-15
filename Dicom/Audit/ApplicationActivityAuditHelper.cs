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
	/// Enum for use with <see cref="ApplicationActivityAuditHelper"/>.
	/// </summary>
	public enum ApplicationActivityType
	{
		ApplicationStarted,
		ApplicationStopped
	}

	/// <summary>
	/// Helper for Application Activity Audit Log
	/// </summary>
	/// <remarks>
	/// This audit message describes the event of an Application Entity starting or stoping.
	/// </remarks>
	public class ApplicationActivityAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="auditSource"></param>
		/// <param name="outcome"></param>
		/// <param name="type"></param>
		/// <param name="idOfApplicationStarted">Add the ID of the Application Started, should be called once.</param>
		public ApplicationActivityAuditHelper(DicomAuditSource auditSource,
			EventIdentificationContentsEventOutcomeIndicator outcome, 
			ApplicationActivityType type,
			AuditProcessActiveParticipant idOfApplicationStarted) : base("ApplicationActivity")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.ApplicationActivity;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			if (type == ApplicationActivityType.ApplicationStarted)
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.ApplicationStart };
			else
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.ApplicationStop };

			idOfApplicationStarted.UserIsRequestor = false;
			idOfApplicationStarted.RoleIdCode = RoleIDCode.Application;

			InternalAddActiveParticipant(idOfApplicationStarted);

		}

		/// <summary>
		/// Add the ID of person or process that started or stopped the Application.  Can be called multiple times.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditActiveParticipant participant)
		{
            participant.RoleIdCode = RoleIDCode.ApplicationLauncher;
			participant.UserIsRequestor = true;

			InternalAddActiveParticipant(participant);
		}
	}
}
