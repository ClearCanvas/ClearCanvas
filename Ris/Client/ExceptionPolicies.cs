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
using System.ServiceModel;
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Policy for exceptions that occur when the RIS server rejects a request because the input
    /// does not validate.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(RequestValidationException))]
    [ExceptionPolicyFor(typeof(FaultException<RequestValidationException>))]
    public class RequestValidationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            // only log this if debugging
            context.Log(LogLevel.Debug, e);

            // report to user
            context.ShowMessageBox(e.Message, true);
        }
    }

    /// <summary>
    /// Policy for exceptions that occur when the RIS server rejects a request because it would modify
    /// objects that have been concurrently modified.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(ConcurrentModificationException))]
    [ExceptionPolicyFor(typeof(FaultException<ConcurrentModificationException>))]
    public class ConcurrentModificationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            // only log this if debugging
            context.Log(LogLevel.Debug, e);

            // report to user
            context.ShowMessageBox(SR.MessageConcurrentModification, true);

            // this exception is not recoverable
            context.Abort();
        }
    }

    /// <summary>
    /// Policy for exceptions that occur when a request times out
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(TimeoutException))]
    public class TimeoutExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            // log this as an error
            context.Log(LogLevel.Error, e);

            // report to user
            context.ShowMessageBox(SR.MessageTimeout, true);
        }
    }

    /// <summary>
    /// Policy for WCF communication exceptions
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(CommunicationException))]
    public class CommunicationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(LogLevel.Error, e);
            context.ShowMessageBox(SR.MessageCommunicationError, true);
        }
    }

	/// <summary>
	/// Policy for endpoint-not-found exceptions
	/// </summary>
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	[ExceptionPolicyFor(typeof(EndpointNotFoundException))]
	public class EndpointNotFoundExceptionPolicy : IExceptionPolicy
	{
		public void Handle(Exception e, IExceptionHandlingContext context)
		{
			context.Log(LogLevel.Error, e);

			// if we're known to be in offline mode, then communicate this, otherwise
			// just report a generic communication error 
			context.ShowMessageBox(
				Desktop.Application.SessionStatus == SessionStatus.Offline
					? SR.MessageEndpointNotFoundOfflineMode
					: SR.MessageCommunicationError, true);
		}
	}

    /// <summary>
    /// Policy for Security Access exceptions
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(SecurityAccessDeniedException))]
    public class SecurityAccessDeniedExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(LogLevel.Debug, e);
            context.ShowMessageBox(SR.MessageAccessDenied, true);

            // this exception is not recoverable
            // (well, technically it is, but we don't want to encourage retries !!!)
            context.Abort();
        }
    }

    /// <summary>
    /// Policy for <see cref="MessageSecurityException"/>.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(MessageSecurityException))]
    public class MessageSecurityExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            try
            {
                // typically this means authentication failed, which is usually because the session is no longer valid
				// so inform the desktop of the situation
            	Desktop.Application.InvalidateSession();
            }
            catch (Exception)
            {
                // if we get another exception attempting to renew the login, then we just log the original exception
                // and inform user that there is a problem communicating with server
                context.Log(LogLevel.Error, e);
                context.ShowMessageBox(SR.MessageCommunicationError, true);
            }
        }
    }
}
