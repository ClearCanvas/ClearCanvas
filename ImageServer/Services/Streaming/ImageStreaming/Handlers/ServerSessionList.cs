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

using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class Session : Dictionary<object, object>
    {
        public new object this[object key]
        {
            get
            {
                if (this.ContainsKey(key))
                    return base[key];
                else
                    return null;
            }
        }
    }

    class ServerSessionList
    {
        private readonly Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        private readonly object _sync = new object();
        public Session this[string sessionId]
        {
            get
            {
                Session session;
                lock (_sync)
                {
                    if (!_sessions.TryGetValue(sessionId, out session))
                    {
                    
                        session = new Session();
                        _sessions.Add(sessionId, session);
                    }
                    
                }

                return session;
            }
        }
    }
}