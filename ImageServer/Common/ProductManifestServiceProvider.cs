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
using ClearCanvas.ImageServer.Common.ServiceModel;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductManifestChecker
    {
        private ProductVerificationResponse _lastResult;
        private const string _cacheKey = "LastResult";
        private const string _cacheRegion = "default";

        public bool VerifyManifest()
        {
            try
            {
                _lastResult = GetLastResultFromCache();
                if (_lastResult == null)
                {
                    var sp = new ProductManifestServiceProvider();
                    var service = sp.GetService(typeof(IProductVerificationService)) as IProductVerificationService;
                    if (service != null)
                    {
                        _lastResult= service.Verify(new ProductVerificationRequest());
                    }

                    if (_lastResult != null)
                    {
                        InsertLastResultIntoCache(_lastResult);
                    }
                }
            }
            catch (Exception ex)
            {
                // This is called on every page. We don't want to fill up the log.
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Error, "Error occurred when trying to communicate with manifest service :{0}", ex.Message);
            }

            return _lastResult != null && _lastResult.IsManifestValid; 
        }

        private ProductVerificationResponse GetLastResultFromCache()
        {
            using(var cache = ClearCanvas.Common.Caching.Cache.CreateClient(GetType().FullName))
            {
                return cache.Get(_cacheKey, new CacheGetOptions(_cacheRegion)) as ProductVerificationResponse;
            }
        }

        private void InsertLastResultIntoCache(ProductVerificationResponse response)
        {
            using (var cache = ClearCanvas.Common.Caching.Cache.CreateClient(GetType().FullName))
            {
                cache.Put(_cacheKey, response, new CachePutOptions(_cacheRegion, TimeSpan.FromMinutes(15), false));
            }
        }
    }

    class ProductManifestServiceProvider : RemoteServiceProviderBase<ImageServerServiceAttribute>
    {
        public ProductManifestServiceProvider()
            : base(GetSettings())
        {
        }

        protected override bool CanProvideService(Type serviceType)
        {
            // Only provide IFilesystemService

            if (serviceType != typeof(IProductVerificationService))
                return false;

            return base.CanProvideService(serviceType);
        }

        private static RemoteServiceProviderArgs GetSettings()
        {
            return new RemoteServiceProviderArgs(
                ProductManifestServiceSettings.Default.BaseUrl,
                ProductManifestServiceSettings.Default.FailoverBaseUrl,
                ProductManifestServiceSettings.Default.ConfigurationClass,
                ProductManifestServiceSettings.Default.MaxReceivedMessageSize,
                ProductManifestServiceSettings.Default.CertificateValidationMode,
                ProductManifestServiceSettings.Default.RevocationMode,
                ProductManifestServiceSettings.Default.UserCredentialsProviderClass
                );
        }
    }
}
