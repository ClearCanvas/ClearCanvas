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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Enterprise.Common.ServiceConfiguration.Server;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Services;
using ClearCanvas.Enterprise.Core.ServiceModel;

namespace ClearCanvas.Ris.Server
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ShredIsolation(Level = ShredIsolationLevel.None)]
	public class Application : Shred, IApplicationRoot
	{
        private ServiceMount _serviceMount;
		private bool _isStarted;

		#region IApplicationRoot Members

		void IApplicationRoot.RunApplication(string[] args)
		{
			StartUp();

			Console.WriteLine("PRESS ANY KEY TO EXIT");
			Console.Read();

			ShutDown();
		}

		#endregion

		#region Shred overrides

		public override void Start()
		{
			if(!_isStarted)
			{
				StartUp();
			}
		}

		public override void Stop()
		{
			if(_isStarted)
			{
				ShutDown();
			}
		}

		public override string GetDisplayName()
		{
			return SR.TitleRisServer;
		}

		public override string GetDescription()
		{
			return SR.MessageRisServerDescription;
		}

		#endregion

		#region Private Helpers

		private void StartUp()
		{
			Platform.Log(LogLevel.Info, "Starting application root {0}", this.GetType().FullName);

			var hostUri = new Uri(WebServicesSettings.Default.BaseUrl);
			_serviceMount = new ServiceMount(hostUri, WebServicesSettings.Default.ConfigurationClass)
			                	{
			                		EnablePerformanceLogging = WebServicesSettings.Default.EnablePerformanceLogging,
			                		MaxReceivedMessageSize = WebServicesSettings.Default.MaxReceivedMessageSize,
			                		SendExceptionDetailToClient = WebServicesSettings.Default.SendExceptionDetailToClient,
									CertificateSearchDirective = GetCertificateSearchDirective(WebServicesSettings.Default, hostUri)
			                	};


			_serviceMount.AddServices(new CoreServiceExtensionPoint());
            _serviceMount.AddServices(new ApplicationServiceExtensionPoint());


            Platform.Log(LogLevel.Info, "Starting WCF services on {0}...", WebServicesSettings.Default.BaseUrl);

            _serviceMount.OpenServices();

            Platform.Log(LogLevel.Info, "WCF Services started on {0}", WebServicesSettings.Default.BaseUrl);

			// kick NHibernate, rather than waiting for it to load on demand
			PersistentStoreRegistry.GetDefaultStore();

			_isStarted = true;
		}

		private void ShutDown()
		{
			Platform.Log(LogLevel.Info, "Stopping WCF services...");

            _serviceMount.CloseServices();

			Platform.Log(LogLevel.Info, "WCF services stopped.");

			_isStarted = false;
		}

		private static CertificateSearchDirective GetCertificateSearchDirective(WebServicesSettings settings, Uri hostUri)
		{
			var directive = string.IsNullOrEmpty(settings.CertificateCustomFindValue)
					? CertificateSearchDirective.CreateBasic(hostUri)
					: CertificateSearchDirective.CreateCustom(settings.CertificateFindType, settings.CertificateCustomFindValue);
			directive.StoreLocation = settings.CertificateStoreLocation;
			directive.StoreName = settings.CertificateStoreName;
			return directive;
		}

		#endregion
	}
}
