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
using System.Runtime.Serialization;

namespace ClearCanvas.Common.Audit
{
	/// <summary>
	/// Contains all information about an audit log entry.
	/// </summary>
	[DataContract]
	public class AuditEntryInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AuditEntryInfo()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="timeStamp"></param>
		/// <param name="hostName"></param>
		/// <param name="application"></param>
		/// <param name="user"></param>
		/// <param name="userSessionId"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		public AuditEntryInfo(string category, DateTime timeStamp, string hostName, string application, string user, string userSessionId, string operation, string details)
		{
			TimeStamp = timeStamp;
			HostName = hostName;
			Application = application;
			User = user;
			UserSessionId = userSessionId;
			Category = category;
			Operation = operation;
			Details = details;
		}

		/// <summary>
		/// Gets or sets the time at which this log entry was created.
		/// </summary>
		[DataMember]
		public DateTime TimeStamp { get; private set; }

		/// <summary>
		/// Gets or sets the hostname of the computer that generated this log entry.
		/// </summary>
		[DataMember]
		public string HostName { get; private set; }

		/// <summary>
		/// Gets or sets the the name of the application that created this log entry.
		/// </summary>
		[DataMember]
		public string Application { get; private set; }

		/// <summary>
		/// Gets or sets the user of the application on whose behalf this log entry was created.
		/// </summary>
		[DataMember]
		public string User { get; private set; }

		/// <summary>
		/// Gets or sets the user session ID on whose behalf this log entry was created.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string UserSessionId { get; private set; }

		/// <summary>
		/// Gets or sets the name of the operation that caused this log entry to be created.
		/// </summary>
		[DataMember]
		public string Operation { get; private set; }

		/// <summary>
		/// Gets or sets the contents of this log entry, which may be text or XML based.
		/// </summary>
		[DataMember]
		public string Details { get; private set; }

		/// <summary>
		/// Gets or sets the category of this audit log entry.
		/// </summary>
		[DataMember]
		public string Category { get; private set; }
	}
}
