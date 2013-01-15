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
using ClearCanvas.Common;
using System.Threading;
using System.Net;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base class representing an entry in a log.
    /// </summary>
    public abstract class LogEntry : Entity
    {
        private DateTime _timestamp;
    	private string _hostName;
        private string _application;
        private string _user;
    	private string _userSessionId;
        private string _operation;
        private string _details;

         /// <summary>
        /// No-args constructor (required to support NHibernate).
        /// </summary>
        protected LogEntry()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="hostName"></param>
        /// <param name="application"></param>
        /// <param name="user"></param>
        /// <param name="userSessionId"></param>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        protected LogEntry(DateTime timestamp, string hostName, string application, string user, string userSessionId, string operation, string details)
        {
            _timestamp = timestamp;
        	_hostName = hostName;
            _application = application;
            _user = user;
        	_userSessionId = userSessionId;
            _operation = operation;
            _details = details;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        protected LogEntry(string operation, string details)
            :this(
				SafeGetTime(),
				SafeGetHostName(),
                null, // TODO: replace this with something more meaningful????
                Thread.CurrentPrincipal.Identity.Name,
				SafeGetUserSessionId(),
                operation,
                details
                )
        {
        }

    	/// <summary>
        /// Gets or sets the time at which this log entry was created.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

		/// <summary>
		/// Gets or sets the hostname of the computer that generated this log entry.
		/// </summary>
		public string HostName
		{
			get { return _hostName; }
			set { _hostName = value; }
		}

		/// <summary>
        /// Gets or sets the the name of the application that created this log entry.
        /// </summary>
        public string Application
        {
            get { return _application; }
            set { _application = value; }
        }

        /// <summary>
        /// Gets or sets the user of the application on whose behalf this log entry was created.
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

		/// <summary>
		/// Gets or sets the user session ID on whose behalf this log entry was created.
		/// </summary>
    	public string UserSessionId
    	{
			get { return _userSessionId; }
			set { _userSessionId = value; }
    	}

        /// <summary>
        /// Gets or sets the name of the operation that caused this log entry to be created.
        /// </summary>
        public string Operation
        {
            get { return _operation; }
            set { _operation = value; }
        }

        /// <summary>
        /// Gets or sets the contents of this log entry, which may be text or XML based.
        /// </summary>
        public string Details
        {
            get { return _details; }
            set { _details = value; }
		}

		#region Helpers

		/// <summary>
		/// Gets the hostname of the local computer, or null if an error occurs.
		/// </summary>
		/// <returns></returns>
		private static string SafeGetHostName()
		{
			try
			{
				return Dns.GetHostName();
			}
			catch(Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns <see cref="Platform.Time"/>, or the local computer time if an error occurs.
		/// </summary>
		/// <returns></returns>
		private static DateTime SafeGetTime()
		{
			try
			{
				return Platform.Time;
			}
			catch(Exception)
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the user session ID of the current thread, if supported.
		/// </summary>
		/// <returns></returns>
		private static string SafeGetUserSessionId()
		{
			return (Thread.CurrentPrincipal is IUserCredentialsProvider) ? ((IUserCredentialsProvider)Thread.CurrentPrincipal).SessionTokenId : null;
		}

		#endregion
	}
}
