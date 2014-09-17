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
using ClearCanvas.Common.Caching;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	///  Marks an interface as Image Server service interface (as opposed to an Enterpise service).
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public class ImageServerServiceAttribute : Attribute { }

	[ExtensionOf(typeof (ServiceProviderExtensionPoint), Enabled = false)]
	public class EnterpriseImageServerServiceProvider : IServiceProvider
	{
		readonly ICacheClient _cache = Cache.CreateClient("EnterpriseImageServerServiceProvider");
		readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(3);

		public object GetService(Type serviceType)
		{
			// Note: The object is cached for performance purpose.
			// For servicibility, cache is invalidated periodically so that changing the settings (EnterpriseImageServerServiceSettings)
			// will not require restarting all services/IIS in the farm.
			lock (_cache)
			{
				const string cacheId = "Settings";
				var provider = _cache.Get(cacheId, new CacheGetOptions("Default")) as ImageServerServiceProvider;
				if (provider==null)
				{
					provider = new ImageServerServiceProvider();
					_cache.Put(cacheId, provider, new CachePutOptions("Default", _cacheDuration, false));
				}

				return provider.GetService(serviceType);
			}
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
				var settings = new EnterpriseImageServerServiceSettings();
				return new RemoteServiceProviderArgs(settings.BaseUrl,
					settings.FailoverBaseUrl,
					settings.ConfigurationClass,
					settings.MaxReceivedMessageSize,
					settings.CertificateValidationMode,
					settings.RevocationMode,
					settings.UserCredentialsProviderClass
					) { SendTimeoutSeconds = settings.SendTimeoutSeconds };
			}
		}
	}
}
