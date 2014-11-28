#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	[ExtensionOf(typeof (CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof (ISystemInfoService))]
	public class SystemInfoService : CoreServiceLayer, ISystemInfoService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.System.Configuration)]
		public GetDerivedSystemSecretKeyResponse GetDerivedSystemSecretKey(GetDerivedSystemSecretKeyRequest request)
		{
			var store = SystemIdentityStore.Load();

			using (var ms = new MemoryStream())
			using (var hash = new SHA512CryptoServiceProvider2())
			{
				var input = Encoding.UTF8.GetBytes(request.Input);
				ms.Write(input, 0, input.Length);
				ms.WriteByte(0);

				var secretKey = store.SecretKey;
				ms.Write(secretKey, 0, secretKey.Length);

				ms.Position = 0;

				var result = new string(BitConverter.ToString(hash.ComputeHash(ms)).Where(c => c != '-').Select(char.ToUpperInvariant).ToArray());
				return new GetDerivedSystemSecretKeyResponse {Key = result};
			}
		}
	}
}