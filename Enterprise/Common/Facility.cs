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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common
{
	public interface ILoginFacilityProvider
	{
		IList<FacilityInfo> GetAvailableFacilities();

		FacilityInfo CurrentFacility { get; set; }
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
			private static readonly FacilityInfo[] _emptyList = new FacilityInfo[0];

			public IList<FacilityInfo> GetAvailableFacilities()
			{
				return _emptyList;
			}

			public FacilityInfo CurrentFacility
			{
				get { return null; }
				set { }
			}
		}
	}

	[DataContract]
	public class FacilityInfo : DataContractBase, IEquatable<FacilityInfo>
	{
		public FacilityInfo() {}

		public FacilityInfo(string code, string name)
		{
			Code = code;
			Name = name;
		}

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		public bool Equals(FacilityInfo facilitySummary)
		{
			return facilitySummary != null && Equals(Code, facilitySummary.Code);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as FacilityInfo);
		}

		public override int GetHashCode()
		{
			return -0x6EBEF583 ^ (string.IsNullOrEmpty(Code) ? 0 : Code.GetHashCode());
		}
	}
}