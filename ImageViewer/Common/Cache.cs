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
using ClearCanvas.Common.Caching;

namespace ClearCanvas.ImageViewer.Common
{
    public interface ICache<T> where T : class
    {
        void Put(string key, T item);
        T Get(string key);
        void Remove(string key);
        void Clear();
    }

    public class Cache<T> : ICache<T> where T : class 
    {
        private const string _defaultRegionId = "default";

        private readonly string _cacheId;
        private readonly string _regionId;
        
        private Cache(string cacheId, string regionId)
        {
            _cacheId = cacheId;
            _regionId = regionId;
            
            Expiration = TimeSpan.FromMinutes(5);
            UseSlidingExpiration = true;
        }

        public TimeSpan Expiration { get; set; }
        public bool UseSlidingExpiration { get; set; }

        public static bool IsSupported
        {
            get { return Cache.IsSupported(); }
        }

        public static ICache<T> Create(string cacheId)
        {
            return Create(cacheId, _defaultRegionId);
        }

        public static ICache<T> Create(string cacheId, string regionId)
        {
            if (!IsSupported)
                throw new NotSupportedException("Caching is not currently supported.");

            return new Cache<T>(cacheId, regionId);
        }

        public void Put(string key, T buffer)
        {
            WithCacheClient(client => client.Put(key, buffer, new CachePutOptions(_regionId, Expiration, UseSlidingExpiration)));
        }

        public T Get(string key)
        {
            T thumb = null;
            WithCacheClient(client => thumb = client.Get(key, new CacheGetOptions(_regionId)) as T);
            return thumb;
        }

        public void Remove(string key)
        {
            WithCacheClient(client => client.Remove(key, new CacheRemoveOptions(_regionId)));
        }

        public void Clear()
        {
            WithCacheClient(client => client.ClearCache());
        }
        
        private void WithCacheClient(Action<ICacheClient> withCacheClient)
        {
            using (var client = Cache.CreateClient(_cacheId))
            {
                withCacheClient(client);
            }
        }
    }
}