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
using System.Collections.Concurrent;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Caching
{
	/// <summary>
	/// Defines an extension point for implementations of <see cref="ICacheProvider"/>.
	/// </summary>
	[ExtensionPoint]
	public class CacheProviderExtensionPoint : ExtensionPoint<ICacheProvider>
	{
	}

	/// <summary>
	/// Static class providing access to the global singleton appliction cache.
	/// </summary>
	public static class Cache
	{
		/// <summary>
		/// Maintains the singleton instance of each class of provider.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, ICacheProvider> _providers = new ConcurrentDictionary<Type, ICacheProvider>();

		/// <summary>
		/// Gets a value indicating if the cache is supported in this environment.
		/// </summary>
		/// <returns></returns>
		public static bool IsSupported()
		{
			var point = new CacheProviderExtensionPoint();
			return point.ListExtensions().Length > 0;
		}

		/// <summary>
		/// Creates a cache client for the specified logical cacheID.
		/// </summary>
		/// <remarks>
		/// This method is safe for concurrent use by multiple threads.
		/// </remarks>
		public static ICacheClient CreateClient(string cacheId)
		{
			// a cacheID is required!
			Platform.CheckForEmptyString(cacheId, "CacheID");

			// TODO a more sophisticated delegate may be required here
			// if more than one cache provider extension exists, there will need to be mechanisms for choosing
			// the appropriate provider, which may be influenced by a) the creation args,
			// and b) potentially some external configuration settings
			var provider = GetProvider(delegate { return true; });

			// create specified cache
			// this call assumes the provider.CreateClient method is thread-safe, which
			// is the responsibility of the provider!
			var client = provider.CreateClient(cacheId);

			// if debug logging enabled, wrap client in a logging decorator set at Debug level
			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				client = new CacheClientLoggingDecorator(client, LogLevel.Debug);
			}
			return client;
		}

		/// <summary>
		/// Thread-safe method to obtain singleton instance of <see cref="ICacheProvider"/>
		/// matching specified filter.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static ICacheProvider GetProvider(Predicate<ExtensionInfo> filter)
		{
			// determine the provider class
			var point = new CacheProviderExtensionPoint();
			var extension = CollectionUtils.FirstElement(point.ListExtensions(filter));
			if (extension == null)
				throw new CacheException("No cache provider extension found, or those that exist do not support all required features.");

			var providerClass = extension.ExtensionClass.Resolve();

			return _providers.GetOrAdd(providerClass, k =>
			{
				// initialize this provider and store it
				var provider = (ICacheProvider) point.CreateExtension(new ClassNameExtensionFilter(providerClass.FullName));
				provider.Initialize(new CacheProviderInitializationArgs());
				return provider;
			});
		}
	}
}
