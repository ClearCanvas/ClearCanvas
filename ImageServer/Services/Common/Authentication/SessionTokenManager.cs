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
using System.Configuration;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    class SessionTokenManager
    {
        private readonly Cache _cache = HttpRuntime.Cache;
        private static readonly SessionTokenManager _instance = new SessionTokenManager();
        private readonly object _sync = new object();
        static public SessionTokenManager Instance
        {
            get{ return _instance; }
        }

        private SessionTokenManager()
        {
        }

        public SessionToken FindSession(string username)
        {
            lock (_sync)
            {
                SessionToken session = _cache[username] as SessionToken;
                if (session != null)
                {
                    return session;
                }
                else
                    return null;
            }
        }

        public void AddSession(SessionToken session)
        {
            if (session == null)
                throw new Exception("Token cannot be null");
            if (session.ExpiryTime < Platform.Time)
            {
                throw new Exception(String.Format("Token {0} already expired. Cannot be updated.", session.Id));
            }

            lock (_sync)
            {
                _cache.Insert(session.Id, session, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} is added", session.Id);
            }
        }

        public void RemoveSession(SessionToken session)
        {
            if (session == null)
                throw new Exception("Token cannot be null");

            lock (_sync)
            {
                _cache.Remove(session.Id);
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} is removed", session.Id);
            }
        }

        public SessionToken UpdateSession(SessionToken token)
        {
            if (token == null)
                throw new Exception("Token cannot be null");

            if (token.ExpiryTime < Platform.Time)
            {
                throw new Exception(String.Format("Token {0} already expired. Cannot be updated.", token.Id));
            }

            lock (_sync)
            {
                RemoveSession(token);

                var newSession = new SessionToken(token.Id, Platform.Time + ServerPlatform.WebSessionTimeout);

                AddSession(newSession);

                return newSession;
            }
        }
    }
}