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
using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Client-side advice to implement transparent failover.
	/// </summary>
	/// <remarks>
	/// If the invocation fails due to a <see cref="CommunicationException"/> (excluding
	/// <see cref="FaultException"/>s) or <see cref="TimeoutException"/>, this interceptor
	/// will attempt to obtain an alternate service channel and retry the service operation
	/// on the alternate channel.  The process is repeated until the operation succeeds or
	/// there are no more alternate channels to try.
	/// </remarks>
	class FailoverClientAdvice : IInterceptor
	{
		private readonly RemoteServiceProviderBase _serviceProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serviceProvider">Service provider instance that provides the failover channel.</param>
		public FailoverClientAdvice(RemoteServiceProviderBase serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			try
			{
				// attempt to call the default service object
				invocation.Proceed();
			}
			catch (Exception e)
			{
				// rethrow if we can't fail-over on this exception
				if (!IsFailoverApplicable(e))
					throw;

				// failover is applicable, so log failure before we attempt failover
				var channel = invocation.InvocationTarget;
				LogFailure(invocation, e, ((IClientChannel)channel).RemoteAddress);

				// attempt failover
				if (!DoFailover(invocation))
					throw;
			}
		}

		#endregion

		private bool DoFailover(IInvocation invocation)
		{
			object channel;

			// loop until we find a channel that succeeds or run out of failover channels
			while ((channel = GetFailoverChannel(invocation)) != null)
			{
				var remoteEndpoint = ((IClientChannel)channel).RemoteAddress;
				LogAttempt(invocation, remoteEndpoint);
				try
				{
					// try again using this channel, being sure to dispose of it
					using (channel as IDisposable)
					{
						var retVal = invocation.Method.Invoke(channel, invocation.Arguments);

						// success!
						invocation.ReturnValue = retVal;
						return true;
					}
				}
				catch (Exception ex)
				{
					// try next failover channel
					LogFailure(invocation, ex, remoteEndpoint);
				}
			}

			// ran out of failover channels without success
			return false;
		}

		private object GetFailoverChannel(IInvocation invocation)
		{
			return _serviceProvider.GetFailoverChannel((IClientChannel)invocation.InvocationTarget);
		}

		private static void LogFailure(IInvocation invocation, Exception e, EndpointAddress failedEndpoint)
		{
			Platform.Log(LogLevel.Error, e,
				"Service operation {0} failed on endpoint {1} with specified exception.",
				invocation.Method.Name,
				failedEndpoint.Uri);
		}

		private static void LogAttempt(IInvocation invocation, EndpointAddress attemptEndpoint)
		{
			Platform.Log(LogLevel.Error,
				"Attempting to retry service operation {0} on endpoint {1}.",
				invocation.Method.Name,
				attemptEndpoint.Uri);
		}

		private static bool IsFailoverApplicable(Exception e)
		{
			// presumably this is a communication timeout exception
			if (e is TimeoutException)
				return true;

			// communication exceptions should prompt fail over
			// but must ignore FaultException (which is a subclass of communication exception)
			if (e is CommunicationException && !(e is FaultException))
				return true;

			// any other exception should not prompt fail over
			return false;
		}
	}
}
