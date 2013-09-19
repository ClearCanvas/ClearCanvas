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
using System.Linq;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.SystemAccounts
{

	[ExtensionPoint]
	public class SystemAccountLocalRegistryExtensionPoint : ExtensionPoint<ISystemAccountLocalRegistry>
	{
	}


	internal class LocalRegistryManager
	{

		public static string[] GetAccounts()
		{
			return GetRegistries().SelectMany(r => r.GetAccounts()).Select(a => a.ToLowerInvariant()).ToArray();
		}

		public static string GetAccountPassword(string account)
		{
			Platform.CheckForEmptyString(account, "account");

			account = account.ToLowerInvariant();
			var registry = GetRegistries().FirstOrDefault(r => r.GetAccounts().Contains(account));
			if(registry == null)
				throw new InvalidOperationException("Unknown account.");

			return registry.GetAccountPassword(account);
		}

		public static void SetAccountPassword(string account, string password)
		{
			Platform.CheckForEmptyString(account, "account");
			Platform.CheckForEmptyString(password, "password");

			account = account.ToLowerInvariant();
			foreach (var registry in GetRegistries().Where(r => r.GetAccounts().Contains(account)))
			{
				registry.SetAccountPassword(account, password);
			}
		}

		private static IEnumerable<ISystemAccountLocalRegistry> GetRegistries()
		{
			return new SystemAccountLocalRegistryExtensionPoint().CreateExtensions().Cast<ISystemAccountLocalRegistry>();
		}
	}
}
