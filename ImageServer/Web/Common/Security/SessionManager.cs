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
using System.Threading;
using System.Web;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    public static class SessionManager
    {
        #region Private Fields

        #endregion

        static SessionManager()
        {
            Platform.Log(LogLevel.Info, " Initializing SessionManager");
        }

        /// <summary>
        /// Returns the current session information
        /// </summary>
        /// <remarks>
        /// The session information is set by calling <see cref="InitializeSession(SessionInfo)"/>. It is null 
        /// if the <see cref="InitializeSession(SessionInfo)"/> hasn't been called.
        /// </remarks>
        public static SessionInfo Current
        {
            get
            {
                if (Thread.CurrentPrincipal is CustomPrincipal)
                {
                    CustomPrincipal p = Thread.CurrentPrincipal as CustomPrincipal;
                    return new SessionInfo(p);
                   
                }
                return null;
            }
            set
            {
                Thread.CurrentPrincipal = value.User;
                HttpContext.Current.User = value.User;
               
            }
        
        }

		/// <summary>
		/// Gets the redirect url for the current session
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static String GetRedirectUrl(SessionInfo session)
		{
			Platform.CheckForNullReference(session, "session");

			var redirectUrl = FormsAuthentication.GetRedirectUrl(session.User.Identity.Name, false);
			if (VirtualPathUtility.IsAbsolute(redirectUrl) || VirtualPathUtility.IsAppRelative(redirectUrl))
			{
				return redirectUrl;
			}

			var tokenParam = "userId=" + session.User.Identity.Name + "&sessionId=" + session.Credentials.SessionToken.Id;
			var uri = new UriBuilder(redirectUrl);
			uri.Query = string.IsNullOrEmpty(uri.Query) ? tokenParam : uri.Query.Substring(1)+"&" + tokenParam;
			
			return uri.ToString();
		}

        /// <summary>
        /// Sets up the principal for the thread and save the authentiction ticket.
        /// </summary>
        /// <param name="session"></param>
        public static void InitializeSession(SessionInfo session)
        {
            // this should throw exception if the session is no longer valid. It also loads the authority tokens}
            if (!session.Valid)
            {
                throw new Exception("This session is no longer valid");
            }
            Current = session;

            string loginId = session.User.Identity.Name;
            var identity = session.User.Identity as CustomIdentity;
            
            if (identity == null)
            {
                Platform.CheckForNullReference(identity, "identity"); // throw exception
            }
            else
            {
                string displayName = identity.DisplayName;
                SessionToken token = session.Credentials.SessionToken;
                string[] authorities = session.Credentials.Authorities;                

                String data = String.Format("{0}|{1}|{2}|{3}", token.Id, displayName, 
                    StringUtilities.Combine(session.User.WarningMessages,"@"), session.User.WarningsDisplayed);

                // the expiry time is determined by the authentication service
                DateTime expiryTime = token.ExpiryTime; 
                
                var authTicket = new
                    FormsAuthenticationTicket(2,  // version
                                              loginId,         // user name
                                              Platform.Time,   // creation
                                              expiryTime,      // Expiration
                                              false,           // Persistent
                                              data);           // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie with the encrypted data
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                //Create an unencrypted cookie that contains the userid and the expiry time so the browser
                //can check for session timeout.

                //TODO: store the expiry time as epoch instead so we don't need to deal with format or regional settings madness
                var expiryCookie = new HttpCookie(GetExpiryTimeCookieName(session), DateTimeFormatter.Format(expiryTime.ToUniversalTime(), ImageServerConstants.CookieDateTimeFormat));
                
                HttpContext.Current.Response.Cookies.Add(authCookie);
                HttpContext.Current.Response.Cookies.Add(expiryCookie);
                
                SessionTimeout = expiryTime.ToUniversalTime() - Platform.Time.ToUniversalTime();
            }
        }


        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="appName"></param>
        public static SessionInfo InitializeSession(string username, string password, string appName)
        {
			return InitializeSession(username, password, appName, true);
        }

        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="appName"></param>
        /// <param name="redirect"></param>
        public static SessionInfo InitializeSession(string username, string password, string appName, bool redirect)
        {
            using (LoginService service = new LoginService())
            {
                SessionInfo session = service.Login(username, password, appName);
                InitializeSession(session);
                Platform.Log(LogLevel.Info, "[{0}]: {1} has successfully logged in.", appName, username);

                if(redirect)
                {
					HttpContext.Current.Response.Redirect(GetRedirectUrl(session), false);
                }
                return session;
            }
        }

        /// <summary>
        /// Terminates the current session and redirects users to the login page and logs the given log message and 
        /// displays the given display message on the screen.
        /// </summary>
        public static void TerminateSession(string logMessage, string displayMessage)
        {
            SignOut();// force to signout by removing the authentication ticket
            if (!String.IsNullOrEmpty(logMessage))
            {
                Platform.Log(LogLevel.Info, "Terminate session because {0}", logMessage);
                Platform.Log(LogLevel.Info, Environment.StackTrace);
            }

            // Redirect to the login page
            // That's a catch: if this happens on the timeout page and 
            // FormsAuthentication.RedirectToLoginPage is used, the return url will point to the timeout page,
            // causing immediate timeout when users log in again.
            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.IndexOf(ImageServerConstants.PageURLs.DefaultTimeoutPage)==0)
            {
                // Redirect to login page but don't include the ReturnUrl
                var baseUrl = VirtualPathUtility.ToAbsolute(FormsAuthentication.LoginUrl);
                var loginUrl = string.Format("{0}?error={1}", baseUrl, displayMessage);
                HttpContext.Current.Response.Redirect(loginUrl, true);
            }
            else
                FormsAuthentication.RedirectToLoginPage(String.Format("error={0}", displayMessage));
            
        }

        /// <summary>
        /// Signs out the current user without redirection to the login screen.
        /// </summary>
        public static void SignOut()
        {
            SignOut(Current);
        }

        public static void SignOut(SessionInfo session)
        {

            FormsAuthentication.SignOut();
            
            if (session != null)
            {
                try
                {
                    ForceOtherPagesToLogout(session);

                    using (LoginService service = new LoginService())
                    {
                        service.Logout(session.Credentials.SessionToken.Id);
                    }
                }
                catch (NotSupportedException)
                {
                    //ignore this.
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Failed to log user out.");
                }
            }
        }

		/// <summary>
		/// Renew the current session
		/// </summary>
		public static void RenewSession()
		{
			var session = Current;

			if (session != null)
			{
				using (var service = new LoginService())
				{
					SessionInfo sessionInfo = service.Renew(session.Credentials.SessionToken.Id, true /* force to bypass local cache */);
					InitializeSession(sessionInfo);
				}
			}
			
		}


		/// <summary>
		/// Verifies the specified session 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="sessionToken"></param>
		/// <param name="authorityTokens"></param>
		/// <param name="bypassCache">True to always bypass the response cache and get a "fresh" response from the server.</param>
		/// <returns></returns>
		public static bool VerifySession(string userId, string sessionToken, out string[] authorityTokens, bool bypassCache = false)
		{
			try
			{
				ValidateSessionResponse response = null;
				var request = new ValidateSessionRequest(userId, new SessionToken(sessionToken));
				if (bypassCache)
				{
					using (new ResponseCacheBypassScope())
					{
						Platform.GetService<IAuthenticationService>(service => response = service.ValidateSession(request));
					}
				}
				else
				{
					Platform.GetService<IAuthenticationService>(service => response = service.ValidateSession(request));
				}

				authorityTokens = response.AuthorityTokens;
				return response.SessionToken.ExpiryTime > Platform.Time; //TODO : Utc?
			}
			catch (FaultException<InvalidUserSessionException> e)
			{

			}

			authorityTokens = null;
			return false;
		}



        /// <summary>
        /// Gets or sets the session time out in minutes.
        /// </summary>
        public static TimeSpan SessionTimeout { get; set; }

        public static string LoginUrl
        {
            get
            {
                return FormsAuthentication.LoginUrl;
            }
        }

        public static string GetExpiryTimeCookieName()
        {
            return GetExpiryTimeCookieName(Current);
        }
        public static string GetExpiryTimeCookieName(SessionInfo session)
        {
            if (session == null)
                return null;

            string loginId = session.User.Identity.Name;
            return "ImageServer." + loginId;
        }

        private static void ForceOtherPagesToLogout(SessionInfo session)
        {
            //NOTE: SessionTimeout.ascx must be updated if this method  is modified.

			// Ideally we want to remove the expiry time cookie on the client.
			// However, this is not possible. We can only update the cookie.
			// By removing the expiry time value in the cookie, we are implicity 
			// other pages to redirect to the login page instead. (see SessionTimeout.ascx)
            HttpCookie expiryCookie = new HttpCookie(GetExpiryTimeCookieName(session))
            {
				Expires = Platform.Time.AddSeconds(-1)
            };

            HttpContext.Current.Response.Cookies.Set(expiryCookie);
        }

    }
}