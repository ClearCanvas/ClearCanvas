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
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	internal class ResponseCachingStrategies
	{
		protected static readonly string SharedCacheRegion = typeof (IApplicationConfigurationReadService).FullName;

		public class Base<TRequest, TResponse> : ResponseDataCachingStrategy<TRequest, TResponse>
		{
			internal Base(Func<TRequest, string> getCacheKey,
			            Func<TRequest, Dictionary<string, object>, object> constructResponse,
			            Func<TRequest, TResponse, CacheData> getCacheData,
			            Func<TRequest, TResponse, string> getCacheKeyToRemove
				)
				: base(SharedCacheRegion, getCacheKey, constructResponse, getCacheData, getCacheKeyToRemove)
			{
			}

			public override string GetCacheRegion()
			{
				//Need to be able to specify our own cache name so that values from the different methods can affect each other.
				return CacheRegion;
			}

			protected static string GetDocumentCacheKey(ConfigurationDocumentKey key)
			{
				return ((IDefinesCacheKey) key).GetCacheKey();
			}
		}

		public class ListSettingsGroups : Base<ListSettingsGroupsRequest, ListSettingsGroupsResponse>
		{
			internal const string CacheKey = "ListSettingsGroups";

			public ListSettingsGroups()
				: base(ignore => CacheKey,
					   GetFirstCacheDataValue, //get the response from the cache
				       (request, response) => new CacheData(CacheKey, response), //Return response from cache
				       null
					)
			{
			}
		}

		public class ImportSettingsGroup : Base<ImportSettingsGroupRequest, ImportSettingsGroupResponse>
		{
			public ImportSettingsGroup()
				: base(ignore => ListSettingsGroups.CacheKey,
				       null, null, //put nothing in the cache
					   (request, response) => ListSettingsGroups.CacheKey //remove the ListSettingsGroups response from the cache
					)
			{
			}
		}

		public class GetConfigurationDocument : Base<GetConfigurationDocumentRequest, GetConfigurationDocumentResponse>
		{
			public GetConfigurationDocument()
				: base(request => GetDocumentCacheKey(request.DocumentKey),
				       GetFirstCacheDataValue, //get the response from the cache
				       (request, response) => new CacheData(GetDocumentCacheKey(request.DocumentKey), response), //put the response in the cache
				       null
					)
			{
			}
		}

		public class SetConfigurationDocument : Base<SetConfigurationDocumentRequest, SetConfigurationDocumentResponse>
		{
			public SetConfigurationDocument()
				: base(request => GetDocumentCacheKey(request.DocumentKey),
				       null, null,
					   (request, response) => GetDocumentCacheKey(request.DocumentKey) //remove the GetConfigurationDocument request from the cache
					)
			{
			}
		}

		public class RemoveConfigurationDocument : Base<RemoveConfigurationDocumentRequest, RemoveConfigurationDocumentResponse>
		{
			public RemoveConfigurationDocument()
				: base(request => GetDocumentCacheKey(request.DocumentKey),
				       null, null,
					   (request, response) => GetDocumentCacheKey(request.DocumentKey) //remove the GetConfigurationDocument request from the cache
					)
			{
			}
		}
	}
}