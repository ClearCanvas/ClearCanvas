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
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Common.Mail;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{
	[ExtensionOf(typeof (CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof (IAuthenticationService))]
	public class AuthenticationService : AuthenticationServiceBase, IAuthenticationService
	{
		#region IAuthenticationService Members

		[UpdateOperation(ChangeSetAuditable = false)]
		public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.Application, "Application");
			Platform.CheckMemberIsSet(request.HostName, "HostName");
			Platform.CheckMemberIsSet(request.Password, "Password");

			return InitiateSessionHelper(
				request.UserName,
				request.Application,
				request.HostName,
				request.GetAuthorizations,
				user => user.InitiateSession(request.Application, request.HostName, request.Password, GetSessionTimeout()));
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.CurrentPassword, "CurrentPassword");
			Platform.CheckMemberIsSet(request.NewPassword, "NewPassword");

			var now = Platform.Time;
			var user = GetUser(request.UserName);

			// ensure user found, account is active and the current password is correct
			if (user == null || !user.IsActive(now) || !user.Password.Verify(request.CurrentPassword))
			{
				// no such user, account not active, or invalid password
				// the error message is deliberately vague
				throw new UserAccessDeniedException();
			}

			// check new password meets policy
			PasswordPolicy.CheckPasswordCandidate(user.AccountType, request.NewPassword, this.Settings);

			var expiryTime = Platform.Time.AddDays(this.Settings.PasswordExpiryDays);

			// change the password
			user.ChangePassword(request.NewPassword, expiryTime);

			return new ChangePasswordResponse();
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			var now = Platform.Time;
			var user = GetUser(request.UserName);

			// ensure user found, account is active and the current password is correct
			if (string.IsNullOrEmpty(user.EmailAddress))
			{
				throw new RequestValidationException(SR.MessageEmailAddressNotConfigured);
			}

			// ensure user found, account is active and the current password is correct
			if (user == null || !user.IsActive(now))
			{
				// no such user, account not active, or invalid password
				// the error message is deliberately vague
				throw new UserAccessDeniedException();
			}

			// Just use the .NET routine
			var newPassword = Membership.GeneratePassword(8, 1);

			var expiryTime = Platform.Time;

			// change the password
			user.ChangePassword(newPassword, expiryTime);

			// send email
			var settings = new PasswordResetEmailSettings();
			var mail = new OutgoingMailMessage(
				settings.FromAddress,
				user.EmailAddress,
				settings.SubjectTemplate.Replace("$USER", user.DisplayName),
				settings.BodyTemplate.Replace("$USER", user.DisplayName).Replace("$PASSWORD", newPassword),
				settings.BodyTemplate.ToLower().Contains("html"));
			mail.Enqueue(OutgoingMailClassification.Normal);

			return new ResetPasswordResponse(user.EmailAddress);
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		[ResponseCaching("GetSessionTokenCacheDirective")]
		public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");

			// get the session
			var session = GetSession(request.SessionToken);
			if (session == null)
				throw new InvalidUserSessionException();

			// determine if still valid
			session.Validate(request.UserName, this.Settings.UserSessionTimeoutEnabled);

			// renew
			if (!request.ValidateOnly)
				session.Renew(GetSessionTimeout());

			// get authority tokens if requested
			var authorizations = request.GetAuthorizations ?
			                                               	PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName) : new string[0];

			// Get DataAccess authority groups if requested
			var groups = request.GetAuthorizations
			             	? PersistenceContext.GetBroker<IAuthorityGroupBroker>().FindDataGroupsByUserName(request.UserName)
			             	: new Guid[0];

			return new ValidateSessionResponse(session.GetToken(), authorizations, groups);
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");

			// get the session and user
			var session = GetSession(request.SessionToken);
			if (session == null)
				throw new InvalidUserSessionException();

			var user = session.User;

			// validate the session, ignoring the expiry time
			session.Validate(request.UserName, false);

			// terminate it
			session.Terminate();

			// delete the session object
			var broker = PersistenceContext.GetBroker<IUserSessionBroker>();
			broker.Delete(session);

			// while we're at it, clean-up any other expired sessions for that user
			CleanExpiredSessions(user);

			return new TerminateSessionResponse();
		}

		[ReadOperation]
		[ResponseCaching("GetAuthorityTokenCacheDirective")]
		public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			//TODO: ideally we should validate the username and session token and check session expiry
			//this would ensure that only a user with a valid session could obtain his authorizations,
			//however, there is an issue in the RIS right now that prevents the session token from being passed
			// in the request... this is a WCF architecture question that needs to be resolved

			var tokens = PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName);

			return new GetAuthorizationsResponse(tokens);
		}

		#endregion

		/// <summary>
		/// Gets the session token response caching directive.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private ResponseCachingDirective GetSessionTokenCacheDirective(object request)
		{
			return new ResponseCachingDirective(
				this.Settings.SessionTokenCachingEnabled,
				TimeSpan.FromSeconds(this.Settings.SessionTokenCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		/// <summary>
		/// Gets the authority token response caching directive.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private ResponseCachingDirective GetAuthorityTokenCacheDirective(object request)
		{
			return new ResponseCachingDirective(
				this.Settings.AuthorityTokenCachingEnabled,
				TimeSpan.FromSeconds(this.Settings.AuthorityTokenCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}
	}
}