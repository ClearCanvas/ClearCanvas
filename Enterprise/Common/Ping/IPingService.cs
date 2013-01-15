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

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Ping
{
	[DataContract]
	public class PingRequest : DataContractBase, IDefinesCacheKey //cache so we're not hitting the service constantly.
	{
		#region IDefinesCacheKey Members

		public string GetCacheKey()
		{
			return "Ping";
		}

		#endregion
	}

	[DataContract]
	public class PingResponse : DataContractBase
	{
	}

	/// <summary>
	/// Defines a "pingable" service.
	/// </summary>
	/// <remarks>
	/// This does not necessarily have to be a service in and of itself.  It can also be implemented by other services
	/// to indicate that they are alive.
	/// </remarks>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(false)]
	public interface IPingService
	{
		/// <summary>
		/// Verifies that the service is up and running.
		/// </summary>
		[OperationContract]
		PingResponse Ping(PingRequest request);
	}
}
