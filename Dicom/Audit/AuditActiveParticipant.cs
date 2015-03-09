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
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Active Participant Identification
	/// </summary>
	/// <remarks>
	/// <para>
	/// From RFC 3881:
	/// The following data identify a user for the purpose of documenting
	/// accountability for the audited event.  A user may be a person, or a
   	/// hardware device or software process for events that are not initiated
   	/// by a person.
	/// </para>
	/// <para>
	/// Optionally, the user's network access location may be specified.
	/// </para>
	/// <para>
	/// There may be more than one user per event, for example, in cases of
   	/// actions initiated by one user for other users, or in events that
   	/// involve more than one user, hardware device, or system process.
	/// However, only one user may be the initiator/requestor for the event.
	/// </para>
	/// </remarks>
	public abstract class AuditActiveParticipant
	{
		protected NetworkAccessPointTypeEnum? _networkAccessPointType = null;
		protected string _userId = null;
		protected string _alternateUserId = null;
		protected string _userName = null;
		protected bool? _userIsRequestor = null;
		protected string _networkAccessPointId = null;
        protected RoleIDCode _roleIdCode = null;

		protected AuditActiveParticipant()
		{
			
		}

		/// <summary>
		/// An identifier for the type of network access point that originated the audit event.
		/// </summary>
		public NetworkAccessPointTypeEnum? NetworkAccessPointType
		{
			get { return _networkAccessPointType; }
		}

		/// <summary>
		/// An identifier for the network access point of the user device.  This could be 
		/// the device id, IP address, or some other identifier associated with a device.
		/// </summary>
		public string NetworkAccessPointId
		{
			get { return _networkAccessPointId; }
		}

		/// <summary>
		/// Unique identifier for the user actively participating in the event (Required)
		/// </summary>
		/// <remarks>
		/// <para>
		/// From RFC 3881:
		/// For cross-system audits, especially with long retention, this user
		/// identifier will permanently tie an audit event to a specific user
		/// via a perpetually unique key.
		///</para>
		/// <para>
		/// For node-based authentication -- where only the system hardware or
		/// process, but not a human user, is identified -- User ID would be
		/// the node name.
		/// </para>
		/// <para>
		/// Remarks from Dicom Supplement 95:
		/// If the participant is a person, then the User ID shall be how that person is identified on this particular
		/// system, in the form of loginName@domain-name. If the participant is a process, then User ID shall be the
		/// process ID as used within the local operating system in the local system logs.
		/// </para>
		/// <para>
		/// When importing or exporting data, e.g. by means of media, the UserID field is used both to identify people
		/// and to identify the media itself. When the Role ID Code is EV(110154, DCM, �Destination Media�) or
		/// EV(110155, DCM, �Source Media�), the UserID may be:
		/// </para>
		/// <list>
		/// <item>
		/// a. a URI (the preferred form) identifying the source or destination,
		/// </item>
		/// <item>
		/// b. an email address of the form �mailto:user@address�
		/// </item>
		/// <item>
		/// c. a description of the media type (e.g. DVD) together with a description of its identifying label, as a
		/// free text field,
		/// </item>
		/// <item>
		/// d. a description of the media type (e.g. paper, film) together with a description of the location of the
		/// media creator (i.e., the printer).
		/// </item>
		/// </list>
		/// <para>
		/// The UserID field for Media needs to be highly flexible given the large variety of media and transports that
		/// might be used.
		/// </para>
		/// </remarks>
		public string UserId
		{
			get { return _userId; }
		}

		/// <summary>
		/// Alternative unique identifier for the user
		/// </summary>
		/// <remarks>
		/// <para>
		/// From RFC 3881:
		/// In some situations a user may authenticate with one identity but, to
		/// access a specific application system, may use a synonymous identify.
   		/// For example, some "single sign on" implementations will do this.  The
   		/// alternative identifier would then be the original identify used for
   		/// authentication, and the User ID is the one known to and used by the
		/// application.
		/// </para>
		/// <para>
		/// From DICOM Supplement 95:
		/// If the participant is a person, then Alternate User ID shall be how that person is uniquely identified within
		/// an enterprise for authentication purposes, for example, a Kerberos Username (user@realm). If the
		/// participant is a DICOM application, then Alternate User ID shall be at least one of the AE Titles supported
		/// listed as:
		/// </para>
		/// <para>
		///  AETITLES=aetitle1;aetitle2;�
		/// </para>
		/// <para>
		///  When importing or exporting data, e.g. by means of media, the Alternate UserID field is used both to
		///  identify people and to identify the media itself. When the Role ID Code is EV(110154, DCM, �Destination
		///  Media�) or EV(110155, DCM, �Source Media�), the Alternate UserID may be any machine readable
		///  identifications on the media, such as media serial number, volume label, DICOMDIR SOP Instance UID.
		/// </para>
		/// </remarks>
		public string AlternateUserId
		{
			get { return _alternateUserId; }
		}

		/// <summary>
		/// The human-meaningful name for the user
		/// </summary>
		/// <remarks>
		/// <para>
		/// From RFC 3881:
		///  The User ID and Alternative User ID may be internal or otherwise
		/// obscure values.  This field assists the auditor in identifying the
		/// actual user.
		/// </para>
		/// <para>
		/// From DICOM Supplment 95:
		/// A human readable identification of the participant, which serves to further clarify who or what the
		/// participant is. If the participant is a person, the person�s name shall be used. If the participant is a
		/// process, then the process name shall be used.
		/// </para>
		/// </remarks>
		public string UserName
		{
			get { return _userName; }
		}

		/// <summary>
		/// Indicator that the user is or is not the requestor, or initiator, for the event being audited.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public bool? UserIsRequestor
		{
			get { return _userIsRequestor; }
			set { _userIsRequestor = value; }
		}

		/// <summary>
		/// Specification of the role(s) the user plays when performing the event, as assigned in role-based access control security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This field is extended by Dicom.  It is typically filled in by the <see cref="DicomAuditHelper"/> derived classes.
		/// </para>
		/// </remarks>
		public RoleIDCode RoleIdCode
		{
			get { return _roleIdCode; }
			set { _roleIdCode = value; }
		}
	}
	/// <summary>
	/// Specialization of <see cref="AuditActiveParticipant"/> for Users/Persons
	/// </summary>
	public class AuditPersonActiveParticipant : AuditActiveParticipant
	{
		public AuditPersonActiveParticipant(string userId, string alternateUserId, string userName)
		{
			_userId = userId;
			_alternateUserId = alternateUserId;
			_userName = userName;

			_networkAccessPointType = NetworkAccessPointTypeEnum.IpAddress;
			_networkAccessPointId = HostingEnvironment.IsHosted ? DicomAuditHelper.ClientIpAddress : DicomAuditHelper.ProcessIpAddress;
		}

		public AuditPersonActiveParticipant(string userId, string alternateUserId, string userName, string ipAddress)
		{
			_userId = userId;
			_alternateUserId = alternateUserId;
			_userName = userName;

			_networkAccessPointType = NetworkAccessPointTypeEnum.IpAddress;
			_networkAccessPointId = ipAddress;
		}
	}

	/// <summary>
	/// Specialization of <see cref="AuditActiveParticipant"/> for Processes/Machines
	/// </summary>
	public class AuditProcessActiveParticipant : AuditActiveParticipant
	{
		public AuditProcessActiveParticipant(string[] aeTitles)
		{
			_userName = DicomAuditHelper.ProcessName;
			var sb = new StringBuilder("AETITLES=");
			foreach (string aeTitle in aeTitles)
			{
				sb.Append(aeTitle);
				sb.Append(";");
			}
			_userId = DicomAuditHelper.ProcessId;
			_alternateUserId = sb.ToString();
			_networkAccessPointId = DicomAuditHelper.ProcessIpAddress;
			_networkAccessPointType = NetworkAccessPointTypeEnum.IpAddress;
			_roleIdCode = RoleIDCode.Application;
		}

		public AuditProcessActiveParticipant(string aeTitle)
		{
			_userName = DicomAuditHelper.ProcessName;
			_userId = DicomAuditHelper.ProcessId;
			_alternateUserId = String.Format("AETITLES={0}", aeTitle);
			_networkAccessPointId = DicomAuditHelper.ProcessIpAddress;
			_networkAccessPointType = NetworkAccessPointTypeEnum.IpAddress;
			_roleIdCode = RoleIDCode.Application;
		}
		public AuditProcessActiveParticipant()
		{
			_userName = DicomAuditHelper.ProcessName;
			_userId = DicomAuditHelper.ProcessId;
			_networkAccessPointId = DicomAuditHelper.ProcessIpAddress;
			_networkAccessPointType = NetworkAccessPointTypeEnum.IpAddress;
			_roleIdCode = RoleIDCode.Application;
		}

		public void SetAlternateUserId(string val)
		{
			_alternateUserId = val;
		}
	}
}
