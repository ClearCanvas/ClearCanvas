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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
	[ExtensionOf(typeof (ServiceProviderExtensionPoint), Enabled = false)]
	public class EnterpriseImageServerServiceProvider : IServiceProvider
	{
		private readonly object _object = new object();

		private volatile ImageServerServiceProvider _provider;

		public object GetService(Type serviceType)
		{
			if (_provider == null)
				lock (_object)
					if (_provider == null)
						_provider = new ImageServerServiceProvider();

			return _provider.GetService(serviceType);
		}

		/// <summary>
		/// Internal implementation of ServiceProvider
		/// </summary>
		/// <remarks>
		/// Had to wrap the real service provider implementation because of a circular reference.  When the extension
		/// is created, it references the enterprise settings, which in turn are dependent on the service provider.
		/// </remarks>
		private class ImageServerServiceProvider : RemoteServiceProviderBase<ImageServerServiceAttribute>
		{
			public ImageServerServiceProvider()
				: base(GetSettings())
			{
			}

			private static RemoteServiceProviderArgs GetSettings()
			{
				return new RemoteServiceProviderArgs(
					EnterpriseImageServerServiceSettings.Default.BaseUrl,
					EnterpriseImageServerServiceSettings.Default.FailoverBaseUrl,
					EnterpriseImageServerServiceSettings.Default.ConfigurationClass,
					EnterpriseImageServerServiceSettings.Default.MaxReceivedMessageSize,
					EnterpriseImageServerServiceSettings.Default.CertificateValidationMode,
					EnterpriseImageServerServiceSettings.Default.RevocationMode,
					EnterpriseImageServerServiceSettings.Default.UserCredentialsProviderClass
					) { SendTimeoutSeconds = EnterpriseImageServerServiceSettings.Default.SendTimeoutSeconds };
			}
		}
	}
}
