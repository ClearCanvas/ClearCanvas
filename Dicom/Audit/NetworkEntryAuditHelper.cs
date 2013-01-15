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
	/// Enum for use with the <see cref="NetworkEntryAuditHelper"/> class.
	/// </summary>
	public enum NetworkEntryType
	{
		Attach,
		Detach
	}

	/// <summary>
	/// Network Entry Audit Message
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of a system, such as a mobile device, entering or leaving the network
	/// as a normal part of operations. It is not intended to report network problems, loose cables, or other
	/// unintentional detach and reattach situations.
	/// </para>
	/// <para>
	/// Note: The machine should attempt to send this message prior to detaching. If this is not possible, it should
	/// retain the message in a local buffer so that it can be sent later. The mobile machine can then capture
	/// audit messages in a local buffer while it is outside the secure domain. When it is reconnected to the
	/// secure domain, it can send the detach message (if buffered), followed by the buffered messages,
	/// followed by a mobile machine message for rejoining the secure domain. The timestamps on these
	/// messages is the time that the event occurred, not the time that the message is sent.
	/// </para>
	/// </remarks>
	public class NetworkEntryAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="outcome">The outcome of the audit event.</param>
		/// <param name="type">Network Attach or Detach</param>
		/// <param name="node">The identity of the node entering or leaving the network</param>
		/// <param name="auditSource">The source of the audit message.</param>
		public NetworkEntryAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome, NetworkEntryType type, AuditProcessActiveParticipant node)
			: base("NetworkEntry")

		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.NetworkEntry;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			if (type == NetworkEntryType.Attach)
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.Attach };
			else
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.Detach };

			node.UserIsRequestor = false;
			InternalAddActiveParticipant(node);

			InternalAddAuditSource(auditSource);
		}
	}
}
