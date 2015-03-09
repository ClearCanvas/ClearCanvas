using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Caching;

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Implementation of <see cref="ICacheProvider"/> that provides a local in-process cache based on <see cref="System.Runtime.Caching.MemoryCache"/>.
	/// </summary>
	[ExtensionOf(typeof(CacheProviderExtensionPoint))]
	public class MemoryCacheProvider : ICacheProvider
	{
		#region CacheClient class

		class CacheClient : ICacheClient
		{
			private static readonly CacheGetOptions _defaultGetOptions = new CacheGetOptions();
			private static readonly CacheRemoveOptions _defaultRemoveOptions = new CacheRemoveOptions();
			private readonly LogicalCache _logicalCache;

			internal CacheClient(LogicalCache logicalCache)
			{
				_logicalCache = logicalCache;
			}

			public string CacheID
			{
				get { return _logicalCache.Id; }
			}

			public object Get(string key, CacheGetOptions options)
			{
				Platform.CheckForNullReference(key, "key");
				Platform.CheckForNullReference(options, "options");

				return GetInternal(key, options);
			}

			public object Get(string key)
			{
				Platform.CheckForNullReference(key, "key");

				return GetInternal(key, _defaultGetOptions);
			}

			public void Put(string key, object value, CachePutOptions options)
			{
				Platform.CheckForNullReference(key, "key");
				Platform.CheckForNullReference(value, "value");
				Platform.CheckForNullReference(options, "options");

				var policy = new CacheItemPolicy();
				if (options.Sliding)
				{
					policy.SlidingExpiration = options.Expiration;
				}
				else
				{
					policy.AbsoluteExpiration = DateTimeOffset.Now + options.Expiration;
				}

				_logicalCache.GetRegion(options.Region).Set(key, value, policy);
			}

			public void Remove(string key, CacheRemoveOptions options)
			{
				Platform.CheckForNullReference(key, "key");
				Platform.CheckForNullReference(options, "options");

				RemoveInternal(key, options);
			}

			public void Remove(string key)
			{
				RemoveInternal(key, _defaultRemoveOptions);
			}

			public bool RegionExists(string region)
			{
				Platform.CheckForNullReference(region, "region");

				return _logicalCache.RegionExists(region);
			}

			public void ClearRegion(string region)
			{
				_logicalCache.ClearRegion(region);
			}

			public void ClearCache()
			{
				_logicalCache.Clear();
			}

			public void Dispose()
			{
				// nothing to do
			}

			private object GetInternal(string key, CacheGetOptions options)
			{
				return _logicalCache.GetRegion(options.Region).Get(key);
			}

			private void RemoveInternal(string key, CacheRemoveOptions options)
			{
				_logicalCache.GetRegion(options.Region).Remove(key);
			}
		}

		#endregion

		#region LogicalCache class

		class LogicalCache
		{
			private readonly string _id;
			private readonly ConcurrentDictionary<string, MemoryCache> _regionCaches;
			private MemoryCache _defaultCache;

			public LogicalCache(string id)
			{
				_id = id;

				// MemoryCache does not natively support regions, but we can emulate this
				// by using multiple instances of MemoryCache. We also have a default instance,
				// for the "null" region.

				_defaultCache = new MemoryCache(MakeCacheName(null));
				_regionCaches = new ConcurrentDictionary<string, MemoryCache>();
			}

			public string Id
			{
				get { return _id; }
			}

			public bool RegionExists(string region)
			{
				return string.IsNullOrEmpty(region) || _regionCaches.ContainsKey(region);
			}

			public MemoryCache GetRegion(string region)
			{
				if (string.IsNullOrEmpty(region))
					return (_defaultCache ?? (_defaultCache = new MemoryCache(MakeCacheName(null))));

				return _regionCaches.GetOrAdd(region, k => new MemoryCache(MakeCacheName(region)));
			}

			public void ClearRegion(string region)
			{
				if (string.IsNullOrEmpty(region))
				{
					ClearDefaultCache();
					return;
				}

				MemoryCache regionCache;
				if (_regionCaches.TryRemove(region, out regionCache))
				{
					regionCache.Dispose();
				}
			}

			public void Clear()
			{
				ClearDefaultCache();

				var regionCaches = _regionCaches.Values.ToList();
				_regionCaches.Clear();

				foreach (var cache in regionCaches)
				{
					cache.Dispose();
				}
			}

			private void ClearDefaultCache()
			{
				if (_defaultCache != null)
				{
					// because it lacks a Clear() method, and Trim(100) doesn't actually work 
					// (see http://stackoverflow.com/questions/4183270/how-to-clear-the-net-4-memorycache)
					// we need to throw it away and recreate it
					_defaultCache.Dispose();
					_defaultCache = null;
				}
			}

			private string MakeCacheName(string region)
			{
				return string.IsNullOrEmpty(region) ? _id : string.Format("{0}:{1}", _id, region);
			}
		}

		#endregion

		private static readonly ConcurrentDictionary<string, LogicalCache> _logicalCaches = new ConcurrentDictionary<string,LogicalCache>(); 


		#region ICacheProvider Members

		/// <summary>
		/// Initializes this cache provider.
		/// </summary>
		public void Initialize(CacheProviderInitializationArgs args)
		{
		}

		/// <summary>
		/// Creates a cache client for the specified logical cache ID.
		/// </summary>
		/// <remarks>
		/// The implementation of this method *must* be safe for multiple threads making concurrent calls.
		/// </remarks>
		/// <returns></returns>
		public ICacheClient CreateClient(string cacheId)
		{
			Platform.CheckForNullReference(cacheId, "cacheId");
			Platform.CheckForEmptyString(cacheId, "cacheId");

			return new CacheClient(GetOrCreateCache(cacheId));
		}

		#endregion

		#region Helpers

		private LogicalCache GetOrCreateCache(string cacheId)
		{
			return _logicalCaches.GetOrAdd(cacheId, k => new LogicalCache(cacheId));
		}

		#endregion
	}
}
