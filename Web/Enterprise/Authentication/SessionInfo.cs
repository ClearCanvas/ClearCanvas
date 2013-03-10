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
using System.Reflection;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    [Obfuscation(Exclude=true, ApplyToMembers=false)]
    public class SessionInfo
    {
        private readonly CustomPrincipal _user;
        private bool _valid;

        public SessionInfo(CustomPrincipal user)
        {
            _user = user;
        }

        public SessionInfo(string loginId, string name, SessionToken token)
            : this(new CustomPrincipal(new CustomIdentity(loginId, name),
                                       CreateLoginCredentials(loginId, name, token)))
        {

        }

        /// <summary>
        /// Gets a value indicating whether or not the session information is valid.
        /// </summary>
        /// <remarks>
        /// Exception will be thrown if session cannot be validated in the process.
        /// </remarks>
        [Obfuscation(Exclude = true)]
        public bool Valid
        {
            get
            {
                Validate();
                return _valid;
            }
        }

        [Obfuscation(Exclude = true)]
        public CustomPrincipal User
        {
            get { return _user; }
        }

        [Obfuscation(Exclude = true)]
        public LoginCredentials Credentials
        {
            get
            {
                return _user.Credentials;
            }
        }

        private static LoginCredentials CreateLoginCredentials(string loginId, string name, SessionToken token)
        {
            var credentials = new LoginCredentials
                                  {
                                      UserName = loginId,
                                      DisplayName = name,
                                      SessionToken = token
                                  };
            return credentials;
        }

        public void Validate()
        {
            _valid = false;

            using(var service = new LoginService())
            {
                try
                {
                    var sessionInfo = service.Renew(Credentials.SessionToken.Id);
                    _user.Credentials.SessionToken = sessionInfo.Credentials.SessionToken;
                    _user.Credentials.Authorities = sessionInfo.Credentials.Authorities;
                    _user.Credentials.DataAccessAuthorityGroups = sessionInfo.Credentials.DataAccessAuthorityGroups;

                    _valid = true;
                }
                catch (Exception)
                {
                    throw new SessionValidationException();
                }
            }   
        }
    }
}