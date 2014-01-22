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

using System.Security.Principal;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Implemenation of <see cref="IPrincipal"/> that determines role information
	/// for the user via the <see cref="IAuthenticationService"/>.
	/// </summary>
	public class DefaultPrincipal : IPrincipal, IUserCredentialsProvider
	{
		/// <summary>
		/// Creates an object that implements <see cref="IPrincipal"/> based on the specified
		/// identity, session token, and authorizations.
		/// </summary>
		/// <param name="identityName"></param>
		/// <param name="sessionToken"></param>
		/// <param name="authorityTokens">The set of authority tokens, if known, otherwise null.</param>
		/// <returns></returns>
		/// <remarks>
		/// If the authority tokens are not known at creation time they will be obtained on-demand
		/// via the <see cref="IAuthenticationService"/>.
		/// </remarks>
		public static DefaultPrincipal CreatePrincipal(string identityName, SessionToken sessionToken, string[] authorityTokens = null)
		{
			return new DefaultPrincipal(new GenericIdentity(identityName), sessionToken, authorityTokens);
		}

		/// <summary>
		/// Creates an object that implements <see cref="IPrincipal"/> based on the specified
		/// identity, session token, and authorizations.
		/// </summary>
		/// <param name="identity"></param>
		/// <param name="sessionToken"></param>
		/// <param name="authorityTokens">The set of authority tokens, if known, otherwise null.</param>
		/// <returns></returns>
		/// <remarks>
		/// If the authority tokens are not known at creation time they will be obtained on-demand
		/// via the <see cref="IAuthenticationService"/>.
		/// </remarks>
		public static DefaultPrincipal CreatePrincipal(IIdentity identity, SessionToken sessionToken, string[] authorityTokens = null)
		{
			return new DefaultPrincipal(identity, sessionToken, authorityTokens);
		}


		private readonly IIdentity _identity;
		private readonly SessionToken _sessionToken;
		private string[] _authorityTokens;

		private DefaultPrincipal(IIdentity identity, SessionToken sessionToken, string[] authorityTokens)
		{
			_identity = identity;
			_sessionToken = sessionToken;
			_authorityTokens = authorityTokens;
		}

		public IIdentity Identity
		{
			get { return _identity; }
		}

		public SessionToken SessionToken
		{
			get { return _sessionToken; }
		}

		public bool IsInRole(string role)
		{
			// initialize auth tokens if not yet initialized
			if (_authorityTokens == null)
				Platform.GetService((IAuthenticationService service) => _authorityTokens = service.GetAuthorizations(new GetAuthorizationsRequest(_identity.Name, _sessionToken)).AuthorityTokens);

			// check that the user was granted this token
			return CollectionUtils.Contains(_authorityTokens, token => token == role);
		}

		#region IUserCredentialsProvider Members

		string IUserCredentialsProvider.UserName
		{
			get { return _identity.Name; }
		}

		string IUserCredentialsProvider.SessionTokenId
		{
			get { return _sessionToken.Id; }
		}

		#endregion
	}
}
