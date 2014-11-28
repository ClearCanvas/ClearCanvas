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

using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common
{
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(true)]
	public interface ISystemInfoService
	{
		/// <summary>
		/// Gets derived system secret keys.
		/// </summary>
		/// <remarks>
		/// System secret keys are algorithmically derived keys based on the given input and an installation-specific
		/// secret identifier that was generated during installation. Thus, install-specific secrets keys can be
		/// derived independently by individual enterprise clients provided that they know the correct input.
		/// </remarks>
		[OperationContract]
		GetDerivedSystemSecretKeyResponse GetDerivedSystemSecretKey(GetDerivedSystemSecretKeyRequest request);
	}

	[DataContract]
	[Obfuscation(Exclude = true)]
	public class GetDerivedSystemSecretKeyRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public string Input { get; set; }
	}

	[DataContract]
	[Obfuscation(Exclude = true)]
	public class GetDerivedSystemSecretKeyResponse : DataContractBase
	{
		[DataMember]
		public string Key { get; set; }
	}
}