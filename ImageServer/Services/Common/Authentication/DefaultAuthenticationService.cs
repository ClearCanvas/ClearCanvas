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
using System.ServiceModel;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    [ServiceImplementsContract(typeof(IAuthenticationService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DefaultAuthenticationService : IApplicationServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
            bool ok = Membership.ValidateUser(request.UserName, request.Password);
            if (ok)
            {
                Guid tokenId = Guid.NewGuid();
                var token = new SessionToken(tokenId.ToString(), Platform.Time + ServerPlatform.WebSessionTimeout);
                string[] authority = Roles.GetRolesForUser(request.UserName);
                string displayName = request.UserName;

                var rsp = new InitiateSessionResponse(token, authority, new Guid[0], displayName,string.Empty);

                SessionTokenManager.Instance.AddSession(token);

                return rsp;
            }
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
            SessionToken session = SessionTokenManager.Instance.FindSession(request.SessionToken.Id);
            if (session!=null)
            {
                if (session.ExpiryTime < Platform.Time)
                {
                    Platform.Log(LogLevel.Error, "Session ID {0} already expired", session.Id);
                    SessionTokenManager.Instance.RemoveSession(request.SessionToken);
                    throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
                }

                if (!request.ValidateOnly)
                    session = SessionTokenManager.Instance.UpdateSession(session);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session ID {0} is updated. Valid until {1}", session.Id, session.ExpiryTime);
                return new ValidateSessionResponse(session, Roles.GetRolesForUser(session.Id));
            }

#if false
            return new ValidateSessionResponse(new SessionToken(request.SessionToken.Id,DateTime.Now.AddHours(1)),
                new[] {AuthorityTokens.DataAccess.AllStudies,
                    AuthorityTokens.DataAccess.AllPartitions,
                    "Viewer/Visible",
                    "Viewer/Clinical"});
#endif

            Platform.Log(LogLevel.Error, "Session ID {0} does not exist", request.SessionToken.Id);
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
        {
            SessionTokenManager.Instance.RemoveSession(request.SessionToken);
            return new TerminateSessionResponse();
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            if (Membership.Provider.ChangePassword(request.UserName, request.CurrentPassword, request.NewPassword))
                return new ChangePasswordResponse();
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            string[] authorities = Roles.GetRolesForUser(request.UserName);
            return new GetAuthorizationsResponse(authorities);
        }

        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}