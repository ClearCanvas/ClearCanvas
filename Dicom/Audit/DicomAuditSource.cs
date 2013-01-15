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
	/// Audit Source as used in DICOM Audit Messages
	/// </summary>
	/// <remarks>
	/// <para>
	/// Comments from RFC 3881.
	/// </para>
	/// <para>
	/// The following data are required primarily for application systems and
   	/// processes.  Since multi-tier, distributed, or composite applications
  	/// make source identification ambiguous, this collection of fields may
  	/// repeat for each application or process actively involved in the
  	/// event.  For example, multiple value-sets can identify participating
  	/// web servers, application processes, and database server threads in an
  	/// n-tier distributed application.  Passive event participants, e.g.,
  	/// low-level network transports, need not be identified.
	/// </para>
	/// <para>
	/// Depending on implementation strategies, it is possible that the
  	/// components in a multi-tier, distributed, or composite applications
  	/// may generate more than one audit message for a single application
  	/// event.  Various data in the audit message may be used to identify
  	/// such cases, supporting subsequent data reduction.  This document
  	/// anticipates that the repository and reporting mechanisms will perform
	/// data reduction when required, but does not specify those mechanism.
	/// </para>
	/// </remarks>
	public class DicomAuditSource
	{
		private readonly string _enterpriseSiteId;
		private readonly string _auditSourceId;
		private readonly AuditSourceTypeCodeEnum? _auditSourceTypeCode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSourceId">Required.  See <see cref="AuditSourceId"/></param>
		public DicomAuditSource(string auditSourceId)
		{
			Platform.CheckForEmptyString(auditSourceId, "auditSourceId");
			
			_auditSourceId = auditSourceId;
			_enterpriseSiteId = null;
			_auditSourceTypeCode = null;
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSourceId">Required.  See <see cref="AuditSourceId"/></param>
		/// <param name="enterpriseSiteId">See <see cref="EnterpriseSiteId"/></param>
		/// <param name="auditSourceTypeCode">See <see cref="AuditSourceType"/></param>
		public DicomAuditSource(string auditSourceId, string enterpriseSiteId, AuditSourceTypeCodeEnum auditSourceTypeCode)
		{
			Platform.CheckForEmptyString(auditSourceId, "auditSourceId");
			
			_auditSourceId = auditSourceId;
			_enterpriseSiteId = enterpriseSiteId;
			_auditSourceTypeCode = auditSourceTypeCode;
		}

		/// <summary>
		/// (optional) Logical source location within the healthcare enterprise network,
		/// e.g., a hospital or other provider location within a multi-entity
		/// provider group.
		/// </summary>
		/// <remarks>
		/// <para>
		///   Unique identifier text string within the healthcare enterprise.
		/// May be unvalued when the audit-generating application is uniquely
		/// identified by Audit Source ID.
		/// </para>
		/// </remarks>
		public string EnterpriseSiteId
		{
			get { return _enterpriseSiteId; }
		}

		/// <summary>
		/// Identifier of the source where the event originated.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Format / Values:
	    /// Unique identifier text string, at least within the Audit
		/// Enterprise Site ID
		/// </para>
		/// <para>
		/// Rationale:
		/// This field ties the event to a specific source system.  It may be
		/// used to group events for analysis according to where the event occurred.
		/// </para>
		/// <para>
		/// Notes:
		/// In some configurations, a load-balancing function distributes work
		/// among two or more duplicate servers.  The values defined for this
		/// field thus may be considered as an source identifier for a group
        /// of servers rather than a specific source system.
		/// </para>
		/// </remarks>
		public string AuditSourceId
		{
			get { return _auditSourceId; }
		}

		/// <summary>
		///  Code specifying the type of source where event originated.
		/// </summary>
		/// <remarks>
		/// Coded-value enumeration, optionally defined by system implementers
		/// or a as a reference to a standard vocabulary. 
		/// </remarks>
		public AuditSourceTypeCodeEnum? AuditSourceType
		{
			get { return _auditSourceTypeCode; }
		}

	}
}
