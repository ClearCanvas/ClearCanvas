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
using System.Net.Security;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Client
{
	/// <summary>
	/// Creates and configures a named-pipe service channel.
	/// </summary>
	//todo: this code is still experimental = doesn't currently work!
	public class NamedPipeConfiguration : IServiceChannelConfiguration
	{
		#region IServiceChannelConfiguration Members

		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
		{
			var binding = new NetNamedPipeBinding();
			//binding.Security.Mode = args.AuthenticationRequired ? NetNamedPipeSecurityMode.Transport : NetNamedPipeSecurityMode.None;
			//binding.Security.Transport.ProtectionLevel = args.AuthenticationRequired ? ProtectionLevel.EncryptAndSign : ProtectionLevel.None;

			// turn off transport security altogether
			//binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;


			//binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
			//if (args.SendTimeoutSeconds > 0)
			//	binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			// allow individual string content to be same size as entire message
			//binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
			//binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

			var channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding, new EndpointAddress(args.ServiceUri));
			channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
			channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

			return channelFactory;
		}

		#endregion
	}
}
