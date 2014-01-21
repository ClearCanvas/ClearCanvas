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

#if UNIT_TESTS


using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using Iesi.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Threading;

namespace ClearCanvas.Enterprise.Authentication.Tests
{
	[TestFixture]
	public class UserTests
	{
		private const string DefaultPassword = "password";
		private const string AlternatePassword = "clearcanvas";

		public UserTests()
		{
			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test_CreateNewUser()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			DateTime now = DateTime.Now;

			User user = User.CreateNewUser(userInfo, DefaultPassword);

			Assert.AreEqual(userInfo.UserName, user.UserName);
			Assert.AreEqual(userInfo.DisplayName, user.DisplayName);
			Assert.AreEqual(true, user.Enabled);	// enabled by default
			Assert.IsTrue(RoughlyEqual(now, user.CreationTime));
			Assert.IsNull(user.LastLoginTime);	// never logged in

			// valid range is empty (eg valid forever)
			Assert.IsNull(userInfo.ValidFrom);
			Assert.IsNull(userInfo.ValidUntil);

			// no authority groups
			Assert.IsTrue(user.AuthorityGroups.IsEmpty);

			// no sessions
			Assert.IsTrue(user.Sessions.IsEmpty);

			// password
			Assert.IsNotNull(user.Password);
			Assert.IsTrue(user.Password.Verify(DefaultPassword));	// initial password should work

			// password expires immediately
			Assert.IsTrue(RoughlyEqual(now, user.Password.ExpiryTime));
		}

		[Test]
		public void Test_CreateNewUser_WithDateRange()
		{
			DateTime now = DateTime.Now;
			DateTime tomorrow = now + TimeSpan.FromDays(1);

			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				now,
				tomorrow);

			User user = User.CreateNewUser(userInfo, DefaultPassword);

			Assert.AreEqual(userInfo.UserName, user.UserName);
			Assert.AreEqual(userInfo.DisplayName, user.DisplayName);
			Assert.AreEqual(true, user.Enabled);	// enabled by default
			Assert.IsTrue(RoughlyEqual(now, user.CreationTime));
			Assert.IsNull(user.LastLoginTime);	// never logged in

			// valid range is empty (eg valid forever)
			Assert.IsTrue(RoughlyEqual(now, user.ValidFrom));
			Assert.IsTrue(RoughlyEqual(tomorrow, user.ValidUntil));
		}

