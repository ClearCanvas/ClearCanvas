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
			WSHttpBinding binding = new WSHttpBinding();
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
			binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
			binding.Security.Mode = WebServicesSettings.Default.SecurityMode;
			binding.Security.Message.ClientCredentialType = args.Authenticated
			                                                	? MessageCredentialType.UserName
			                                                	: MessageCredentialType.None;
			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");

			// expose meta-data via HTTP GET
			ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metadataBehavior == null)
			{
				metadataBehavior = new ServiceMetadataBehavior();
				metadataBehavior.HttpGetEnabled = true;
				host.Description.Behaviors.Add(metadataBehavior);
			}

			//TODO (Rockstar): remove this after refactoring to do per-sop edits
			foreach (var endpoint in host.Description.Endpoints)
				foreach (var operation in endpoint.Contract.Operations)
					operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = args.MaxReceivedMessageSize;

			// set up the certificate 
			if (WebServicesSettings.Default.SecurityMode == SecurityMode.Message
			    || WebServicesSettings.Default.SecurityMode == SecurityMode.TransportWithMessageCredential)
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
			WSHttpBinding binding = new WSHttpBinding();
			binding.Security.Mode = WebServicesSettings.Default.SecurityMode;
			binding.Security.Message.ClientCredentialType =
				args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			// allow individual string content to be same size as entire message
			binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
			binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

			//binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
			//binding.SendTimeout = new TimeSpan(0, 0, 10);

			ChannelFactory channelFactory = (ChannelFactory) Activator.CreateInstance(args.ChannelFactoryClass, binding,
			                                                                          new EndpointAddress(args.ServiceUri));
			channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
			channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

			//TODO (Rockstar): remove this after refactoring to do per-sop edits
			foreach (var operation in channelFactory.Endpoint.Contract.Operations)
				operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = args.MaxReceivedMessageSize;

			return channelFactory;
		}

		#endregion
	}
}