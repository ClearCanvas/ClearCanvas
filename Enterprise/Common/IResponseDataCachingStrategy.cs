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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// An interface defining a custom strategy for caching responses, or data contained in the responses, for service methods.
	/// </summary>
	public interface IResponseDataCachingStrategy
	{
		/// <summary>
		/// Gets a cache region name for the data affected by the service method being called.
		/// </summary>
		/// <remarks>Return null to let the caller figure one out.</remarks>
		string GetCacheRegion();
		/// <summary>
		/// Gets the cache keys for data that may have been returned from a previous call to <see cref="GetCacheDataToPut" />.
		/// </summary>
		string[] GetCacheKeys(object request);
		
		/// <summary>
		/// Gets data to be put in the cache, given the request and response.
		/// </summary>
		CacheData[] GetCacheDataToPut(object request, object response);

		/// <summary>
		/// Gets cache keys to be removed as a result of calling the service method.
		/// </summary>
		string[] GetCacheKeysToRemove(object request, object response);
	
		/// <summary>
		/// Constructs a response from cached data that was previously returned by <see cref="GetCacheDataToPut" />.
		/// </summary>
		object ConstructResponse(object request, Dictionary<string, object> cacheData);
	}

	public abstract class ResponseDataCachingStrategy : IResponseDataCachingStrategy
	{
		public virtual string GetCacheRegion()
		{
			//Null means the caller should figure it out.
			return null;
		}

		public abstract string[] GetCacheKeys(object request);
		public abstract CacheData[] GetCacheDataToPut(object request, object response);
		public virtual string[] GetCacheKeysToRemove(object request, object response)
		{
			//A lot of strategies won't remove anything.
			return new string[0];
		}

		public abstract object ConstructResponse(object request, Dictionary<string, object> cacheData);

		public static IResponseDataCachingStrategy Get(MethodInfo method)
		{
			var cacheAttribute = AttributeUtils.GetAttribute<ResponseDataCachingStrategyAttribute>(method, true);
			if (cacheAttribute != null)
				return (IResponseDataCachingStrategy)Activator.CreateInstance(cacheAttribute.Type);

			return new DefaultResponseCachingStrategy();
		}

		protected static T[] MakeArray<T>(T item)
		{
			return new []{item};
		}

		protected static object GetFirstCacheDataValue(object request, Dictionary<string, object> cacheData)
		{
			return cacheData != null && cacheData.Count > 0 ? cacheData.Values.First() : null;
		}
	}

	public class CacheData
	{
		public readonly string CacheKey;
		public readonly object Data;

		public CacheData(string cacheKey, object data)
		{
			CacheKey = cacheKey;
			Data = data;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
	public class ResponseDataCachingStrategyAttribute : Attribute
	{
		public ResponseDataCachingStrategyAttribute(Type type)
		{
			Type = type;
		}

		public readonly Type Type;
	}

	/// <summary>
	/// Generic base class for the simplest case, where the cache data to be put or removed is a single object.
	/// </summary>
	public abstract class ResponseDataCachingStrategy<TRequest, TResponse> : ResponseDataCachingStrategy
	{
		protected readonly string CacheRegion;

		private readonly Func<TRequest, string> _getCacheKey;
		private readonly Func<TRequest, TResponse, CacheData> _getCacheDataToPut;
		private readonly Func<TRequest, TResponse, string> _getCacheKeyToRemove; 
		private readonly Func<TRequest, Dictionary<string, object>, object> _constructResponse;

		protected ResponseDataCachingStrategy(
			string cacheRegion,
			Func<TRequest, string> getCacheKey,
			Func<TRequest, Dictionary<string, object>, object> constructResponse,
			Func<TRequest, TResponse, CacheData> getCacheDataToPut = null,
			Func<TRequest, TResponse, string> getCacheKeyToRemove = null
			)
		{
			Platform.CheckForNullReference(getCacheKey, "getCacheKey");
			Platform.CheckFalse(getCacheDataToPut == null && getCacheKeyToRemove == null, "Strategy must add or remove data from the cache (or both)");

			CacheRegion = cacheRegion;
			_getCacheKey = getCacheKey;
			_getCacheDataToPut = getCacheDataToPut;
			_getCacheKeyToRemove = getCacheKeyToRemove;
			_constructResponse = constructResponse;
		}

		#region IResponseDataCachingStrategy Members

		public override string GetCacheRegion()
		{
			return CacheRegion;
		}

		public override string[] GetCacheKeys(object request)
		{
			return MakeArray(_getCacheKey((TRequest) request));
		}

		public override CacheData[] GetCacheDataToPut(object request, object response)
		{
			return _getCacheDataToPut == null ? new CacheData[0] : MakeArray(_getCacheDataToPut((TRequest)request, (TResponse)response));
		}

		public override string[] GetCacheKeysToRemove(object request, object response)
		{
			return _getCacheKeyToRemove == null ? new string[0] : MakeArray(_getCacheKeyToRemove((TRequest) request, (TResponse) response));
		}

		public override object ConstructResponse(object request, Dictionary<string, object> cacheData)
		{
			return _constructResponse == null ? null : _constructResponse((TRequest) request, cacheData);
		}

		#endregion
	}

	/// <summary>
	/// The default strategy, which uses IDefinesCacheKey on the request to determine if anything should be cached, and simply caches the response itself.
	/// </summary>
	internal class DefaultResponseCachingStrategy : ResponseDataCachingStrategy
	{
		public override string[] GetCacheKeys(object request)
		{
			var definesCacheKey = request as IDefinesCacheKey;
			if (definesCacheKey != null)
				return new[] {definesCacheKey.GetCacheKey()};

			return new string[0];
		}

		public override CacheData[] GetCacheDataToPut(object request, object response)
		{
			var definesCacheKey = request as IDefinesCacheKey;
			if (definesCacheKey != null)
			{
				//Return the response itself.
				return new[] {new CacheData((definesCacheKey).GetCacheKey(), response)};
			}

			return new CacheData[0];
		}

		public override object ConstructResponse(object request, Dictionary<string, object> cacheData)
		{
			//We cache the response itself, so if there's anything cached, that's what we return.
			return GetFirstCacheDataValue(request, cacheData);
		}
	}
}