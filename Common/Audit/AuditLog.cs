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
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace ClearCanvas.Common.Audit
{
	/// <summary>
	/// Defines an extension point for audit sinks.
	/// </summary>
	[ExtensionPoint]
	public class AuditSinkExtensionPoint : ExtensionPoint<IAuditSink>
	{
	}

	/// <summary>
	/// Represents an audit log.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class AuditLog
	{
		private readonly string _application;
		private readonly string _category;
		private readonly IAuditSink[] _sinks;

		#region Constructors
		
		/// <summary>
		/// Constructs an audit log with the specified category.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="category"></param>
		public AuditLog(string application, string category)
			:this(application, category, new []{ CreateSink() })
		{
		}

		/// <summary>
		/// Constructs an audit log with the specified category and sinks.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="category"></param>
		/// <param name="sinks"></param>
		private AuditLog(string application, string category, IAuditSink[] sinks)
		{
			_application = application;
			_category = category;
			_sinks = sinks;			
		}
 
		#endregion	
		
		#region Public API

		/// <summary>
		/// Writes an entry to the audit log containing the specified information,
		/// on behalf of the current application user.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		public void WriteEntry(string operation, string details)
		{
			WriteEntry(operation, details, GetUserName(), GetUserSessionId());
		}

		/// <summary>
		/// Writes an entry to the audit log containing the specified information,
		/// on behalf of the specified application user.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		/// <param name="user"></param>
		/// <param name="userSessionId"></param>
		public void WriteEntry(string operation, string details, string user, string userSessionId)
		{
			var entry = new AuditEntryInfo(
				_category,
				Platform.Time,
				Dns.GetHostName(),
				_application,
				user,
				userSessionId,
				operation,
				details);

			foreach (var sink in _sinks)
			{
				sink.WriteEntry(entry);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the identity of the current thread or null if not established.
		/// </summary>
		/// <returns></returns>
		private static string GetUserName()
		{
			var p = Thread.CurrentPrincipal;

            if (p == null || p.Identity==null)
                return null;

	    //TODO (CR September 2011) - this is not a robust solution.
            // Check if it's being called in a service.
            if (p.Identity is WindowsIdentity)
                return null;

		    return p.Identity.Name;
		}

		/// <summary>
		/// Gets the session token ID of the current thread or null if not established.
		/// </summary>
		/// <returns></returns>
		private static string GetUserSessionId()
		{
			var p = Thread.CurrentPrincipal as IUserCredentialsProvider;
			return (p != null) ? p.SessionTokenId : null;
		}

		/// <summary>
		/// Creates the a single audit sink via the <see cref="AuditSinkExtensionPoint"/>.
		/// </summary>
		/// <returns></returns>
		private static IAuditSink CreateSink()
		{
			try
			{
				return (IAuditSink)(new AuditSinkExtensionPoint()).CreateExtension();
			}
			catch(NotSupportedException)
			{
				//TODO: should there be some kind of default audit sink that just writes to a local log file or something
				throw;
			}
		}

		#endregion
	}
}
