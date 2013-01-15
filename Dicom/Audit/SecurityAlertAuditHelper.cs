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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{

	/// <summary>
	/// Security alert event types for use with <see cref="SecurityAlertAuditHelper"/>
	/// </summary>
	public enum SecurityAlertEventTypeCodeEnum
	{
		NodeAuthentication,
		EmergencyOverride,
		NetworkConfiguration,
		SecurityConfiguration,
		HardwareConfiguration,
		SoftwareConfiguration,
		UseOfRestrictedFunction,
		AuditRecordingStopped,
		AuditRecordingStarted,
		ObjectSecurityAttributesChanged,
		SecurityRolesChanged,
		UserSecurityAttributesChanged
	}

	/// <summary>
	/// Security Alert Audit Message Helper
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes any event for which a node needs to report a security alert, e.g., a node
	/// authentication failure when establishing a secure communications channel.
	/// </para>
	/// <para>
	/// Note: The Node Authentication event can be used to report both successes and failures. If reporting of
	/// success is done, this could generate a very large number of audit messages, since every authenticated
	/// DICOM association, HL7 transaction, and HTML connection should result in a successful node
	/// authentication. It is expected that in most situations only the node authentication failures will be
	/// reported.
	/// </para>
	/// </remarks>
	public class SecurityAlertAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="auditSource"></param>
		/// <param name="outcome">Success implies an informative alert. The other failure values
		/// imply warning codes that indicate the severity of the alert. A Minor
		/// or Serious failure indicates that mitigation efforts were effective in
		/// maintaining system security. A Major failure indicates that
		/// mitigation efforts may not have been effective, and that the security
		/// system may have been compromised.</param>
		/// <param name="eventTypeCode">The type of Security Alert event</param>
		public SecurityAlertAuditHelper(DicomAuditSource auditSource,
			EventIdentificationContentsEventOutcomeIndicator outcome,
			SecurityAlertEventTypeCodeEnum eventTypeCode)
			: base("SecurityAlert")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.SecurityAlert;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;
			AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { GetEventTypeCode(eventTypeCode) };

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// The identity of the person or process that detected the activity of
		/// concern. If both are known, then two active participants shall be
		/// included (both the person and the process).
		/// </summary>
		/// <param name="userId">
		/// The identity of the person or process that detected the activity of
		/// concern. If both are known, then two active participants shall be
		/// included (both the person and the process).
		/// </param>
		/// <param name="participant">The reporting participant</param>
		public void AddReportingUser(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// The identity of the person, process, node, or other actor that
		/// performed the activity reported by the alert. If multiple such
		/// participants are known, then all shall be included.
		/// </summary>
		/// <remarks>
		/// Note: In some cases, the user identity is not known precisely. In
		/// such cases, the Active Participant can be left out.
		/// </remarks>
		/// <param name="participant">The participant.</param>
		public void AddActiveParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = false;
			InternalAddActiveParticipant(participant);
		}

        /// <summary>
        /// Add details of a SecurityAlert Participant.
        /// </summary>
        /// <param name="o"></param>
        public void AddSecurityAlertParticipant(AuditSecurityAlertParticipantObject o)
        {
            _participantObjectList.Add(o.ParticipantObjectId, o);
        }

		/// <summary>
		/// Method for transforming event code enum into a CodedValueType.
		/// </summary>
		/// <param name="eventTypeCode"></param>
		/// <returns></returns>
        private static EventTypeCode GetEventTypeCode(SecurityAlertEventTypeCodeEnum eventTypeCode)
		{
            EventTypeCode type = null;
			if (eventTypeCode == SecurityAlertEventTypeCodeEnum.NodeAuthentication)
				type = EventTypeCode.NodeAuthentication;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.EmergencyOverride)
				type = EventTypeCode.EmergencyOverride;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.NetworkConfiguration)
				type = EventTypeCode.NetworkConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SecurityConfiguration)
				type = EventTypeCode.SecurityConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.HardwareConfiguration)
				type = EventTypeCode.HardwareConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SoftwareConfiguration)
				type = EventTypeCode.SoftwareConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.UseOfRestrictedFunction)
				type = EventTypeCode.UseOfRestrictedFunction;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.AuditRecordingStopped)
				type = EventTypeCode.AuditRecordingStopped;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.AuditRecordingStarted)
				type = EventTypeCode.AuditRecordingStarted;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.ObjectSecurityAttributesChanged)
				type = EventTypeCode.ObjectSecurityAttributesChanged;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SecurityRolesChanged)
				type = EventTypeCode.SecurityRolesChanged;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.UserSecurityAttributesChanged)
				type = EventTypeCode.UserSecurityAttributesChanged;
			return type;
		}
	}
}
