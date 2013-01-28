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
using System.Threading;
using System.Web;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Web.Common.Exceptions;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    class CustomFormAuthenticationModule : IHttpModule
    {
        private bool _contextDisposed;

        #region IHttpModule Members

        private HttpApplication _context;
        public void Dispose()
        {
        	AppDomain.CurrentDomain.DomainUnload-=CurrentDomain_DomainUnload;
        	
            if (_context != null && !_contextDisposed)
            {
                _context.AuthorizeRequest -= AuthorizeRequest;
                _context.Disposed -= ContextDisposed;
            }
        }

        public void Init(HttpApplication context)
        {
            _context = context;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            _context.AuthorizeRequest += AuthorizeRequest;
            _context.Disposed += ContextDisposed;
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Platform.Log(LogLevel.Info, "App Domain Is Unloaded");
        }

        void ContextDisposed(object sender, EventArgs e)
        {
            _contextDisposed = true;
        }

        static void AuthorizeRequest(object sender, EventArgs e)
        {
            SessionInfo session=null;
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
                {
                    // Note: If user signed out in another window, the ticket would have been 
                    // removed from the browser and this code shoudn't be executed.
                    
                    // resemble the SessionInfo from the ticket.
                    var loginId = (FormsIdentity) HttpContext.Current.User.Identity ;
                    FormsAuthenticationTicket ticket = loginId.Ticket;

                    String[] fields = ticket.UserData.Split('|');
                    String tokenId = fields[0];
                    String userDisplayName = fields[1];
                    SessionToken token = new SessionToken(tokenId, ticket.Expiration);
                    session = new SessionInfo(loginId.Name, userDisplayName, token);
                     
                    if (ticket.Version > 1)
                    {
                        if (!string.IsNullOrEmpty(fields[2]))
                        {
                            var warningMessages = fields[2].Split('@');
                            session.User.WarningMessages = warningMessages.Length > 0 
                                ? new List<string>(warningMessages) : new List<string>();
                        }

                        if (!string.IsNullOrEmpty(fields[3]))
                        {
                            session.User.WarningsDisplayed = bool.Parse(fields[3]);
                        }
                    }

                    // Initialize the session. This will throw exception if the session is no longer
                    // valid. For eg, time-out.
                    SessionManager.InitializeSession(session);
                }

                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                {
                    String user = SessionManager.Current != null ? SessionManager.Current.User.Identity.Name : "Unknown";

                    Thread.CurrentThread.Name =
                        String.Format(SR.WebGUILogHeader, 
                            HttpContext.Current.Request.UserHostAddress,
                            HttpContext.Current.Request.Browser.Browser,
                            HttpContext.Current.Request.Browser.Version,
                            user);
                }
                
            }
            catch (SessionValidationException)
            {
                // SessionValidationException is thrown when the session id is invalid or the session already expired.
                // If session already expired, 
                if (session != null && session.Credentials.SessionToken.ExpiryTime < Platform.Time)
                {
                    SessionManager.SignOut(session);
                }
                else
                {
                    // redirect to login screen
                    SessionManager.TerminateSession("The current session is no longer valid.", SR.MessageCurrentSessionNoLongerValid);
                }
            }
            catch(Exception ex)
            {
                // log the exception
                ExceptionHandler.ThrowException(ex);
            }
            
           
        }

        #endregion
    }
}
