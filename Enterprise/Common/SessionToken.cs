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
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    [DataContract]
    public class SessionToken : IEquatable<SessionToken>
    {
        private string _id;
        private DateTime _expiryTime;

        /// <summary>
        /// Creates a session token with no expiry time.
        /// </summary>
        /// <param name="id"></param>
        public SessionToken(string id)
        {
            _id = id;
        }

        /// <summary>
        /// Creates a session token with the specified expiry time.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expiryTime"></param>
        public SessionToken(string id, DateTime expiryTime)
        {
            _id = id;
            _expiryTime = expiryTime;
        }

        [DataMember]
        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        [DataMember]
        public DateTime ExpiryTime
        {
            get { return _expiryTime; }
            private set { _expiryTime = value; }
        }

        public override bool Equals(object obj)
        {
            SessionToken other = obj as SessionToken;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() ^ _expiryTime.GetHashCode();
        }

        #region IEquatable<SessionToken> Members

        public bool Equals(SessionToken other)
        {
            if (other == null)
                return false;
            return _id == other._id
                && _expiryTime == other._expiryTime;
        }

        #endregion
    }
}