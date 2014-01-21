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
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Authentication
{
	public class AuthenticationServiceBase : CoreServiceLayer
	{
		private AuthenticationSettings _settings;

		protected InitiateSessionResponse InitiateSessionHelper(
			string userName,
			string application,
			string host,
			bool getAuthorizations,
			Func<User, UserSession> sessionFactory)
		{
			// check host name against white-list
			if (!CheckWhiteList(this.Settings.HostNameWhiteList, host))
				throw new UserAccessDeniedException();

			// check application name against white-list
			if (!CheckWhiteList(this.Settings.ApplicationWhiteList, application))
				throw new UserAccessDeniedException();


			// find user
			var user = GetUser(userName);
			if (user == null)
				throw new UserAccessDeniedException();

			// clean-up any expired sessions
			CleanExpiredSessions(user);

			// initiate new session
			var session = sessionFactory(user);

			// get authority tokens if requested
			var authorizations = getAuthorizations ?
				PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(userName) : new string[0];

			// Get DataAccess authority groups if requested
			var dataGroups = getAuthorizations
							 ? PersistenceContext.GetBroker<IAuthorityGroupBroker>().FindDataGroupsByUserName(userName)
							 : new Guid[0];

			return new InitiateSessionResponse(session.GetToken(), authorizations, dataGroups, user.DisplayName, user.EmailAddress);
		}

		/// <summary>
		/// Gets the user specified by the user name, or null if no such user exists.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		protected User GetUser(string userName)
		{
			var criteria = new UserSearchCriteria();
			criteria.UserName.EqualTo(userName);

			// use query caching here to make this fast (assuming the user table is not often updated)
			var users = PersistenceContext.GetBroker<IUserBroker>().Find(
				criteria, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			// bug #3701: to ensure the username match is case-sensitive, we need to compare the stored name to the supplied name
			// returns null if no match
			return CollectionUtils.SelectFirst(users, u => u.UserName == userName);
		}

		/// <summary>
		/// Gets the session identified by the specified session token, or null if no session exists.
		/// </summary>
		/// <param name="sessionToken"></param>
		/// <returns></returns>
		protected UserSession GetSession(SessionToken sessionToken)
		{
			if (String.IsNullOrEmpty(sessionToken.Id))
				return null; //we know this isn't valid, so don't go to the database.

			var where = new UserSessionSearchCriteria();
			where.SessionId.EqualTo(sessionToken.Id);

			// use query caching here to hopefully speed this up a bit
			var sessions = PersistenceContext.GetBroker<IUserSessionBroker>().Find(
				where, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			// ensure case-sensitive match, returns null if no match
			return CollectionUtils.SelectFirst(sessions, s => s.SessionId == sessionToken.Id);
		}

		/// <summary>
		/// Perform clean-up of any expired sessions that may be left over for the specified user.
		/// </summary>
		/// <param name="user"></param>
		protected void CleanExpiredSessions(User user)
		{
			var expiredSessions = user.TerminateExpiredSessions();

			// delete the session objects
			var broker = PersistenceContext.GetBroker<IUserSessionBroker>();
			foreach (var session in expiredSessions)
			{
				broker.Delete(session);
			}
		}

		/// <summary>
		/// Asserts that the specified value is contained in the specified list.
		/// </summary>
		/// <param name="commaDelimitedList"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static bool CheckWhiteList(string commaDelimitedList, string value)
		{
			if (commaDelimitedList == null)
				return true;

			value = StringUtilities.EmptyIfNull(value).Trim();

			var items = CollectionUtils.Map(
				commaDelimitedList.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
				(string s) => s.Trim());

			return items.Count == 0 || CollectionUtils.Contains(items,
				s => s.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Gets the user session timeout from settings.
		/// </summary>
		/// <returns></returns>
		protected TimeSpan GetSessionTimeout()
		{
			return TimeSpan.FromMinutes(this.Settings.UserSessionTimeoutMinutes);
		}

		/// <summary>
		/// Gets an instance of the settings.  The instance is loaded on demand
		/// once per instance of this service class.
		/// </summary>
		internal AuthenticationSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = new AuthenticationSettings();

					VerifySettings();
				}
				return _settings;
			}
		}

		/// <summary>
		/// Verify the settings are ok
		/// </summary>
		private void VerifySettings()
		{
			if (_settings.SessionTokenCachingEnabled)
			{
				// User session cache duration must be less than the session timeout duration so that client apps can renew the session.
				if (TimeSpan.FromSeconds(_settings.SessionTokenCachingTimeToLiveSeconds) >= TimeSpan.FromMinutes(_settings.UserSessionTimeoutMinutes))
				{
					string message = SR.MessageIncorrectApplicationSettings_CacheDuration;
					Platform.Log(LogLevel.Error, message);
					throw new Exception(message);
				}
			}
		}
	}
}
