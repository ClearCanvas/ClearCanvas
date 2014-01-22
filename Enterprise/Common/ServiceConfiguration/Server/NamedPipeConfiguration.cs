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

using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Configures a named-pipe service host.
	/// </summary>
	//todo: this code is still experimental = doesn't currently work!
	public class NamedPipeConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

		/// <summary>
		/// Configures the specified service host, according to the specified arguments.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="args"></param>
		public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
			var binding = new NetNamedPipeBinding();
			//binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
			//binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
			//binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
			//binding.Security.Mode = args.AuthenticationRequired ? NetNamedPipeSecurityMode.Transport : NetNamedPipeSecurityMode.None;
			//binding.Security.Transport.ProtectionLevel = args.AuthenticationRequired ? ProtectionLevel.EncryptAndSign : ProtectionLevel.None;

			// turn off transport security altogether
			//binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");
		}

		#endregion
	}
}
