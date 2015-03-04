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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Creates and configures a WS-HTTP service channel for the server.
	/// </summary>
	public class ServerWsHttpConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

		public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
			var settings = new EnterpriseImageServerServiceSettings();

			// Per MSDN: Transport security is provided externally to WCF. If you are creating a self-hosted WCF application, you can bind an SSL certificate to the address using the HttpCfg.exe tool.
			// The service may appears running but client will not be able to connect. For this reason, it's best to explicitly disallow this mode.
			if (settings.SecurityMode == SecurityMode.Transport)
			{
				throw new Exception("Transport security is not supported. Please change EnterpriseImageServerServiceSettings.SecurityMode");
			}
			

			var binding = new WSHttpBinding
				{
					MaxReceivedMessageSize = args.MaxReceivedMessageSize
				};

			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			binding.ReaderQuotas.MaxStringContentLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);
			binding.ReaderQuotas.MaxArrayLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);
			binding.Security.Mode = settings.SecurityMode;
			binding.Security.Message.ClientCredentialType = args.Authenticated
			                                                	? MessageCredentialType.UserName
			                                                	: MessageCredentialType.None;


			// TransportWithMessageCredential cannot be used in conjuction with ClientCredentialType=None
			if (binding.Security.Mode == SecurityMode.TransportWithMessageCredential &&
			    binding.Security.Message.ClientCredentialType == MessageCredentialType.None)
			{
				throw new Exception(string.Format("TransportWithMessageCredential is not supported for '{0}' service. Please change EnterpriseImageServerServiceSettings.SecurityMode", args.ServiceContract.Name));
			}
			

			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");

			// expose meta-data via HTTP GET
			var metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metadataBehavior == null)
			{
				metadataBehavior = new ServiceMetadataBehavior
					{
						HttpGetEnabled = true
					};
				host.Description.Behaviors.Add(metadataBehavior);
			}

			//TODO (Rockstar): remove this after refactoring to do per-sop edits
			foreach (var endpoint in host.Description.Endpoints)
				foreach (var operation in endpoint.Contract.Operations)
					operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);

			// set up the certificate 
			if (settings.SecurityMode == SecurityMode.Message
				|| settings.SecurityMode == SecurityMode.TransportWithMessageCredential)
			{
				host.Credentials.ServiceCertificate.SetCertificate(
					StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, args.HostUri.Host);
			}
		}

		#endregion
	}


	/// <summary>
	/// Creates and configures a WS-HTTP service channel for the client.
	/// </summary>
	public class ClientWsHttpConfiguration : IServiceChannelConfiguration
	{
		#region IServiceChannelConfiguration Members

		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
		{
			var settings = new EnterpriseImageServerServiceSettings();
			var binding = new WSHttpBinding
				{
					Security =
						{
							Mode = settings.SecurityMode,
							Message =
								{
									ClientCredentialType = args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None
								}
						}
				};
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			// allow individual string content to be same size as entire message
			binding.ReaderQuotas.MaxStringContentLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);
			binding.ReaderQuotas.MaxArrayLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);

			//binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
			//binding.SendTimeout = new TimeSpan(0, 0, 10);

			var channelFactory = (ChannelFactory) Activator.CreateInstance(args.ChannelFactoryClass, binding,
			                                                                          new EndpointAddress(args.ServiceUri));
			channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
			channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

			//TODO (Rockstar): remove this after refactoring to do per-sop edits
			foreach (var operation in channelFactory.Endpoint.Contract.Operations)
				operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);

			return channelFactory;
		}

		#endregion
	}
}