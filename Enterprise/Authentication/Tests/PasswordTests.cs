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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;
using System.Threading;

namespace ClearCanvas.Enterprise.Authentication.Tests
{
	[TestFixture]
	public class PasswordTests
	{
		private const string DefaultPassword = "password";
		private const string AlternatePassword = "clearcanvas";

		public PasswordTests()
		{
			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test_CreatePassword()
		{
			DateTime expiry = DateTime.Now + TimeSpan.FromDays(30);
			Password password = Password.CreatePassword(DefaultPassword, expiry);

			Assert.AreEqual(expiry, password.ExpiryTime);
			Assert.IsFalse(password.IsExpired(DateTime.Now));
            Assert.IsTrue(password.Verify(DefaultPassword));
		}

		[Test]
		public void Test_CreatePassword_NoExpiry()
		{
			Password password = Password.CreatePassword(DefaultPassword, null);

			Assert.AreEqual(null, password.ExpiryTime);
			Assert.IsFalse(password.IsExpired(DateTime.Now));

			Assert.IsFalse(password.IsExpired(DateTime.MinValue));	// never expires
			Assert.IsFalse(password.IsExpired(DateTime.MaxValue));	// never expires
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_CreatePassword_NullText()
		{
			Password.CreatePassword(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_CreatePassword_EmptyText()
		{
			Password.CreatePassword("", null);
		}

		[Test]
		public void Test_CreateTemporaryPassword()
		{
			// should expire immediately
			Password password = Password.CreateTemporaryPassword(DefaultPassword);

			Assert.IsNotNull(password.ExpiryTime);

			Thread.Sleep(10);	// allow password to expire

			Assert.Less(password.ExpiryTime.Value, DateTime.Now);
			Assert.IsTrue(password.IsExpired(DateTime.Now));
			Assert.IsTrue(password.Verify(DefaultPassword));
		}

		[Test]
		public void Test_IsExpired()
		{
			DateTime now = DateTime.Now;
			DateTime expiry = now + TimeSpan.FromMilliseconds(100);	// expire in 100 msec
			Password password = Password.CreatePassword(DefaultPassword, expiry);

			Assert.IsFalse(password.IsExpired(now));
			Thread.Sleep(150);	// sleep longer than 100 msec
			Assert.IsTrue(password.IsExpired(DateTime.Now));
		}

		[Test]
		public void Test_Verify()
		{
			Password password = Password.CreatePassword(DefaultPassword, null);
			Assert.IsFalse(password.Verify(null));
			Assert.IsFalse(password.Verify(""));
			Assert.IsFalse(password.Verify(AlternatePassword));
			Assert.IsTrue(password.Verify(DefaultPassword));

			password = Password.CreatePassword(AlternatePassword, null);
			Assert.IsFalse(password.Verify(null));
			Assert.IsFalse(password.Verify(""));
			Assert.IsFalse(password.Verify(DefaultPassword));
			Assert.IsTrue(password.Verify(AlternatePassword));

		}

		[Test]
		public void Test_Verify_Expired()
		{
			// create an expired password
			Password password = Password.CreatePassword(AlternatePassword, DateTime.MinValue);
			Assert.IsTrue(password.IsExpired(DateTime.Now));

			// Verify behaviour is not affected by the expiry time - it can succeed even
			// for an expired password
			Assert.IsFalse(password.Verify(null));
			Assert.IsFalse(password.Verify(""));
			Assert.IsFalse(password.Verify(DefaultPassword));
			Assert.IsTrue(password.Verify(AlternatePassword));
		}
	}
}

#endif
