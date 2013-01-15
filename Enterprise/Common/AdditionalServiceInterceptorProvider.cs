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

using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines the possible service interception sites.
	/// </summary>
	public enum ServiceInterceptSite
	{
		/// <summary>
		/// Server-side interception.
		/// </summary>
		Server,

		/// <summary>
		/// Client-side interception.
		/// </summary>
		Client
	}

	/// <summary>
	/// Defines an interface to an object that provides additional service interceptors.
	/// </summary>
	public interface IAdditionalServiceInterceptorProvider
	{
		/// <summary>
		/// Obtains a new instance of each additional interceptor.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		/// <remarks>
		/// Note that implementors have absolutely no control over where the additional interceptors
		/// are placed within the interception chain.  Therefore, additional interceptors must not
		/// rely in any way on being inserted before or after any of the default interceptors.
		/// </remarks>
		IList<IInterceptor> GetInterceptors(ServiceInterceptSite site);
	}

	/// <summary>
	/// Defines an extension point for providing additional service interceptors.
	/// </summary>
	[ExtensionPoint]
	public class AdditionalServiceInterceptorProviderExtensionPoint : ExtensionPoint<IAdditionalServiceInterceptorProvider>
	{
	}

	/// <summary>
	/// Utility class for instantiating additional service interceptors.
	/// </summary>
	public static class AdditionalServiceInterceptorProvider
	{
		/// <summary>
		/// Obtains a new instance of each additional interceptor.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public static IList<IInterceptor> GetInterceptors(ServiceInterceptSite site)
		{
			return new AdditionalServiceInterceptorProviderExtensionPoint().CreateExtensions()
				.Cast<IAdditionalServiceInterceptorProvider>()
				.SelectMany(p => p.GetInterceptors(site))
				.ToList();
		}
	}
}
