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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Represents an entry in an audit log.
    /// </summary>
    public class AuditLogEntry : LogEntry
    {
        private string _category;

        /// <summary>
        /// Private no-args constructor (required to support NHibernate).
        /// </summary>
        protected AuditLogEntry()
        {

        }

		/// <summary>
		/// All args constructor.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="timestamp"></param>
		/// <param name="hostName"></param>
		/// <param name="application"></param>
		/// <param name="user"></param>
		/// <param name="userSessionId"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		protected internal AuditLogEntry(string category, DateTime timestamp, string hostName, string application, string user, string userSessionId, string operation, string details)
			:base(timestamp, hostName, application, user, userSessionId, operation, details)
		{
			_category = category;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        public AuditLogEntry(string category, string operation, string details)
            :base(operation, details)
        {
            _category = category;
        }

        /// <summary>
        /// Gets or sets the category of this audit log entry.
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

		/// <summary>
		/// Gets or sets the time this entry was received by the audit service.
		/// </summary>
		public DateTime ServerReceivedTimeStamp { get; set; }
    }
}
