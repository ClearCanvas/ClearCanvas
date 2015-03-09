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

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Implementation of <see cref="ICacheClient"/> for <see cref="AspWebCacheProvider"/>.
	/// </summary>
	internal class AspWebCacheClient : ICacheClient
	{
		private static readonly CacheGetOptions _defaultGetOptions = new CacheGetOptions();
		private static readonly CacheRemoveOptions _defaultRemoveOptions = new CacheRemoveOptions();

		private readonly AspWebCacheProvider _provider;
		private readonly string _cacheId;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="cacheId"></param>
		internal AspWebCacheClient(AspWebCacheProvider provider, string cacheId)
		{
			_provider = provider;
			_cacheId = cacheId;
		}

		#region ICacheClient Members

		/// <summary>
		/// Gets the ID of the logical cache that this client is connected to.
		/// </summary>
		public string CacheID
		{
			get { return _cacheId; }
		}

		/// <summary>
		/// Gets the object at the specified key from the cache, or null if the key does not exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public object Get(string key, CacheGetOptions options)
		{
			return _provider.Get(_cacheId, key, options);
		}

		public object Get(string key)
		{
			return _provider.Get(_cacheId, key, _defaultGetOptions);
		}

		/// <summary>
		/// Puts the specified object into the cache at the specified key,
		/// using the specified options.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="options"></param>
		public void Put(string key, object value, CachePutOptions options)
		{
			_provider.Put(_cacheId, key, value, options);
		}

		/// <summary>
		/// Removes the specified item from the cache, or does nothing if the item does not
		/// exist.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <param name="options"></param>
		public void Remove(string key, CacheRemoveOptions options)
		{
			_provider.Remove(_cacheId, key, options);
		}

		public void Remove(string key)
		{
			_provider.Remove(_cacheId, key, _defaultRemoveOptions);
		}

		/// <summary>
		/// Gets a value indicating whether the specified region exists.
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public bool RegionExists(string region)
		{
			return _provider.RegionExists(_cacheId, region);
		}

		/// <summary>
		/// Clears the entire cache region.
		/// </summary>
		public void ClearRegion(string region)
		{
			_provider.ClearRegion(_cacheId, region);
		}

		/// <summary>
		/// Clears the entire logical cache (as identified by <see cref="ICacheClient.CacheID"/>.
		/// </summary>
		public void ClearCache()
		{
			_provider.ClearCache(_cacheId);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// nothing to do
		}

		#endregion
	}
}