		[Test]
		public void Test_CreateNewUser_WithExplicitPasswordAndGroups()
		{
			DateTime now = DateTime.Now;
			DateTime tomorrow = now + TimeSpan.FromDays(1);
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			AuthorityGroup group1 = new AuthorityGroup();
			AuthorityGroup group2 = new AuthorityGroup();
			HashedSet<AuthorityGroup> groups = new HashedSet<AuthorityGroup>();
			groups.Add(group1);
			groups.Add(group2);

			User user = User.CreateNewUser(userInfo, Password.CreatePassword(DefaultPassword, tomorrow), groups);

			Assert.AreEqual(userInfo.UserName, user.UserName);
			Assert.AreEqual(userInfo.DisplayName, user.DisplayName);
			Assert.AreEqual(true, user.Enabled);	// enabled by default
			Assert.IsTrue(RoughlyEqual(now, user.CreationTime));
			Assert.IsNull(user.LastLoginTime);	// never logged in

			// password
			Assert.IsNotNull(user.Password);
			Assert.IsTrue(user.Password.Verify(DefaultPassword));	// initial password should work

			// password expiry time
			Assert.IsTrue(RoughlyEqual(tomorrow, user.Password.ExpiryTime));

			// groups
			Assert.AreEqual(2, user.AuthorityGroups.Count);
			Assert.IsTrue(user.AuthorityGroups.ContainsAll(groups));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_CreateNewUser_NullUserName()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				null,
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, DefaultPassword);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_CreateNewUser_EmptyUserName()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, DefaultPassword);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_CreateNewUser_IllegalUserName()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo>foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, DefaultPassword);
		}

		[Test]
		public void Test_CreateNewUser_NullDisplayName()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				null,
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, DefaultPassword);
		}

		[Test]
		public void Test_CreateNewUser_EmptyDisplayName()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, DefaultPassword);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_CreateNewUser_NullPassword()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_CreateNewUser_EmptyPassword()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				null,
				null);

			User.CreateNewUser(userInfo, "");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_CreateNewUser_InvalidDateRange()
		{
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				null,
				DateTime.Now + TimeSpan.FromDays(1),
				DateTime.Now - TimeSpan.FromDays(1));

			User.CreateNewUser(userInfo, "");
		}


		[Test]
		public void Test_User_IsActive()
		{
			DateTime now = DateTime.Now;
			DateTime oneSecondAgo = now - TimeSpan.FromSeconds(1);
			DateTime oneSecondFromNow = now + TimeSpan.FromSeconds(1);
			DateTime yesterday = now - TimeSpan.FromDays(1);
			DateTime tomorrow = now + TimeSpan.FromDays(1);

			// true cases
			User user = CreateUser(yesterday, tomorrow);
			Assert.IsTrue(user.IsActive(now));

			user = CreateUser(null, tomorrow);
			Assert.IsTrue(user.IsActive(now));

			user = CreateUser(yesterday, null);
			Assert.IsTrue(user.IsActive(now));

			user = CreateUser(null, oneSecondFromNow);
			Assert.IsTrue(user.IsActive(now));

			user = CreateUser(oneSecondAgo, null);
			Assert.IsTrue(user.IsActive(now));

			// false cases
			user = CreateUser(tomorrow, null);
			Assert.IsFalse(user.IsActive(now));

			user = CreateUser(null, yesterday);
			Assert.IsFalse(user.IsActive(now));

			user = CreateUser(tomorrow, yesterday);
			Assert.IsFalse(user.IsActive(now));	// TODO: should this throw an exception??

			user = CreateUser(now, null);
			Assert.IsFalse(user.IsActive(now));	// boundary case

			user = CreateUser(null, now);
			Assert.IsFalse(user.IsActive(now));	// boundary case

			user = CreateUser(now, now);
			Assert.IsFalse(user.IsActive(now));	// boundary case

			user = CreateUser(yesterday, tomorrow);
			user.Enabled = false;	// explicitly disable the account
			Assert.IsFalse(user.IsActive(now));
		}

		[Test]
		public void Test_ChangePassword()
		{
			User user = CreateUser(null, null);

			// password
			Assert.IsNotNull(user.Password);
			Assert.IsTrue(user.Password.Verify(DefaultPassword));	// initial password should work

			user.ChangePassword(AlternatePassword, DateTime.MaxValue);
			Assert.IsNotNull(user.Password);
			Assert.IsTrue(RoughlyEqual(DateTime.MaxValue, user.Password.ExpiryTime));
			Assert.IsFalse(user.Password.Verify(DefaultPassword));	// initial password should not work
			Assert.IsTrue(user.Password.Verify(AlternatePassword));	// new password should work
		}

		[Test]
		public void Test_ResetPassword()
		{
			User user = CreateUser(null, null);

			Assert.IsNotNull(user.Password);
			Assert.IsTrue(user.Password.Verify(DefaultPassword));	// initial password should work

			// change password 
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);
			Assert.IsNotNull(user.Password);
			Assert.IsTrue(user.Password.Verify(AlternatePassword));	// alternate password should work

			// reset to default
			user.ResetPassword(DefaultPassword);
			Assert.IsNotNull(user.Password);
			Assert.IsFalse(user.Password.Verify(AlternatePassword));	// initial password should not work
			Assert.IsTrue(user.Password.Verify(DefaultPassword));	// default password should work
			Assert.IsTrue(user.Password.IsExpired(DateTime.Now + TimeSpan.FromMilliseconds(1)));	// password should expire immediately
		}

		[Test]
		public void Test_InitiateSession_Normal()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			// no previous login time, no sessions
			Assert.IsNull(user.LastLoginTime);
			Assert.IsTrue(user.Sessions.IsEmpty);

			string app = "app";
			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			UserSession session = user.InitiateSession(app, host, AlternatePassword, timeout);

			Assert.IsNotNull(session);
			Assert.IsNotNull(session.SessionId);
			Assert.AreEqual(app, session.Application);
			Assert.AreEqual(host, session.HostName);
			Assert.AreEqual(user, session.User);
			Assert.IsTrue(RoughlyEqual(DateTime.Now, session.CreationTime));
			Assert.IsTrue(RoughlyEqual(DateTime.Now + timeout, session.ExpiryTime));
			Assert.IsFalse(session.IsExpired);
			Assert.IsFalse(session.IsTerminated);

			// last login time should be updated
			Assert.IsTrue(RoughlyEqual(DateTime.Now, user.LastLoginTime));

			// sessions collection should contain the new session
			Assert.AreEqual(1, user.Sessions.Count);
			Assert.IsTrue(user.Sessions.Contains(session));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_InitiateSession_NullApplication()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(null, host, AlternatePassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InitiateSession_EmptyApplication()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession("", host, AlternatePassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_InitiateSession_NullHost()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string app = "app";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, null, AlternatePassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InitiateSession_EmptyHost()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string app = "app";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, "", AlternatePassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_InitiateSession_NullPassword()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string app = "app";
			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, host, null, timeout);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InitiateSession_ZeroTimeout()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string app = "app";
			string host = "host";

			user.InitiateSession(app, host, AlternatePassword, TimeSpan.Zero);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InitiateSession_NegativeTimeout()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			string app = "app";
			string host = "host";

			user.InitiateSession(app, host, AlternatePassword, TimeSpan.FromHours(-1));
		}

		[Test]
		[ExpectedException(typeof(UserAccessDeniedException))]
		public void Test_InitiateSession_InActiveUser()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			// make user inactive
			user.Enabled = false;
			Assert.IsFalse(user.IsActive(DateTime.Now));

			string app = "app";
			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, host, AlternatePassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(UserAccessDeniedException))]
		public void Test_InitiateSession_IncorrectPassword()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			// make user inactive
			user.Enabled = false;
			Assert.IsFalse(user.IsActive(DateTime.Now));

			string app = "app";
			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, host, DefaultPassword, timeout);
		}

		[Test]
		[ExpectedException(typeof(PasswordExpiredException))]
		public void Test_InitiateSession_ExpiredPassword()
		{
			User user = CreateUser(null, null);

			Thread.Sleep(50);	// sleep a few msec to allow original password to expire

			Assert.IsTrue(user.Password.IsExpired(DateTime.Now));

			string app = "app";
			string host = "host";
			TimeSpan timeout = TimeSpan.FromHours(1);

			user.InitiateSession(app, host, DefaultPassword, timeout);
		}

		[Test]
		public void Test_TerminateExpiredSessions()
		{
			User user = CreateUser(null, null);

			// change password so that it won't expire
			user.ChangePassword(AlternatePassword, DateTime.MaxValue);

			// no previous login time, no sessions
			Assert.IsNull(user.LastLoginTime);
			Assert.IsTrue(user.Sessions.IsEmpty);

			string app = "app";
			string host = "host";

			// start 4 concurrent sessions, with different expiration times
			UserSession session1 = user.InitiateSession(app, host, AlternatePassword, TimeSpan.FromMilliseconds(100));
			UserSession session2 = user.InitiateSession(app, host, AlternatePassword, TimeSpan.FromMilliseconds(200));
			UserSession session3 = user.InitiateSession(app, host, AlternatePassword, TimeSpan.FromMilliseconds(300));
			UserSession session4 = user.InitiateSession(app, host, AlternatePassword, TimeSpan.FromMinutes(1));

			UserSession[] sessions = new UserSession[] { session1, session2, session3, session4 };

			// all sessions are active
			Assert.AreEqual(4, user.Sessions.Count);
			Assert.IsTrue(user.Sessions.ContainsAll(sessions));

			// before any sessions expire, this should return an empty list
			List<UserSession> expiredSessions = user.TerminateExpiredSessions();
			Assert.AreEqual(0, expiredSessions.Count);

			// wait for all but 1 session to expire
			Thread.Sleep(400);

			expiredSessions = user.TerminateExpiredSessions();

			// should have 3 expired
			Assert.AreEqual(3, expiredSessions.Count);
			Assert.IsTrue(user.Sessions.Contains(session4));	// only session 4 is alive

			// others are expired
			Assert.IsTrue(expiredSessions.Contains(session1));
			Assert.IsTrue(expiredSessions.Contains(session2));
			Assert.IsTrue(expiredSessions.Contains(session3));
		}


		private static User CreateUser(DateTime? validFrom, DateTime? validUntil)
		{
			string password = "password";
			UserInfo userInfo = new UserInfo(
				UserAccountType.U,
				"foo",
				"Mr. Foo",
				"test@clearcanvas.ca",
				validFrom,
				validUntil);

			return User.CreateNewUser(userInfo, password);
		}

		private static bool RoughlyEqual(DateTime? x, DateTime? y)
		{
			if (!x.HasValue && !y.HasValue)
				return true;

			if (!x.HasValue || !y.HasValue)
				return false;

			DateTime xx = x.Value;
			DateTime yy = y.Value;

			// for these purposes, if the times are within 1 second, that is good enough
			return Math.Abs((xx - yy).TotalSeconds) < 1;
		}
	}
}

#endif
