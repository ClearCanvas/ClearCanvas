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

using System.Security.Permissions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Authentication
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IImpersonationService))]
	public class ImpersonationService : AuthenticationServiceBase, IImpersonationService
	{
		#region IImpersonationService Members

		[UpdateOperation(ChangeSetAuditable = false)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Login.Impersonate)]
		public InitiateSessionResponse InitiateSession(InitiateImpersonatedSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.Application, "Application");
			Platform.CheckMemberIsSet(request.HostName, "HostName");

			var timeout = request.SessionTimeout ?? GetSessionTimeout();
			return InitiateSessionHelper(
				request.UserName,
				request.Application,
				request.HostName,
				request.GetAuthorizations,
				user => user.InitiateSessionImpersonated(request.Application, request.HostName, timeout));
		}

		#endregion
	}
}
