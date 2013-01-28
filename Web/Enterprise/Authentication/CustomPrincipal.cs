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
using System.Reflection;
using System.Security.Principal;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    /// <summary>
    /// Custom principal
    /// </summary>
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class CustomPrincipal : IPrincipal, IUserCredentialsProvider
    {
        private IIdentity _identity;
        private LoginCredentials _credentials;

        public CustomPrincipal(IIdentity identity, LoginCredentials credentials)
        {
            _identity = identity;
            _credentials = credentials;
            WarningMessages = new List<string>();
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }

        public LoginCredentials Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        public bool IsInRole(string role)
        {
            // check that the user was granted this token
            return CollectionUtils.Contains(_credentials.Authorities,
                                            delegate(string token) { return token == role; });
        }

        public List<string> WarningMessages { get; set; }

        #endregion

        public string DisplayName
        {
            get { return _credentials.DisplayName; }
        }

        public bool WarningsDisplayed { get; set; }

        #region IUserCredentialsProvider

        public string UserName
        {
            get { return _credentials.UserName; }
        }

        public string SessionTokenId
        {
            get { return _credentials.SessionToken.Id; }
        }

        #endregion
    }
}