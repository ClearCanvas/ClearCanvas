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

using System.Text.RegularExpressions;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Authentication
{
	static class PasswordPolicy
	{
		/// <summary>
		/// Checks that a candidate password meets the enterprise-wide password policy.
		/// </summary>
		/// <param name="accountType"></param>
		/// <param name="passwordCandidate"></param>
		/// <param name="settings"></param>
		public static void CheckPasswordCandidate(UserAccountType accountType, string passwordCandidate, AuthenticationSettings settings)
		{
			if (accountType == UserAccountType.S)
			{
				if (string.IsNullOrEmpty(passwordCandidate) || passwordCandidate.Length < 8)
					throw new RequestValidationException(SR.SystemAccountValidPasswordMessage);

				return;
			}

			// password cannot be empty
			if (string.IsNullOrEmpty(passwordCandidate))
				throw new RequestValidationException(settings.ValidPasswordMessage);

			// if no regex specified, then any non-empty password is valid
			if (string.IsNullOrEmpty(settings.ValidPasswordRegex))
				return;

			// otherwise match the regex
			if (!Regex.Match(passwordCandidate, settings.ValidPasswordRegex).Success)
				throw new RequestValidationException(settings.ValidPasswordMessage);
		}
	}
}
