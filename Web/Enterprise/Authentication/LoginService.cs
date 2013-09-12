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
using System.Configuration;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    
    /// <summary>
    /// Wrapper for <see cref="IAuthenticationService"/> service.
    /// </summary>
    [Obfuscation(Exclude = true, ApplyToMembers = false)]
    public sealed class LoginService : IDisposable
    {
        [Obfuscation(Exclude = true)]
        public SessionInfo Login(string userName, string password, string appName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException(SR.UserIDIsEmpty);

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(SR.PasswordIsEmpty);

            Platform.CheckForEmptyString(password, "password");
            Platform.CheckForEmptyString(appName, "appName");

            SessionInfo session = null;
            
            Platform.GetService(
                delegate(IAuthenticationService  service)
                    {
                        try
                        {
                            var request = new InitiateSessionRequest(userName, appName,
                                                                     Dns.GetHostName(), password)
                                              {
                                                  GetAuthorizations = true
                                              };

                            InitiateSessionResponse response = service.InitiateSession(request);
                            if (response != null)
                            {
                                var credentials = new LoginCredentials
                                                      {
                                                          UserName = userName,
                                                          DisplayName = response.DisplayName,
                                                          SessionToken = response.SessionToken,
                                                          Authorities = response.AuthorityTokens,
                                                          DataAccessAuthorityGroups = response.DataGroupOids,
                                                          EmailAddress = response.EmailAddress
                                                      };
                                var user = new CustomPrincipal(new CustomIdentity(userName, response.DisplayName),credentials);
                                Thread.CurrentPrincipal = user;

                                session = new SessionInfo(user);
                                session.User.WarningMessages = response.WarningMessages;

                                // Note: need to insert into the cache before calling SessionInfo.Validate()
                                SessionCache.Instance.AddSession(response.SessionToken.Id, session);
                                session.Validate();
                                
                                Platform.Log(LogLevel.Info, "{0} has successfully logged in.", userName);                                
                            }                            
                        }
                        catch (FaultException<PasswordExpiredException> ex)
                        {
                            throw ex.Detail;
                        }
                        catch(FaultException<UserAccessDeniedException> ex)
                        {
                            throw ex.Detail;
                        }
                        catch (FaultException<RequestValidationException> ex)
                        {
                            throw ex.Detail;
                        }
                    }
                );

            return session;
        }

        [Obfuscation(Exclude = true)]
        public void Logout(string tokenId)
        {
            var session = SessionCache.Instance.Find(tokenId);
            if (session == null)
            {
                throw new Exception(String.Format("Unexpected error: session {0} does not exist in the cache", tokenId));
            }

            var request = new TerminateSessionRequest(session.Credentials.UserName,
                                                      session.Credentials.SessionToken);


            Platform.GetService(
                delegate(IAuthenticationService service)
                    {
                        service.TerminateSession(request);
                        SessionCache.Instance.RemoveSession(tokenId);
                    });
        }

		/// <summary>
		/// Renew the specified session
		/// </summary>
		/// <param name="tokenId"></param>
		/// <param name="bypassCache"></param>
		/// <returns></returns>
        [Obfuscation(Exclude = true)]
		public SessionInfo Renew(string tokenId, bool bypassCache = false)
        {
			return RenewSession(tokenId, bypassCache);
        }

        /// <summary>
        /// Verifies that the session exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionTokenId"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public SessionInfo VerifySession(string userId, string sessionTokenId)
        {
            SessionInfo sessionInfo = SessionCache.Instance.Find(sessionTokenId);
            if (sessionInfo != null) return sessionInfo;

            // Session does not exist in the cache.
            // Go to the server to see if the session exists
            // It may 
            var request = new ValidateSessionRequest(userId, new SessionToken(sessionTokenId))
            {
                GetAuthorizations = true,
                ValidateOnly = false // load the tokens too since we don't know anything about session yet
            };

            try
            {
                using (new ResponseCacheBypassScope()) //bypass the response cache
                {
                    Platform.GetService(
                    delegate(IAuthenticationService service)
                    {

                        ValidateSessionResponse response = service.ValidateSession(request);
                        sessionInfo = new SessionInfo(userId, userId, response.SessionToken);

                        SessionCache.Instance.AddSession(sessionTokenId, sessionInfo);
                        sessionInfo.Validate();

                    });

                }
                
                return sessionInfo;
            }
            catch (FaultException<InvalidUserSessionException> ex)
            {
                SessionCache.Instance.RemoveSession(sessionTokenId);
                throw new SessionValidationException(ex.Detail);
            }
            catch (FaultException<UserAccessDeniedException> ex)
            {
                SessionCache.Instance.RemoveSession(sessionTokenId);
                throw new SessionValidationException(ex.Detail);
            }
            catch (Exception ex)
            {
                //TODO: for now we can't distinguish communicate errors and credential validation errors.
                // All exceptions are treated the same: we can't verify the session.
                var e = new SessionValidationException(ex);
                throw e;
            }         
        }

        [Obfuscation(Exclude = true)]
        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                var request = new ChangePasswordRequest(userName, oldPassword, newPassword);
                Platform.GetService(
                    delegate(IAuthenticationService service)
                        {
                            service.ChangePassword(request);
                            Platform.Log(LogLevel.Info, "Password for {0} has been changed.", userName);
                        });
            }
            catch (FaultException<UserAccessDeniedException> ex)
            {
                throw ex.Detail;
            }
            catch(FaultException<RequestValidationException> ex)
            {
                throw ex.Detail;
            }
        }

        [Obfuscation(Exclude = true)]
        public void ResetPassword(string userName)
        {
            ResetPasswordResponse response;
            var request = new ResetPasswordRequest(userName);
            Platform.GetService(
                delegate(IAuthenticationService service)
                {
                    response = service.ResetPassword(request);
                    Platform.Log(LogLevel.Info, "Password for {0} has been reset and email sent to {1}.", userName, response.EmailAddress);
                });
        }


		#region Private Methods


		private SessionInfo RenewSession(string tokenId, bool bypassCache = false)
		{
			try
			{
				if (bypassCache)
				{
					using (new ResponseCacheBypassScope())
					{
						return DoRenewSession(tokenId);
					}
				}
				else
				{
					return DoRenewSession(tokenId);
				}

			}
			catch (FaultException<InvalidUserSessionException> ex)
			{
				SessionCache.Instance.RemoveSession(tokenId);
				throw new SessionValidationException(ex.Detail);
			}
			catch (FaultException<UserAccessDeniedException> ex)
			{
				SessionCache.Instance.RemoveSession(tokenId);
				throw new SessionValidationException(ex.Detail);
			}
			catch (Exception ex)
			{
				//TODO: for now we can't distinguish communicate errors and credential validation errors.
				// All exceptions are treated the same: we can't verify the login.
				var e = new SessionValidationException(ex);
				throw e;
			}
		}

		private SessionInfo DoRenewSession(string tokenId)
		{
			SessionInfo sessionInfo = SessionCache.Instance.Find(tokenId);
			if (sessionInfo == null)
			{
				throw new Exception(String.Format("Unexpected error: session {0} does not exist in the cache", tokenId));
			}


			var request = new ValidateSessionRequest(sessionInfo.Credentials.UserName,
													 sessionInfo.Credentials.SessionToken)
			{
				GetAuthorizations = true,
				ValidateOnly = false
			};

			SessionToken newToken = null;
			Platform.GetService(
				delegate(IAuthenticationService service)
				{
					DateTime originalExpiryTime = sessionInfo.Credentials.SessionToken.ExpiryTime;
					ValidateSessionResponse response = service.ValidateSession(request);

					//If we're renewing, might as well renew well before expiry. If we're only validating,
					//we don't want to force a call to the server unless it's actually expired.
					var addSeconds = 0.0;

					//Minor hack - the ImageServer client shows a little bar at the top of the page counting down to session timeout,
					//and allowing the user to cancel. This timeout value determines when that bar shows up. If, however, the 
					//validate response is being cached, we need to make sure we hit the server - otherwise the bar doesn't go away.
					double.TryParse(ConfigurationManager.AppSettings.Get("ClientTimeoutWarningMinDuration"), out addSeconds);
					addSeconds = Math.Max(addSeconds, 30);

					if (Platform.Time.AddSeconds(addSeconds) >= response.SessionToken.ExpiryTime)
					{
						Platform.Log(LogLevel.Debug, "Session is at or near expiry; bypassing cache.");

						//The response likely came from the cache, and we want to make sure we get a real validation response.
						//Note that this only really matters when 2 processes are sharing a session, like ImageServer and Webstation.
						using (new ResponseCacheBypassScope())
						{
							response = service.ValidateSession(request);
						}
					}

					// update session info
					string id = response.SessionToken.Id;
					newToken = SessionCache.Instance.Renew(id, response.SessionToken.ExpiryTime);
					sessionInfo.Credentials.Authorities = response.AuthorityTokens;
					sessionInfo.Credentials.SessionToken = newToken;

					if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					{
						Platform.Log(LogLevel.Debug, "Session {0} for {1} is renewed. Valid until {2}", id,
									 sessionInfo.Credentials.UserName, newToken.ExpiryTime);

						if (originalExpiryTime == newToken.ExpiryTime)
						{
							Platform.Log(LogLevel.Warn, "Session expiry time is not changed. Is it cached?");
						}
					}
				});

			return sessionInfo;
		}


		#endregion

		#region IDisposable Members

		public void Dispose()
        {
        }

        #endregion

    }

    /// <summary>
    /// Internal session cache. Used to force logout of the sessions upon expiration.
    /// </summary>
    public class SessionCache : IDisposable
    {
        private static readonly SessionCache _instance = new SessionCache();
        private static readonly Dictionary<string, SessionInfo> _cacheSessionInfo = new Dictionary<string, SessionInfo>();
        private Timer _timer;
        private readonly object _sync = new object();

        public static SessionCache Instance
        {
            get { return _instance; }
        }

        private SessionCache()
        {
            _timer = new Timer(OnTimer, this, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private void OnTimer(object state)
        {
            if (_cacheSessionInfo.Count == 0)
                return;

            var list = new List<SessionInfo>(_cacheSessionInfo.Values);
            var active = new StringBuilder();
            active.AppendLine("Active Sessions:");
            var inactive = new StringBuilder();
            inactive.AppendLine("Inactive Sessions:");

            int activeCount = 0;
            int inactiveCount = 0;
            foreach (SessionInfo session in list)
            {
                if (session.Credentials.SessionToken.ExpiryTime < Platform.Time)
                {
                    if (Platform.Time - session.Credentials.SessionToken.ExpiryTime > TimeSpan.FromSeconds(10))
                    {
                        CleanupSession(session);
                        Platform.Log(LogLevel.Debug, "Removed expired idle session: {0} for user {1}",
                                     session.Credentials.SessionToken.Id, session.Credentials.UserName);
                    }
                    else
                    {
                        inactive.AppendLine(String.Format("\t{0}\t{1}: Expired on {2}", session.Credentials.UserName,
                                                          session.Credentials.SessionToken.Id,
                                                          session.Credentials.SessionToken.ExpiryTime));
                        inactiveCount++;
                    }
                }
                else
                {
                    activeCount++;
                    active.AppendLine(String.Format("\t{0}\t{1}: Active. Expiring on {2}", session.Credentials.UserName,
                                                    session.Credentials.SessionToken.Id, session.Credentials.SessionToken.ExpiryTime));
                }
            }
            if (activeCount > 0)
                Platform.Log(LogLevel.Debug, active.ToString());
            if (inactiveCount > 0)
                Platform.Log(LogLevel.Debug, inactive.ToString());
            
        }

        public void AddSession(string id, SessionInfo session)
        {
            lock (_sync)
            {
                _cacheSessionInfo.Add(id, session);
            }

        }

        private void OnSessionRemoved(SessionInfo session)
        {

        }

        private void CleanupSession(SessionInfo session)
        {
            lock (_sync)
            {
                using (var service = new LoginService())
                {
                    try
                    {
                        try
                        {
                            service.Logout(session.Credentials.SessionToken.Id);
                        }
                        catch(Exception ex)
                        {
                            Platform.Log(LogLevel.Warn, ex, "Unable to terminate session {0} gracefully",
                                         session.Credentials.SessionToken.Id);
                        }
                    }
                    finally
                    {
                        RemoveSession(session.Credentials.SessionToken.Id);
                    }
                }
            }
        }

        public void RemoveSession(string id)
        {
            lock (_sync)
            {
                SessionInfo session;
                if (_cacheSessionInfo.TryGetValue(id, out session))
                {
                    _cacheSessionInfo.Remove(id);
                    OnSessionRemoved(session);
                }
            }
        }

        public SessionInfo Find(string id)
        {
            lock (_sync)
            {
                if (_cacheSessionInfo.ContainsKey(id))
                    return _cacheSessionInfo[id];

                return null;
            }
        }

        public SessionToken Renew(string tokenId, DateTime time)
        {
            lock (_sync)
            {
                var sessionInfo = _cacheSessionInfo[tokenId];
                var newToken = new SessionToken(sessionInfo.Credentials.SessionToken.Id, time);
                sessionInfo.Credentials.SessionToken = newToken;

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} renewed. Will expire on {1}", sessionInfo.Credentials.SessionToken.Id, sessionInfo.Credentials.SessionToken.ExpiryTime);
                return newToken;
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}