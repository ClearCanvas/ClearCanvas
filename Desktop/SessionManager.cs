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

namespace ClearCanvas.Desktop
{
	public enum SessionStatus
	{
		/// <summary>
		/// Operating as a standalone installation, without an enterprise server.
		/// </summary>
		LocalOnly = 0,

		/// <summary>
		/// Not yet determined.
		/// </summary>
		Unknown,

		/// <summary>
		/// Online
		/// </summary>
		Online,

		/// <summary>
		/// Online, but session has expired, and user must re-authenticate.
		/// </summary>
		Expired,

		/// <summary>
		/// Offline
		/// </summary>
		Offline
	}

	public class SessionStatusChangedEventArgs : EventArgs
	{
		internal SessionStatusChangedEventArgs(SessionStatus oldStatus, SessionStatus newStatus)
		{
			OldStatus = oldStatus;
			NewStatus = newStatus;
		}

		public SessionStatus OldStatus { get; private set; }
		public SessionStatus NewStatus { get; private set; }
	}

	/// <summary>
	/// Defines the interface to extensions of <see cref="SessionManagerExtensionPoint"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A session manager extension is optional.  If present, the application will load the session manager and
	/// call its <see cref="InitiateSession"/> and <see cref="TerminateSession"/> at the beginning and end
	/// of the applications execution, respectively.
	/// </para>
	/// <para>
	/// The purpose of the session manager is to provide a hook through which custom session management can occur.
	/// A typical session manager implemenation may show a login dialog at start-up in order to gather user credentials,
	/// and may perform other custom initialization.
	/// </para>
	/// </remarks>
	public interface ISessionManager
	{
		/// <summary>
		/// Called by the framework at start-up to initiate a session.
		/// </summary>
		/// <remarks>
		/// This method is called after the GUI system and application view have been initialized,
		/// so the implementation may interact with the user if necessary, and may
		/// make use of the <see cref="Application"/> object.  However, no desktop windows exist yet.
		/// Any exception thrown from this method will effectively prevent the establishment of a session, causing
		/// execution to terminate with an error.  A return value of false may be used
		/// to silently refuse initiation of a session.  In this case, no error is reported, but the application
		/// terminates immediately.
		/// </remarks>
		bool InitiateSession();

		/// <summary>
		/// Called to mark the existing session as invalid, which will typically require the user to
		/// re-authenticate in order to continue using the application.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Note that calling this method does not itself renew the session, but merely marks the session
		/// as invalid.  Hence, the session not necessarily have been renewed yet when this method returns.
		/// </remarks>
		void InvalidateSession();

		/// <summary>
		/// Called by the framework at shutdown to terminate an existing session.
		/// </summary>
		/// <remarks>
		/// This method is called prior to terminating the GUI subsytem and application view, so the
		/// implementation may interact with the user if necessary.
		/// </remarks>
		void TerminateSession();

		/// <summary>
		/// Gets the current status of the session.
		/// </summary>
		SessionStatus SessionStatus { get; }

		/// <summary>
		/// Occurs when the <see cref="SessionStatus"/> property changes.
		/// </summary>
		event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
	}

	#region Static Part

	/// <summary>
	/// Session manager base class.
	/// </summary>
	public partial class SessionManager
	{
		#region NullSessionManager Implementation

		private class NullSessionManager : SessionManager
		{
			public NullSessionManager()
				: base(SessionStatus.LocalOnly)
			{
			}

			protected override bool InitiateSession()
			{
				return true;
			}

			protected override void TerminateSession()
			{
			}
		}

		#endregion

		static SessionManager()
		{
			Current = Create();
		}

		/// <summary>
		/// Gets the current session manager.
		/// </summary>
		internal static readonly ISessionManager Current;

		/// <summary>
		/// Instantiates the session manager.
		/// </summary>
		/// <returns></returns>
		private static ISessionManager Create()
		{
			try
			{
				var sessionManager = (ISessionManager)(new SessionManagerExtensionPoint()).CreateExtension();
				Platform.Log(LogLevel.Debug, string.Format("Using session manager extension: {0}", sessionManager.GetType().FullName));
				return sessionManager;
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No session manager extension found");
				return new NullSessionManager();
			}
		}
	}

