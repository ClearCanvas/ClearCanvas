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
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Client
{
	/// <summary>
	/// Creates and configures a TCP service channel.
	/// </summary>
	public class NetTcpConfiguration : IServiceChannelConfiguration
	{
		#region IServiceChannelConfiguration Members

		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
		{
			var binding = new NetTcpBinding
				{
					TransferMode = args.TransferMode,
					MaxReceivedMessageSize = args.TransferMode == TransferMode.Buffered
												 ? Math.Min(int.MaxValue, args.MaxReceivedMessageSize)
												 : args.MaxReceivedMessageSize,
					// allow individual string content to be same size as entire message
					ReaderQuotas =
						{
							MaxStringContentLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize),
							MaxArrayLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize)
						},
					Security =
					{
						Mode = args.AuthenticationRequired ? SecurityMode.TransportWithMessageCredential : SecurityMode.Transport,
						Message =
						{
							ClientCredentialType =
								args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None
						},

						// turn off transport security altogether
						Transport =
							{
								ClientCredentialType = TcpClientCredentialType.None
							}
					}
				};

			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			var channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding, new EndpointAddress(args.ServiceUri));
			channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
			channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

			return channelFactory;
		}

		#endregion
	}
}
