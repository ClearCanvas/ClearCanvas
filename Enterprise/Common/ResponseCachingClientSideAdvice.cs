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

using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Advice to implement transparent caching on the caller side,
	/// where the callee returns an appropriate <see cref="ResponseCachingDirective"/>.
	/// </summary>
	class ResponseCachingClientSideAdvice : ResponseCachingAdviceBase, IInterceptor
	{
		#region IInterceptor Members

		void IInterceptor.Intercept(IInvocation invocation)
		{
			// establish an operation context scope, in case we need to read message headers 
			var service = invocation.InvocationTarget;
			using (new OperationContextScope((IContextChannel)service))
			{
				ProcessInvocation(invocation, "ClientSideResponseCache");
			}
		}

		#endregion

		/// <summary>
		/// Implemented by the subclass to cache the response, based on the specified caching directive.
		/// </summary>
		protected override void CacheResponse(IInvocation invocation, ICacheClient cacheClient, string cacheKey, string region, ResponseCachingDirective directive)
		{
			// put the response in the local cache
			PutResponseInCache(invocation, cacheClient, cacheKey, region, directive);
		}

		/// <summary>
		/// Gets the caching directive.
		/// </summary>
		protected override ResponseCachingDirective GetCachingDirective(IInvocation invocation)
		{
			// read cache directive from server
			return ReadCachingDirectiveHeaders(OperationContext.Current);
		}
	}
}