	#endregion

	#region Instance Part

	public abstract partial class SessionManager : ISessionManager
	{
		private readonly object _syncLock = new object();
		private SessionStatus _sessionStatus;
		private event EventHandler<SessionStatusChangedEventArgs> _sessionStatusChanged;

		protected SessionManager()
		{
		}

		protected SessionManager(SessionStatus initialStatus)
		{
			_sessionStatus = initialStatus;
		}

		#region ISessionManager Members

		/// <summary>
		/// Called by the framework at start-up to initiate a session.
		/// </summary>
		bool ISessionManager.InitiateSession()
		{
			return InitiateSession();
		}

		/// <summary>
		/// Called to request that the existing session be renewed, which typically involves asking the user to re-enter
		/// their credentials.
		/// </summary>
		void ISessionManager.InvalidateSession()
		{
			InvalidateSession();
		}

		/// <summary>
		/// Called by the framework at shutdown to terminate an existing session.
		/// </summary>
		void ISessionManager.TerminateSession()
		{
			TerminateSession();
		}


		/// <summary>
		/// Gets the current status of the session.
		/// </summary>
		SessionStatus ISessionManager.SessionStatus
		{
			get { return this.SessionStatus; }
		}

		/// <summary>
		/// Occurs when the <see cref="ISessionManager.SessionStatus"/> property changes.
		/// </summary>
		event EventHandler<SessionStatusChangedEventArgs> ISessionManager.SessionStatusChanged
		{
			add { this.SessionStatusChanged += value; }
			remove { this.SessionStatusChanged -= value; }
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Gets or sets the session status.
		/// </summary>
		protected SessionStatus SessionStatus
		{
			get
			{
				lock (_syncLock)
				{
					return _sessionStatus;
				}
			}
			set
			{
				lock (_syncLock)
				{
					if (_sessionStatus == value)
						return;

					var old = _sessionStatus;
					_sessionStatus = value;
					NotifyStatusChanged(old, value);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="SessionStatus"/> property changes.
		/// </summary>
		protected event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged
		{
			add
			{
				lock (_syncLock)
				{
					_sessionStatusChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_sessionStatusChanged -= value;
				}
			}
		}

		/// <summary>
		/// Called to initiate the session.
		/// </summary>
		/// <returns></returns>
		protected abstract bool InitiateSession();

		/// <summary>
		/// Called to invalidate the existing session.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// The default implementation sets the session status to <see cref="Desktop.SessionStatus.Expired"/>.
		/// </remarks>
		protected virtual void InvalidateSession()
		{
			this.SessionStatus = SessionStatus.Expired;
		}

		/// <summary>
		/// Called to terminate the session.
		/// </summary>
		protected abstract void TerminateSession();

		/// <summary>
		/// Called when the session status changes.
		/// </summary>
		/// <param name="oldStatus"></param>
		/// <param name="newStatus"></param>
		protected virtual void OnStatusChanged(SessionStatus oldStatus, SessionStatus newStatus)
		{
		}

		#endregion

		private void NotifyStatusChanged(SessionStatus oldStatus, SessionStatus newStatus)
		{
			Action<object> statusChanged = ignored => OnStatusChanged(oldStatus, newStatus);
			Application.MarshalDelegate(statusChanged, this);

			Delegate[] delegates;
			lock (_syncLock)
			{
				if (_sessionStatusChanged == null)
					return;

				delegates = _sessionStatusChanged.GetInvocationList();
			}

			var args = new SessionStatusChangedEventArgs(oldStatus, newStatus);
			foreach (var @delegate in delegates)
			{
				if (!Application.MarshalDelegate(@delegate, this, args))
					@delegate.DynamicInvoke(this, args);
			}
		}
	}

	#endregion
}