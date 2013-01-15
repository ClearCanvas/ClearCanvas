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
using System.ServiceModel;
using Castle.Core.Interceptor;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Ping;

namespace ClearCanvas.Enterprise.Common
{
	partial class RemoteCoreServiceProvider
	{
		#region GateAdvice class

		internal class GateAdvice : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				// ping service calls are treated as a special case
				if (invocation.InvocationTarget is IPingService)
				{
					// proceed with invocation, but treat failure as normal (log at debug level only)
					Proceed(invocation, LogLevel.Debug);
				}
				else
				{
					// if this endpoint is known to be offline, don't attempt to forward call to server,
					// just throw an exception
					if (!IsOnline)
					{
						var channel = (IClientChannel)invocation.InvocationTarget;
						var msg = string.Format("The service at endpoint {0} is unreachable.", channel.RemoteAddress.Uri);
						throw new EndpointNotFoundException(msg);
					}

					// otherwise proceed normally, treating failure as an error condition (since we expect to be online)
					Proceed(invocation, LogLevel.Error);
				}
			}

			private static void Proceed(IInvocation invocation, LogLevel failureSeverity)
			{
				try
				{
					invocation.Proceed();
					UpdateStatus(true);
				}
				catch (EndpointNotFoundException e)
				{
					var channel = (IClientChannel)invocation.InvocationTarget;
					Platform.Log(failureSeverity, e, "The service at endpoint {0} is unreachable.", channel.RemoteAddress.Uri);
					UpdateStatus(false);
					throw;
				}
			}
		}

		#endregion

		private static readonly System.Threading.Timer _pingTimer;
		private static volatile bool _isOnline = true; // assume we are online until we determine otherwise

		/// <summary>
		/// Class initializer
		/// </summary>
		static RemoteCoreServiceProvider()
		{
			_pingTimer = new System.Threading.Timer(ignore => PingServer(), null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
		}

		/// <summary>
		/// Gets a value indicating whether remote core services are currently online.
		/// </summary>
		public static bool IsOnline
		{
			get { return _isOnline; }
		}

		/// <summary>
		/// Occurs when the value of <see cref="IsOnline"/> changes.
		/// </summary>
		/// <remarks>
		/// This event is not fired on the GUI thread.
		/// </remarks>
		public static event EventHandler IsOnlineChanged;

		protected override void ApplyInterceptors(Type serviceType, IList<IInterceptor> interceptors)
		{
			base.ApplyInterceptors(serviceType, interceptors);

			// insert the GateAdvice right before the FailoverClientAdvice, or at the end of the list
			var failoverAdvice = CollectionUtils.SelectFirst(interceptors, i => i is FailoverClientAdvice);
			var k = failoverAdvice != null ? interceptors.IndexOf(failoverAdvice) : interceptors.Count;
			interceptors.Insert(k, new GateAdvice());
		}

		#region Helpers

		private static void UpdateStatus(bool online)
		{
			if (online != _isOnline)
			{
				_isOnline = online;
				EventsHelper.Fire(IsOnlineChanged, null, EventArgs.Empty);
			}
		}

		private static void PingServer()
		{
			Platform.GetService<IPingService>(
				service =>
				{
					try
					{
						service.Ping(new PingRequest());
					}
					catch (Exception)
					{
						// exception was logged by the GateAdvice
					}
				});
		}

		#endregion

	}
}
