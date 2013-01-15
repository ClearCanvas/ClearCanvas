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
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Core.Tests
{
	/// <summary>
	/// Class that mimicks the NH entity proxy mechanism.
	/// </summary>
	static class EntityProxyFactory
	{
		internal class EntityProxyInterceptor : IInterceptor
		{
			private readonly object _rawInstance;

			internal EntityProxyInterceptor(object rawInstance)
			{
				_rawInstance = rawInstance;
			}

			public bool Intercepted { get; private set; }

			void IInterceptor.Intercept(IInvocation invocation)
			{
				// record that an invocation was intercepted
				this.Intercepted = true;

				// forward the call
				invocation.ReturnValue = invocation.Method.Invoke(_rawInstance, invocation.Arguments);
			}
		}

		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		internal static TEntity CreateProxy<TEntity>(TEntity raw, out EntityProxyInterceptor interceptor)
		{
			interceptor = new EntityProxyInterceptor(raw);
			return (TEntity)_generator.CreateClassProxy(typeof(TEntity), interceptor);
		}

		internal static TEntity CreateProxy<TEntity>(TEntity raw)
		{
			EntityProxyInterceptor interceptor;
			return CreateProxy(raw, out interceptor);
		}
	}
}
