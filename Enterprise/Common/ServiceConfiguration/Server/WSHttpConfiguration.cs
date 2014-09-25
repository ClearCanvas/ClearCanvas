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
using System.ServiceModel.Description;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Configures a WS-HTTP service host.
	/// </summary>
	public class WSHttpConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

		/// <summary>
		/// Configures the specified service host, according to the specified arguments.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="args"></param>
		public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
			var binding = new WSHttpBinding();
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
			if (args.SendTimeoutSeconds > 0)
				binding.SendTimeout = TimeSpan.FromSeconds(args.SendTimeoutSeconds);

			binding.ReaderQuotas.MaxStringContentLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);
			binding.ReaderQuotas.MaxArrayLength = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);
			binding.Security.Mode = SecurityMode.Message;
			binding.Security.Message.ClientCredentialType = args.Authenticated ?
				MessageCredentialType.UserName : MessageCredentialType.None;

			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");

			// expose meta-data via HTTP GET
			var metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metadataBehavior == null)
			{
				metadataBehavior = new ServiceMetadataBehavior();
				metadataBehavior.HttpGetEnabled = true;
				host.Description.Behaviors.Add(metadataBehavior);
			}

			//TODO (Rockstar): remove this after refactoring to do per-sop edits
			foreach (var endpoint in host.Description.Endpoints)
				foreach (var operation in endpoint.Contract.Operations)
					operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = (int)Math.Min(int.MaxValue, args.MaxReceivedMessageSize);

			// set up the certificate - required for WSHttpBinding
			host.Credentials.ServiceCertificate.SetCertificate(
				args.CertificateSearchDirective.StoreLocation, args.CertificateSearchDirective.StoreName,
				args.CertificateSearchDirective.FindType, args.CertificateSearchDirective.FindValue);
		}

		#endregion
	}
}
