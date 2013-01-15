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
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IAuthorizationPolicy"/> that establishes
    /// an instance of <see cref="IPrincipal"/> that uses the <see cref="IAuthenticationService"/>
    /// to determine authorization.
    /// </summary>
    public class DefaultAuthorizationPolicy : IAuthorizationPolicy
    {
    	private readonly string _id = Guid.NewGuid().ToString();
    	private readonly string _userName;
		private readonly SessionToken _sessionToken;

		public DefaultAuthorizationPolicy(string userName, SessionToken sessionToken)
		{
			_userName = userName;
			_sessionToken = sessionToken;
		}

		public string Id
		{
			get { return _id; }
		}

		public ClaimSet Issuer
		{
			get { return ClaimSet.System; }
		}

		public bool Evaluate(EvaluationContext context, ref object state)
		{
			object obj;
			if (!context.Properties.TryGetValue("Identities", out obj))
				return false;

			IList<IIdentity> identities = obj as IList<IIdentity>;
			if (obj == null)
				return false;

			// find the matching identity
			IIdentity clientIdentity = CollectionUtils.SelectFirst(identities,
				delegate(IIdentity i) { return i.Name == _userName; });

			if (clientIdentity == null)
				return false;

			// set the principal
            context.Properties["Principal"] = DefaultPrincipal.CreatePrincipal(clientIdentity, _sessionToken);

			return true;
		}
	}
}