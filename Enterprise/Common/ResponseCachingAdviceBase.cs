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
using System.ServiceModel.Channels;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Caching;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used to explicity bypass the response cache and get a "fresh" response.
    /// </summary>
    /// <remarks>The new response will still be cached. Use this class sparingly, for example when a response is known
    /// not to make sense (like a renewed session token whose expiry time is in the past!).</remarks>
    public class ResponseCacheBypassScope : IDisposable
    {
        [ThreadStatic]
        internal static int BypassCount;

        private bool _disposed;

        public ResponseCacheBypassScope()
        {
            BypassCount++;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposed)return;

            _disposed = true;
            if (BypassCount > 0)
                --BypassCount;
        }

        #endregion
    }

    /// <summary>
	/// Base class for advice's that implement response caching.
	/// </summary>
	public abstract class ResponseCachingAdviceBase
	{
		public const string HeaderName = "ResponseCachingDirective";
		public const string HeaderNamespace = "urn:http://www.clearcanvas.ca";

		#region Protected API

		/// <summary>
		/// Called from subclasses within the intercept method to process the invocation, adding caching as appropriate.
		/// </summary>
		/// <param name="invocation"></param>
		/// <param name="cacheId"></param>
		protected void ProcessInvocation(IInvocation invocation, string cacheId)
		{
			using (var cacheClient = Cache.CreateClient(cacheId))
			{
				var region = GetCacheRegion(invocation);
				var request = invocation.Arguments[0];
				var cacheKey = GetCacheKey(request);

				// check for a cached response, unless it is being explicity bypassed
				// must do this even if this operation does not support caching, because have no way of knowing
				// whether it does or does not support caching
                if (ResponseCacheBypassScope.BypassCount == 0 && GetCachedResponse(invocation, cacheClient, cacheKey, region))
					return;

				// invoke the operation
				invocation.Proceed();

				// get cache directive
				var directive = GetCachingDirective(invocation);

				// cache the response if directed
				if (directive != null && directive.EnableCaching && directive.TimeToLive > TimeSpan.Zero)
				{
					CacheResponse(invocation, cacheClient, cacheKey, region, directive);
				}
			}
		}

		/// <summary>
		/// Gets the caching directive.
		/// </summary>
		/// <param name="invocation"></param>
		/// <returns></returns>
		protected abstract ResponseCachingDirective GetCachingDirective(IInvocation invocation);

		/// <summary>
		/// Implemented by the subclass to cache the response, based on the specified caching directive.
		/// </summary>
		/// <param name="invocation"></param>
		/// <param name="cacheClient"></param>
		/// <param name="cacheKey"></param>
		/// <param name="region"></param>
		/// <param name="directive"></param>
		protected abstract void CacheResponse(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region, ResponseCachingDirective directive);

		/// <summary>
		/// Puts the invocation response in the specified cache.
		/// </summary>
		/// <param name="invocation"></param>
		/// <param name="cacheClient"></param>
		/// <param name="cacheKey"></param>
		/// <param name="region"></param>
		/// <param name="directive"></param>
		protected static void PutResponseInCache(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region, ResponseCachingDirective directive)
		{
			// bail if the directive does not tell us to cache anything
			if (directive == null || !directive.EnableCaching || directive.TimeToLive == TimeSpan.Zero)
				return;

			// if we don't have a cache key, this is an error
			if (cacheKey == null)
				throw new InvalidOperationException(
					string.Format("{0} is cacheable but the request class does not implement IDefinesCacheKey.", invocation.GetType().FullName));

			// put response in cache
			cacheClient.Put(cacheKey, invocation.ReturnValue, new CachePutOptions(region, directive.TimeToLive, false));
		}

		/// <summary>
		/// Writes the cache directive to the operation context.
		/// </summary>
		/// <param name="directive"></param>
		/// <param name="operationContext"></param>
		protected internal static void WriteCachingDirectiveHeaders(ResponseCachingDirective directive, OperationContext operationContext)
		{
			// add caching directive to WCF message headers so that we send it to the client
			var header = MessageHeader.CreateHeader(HeaderName, HeaderNamespace, directive);
			operationContext.OutgoingMessageHeaders.Add(header);
		}

		/// <summary>
		/// Attempts to read the cache directive header from the operation context, returning null if the header doesn't exist.
		/// </summary>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		protected internal static ResponseCachingDirective ReadCachingDirectiveHeaders(OperationContext operationContext)
		{
			var h = operationContext.IncomingMessageHeaders.FindHeader(HeaderName, HeaderNamespace);
			return h > -1 ? operationContext.IncomingMessageHeaders.GetHeader<ResponseCachingDirective>(h) : null;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Tries to get a previously cached response for the specified invocation, returning false if not found.
		/// </summary>
		/// <param name="invocation"></param>
		/// <param name="cacheClient"></param>
		/// <param name="cacheKey"></param>
		/// <param name="region"></param>
		/// <returns></returns>
		private static bool GetCachedResponse(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region)
		{
			if (cacheKey == null || !cacheClient.RegionExists(region))
				return false;

			// check cache
			var response = cacheClient.Get(cacheKey, new CacheGetOptions(region));
			if (response != null)
			{
				invocation.ReturnValue = response;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the cache region for the specified invocation.
		/// </summary>
		/// <param name="invocation"></param>
		/// <returns></returns>
		private static string GetCacheRegion(IInvocation invocation)
		{
			return string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);
		}


		/// <summary>
		/// Obtains the cache key for the specified request object.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static string GetCacheKey(object request)
		{
			if (request is IDefinesCacheKey)
			{
				return (request as IDefinesCacheKey).GetCacheKey();
			}
			// for now, the request must implement an interface to get a cache key
			// in future, we could add some automatic serialization to turn it into a cache key
			return null;
		}

		#endregion
	}

}
