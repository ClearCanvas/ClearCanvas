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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	public interface ILoginFacilityProvider
	{
		IList<string> GetAvailableFacilities();

		string CurrentFacility { get; set; }
	}

	[ExtensionPoint]
	public sealed class LoginFacilityProviderExtensionPoint : ExtensionPoint<ILoginFacilityProvider>
	{
		private static ILoginFacilityProvider _provider;

		public static ILoginFacilityProvider GetProvider()
		{
			return _provider ?? (_provider = CreateProvider());
		}

		private static ILoginFacilityProvider CreateProvider()
		{
			try
			{
				var provider = (ILoginFacilityProvider) new LoginFacilityProviderExtensionPoint().CreateExtension();
				if (provider != null) return provider;
			}
			catch (NotSupportedException ex)
			{
				Platform.Log(LogLevel.Debug, ex, "No facilities provider defined.");
			}
			return new DefaultLoginFacilityProvider();
		}

		private class DefaultLoginFacilityProvider : ILoginFacilityProvider
		{
			private static readonly string[] _emptyList = new string[0];

			public IList<string> GetAvailableFacilities()
			{
				return _emptyList;
			}

			public string CurrentFacility
			{
				get { return null; }
				set { }
			}
		}
	}
}