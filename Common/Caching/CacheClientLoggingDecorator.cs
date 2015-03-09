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
	internal class CacheClientLoggingDecorator : ICacheClient
	{
		private readonly ICacheClient _cacheClient;
		private readonly LogLevel _logLevel;

		public CacheClientLoggingDecorator(ICacheClient cacheClient, LogLevel logLevel)
		{
			_cacheClient = cacheClient;
			_logLevel = logLevel;
		}


		#region ICacheClient Members

		public string CacheID
		{
			get { return _cacheClient.CacheID; }
		}

		public object Get(string key, CacheGetOptions options)
		{
			object value = _cacheClient.Get(key, options);
			LogGet(key, value, options);
			return value;
		}

		public object Get(string key)
		{
			object value = _cacheClient.Get(key);
			LogGet(key, value, null);
			return value;
		}

		public void Put(string key, object value, CachePutOptions options)
		{
			_cacheClient.Put(key, value, options);
			LogPut(key, value, options);
		}

		public void Remove(string key, CacheRemoveOptions options)
		{
			_cacheClient.Remove(key, options);
			LogRemove(key, options);
		}

		public void Remove(string key)
		{
			_cacheClient.Remove(key);
			LogRemove(key, null);
		}

		public bool RegionExists(string region)
		{
			return _cacheClient.RegionExists(region);
		}

		public void ClearRegion(string region)
		{
			_cacheClient.ClearRegion(region);
			LogClearRegion(region);
		}

		public void ClearCache()
		{
			_cacheClient.ClearCache();
			LogClearCache();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_cacheClient.Dispose();
		}

		#endregion

		private void LogGet(string key, object value, CacheGetOptions options)
		{
			string action = value == null ? "miss" : "hit";
			LogItem(action, options != null ? options.Region : null, key, value);
		}

		private void LogPut(string key, object value, CachePutOptions options)
		{
			LogItem("put", options != null ? options.Region : null, key, value);
		}

		private void LogRemove(string key, CacheRemoveOptions options)
		{
			LogItem("remove", options != null ? options.Region : null, key, null);
		}

		private void LogClearRegion(string region)
		{
			Platform.Log(_logLevel,
			             "Cache (ID = {0}, Region = {1}): cleared region",
			             _cacheClient.CacheID,
			             string.IsNullOrEmpty(region) ? "<none>" : region);
		}

		private void LogClearCache()
		{
			Platform.Log(_logLevel,
			             "Cache (ID = {0}): cleared cache",
			             _cacheClient.CacheID);
		}

		private void LogItem(string action, string region, string key, object value)
		{
			Platform.Log(_logLevel,
				"Cache (ID = {0}, Region = {1}): {2} key = {3}, value = {4}",
				_cacheClient.CacheID,
				string.IsNullOrEmpty(region) ? "<none>" : region,
				action,
				key,
				value);
		}
	}
}
