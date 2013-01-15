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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class GetOrderDetailRequest : DataContractBase
	{
		public GetOrderDetailRequest(EntityRef orderRef,
			bool includeVisit,
			bool includeProcedures,
			bool includeAlerts,
			bool includeNotes,
			bool includeAttachments,
			bool includeResultRecipients)
		{
			this.OrderRef = orderRef;
			this.IncludeVisit = includeVisit;
			this.IncludeProcedures = includeProcedures;
			this.IncludeAlerts = includeAlerts;
			this.IncludeNotes = includeNotes;
			this.IncludeAttachments = includeAttachments;
			this.IncludeResultRecipients = includeResultRecipients;
		}

		public GetOrderDetailRequest()
		{
		}

		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// Include order alerts.
		/// </summary>
		[DataMember]
		public bool IncludeAlerts;

		/// <summary>
		/// Include visit information.
		/// </summary>
		[DataMember]
		public bool IncludeVisit;

		/// <summary>
		/// Include detailed procedure information.
		/// </summary>
		[DataMember]
		public bool IncludeProcedures;

		/// <summary>
		/// Include order notes.
		/// </summary>
		[DataMember]
		public bool IncludeNotes;

		/// <summary>
		/// Include order attachments.
		/// </summary>
		[DataMember]
		public bool IncludeAttachments;

		/// <summary>
		/// Include order result recipients.
		/// </summary>
		[DataMember]
		public bool IncludeResultRecipients;

		/// <summary>
		/// A list of filters that determine which categories of order notes are returned. Optional, defaults to all.
		/// Ignored if <see cref="IncludeNotes"/> is false.
		/// </summary>
		[DataMember]
		public List<string> NoteCategoriesFilter;

		/// <summary>
		/// Include order extended properties.
		/// </summary>
		[DataMember]
		public bool IncludeExtendedProperties;
	}
}
