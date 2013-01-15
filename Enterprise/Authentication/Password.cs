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
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Enterprise.Authentication {

    // This class is based on code found here:
    // http://msdn.microsoft.com/msdnmag/issues/03/08/SecurityBriefs/

    /// <summary>
    /// Password component
    /// </summary>
	public partial class Password
	{
        private const int saltLength = 6;

        /// <summary>
        /// Creates a new <see cref="Password"/> object from the specified clear-text password string,
        /// and assigns the specified expiry time.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        public static Password CreatePassword(string clearTextPassword, DateTime? expiryTime)
        {
            Platform.CheckForEmptyString(clearTextPassword, "clearTextPassword");
            return CreatePasswordHelper(clearTextPassword, expiryTime);
        }

        /// <summary>
        /// Creates a new <see cref="Password"/> object that expires immediately.
        /// </summary>
        /// <returns></returns>
		public static Password CreateTemporaryPassword(string clearTextPassword)
        {
			return CreatePassword(clearTextPassword, Platform.Time);
        }

        /// <summary>
        /// Verifies whether the specified password string matches this <see cref="Password"/> object.
        /// Does not consider the <see cref="ExpiryTime"/>.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <returns></returns>
        public bool Verify(string clearTextPassword)
        {
			// treat null as emtpy
        	clearTextPassword = StringUtilities.EmptyIfNull(clearTextPassword);

            string h = CalculateHash(_salt, clearTextPassword);
            return _saltedHash.Equals(h);
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="ExpiryTime"/> has been exceeded
        /// with respect to the current time.
        /// </summary>
        public bool IsExpired(DateTime currentTime)
        {
			return _expiryTime.HasValue && _expiryTime < currentTime;
        }

        #region Utilities

        private static Password CreatePasswordHelper(string clearTextPassword, DateTime? expiryTime)
        {
            string salt = CreateSalt();
            string hash = CalculateHash(salt, clearTextPassword);
            return new Password(salt, hash, expiryTime);
        }

        private static string CreateSalt()
        {
            byte[] r = CreateRandomBytes(saltLength);
            return Convert.ToBase64String(r);
        }

        private static byte[] CreateRandomBytes(int len)
        {
            byte[] r = new byte[len];
            new RNGCryptoServiceProvider().GetBytes(r);
            return r;
        }

        private static string CalculateHash(string salt, string password)
        {
            byte[] data = ToByteArray(salt + password);
            byte[] hash = CalculateHash(data);
            return Convert.ToBase64String(hash);
        }

        private static byte[] CalculateHash(byte[] data)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(data);
        }

        private static byte[] ToByteArray(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        #endregion


        /// <summary>
        /// Not used.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}