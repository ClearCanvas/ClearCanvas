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
using System.Threading;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    /// <summary>
    /// User credentials
    /// </summary>
    [Obfuscation(Exclude=true, ApplyToMembers=true)]
    public class LoginCredentials
    {
        public string UserName;
        public string DisplayName;
        public string EmailAddress;
        public SessionToken SessionToken;
        public string[] Authorities;
        public Guid[] DataAccessAuthorityGroups;

        /// <summary>
        /// Gets the credentials for the current user
        /// </summary>
        public static LoginCredentials Current
        {
            get
            {
                if (Thread.CurrentPrincipal is CustomPrincipal)
                {
                    CustomPrincipal p = Thread.CurrentPrincipal as CustomPrincipal;
                    return p.Credentials;

                }
                return null;
            }
            set
            {
                Thread.CurrentPrincipal = new CustomPrincipal(
                    new CustomIdentity(value.UserName, value.DisplayName), value);
            }
        }
    }
}